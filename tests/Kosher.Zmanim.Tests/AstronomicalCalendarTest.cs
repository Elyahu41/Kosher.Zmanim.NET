/*
 * Zmanim Java API
 * Copyright © 2004-2026 Eliyahu Hershfeld
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; version 2.1 of the License.
 *
 * This library is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not, write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA,
 * or connect to: https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html
 */
using Kosher.Zmanim;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;
using static Kosher.Zmanim.Tests.JUnitAssert;

namespace Kosher.Zmanim.Tests;

/// <summary>
/// Regression and behavioral coverage for <see cref="AstronomicalCalendar"/>. The time values are the library's own current
/// output for the fixture (Lakewood, NJ on 2017-10-17); the behavioral tests exercise the polar no-event <c>null</c>
/// contract, calculator swapping, and the twilight/offset relationships. (Complements the existing
/// <see cref="AstronomicalCalendarRegressionTest"/>, which covers local-mean-time/DST edge cases.)
/// </summary>
public class AstronomicalCalendarTest
{
    private static readonly DateOnly FixtureDate = new DateOnly(2017, 10, 17);

    private static AstronomicalCalendar FixtureCalendar()
    {
        var calendar = new AstronomicalCalendar(TestLocations.Lakewood());
        calendar.LocalDate = FixtureDate;
        return calendar;
    }

    private static void AssertInstant(string label, string expectedIso, DateTimeOffset? actual) =>
        TestTime.AssertInstant(label, expectedIso, actual);

    [Fact]
    public void SunriseAndSunset()
    {
        AstronomicalCalendar calendar = FixtureCalendar();
        AssertInstant("sunrise", "2017-10-17T11:09:11.571783718Z", calendar.GetSunrise());
        AssertInstant("sunset", "2017-10-17T22:14:38.994862349Z", calendar.GetSunset());
        AssertInstant("seaLevelSunrise", "2017-10-17T11:09:51.403184642Z", calendar.GetSeaLevelSunrise());
        AssertInstant("seaLevelSunset", "2017-10-17T22:13:59.201432122Z", calendar.GetSeaLevelSunset());
    }

    [Fact]
    public void TransitAndSolarMidnight()
    {
        AstronomicalCalendar calendar = FixtureCalendar();
        AssertInstant("sunTransit", "2017-10-17T16:42:12.781249470Z", calendar.GetSunTransit());
        AssertInstant("solarMidnight", "2017-10-18T04:42:06.833038724Z", calendar.GetSolarMidnight());
    }

    [Fact]
    public void Twilights()
    {
        AstronomicalCalendar calendar = FixtureCalendar();
        AssertInstant("beginCivil", "2017-10-17T10:42:27.439221886Z", calendar.GetBeginCivilTwilight());
        AssertInstant("beginNautical", "2017-10-17T10:10:57.242471901Z", calendar.GetBeginNauticalTwilight());
        AssertInstant("beginAstronomical", "2017-10-17T09:39:33.523030241Z", calendar.GetBeginAstronomicalTwilight());
        AssertInstant("endCivil", "2017-10-17T22:41:21.435143220Z", calendar.GetEndCivilTwilight());
        AssertInstant("endNautical", "2017-10-17T23:12:49.151356447Z", calendar.GetEndNauticalTwilight());
        AssertInstant("endAstronomical", "2017-10-17T23:44:09.707296295Z", calendar.GetEndAstronomicalTwilight());
    }

    [Fact]
    public void TemporalHour()
    {
        TestTime.AssertDuration("temporalHour", "PT55M20.649853956S", FixtureCalendar().GetTemporalHour());
    }

    [Fact]
    public void OffsetByDegreesMatchesNamedTwilight()
    {
        AstronomicalCalendar calendar = FixtureCalendar();
        AssertEquals(calendar.GetBeginCivilTwilight(), calendar.GetSunriseOffsetByDegrees(96.0));
        AssertEquals(calendar.GetEndCivilTwilight(), calendar.GetSunsetOffsetByDegrees(96.0));
    }

    [Fact]
    public void PolarNoEventReturnsNull()
    {
        var calendar = new AstronomicalCalendar(TestLocations.Norway(TestLocations.Utc));
        calendar.LocalDate = new DateOnly(2017, 6, 21);
        AssertNull("polar sunrise", calendar.GetSunrise());
        AssertNull("polar sunset", calendar.GetSunset());
    }

    [Fact]
    public void SwappingCalculatorChangesResult()
    {
        AstronomicalCalendar noaa = FixtureCalendar();
        DateTimeOffset? noaaSunset = noaa.GetSunset();
        AstronomicalCalendar sunTimes = FixtureCalendar();
#pragma warning disable CS0618 // SunTimesCalculator is obsolete
        sunTimes.AstronomicalCalculator = new SunTimesCalculator();
#pragma warning restore CS0618
        DateTimeOffset? sunTimesSunset = sunTimes.GetSunset();
        AssertNotEquals(noaaSunset, sunTimesSunset);
    }

    /// <summary>
    /// Near the date line, a polar sunrise must be on the calendar date it was requested for, before that day's sunset.
    /// </summary>
    [Fact]
    public void PolarSunriseNearTheDateLineIsOnItsOwnDate()
    {
        TimeZoneInfo anadyr = TestTime.Zone("Asia/Anadyr");
        var date = new DateOnly(2021, 1, 10);
        AstronomicalCalculator[] calculators = { new NOAACalculator(), new MeeusCalculator(), new SPACalculator() };
        foreach (AstronomicalCalculator calculator in calculators)
        {
            var calendar = new AstronomicalCalendar(new GeoLocation("Cape Schmidt", 68.88, -179.37, anadyr));
            calendar.LocalDate = date;
            calendar.AstronomicalCalculator = calculator;
            DateTimeOffset sunrise = TimeZoneInfo.ConvertTime(calendar.GetSeaLevelSunrise()!.Value, anadyr);
            DateTimeOffset sunset = TimeZoneInfo.ConvertTime(calendar.GetSeaLevelSunset()!.Value, anadyr);
            AssertEquals(calculator.CalculatorName, date, DateOnly.FromDateTime(sunrise.DateTime));
            AssertTrue(calculator.CalculatorName, sunrise < sunset);
        }
    }

    /// <summary>
    /// Asserts that <c>event</c> at the given place, in a zone a whole number of <c>zoneHours</c> from UTC, comes out
    /// within a second of <c>expectedMillis</c>. A result on the wrong day is 24 hours off.
    /// </summary>
    private static void AssertEventNear(string label, double latitude, double longitude, int zoneHours, DateOnly date,
            Func<AstronomicalCalendar, DateTimeOffset?> @event, long expectedMillis)
    {
        var calendar = new AstronomicalCalendar(new GeoLocation(label, latitude, longitude, TestTime.FixedOffset(zoneHours)));
        calendar.LocalDate = date;
        DateTimeOffset? actual = @event(calendar);
        DateTimeOffset expected = DateTimeOffset.FromUnixTimeMilliseconds(expectedMillis);
        AssertTrue(label + ": " + actual + " is not near " + expected,
                actual.HasValue && (actual.Value - expected).Duration() < TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SunriseAfterSixPmLocalMeanTimeMovesBackADay()
    {
        AssertEventNear("sunrise after 18:00", -70, 35.235, 2, new DateOnly(2021, 1, 25),
                c => c.GetSeaLevelSunrise(), 1611527037358L); // 2021-01-25T00:23:57+02:00
    }

    [Fact]
    public void SunsetBeforeSixAmLocalMeanTimeMovesForwardADay()
    {
        AssertEventNear("sunset before 06:00", -70, -179.37, -12, new DateOnly(2021, 1, 23),
                c => c.GetSeaLevelSunset(), 1611488915349L); // 2021-01-23T23:48:35-12:00
    }

    [Fact]
    public void SunsetAfterThirtyHoursLocalMeanTimeMovesBackADay()
    {
        AssertEventNear("sunset after 30:00", 68.88, 179, 12, new DateOnly(2021, 12, 2),
                c => c.GetSeaLevelSunset(), 1638403159563L); // 2021-12-02T11:59:19+12:00
    }

    [Fact]
    public void SolarMidnightBeforeNoonLocalMeanTimeMovesForwardADay()
    {
        AssertEventNear("midnight before 12:00", -70, -179.37, -12, new DateOnly(2021, 1, 1),
                c => c.GetSolarMidnight(), 1609588897446L); // 2021-01-02T00:01:37-12:00
    }

    [Fact]
    public void SolarNoonBeforeMidnightLocalMeanTimeMovesForwardADay()
    {
        AssertEventNear("noon before 00:00", -70, -179.37, -12, new DateOnly(2021, 1, 1),
                c => c.GetSunTransit(), 1609545683526L); // 2021-01-01T12:01:23-12:00
    }

    [Fact]
    public void SolarNoonAfterTwentyFourHoursLocalMeanTimeMovesBackADay()
    {
        AssertEventNear("noon after 24:00", -70, 179, 12, new DateOnly(2021, 9, 14),
                c => c.GetSunTransit(), 1631577581205L); // 2021-09-14T11:59:41+12:00
    }
}
