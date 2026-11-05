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
/// Coverage for the location- and date-gated zmanim of <see cref="ComprehensiveZmanimCalendar"/> that only produce a value
/// under specific conditions: the <em>chametz</em> times (only on Erev Pesach, 14 Nissan), the <em>Kidush Levana</em> /
/// molad times (only within their lunar-month windows), and the <em>polar</em> zmanim (only inside the Arctic/Antarctic
/// circle during polar day/night, when there is no ordinary sunrise or sunset). The broader
/// <see cref="ComprehensiveZmanimCalendarTest"/> net pins these as <c>null</c> for its temperate ordinary-day fixture; this
/// test verifies they return the correct times under the conditions they actually apply.
/// </summary>
public class SpecialDayZmanimTest
{

    private ComprehensiveZmanimCalendar CalendarFor(DateOnly date)
    {
        ComprehensiveZmanimCalendar calendar = new ComprehensiveZmanimCalendar(TestLocations.Lakewood());
        calendar.LocalDate = date;
        return calendar;
    }

    private static void AssertInstant(string label, string expectedIso, DateTimeOffset? actual)
    {
        TestTime.AssertInstant(label, expectedIso, actual);
    }

    /// <summary>
    /// 14 Nissan 5777 (Erev Pesach) falls on 2017-04-10.
    /// </summary>
    private static readonly DateOnly ErevPesach = new DateOnly(2017, 4, 10);

    [Fact]
    public void ChametzTimesOnErevPesach()
    {
        ComprehensiveZmanimCalendar calendar = CalendarFor(ErevPesach);

        // All ten sof zman achilas/biur chametz opinions must produce a time on Erev Pesach.
        AssertNotNull(calendar.GetSofZmanAchilasChametzGRA());
        AssertNotNull(calendar.GetSofZmanAchilasChametzMGA72Minutes());
        AssertNotNull(calendar.GetSofZmanAchilasChametzMGA72MinutesZmanis());
        AssertNotNull(calendar.GetSofZmanAchilasChametzMGA16Point1Degrees());
        AssertNotNull(calendar.GetSofZmanAchilasChametzBaalHatanya());
        AssertNotNull(calendar.GetSofZmanBiurChametzGRA());
        AssertNotNull(calendar.GetSofZmanBiurChametzMGA72Minutes());
        AssertNotNull(calendar.GetSofZmanBiurChametzMGA72MinutesZmanis());
        AssertNotNull(calendar.GetSofZmanBiurChametzMGA16Point1Degrees());
        AssertNotNull(calendar.GetSofZmanBiurChametzBaalHatanya());

        // Regression on the GRA opinions; biur (end of the 5th hour) is one shaah zmanis after achilas (end of the 4th).
        AssertInstant("achilasGRA", "2017-04-10T14:47:45.675920561Z", calendar.GetSofZmanAchilasChametzGRA());
        AssertInstant("biurGRA", "2017-04-10T15:53:07.762314340Z", calendar.GetSofZmanBiurChametzGRA());
    }

    [Fact]
    public void ChametzTimesAreNullOnAnOrdinaryDay()
    {
        ComprehensiveZmanimCalendar calendar = CalendarFor(new DateOnly(2017, 10, 17));
        AssertNull(calendar.GetSofZmanAchilasChametzGRA());
        AssertNull(calendar.GetSofZmanBiurChametzGRA());
    }

    [Fact]
    public void ZmanMoladAndTchilasKidushLevana()
    {
        // The molad of Cheshvan 5778 (the moment itself falls on 2017-10-20).
        AssertInstant("zmanMolad", "2017-10-20T09:52:00.170666666Z", CalendarFor(new DateOnly(2017, 10, 20)).GetZmanMolad());

        // Earliest Kidush Levana: 3 days and 7 days after the molad.
        AssertInstant("tchilas3Days", "2017-10-23T09:52:00.170666666Z",
                CalendarFor(new DateOnly(2017, 10, 23)).GetTchilasZmanKidushLevana3Days());
        AssertInstant("tchilas7Days", "2017-10-27T09:52:00.170666666Z",
                CalendarFor(new DateOnly(2017, 10, 27)).GetTchilasZmanKidushLevana7Days());
    }

    [Fact]
    public void SofZmanKidushLevana()
    {
        // Latest Kidush Levana for the month whose molad was in late September 2017 (sayable through 2017-10-05).
        AssertInstant("sof15Days", "2017-10-05T21:07:56.837333333Z",
                CalendarFor(new DateOnly(2017, 10, 5)).GetSofZmanKidushLevana15Days());
        AssertInstant("sofBetweenMoldos", "2017-10-05T15:29:58.503333333Z",
                CalendarFor(new DateOnly(2017, 10, 5)).GetSofZmanKidushLevanaBetweenMoldos());
    }

    [Fact]
    public void KidushLevanaIsNullOutsideItsWindow()
    {
        // 2017-10-17 is past the previous month's sof zman and before the next month's tchilas zman.
        ComprehensiveZmanimCalendar calendar = CalendarFor(new DateOnly(2017, 10, 17));
        AssertNull(calendar.GetZmanMolad());
        AssertNull(calendar.GetTchilasZmanKidushLevana3Days());
        AssertNull(calendar.GetSofZmanKidushLevana15Days());
    }

    private ComprehensiveZmanimCalendar NorwayCalendarFor(DateOnly date)
    {
        ComprehensiveZmanimCalendar calendar =
                new ComprehensiveZmanimCalendar(TestLocations.Norway(TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo")));
        calendar.LocalDate = date;
        return calendar;
    }

    /// <summary>
    /// 2017-06-21 (summer solstice) is inside the polar day at the Norway fixture: no ordinary sunrise or sunset.
    /// </summary>
    private static readonly DateOnly NorwayPolarDay = new DateOnly(2017, 6, 21);

    [Fact]
    public void PolarZmanimDuringPolarDay()
    {
        ComprehensiveZmanimCalendar calendar = NorwayCalendarFor(NorwayPolarDay);

        // Precondition: there is genuinely no sunrise/sunset here, which is what activates the polar alternatives.
        AssertNull(calendar.GetSunrise());
        AssertNull(calendar.GetSunset());

        // Ben Ish Chai: use the sun's crossing of due east (90) / due west (270) as the "sunrise"/"sunset".
        AssertInstant("polarSunriseBenIshChai", "2017-06-21T06:01:13.021342105Z", calendar.GetPolarSunriseBenIshChai());
        AssertInstant("polarSunsetBenIshChai", "2017-06-21T16:49:17.587975180Z", calendar.GetPolarSunsetBenIshChai());
        AssertInstant("polarPlagBenIshChai", "2017-06-21T15:41:47.112284232Z", calendar.GetPolarPlagHaminchaBenIshChai());

        // Teshuvos Vehanhagos: during polar summer the start of day is chatzos halayla.
        AssertInstant("polarStartOfDayTeshuvos", "2017-06-21T23:25:21.765329979Z",
                calendar.GetPolarStartOfDayTeshuvosVehanhagos());
        AssertInstant("polarPlagTeshuvos", "2017-06-21T20:55:21.765329979Z",
                calendar.GetPolarPlagHaminchaTeshuvosVehanhagos());
    }

    [Fact]
    public void PolarZmanimAreNullWhereSunRisesAndSets()
    {
        // At the temperate Lakewood fixture the sun rises and sets normally, so the polar alternatives do not apply.
        ComprehensiveZmanimCalendar calendar = CalendarFor(new DateOnly(2017, 10, 17));
        AssertNull(calendar.GetPolarSunriseBenIshChai());
        AssertNull(calendar.GetPolarSunsetBenIshChai());
        AssertNull(calendar.GetPolarPlagHaminchaBenIshChai());
        AssertNull(calendar.GetPolarStartOfDayTeshuvosVehanhagos());
        AssertNull(calendar.GetPolarPlagHaminchaTeshuvosVehanhagos());
    }
}
