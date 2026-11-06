/*
 * Zmanim .NET API
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
using System.Globalization;

namespace Kosher.Zmanim.Internal;

/// <summary>
/// Numeric helpers that reproduce the exact semantics of the <c>java.lang.Math</c> and <c>java.lang.Double</c> methods
/// used by KosherJava, so that this port returns the same values as the Java library.
/// </summary>
internal static class JavaMath
{
    /// <summary>The value of <c>java.lang.Math.DEGREES_TO_RADIANS</c>.</summary>
    private const double DegreesToRadians = 0.017453292519943295;

    /// <summary>The value of <c>java.lang.Math.RADIANS_TO_DEGREES</c>.</summary>
    private const double RadiansToDegrees = 57.29577951308232;

    /// <summary>Equivalent of <c>Math.toRadians(double)</c>.</summary>
    public static double ToRadians(double degrees) => degrees * DegreesToRadians;

    /// <summary>Equivalent of <c>Math.toDegrees(double)</c>.</summary>
    public static double ToDegrees(double radians) => radians * RadiansToDegrees;

    /// <summary>
    /// Equivalent of <c>Math.round(double)</c>: the closest <see cref="long"/>, with ties rounding toward positive infinity
    /// (unlike <see cref="Math.Round(double)"/>, which rounds ties to even).
    /// </summary>
    public static long Round(double value)
    {
        if (double.IsNaN(value))
        {
            return 0;
        }
        double floor = Math.Floor(value);
        if (floor >= long.MaxValue)
        {
            return long.MaxValue;
        }
        if (floor <= long.MinValue)
        {
            return long.MinValue;
        }
        long result = (long)floor;
        return value - floor >= 0.5 ? result + 1 : result;
    }

    /// <summary>
    /// Equivalent of Java's <c>(long)</c> cast of a <see cref="double"/>: truncates toward zero, <see cref="double.NaN"/> becomes 0
    /// and values outside the <see cref="long"/> range saturate. A plain C# cast only behaves this way on .NET 9 and later; on
    /// .NET 8 a NaN, infinite or out of range value gives an unspecified result (<see cref="long.MinValue"/> on x64).
    /// </summary>
    public static long ToLong(double value)
    {
        if (double.IsNaN(value))
        {
            return 0;
        }
        if (value >= 9.2233720368547758E18)
        {
            return long.MaxValue;
        }
        if (value <= -9.2233720368547758E18)
        {
            return long.MinValue;
        }
        return (long)value;
    }

    /// <summary>Equivalent of <c>Math.round(float)</c> for <see cref="int"/> results.</summary>
    public static int RoundToInt(double value) => (int)Round(value);

    /// <summary>Equivalent of <c>Math.floorMod(int, int)</c>.</summary>
    public static int FloorMod(int x, int y)
    {
        int mod = x % y;
        return (mod != 0 && ((mod ^ y) < 0)) ? mod + y : mod;
    }

    /// <summary>Equivalent of <c>Math.floorMod(long, long)</c>.</summary>
    public static long FloorMod(long x, long y)
    {
        long mod = x % y;
        return (mod != 0 && ((mod ^ y) < 0)) ? mod + y : mod;
    }

    /// <summary>Equivalent of <c>Math.floorDiv(int, int)</c>.</summary>
    public static int FloorDiv(int x, int y)
    {
        int q = x / y;
        return ((x % y != 0) && ((x ^ y) < 0)) ? q - 1 : q;
    }

    /// <summary>Equivalent of <c>Math.floorDiv(long, long)</c>.</summary>
    public static long FloorDiv(long x, long y)
    {
        long q = x / y;
        return ((x % y != 0) && ((x ^ y) < 0)) ? q - 1 : q;
    }

    /// <summary>Equivalent of <c>Math.hypot(double, double)</c>.</summary>
    public static double Hypot(double x, double y) => Math.Sqrt(x * x + y * y);

    /// <summary>
    /// Equivalent of <c>Double.compare(double, double)</c>: <c>-0.0</c> sorts before <c>0.0</c> and <c>NaN</c> is equal to
    /// itself and greater than every other value.
    /// </summary>
    public static int Compare(double a, double b)
    {
        if (a < b)
        {
            return -1;
        }
        if (a > b)
        {
            return 1;
        }
        long aBits = BitConverter.DoubleToInt64Bits(double.IsNaN(a) ? double.NaN : a);
        long bBits = BitConverter.DoubleToInt64Bits(double.IsNaN(b) ? double.NaN : b);
        return aBits == bBits ? 0 : (aBits < bBits ? -1 : 1);
    }

    /// <summary>
    /// Formats a <see cref="double"/> the way <c>Double.toString(double)</c> does: the shortest representation that round
    /// trips, always with a decimal point (<c>1.0</c>), and computerized scientific notation (<c>1.0E-4</c>) for magnitudes
    /// below 10<sup>-3</sup> or at least 10<sup>7</sup>. Output is culture invariant.
    /// </summary>
    public static string ToJavaString(double value)
    {
        if (double.IsNaN(value))
        {
            return "NaN";
        }
        if (double.IsPositiveInfinity(value))
        {
            return "Infinity";
        }
        if (double.IsNegativeInfinity(value))
        {
            return "-Infinity";
        }
        if (value == 0)
        {
            return BitConverter.DoubleToInt64Bits(value) < 0 ? "-0.0" : "0.0";
        }
        double abs = Math.Abs(value);
        if (abs >= 1e-3 && abs < 1e7)
        {
            string s = value.ToString("R", CultureInfo.InvariantCulture);
            if (s.Contains('E'))
            {
                s = decimal.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            }
            return s.Contains('.') ? s : s + ".0";
        }
        string e = value.ToString("E16", CultureInfo.InvariantCulture);
        // Use the shortest round-trip digits rather than a fixed 17 digit mantissa.
        string r = value.ToString("R", CultureInfo.InvariantCulture);
        string digits;
        int exponent;
        if (r.Contains('E'))
        {
            int idx = r.IndexOf('E');
            digits = r.Substring(0, idx);
            exponent = int.Parse(r.Substring(idx + 1), CultureInfo.InvariantCulture);
        }
        else
        {
            // e.g. 12345678 -> 1.2345678E7
            string mantissa = e.Substring(0, e.IndexOf('E'));
            exponent = int.Parse(e.Substring(e.IndexOf('E') + 1), CultureInfo.InvariantCulture);
            digits = r;
            string sign = digits.StartsWith('-') ? "-" : string.Empty;
            string raw = digits.TrimStart('-').Replace(".", string.Empty).TrimStart('0');
            raw = raw.TrimEnd('0');
            if (raw.Length == 0)
            {
                raw = "0";
            }
            digits = sign + raw[0] + "." + (raw.Length > 1 ? raw.Substring(1) : "0");
            _ = mantissa;
            return digits + "E" + exponent.ToString(CultureInfo.InvariantCulture);
        }
        if (!digits.Contains('.'))
        {
            digits += ".0";
        }
        return digits + "E" + exponent.ToString(CultureInfo.InvariantCulture);
    }
}
