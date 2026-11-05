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
using Kosher.Zmanim.Tests;

using System.Globalization;
using Kosher.Zmanim;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;
using static Kosher.Zmanim.Tests.JUnitAssert;

namespace Kosher.Zmanim.Tests.Util;

/// <summary>
/// Regression coverage for , the library's default sunrise/sunset engine. Expected values are the
/// library's own current output asserted to 8 decimals. Cases span both hemispheres, the equator, the dateline, a polar
/// no-event, a leap day, a year boundary and a future ΔT date. Each test iterates a labeled case table.
/// </summary>
public class NOAACalculatorTest
{

    private const double Delta = 1e-8;
    private const double Zenith = 90.0; // AstronomicalCalculator.GEOMETRIC_ZENITH
    private const double NoEvent = double.NaN;

    private NOAACalculator Calculator()
    {
        return new NOAACalculator();
    }

    private GeoLocation Geo(string location)
    {
        switch (location)
        {
            case "NJ": return TestLocations.Nj(TestLocations.Utc);
            case "LA": return TestLocations.La(TestLocations.Utc);
            case "JERUSALEM": return TestLocations.Jerusalem(TestLocations.Utc);
            case "NORWAY": return TestLocations.Norway(TestLocations.Utc);
            case "SYDNEY": return TestLocations.Sydney(TestLocations.Utc);
            case "MACAPA": return TestLocations.Macapa(TestLocations.Utc);
            case "SUVA": return TestLocations.Suva(TestLocations.Utc);
            case "USHUAIA": return TestLocations.Ushuaia(TestLocations.Utc);
            default: throw new ArgumentException("Unknown location: " + location);
        }
    }

    private DateOnly Date(string iso)
    {
        return DateOnly.Parse(iso, CultureInfo.InvariantCulture);
    }

    private DateTimeOffset Instant(string isoDateTime)
    {
        return new DateTimeOffset(DateTime.ParseExact(isoDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), TimeSpan.Zero);
    }

    private void AssertUtcHour(string label, double expected, double actual)
    {
        if (double.IsNaN(expected))
        {
            AssertTrue(label + " should have no event (NaN)", double.IsNaN(actual));
        }
        else
        {
            AssertEquals(label, expected, actual, Delta);
        }
    }

    [Fact]
    public void GetUTCSunrise()
    {
        object[][] cases = {
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 11.13540831},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 14.00704301},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 4.11872483},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", NoEvent},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 19.68246214},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 9.39008078},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 18.60488873},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 12.95743787},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 9.44903860},
        };
        foreach (object[] c in cases)
        {
            double actual = Calculator().GetUTCSunrise(Date((string)c[1]), Geo((string)c[2]), Zenith, true);
            AssertUtcHour((string)c[0], (double)c[3], actual);
        }
    }

    [Fact]
    public void GetUTCSunset()
    {
        object[][] cases = {
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 22.24081501},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 1.31823827},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 15.64544001},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", NoEvent},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 8.56373515},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 21.53146217},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 5.65781259},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 20.21256620},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 0.58065990},
        };
        foreach (object[] c in cases)
        {
            double actual = Calculator().GetUTCSunset(Date((string)c[1]), Geo((string)c[2]), Zenith, true);
            AssertUtcHour((string)c[0], (double)c[3], actual);
        }
    }

    [Fact]
    public void GetUTCNoon()
    {
        object[][] cases = {
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 16.69315478},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 19.66659743},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 9.87832210},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", 11.42090460},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 2.12779094},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 15.46078241},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 0.13137581},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 16.58492880},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 17.01703523},
        };
        foreach (object[] c in cases)
        {
            double actual = Calculator().GetUTCNoon(Date((string)c[1]), Geo((string)c[2]));
            AssertUtcHour((string)c[0], (double)c[3], actual);
        }
    }

    [Fact]
    public void GetUTCMidnight()
    {
        object[][] cases = {
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 4.69150247},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 7.66495567},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 21.87692914},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", 23.42271259},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 14.12619634},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 3.46470789},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 12.13318970},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 4.58673539},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 5.01848806},
        };
        foreach (object[] c in cases)
        {
            double actual = Calculator().GetUTCMidnight(Date((string)c[1]), Geo((string)c[2]));
            AssertUtcHour((string)c[0], (double)c[3], actual);
        }
    }

    [Fact]
    public void GetTimeAtAzimuth()
    {
        object[][] cases = {
            new object[] {"NJ East", "2017-10-17", "NJ", 90.0, 9.96599151},
            new object[] {"NJ West", "2017-10-17", "NJ", 270.0, 23.43667951},
            new object[] {"LA East", "2017-10-17", "LA", 90.0, 12.71769344},
            new object[] {"LA West", "2017-10-17", "LA", 270.0, 2.63764230},
            new object[] {"Sydney East", "2020-02-29", "SYDNEY", 90.0, 20.92605878},
            new object[] {"Sydney West", "2020-02-29", "SYDNEY", 270.0, 7.34651923},
            new object[] {"Macapa East No Cross", "2000-01-01", "MACAPA", 90.0, NoEvent},
            new object[] {"Polar East", "2017-06-21", "NORWAY", 90.0, 6.02028371},
        };
        foreach (object[] c in cases)
        {
            double actual = Calculator().GetTimeAtAzimuth(Date((string)c[1]), Geo((string)c[2]), (double)c[3]);
            AssertUtcHour((string)c[0], (double)c[4], actual);
        }
    }

    [Fact]
    public void GetSolarElevation()
    {
        object[][] cases = {
            new object[] {"NJ Noon", "2017-10-17 12:00:00", "NJ", 8.24496867},
            new object[] {"NJ Evening", "2017-10-17 18:30:00", "NJ", 33.56992906},
            new object[] {"LA Evening", "2017-10-17 20:00:00", "LA", 46.19384884},
            new object[] {"Polar Midnight", "2017-06-21 00:00:00", "NORWAY", 3.98724677},
            new object[] {"Sydney Morning", "2020-02-29 20:00:00", "SYDNEY", 2.89748529},
            new object[] {"Macapa Local Noon", "2000-01-01 15:00:00", "MACAPA", 65.99072692},
            new object[] {"Suva Afternoon", "2023-06-21 02:00:00", "SUVA", 40.24513182},
        };
        foreach (object[] c in cases)
        {
            double actual = Calculator().GetSolarElevation(Instant((string)c[1]), Geo((string)c[2]));
            AssertEquals((string)c[0], (double)c[3], actual, Delta);
        }
    }

    [Fact]
    public void GetSolarAzimuth()
    {
        object[][] cases = {
            new object[] {"NJ Noon", "2017-10-17 12:00:00", "NJ", 110.14145579},
            new object[] {"NJ Evening", "2017-10-17 18:30:00", "NJ", 212.62717267},
            new object[] {"LA Evening", "2017-10-17 20:00:00", "LA", 187.13298966},
            new object[] {"Polar Midnight", "2017-06-21 00:00:00", "NORWAY", 8.00635444},
            new object[] {"Sydney Morning", "2020-02-29 20:00:00", "SYDNEY", 97.32262775},
            new object[] {"Macapa Local Noon", "2000-01-01 15:00:00", "MACAPA", 164.21440268},
            new object[] {"Suva Afternoon", "2023-06-21 02:00:00", "SUVA", 325.62368833},
        };
        foreach (object[] c in cases)
        {
            double actual = Calculator().GetSolarAzimuth(Instant((string)c[1]), Geo((string)c[2]));
            AssertEquals((string)c[0], (double)c[3], actual, Delta);
        }
    }

    [Fact]
    public void GetUTCSunriseAtZenith()
    {
        object[][] cases = {
            new object[] {"NJ Civil", "2017-10-17", "NJ", 96.0, 10.70891556},
            new object[] {"NJ Nautical", "2017-10-17", "NJ", 102.0, 10.17562194},
            new object[] {"NJ Astronomical", "2017-10-17", "NJ", 108.0, 9.64406486},
            new object[] {"Sydney Civil", "2020-02-29", "SYDNEY", 96.0, 19.27827196},
        };
        foreach (object[] c in cases)
        {
            double actual = Calculator().GetUTCSunrise(Date((string)c[1]), Geo((string)c[2]), (double)c[3], true);
            AssertUtcHour((string)c[0], (double)c[4], actual);
        }
    }

    [Fact]
    public void GetUTCSunriseWithoutElevationAdjustment()
    {
        object[][] cases = {
            new object[] {"NJ No Elev Adj", "2017-10-17", "NJ", 11.17291440},
            new object[] {"Jerusalem No Elev Adj", "1955-02-26", "JERUSALEM", 4.18884065},
        };
        foreach (object[] c in cases)
        {
            double actual = Calculator().GetUTCSunrise(Date((string)c[1]), Geo((string)c[2]), Zenith, false);
            AssertUtcHour((string)c[0], (double)c[3], actual);
        }
    }

    /// <summary>
    /// Days when the Sun only just reaches the zenith, around midnight. Using the Sun's declination at noon the zenith looks
    /// out of reach, but using the declination at solar midnight it is reached, so these return a time rather than NaN.
    /// </summary>
    [Fact]
    public void GetUTCSunRiseSetWhenOnlyJustReached()
    {
        GeoLocation london = new GeoLocation("London", 51.5074, -0.1278, 0, TestLocations.Utc);
        GeoLocation arctic = new GeoLocation("Arctic", 75.51550698948259, -0.9961193842273985, 1516.0617032522935,
                TestLocations.Utc);
        object[][] cases = {
            new object[] {"London 16.1\u00b0 alos, Sun just reaches it", london, "2024-06-03", 106.1, true, 0.17183556},
            new object[] {"London 18\u00b0 tzais, Sun just reaches it", london, "2024-07-20", 108.0, false, 23.85934340},
            new object[] {"75.5\u00b0N sunrise just after midnight", arctic, "2093-04-22", Zenith, true, 0.21591651},
        };
        foreach (object[] c in cases)
        {
            GeoLocation location = (GeoLocation)c[1];
            double zenith = (double)c[3];
            double actual = (bool)c[4]
                    ? Calculator().GetUTCSunrise(Date((string)c[2]), location, zenith, zenith == Zenith)
                    : Calculator().GetUTCSunset(Date((string)c[2]), location, zenith, zenith == Zenith);
            AssertUtcHour((string)c[0], (double)c[5], actual);
        }
    }

    [Fact]
    public void GetTimeAtAzimuthRejectsUnsupportedAzimuth()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Calculator().GetTimeAtAzimuth(Date("2017-10-17"), Geo("NJ"), 123.0);
        });
    }

    [Fact]
    public void CalculatorNameTest()
    {
        AssertEquals("US National Oceanic and Atmospheric Administration Algorithm", Calculator().CalculatorName);
    }
}
