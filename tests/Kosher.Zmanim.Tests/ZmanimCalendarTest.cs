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
/// Regression and behavioral coverage for <see cref="ZmanimCalendar"/>, the standard-zmanim entry point. Every zero-argument
/// zman getter is pinned against the library's own current output for the fixture (Lakewood, NJ on 2017-10-17), and the
/// elevation toggle is verified to actually move the sunrise-based zmanim. (The full opinion-heavy surface is covered by
/// <see cref="ComprehensiveZmanimCalendarTest"/>.)
/// </summary>
public class ZmanimCalendarTest
{

    private static readonly DateOnly FixtureDate = new DateOnly(2017, 10, 17);

    private ZmanimCalendar FixtureCalendar()
    {
        ZmanimCalendar calendar = new ZmanimCalendar(TestLocations.Lakewood());
        calendar.LocalDate = FixtureDate;
        return calendar;
    }

    private static void AssertInstant(string label, string expectedIso, DateTimeOffset? actual)
    {
        TestTime.AssertInstant(label, expectedIso, actual);
    }

    [Fact]
    public void AlosAndMisheyakir()
    {
        ZmanimCalendar calendar = FixtureCalendar();
        AssertInstant("alos16.1", "2017-10-17T09:49:30.219906135Z", calendar.GetAlos16Point1Degrees());
        AssertInstant("alos72", "2017-10-17T09:57:51.403184642Z", calendar.GetAlos72Minutes());
    }

    [Fact]
    public void SofZmanShmaAndTfila()
    {
        ZmanimCalendar calendar = FixtureCalendar();
        AssertInstant("sofZmanShmaGRA", "2017-10-17T13:55:53.352746510Z", calendar.GetSofZmanShmaGRA());
        AssertInstant("sofZmanShmaMGA72", "2017-10-17T13:19:53.352746510Z", calendar.GetSofZmanShmaMGA72Minutes());
        AssertInstant("sofZmanTfilaGRA", "2017-10-17T14:51:14.002600466Z", calendar.GetSofZmanTfilaGRA());
        AssertInstant("sofZmanTfilaMGA72", "2017-10-17T14:27:14.002600466Z", calendar.GetSofZmanTfilaMGA72Minutes());
    }

    [Fact]
    public void Chatzos()
    {
        ZmanimCalendar calendar = FixtureCalendar();
        AssertInstant("chatzosHayom", "2017-10-17T16:42:12.781249470Z", calendar.GetChatzosHayom());
        AssertInstant("chatzosHayomAsHalfDay", "2017-10-17T16:41:55.302308378Z", calendar.GetChatzosHayomAsHalfDay());
        AssertInstant("chatzosHalayla", "2017-10-18T04:42:06.833038724Z", calendar.GetChatzosHalayla());
    }

    [Fact]
    public void MinchaAndPlag()
    {
        ZmanimCalendar calendar = FixtureCalendar();
        AssertInstant("minchaGedolaGRA", "2017-10-17T17:09:35.627235356Z", calendar.GetMinchaGedolaGRA());
        AssertInstant("minchaKetanaGRA", "2017-10-17T19:55:37.576797224Z", calendar.GetMinchaKetanaGRA());
        AssertInstant("plagHaminchaGRA", "2017-10-17T21:04:48.389114669Z", calendar.GetPlagHaminchaGRA());
    }

    [Fact]
    public void CandleLightingAndTzais()
    {
        ZmanimCalendar calendar = FixtureCalendar();
        AssertInstant("candleLighting", "2017-10-17T21:55:59.201432122Z", calendar.GetCandleLighting());
        AssertInstant("tzais72", "2017-10-17T23:25:59.201432122Z", calendar.GetTzais72Minutes());
        AssertInstant("tzaisGeonim8.5", "2017-10-17T22:54:29.772724455Z", calendar.GetTzaisGeonim8Point5Degrees());
    }

    [Fact]
    public void ShaahZmanis()
    {
        ZmanimCalendar calendar = FixtureCalendar();
        TestTime.AssertDuration("shaahZmanisGRA", "PT55M20.649853956S", calendar.GetShaahZmanisGRA());
        TestTime.AssertDuration("shaahZmanis72", "PT1H7M20.649853956S", calendar.GetShaahZmanis72Minutes());
    }

    /// <summary>
    /// Toggling <see cref="ZmanimCalendar.UseElevation"/> must change the sunrise/sunset-based zmanim, since the
    /// elevation adjustment shifts the underlying sunrise. Written relative to the default so it does not depend on what
    /// that default currently is.
    /// </summary>
    [Fact]
    public void ElevationToggleAffectsZmanim()
    {
        ZmanimCalendar calendar = FixtureCalendar();
        bool defaultSetting = calendar.UseElevation;
        DateTimeOffset? atDefault = calendar.GetSofZmanShmaGRA();

        calendar.UseElevation = !defaultSetting;
        DateTimeOffset? toggled = calendar.GetSofZmanShmaGRA();

        AssertNotEquals(atDefault, toggled);
    }
}
