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
/// Coverage for , the NREL Solar Position Algorithm engine. Most cases are regression against the
/// library's own current output (8-decimal delta), but this suite also includes a Tier-A independently verifiable
/// anchor: the published NREL reference case (Reda &amp; Andreas, NREL/TP-560-34302), plus coverage of the
/// pressure/temperature/ΔT configuration knobs.
/// </summary>
public class SPACalculatorTest
{

    private const double Delta = 1e-8;
    private const double Zenith = 90.0;
    private const double NoEvent = double.NaN;

    private SPACalculator Calculator()
    {
        return new SPACalculator();
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
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 11.13593479},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 14.00756564},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 4.11873531},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", NoEvent},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 19.68292944},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 9.38998717},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 18.60534641},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 12.95741136},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 9.44905416},
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
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 22.24099364},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 1.31843788},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 15.64511647},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", NoEvent},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 8.56367311},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 21.53102585},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 5.65791097},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 20.21191650},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 0.58022832},
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
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 16.69350757},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 19.66695881},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 9.87816576},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", 11.42056091},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 2.12799374},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 15.46051863},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 0.13165412},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 16.58459142},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 17.01682783},
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
            new object[] {"NJ Standard Date", "2017-10-17", "NJ", 4.69189023},
            new object[] {"LA Standard Date", "2017-10-17", "LA", 7.66535207},
            new object[] {"Jerusalem Hist. Date", "1955-02-26", "JERUSALEM", 21.87679780},
            new object[] {"Norway Polar Solstice", "2017-06-21", "NORWAY", 23.42238409},
            new object[] {"Sydney Leap Day", "2020-02-29", "SYDNEY", 14.12642200},
            new object[] {"Macapa Equator New Year", "2000-01-01", "MACAPA", 3.46446150},
            new object[] {"Suva Fiji Solstice", "2023-06-21", "SUVA", 12.13348221},
            new object[] {"Ushuaia Southern Winter", "2017-06-21", "USHUAIA", 4.58641356},
            new object[] {"NJ Future DeltaT", "2100-07-04", "NJ", 5.01829184},
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
            new object[] {"NJ East", "2017-10-17", "NJ", 90.0, 9.96630489},
            new object[] {"NJ West", "2017-10-17", "NJ", 270.0, 23.43696124},
            new object[] {"LA East", "2017-10-17", "LA", 90.0, 12.71802515},
            new object[] {"LA West", "2017-10-17", "LA", 270.0, 2.63792459},
            new object[] {"Sydney East", "2020-02-29", "SYDNEY", 90.0, 20.92601768},
            new object[] {"Sydney West", "2020-02-29", "SYDNEY", 270.0, 7.34685571},
            new object[] {"Macapa East No Cross", "2000-01-01", "MACAPA", 90.0, NoEvent},
            new object[] {"Polar East", "2017-06-21", "NORWAY", 90.0, 6.01986389},
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
            new object[] {"NJ Noon", "2017-10-17 12:00:00", "NJ", 8.24144641},
            new object[] {"NJ Evening", "2017-10-17 18:30:00", "NJ", 33.57166079},
            new object[] {"LA Evening", "2017-10-17 20:00:00", "LA", 46.19392802},
            new object[] {"Polar Midnight", "2017-06-21 00:00:00", "NORWAY", 3.98549992},
            new object[] {"Sydney Morning", "2020-02-29 20:00:00", "SYDNEY", 2.89090468},
            new object[] {"Macapa Local Noon", "2000-01-01 15:00:00", "MACAPA", 65.99118866},
            new object[] {"Suva Afternoon", "2023-06-21 02:00:00", "SUVA", 40.24646972},
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
            new object[] {"NJ Noon", "2017-10-17 12:00:00", "NJ", 110.13757702},
            new object[] {"NJ Evening", "2017-10-17 18:30:00", "NJ", 212.62160660},
            new object[] {"LA Evening", "2017-10-17 20:00:00", "LA", 187.12536114},
            new object[] {"Polar Midnight", "2017-06-21 00:00:00", "NORWAY", 8.01129603},
            new object[] {"Sydney Morning", "2020-02-29 20:00:00", "SYDNEY", 97.32308820},
            new object[] {"Macapa Local Noon", "2000-01-01 15:00:00", "MACAPA", 164.22298555},
            new object[] {"Suva Afternoon", "2023-06-21 02:00:00", "SUVA", 325.62781823},
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
            new object[] {"NJ Civil", "2017-10-17", "NJ", 96.0, 10.70943766},
            new object[] {"NJ Nautical", "2017-10-17", "NJ", 102.0, 10.17613755},
            new object[] {"NJ Astronomical", "2017-10-17", "NJ", 108.0, 9.64457274},
            new object[] {"Sydney Civil", "2020-02-29", "SYDNEY", 96.0, 19.27874263},
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
            new object[] {"NJ No Elev Adj", "2017-10-17", "NJ", 11.17344124},
            new object[] {"Jerusalem No Elev Adj", "1955-02-26", "JERUSALEM", 4.18885159},
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
            new object[] {"NJ dT off", "2017-10-17", "NJ", 11.13586773},
            new object[] {"Jerusalem dT off", "1955-02-26", "JERUSALEM", 4.11871827},
        };
        foreach (object[] c in cases)
        {
            SPACalculator calc = Calculator();
            calc.ApplyDeltaT = false;
            double actual = calc.GetUTCSunrise(Date((string)c[1]), Geo((string)c[2]), Zenith, true);
            AssertUtcHour((string)c[0], (double)c[3], actual);
        }
    }

    /// <summary>
    /// Near the poles the closed-form first guess can say an event happens on a day the Sun only almost reaches the requested angle.
    /// The secant search then never converges; it must report no event (NaN), not its last guess. Example from the kosher-rust
    /// comparison: on 2127-03-14 at -77.2688, -92.8693 (3965 m) the Sun only gets about 10.04 degrees below the horizon, so there is no
    /// 10.2 degree (misheyakir) time.
    /// </summary>
    [Fact]
    public void UnconvergedRiseSetIsNoEvent()
    {
        GeoLocation location = new GeoLocation("Unconverged", -77.2688, -92.8693, 3965, TimeZoneInfo.FindSystemTimeZoneById("UTC"));
        double actual = Calculator().GetUTCSunrise(Date("2127-03-14"), location, 100.2, true);
        AssertTrue("expected NaN but got " + actual, double.IsNaN(actual));
    }

    [Fact]
    public void GetTimeAtAzimuthRejectsUnsupportedAzimuth()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Calculator().GetTimeAtAzimuth(Date("2017-10-17"), Geo("NJ"), 123.0);
        });
    }

    /*
     * Tier-A independently verifiable reference: the published NREL Solar Position Algorithm example
     * (Reda & Andreas, NREL/TP-560-34302). 2003-10-17 12:30:30 local (tz -7 => 19:30:30 UTC),
     * longitude -105.1786, latitude 39.742476, elevation 1830.14 m, pressure 820 mb, temperature 11 C,
     * delta-T 67 s. The paper reports topocentric elevation 39.888378 and azimuth 194.340241.
     */
    private SPACalculator NrelCalculator()
    {
        SPACalculator calc = new SPACalculator();
        calc.DeltaTOverride = 67.0;
        calc.Pressure = 820;
        calc.Temperature = 11;
        return calc;
    }

    private DateTimeOffset NrelInstant()
    {
        return Instant("2003-10-17 19:30:30");
    }

    private GeoLocation NrelLocation()
    {
        return new GeoLocation("NREL Reference", 39.742476, -105.1786, 1830.14, TestLocations.Utc);
    }

    [Fact]
    public void GetSolarElevationNrelReference()
    {
        double elevation = NrelCalculator().GetSolarElevation(NrelInstant(), NrelLocation());
        AssertEquals("NREL published topocentric elevation", 39.888378, elevation, 0.001);
    }

    [Fact]
    public void GetSolarAzimuthNrelReference()
    {
        double azimuth = NrelCalculator().GetSolarAzimuth(NrelInstant(), NrelLocation());
        AssertEquals("NREL published topocentric azimuth", 194.340241, azimuth, 0.001);
    }

    [Fact]
    public void DefaultsAndSetters()
    {
        SPACalculator calc = new SPACalculator();
        AssertTrue(calc.ApplyDeltaT);
        AssertNull(calc.DeltaTOverride);
        AssertEquals(1013.25, calc.Pressure, 0);
        AssertEquals(10.0, calc.Temperature, 0);

        calc.DeltaTOverride = 67.0;
        calc.Pressure = 820;
        calc.Temperature = 11;
        calc.ApplyDeltaT = false;

        AssertEquals((double?)67.0, calc.DeltaTOverride);
        AssertEquals(820.0, calc.Pressure, 0);
        AssertEquals(11.0, calc.Temperature, 0);
        AssertTrue(!calc.ApplyDeltaT);
    }

    [Fact]
    public void CalculatorNameTest()
    {
        AssertEquals("NREL Solar Position Algorithm", Calculator().CalculatorName);
    }
}
