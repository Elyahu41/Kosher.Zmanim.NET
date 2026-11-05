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
/// Implementation of sunrise and sunset methods using the <b>higher-accuracy</b> solar position algorithm of
/// <a href="https://en.wikipedia.org/wiki/Jean_Meeus">Jean Meeus</a> from
/// <a href="https://www.willbell.com/math/mc1.htm">Astronomical Algorithms</a> (2nd ed.). Unlike the <see cref="NOAACalculator"/> (which
/// uses the low-accuracy method from the same work, with a three-term equation of center and W. M. Smart's truncated equation of time,
/// good to roughly 0.01° / about 1 minute of time), this calculator computes the Earth's heliocentric position from the abridged <a
/// href="https://en.wikipedia.org/wiki/VSOP_(planets)">VSOP87</a> series printed in Meeus's Appendix III, then derives the Sun's
/// apparent geocentric longitude (applying the FK5 frame correction, nutation in longitude and the aberration of light), apparent
/// right ascension and declination, and the equation of time per Meeus chapter 28. The resulting solar position is good to roughly an
/// arc-second, an order of magnitude better than the low-accuracy method.
/// <para><b>Practical note on accuracy.</b> For horizon events (sunrise / sunset) the extra precision is largely masked by atmospheric
/// refraction (uncertain by arc-minutes with temperature and pressure) and is therefore typically worth only a few seconds over
/// <see cref="NOAACalculator"/>. The improvement is most visible for solar noon, the equation of time, and the Sun's elevation / azimuth
/// when it is well above the horizon.</para>
/// <para><b>Refraction and the Sun's radius are applied at sunrise and sunset only.</b> Like <see cref="NOAACalculator"/>, this class does not
/// model atmospheric refraction itself; the horizon corrections come entirely from the parent <see cref="AstronomicalCalculator.AdjustZenith"/>, which adds <see cref="AstronomicalCalculator.Refraction"/> (default 0.5667° / 34′), the solar radius - the date-based <see cref="AstronomicalCalculator.GetApparentSolarRadius"/> by default (it varies slightly day to day), or the fixed <see cref="AstronomicalCalculator.SolarRadius"/> (default
/// 0.26667° / 16′) when <see cref="AstronomicalCalculator.UseApparentSolarRadius"/> is <c>false</c> - and the <see cref="AstronomicalCalculator.GetElevationAdjustment">elevation dip</see>, but <em>only</em> when the requested zenith is exactly the geometric 90°, that is,
/// only for true sunrise and sunset. For every other zenith - twilight angles and depression-angle <em>zmanim</em> such as
/// <em>alos</em> and <em>tzais</em> (e.g. 16.1°, 19.8°) - no refraction or solar-radius term is added and the time is solved for the
/// Sun's true geometric depression, as those <em>zmanim</em> are defined geometrically. Consequently <see cref="AstronomicalCalculator.Refraction"/>
/// nd <see cref="AstronomicalCalculator.SolarRadius"/> affect sunrise / sunset (and any 90°-based time) but have no effect on depression-angle
/// <em>zmanim</em>. This calculator has no temperature or pressure inputs of any kind, since the only place they would matter is a
/// refraction model it does not use.</para>
/// <para><b>ΔT (terrestrial vs. universal time).</b> Meeus computes positions in Terrestrial Time (TT), whereas the civil times returned
/// here are Universal Time (UT ≈ UTC). This calculator therefore applies an estimate of <a href=
/// "https://en.wikipedia.org/wiki/%CE%94T_(timekeeping)">ΔT</a> (currently about a minute) when evaluating the Sun's position. This is
/// more correct than <see cref="NOAACalculator"/>, which omits ΔT. The correction can be disabled via <see cref="ApplyDeltaT"/> to
/// reproduce NOAA-style UT = TT behavior, which is useful for direct comparison.</para>
/// <para><b>Coefficient coverage.</b> The VSOP87 tables below are the abridged Earth series from Meeus Appendix III and the principal
/// (Meeus chapter 22 abridged) nutation terms. They yield sub-arc-second to a few arc-seconds of accuracy - far beyond what any
/// <em>zman</em> requires. For full textbook precision the term tables can simply be extended with the remaining Appendix III /
/// IAU 1980 entries; the algorithm itself does not change.</para>
/// </summary>
public class MeeusCalculator : AstronomicalCalculator
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
    /// Whether to apply the ΔT (TT - UT) correction when evaluating the Sun's position. Defaults to
    /// <c>true</c> (the astronomically correct behavior). Set to <c>false</c> to reproduce NOAA-style UT = TT
    /// results for direct comparison with <see cref="NOAACalculator"/>.
    /// </summary>
    /// <seealso cref="ApplyDeltaT"/>
    private bool applyDeltaT = true;

    /// <summary>
    /// An <c>enum</c> to indicate what type of solar event (<see cref="Sunrise">SUNRISE</see>, <see cref="Sunset">SUNSET</see>,
    /// <see cref="Noon">NOON</see> or <see cref="Midnight">MIDNIGHT</see>) is being calculated.
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
    /// Default constructor of the MeeusCalculator.
    /// </summary>
    public MeeusCalculator()
    {
    }

    /// <inheritdoc/>
    public override string CalculatorName => "Jean Meeus Higher-Accuracy (VSOP87) Algorithm";

    /// <summary>
    /// is the <see cref="ApplyDeltaT">ΔT</see> correction being applied.
    /// </summary>
    /// <value><c>true</c> to apply ΔT (default, astronomically correct), <c>false</c> to omit it and
    ///         reproduce NOAA-style UT = TT behavior.</value>
    /// <remarks>
    /// A method to enable or disable the <see cref="ApplyDeltaT">ΔT</see> (TT - UT) correction.
    /// </remarks>
    /// <seealso cref="ApplyDeltaT"/>
    public bool ApplyDeltaT
    {
        get => applyDeltaT;
        set
        {
            this.applyDeltaT = value;
            ClearResultCache();

        }
    }

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
    /// <param name="zenith">the azimuth below the vertical zenith of 90°, <see cref="AstronomicalCalculator.AdjustZenith">adjusted</see> for refraction, the
    ///         sun's radius and (optionally) elevation.</param>
    /// <param name="adjustForElevation">Should the time be adjusted for elevation.</param>
    /// <param name="solarEvent">if the calculation is for <see cref="SolarEvent.Sunrise"/> or <see cref="SolarEvent.Sunset"/>.</param>
    /// <returns>The UTC time of sunrise or sunset in 24-hour format. 5:45:00 AM will return 5.75, while 5:45 PM will return
    ///         17.75. If an error was encountered in the calculation (expected behavior for some locations such as near the
    ///         poles, <see cref="double.NaN"/> will be returned.</returns>
    /// <seealso cref="AstronomicalCalculator.GetElevationAdjustment"/>
    private double GetUTCSunRiseSet(DateOnly localDate, GeoLocation geoLocation, double zenith,
            bool adjustForElevation, SolarEvent solarEvent)
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
    /// Return the <a href="https://en.wikipedia.org/wiki/Universal_Coordinated_Time">Universal Coordinated Time</a> (UTC)
    /// of solar noon, or the upcoming midnight (about 12 hours after solar noon) of the given day at the given location, in
    /// minutes after 0:00 UTC. Uses the high-accuracy equation of time.
    /// </summary>
    /// <param name="julianDay">The Julian day (UT, for the start of the day).</param>
    /// <param name="longitude">The longitude of the observer in degrees (positive west, matching the value passed by the callers).</param>
    /// <param name="solarEvent">If the calculation is for <see cref="SolarEvent.Noon">NOON</see> or <see cref="SolarEvent.Midnight">MIDNIGHT</see>.</param>
    /// <returns>the time in minutes after 0:00 UTC.</returns>
    private double GetSolarNoonMidnightUTC(double julianDay, double longitude, SolarEvent solarEvent)
    {
        // no day-half shift: the loop epoch julianDay + solNoonUTC/1440 already lands on the event
        // First pass: seed includes the 720/1440-minute transit base so the equation of time is sampled near the true
        // transit (the NOAACalculator omits this base in its seed; here it is included for consistency with the loop).
        double baseMinutes = (solarEvent == SolarEvent.Noon) ? 720 : 1440;
        double tnoon = GetJulianCenturiesFromJulianDay(julianDay + longitude / 360.0);
        double equationOfTime = GetEquationOfTime(tnoon);
        double solNoonUTC = baseMinutes + (longitude * 4) - equationOfTime; // minutes

        // Refine the equation of time at the calculated transit time.
        for (int i = 0; i < 3; i++)
        {
            double newt = GetJulianCenturiesFromJulianDay(julianDay + solNoonUTC / 1440.0);
            equationOfTime = GetEquationOfTime(newt);
            solNoonUTC = baseMinutes + (longitude * 4) - equationOfTime;
        }
        return solNoonUTC;
    }

    /// <summary>
    /// The part of a rise / set solution that does not depend on the zenith: the equation of time and the declination at solar noon.
    /// Every zman of a day needs the same values, so the last ones calculated are kept in <c>LastNoonSolution</c> and reused. The
    /// key is every input they depend on (the Julian day, the longitude and <see cref="ApplyDeltaT"/>), so a cached value is only ever
    /// returned for identical inputs, and the results are bit-for-bit identical to calculating them each time. Immutable, so it can be
    /// shared between threads and clones.
    /// </summary>
    private sealed class NoonSolution
    {
        /// <summary>
        /// The Julian day (0h UT) of the date.
        /// </summary>
        internal double julianDay;
        /// <summary>
        /// The longitude in degrees, positive west.
        /// </summary>
        internal double longitude;
        /// <summary>
        /// The <see cref="MeeusCalculator.ApplyDeltaT"/> setting the values were calculated with.
        /// </summary>
        internal bool applyDeltaT;
        /// <summary>
        /// The equation of time at noon, in minutes.
        /// </summary>
        internal double equationOfTime;
        /// <summary>
        /// The solar declination at noon, in degrees.
        /// </summary>
        internal double solarDeclination;

        /// <summary>
        /// Creates a noon solution.
        /// </summary>
        /// <param name="julianDay">the Julian day (0h UT) of the date</param>
        /// <param name="longitude">the longitude in degrees, positive west</param>
        /// <param name="applyDeltaT">whether ΔT was applied</param>
        /// <param name="equationOfTime">the equation of time at noon, in minutes</param>
        /// <param name="solarDeclination">the solar declination at noon, in degrees</param>
        internal NoonSolution(double julianDay, double longitude, bool applyDeltaT, double equationOfTime, double solarDeclination)
        {
            this.julianDay = julianDay;
            this.longitude = longitude;
            this.applyDeltaT = applyDeltaT;
            this.equationOfTime = equationOfTime;
            this.solarDeclination = solarDeclination;
        }
    }

    /// <summary>
    /// The last <see cref="NoonSolution"/> calculated. A single entry (callers almost always move one day at a time), so memory use is fixed
    /// and nothing accumulates. Swapped atomically as one immutable object, so concurrent callers at most recalculate, and can never see a
    /// partial or mismatched entry. Not part of <see cref="Equals"/> or <see cref="HashCode"/>.
    /// </summary>
    private volatile NoonSolution? lastNoonSolution;

    /// <summary>
    /// Returns the noon solution for the day and longitude, reusing the last one when every input matches.
    /// </summary>
    /// <param name="julianDay">the Julian day</param>
    /// <param name="longitude">the longitude</param>
    /// <returns>the noon solution</returns>
    private NoonSolution GetNoonSolution(double julianDay, double longitude)
    {
        bool applyDeltaTNow = applyDeltaT; // read the setting once, so the key and the calculation use the same value
        NoonSolution? cached = lastNoonSolution;
        if (cached != null && cached.julianDay == julianDay && cached.longitude == longitude && cached.applyDeltaT == applyDeltaTNow)
        {
            return cached;
        }
        double noonmin = GetSolarNoonMidnightUTC(julianDay, longitude, SolarEvent.Noon);
        double tnoon = GetJulianCenturiesFromJulianDay(julianDay + noonmin / 1440.0);
        double[] sun = GetSunApparentEclipticCoordinates(tnoon); // shared by the equation of time and the declination
        NoonSolution solution = new NoonSolution(julianDay, longitude, applyDeltaTNow, GetEquationOfTime(tnoon, sun), GetSunDeclination(tnoon, sun));
        if (applyDeltaT == applyDeltaTNow)
        { // only cache if the setting did not change mid calculation
            lastNoonSolution = solution;
        }
        return solution;
    }

    /// <summary>
    /// Calculates sunrise or sunset in minutes after 0h UTC, starting from the cached noon solution and refining it with two
    /// further passes at the event time.
    /// </summary>
    /// <param name="localDate">the date</param>
    /// <param name="latitude">the latitude in degrees</param>
    /// <param name="longitude">the longitude in degrees, positive west</param>
    /// <param name="zenith">the zenith in degrees</param>
    /// <param name="solarEvent">sunrise or sunset</param>
    /// <returns>minutes after 0h UTC, or <see cref="double.NaN"/> if the sun does not reach the zenith that day</returns>
    private double GetSunRiseSetUTC(DateOnly localDate, double latitude, double longitude, double zenith,
            SolarEvent solarEvent)
    {
        double julianDay = GetJulianDay(localDate);

        // Use the declination at local solar noon as the starting point, then iterate.
        NoonSolution noon = GetNoonSolution(julianDay, longitude); // independent of the zenith, so shared by every zman of the day
        double equationOfTime = noon.equationOfTime;
        double solarDeclination = noon.solarDeclination;
        double hourAngle = GetSunHourAngle(latitude, solarDeclination, zenith, solarEvent);
        if (double.IsNaN(hourAngle))
        {
            // Using the Sun's declination at noon, the Sun never reaches this zenith today. But the declination changes
            // during the day (by up to about 0.2° in 12 hours), so the Sun may still just reach it around midnight, when it
            // is at its lowest. Before reporting that there is no event, try again with the declination and equation of time
            // at that solar midnight: the one before sunrise, or the one after sunset.
            double noonmin = GetSolarNoonMidnightUTC(julianDay, longitude, SolarEvent.Noon);
            double midnight = noonmin + (solarEvent == SolarEvent.Sunrise ? -720 : 720);
            double tmidnight = GetJulianCenturiesFromJulianDay(julianDay + midnight / 1440.0);
            equationOfTime = GetEquationOfTime(tmidnight);
            solarDeclination = GetSunDeclination(tmidnight);
            hourAngle = GetSunHourAngle(latitude, solarDeclination, zenith, solarEvent);
        }
        double delta = longitude - JavaMath.ToDegrees(hourAngle);
        double timeUTC = 720 + (4 * delta) - equationOfTime;

        // Additional passes including the fractional Julian Day for improved accuracy (a third pass over the NOAA
        // implementation's two helps convergence at high latitudes).
        for (int i = 0; i < 2; i++)
        {
            double newt = GetJulianCenturiesFromJulianDay(julianDay + timeUTC / 1440.0);
            double[] sun = GetSunApparentEclipticCoordinates(newt); // shared by the equation of time and the declination
            equationOfTime = GetEquationOfTime(newt, sun);
            solarDeclination = GetSunDeclination(newt, sun);
            hourAngle = GetSunHourAngle(latitude, solarDeclination, zenith, solarEvent);
            if (double.IsNaN(hourAngle))
            {
                return double.NaN;
            }
            delta = longitude - JavaMath.ToDegrees(hourAngle);
            timeUTC = 720 + (4 * delta) - equationOfTime;
        }
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
            { // Handle Tropics, Polar Regions, and Equator line divisions
                return double.NaN;
            }

            double offset = ((targetAzimuth == 90.0) ? -1.0 : 1.0) * (AcosDegrees(ratio) / 360.0);
            dateTime = solarNoonBase + offset - (GetEquationOfTime(julianCenturies) / 1440.0);
        }

        double timeUTC = dateTime * 24.0;
        return (timeUTC % 24 + 24) % 24; // normalize to [0,24) like the sibling UTC methods
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
    /// Return the <a href="https://en.wikipedia.org/wiki/Celestial_coordinate_system">solar elevation</a> or
    /// <a href="https://en.wikipedia.org/wiki/Celestial_coordinate_system">solar azimuth</a> at the given location and
    /// time, using the high-accuracy solar declination and equation of time. Elevation is sea-level based and corrected for
    /// atmospheric refraction; it is not adjusted for the observer's altitude.
    /// </summary>
    /// <param name="instant">date-time of calculation.</param>
    /// <param name="geoLocation">the location for calculating the elevation or azimuth.</param>
    /// <param name="isAzimuth"><c>true</c> for azimuth, <c>false</c> for elevation.</param>
    /// <returns>solar elevation or azimuth in degrees.</returns>
    private double GetSolarElevationAzimuth(DateTimeOffset instant, GeoLocation geoLocation, bool isAzimuth)
    {
        double lat = geoLocation.Latitude;
        double lon = geoLocation.Longitude;
        DateTimeOffset utc = instant.ToUniversalTime();
        double fractionalDay = (utc.Hour + (utc.Minute
                + (utc.Second + JavaTime.GetNano(utc) / 1_000_000_000.0) / 60.0) / 60.0) / 24.0;
        double jc = GetJulianCenturiesFromJulianDay(GetJulianDay(DateOnly.FromDateTime(utc.DateTime)) + fractionalDay);
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
    /// Apply an atmospheric refraction adjustment to the solar elevation (same model used by <see cref="NOAACalculator"/>).
    /// </summary>
    /// <param name="elevation">the elevation to adjust.</param>
    /// <returns>the refraction adjustment in degrees.</returns>
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

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/Hour_angle">hour angle</a> of the sun in radians for the given
    /// latitude, declination and zenith.
    /// </summary>
    /// <param name="latitude">the latitude of observer in degrees.</param>
    /// <param name="solarDeclination">the declination of the sun in degrees.</param>
    /// <param name="zenith">the (adjusted) zenith in degrees.</param>
    /// <param name="solarEvent"><see cref="SolarEvent.Sunrise"/> or <see cref="SolarEvent.Sunset"/>.</param>
    /// <returns>the hour angle in radians, or <see cref="double.NaN"/> if the sun does not reach the zenith on that day.</returns>
    private static double GetSunHourAngle(double latitude, double solarDeclination, double zenith, SolarEvent solarEvent)
    {
        double ratio = CosDegrees(zenith) / (CosDegrees(latitude) * CosDegrees(solarDeclination))
                - TanDegrees(latitude) * TanDegrees(solarDeclination);
        if (ratio < -1.0 || ratio > 1.0)
        {
            return double.NaN; // the sun never reaches this zenith (polar day / night)
        }
        double hourAngle = Math.Acos(ratio);
        if (solarEvent == SolarEvent.Sunset)
        {
            hourAngle = -hourAngle;
        }
        return hourAngle;
    }

    // ---------------------------------------------------------------------------------------------------------------
    // Time scales
    // ---------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/Julian_day">Julian day</a> for the start (0h UT) of the given
    /// date. Fractional days / time of day should be added later. (Meeus, chapter 7.)
    /// </summary>
    /// <param name="localDate">the <see cref="DateOnly"/>.</param>
    /// <returns>the Julian day for 0h UT of the date.</returns>
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
    /// Convert a Universal-Time <a href="https://en.wikipedia.org/wiki/Julian_day">Julian day</a> to Julian centuries
    /// since <a href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#J2000">J2000.0</a> in <b>Terrestrial Time</b>. When
    /// <see cref="ApplyDeltaT"/> is <c>true</c> the <see cref="GetDeltaT">ΔT</see> (TT - UT) offset is
    /// added so that the solar-position series, which are defined in TT, are evaluated correctly.
    /// </summary>
    /// <param name="julianDayUT">the UT Julian day to convert.</param>
    /// <returns>the Terrestrial-Time centuries since J2000.0.</returns>
    private double GetJulianCenturiesFromJulianDay(double julianDayUT)
    {
        double jde = julianDayUT;
        if (applyDeltaT)
        {
            jde += GetDeltaT(julianDayUT) / 86400.0; // ΔT is in seconds; convert to days
        }
        return (jde - JulianDayJan12000) / JulianDaysPerCentury;
    }

    /// <summary>
    /// Estimate <a href="https://en.wikipedia.org/wiki/%CE%94T_(timekeeping)">ΔT</a> = TT - UT in
    /// seconds for the given Julian day, using the polynomial expressions of Espenak and Meeus. The value is
    /// approximate (especially in the far past / future) but accurate to a few seconds for the modern era, which is well
    /// within the precision relevant here.
    /// </summary>
    /// <param name="julianDay">the (UT) Julian day.</param>
    /// <returns>ΔT in seconds.</returns>
    private static double GetDeltaT(double julianDay)
    {
        double year = 2000.0 + (julianDay - JulianDayJan12000) / 365.25;
        double t;
        if (year >= 2005 && year < 2050)
        {
            t = year - 2000;
            return 62.92 + 0.32217 * t + 0.005589 * t * t;
        }
        else if (year >= 1986 && year < 2005)
        {
            t = year - 2000;
            return 63.86 + 0.3345 * t - 0.060374 * t * t + 0.0017275 * t * t * t
                    + 0.000651814 * Math.Pow(t, 4) + 0.00002373599 * Math.Pow(t, 5);
        }
        else if (year >= 1961 && year < 1986)
        {
            t = year - 1975;
            return 45.45 + 1.067 * t - t * t / 260.0 - t * t * t / 718.0;
        }
        else if (year >= 2050 && year < 2150)
        {
            return -20 + 32 * Math.Pow((year - 1820) / 100.0, 2) - 0.5628 * (2150 - year);
        }
        else
        { // generic parabola of Espenak and Meeus for dates well outside the calibrated range
            double u = (year - 1820) / 100.0;
            return -20 + 32 * u * u;
        }
    }

    // ---------------------------------------------------------------------------------------------------------------
    // High-accuracy solar position (Meeus chapters 22, 25, 28 with abridged VSOP87, Appendix III)
    // ---------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Return the apparent <a href="https://en.wikipedia.org/wiki/Declination">declination</a> of the sun in degrees,
    /// computed from the apparent geocentric ecliptic coordinates derived from the VSOP87 series.
    /// </summary>
    /// <param name="julianCenturies">Terrestrial-Time centuries since J2000.0.</param>
    /// <returns>the sun's apparent declination in degrees.</returns>
    private static double GetSunDeclination(double julianCenturies)
    {
        return GetSunDeclination(julianCenturies, GetSunApparentEclipticCoordinates(julianCenturies));
    }

    /// <summary>
    /// <see cref="GetSunDeclination(double)"/> from already calculated apparent ecliptic coordinates.
    /// </summary>
    /// <param name="julianCenturies">the Julian centuries (TT)</param>
    /// <param name="sun"><see cref="GetSunApparentEclipticCoordinates"/> for the same time</param>
    /// <returns>the declination in degrees</returns>
    private static double GetSunDeclination(double julianCenturies, double[] sun)
    { // {lambda, beta} in degrees
        double lambda = sun[0];
        double beta = sun[1];
        double epsilon = GetTrueObliquity(julianCenturies);
        double sinDec = SinDegrees(beta) * CosDegrees(epsilon) + CosDegrees(beta) * SinDegrees(epsilon) * SinDegrees(lambda);
        return AsinDegrees(sinDec);
    }

    /// <summary>
    /// Calculates the sun's right ascension from already calculated apparent ecliptic coordinates.
    /// </summary>
    /// <param name="julianCenturies">the Julian centuries (TT)</param>
    /// <param name="sun"><see cref="GetSunApparentEclipticCoordinates"/> for the same time</param>
    /// <returns>the right ascension in degrees</returns>
    private static double GetSunRightAscension(double julianCenturies, double[] sun)
    {
        double lambda = sun[0];
        double beta = sun[1];
        double epsilon = GetTrueObliquity(julianCenturies);
        double y = SinDegrees(lambda) * CosDegrees(epsilon) - TanDegrees(beta) * SinDegrees(epsilon);
        double x = CosDegrees(lambda);
        double ra = JavaMath.ToDegrees(Math.Atan2(y, x));
        return (ra % 360 + 360) % 360;
    }

    /// <summary>
    /// Return the sun's <b>apparent</b> geocentric ecliptic longitude and latitude in degrees, built from the Earth's
    /// heliocentric VSOP87 position with the FK5 frame correction, nutation in longitude and the aberration of light
    /// applied (Meeus chapter 25).
    /// </summary>
    /// <param name="julianCenturies">Terrestrial-Time centuries since J2000.0.</param>
    /// <returns>a two-element array {apparent longitude λ, apparent latitude β} in degrees.</returns>
    private static double[] GetSunApparentEclipticCoordinates(double julianCenturies)
    {
        double tau = julianCenturies / 10.0; // Julian millennia
        double earthL = JavaMath.ToDegrees(SumSeries(EarthL, tau));   // Earth heliocentric longitude (deg)
        double earthB = JavaMath.ToDegrees(SumSeries(EarthB, tau));   // Earth heliocentric latitude (deg)
        double earthR = SumSeries(EarthR, tau);                   // Earth radius vector (AU)

        // Geocentric position of the Sun is diametrically opposite the heliocentric Earth.
        double theta = earthL + 180.0;       // geocentric longitude (deg), dynamical (VSOP87) frame
        double beta = -earthB;               // geocentric latitude (deg)

        // FK5 frame correction (Meeus 25.9): convert from the VSOP87 dynamical frame to FK5.
        double lambdaPrime = theta - 1.397 * julianCenturies - 0.00031 * julianCenturies * julianCenturies;
        double deltaLongFK5 = -0.09033 + 0.03916 * (CosDegrees(lambdaPrime) + SinDegrees(lambdaPrime)) * TanDegrees(beta);
        double deltaLatFK5 = 0.03916 * (CosDegrees(lambdaPrime) - SinDegrees(lambdaPrime));
        theta += deltaLongFK5 / 3600.0;
        beta += deltaLatFK5 / 3600.0;

        // Nutation in longitude (apparent place) and aberration of light.
        double[] nut = GetNutation(julianCenturies); // {Δψ, Δε} in arc-seconds
        double aberration = -20.4898 / earthR;        // arc-seconds (Meeus 25, abridged)
        double lambda = theta + nut[0] / 3600.0 + aberration / 3600.0;

        return new double[] { (lambda % 360 + 360) % 360, beta };
    }

    /// <summary>
    /// Return the true <a href="https://en.wikipedia.org/wiki/Axial_tilt">obliquity of the ecliptic</a> ε =
    /// ε<sub>0</sub> + Δε in degrees (mean obliquity, Meeus 22.2, plus nutation in
    /// obliquity).
    /// </summary>
    /// <param name="julianCenturies">Terrestrial-Time centuries since J2000.0.</param>
    /// <returns>the true obliquity in degrees.</returns>
    private static double GetTrueObliquity(double julianCenturies)
    {
        double seconds = 21.448 - julianCenturies
                * (46.8150 + julianCenturies * (0.00059 - julianCenturies * 0.001813));
        double epsilon0 = 23.0 + (26.0 + (seconds / 60.0)) / 60.0;
        double deltaEpsilon = GetNutation(julianCenturies)[1] / 3600.0;
        return epsilon0 + deltaEpsilon;
    }

    /// <summary>
    /// Return the principal terms of the nutation in longitude (Δψ) and obliquity (Δε) in
    /// arc-seconds (Meeus chapter 22, abridged). These dominant terms give sub-arc-second accuracy in the nutation, which
    /// is far beyond what is required here; the full IAU 1980 series can be substituted for textbook precision.
    /// </summary>
    /// <param name="julianCenturies">Terrestrial-Time centuries since J2000.0.</param>
    /// <returns>a two-element array {Δψ, Δε} in arc-seconds.</returns>
    private static double[] GetNutation(double julianCenturies)
    {
        double t = julianCenturies;
        double omega = 125.04452 - 1934.136261 * t + 0.0020708 * t * t + t * t * t / 450000.0; // moon's ascending node
        double lSun = 280.4665 + 36000.7698 * t;   // mean longitude of the sun
        double lMoon = 218.3165 + 481267.8813 * t; // mean longitude of the moon
        double deltaPsi = -17.20 * SinDegrees(omega) - 1.32 * SinDegrees(2 * lSun)
                - 0.23 * SinDegrees(2 * lMoon) + 0.21 * SinDegrees(2 * omega);
        double deltaEpsilon = 9.20 * CosDegrees(omega) + 0.57 * CosDegrees(2 * lSun)
                + 0.10 * CosDegrees(2 * lMoon) - 0.09 * CosDegrees(2 * omega);
        return new double[] { deltaPsi, deltaEpsilon };
    }

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/Equation_of_time">equation of time</a> - apparent minus
    /// mean solar time - in minutes of time, following Meeus chapter 28 (E = L<sub>0</sub> - 0.0057183°
    /// - α + Δψ·cos ε). The sign convention matches <see cref="NOAACalculator"/> so the
    /// rise/set formulas are interchangeable.
    /// </summary>
    /// <param name="julianCenturies">Terrestrial-Time centuries since J2000.0.</param>
    /// <returns>the equation of time in minutes of time.</returns>
    private static double GetEquationOfTime(double julianCenturies)
    {
        return GetEquationOfTime(julianCenturies, GetSunApparentEclipticCoordinates(julianCenturies));
    }

    /// <summary>
    /// <see cref="GetEquationOfTime(double)"/> from already calculated apparent ecliptic coordinates.
    /// </summary>
    /// <param name="julianCenturies">the Julian centuries (TT)</param>
    /// <param name="sun"><see cref="GetSunApparentEclipticCoordinates"/> for the same time</param>
    /// <returns>the equation of time in minutes</returns>
    private static double GetEquationOfTime(double julianCenturies, double[] sun)
    {
        double tau = julianCenturies / 10.0;
        // Mean longitude of the sun (Meeus 28.1), in degrees.
        double l0 = 280.4664567 + tau * (360007.6982779 + tau * (0.03032028
                + tau * (1.0 / 49931.0 - tau * (1.0 / 15300.0 + tau / 2000000.0))));
        l0 = (l0 % 360 + 360) % 360;
        double alpha = GetSunRightAscension(julianCenturies, sun);
        double[] nut = GetNutation(julianCenturies);
        double epsilon = GetTrueObliquity(julianCenturies);
        double e = l0 - 0.0057183 - alpha + (nut[0] / 3600.0) * CosDegrees(epsilon); // degrees
        // Reduce to (-180, 180] so the *4 conversion gives the customary +/- ~16 minute range.
        e = ((e + 180) % 360 + 360) % 360 - 180;
        return e * 4.0; // 1 degree = 4 minutes of time
    }

    /// <summary>
    /// Evaluate a VSOP87 series of the form Σ<sub>n</sub> (Σ<sub>i</sub> A<sub>i</sub>·cos(B<sub>i</sub>
    /// + C<sub>i</sub>·τ)) · τ<sup>n</sup>, where the coefficients are stored in 1e-8 of the series'
    /// native unit (radians for longitude/latitude, AU for the radius vector).
    /// </summary>
    /// <param name="series">the series, indexed as <c>series[n][term][{A, B, C}]</c>.</param>
    /// <param name="tau">the time in Julian millennia from J2000.0.</param>
    /// <returns>the evaluated quantity (radians or AU).</returns>
    private static double SumSeries(double[][][] series, double tau)
    {
        double total = 0.0;
        double tauPow = 1.0;
        foreach (double[][] terms in series)
        {
            double sum = 0.0;
            foreach (double[] term in terms)
            {
                sum += term[0] * Math.Cos(term[1] + term[2] * tau);
            }
            total += sum * tauPow;
            tauPow *= tau;
        }
        return total / 1.0e8;
    }

    /// <summary>
    /// Earth heliocentric longitude series L0..L5. Abridged VSOP87 Earth series (Meeus, Astronomical Algorithms 2nd ed., Appendix
    /// III). Each term is {A, B, C}; the series value is Σ A·cos(B + C·τ), τ in Julian millennia, result in 1e-8 rad (L, B) or
    /// 1e-8 AU (R).
    /// </summary>
    private static readonly double[][][] EarthL = {
        new double[][] { // L0
            new double[] {175347046, 0, 0}, new double[] {3341656, 4.6692568, 6283.07585}, new double[] {34894, 4.6261, 12566.1517},
            new double[] {3497, 2.7441, 5753.3849}, new double[] {3418, 2.8289, 3.5231}, new double[] {3136, 3.6277, 77713.7715},
            new double[] {2676, 4.4181, 7860.4194}, new double[] {2343, 6.1352, 3930.2097}, new double[] {1324, 0.7425, 11506.7698},
            new double[] {1273, 2.0371, 529.6910}, new double[] {1199, 1.1096, 1577.3435}, new double[] {990, 5.233, 5884.927},
            new double[] {902, 2.045, 26.298}, new double[] {857, 3.508, 398.149}, new double[] {780, 1.179, 5223.694},
            new double[] {753, 2.533, 5507.553}, new double[] {505, 4.583, 18849.228}, new double[] {492, 4.205, 775.523},
            new double[] {357, 2.920, 0.067}, new double[] {317, 5.849, 11790.629}, new double[] {284, 1.899, 796.298},
            new double[] {271, 0.315, 10977.079}, new double[] {243, 0.345, 5486.778}, new double[] {206, 4.806, 2544.314},
            new double[] {205, 1.869, 5573.143}, new double[] {202, 2.458, 6069.777}, new double[] {156, 0.833, 213.299},
            new double[] {132, 3.411, 2942.463}, new double[] {126, 1.083, 20.775}, new double[] {115, 0.645, 0.980},
            new double[] {103, 0.636, 4694.003}, new double[] {102, 0.976, 15720.839}, new double[] {102, 4.267, 7.114},
            new double[] {99, 6.21, 2146.17}, new double[] {98, 0.68, 155.42}, new double[] {86, 5.98, 161000.69},
            new double[] {85, 1.30, 6275.96}, new double[] {85, 3.67, 71430.70}, new double[] {80, 1.81, 17260.15},
            new double[] {79, 3.04, 12036.46}, new double[] {75, 1.76, 5088.63}, new double[] {74, 3.50, 3154.69},
            new double[] {74, 4.68, 801.82}, new double[] {70, 0.83, 9437.76}, new double[] {62, 3.98, 8827.39},
            new double[] {61, 1.82, 7084.90}, new double[] {57, 2.78, 6286.60}, new double[] {56, 4.39, 14143.50},
            new double[] {56, 3.47, 6279.55}, new double[] {52, 0.19, 12139.55}, new double[] {52, 1.33, 1748.02},
            new double[] {51, 0.28, 5856.48}, new double[] {49, 0.49, 1194.45}, new double[] {41, 5.37, 8429.24},
            new double[] {41, 2.40, 19651.05}, new double[] {39, 6.17, 10447.39}, new double[] {37, 6.04, 10213.29},
            new double[] {37, 2.57, 1059.38}, new double[] {36, 1.71, 2352.87}, new double[] {36, 1.78, 6812.77},
            new double[] {33, 0.59, 17789.85}, new double[] {30, 0.44, 83996.85}, new double[] {30, 2.74, 1349.87},
            new double[] {25, 3.16, 4690.48}
        },
        new double[][] { // L1
            new double[] {628331966747.0, 0, 0}, new double[] {206059, 2.678235, 6283.07585}, new double[] {4303, 2.6351, 12566.1517},
            new double[] {425, 1.590, 3.523}, new double[] {119, 5.796, 26.298}, new double[] {109, 2.966, 1577.344},
            new double[] {93, 2.59, 18849.23}, new double[] {72, 1.14, 529.69}, new double[] {68, 1.87, 398.15},
            new double[] {67, 4.41, 5507.55}, new double[] {59, 2.89, 5223.69}, new double[] {56, 2.17, 155.42},
            new double[] {45, 0.40, 796.30}, new double[] {36, 0.47, 775.52}, new double[] {29, 2.65, 7.11},
            new double[] {21, 5.34, 0.98}, new double[] {19, 1.85, 5486.78}, new double[] {19, 4.97, 213.30},
            new double[] {17, 2.99, 6275.96}, new double[] {16, 0.03, 2544.31}, new double[] {16, 1.43, 2146.17},
            new double[] {15, 1.21, 10977.08}, new double[] {12, 2.83, 1748.02}, new double[] {12, 3.26, 5088.63},
            new double[] {12, 5.27, 1194.45}, new double[] {12, 2.08, 4694.00}, new double[] {11, 0.77, 553.57},
            new double[] {10, 1.30, 6286.60}, new double[] {10, 4.24, 1349.87}, new double[] {9, 2.70, 242.73},
            new double[] {9, 5.64, 951.72}, new double[] {8, 5.30, 2352.87}, new double[] {6, 2.65, 9437.76},
            new double[] {6, 4.67, 4690.48}
        },
        new double[][] { // L2
            new double[] {52919, 0, 0}, new double[] {8720, 1.0721, 6283.0758}, new double[] {309, 0.867, 12566.152},
            new double[] {27, 0.05, 3.52}, new double[] {16, 5.19, 26.30}, new double[] {16, 3.68, 155.42},
            new double[] {10, 0.76, 18849.23}, new double[] {9, 2.06, 77713.77}, new double[] {7, 0.83, 775.52},
            new double[] {5, 4.66, 1577.34}, new double[] {4, 1.03, 7.11}, new double[] {4, 3.44, 5573.14},
            new double[] {3, 5.14, 796.30}, new double[] {3, 6.05, 5507.55}, new double[] {3, 1.19, 242.73},
            new double[] {3, 6.12, 529.69}, new double[] {3, 0.31, 398.15}, new double[] {3, 2.28, 553.57},
            new double[] {2, 4.38, 5223.69}, new double[] {2, 3.75, 0.98}
        },
        new double[][] { // L3
            new double[] {289, 5.844, 6283.076}, new double[] {35, 0, 0}, new double[] {17, 5.49, 12566.15},
            new double[] {3, 5.20, 155.42}, new double[] {1, 4.72, 3.52}, new double[] {1, 5.30, 18849.23}, new double[] {1, 5.97, 242.73}
        },
        new double[][] { // L4
            new double[] {114, 3.142, 0}, new double[] {8, 4.13, 6283.08}, new double[] {1, 3.84, 12566.15}
        },
        new double[][] { // L5
            new double[] {1, 3.14, 0}
        }
    };

    /// <summary>
    /// Earth heliocentric latitude series B0..B1.
    /// </summary>
    private static readonly double[][][] EarthB = {
        new double[][] { // B0
            new double[] {280, 3.199, 84334.662}, new double[] {102, 5.422, 5507.553}, new double[] {80, 3.88, 5223.69},
            new double[] {44, 3.70, 2352.87}, new double[] {32, 4.00, 1577.34}
        },
        new double[][] { // B1
            new double[] {9, 3.90, 5507.55}, new double[] {6, 1.73, 5223.69}
        }
    };

    /// <summary>
    /// Earth radius-vector series R0..R4 (AU).
    /// </summary>
    private static readonly double[][][] EarthR = {
        new double[][] { // R0
            new double[] {100013989, 0, 0}, new double[] {1670700, 3.0984635, 6283.07585}, new double[] {13956, 3.05525, 12566.1517},
            new double[] {3084, 5.1985, 77713.7715}, new double[] {1628, 1.1739, 5753.3849}, new double[] {1576, 2.8469, 7860.4194},
            new double[] {925, 5.453, 11506.770}, new double[] {542, 4.564, 3930.210}, new double[] {472, 3.661, 5884.927},
            new double[] {346, 0.964, 5507.553}, new double[] {329, 5.900, 5223.694}, new double[] {307, 0.299, 5573.143},
            new double[] {243, 4.273, 11790.629}, new double[] {212, 5.847, 1577.344}, new double[] {186, 5.022, 10977.079},
            new double[] {175, 3.012, 18849.228}, new double[] {110, 5.055, 5486.778}, new double[] {98, 0.89, 6069.78},
            new double[] {86, 5.69, 15720.84}, new double[] {86, 1.27, 161000.69}, new double[] {65, 0.27, 17260.15},
            new double[] {63, 0.92, 529.69}, new double[] {57, 2.01, 83996.85}, new double[] {56, 5.24, 71430.70},
            new double[] {49, 3.25, 2544.31}, new double[] {47, 2.58, 775.52}, new double[] {45, 5.54, 9437.76},
            new double[] {43, 6.01, 6275.96}, new double[] {39, 5.36, 4694.00}, new double[] {38, 2.39, 8827.39},
            new double[] {37, 0.83, 19651.05}, new double[] {37, 4.90, 12139.55}, new double[] {36, 1.67, 12036.46},
            new double[] {35, 1.84, 2942.46}, new double[] {33, 0.24, 7084.90}, new double[] {32, 0.18, 5088.63},
            new double[] {32, 1.78, 398.15}, new double[] {28, 1.21, 6286.60}, new double[] {28, 1.90, 6279.55},
            new double[] {26, 4.59, 10447.39}
        },
        new double[][] { // R1
            new double[] {103019, 1.10749, 6283.07585}, new double[] {1721, 1.0644, 12566.1517}, new double[] {702, 3.142, 0},
            new double[] {32, 1.02, 18849.23}, new double[] {31, 2.84, 5507.55}, new double[] {25, 1.32, 5223.69},
            new double[] {18, 1.42, 1577.34}, new double[] {10, 5.91, 10977.08}, new double[] {9, 1.42, 6275.96}, new double[] {9, 0.27, 5486.78}
        },
        new double[][] { // R2
            new double[] {4359, 5.7846, 6283.0758}, new double[] {124, 5.579, 12566.152}, new double[] {12, 3.14, 0},
            new double[] {9, 3.63, 77713.77}, new double[] {6, 1.87, 5573.14}, new double[] {3, 5.47, 18849.23}
        },
        new double[][] { // R3
            new double[] {145, 4.273, 6283.076}, new double[] {7, 3.92, 12566.15}
        },
        new double[][] { // R4
            new double[] {4, 2.56, 6283.08}
        }
    };


    /// <summary>
    /// Two <c>MeeusCalculator</c> instances are considered equal if their <see cref="ApplyDeltaT"/> value is identical.
    /// </summary>
    /// <param name="obj">the reference object with which to compare</param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (!base.Equals(obj))
        {
            return false;
        }
        return this.applyDeltaT == ((MeeusCalculator)obj).applyDeltaT;
    }

    /// <summary>
    /// This implementation hashes the <c>applyDeltaT</c> property to maintain the contract with <see cref="Equals"/>.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), applyDeltaT);
    }
}
