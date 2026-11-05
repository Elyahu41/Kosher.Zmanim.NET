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
using Kosher.Zmanim.Internal;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;
using static Kosher.Zmanim.Tests.JUnitAssert;

namespace Kosher.Zmanim.Tests.Util;

/// <summary>
/// Coverage for the <see cref="AstronomicalCalculator"/> abstract base: the pluggable-calculator contract (<c>getDefault</c>,
/// naming, <c>equals</c>/<c>clone</c>), the refraction / solar-radius / earth-radius configuration, the date-based
/// <see cref="AstronomicalCalculator.GetApparentSolarRadius">apparent solar radius</see>, and the shared degree-based
/// trig helpers. The trig helpers are <b>Tier-A</b> checks verified against <see cref="Math"/>; this test lives in the
/// <c>util</c> package so it can reach those <c>protected static</c> helpers.
/// </summary>
public class AstronomicalCalculatorTest
{

    [Fact]
    public void GetDefaultReturnsNoaaCalculator()
    {
        AstronomicalCalculator calculator = AstronomicalCalculator.GetDefault();
        AssertTrue(calculator is NOAACalculator);
    }

    [Fact]
    public void CalculatorNames()
    {
        AssertEquals("US National Oceanic and Atmospheric Administration Algorithm",
                new NOAACalculator().CalculatorName);
        AssertEquals("US Naval Almanac Algorithm", new SunTimesCalculator().CalculatorName);
        AssertEquals("Jean Meeus Higher-Accuracy (VSOP87) Algorithm", new MeeusCalculator().CalculatorName);
        AssertEquals("NREL Solar Position Algorithm", new SPACalculator().CalculatorName);
    }

    [Fact]
    public void DefaultConfiguration()
    {
        AstronomicalCalculator calculator = new NOAACalculator();
        AssertEquals(34 / 60d, calculator.Refraction, 0);
        AssertEquals(16 / 60d, calculator.SolarRadius, 0);
        AssertEquals(6371.0088, calculator.EarthRadius, 0);
        AssertTrue(calculator.UseApparentSolarRadius);
    }

    [Fact]
    public void RefractionAndEarthRadiusSetters()
    {
        AstronomicalCalculator calculator = new NOAACalculator();
        calculator.Refraction = 0.5;
        calculator.EarthRadius = 6371.0;
        AssertEquals(0.5, calculator.Refraction, 0);
        AssertEquals(6371.0, calculator.EarthRadius, 0);
    }

    [Fact]
    public void SetSolarRadiusDisablesApparentRadius()
    {
        AstronomicalCalculator calculator = new NOAACalculator();
        calculator.SolarRadius = 16 / 60d;
        AssertEquals(16 / 60d, calculator.SolarRadius, 0);
        AssertFalse(calculator.UseApparentSolarRadius);
    }

    [Fact]
    public void SetSolarRadiusRejectsNegative()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new NOAACalculator().SolarRadius = -1;
        });
    }

    [Fact]
    public void ApparentSolarRadius()
    {
        AstronomicalCalculator calculator = new NOAACalculator();
        // null date falls back to the fixed 16' radius
        AssertEquals(16 / 60d, calculator.GetApparentSolarRadius(null), 0);
        // table is keyed by day-of-year against the (common-year) reference year 2050
        AssertEquals(0.27108024, calculator.GetApparentSolarRadius(new DateOnly(2050, 1, 1)), 1e-8);
        // the perihelion (~Jan 3) is near the annual maximum, the aphelion (~Jul 5) near the minimum
        AssertTrue(calculator.GetApparentSolarRadius(new DateOnly(2020, 1, 3))
                > calculator.GetApparentSolarRadius(new DateOnly(2020, 7, 5)));
        // Feb 29 resolves to Feb 28's value via withYear(2050)
        AssertEquals(calculator.GetApparentSolarRadius(new DateOnly(2020, 2, 28)),
                calculator.GetApparentSolarRadius(new DateOnly(2020, 2, 29)), 0);
    }

    [Fact]
    public void EqualsAndHashCode()
    {
        AstronomicalCalculator a = new NOAACalculator();
        AstronomicalCalculator b = new NOAACalculator();
        AssertEquals(a, b);
        AssertEquals(a.GetHashCode(), b.GetHashCode());

        b.Refraction = 0.5;
        AssertNotEquals(a, b);

        // different concrete classes with identical settings are not equal
        AssertNotEquals<AstronomicalCalculator>(new NOAACalculator(), new SunTimesCalculator());
    }

    [Fact]
    public void CloneProducesEqualIndependentCopy()
    {
        AstronomicalCalculator original = new NOAACalculator();
        AstronomicalCalculator clone = original.Clone();
        AssertNotSame(original, clone);
        AssertEquals(original, clone);

        clone.Refraction = 0.1;
        AssertNotEquals(original, clone);
    }

    /*
     * Tier-A: the degree-based trig helpers must agree with java.lang.Math. They are protected static, reachable here
     * because this test shares the com.kosherjava.zmanim.util package.
     */
    [Fact]
    public void DegreeTrigHelpersMatchMath()
    {
        double[] angles = { 0, 30, 45, 60, 90, 123.456, -17.5, 200 };
        foreach (double angle in angles)
        {
            AssertEquals("sin " + angle, Math.Sin(JavaMath.ToRadians(angle)),
                    AstronomicalCalculator.SinDegrees(angle), 1e-12);
            AssertEquals("cos " + angle, Math.Cos(JavaMath.ToRadians(angle)),
                    AstronomicalCalculator.CosDegrees(angle), 1e-12);
            AssertEquals("tan " + angle, Math.Tan(JavaMath.ToRadians(angle)),
                    AstronomicalCalculator.TanDegrees(angle), 1e-9);
        }
        double[] ratios = { -1, -0.5, 0, 0.25, 0.5, 1 };
        foreach (double ratio in ratios)
        {
            AssertEquals("asin " + ratio, JavaMath.ToDegrees(Math.Asin(ratio)),
                    AstronomicalCalculator.AsinDegrees(ratio), 1e-12);
            AssertEquals("acos " + ratio, JavaMath.ToDegrees(Math.Acos(ratio)),
                    AstronomicalCalculator.AcosDegrees(ratio), 1e-12);
        }
    }
}
