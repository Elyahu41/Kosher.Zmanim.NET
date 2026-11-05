using Kosher.Zmanim.Tests.Util;

using Kosher.Zmanim;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;
using static Kosher.Zmanim.Tests.JUnitAssert;

namespace Kosher.Zmanim.Tests;

public class AstronomicalCalendarRegressionTest
{

    [Fact]
    public void LocalMeanTimeForLateEveningStaysOnRequestedLocalDate()
    {
        GeoLocation geoLocation = new GeoLocation("Chicago", 41.8781, -87.6298, TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
        AstronomicalCalendar astronomicalCalendar = new AstronomicalCalendar(geoLocation);
        DateOnly localDate = new DateOnly(2026, 6, 1);
        astronomicalCalendar.LocalDate = localDate;

        DateTimeOffset actual = astronomicalCalendar.GetLocalMeanTime(new TimeOnly(23, 0))!.Value;
        DateTime civilLocal = localDate.ToDateTime(new TimeOnly(23, 0));
        DateTimeOffset civilTime = new DateTimeOffset(civilLocal, geoLocation.TimeZone.GetUtcOffset(civilLocal));
        DateTimeOffset expected = civilTime.AddMilliseconds(-geoLocation.GetLocalMeanTimeOffset(civilTime));

        AssertEquals(expected, actual);
        AssertEquals(localDate, DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(actual, geoLocation.TimeZone).DateTime));
    }

    [Fact]
    public void LocalMeanTimeUsesOffsetAtRequestedTimeOnDstTransitionDate()
    {
        GeoLocation geoLocation = new GeoLocation("New York", 40.7128, -74.0060, TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
        AstronomicalCalendar astronomicalCalendar = new AstronomicalCalendar(geoLocation);
        DateOnly localDate = new DateOnly(2026, 3, 8);
        astronomicalCalendar.LocalDate = localDate;

        DateTimeOffset actual = astronomicalCalendar.GetLocalMeanTime(new TimeOnly(12, 0))!.Value;
        DateTime civilLocal = localDate.ToDateTime(new TimeOnly(12, 0));
        DateTimeOffset civilTime = new DateTimeOffset(civilLocal, geoLocation.TimeZone.GetUtcOffset(civilLocal));
        DateTimeOffset expected = civilTime.AddMilliseconds(-geoLocation.GetLocalMeanTimeOffset(civilTime));

        AssertEquals(expected, actual);
        AssertEquals(new TimeOnly(12, 56, 1, 440), TimeOnly.FromDateTime(TimeZoneInfo.ConvertTime(actual, geoLocation.TimeZone).DateTime));
    }
}
