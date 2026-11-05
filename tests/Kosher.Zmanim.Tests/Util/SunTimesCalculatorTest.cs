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
/// Regression coverage for <see cref="SunTimesCalculator"/> (the older USNO / "Almanac for Computers" algorithm). Expected
/// values are the library's own current output asserted to 8 decimals. Unlike <see cref="NOAACalculator"/>, this calculator
/// returns <see cref="double.NaN"/> for solar noon/midnight at the polar location and does not implement solar
/// elevation/azimuth or time-at-azimuth (those throw <see cref="UnsupportedOperationException"/>).
/// </summary>
public class SunTimesCalculatorTest
{

    private const double Delta = 1e-8;
    private const double Zenith = 90.0;
    private const double NoEvent = double.NaN;

    private SunTimesCalculator Calculator()
    {
        return new SunTimesCalculator();
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
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 11.12643745},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 14.00076197},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 4.11037446},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", NoEvent},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 19.68724563},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 9.39464043},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 18.60351329},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 12.95527929},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 9.43972880},
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
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 22.25383390},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 1.32889810},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 15.65101844},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", NoEvent},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 8.56114598},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 21.53593703},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 5.65641338},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 20.20896383},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 0.57971462},
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
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 16.69011947},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 19.66482200},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 9.88071908},
            new object[] {"Norway Polar No Noon", "2017-06-21", "NORWAY", NoEvent},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 2.12420286},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 15.46529012},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 0.12996327},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 16.58212152},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 17.00973685},
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
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 4.69011947},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 7.66482200},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 21.88071908},
            new object[] {"Norway Polar No Midnight", "2017-06-21", "NORWAY", NoEvent},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 14.12420286},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 3.46529012},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 12.12996327},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 4.58212152},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 5.00973685},
        };
        foreach (object[] c in cases)
        {
            double actual = Calculator().GetUTCMidnight(Date((string)c[1]), Geo((string)c[2]));
            AssertUtcHour((string)c[0], (double)c[3], actual);
        }
    }

    [Fact]
    public void GetUTCSunriseAtZenith()
    {
        object[][] cases = {
            new object[] {"NJ Civil", "2017-10-17", "NJ", 96.0, 10.70056986},
            new object[] {"NJ Nautical", "2017-10-17", "NJ", 102.0, 10.16784691},
            new object[] {"NJ Astronomical", "2017-10-17", "NJ", 108.0, 9.63665090},
            new object[] {"Sydney Civil", "2020-02-29", "SYDNEY", 96.0, 19.28346936},
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
            new object[] {"NJ No Elev Adj", "2017-10-17", "NJ", 11.16388056},
            new object[] {"Jerusalem No Elev Adj", "1955-02-26", "JERUSALEM", 4.18050398},
        };
        foreach (object[] c in cases)
        {
            double actual = Calculator().GetUTCSunrise(Date((string)c[1]), Geo((string)c[2]), Zenith, false);
            AssertUtcHour((string)c[0], (double)c[3], actual);
        }
    }

    [Fact]
    public void GetTimeAtAzimuthUnsupported()
    {
        Assert.Throws<NotSupportedException>(() =>
        {
            Calculator().GetTimeAtAzimuth(Date("2017-10-17"), Geo("NJ"), 90.0);
        });
    }

    [Fact]
    public void GetSolarElevationUnsupported()
    {
        Assert.Throws<NotSupportedException>(() =>
        {
            Calculator().GetSolarElevation(DateTimeOffset.Parse("2017-10-17T12:00:00Z"), Geo("NJ"));
        });
    }

    [Fact]
    public void GetSolarAzimuthUnsupported()
    {
        Assert.Throws<NotSupportedException>(() =>
        {
            Calculator().GetSolarAzimuth(DateTimeOffset.Parse("2017-10-17T12:00:00Z"), Geo("NJ"));
        });
    }

    [Fact]
    public void CalculatorNameTest()
    {
        AssertEquals("US Naval Almanac Algorithm", Calculator().CalculatorName);
    }
}
