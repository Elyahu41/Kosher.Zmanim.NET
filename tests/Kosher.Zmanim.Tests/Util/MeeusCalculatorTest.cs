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
/// Regression coverage for , the higher-accuracy Jean Meeus (VSOP87) engine with ΔT
/// support. Expected values are the library's own current output (defaults, ΔT on) asserted to 8 decimals, plus a
/// ΔT-disabled variant via .
/// </summary>
public class MeeusCalculatorTest
{

    private const double Delta = 1e-8;
    private const double Zenith = 90.0;
    private const double NoEvent = double.NaN;

    private MeeusCalculator Calculator()
    {
        return new MeeusCalculator();
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
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 11.13565507},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 14.00730639},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 4.11849856},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", NoEvent},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 19.68266925},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 9.38974837},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 18.60510228},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 12.95697826},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 9.44873166},
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
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 22.24115497},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 1.31857880},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 15.64527430},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", NoEvent},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 8.56381339},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 21.53114700},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 5.65803778},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 20.21223110},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 0.58039250},
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
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 16.69344845},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 19.66689969},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 9.87812628},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", 11.42050168},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 2.12793408},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 15.46045983},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 0.13159547},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 16.58453220},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 17.01674900},
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
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 4.69183111},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 7.66529295},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 21.87675832},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", 23.42232487},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 14.12636235},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 3.46440271},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 12.13342359},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 4.58635433},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 5.01821298},
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
            new object[] {"NJ East", "2017-10-17", "NJ", 90.0, 9.96630242},
            new object[] {"NJ West", "2017-10-17", "NJ", 270.0, 23.43696025},
            new object[] {"LA East", "2017-10-17", "LA", 90.0, 12.71802243},
            new object[] {"LA West", "2017-10-17", "LA", 270.0, 2.63792382},
            new object[] {"Sydney East", "2020-02-29", "SYDNEY", 90.0, 20.92601400},
            new object[] {"Sydney West", "2020-02-29", "SYDNEY", 270.0, 7.34685529},
            new object[] {"Macapa East No Cross", "2000-01-01", "MACAPA", 90.0, NoEvent},
            new object[] {"Polar East", "2017-06-21", "NORWAY", 90.0, 6.01986159},
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
            new object[] {"NJ Noon", "2017-10-17 12:00:00", "NJ", 8.24234881},
            new object[] {"NJ Evening", "2017-10-17 18:30:00", "NJ", 33.57213757},
            new object[] {"LA Evening", "2017-10-17 20:00:00", "LA", 46.19473579},
            new object[] {"Polar Midnight", "2017-06-21 00:00:00", "NORWAY", 3.98711013},
            new object[] {"Sydney Morning", "2020-02-29 20:00:00", "SYDNEY", 2.89445709},
            new object[] {"Macapa Local Noon", "2000-01-01 15:00:00", "MACAPA", 65.99208585},
            new object[] {"Suva Afternoon", "2023-06-21 02:00:00", "SUVA", 40.24688524},
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
            new object[] {"NJ Noon", "2017-10-17 12:00:00", "NJ", 110.13820751},
            new object[] {"NJ Evening", "2017-10-17 18:30:00", "NJ", 212.62255322},
            new object[] {"LA Evening", "2017-10-17 20:00:00", "LA", 187.12661462},
            new object[] {"Polar Midnight", "2017-06-21 00:00:00", "NORWAY", 8.01211355},
            new object[] {"Sydney Morning", "2020-02-29 20:00:00", "SYDNEY", 97.32257044},
            new object[] {"Macapa Local Noon", "2000-01-01 15:00:00", "MACAPA", 164.22488663},
            new object[] {"Suva Afternoon", "2023-06-21 02:00:00", "SUVA", 325.62695543},
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
            new object[] {"NJ Civil", "2017-10-17", "NJ", 96.0, 10.70916170},
            new object[] {"NJ Nautical", "2017-10-17", "NJ", 102.0, 10.17586674},
            new object[] {"NJ Astronomical", "2017-10-17", "NJ", 108.0, 9.64430764},
            new object[] {"Sydney Civil", "2020-02-29", "SYDNEY", 96.0, 19.27848095},
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
            new object[] {"NJ No Elev Adj", "2017-10-17", "NJ", 11.17316120},
            new object[] {"Jerusalem No Elev Adj", "1955-02-26", "JERUSALEM", 4.18861444},
        };
        foreach (object[] c in cases)
        {
            double actual = Calculator().GetUTCSunrise(Date((string)c[1]), Geo((string)c[2]), Zenith, false);
            AssertUtcHour((string)c[0], (double)c[3], actual);
        }
    }

    [Fact]
    public void GetUTCSunriseWithDeltaTDisabled()
    {
        object[][] cases = {
            new object[] {"NJ dT off", "2017-10-17", "NJ", 11.13563996},
            new object[] {"Jerusalem dT off", "1955-02-26", "JERUSALEM", 4.11850671},
        };
        foreach (object[] c in cases)
        {
            MeeusCalculator calc = Calculator();
            calc.ApplyDeltaT = false;
            double actual = calc.GetUTCSunrise(Date((string)c[1]), Geo((string)c[2]), Zenith, true);
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
            new object[] {"London 16.1\u00b0 alos, Sun just reaches it", london, "2024-06-03", 106.1, true, 0.17268715},
            new object[] {"London 18\u00b0 tzais, Sun just reaches it", london, "2024-07-20", 108.0, false, 23.85897993},
            new object[] {"75.5\u00b0N sunrise just after midnight", arctic, "2093-04-22", Zenith, true, 0.22098753},
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
        AssertEquals("Jean Meeus Higher-Accuracy (VSOP87) Algorithm", Calculator().CalculatorName);
    }
}
