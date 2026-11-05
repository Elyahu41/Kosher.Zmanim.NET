/*
 * Zmanim Java API
 * Copyright © 2004-2026 Eliyahu Hershfeld
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; version 2.1 of the License.
 *
 * This library is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
 * details.
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not, write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA,
 * or connect to: https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html
 */
namespace Kosher.Zmanim.Util;

/// <summary>
/// Implementation of sunrise and sunset methods to calculate astronomical times based on the <a
/// href="https://noaa.gov">NOAA</a> algorithm. This calculator uses the Java algorithm based on the implementation by <a
/// href="https://noaa.gov">NOAA - National Oceanic and Atmospheric Administration</a>'s <a href =
/// "https://www.srrb.noaa.gov/highlights/sunrise/sunrise.html">Surface Radiation Research Branch</a>. NOAA's <a
/// href="https://www.srrb.noaa.gov/highlights/sunrise/solareqns.PDF">implementation</a> is based on equations from <a
/// href="https://www.amazon.com/Astronomical-Table-Sun-Moon-Planets/dp/1942675038/">Astronomical Algorithms</a> by <a
/// href="https://en.wikipedia.org/wiki/Jean_Meeus">Jean Meeus</a>. Added to the algorithm is an adjustment of the zenith
/// to account for elevation. The algorithm can be found in the <a
/// href="https://en.wikipedia.org/wiki/Sunrise_equation">Wikipedia Sunrise Equation</a> article.
/// </summary>
public class NOAACalculator : AstronomicalCalculator
{

    /// <summary>
    /// This calculator's results depend only on the arguments and on settings whose setters clear the cache.
    /// </summary>
    protected override bool IsResultCacheable()
    {
        return true;
    }

    /// <summary>
    /// The <a href="https://en.wikipedia.org/wiki/Julian_day">Julian day</a> of January 1, 2000, known as
    /// <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.
    /// </summary>
    private const double JulianDayJan12000 = 2451545.0;

    /// <summary>
    /// Julian days per century.
    /// </summary>
    private const double JulianDaysPerCentury = 36525.0;

    /// <summary>
    /// An <c>enum</c> to indicate what type of solar event (<see cref="Sunrise">SUNRISE</see>, <see cref="Sunset">SUNSET</see>,
    /// <see cref="Noon">NOON</see> or <see cref="Midnight">MIDNIGHT</see>) is being calculated.
    /// .
    /// </summary>
    protected enum SolarEvent
    {
        /// <summary>
        /// SUNRISE A solar event related to sunrise
        /// </summary>
        Sunrise,
        /// <summary>
        /// SUNSET A solar event related to sunset
        /// </summary>
        Sunset,
        /// <summary>
        /// NOON A solar event related to noon
        /// </summary>
        Noon,
        /// <summary>
        /// MIDNIGHT A solar event related to midnight
        /// </summary>
        Midnight
    }

    /// <summary>
    /// Default constructor of the NOAACalculator.
    /// </summary>
    public NOAACalculator()
    {
    }

    /// <inheritdoc/>
    public override string CalculatorName => "US National Oceanic and Atmospheric Administration Algorithm"; // Implementation of the Jean Meeus algorithm

    /// <inheritdoc/>
    public override double GetUTCSunrise(DateOnly dt, GeoLocation geoLocation, double zenith, bool adjustForElevation)
    {
        return GetUTCSunRiseSet(dt, geoLocation, zenith, adjustForElevation, SolarEvent.Sunrise);
    }

    /// <inheritdoc/>
    public override double GetUTCSunset(DateOnly dt, GeoLocation geoLocation, double zenith, bool adjustForElevation)
    {
        return GetUTCSunRiseSet(dt, geoLocation, zenith, adjustForElevation, SolarEvent.Sunset);
    }

    /// <summary>
    /// A method that calculates UTC sunrise or sunset as well as any time based on an angle above or below sunset and
    /// returns it as a <c>double</c> in 24-hour format. 5:45:00 AM will return 5.75.
    /// </summary>
    /// <param name="localDate">Used to calculate day of year.</param>
    /// <param name="geoLocation">The location information used for astronomical calculation of solar times.</param>
    /// <param name="zenith">the azimuth below the vertical zenith of 90°. For sunset typically the <see cref="AstronomicalCalculator.AdjustZenith">zenith</see> used for
    ///         the calculation uses geometric zenith of 90° and <see cref="AstronomicalCalculator.AdjustZenith">adjusts</see> this slightly to account for solar
    ///         refraction and the sun's radius. Another example would be
    ///         <see cref="AstronomicalCalendar.GetEndNauticalTwilight"/> that passes
    ///         <see cref="AstronomicalCalendar.NauticalZenith"/> to this method.</param>
    /// <param name="adjustForElevation">Should the time be adjusted for elevation</param>
    /// <param name="solarEvent">if the calculation is for <see cref="SolarEvent.Sunrise"/> or <see cref="SolarEvent.Sunset"/></param>
    /// <returns>The UTC time of sunrise or sunset in 24-hour format. 5:45:00 AM will return 5.75, while 5:45 PM will return 17.75. If
    ///         an error was encountered in the calculation (expected behavior for some locations such as near the poles,
    ///         <see cref="double.NaN"/> will be returned.</returns>
    /// <seealso cref="AstronomicalCalculator.GetElevationAdjustment"/>
    private double GetUTCSunRiseSet(DateOnly localDate, GeoLocation geoLocation, double zenith, bool adjustForElevation,
            SolarEvent solarEvent)
    {
        double elevation = 0;
        if (adjustForElevation)
        {
            if (solarEvent == SolarEvent.Sunrise)
            {
                elevation = geoLocation.SunriseElevation;
            }
            else
            {
                elevation = geoLocation.SunsetElevation;
            }
        }
        double adjustedZenith = AdjustZenith(zenith, elevation, localDate);
        double riseSet = GetSunRiseSetUTC(localDate, geoLocation.Latitude, -geoLocation.Longitude,
                adjustedZenith, solarEvent);
        riseSet = riseSet / 60;
        return (riseSet % 24 + 24) % 24; // ensure that the time is >= 0 and < 24
    }

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/Julian_day">Julian day</a> from a Java <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="localDate">The LocalDate</param>
    /// <returns>the Julian day corresponding to the date Note: Number is returned for the start of the Julian day. Fractional days
    ///         / time should be added later.</returns>
    private static double GetJulianDay(DateOnly localDate)
    {
        int year = localDate.Year;
        int month = localDate.Month;
        int day = localDate.Day;

        if (month <= 2)
        {
            year -= 1;
            month += 12;
        }

        int a = year / 100;
        int b = 2 - a + a / 4;

        return Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + b - 1524.5;
    }

    /// <summary>
    /// Convert <a href="https://en.wikipedia.org/wiki/Julian_day">Julian day</a> to centuries since
    /// <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.
    /// </summary>
    /// <param name="julianDay">the Julian Day to convert</param>
    /// <returns>the centuries since 2000 Julian corresponding to the Julian Day</returns>
    private static double GetJulianCenturiesFromJulianDay(double julianDay)
    {
        return (julianDay - JulianDayJan12000) / JulianDaysPerCentury;
    }

    /// <summary>
    /// Returns the Geometric <a href="https://en.wikipedia.org/wiki/Mean_longitude">Mean Longitude</a> of the Sun.
    /// </summary>
    /// <param name="julianCenturies">the number of Julian centuries since
    ///         <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.</param>
    /// <returns>the Geometric Mean Longitude of the Sun in degrees</returns>
    private static double GetSunGeometricMeanLongitude(double julianCenturies)
    {
        double longitude = 280.46646 + julianCenturies * (36000.76983 + 0.0003032 * julianCenturies);
        return (longitude % 360 + 360) % 360; // return a longitude is in the range of 0 - 360
    }

    /// <summary>
    /// Returns the Geometric <a href="https://en.wikipedia.org/wiki/Mean_anomaly">Mean Anomaly</a> of the Sun in degrees.
    /// </summary>
    /// <param name="julianCenturies">the number of Julian centuries since
    ///         <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.</param>
    /// <returns>the Geometric Mean Anomaly of the Sun in degrees</returns>
    private static double GetSunGeometricMeanAnomaly(double julianCenturies)
    {
        return 357.52911 + julianCenturies * (35999.05029 - 0.0001537 * julianCenturies);
    }

    /// <summary>
    /// Return the unitless <a href="https://en.wikipedia.org/wiki/Eccentricity_%28orbit%29">eccentricity of earth's orbit</a>.
    /// </summary>
    /// <param name="julianCenturies">the number of Julian centuries since
    ///         <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.</param>
    /// <returns>the unitless eccentricity</returns>
    private static double GetEarthOrbitEccentricity(double julianCenturies)
    {
        return 0.016708634 - julianCenturies * (0.000042037 + 0.0000001267 * julianCenturies);
    }

    /// <summary>
    /// Returns the <a href="https://en.wikipedia.org/wiki/Equation_of_the_center">equation of center</a> for the sun in degrees.
    /// </summary>
    /// <param name="julianCenturies">the number of Julian centuries since
    ///         <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.</param>
    /// <returns>the equation of center for the sun in degrees</returns>
    private static double GetSunEquationOfCenter(double julianCenturies)
    {
        double m = GetSunGeometricMeanAnomaly(julianCenturies);
        double sinm = SinDegrees(m);
        double sin2m = SinDegrees(m + m);
        double sin3m = SinDegrees(m + m + m);
        return sinm * (1.914602 - julianCenturies * (0.004817 + 0.000014 * julianCenturies)) + sin2m
                * (0.019993 - 0.000101 * julianCenturies) + sin3m * 0.000289;
    }

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/True_longitude">true longitude</a> of the sun.
    /// </summary>
    /// <param name="julianCenturies">the number of Julian centuries since
    ///         <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.</param>
    /// <returns>the sun's true longitude in degrees</returns>
    private static double GetSunTrueLongitude(double julianCenturies)
    {
        double sunLongitude = GetSunGeometricMeanLongitude(julianCenturies);
        double center = GetSunEquationOfCenter(julianCenturies);
        return sunLongitude + center;
    }

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/Apparent_longitude">apparent longitude</a> of the sun.
    /// </summary>
    /// <param name="julianCenturies">the number of Julian centuries since
    ///         <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.</param>
    /// <returns>sun's apparent longitude in degrees</returns>
    private static double GetSunApparentLongitude(double julianCenturies)
    {
        double sunTrueLongitude = GetSunTrueLongitude(julianCenturies);
        double omega = 125.04 - 1934.136 * julianCenturies;
        return sunTrueLongitude - 0.00569 - 0.00478 * SinDegrees(omega);
    }

    /// <summary>
    /// Returns the mean <a href="https://en.wikipedia.org/wiki/Axial_tilt">obliquity of the ecliptic</a> (Axial tilt).
    /// </summary>
    /// <param name="julianCenturies">the number of Julian centuries since
    ///         <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.</param>
    /// <returns>the mean obliquity in degrees</returns>
    private static double GetMeanObliquityOfEcliptic(double julianCenturies)
    {
        double seconds = 21.448 - julianCenturies
                * (46.8150 + julianCenturies * (0.00059 - julianCenturies * (0.001813)));
        return 23.0 + (26.0 + (seconds / 60.0)) / 60.0;
    }

    /// <summary>
    /// Returns the corrected <a href="https://en.wikipedia.org/wiki/Axial_tilt">obliquity of the ecliptic</a> (Axial tilt).
    /// </summary>
    /// <param name="julianCenturies">the number of Julian centuries since
    ///         <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.</param>
    /// <returns>the corrected obliquity in degrees</returns>
    private static double GetObliquityCorrection(double julianCenturies)
    {
        double obliquityOfEcliptic = GetMeanObliquityOfEcliptic(julianCenturies);
        double omega = 125.04 - 1934.136 * julianCenturies;
        return obliquityOfEcliptic + 0.00256 * CosDegrees(omega);
    }

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/Declination">declination</a> of the sun.
    /// </summary>
    /// <param name="julianCenturies">the number of Julian centuries since
    ///         <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.</param>
    /// <returns>the sun's declination in degrees</returns>
    private static double GetSunDeclination(double julianCenturies)
    {
        double obliquityCorrection = GetObliquityCorrection(julianCenturies);
        double lambda = GetSunApparentLongitude(julianCenturies);
        double sint = SinDegrees(obliquityCorrection) * SinDegrees(lambda);
        return AsinDegrees(sint);
    }

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/Equation_of_time">Equation of Time</a> - the difference between
    /// true solar time and mean solar time
    /// </summary>
    /// <param name="julianCenturies">the number of Julian centuries since
    ///         <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.</param>
    /// <returns>equation of time in minutes of time</returns>
    private static double GetEquationOfTime(double julianCenturies)
    {
        double epsilon = GetObliquityCorrection(julianCenturies);
        double geomMeanLongSun = GetSunGeometricMeanLongitude(julianCenturies);
        double eccentricityEarthOrbit = GetEarthOrbitEccentricity(julianCenturies);
        double geomMeanAnomalySun = GetSunGeometricMeanAnomaly(julianCenturies);
        double y = TanDegrees(epsilon / 2.0);
        y *= y;
        double sin2l0 = SinDegrees(2.0 * geomMeanLongSun);
        double sinm = SinDegrees(geomMeanAnomalySun);
        double cos2l0 = CosDegrees(2.0 * geomMeanLongSun);
        double sin4l0 = SinDegrees(4.0 * geomMeanLongSun);
        double sin2m = SinDegrees(2.0 * geomMeanAnomalySun);
        double equationOfTime = y * sin2l0 - 2.0 * eccentricityEarthOrbit * sinm + 4.0 * eccentricityEarthOrbit * y
                * sinm * cos2l0 - 0.5 * y * y * sin4l0 - 1.25 * eccentricityEarthOrbit * eccentricityEarthOrbit * sin2m;
        return JavaMath.ToDegrees(equationOfTime) * 4.0;
    }

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/Hour_angle">hour angle</a> of the sun in
    /// <a href="https://en.wikipedia.org/wiki/Radian">radians</a> at for the latitude.
    /// </summary>
    /// <param name="latitude">the latitude of observer in degrees</param>
    /// <param name="solarDeclination">the declination angle of sun in degrees</param>
    /// <param name="zenith">the zenith</param>
    /// <param name="solarEvent">If the hour angle is for <see cref="SolarEvent.Sunrise">SUNRISE</see> or <see cref="SolarEvent.Sunset">SUNSET</see></param>
    /// <returns>hour angle of sunrise in <a href="https://en.wikipedia.org/wiki/Radian">radians</a></returns>
    private static double GetSunHourAngle(double latitude, double solarDeclination, double zenith, SolarEvent solarEvent)
    {
        double ratio = CosDegrees(zenith) / (CosDegrees(latitude) * CosDegrees(solarDeclination)) - TanDegrees(latitude)
                * TanDegrees(solarDeclination);
        double hourAngle = Math.Acos(ratio);

        if (solarEvent == SolarEvent.Sunset)
        {
            hourAngle = -hourAngle;
        }
        return hourAngle;
    }



    /// <inheritdoc/>
    public override double GetSolarElevation(DateTimeOffset instant, GeoLocation geoLocation)
    {
        return GetSolarElevationAzimuth(instant, geoLocation, false);
    }

    /// <inheritdoc/>
    public override double GetSolarAzimuth(DateTimeOffset instant, GeoLocation geoLocation)
    {
        return GetSolarElevationAzimuth(instant, geoLocation, true);
    }

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/Celestial_coordinate_system">Solar Elevation</a> or
    /// <a href="https://en.wikipedia.org/wiki/Celestial_coordinate_system">Solar Azimuth</a> at the given location
    /// and time. Can be negative if the sun is below the horizon. Elevation is based on sea-level and is not
    /// adjusted for altitude.
    /// </summary>
    /// <param name="instant">date-time of calculation</param>
    /// <param name="geoLocation">The location for calculating the elevation or azimuth.</param>
    /// <param name="isAzimuth">true for azimuth, false for elevation</param>
    /// <returns>solar elevation or azimuth in degrees.</returns>
    /// <seealso cref="GetSolarElevation"/>
    /// <seealso cref="GetSolarAzimuth"/>
    private double GetSolarElevationAzimuth(DateTimeOffset instant, GeoLocation geoLocation, bool isAzimuth)
    {
        double lat = geoLocation.Latitude;
        double lon = geoLocation.Longitude;
        DateTimeOffset utc = instant.ToUniversalTime();
        double fractionalDay = (utc.Hour + (utc.Minute
                + (utc.Second + JavaTime.GetNano(utc) / 1_000_000_000.0) / 60.0) / 60.0) / 24.0;
        double jd = GetJulianDay(DateOnly.FromDateTime(utc.DateTime)) + fractionalDay;
        double jc = GetJulianCenturiesFromJulianDay(jd);
        double decl = GetSunDeclination(jc);
        double eot = GetEquationOfTime(jc);
        double trueSolarTime = ((fractionalDay + eot / 1440.0 + lon / 360.0) + 2) % 1;
        double hourAngle = trueSolarTime * 2 * Math.PI - Math.PI;
        double cosZenith = SinDegrees(lat) * SinDegrees(decl) + CosDegrees(lat) * CosDegrees(decl) * Math.Cos(hourAngle);
        double zenithDeg = AcosDegrees(Math.Max(-1, Math.Min(1, cosZenith)));
        double elevation = (90.0 - zenithDeg) + AdjustElevationForRefraction(90.0 - zenithDeg);
        double azimuth;
        double azDenom = CosDegrees(lat) * SinDegrees(zenithDeg);

        if (Math.Abs(azDenom) > 0.001)
        {
            double az = (SinDegrees(lat) * CosDegrees(zenithDeg) - SinDegrees(decl)) / azDenom;
            azimuth = 180 - AcosDegrees(Math.Max(-1, Math.Min(1, az))) * (hourAngle > 0 ? -1 : 1);
        }
        else
        {
            azimuth = lat > 0 ? 180 : 0;
        }
        return isAzimuth ? (azimuth + 360) % 360 : elevation;
    }


    /// <summary>
    /// Apply refraction adjustment to solar elevation.
    /// </summary>
    /// <param name="elevation">the elevation to adjust.</param>
    /// <returns>the adjusted elevation.</returns>
    private double AdjustElevationForRefraction(double elevation)
    {
        if (elevation > 85.0)
        {
            return 0.0;
        }

        double te = TanDegrees(elevation);
        double correction;

        if (elevation > 5.0)
        {
            correction = 58.1 / te - 0.07 / Math.Pow(te, 3) + 0.000086 / Math.Pow(te, 5);
        }
        else if (elevation > -0.575)
        {
            correction = 1735.0 + elevation * (-518.2 + elevation * (103.4 + elevation * (-12.79 + 0.711 * elevation)));
        }
        else
        {
            correction = -20.774 / te;
        }
        return correction / 3600.0;
    }

    /// <seealso cref="GetSolarNoonMidnightUTC"/>
    public override double GetUTCNoon(DateOnly localDate, GeoLocation geoLocation)
    {
        double noon = GetSolarNoonMidnightUTC(GetJulianDay(localDate), -geoLocation.Longitude, SolarEvent.Noon);
        noon = noon / 60;
        return (noon % 24 + 24) % 24; // ensure that the time is >= 0 and < 24
    }

    /// <seealso cref="GetSolarNoonMidnightUTC"/>
    public override double GetUTCMidnight(DateOnly localDate, GeoLocation geoLocation)
    {
        double midnight = GetSolarNoonMidnightUTC(GetJulianDay(localDate), -geoLocation.Longitude, SolarEvent.Midnight);
        midnight = midnight / 60;
        return (midnight % 24 + 24) % 24; // ensure that the time is >= 0 and < 24
    }

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/Universal_Coordinated_Time">Universal Coordinated Time</a> (UTC) of the
    /// current day <a href="http://en.wikipedia.org/wiki/Noon#Solar_noon">solar noon</a> or the upcoming midnight (about 12 hours
    /// after solar noon) of the given day at the given location on earth.
    /// </summary>
    /// <param name="julianDay">The Julian day since <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a>.</param>
    /// <param name="longitude">The longitude of observer in degrees</param>
    /// <param name="solarEvent">If the calculation is for <see cref="SolarEvent.Noon">NOON</see> or <see cref="SolarEvent.Midnight">MIDNIGHT</see></param>
    /// <returns>The UTC time of sunset in 24-hour format. 12:45:00 AM will return 0.75 while 1:45:00 PM will return 13.75. If an error
    ///         was encountered in the calculation (expected behavior for some locations such as near the poles, <see cref="double.NaN"/>
    ///         will be returned.</returns>
    /// <seealso cref="GetUTCNoon"/>
    /// <seealso cref="GetUTCMidnight"/>
    private static double GetSolarNoonMidnightUTC(double julianDay, double longitude, SolarEvent solarEvent)
    {
        // no day-half shift: the loop epoch julianDay + solNoonUTC/1440 already lands on the event
        // First pass for approximate solar noon to calculate equation of time
        double tnoon = GetJulianCenturiesFromJulianDay(julianDay + longitude / 360.0);
        double equationOfTime = GetEquationOfTime(tnoon);
        double solNoonUTC = (longitude * 4) - equationOfTime; // minutes

        // Refine the equation of time at the calculated transit time.
        double newt;
        for (int i = 0; i < 2; i++)
        {
            newt = GetJulianCenturiesFromJulianDay(julianDay + solNoonUTC / 1440.0);
            equationOfTime = GetEquationOfTime(newt);
            solNoonUTC = (solarEvent == SolarEvent.Noon ? 720 : 1440) + (longitude * 4) - equationOfTime;
        }
        return solNoonUTC;
    }

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/Universal_Coordinated_Time">Universal Coordinated Time</a> (UTC)
    /// of sunrise or sunset in minutes for the given day at the given location on earth.
    /// </summary>
    /// <param name="localDate">The <see cref="DateOnly"/>.</param>
    /// <param name="latitude">The latitude of observer in degrees</param>
    /// <param name="longitude">Longitude of observer in degrees</param>
    /// <param name="zenith">Zenith</param>
    /// <param name="solarEvent">If the calculation is for <see cref="SolarEvent.Sunrise">SUNRISE</see> or <see cref="SolarEvent.Sunset">SUNSET</see></param>
    /// <returns>The UTC time of sunrise or sunset in 24-hour format. 5:45:00 AM will return 5.75, while 5:45 PM will return 17.75. If
    ///         an error was encountered in the calculation (expected behavior for some locations such as near the poles,
    ///         <see cref="double.NaN"/> will be returned.</returns>
    /// <remarks>
    /// <b>To do:</b> Possibly increase the number of passes for improved accuracy, especially in the Arctic areas.
    /// </remarks>
    private static double GetSunRiseSetUTC(DateOnly localDate, double latitude, double longitude, double zenith,
            SolarEvent solarEvent)
    {
        double julianDay = GetJulianDay(localDate);

        // Find the time of solar noon at the location, and use that declination.
        // This is better than start of the Julian day
        // TODO really not needed since the Julian day starts from local fixed noon. Changing this would be more
        // efficient but would likely cause a very minor discrepancy in the calculated times (likely not reducing
        // accuracy, just slightly different, thus potentially breaking test cases). Regardless, it would be within
        // milliseconds.
        double noonmin = GetSolarNoonMidnightUTC(julianDay, longitude, SolarEvent.Noon);
        double tnoon = GetJulianCenturiesFromJulianDay(julianDay + noonmin / 1440.0);
        // First calculates sunrise and approximate length of day
        double equationOfTime = GetEquationOfTime(tnoon);
        double solarDeclination = GetSunDeclination(tnoon);
        double hourAngle = GetSunHourAngle(latitude, solarDeclination, zenith, solarEvent);
        if (double.IsNaN(hourAngle))
        {
            // Using the Sun's declination at noon, the Sun never reaches this zenith today. But the declination changes
            // during the day (by up to about 0.2° in 12 hours), so the Sun may still just reach it around midnight, when it
            // is at its lowest. Before reporting that there is no event, try again with the declination and equation of time
            // at that solar midnight: the one before sunrise, or the one after sunset.
            double midnight = noonmin + (solarEvent == SolarEvent.Sunrise ? -720 : 720);
            double tmidnight = GetJulianCenturiesFromJulianDay(julianDay + midnight / 1440.0);
            equationOfTime = GetEquationOfTime(tmidnight);
            solarDeclination = GetSunDeclination(tmidnight);
            hourAngle = GetSunHourAngle(latitude, solarDeclination, zenith, solarEvent);
        }
        double delta = longitude - JavaMath.ToDegrees(hourAngle);
        double timeDiff = 4 * delta;
        double timeUTC = 720 + timeDiff - equationOfTime;
        // Second pass includes fractional Julian Day in gamma calc
        double newt = GetJulianCenturiesFromJulianDay(julianDay + timeUTC / 1440.0);
        equationOfTime = GetEquationOfTime(newt);
        solarDeclination = GetSunDeclination(newt);
        hourAngle = GetSunHourAngle(latitude, solarDeclination, zenith, solarEvent);
        delta = longitude - JavaMath.ToDegrees(hourAngle);
        timeDiff = 4 * delta;
        timeUTC = 720 + timeDiff - equationOfTime;
        return timeUTC;
    }

    /// <summary>
    /// The current implementation in this class only supports azimuth values of 90° (directly east) or 270° (directly west) that are
    /// directly needed in this library for the <see cref="ComprehensiveZmanimCalendar.GetPolarSunsetBenIshChai"/>
    /// and <see cref="ComprehensiveZmanimCalendar.GetPolarSunriseBenIshChai"/>.
    /// </summary>
    /// <exception cref="ArgumentException">if the azimuth is not 90° or 270°.</exception>
    /// <remarks>
    /// <b>To do:</b> complete the implementation for other azimuths. While not needed by this library, they may be of value to some projects.
    ///         There will be edge cases where the azimuth will occur more than once a day when based on the equation of time, the day
    ///         is shorter than 24 hours. In that case, the time for the first one will be returned.
    /// </remarks>
    public override double GetTimeAtAzimuth(DateOnly localDate, GeoLocation geoLocation, double targetAzimuth)
    {
        if (targetAzimuth != 90.0 && targetAzimuth != 270.0)
        {
            throw new ArgumentException("The targetAzimuth must be 90 or 270. Other azimuth values are not supported");
        }

        double julianDay = GetJulianDay(localDate);
        double solarNoonBase = 0.5 - (geoLocation.Longitude / 360.0);
        double dateTime = solarNoonBase + ((targetAzimuth == 90.0) ? 0.25 : 0.75);

        for (int i = 0; i < 3; i++)
        {
            double julianCenturies = GetJulianCenturiesFromJulianDay(julianDay + dateTime);
            double ratio = TanDegrees(GetSunDeclination(julianCenturies)) / TanDegrees(geoLocation.Latitude);

            if (double.IsNaN(ratio) || ratio > 1.0 || ratio < -1.0)
            { // Handle Tropics, the Poles, and Equator line divisions
                return double.NaN;
            }

            double offset = ((targetAzimuth == 90.0) ? -1.0 : 1.0) * (AcosDegrees(ratio) / 360.0);
            dateTime = solarNoonBase + offset - (GetEquationOfTime(julianCenturies) / 1440.0);
        }

        return (dateTime * 24 % 24 + 24) % 24;
    }
}
