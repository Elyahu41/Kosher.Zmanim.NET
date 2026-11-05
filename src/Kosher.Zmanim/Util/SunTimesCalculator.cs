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
namespace Kosher.Zmanim.Util;

/// <summary>
/// Implementation of sunrise and sunset methods to calculate astronomical times. This calculator uses the Java algorithm
/// written by <a href="https://web.archive.org/web/20090531215353/http://www.kevinboone.com/suntimes.html">Kevin
/// Boone</a> that is based on the <a href = "https://aa.usno.navy.mil/">US Naval Observatory's</a><a
/// href="https://aa.usno.navy.mil/publications/asa">Astronomical Almanac</a> and used with his permission. Added to Kevin's
/// code is adjustment of the zenith to account for elevation. This algorithm returns the same time every year and does not
/// account for leap years. It is not as accurate as the Jean Meeus based <see cref="NOAACalculator"/> that is the default calculator
/// use by the KosherJava <em>zmanim</em> library, and is certainly not as accurate as some newer calculators such as the <see cref="MeeusCalculator"/> ot the <see cref="SPACalculator"/>. It also does not have an implementation of some solar calculation methods as
/// detailed in the deprecation notes.
/// </summary>
/// <remarks>
/// <b>Deprecated:</b> This calculator implements the legacy <em><a href=
///         "https://www.google.com/books/edition/Almanac_for_Computers/4f8TAQAAIAAJ">Almanac for Computers</a></em> procedure of the
///         US Naval Observatory, a deliberately simplified solar model (a single-term equation of center, fixed mean obliquity, and
///         no nutation or aberration) that was designed for roughly one-minute accuracy on early calculators and personal computers.
///         It is the least accurate of the calculators in this library by about an order of magnitude: in a grid study over latitudes
///         60°S to 60°N for all of 2024, its sunrise and sunset differed from an exact ephemeris by a median of approximately 8
///         seconds, compared with roughly 1 second for <see cref="NOAACalculator"/> and under 1 second for <see cref="MeeusCalculator"/> and
///         <see cref="SPACalculator"/>, with worst cases of tens of seconds and far larger errors at high latitudes and for deep (18°)
///         twilight. In addition, this calculator does not compute a true solar transit: <see cref="GetUTCNoon"/>
///         returns the midpoint of its own sunrise and sunset, <see cref="GetUTCMidnight"/> returns that value
///         plus twelve hours, and <see cref="GetSolarElevation"/> is not implemented. It is retained only for
///         backward compatibility and for cases where its simplicity is preferred over accuracy. For new code use
///         <see cref="SPACalculator"/> (the NREL Solar Position Algorithm, the most accurate option) or <see cref="MeeusCalculator"/> (the
///         high-accuracy method of Jean Meeus); where a lighter NOAA-family calculator is sufficient, <see cref="NOAACalculator"/> is the
///         library default and is already substantially more accurate than this class. There are no current plans on removing this
///         calculator. There is value on using it as a reference for various reasons, key among them is to compare and validate
///         historically calculates astronomical times.
/// </remarks>
[Obsolete("This calculator implements the legacy Almanac for Computers procedure of the US Naval Observatory, a deliberately simplified solar model (a single-term equation of center, fixed mean obliquity, and no nutation or aberration) that was designed for roughly one-minute accuracy on early calculators and personal computers.")]
public class SunTimesCalculator : AstronomicalCalculator
{

    /// <summary>
    /// This calculator's results depend only on the arguments and on settings whose setters clear the cache.
    /// </summary>
    protected override bool IsResultCacheable()
    {
        return true;
    }

    /// <summary>
    /// Default constructor of the SunTimesCalculator.
    /// </summary>
    public SunTimesCalculator()
    {
    }
    /// <inheritdoc/>
    public override string CalculatorName => "US Naval Almanac Algorithm";
    /// <inheritdoc/>
    public override double GetUTCSunrise(DateOnly localDate, GeoLocation geoLocation, double zenith, bool adjustForElevation)
    {
        double elevation = adjustForElevation ? geoLocation.SunriseElevation : 0;
        double adjustedZenith = AdjustZenith(zenith, elevation, localDate);
        return GetTimeUTC(localDate, geoLocation, adjustedZenith, true);
    }
    /// <inheritdoc/>
    public override double GetUTCSunset(DateOnly localDate, GeoLocation geoLocation, double zenith, bool adjustForElevation)
    {
        double elevation = adjustForElevation ? geoLocation.SunsetElevation : 0;
        double adjustedZenith = AdjustZenith(zenith, elevation, localDate);
        return GetTimeUTC(localDate, geoLocation, adjustedZenith, false);
    }

    /// <summary>
    /// The number of degrees of longitude that corresponds to one hour of time difference.
    /// </summary>
    private const double DegPerHour = 360.0 / 24.0;

    /// <summary>
    /// Get time difference between location's longitude and the Meridian, in hours.
    /// </summary>
    /// <param name="longitude">the longitude</param>
    /// <returns>time difference between the location's longitude and the Meridian, in hours. West of Meridian has a negative
    ///         time difference</returns>
    private static double GetHoursFromMeridian(double longitude)
    {
        return longitude / DegPerHour;
    }

    /// <summary>
    /// Calculate the approximate time of sunset or sunrise in days since midnight Jan 1st, assuming 6am and 6pm events. We
    /// need this figure to derive the Sun's mean anomaly.
    /// </summary>
    /// <param name="dayOfYear">the day of year</param>
    /// <param name="hoursFromMeridian">hours from the meridian</param>
    /// <param name="isSunrise">true for sunrise and false for sunset</param>
    /// <returns>the approximate time of sunset or sunrise in days since midnight Jan 1st, assuming 6am and 6pm events. We
    ///         need this figure to derive the Sun's mean anomaly.</returns>
    private static double GetApproxTimeDays(int dayOfYear, double hoursFromMeridian, bool isSunrise)
    {
        if (isSunrise)
        {
            return dayOfYear + ((6.0 - hoursFromMeridian) / 24);
        }
        else
        { // sunset
            return dayOfYear + ((18.0 - hoursFromMeridian) / 24);
        }
    }

    /// <summary>
    /// Calculate the Sun's mean anomaly in degrees, at sunrise or sunset, given the longitude in degrees
    /// </summary>
    /// <param name="dayOfYear">the day of the year</param>
    /// <param name="longitude">longitude</param>
    /// <param name="isSunrise">true for sunrise and false for sunset</param>
    /// <returns>the Sun's mean anomaly in degrees</returns>
    private static double GetMeanAnomaly(int dayOfYear, double longitude, bool isSunrise)
    {
        return (0.9856 * GetApproxTimeDays(dayOfYear, GetHoursFromMeridian(longitude), isSunrise)) - 3.289;
    }

    /// <summary>
    /// Returns the Sun's true longitude in degrees.
    /// </summary>
    /// <param name="sunMeanAnomaly">the Sun's mean anomaly in degrees</param>
    /// <returns>the Sun's true longitude in degrees. The result is an angle &gt;= 0 and &lt;= 360.</returns>
    private static double GetSunTrueLongitude(double sunMeanAnomaly)
    {
        double l = sunMeanAnomaly + (1.916 * SinDegrees(sunMeanAnomaly)) + (0.020 * SinDegrees(2 * sunMeanAnomaly)) + 282.634;
        return (l % 360 + 360) % 360; // return longitude as a 0-360 degree range

    }

    /// <summary>
    /// Calculates the Sun's right ascension in hours.
    /// </summary>
    /// <param name="sunTrueLongitude">the Sun's true longitude in degrees &gt; 0 and &lt; 360.</param>
    /// <returns>the Sun's right ascension in hours in angles &gt; 0 and &lt; 360.</returns>
    private static double GetSunRightAscensionHours(double sunTrueLongitude)
    {
        double a = 0.91764 * TanDegrees(sunTrueLongitude);
        double ra = 360.0 / (2.0 * Math.PI) * Math.Atan(a);

        double lQuadrant = Math.Floor(sunTrueLongitude / 90.0) * 90.0;
        double raQuadrant = Math.Floor(ra / 90.0) * 90.0;
        ra = ra + (lQuadrant - raQuadrant);

        return ra / DegPerHour; // convert to hours
    }

    /// <summary>
    /// Calculate the cosine of the Sun's local hour angle
    /// </summary>
    /// <param name="sunTrueLongitude">the sun's true longitude</param>
    /// <param name="latitude">the latitude</param>
    /// <param name="zenith">the zenith</param>
    /// <returns>the cosine of the Sun's local hour angle</returns>
    private static double GetCosLocalHourAngle(double sunTrueLongitude, double latitude, double zenith)
    {
        double sinDec = 0.39782 * SinDegrees(sunTrueLongitude);
        double cosDec = CosDegrees(AsinDegrees(sinDec));
        return (CosDegrees(zenith) - (sinDec * SinDegrees(latitude))) / (cosDec * CosDegrees(latitude));
    }

    /// <summary>
    /// Calculate local mean time of rising or setting. By 'local' is meant the exact time at the location, assuming that
    /// there were no time zone. That is, the time difference between the location and the Meridian depended entirely on
    /// the longitude. We can't do anything with this time directly; we must convert it to UTC and then to a local time.
    /// </summary>
    /// <param name="localHour">the local hour</param>
    /// <param name="sunRightAscensionHours">the sun's right ascension in hours</param>
    /// <param name="approxTimeDays">approximate time days</param>
    /// <returns>the fractional number of hours since midnight as a double</returns>
    private static double GetLocalMeanTime(double localHour, double sunRightAscensionHours, double approxTimeDays)
    {
        return localHour + sunRightAscensionHours - (0.06571 * approxTimeDays) - 6.622;
    }

    /// <summary>
    /// Get sunrise or sunset time in UTC, according to flag. This time is returned as a double and is not adjusted for time-zone.
    /// </summary>
    /// <param name="localDate">the <see cref="DateOnly"/> object to extract the day of year for calculation</param>
    /// <param name="geoLocation">The location information used for astronomical calculation of solar times.</param>
    /// <param name="zenith">Sun's zenith in degrees</param>
    /// <param name="isSunrise"><c>true</c> for sunrise and false for sunset.</param>
    /// <returns>the time as a double. If an error was encountered in the calculation (expected behavior for some locations such as
    ///         near the poles, <see cref="double.NaN"/> will be returned.</returns>
    private static double GetTimeUTC(DateOnly localDate, GeoLocation geoLocation, double zenith, bool isSunrise)
    {
        int dayOfYear = localDate.DayOfYear;
        double sunMeanAnomaly = GetMeanAnomaly(dayOfYear, geoLocation.Longitude, isSunrise);
        double sunTrueLong = GetSunTrueLongitude(sunMeanAnomaly);
        double sunRightAscensionHours = GetSunRightAscensionHours(sunTrueLong);
        double cosLocalHourAngle = GetCosLocalHourAngle(sunTrueLong, geoLocation.Latitude, zenith);

        double localHourAngle;
        if (isSunrise)
        {
            localHourAngle = 360.0 - AcosDegrees(cosLocalHourAngle);
        }
        else
        { // sunset
            localHourAngle = AcosDegrees(cosLocalHourAngle);
        }
        double localHour = localHourAngle / DegPerHour;

        double localMeanTime = GetLocalMeanTime(localHour, sunRightAscensionHours,
                GetApproxTimeDays(dayOfYear, GetHoursFromMeridian(geoLocation.Longitude), isSunrise));
        double pocessedTime = localMeanTime - GetHoursFromMeridian(geoLocation.Longitude);
        return (pocessedTime % 24 + 24) % 24; // ensure that the time is >= 0 and < 24
    }
    /// <inheritdoc/>
    public override double GetUTCNoon(DateOnly localDate, GeoLocation geoLocation)
    {
        double sunrise = GetUTCSunrise(localDate, geoLocation, 90, false);
        double sunset = GetUTCSunset(localDate, geoLocation, 90, false);
        double noon = sunrise + ((sunset - sunrise) / 2);
        if (noon < sunrise)
        {
            noon -= 12;
        }
        return (noon % 24 + 24) % 24; // ensure that the time is >= 0 and < 24
    }
    /// <inheritdoc/>
    public override double GetUTCMidnight(DateOnly localDate, GeoLocation geoLocation)
    {
        return (GetUTCNoon(localDate, geoLocation) + 12) % 24;
    }

    /// <summary>
    /// <b>This calculator class does not implement the getSolarAzimuth method, and throws a <see cref="NotSupportedException"/>.
    /// Use the <see cref="NOAACalculator"/>if this method is required</b>.
    /// <br/>
    /// </summary>
    /// <exception cref="NotSupportedException">This calculator class does not implement the getSolarAzimuth method. Use the
    ///         <see cref="NOAACalculator"/> instead.</exception>
    public override double GetSolarAzimuth(DateTimeOffset instant, GeoLocation geoLocation)
    {
        throw new NotSupportedException(
                "The SunTimesCalculator class does not implement the getSolarAzimuth method. Use the {@link NOAACalculator} instead.");
    }

    /// <summary>
    /// <b>This calculator class does not implement the getSolarElevation method, and throws a <see cref="NotSupportedException"/>.
    /// Use the <see cref="NOAACalculator"/>if this method is required</b>.
    /// <br/>
    /// </summary>
    /// <exception cref="NotSupportedException">This calculator class does not implement the getSolarElevation method. Use the
    ///         <see cref="NOAACalculator"/> instead.</exception>
    public override double GetSolarElevation(DateTimeOffset instant, GeoLocation geoLocation)
    {
        throw new NotSupportedException(
                "The SunTimesCalculator class does not implement the getSolarElevation method. Use the NOAACalculator instead.");
    }

    /// <summary>
    /// <b>This calculator class does not implement the getTimeAtAzimuth method, and throws a <see cref="NotSupportedException"/>.
    /// Use the <see cref="NOAACalculator"/>if this method is required</b>.
    /// <br/>
    /// </summary>
    /// <exception cref="NotSupportedException">This calculator class does not implement the getTimeAtAzimuth method. Use the
    ///         <see cref="NOAACalculator"/> instead.</exception>
    public override double GetTimeAtAzimuth(DateOnly localDate, GeoLocation geoLocation, double azimuth)
    {
        throw new NotSupportedException(
                "The SunTimesCalculator class does not implement the getTimeAtAzimuth method. Use the {@link NOAACalculator} instead.");
    }
}
