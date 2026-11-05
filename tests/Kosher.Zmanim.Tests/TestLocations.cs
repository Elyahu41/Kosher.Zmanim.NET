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
using Kosher.Zmanim.Tests.Util;

using Kosher.Zmanim;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;
using static Kosher.Zmanim.Tests.JUnitAssert;

namespace Kosher.Zmanim.Tests;

/// <summary>
/// Shared test fixture holding the reference locations used across the calculator and calendar test suites. The
/// locations span both hemispheres, the equator, the dateline and the Arctic Circle to exercise a range of edge cases.
/// Each location carries its real <see cref="TimeZoneInfo"/> for the calendar-level tests (the low-level calculator tests operate
/// purely in UTC hours and are unaffected by the zone).
/// </summary>
public class TestLocations
{

    private TestLocations()
    {
        // utility holder; not instantiable
    }

    /// <summary>
    /// Woodcliff Lake area, New Jersey.
    /// </summary>
    public const double NjLat = 41.1181036;
    public const double NjLon = -74.0840691;
    public const double NjElev = 167;

    /// <summary>
    /// Los Angeles area, California.
    /// </summary>
    public const double LaLat = 34.0201613;
    public const double LaLon = -118.6919095;
    public const double LaElev = 71;

    /// <summary>
    /// Jerusalem, Israel.
    /// </summary>
    public const double JerusalemLat = 31.7962994;
    public const double JerusalemLon = 35.1053185;
    public const double JerusalemElev = 754;

    /// <summary>
    /// Northern Norway (inside the Arctic Circle - polar day/night edge cases).
    /// </summary>
    public const double NorwayLat = 70.1498248;
    public const double NorwayLon = 9.1456867;
    public const double NorwayElev = 0;

    /// <summary>
    /// Sydney, Australia (southern hemisphere).
    /// </summary>
    public const double SydneyLat = -33.8688;
    public const double SydneyLon = 151.2093;
    public const double SydneyElev = 58;

    /// <summary>
    /// Macapá, Brazil (on the equator).
    /// </summary>
    public const double MacapaLat = 0.0349;
    public const double MacapaLon = -51.0694;
    public const double MacapaElev = 15;

    /// <summary>
    /// Suva, Fiji (near the dateline).
    /// </summary>
    public const double SuvaLat = -18.1416;
    public const double SuvaLon = 178.4419;
    public const double SuvaElev = 6;

    /// <summary>
    /// Ushuaia, Argentina (far southern latitude).
    /// </summary>
    public const double UshuaiaLat = -54.8019;
    public const double UshuaiaLon = -68.3030;
    public const double UshuaiaElev = 23;

    /// <summary>
    /// Lakewood, New Jersey - the library's canonical reference location.
    /// </summary>
    public const double LakewoodLat = 40.0721087;
    public const double LakewoodLon = -74.2400243;
    public const double LakewoodElev = 15;

    /// <summary>
    /// A <see cref="TimeZoneInfo"/> of <c>UTC</c>, used for the low-level calculator tests where the calculation is done entirely in
    /// UTC hours and the zone is irrelevant.
    /// </summary>
    public static readonly TimeZoneInfo Utc = TimeZoneInfo.FindSystemTimeZoneById("UTC");

    public static GeoLocation Nj(TimeZoneInfo zoneId)
    {
        return new GeoLocation("Woodcliff Lake, NJ", NjLat, NjLon, NjElev, zoneId);
    }

    public static GeoLocation La(TimeZoneInfo zoneId)
    {
        return new GeoLocation("Los Angeles, CA", LaLat, LaLon, LaElev, zoneId);
    }

    public static GeoLocation Jerusalem(TimeZoneInfo zoneId)
    {
        return new GeoLocation("Jerusalem, Israel", JerusalemLat, JerusalemLon, JerusalemElev, zoneId);
    }

    public static GeoLocation Norway(TimeZoneInfo zoneId)
    {
        return new GeoLocation("Northern Norway", NorwayLat, NorwayLon, NorwayElev, zoneId);
    }

    public static GeoLocation Sydney(TimeZoneInfo zoneId)
    {
        return new GeoLocation("Sydney, Australia", SydneyLat, SydneyLon, SydneyElev, zoneId);
    }

    public static GeoLocation Macapa(TimeZoneInfo zoneId)
    {
        return new GeoLocation("Macapá, Brazil", MacapaLat, MacapaLon, MacapaElev, zoneId);
    }

    public static GeoLocation Suva(TimeZoneInfo zoneId)
    {
        return new GeoLocation("Suva, Fiji", SuvaLat, SuvaLon, SuvaElev, zoneId);
    }

    public static GeoLocation Ushuaia(TimeZoneInfo zoneId)
    {
        return new GeoLocation("Ushuaia, Argentina", UshuaiaLat, UshuaiaLon, UshuaiaElev, zoneId);
    }

    public static GeoLocation Lakewood()
    {
        return new GeoLocation("Lakewood, NJ", LakewoodLat, LakewoodLon, LakewoodElev, TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
    }
}
