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
/// Implementation of solar times using the <a href="https://midcdmz.nrel.gov/spa/">Solar Position Algorithm (SPA)</a>. This was the
/// work of Ibrahim Reda and Afshin Andreas (NREL/TP-560-34302, <a href="https://doi.org/10.1016/j.solener.2003.12.003">Solar Energy
/// 76 (2004) 577-589</a>). SPA computes the Earth's heliocentric position from the <a href=
/// "https://en.wikipedia.org/wiki/VSOP_(planets)">VSOP87</a> series, applies nutation, aberration and the obliquity of the ecliptic
/// to obtain the Sun's apparent geocentric right ascension and declination, then performs the full <b>topocentric</b> reduction -
/// observer parallax (using latitude, longitude and elevation) and atmospheric refraction - to yield the observed topocentric zenith
/// and azimuth. Its stated uncertainty is ±0.0003° (about one arc-second) for the years −2000 to +6000.
/// <para><b>Refraction is fully configurable through the parent <see cref="AstronomicalCalculator"/>.</b> Many SPA ports hardcode their
/// refraction model (and even their solar radius); this one does not:
/// <list type="bullet">
/// <item><description><b>Sunrise / sunset (the geometric 90° zenith only).</b> The horizon geometry is taken entirely from the parent's <see cref="AstronomicalCalculator.AdjustZenith"/>: <see cref="AstronomicalCalculator.Refraction"/> / <see cref="AstronomicalCalculator.Refraction"/> (default 0.5667° / 34′);
///     the solar radius - the date-based <see cref="AstronomicalCalculator.GetApparentSolarRadius"/> by default (it varies slightly day to day), or the
///     fixed <see cref="AstronomicalCalculator.SolarRadius"/> (default 0.26667° / 16′) when <see cref="AstronomicalCalculator.UseApparentSolarRadius"/> is <c>false</c>;
///     and the <see cref="AstronomicalCalculator.GetElevationAdjustment">elevation dip</see>. SPA's own atmospheric refraction is <em>not</em> applied on this
///     path - the calculator solves for the geometric (parallax-included, un-refracted) center of the Sun reaching the parent-supplied
///     zenith, exactly as <see cref="NOAACalculator"/> and <see cref="MeeusCalculator"/> do. The parent only adds these corrections when the
///     zenith is exactly 90°, so <see cref="AstronomicalCalculator.Refraction"/> and <see cref="AstronomicalCalculator.SolarRadius"/> change sunrise / sunset (and any
///     90°-based time) and nothing else.</description></item>
/// <item><description><b>Twilight and depression-angle <em>zmanim</em> (e.g. <em>alos</em> / <em>tzais</em> at 16.1°, 19.8°).</b> These pass a zenith
///     other than 90°, so <see cref="AstronomicalCalculator.AdjustZenith"/> adds nothing: no refraction, no solar radius. The time is
///     solved for the Sun's <em>true geometric</em> depression, as such <em>zmanim</em> are defined. None of <see cref="AstronomicalCalculator.Refraction"/>, <see cref="AstronomicalCalculator.SolarRadius"/>, <see cref="Pressure"/> or <see cref="Temperature"/>
///     affect them.</description></item>
/// <item><description><b>Instantaneous <see cref="GetSolarElevation">elevation</see> / <see cref="GetSolarAzimuth">azimuth</see> only.</b> Here the full SPA (Reda-Andreas / Sæmundsson) atmospheric-refraction model is used, with its horizon
///     parameter taken from the parent's <see cref="AstronomicalCalculator.Refraction"/> and the <see cref="Pressure">pressure</see> and <see cref="Temperature">temperature</see> settable. <b>These two settings affect this path and only this path</b> - they have no
///     effect on sunrise, sunset, twilight or any depression-angle <em>zman</em>. (Note that within the KosherJava calendar nothing
///     calls  <see cref="GetSolarElevation"/> / <see cref="GetSolarAzimuth"/>; they are direct-query
///     methods, so in normal calendar use the pressure and temperature settings are inert.)</description></item>
/// </list></para>
/// <para><b>ΔT</b> (TT − UT) is integral to SPA and is applied here. It defaults to an Espenak-Meeus estimate but can be overridden
/// exactly via <see cref="DeltaTOverride"/> (the published SPA test case fixes ΔT = 67 seconds), or disabled via <see cref="ApplyDeltaT"/>.</para>
/// <para><b>Topocentric note.</b> Unlike <see cref="NOAACalculator.GetSolarElevation"/> (sea-level, altitude-independent),
/// this calculator's elevation / azimuth are genuinely topocentric: the observer's <see cref="GeoLocation.Elevation">elevation</see> feeds
/// the parallax term. The <c>adjustForElevation</c> flag on sunrise / sunset continues to control only the geometric horizon
/// <em>dip</em> via the parent.</para>
/// <para><b>Coefficient coverage.</b> The VSOP87 Earth tables are the abridged Appendix III set (validated to about 0.1′ against Meeus's
/// worked example), and the nutation uses the principal terms. These give a few arc-seconds of accuracy - vastly beyond any
/// <em>zmanim</em> requirements. For the full published SPA ±0.0003° envelope, the VSOP87 tables and the IAU 1980 nutation series can
/// be extended without changing the algorithm.</para>
/// </summary>
public class SPACalculator : AstronomicalCalculator
{

    /// <summary>
    /// This calculator's results depend only on the arguments and on settings whose setters clear the cache.
    /// </summary>
    protected override bool IsResultCacheable()
    {
        return true;
    }

    /// <summary>
    /// The <a href="https://en.wikipedia.org/wiki/Julian_day">Julian day</a> of J2000.0.
    /// </summary>
    private const double JulianDayJan12000 = 2451545.0;

    /// <summary>
    /// Julian days per century.
    /// </summary>
    private const double JulianDaysPerCentury = 36525.0;

    /// <summary>
    /// Whether to apply the ΔT (TT−UT) correction. Defaults to <c>true</c>.
    /// </summary>
    private bool applyDeltaT = true;

    /// <summary>
    /// An explicit ΔT (seconds) override; <c>null</c> means estimate it from the date.
    /// </summary>
    private double? deltaTOverride = null;

    /// <summary>
    /// Annual average local pressure in millibars, used by the SPA refraction model. Default 1013.25.
    /// </summary>
    private double pressure = 1013.25;

    /// <summary>
    /// Annual average local temperature in °C, used by the SPA refraction model. Default 10.
    /// </summary>
    private double temperature = 10.0;

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
    /// The part of a rise / set solution that does not depend on the zenith: the declination and the equation of time at solar
    /// noon. Every zman of a day needs the same values, so the last one calculated is kept in  and reused.
    /// The key is every input the values depend on (the Julian day, the longitude and the two ΔT settings), so a cached value is
    /// only ever returned for identical inputs, and the results are bit-for-bit identical to calculating them each time. Immutable, so
    /// it can be shared between threads and clones.
    /// </summary>
    private sealed class NoonSolution
    {
        /// <summary>
        /// The Julian day (0h UT) of the date.
        /// </summary>
        internal double jdDay;
        /// <summary>
        /// The longitude in degrees, positive west.
        /// </summary>
        internal double lonWest;
        /// <summary>
        /// The <see cref="SPACalculator.ApplyDeltaT"/> setting the values were calculated with.
        /// </summary>
        internal bool applyDeltaT;
        /// <summary>
        /// The ΔT override the values were calculated with, or null for the estimate.
        /// </summary>
        internal double? deltaTOverride;
        /// <summary>
        /// The solar declination at noon, in degrees.
        /// </summary>
        internal double declNoon;
        /// <summary>
        /// The equation of time at noon, in minutes.
        /// </summary>
        internal double eot;

        /// <summary>
        /// Creates a noon solution.
        /// </summary>
        /// <param name="jdDay">the Julian day (0h UT) of the date</param>
        /// <param name="lonWest">the longitude in degrees, positive west</param>
        /// <param name="applyDeltaT">whether ΔT was applied</param>
        /// <param name="deltaTOverride">the ΔT override, or null</param>
        /// <param name="declNoon">the solar declination at noon, in degrees</param>
        /// <param name="eot">the equation of time at noon, in minutes</param>
        internal NoonSolution(double jdDay, double lonWest, bool applyDeltaT, double? deltaTOverride, double declNoon, double eot)
        {
            this.jdDay = jdDay;
            this.lonWest = lonWest;
            this.applyDeltaT = applyDeltaT;
            this.deltaTOverride = deltaTOverride;
            this.declNoon = declNoon;
            this.eot = eot;
        }

        /// <summary>
        /// Whether this solution was calculated for exactly these inputs.
        /// </summary>
        /// <param name="jdDay">the Julian day (0h UT) of the date</param>
        /// <param name="lonWest">the longitude in degrees, positive west</param>
        /// <param name="applyDeltaT">whether ΔT is applied</param>
        /// <param name="deltaTOverride">the ΔT override, or null</param>
        /// <returns>true if every input matches</returns>
        internal bool Matches(double jdDay, double lonWest, bool applyDeltaT, double? deltaTOverride)
        {
            return this.jdDay == jdDay && this.lonWest == lonWest && this.applyDeltaT == applyDeltaT
                    && Equals(this.deltaTOverride, deltaTOverride);
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
    /// <param name="jdDay">the Julian day</param>
    /// <param name="lonWest">the longitude, positive west</param>
    /// <returns>the noon solution</returns>
    private NoonSolution GetNoonSolution(double jdDay, double lonWest)
    {
        bool applyDeltaTNow = applyDeltaT; // read the settings once, so the key and the calculation use the same values
        double? deltaTOverrideNow = deltaTOverride;
        NoonSolution? cached = lastNoonSolution;
        if (cached != null && cached.Matches(jdDay, lonWest, applyDeltaTNow, deltaTOverrideNow))
        {
            return cached;
        }
        double noonMin = SolveNoonMidnight(jdDay, lonWest, SolarEvent.Noon);
        double declNoon = SolarCoords(jdDay + noonMin / 1440.0)[1];
        double eot = EquationOfTime(jdDay + noonMin / 1440.0);
        NoonSolution solution = new NoonSolution(jdDay, lonWest, applyDeltaTNow, deltaTOverrideNow, declNoon, eot);
        if (applyDeltaT == applyDeltaTNow && Equals(deltaTOverride, deltaTOverrideNow))
        { // only cache if the settings did not change mid calculation
            lastNoonSolution = solution;
        }
        return solution;
    }

    /// <summary>
    /// Default constructor of the SPACalculator.
    /// </summary>
    public SPACalculator()
    {
    }

    /// <inheritdoc/>
    public override string CalculatorName => "NREL Solar Position Algorithm";

    /// <summary>
    /// Returns whether the ΔT correction is being applied.
    /// </summary>
    /// <value><c>true</c> (default) to apply ΔT, <c>false</c> to omit it (UT = TT).</value>
    /// <remarks>
    /// Sets to apply ΔT <c>true</c> (default), or <c>false</c> to omit it (UT = TT).
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

    /// <summary>
    /// Return the explicit ΔT (TT - UT) override in seconds, or <c>null</c> if ΔT is being estimated from the
    /// date (the default). Note that this returns the override setting, not the estimated value in use.
    /// </summary>
    /// <value>ΔT in seconds, or <c>null</c> to estimate.</value>
    /// <remarks>
    /// Override the estimated ΔT with an explicit value in seconds (for example 67 seconds for the published SPA test
    /// case), or <c>null</c> to revert to the date-based estimate.
    /// </remarks>
    /// <seealso cref="DeltaTOverride"/>
    public double? DeltaTOverride
    {
        get => deltaTOverride;
        set
        {
            this.deltaTOverride = value;
            ClearResultCache();

        }
    }

    /// <summary>
    /// Return the pressure in millibars used by the refraction model.
    /// </summary>
    /// <value>the pressure in millibars (hPa).</value>
    /// <remarks>
    /// Set the annual average local pressure used by the SPA atmospheric-refraction model. <b>This affects only
    /// <see cref="GetSolarElevation"/> and <see cref="GetSolarAzimuth"/></b>; it has no
    /// effect on sunrise, sunset, twilight or any depression-angle <em>zman</em>, whose refraction (if any) comes from the
    /// parent's fixed <see cref="AstronomicalCalculator.Refraction"/>. See the class documentation.
    /// </remarks>
    public double Pressure
    {
        get => pressure;
        set
        {
            this.pressure = value;
            ClearResultCache();

        }
    }

    /// <summary>
    /// Returns the temperature in °C used by the refraction model.
    /// </summary>
    /// <value>the temperature in degrees Celsius.</value>
    /// <remarks>
    /// Set the annual average local temperature used by the SPA atmospheric-refraction model. <b>This affects only
    /// <see cref="GetSolarElevation"/> and <see cref="GetSolarAzimuth"/></b>; it has no
    /// effect on sunrise, sunset, twilight or any depression-angle <em>zman</em>, whose refraction (if any) comes from the
    /// parent's fixed <see cref="AstronomicalCalculator.Refraction"/>. See the class documentation.
    /// </remarks>
    public double Temperature
    {
        get => temperature;
        set
        {
            this.temperature = value;
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
    /// Calculate UTC sunrise or sunset (or any twilight angle) in 24-hour format. The horizon geometry (refraction, the sun's radius
    /// and the elevation dip) is supplied entirely by the parent <see cref="AstronomicalCalculator.AdjustZenith"/>, so the SPA
    /// topocentric solution is solved for the un-refracted, parallax-corrected center of the Sun reaching that zenith.
    /// </summary>
    /// <param name="localDate">the date.</param>
    /// <param name="geoLocation">the observer location.</param>
    /// <param name="zenith">the (un-adjusted) zenith; <see cref="AstronomicalCalculator.AdjustZenith">adjusted</see> by the parent before use.</param>
    /// <param name="adjustForElevation">whether to include the geometric elevation dip.</param>
    /// <param name="solarEvent"><see cref="SolarEvent.Sunrise"/> or <see cref="SolarEvent.Sunset"/>.</param>
    /// <returns>the UTC time in hours, or <see cref="double.NaN"/> if the event does not occur (polar day / night).</returns>
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
        double riseSet = SolveRiseSet(localDate, geoLocation, adjustedZenith, solarEvent);
        if (double.IsNaN(riseSet))
        {
            return double.NaN;
        }
        riseSet = riseSet / 60;
        return (riseSet % 24 + 24) % 24;
    }

    /// <summary>
    /// Solve for the UTC time, in minutes after 0:00, at which the topocentric (parallax-corrected, un-refracted) center of
    /// the Sun reaches <c>adjustedZenith</c>. A first guess is taken from the closed-form hour-angle equation (geocentric
    /// declination at local noon), then refined by a secant iteration on the full SPA topocentric zenith.
    /// </summary>
    /// <param name="localDate">the date.</param>
    /// <param name="geoLocation">the observer location.</param>
    /// <param name="adjustedZenith">the target zenith (already including refraction / radius / dip from the parent).</param>
    /// <param name="solarEvent"><see cref="SolarEvent.Sunrise"/> or <see cref="SolarEvent.Sunset"/>.</param>
    /// <returns>UTC minutes after 0:00, or <see cref="double.NaN"/> if the event does not occur.</returns>
    private double SolveRiseSet(DateOnly localDate, GeoLocation geoLocation, double adjustedZenith, SolarEvent solarEvent)
    {
        double jdDay = GetJulianDay(localDate);
        double lonWest = -geoLocation.Longitude;
        double lat = geoLocation.Latitude;
        double elevation = geoLocation.Elevation;

        // Closed-form first guess using the geocentric declination at local apparent noon.
        NoonSolution noon = GetNoonSolution(jdDay, lonWest); // independent of the zenith, so shared by every zman of the day
        double declNoon = noon.declNoon;
        double eot = noon.eot;
        double cosH0 = (CosDegrees(adjustedZenith) - SinDegrees(lat) * SinDegrees(declNoon))
                / (CosDegrees(lat) * CosDegrees(declNoon));
        if (cosH0 < -1.0 || cosH0 > 1.0)
        {
            return BracketRiseSet(jdDay, lat, geoLocation.Longitude, elevation, adjustedZenith, solarEvent); // the estimate says no event: let the exact search decide
        }
        double h0 = AcosDegrees(cosH0); // degrees, positive
        double signed = (solarEvent == SolarEvent.Sunrise) ? h0 : -h0;
        double guess = 720 + 4 * (lonWest - signed) - eot; // minutes

        // Fast path: the secant on interpolated solar coordinates, accepted only after it is verified exactly and lies in the window.
        double[] stats = new double[2];
        double ti = Secant(jdDay, lat, geoLocation.Longitude, elevation, adjustedZenith, guess, true, stats);
        double[] fastWindow = GetRiseSetWindowMinutes(geoLocation.Longitude, solarEvent);
        bool fastInWindow = solarEvent == SolarEvent.Sunrise ? ti > fastWindow[0] && ti <= fastWindow[1] : ti >= fastWindow[0] && ti < fastWindow[1];
        if (fastInWindow && IsInterpolatedSolutionAcceptable(ti, stats, jdDay, lat, geoLocation.Longitude, elevation, adjustedZenith))
        {
            return ti;
        }

        // Exact path: secant refinement on f(t) = topocentricTrueZenith(t) - adjustedZenith.
        double t0 = guess;
        double t1 = guess + 0.5;
        double f0 = TopocentricTrueZenith(jdDay + t0 / 1440.0, lat, geoLocation.Longitude, elevation) - adjustedZenith;
        double f1 = TopocentricTrueZenith(jdDay + t1 / 1440.0, lat, geoLocation.Longitude, elevation) - adjustedZenith;
        double minSlope = Math.Abs((f1 - f0) / (t1 - t0)); // degrees per minute
        for (int i = 0; i < 12 && Math.Abs(f1) > 1e-9; i++)
        {
            double denom = (f1 - f0);
            if (denom == 0)
            {
                break;
            }
            minSlope = Math.Min(minSlope, Math.Abs(denom / (t1 - t0)));
            double t2 = t1 - f1 * (t1 - t0) / denom;
            t0 = t1; f0 = f1;
            t1 = t2;
            f1 = TopocentricTrueZenith(jdDay + t1 / 1440.0, lat, geoLocation.Longitude, elevation) - adjustedZenith;
        }
        // Accept the secant result only if it converged onto the requested zenith inside this date's window. Otherwise (no
        // convergence near the poles or on a grazing day, or a crossing belonging to a neighbouring date) use the bracketing search.
        double[] window = GetRiseSetWindowMinutes(geoLocation.Longitude, solarEvent);
        bool inWindow = solarEvent == SolarEvent.Sunrise ? t1 > window[0] && t1 <= window[1] : t1 >= window[0] && t1 < window[1];
        // A slow crossing anywhere along the path means a near-grazing day, where the secant can settle on a near miss (the closest
        // approach to the zenith) or on another crossing; the bracketing search decides those days.
        if (inWindow && Math.Abs(f1) <= MaxZenithResidual && minSlope >= GrazingMinSlope)
        {
            return t1;
        }
        return BracketRiseSet(jdDay, lat, geoLocation.Longitude, elevation, adjustedZenith, solarEvent);
    }

    /// <summary>
    /// Verifies a rise / set time found with the interpolated solar coordinates. It is accepted only if the search was well conditioned
    /// all along its path (the sun crossed the zenith at no less than <see cref="GuardMinSlope"/>), so it converged to the same crossing the
    /// exact solve would, and if the exact zenith at that time is within <see cref="GuardToleranceMinutes"/> of the target. Otherwise the
    /// caller repeats the solve exactly.
    /// </summary>
    /// <param name="t">the time found, in minutes from 0h UT of the date</param>
    /// <param name="stats">the path statistics from <see cref="Secant"/>: {minimum |slope|, last slope}, in degrees per minute</param>
    /// <param name="jdDay">the Julian day (0h UT) of the date</param>
    /// <param name="lat">the latitude in degrees</param>
    /// <param name="longitude">the longitude in degrees</param>
    /// <param name="elevation">the elevation in meters</param>
    /// <param name="adjustedZenith">the target zenith in degrees</param>
    /// <returns>true if the interpolated solution can be used, false if the solve must be repeated exactly</returns>
    private bool IsInterpolatedSolutionAcceptable(double t, double[] stats, double jdDay, double lat, double longitude, double elevation,
            double adjustedZenith)
    {
        if (double.IsNaN(t) || double.IsInfinity(t) || !(stats[0] >= GuardMinSlope))
        {
            return false;
        }
        double exactResidual = TopocentricTrueZenith(jdDay + t / 1440.0, lat, longitude, elevation, false) - adjustedZenith;
        return Math.Abs(exactResidual / stats[1]) <= GuardToleranceMinutes;
    }

    /// <summary>
    /// The secant search for the time (minutes from 0h UT) at which the topocentric true zenith equals the adjusted zenith. With
    /// interpolate false this is exactly the original algorithm.
    /// </summary>
    /// <param name="jdDay">the Julian day (0h UT) of the date</param>
    /// <param name="lat">the latitude in degrees</param>
    /// <param name="longitude">the longitude in degrees</param>
    /// <param name="elevation">the elevation in meters</param>
    /// <param name="adjustedZenith">the target zenith in degrees</param>
    /// <param name="guess">the starting guess, in minutes from 0h UT</param>
    /// <param name="interpolate">whether to use the interpolated series values</param>
    /// <param name="stats">if not null, receives {minimum |slope| along the path (0 if not converged), last slope}, slopes in degrees per minute</param>
    /// <returns>the time in minutes from 0h UT of the date (not checked for convergence; see the callers)</returns>
    private double Secant(double jdDay, double lat, double longitude, double elevation, double adjustedZenith, double guess, bool interpolate,
            double[] stats)
    {
        double t0 = guess;
        double t1 = guess + 0.5;
        double f0 = TopocentricTrueZenith(jdDay + t0 / 1440.0, lat, longitude, elevation, interpolate) - adjustedZenith;
        double f1 = TopocentricTrueZenith(jdDay + t1 / 1440.0, lat, longitude, elevation, interpolate) - adjustedZenith;
        double minSlope = double.MaxValue;
        double lastSlope = (f1 - f0) / (t1 - t0);
        for (int i = 0; i < 12 && Math.Abs(f1) > 1e-9; i++)
        {
            double denom = (f1 - f0);
            if (denom == 0)
            {
                break;
            }
            lastSlope = denom / (t1 - t0);
            minSlope = Math.Min(minSlope, Math.Abs(lastSlope));
            double t2 = t1 - f1 * (t1 - t0) / denom;
            t0 = t1; f0 = f1;
            t1 = t2;
            f1 = TopocentricTrueZenith(jdDay + t1 / 1440.0, lat, longitude, elevation, interpolate) - adjustedZenith;
        }
        if (stats != null)
        {
            stats[0] = Math.Abs(f1) > 1e-6 ? 0 : Math.Min(minSlope, Math.Abs(lastSlope)); // not converged: use the exact path
            stats[1] = lastSlope;
        }
        return t1;
    }

    /// <summary>
    /// Interpolation nodes per one day (TT) window: 5 nodes, so quartic interpolation.
    /// </summary>
    private const int InterpolationNodes = 5;
    /// <summary>
    /// Number of day windows kept. A day's zmanim span at most two or three UT/TT days, so 4 avoids thrashing.
    /// </summary>
    private const int InterpolationWindows = 4;
    /// <summary>
    /// Minimum rate (degrees per minute) at which the sun must cross the zenith along the whole search for an interpolated result to be used.
    /// </summary>
    private const double GuardMinSlope = 0.02;
    /// <summary>
    /// Maximum time error (minutes) of an interpolated result, checked with one exact evaluation: 0.5 ms.
    /// </summary>
    private const double GuardToleranceMinutes = 0.5 / 60000.0;

    /// <summary>
    /// The exact VSOP87 Earth series (heliocentric longitude and latitude in degrees, radius vector in AU) and nutation
    /// (Δψ, Δε in degrees).
    /// </summary>
    /// <param name="jme">the Julian ephemeris millennium</param>
    /// <param name="jce">the Julian ephemeris century</param>
    /// <returns>{L, B, R, Δψ, Δε}</returns>
    private static double[] ExactSeries(double jme, double jce)
    {
        double[] nut = Nutation(jce);
        return new double[] { JavaMath.ToDegrees(SumSeries(EarthL, jme)), JavaMath.ToDegrees(SumSeries(EarthB, jme)), SumSeries(EarthR, jme),
                nut[0], nut[1] };
    }

    /// <summary>
    /// The exact series values at <see cref="InterpolationNodes"/> evenly spaced times across one day (TT), used to interpolate the series
    /// within that day. The series depend only on the time (TT), not on the location or any setting. Immutable.
    /// </summary>
    private sealed class SeriesWindow
    {
        /// <summary>
        /// The Julian ephemeris day (TT) at the start of the window.
        /// </summary>
        internal double start;
        /// <summary>
        /// The exact series values at each node, as <see cref="ExactSeries"/>.
        /// </summary>
        internal double[][] nodes;

        /// <summary>
        /// Calculates the exact series values at the window's nodes.
        /// </summary>
        /// <param name="start">the Julian ephemeris day (TT) at the start of the window</param>
        internal SeriesWindow(double start)
        {
            this.start = start;
            this.nodes = new double[InterpolationNodes][];
            for (int k = 0; k < InterpolationNodes; k++)
            {
                double jce = (start + (double)k / (InterpolationNodes - 1) - JulianDayJan12000) / JulianDaysPerCentury;
                nodes[k] = ExactSeries(jce / 10.0, jce);
            }
        }
    }

    /// <summary>
    /// The most recently used day windows, newest first, at most <see cref="InterpolationWindows"/>. The array is never modified after it
    /// is published: a new window is added by publishing a new array, so concurrent callers can not see a partial state (at most one
    /// window is computed twice). Fixed memory. Not part of <see cref="Equals"/> or <see cref="HashCode"/>.
    /// </summary>
    private volatile SeriesWindow[]? seriesWindows;

    /// <summary>
    /// Returns the series values at a time, interpolated (Lagrange, through all nodes) within its one day window.
    /// </summary>
    /// <param name="jde">the Julian ephemeris day (TT)</param>
    /// <returns>{L, B, R, Δψ, Δε} as </returns>
    private double[] GetInterpolatedSeries(double jde)
    {
        double start = Math.Floor(jde - 0.5) + 0.5;
        SeriesWindow? window = null;
        SeriesWindow[]? windows = seriesWindows;
        if (windows != null)
        {
            foreach (SeriesWindow w in windows)
            {
                if (w.start == start)
                {
                    window = w;
                    break;
                }
            }
        }
        if (window == null)
        {
            window = new SeriesWindow(start);
            int keep = windows == null ? 0 : Math.Min(windows.Length, InterpolationWindows - 1);
            SeriesWindow[] updated = new SeriesWindow[keep + 1];
            updated[0] = window;
            if (keep > 0)
            {
                Array.Copy(windows!, 0, updated, 1, keep);
            }
            seriesWindows = updated;
        }
        double x = (jde - start) * (InterpolationNodes - 1);
        double[] weights = new double[InterpolationNodes];
        for (int i = 0; i < InterpolationNodes; i++)
        {
            double w = 1;
            for (int j = 0; j < InterpolationNodes; j++)
            {
                if (j != i)
                {
                    w *= (x - j) / (i - j);
                }
            }
            weights[i] = w;
        }
        double[] result = new double[5];
        for (int c = 0; c < 5; c++)
        {
            double sum = 0;
            for (int i = 0; i < InterpolationNodes; i++)
            {
                sum += weights[i] * window.nodes[i][c];
            }
            result[c] = sum;
        }
        return result;
    }

    /// <summary>
    /// Largest distance in degrees between the requested zenith and the Sun's zenith at an accepted secant result. Converged results end
    /// within about 1e-7° to 8e-7° (the time resolution of a Julian date held in a double), failed ones far away; 1e-4° is well
    /// above the first and below SPA's stated ±0.0003° accuracy.
    /// </summary>
    private const double MaxZenithResidual = 1e-4;

    /// <summary>
    /// Minimum rate, in degrees per minute, at which the Sun must cross the zenith along the whole secant search for its result to be
    /// used. Ordinary crossings are about 0.1 - 0.25°/min; below this the day is near grazing and the bracketing search is used.
    /// </summary>
    private const double GrazingMinSlope = 0.02;

    /// <summary>
    /// The window, in minutes from 0h UT of the date, in which a rise or set belongs to the date. These are exactly the windows
    /// <see cref="AstronomicalCalendar"/> uses when it converts the 0 - 24 hour result back to an instant (by local mean
    /// time: a rise after 18:00 belongs to the previous date, a set before 06:00 to the next), so a result found here always maps back
    /// to the right instant. Rises: (18:00 local mean time the evening before, 18:00]; sets: [06:00, 06:00 the next morning).
    /// </summary>
    /// <param name="longitude">the longitude in degrees, positive east</param>
    /// <param name="solarEvent"><see cref="SolarEvent.Sunrise"/> or <see cref="SolarEvent.Sunset"/></param>
    /// <returns>{start, end} in minutes from 0h UT of the date</returns>
    private static double[] GetRiseSetWindowMinutes(double longitude, SolarEvent solarEvent)
    {
        double startHours = (solarEvent == SolarEvent.Sunrise ? -6.0 : 6.0) - longitude / 15.0;
        return new double[] { startHours * 60.0, (startHours + 24.0) * 60.0 };
    }

    /// <summary>
    /// Bracket resolution of the search: one millisecond, in hours (as solarpositioning).
    /// </summary>
    private const double TimeTolerance = 1.0 / 3600.0 / 1000.0;
    /// <summary>
    /// Allowance for Julian date quantisation and floating point evaluation (as solarpositioning).
    /// </summary>
    private const double RoundingError = 1e-8;

    /// <summary>
    /// Finds the first rise (or set) crossing of the adjusted zenith inside this date's window (see <see cref="GetRiseSetWindowMinutes"/>) with the bracketing search of Klaus Brunner's solarpositioning library 3.0
    /// (<c>SolarEvents</c>): the interval is split until a bracket with a guaranteed sign change is found, then refined with ITP to one
    /// millisecond, keeping the later endpoint. A tangency is not a crossing. Returns NaN when no crossing exists in the window.
    /// </summary>
    /// <param name="jdDay">the Julian day (0h UT) of the date</param>
    /// <param name="latitude">the latitude in degrees</param>
    /// <param name="longitude">the longitude in degrees</param>
    /// <param name="elevation">the elevation in meters</param>
    /// <param name="adjustedZenith">the target zenith in degrees</param>
    /// <param name="solarEvent">sunrise or sunset</param>
    /// <returns>minutes from 0h UT of the date, or NaN</returns>
    private double BracketRiseSet(double jdDay, double latitude, double longitude, double elevation, double adjustedZenith,
            SolarEvent solarEvent)
    {
        double[] window = GetRiseSetWindowMinutes(longitude, solarEvent);
        double startHours = window[0] / 60.0;
        double hours = (window[1] - window[0]) / 60.0;
        double direction = solarEvent == SolarEvent.Sunrise ? 1.0 : -1.0;
        double horizon = Math.Sin(JavaMath.ToRadians(90.0 - adjustedZenith));
        // Engineering estimate for |f''| in hours, as solarpositioning: daily rotation contributes about (2*pi/24)^2 * cos(latitude).
        double curvature = 0.1 * Math.Abs(Math.Cos(JavaMath.ToRadians(latitude))) + 0.0001;
        Func<double, double> f = t => direction
                * (Math.Sin(JavaMath.ToRadians(Topocentric(jdDay + (startHours + t) / 24.0, latitude, longitude, elevation)[0])) - horizon);
        CrossingSearch search = new CrossingSearch(f, curvature);
        double b = search.Value(hours);
        double crossing = search.Find(0.0, hours, search.Value(0.0), b);
        if (double.IsNaN(crossing) && b == 0.0)
        {
            // Include a zero endpoint only when it crosses, rather than touches, the horizon.
            if (search.Value(Math.Max(0.0, hours - TimeTolerance)) < 0.0 && search.Value(hours + TimeTolerance) > 0.0)
            {
                crossing = hours;
            }
        }
        return double.IsNaN(crossing) ? double.NaN : (startHours + crossing) * 60.0;
    }

    /// <summary>
    /// The bracketing crossing search of solarpositioning 3.0 (<c>SolarEvents.CrossingSearch</c>), unchanged.
    /// </summary>
    private sealed class CrossingSearch
    {
        /// <summary>
        /// The function whose sign change is searched for, of time in hours from the window start.
        /// </summary>
        internal Func<double, double> function;
        /// <summary>
        /// An upper bound estimate of |f''|, used to decide when an interval can't hide a crossing.
        /// </summary>
        internal double curvature;

        /// <summary>
        /// Creates a search.
        /// </summary>
        /// <param name="function">the function whose sign change is searched for</param>
        /// <param name="curvature">an upper bound estimate of |f''|</param>
        internal CrossingSearch(Func<double, double> function, double curvature)
        {
            this.function = function;
            this.curvature = curvature;
        }

        /// <summary>
        /// Evaluates the function.
        /// </summary>
        /// <param name="time">hours from the window start</param>
        /// <returns>the function value</returns>
        internal double Value(double time)
        {
            return function(time);
        }

        /// <summary>
        /// Recursively splits the interval until a bracket with a sign change is found, then refines it.
        /// </summary>
        /// <param name="start">the interval start</param>
        /// <param name="end">the interval end</param>
        /// <param name="a">the function value at start</param>
        /// <param name="b">the function value at end</param>
        /// <returns>the crossing time, or NaN if the interval holds none</returns>
        internal double Find(double start, double end, double a, double b)
        {
            double width = end - start;
            double slope = (b - a) / width;
            double slopeError = curvature * width / 2.0 + 2.0 * RoundingError / width;
            // Standard interpolation bounds, using the conservative curvature estimate above:
            // slope error M*w/2, chord error M*w*w/8 (https://dlmf.nist.gov/3.3.E5).
            // Discard only intervals that cannot rise through zero; otherwise search left first.
            if (slope + slopeError <= 0.0
                    || ((a > 0.0) == (b > 0.0) && Math.Min(Math.Abs(a), Math.Abs(b)) > curvature * width * width / 8.0 + RoundingError))
            {
                return double.NaN;
            }
            if (a < 0.0 && b >= 0.0 && slope > slopeError)
            {
                return Refine(start, end, a, b);
            }
            if (width <= TimeTolerance)
            {
                return a < 0.0 && b > 0.0 ? end : double.NaN;
            }
            double middle = start + width / 2.0;
            double m = Value(middle);
            // Keep a shallow crossing inside a bracket instead of splitting at zero.
            // Stay strictly inside the interval so both recursive searches progress.
            if (m == 0.0 && (a != 0.0 || b != 0.0))
            {
                middle += Math.Min(TimeTolerance, width / 4.0);
                m = Value(middle);
            }
            double left = Find(start, middle, a, m);
            return double.IsNaN(left) ? Find(middle, end, m, b) : left;
        }

        /// <summary>
        /// Refines a bracket with a sign change to the crossing using ITP.
        /// </summary>
        /// <param name="start">the bracket start</param>
        /// <param name="end">the bracket end</param>
        /// <param name="a">the function value at start</param>
        /// <param name="b">the function value at end</param>
        /// <returns>the crossing time (the later endpoint of the final bracket)</returns>
        double Refine(double start, double end, double a, double b)
        {
            // ITP (Oliveira & Takahashi): https://doi.org/10.1145/3423597.
            // Use k1 = 0.2 / initial width, k2 = 2, n0 = 1. The projection allows
            // at most one extra iteration over bisection while favouring interpolation.
            double scale = 0.2 / (end - start);
            // Maximum remaining width after a step; halve the allowance each time.
            double maxWidth = TimeTolerance;
            while (maxWidth < end - start)
            {
                maxWidth *= 2.0;
            }
            while (end - start > TimeTolerance)
            {
                double width = end - start;
                double middle = start + width / 2.0;
                // Equal values can occur on a rounded zero; use the midpoint then.
                double interpolated = a == b ? middle : start - a * width / (b - a);
                double towardsMiddle = middle - interpolated;
                double truncated = interpolated + Math.CopySign(Math.Min(scale * width * width, Math.Abs(towardsMiddle)), towardsMiddle);
                double radius = Math.Max(0.0, maxWidth - width / 2.0);
                double trial = middle + Math.Max(-radius, Math.Min(radius, truncated - middle));
                double value = Value(trial);
                if (value > 0.0)
                {
                    end = trial;
                    b = value;
                }
                else
                {
                    start = trial;
                    a = value;
                }
                maxWidth /= 2.0;
            }
            // Keep the later endpoint, so the next search cannot rediscover this crossing.
            return end;
        }
    }

    ///
    public override double GetUTCNoon(DateOnly localDate, GeoLocation geoLocation)
    {
        double noon = SolveNoonMidnight(GetJulianDay(localDate), -geoLocation.Longitude, SolarEvent.Noon) / 60;
        return (noon % 24 + 24) % 24;
    }

    ///
    public override double GetUTCMidnight(DateOnly localDate, GeoLocation geoLocation)
    {
        double midnight = SolveNoonMidnight(GetJulianDay(localDate), -geoLocation.Longitude, SolarEvent.Midnight) / 60;
        return (midnight % 24 + 24) % 24;
    }

    /// <summary>
    /// Solar transit (noon) or anti-transit (midnight) in UTC minutes after 0:00, using the high-accuracy SPA equation of
    /// time. Parallax has a negligible (sub-second) effect on transit time, so the geocentric equation of time is used.
    /// </summary>
    /// <param name="julianDay">the (UT) Julian day for 0:00.</param>
    /// <param name="lonWest">the observer longitude in degrees, positive west.</param>
    /// <param name="solarEvent"><see cref="SolarEvent.Noon"/> or <see cref="SolarEvent.Midnight"/>.</param>
    /// <returns>UTC minutes after 0:00.</returns>
    private double SolveNoonMidnight(double julianDay, double lonWest, SolarEvent solarEvent)
    {
        // Rigorous meridian solve: find the UT at which the Sun's apparent hour angle equals 0 (transit / noon)
        // or 180 (anti-transit / midnight). This matches the definition used by reference ephemerides (and the
        // NREL SPA Appendix A.2 transit procedure); the previous equation-of-time formulation differed from the
        // true meridian crossing by a near-constant ~0.2 s.
        double lonEast = -lonWest;
        double targetHourAngle = (solarEvent == SolarEvent.Noon) ? 0.0 : 180.0;
        double dayFraction = ((solarEvent == SolarEvent.Noon) ? 0.5 : 1.0) - lonEast / 360.0;
        for (int i = 0; i < 3; i++)
        {
            double[] sc = SolarCoords(julianDay + dayFraction);
            double alpha = sc[0]; // apparent right ascension (degrees)
            double nu = sc[3];    // apparent sidereal time at Greenwich (degrees)
            double hourAngle = ((nu + lonEast - alpha - targetHourAngle) % 360 + 540) % 360 - 180; // [-180, 180)
            dayFraction -= hourAngle / 360.0; // the Sun's hour angle advances 360 deg per solar day
        }
        return dayFraction * 1440.0; // UTC minutes after 0:00
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
        for (int i = 0; i < 4; i++)
        {
            double jd = julianDay + dateTime;
            double decl = SolarCoords(jd)[1];
            double ratio = TanDegrees(decl) / TanDegrees(geoLocation.Latitude);
            if (double.IsNaN(ratio) || ratio > 1.0 || ratio < -1.0)
            {
                return double.NaN;
            }
            double offset = ((targetAzimuth == 90.0) ? -1.0 : 1.0) * (AcosDegrees(ratio) / 360.0);
            dateTime = solarNoonBase + offset - (EquationOfTime(jd) / 1440.0);
        }
        double timeUTC = dateTime * 24.0;
        return (timeUTC % 24 + 24) % 24;
    }

    /// <inheritdoc/>
    public override double GetSolarElevation(DateTimeOffset instant, GeoLocation geoLocation)
    {
        double jd = JulianDayFromInstant(instant);
        double[] topo = Topocentric(jd, geoLocation.Latitude, geoLocation.Longitude, geoLocation.Elevation);
        return topo[1]; // observed (refraction-corrected) topocentric elevation
    }

    /// <inheritdoc/>
    public override double GetSolarAzimuth(DateTimeOffset instant, GeoLocation geoLocation)
    {
        double jd = JulianDayFromInstant(instant);
        double[] topo = Topocentric(jd, geoLocation.Latitude, geoLocation.Longitude, geoLocation.Elevation);
        return topo[2]; // topocentric azimuth, eastward from north
    }

    /// <summary>
    /// Compute the Sun's <b>geocentric</b> apparent coordinates and the apparent sidereal time for the given UT Julian day.
    /// </summary>
    /// <param name="julianDayUT">the UT Julian day (including fractional day).</param>
    /// <returns>{alpha (geocentric apparent RA, deg 0-360), delta (geocentric apparent declination, deg), epsilon (true
    ///         obliquity, deg), nu (apparent Greenwich sidereal time, deg 0-360), radius (AU), lambda (apparent longitude, deg)}.</returns>
    private double[] SolarCoords(double julianDayUT)
    {
        return SolarCoords(julianDayUT, false);
    }

    /// <summary>
    /// Solar coordinates, optionally with the VSOP87 series and nutation taken from the per-day interpolation (see
    /// <see cref="GetInterpolatedSeries"/>). Only the first rise / set search uses interpolated values, and its result is always
    /// verified exactly (see <see cref="IsInterpolatedSolutionAcceptable"/>); everything else is exact.
    /// </summary>
    /// <param name="julianDayUT">the Julian day (UT)</param>
    /// <param name="interpolate">whether to use the interpolated series values</param>
    /// <returns>the same values as <see cref="SolarCoords(double)"/></returns>
    private double[] SolarCoords(double julianDayUT, bool interpolate)
    {
        double deltaTSeconds = applyDeltaT ? (deltaTOverride ?? EstimateDeltaT(julianDayUT)) : 0.0;
        double jde = julianDayUT + deltaTSeconds / 86400.0;
        double jce = (jde - JulianDayJan12000) / JulianDaysPerCentury;
        double jc = (julianDayUT - JulianDayJan12000) / JulianDaysPerCentury;
        double jme = jce / 10.0;

        double[] series = interpolate ? GetInterpolatedSeries(jde) : ExactSeries(jme, jce);
        double earthL = series[0];
        double earthB = series[1];
        double radius = series[2]; // AU

        double theta = (earthL + 180.0) % 360.0;   // geocentric longitude
        double beta = -earthB;                      // geocentric latitude

        double[] nut = { series[3], series[4] };     // {Δψ, Δε} in degrees
        double epsilon = MeanObliquity(jme) + nut[1];
        double aberration = -20.4898 / (3600.0 * radius); // degrees
        double lambda = theta + nut[0] + aberration;      // apparent longitude

        // apparent sidereal time at Greenwich
        double nu0 = 280.46061837 + 360.98564736629 * (julianDayUT - JulianDayJan12000)
                + 0.000387933 * jc * jc - jc * jc * jc / 38710000.0;
        nu0 = (nu0 % 360 + 360) % 360;
        double nu = nu0 + nut[0] * CosDegrees(epsilon);

        double alpha = JavaMath.ToDegrees(Math.Atan2(
                SinDegrees(lambda) * CosDegrees(epsilon) - TanDegrees(beta) * SinDegrees(epsilon),
                CosDegrees(lambda)));
        alpha = (alpha % 360 + 360) % 360;
        double delta = AsinDegrees(SinDegrees(beta) * CosDegrees(epsilon)
                + CosDegrees(beta) * SinDegrees(epsilon) * SinDegrees(lambda));

        return new double[] { alpha, delta, epsilon, nu, radius, lambda };
    }

    /// <summary>
    /// Full SPA topocentric reduction (observer parallax + atmospheric refraction).
    /// </summary>
    /// <param name="julianDayUT">the UT Julian day.</param>
    /// <param name="latitude">observer latitude in degrees.</param>
    /// <param name="longitude">observer longitude in degrees, positive east.</param>
    /// <param name="elevationMeters">observer elevation in meters.</param>
    /// <returns>{trueElevation (topocentric, no refraction, deg), observedElevation (refraction-corrected, deg),
    ///         azimuth (eastward from north, deg 0-360)}.</returns>
    private double[] Topocentric(double julianDayUT, double latitude, double longitude, double elevationMeters)
    {
        return Topocentric(julianDayUT, latitude, longitude, elevationMeters, false);
    }

    /// <summary>
    /// As <see cref="Topocentric(double, double, double, double)"/>, optionally using the interpolated series values.
    /// </summary>
    /// <param name="julianDayUT">the Julian day (UT)</param>
    /// <param name="latitude">the latitude in degrees</param>
    /// <param name="longitude">the longitude in degrees</param>
    /// <param name="elevationMeters">the elevation in meters</param>
    /// <param name="interpolate">whether to use the interpolated series values</param>
    /// <returns>as <see cref="Topocentric(double, double, double, double)"/></returns>
    private double[] Topocentric(double julianDayUT, double latitude, double longitude, double elevationMeters, bool interpolate)
    {
        double[] sc = SolarCoords(julianDayUT, interpolate);
        double alpha = sc[0], delta = sc[1], nu = sc[3], radius = sc[4];

        double h = (nu + longitude - alpha) % 360.0; // observer local hour angle (deg), positive west
        h = (h + 360) % 360;

        // equatorial horizontal parallax of the sun
        double xi = 8.794 / (3600.0 * radius); // degrees
        double u = Math.Atan(0.99664719 * TanDegrees(latitude));
        double x = Math.Cos(u) + (elevationMeters / 6378140.0) * CosDegrees(latitude);
        double y = 0.99664719 * Math.Sin(u) + (elevationMeters / 6378140.0) * SinDegrees(latitude);

        double deltaAlpha = JavaMath.ToDegrees(Math.Atan2(-x * SinDegrees(xi) * SinDegrees(h),
                CosDegrees(delta) - x * SinDegrees(xi) * CosDegrees(h)));
        double deltaPrime = JavaMath.ToDegrees(Math.Atan2(
                (SinDegrees(delta) - y * SinDegrees(xi)) * CosDegrees(deltaAlpha),
                CosDegrees(delta) - x * SinDegrees(xi) * CosDegrees(h))); // topocentric declination
        double hPrime = h - deltaAlpha; // topocentric local hour angle

        double e0 = AsinDegrees(SinDegrees(latitude) * SinDegrees(deltaPrime)
                + CosDegrees(latitude) * CosDegrees(deltaPrime) * CosDegrees(hPrime)); // true (no refraction)

        double deltaE = RefractionCorrection(e0); // degrees
        double e = e0 + deltaE;                    // observed elevation

        double gamma = JavaMath.ToDegrees(Math.Atan2(SinDegrees(hPrime),
                CosDegrees(hPrime) * SinDegrees(latitude) - TanDegrees(deltaPrime) * CosDegrees(latitude)));
        double azimuth = (gamma + 180.0) % 360.0; // eastward from north
        azimuth = (azimuth + 360) % 360;

        return new double[] { e0, e, azimuth };
    }

    /// <summary>
    /// Topocentric <em>true</em> (un-refracted) zenith of the Sun's center, used by the rise/set solver so that refraction
    /// is supplied solely by the parent <see cref="AstronomicalCalculator.AdjustZenith">adjustZenith</see>.
    /// </summary>
    /// <param name="julianDayUT">the UT Julian day.</param>
    /// <param name="latitude">observer latitude in degrees.</param>
    /// <param name="longitude">observer longitude in degrees, positive east.</param>
    /// <param name="elevationMeters">observer elevation in meters.</param>
    /// <returns>the topocentric true zenith in degrees.</returns>
    private double TopocentricTrueZenith(double julianDayUT, double latitude, double longitude, double elevationMeters)
    {
        return TopocentricTrueZenith(julianDayUT, latitude, longitude, elevationMeters, false);
    }

    /// <summary>
    /// As <see cref="TopocentricTrueZenith(double, double, double, double)"/>, optionally using the interpolated series values.
    /// </summary>
    /// <param name="julianDayUT">the Julian day (UT)</param>
    /// <param name="latitude">the latitude in degrees</param>
    /// <param name="longitude">the longitude in degrees</param>
    /// <param name="elevationMeters">the elevation in meters</param>
    /// <param name="interpolate">whether to use the interpolated series values</param>
    /// <returns>the topocentric zenith angle in degrees, without refraction</returns>
    private double TopocentricTrueZenith(double julianDayUT, double latitude, double longitude, double elevationMeters, bool interpolate)
    {
        return 90.0 - Topocentric(julianDayUT, latitude, longitude, elevationMeters, interpolate)[0];
    }

    /// <summary>
    /// The SPA atmospheric-refraction correction (Reda and Andreas) in degrees. The horizon refraction parameter is the parent's
    /// <see cref="AstronomicalCalculator.Refraction"/>, and the <see cref="Pressure"/> / <see cref="Temperature"/> are settable, so the model is fully configurable
    /// rather than hardcoded.
    /// </summary>
    /// <param name="trueElevation">the true (un-refracted) topocentric elevation in degrees.</param>
    /// <returns>the refraction correction in degrees to add to the true elevation.</returns>
    private double RefractionCorrection(double trueElevation)
    {
        double atmosRefract = Refraction; // parent's configurable horizon refraction (degrees), default 34'
        // Fixed solar-radius constant (~16'), used only as a coarse below-horizon cutoff for refraction —
        // deliberately NOT the date-based apparent radius, since the seasonal ~16" variation is irrelevant to this gate.
        double sunRadius = SolarRadius;
        if (trueElevation < -1.0 * (sunRadius + atmosRefract))
        {
            return 0.0; // sun fully below the refracted horizon
        }
        return (pressure / 1010.0) * (283.0 / (273.0 + temperature))
                * 1.02 / (60.0 * TanDegrees(trueElevation + 10.3 / (trueElevation + 5.11)));
    }

    /// <summary>
    /// The <a href="https://en.wikipedia.org/wiki/Equation_of_time">equation of time</a> (apparent minus mean solar time)
    /// in minutes of time (Meeus chapter 28), built from the SPA geocentric apparent right ascension. The sign matches
    /// <see cref="NOAACalculator"/>.
    /// </summary>
    /// <param name="julianDayUT">the UT Julian day.</param>
    /// <returns>the equation of time in minutes.</returns>
    private double EquationOfTime(double julianDayUT)
    {
        double[] sc = SolarCoords(julianDayUT);
        double alpha = sc[0], epsilon = sc[2];
        double deltaTSeconds = applyDeltaT ? (deltaTOverride ?? EstimateDeltaT(julianDayUT)) : 0.0;
        double jme = ((julianDayUT + deltaTSeconds / 86400.0) - JulianDayJan12000) / JulianDaysPerCentury / 10.0;
        double l0 = 280.4664567 + jme * (360007.6982779 + jme * (0.03032028
                + jme * (1.0 / 49931.0 - jme * (1.0 / 15300.0 + jme / 2000000.0))));
        l0 = (l0 % 360 + 360) % 360;
        double deltaPsi = Nutation(((julianDayUT + deltaTSeconds / 86400.0) - JulianDayJan12000) / JulianDaysPerCentury)[0];
        double e = l0 - 0.0057183 - alpha + deltaPsi * CosDegrees(epsilon); // degrees
        e = ((e + 180) % 360 + 360) % 360 - 180;
        return e * 4.0;
    }

    /// <summary>
    /// Return the <a href="https://en.wikipedia.org/wiki/Julian_day">Julian day</a> from a <see cref="DateOnly"/>.
    /// Julian day for 0h UT of the date (Meeus chapter 7).
    /// </summary>
    /// <param name="localDate">the <see cref="DateOnly"/> to get the Julian date for.</param>
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
    /// Julian day (including fractional day) for an <see cref="DateTimeOffset"/> in UTC.
    /// </summary>
    /// <param name="instant">the day to calculate the Julian day for.</param>
    /// <returns>the Julian day as a <c>double</c></returns>
    private static double JulianDayFromInstant(DateTimeOffset instant)
    {
        DateTimeOffset utc = instant.ToUniversalTime();
        double fractionalDay = (utc.Hour + (utc.Minute
                + (utc.Second + JavaTime.GetNano(utc) / 1_000_000_000.0) / 60.0) / 60.0) / 24.0;
        return GetJulianDay(DateOnly.FromDateTime(utc.DateTime)) + fractionalDay;
    }

    /// <summary>
    /// The mean obliquity of the ecliptic in degrees (SPA / Meeus 22.2 polynomial in U = JME/10).
    /// </summary>
    /// <param name="jme">Julian millennia of ephemeris time since J2000.0.</param>
    /// <returns>mean obliquity in degrees.</returns>
    private static double MeanObliquity(double jme)
    {
        double u = jme / 10.0;
        double seconds = 84381.448 - 4680.93 * u - 1.55 * u * u + 1999.25 * Math.Pow(u, 3) - 51.38 * Math.Pow(u, 4)
                - 249.67 * Math.Pow(u, 5) - 39.05 * Math.Pow(u, 6) + 7.12 * Math.Pow(u, 7) + 27.87 * Math.Pow(u, 8)
                + 5.79 * Math.Pow(u, 9) + 2.45 * Math.Pow(u, 10);
        return seconds / 3600.0;
    }

    /// <summary>
    /// Nutation in longitude and obliquity, using the full 63-term SPA series (Reda &amp; Andreas, Table A4.3) as implemented in
    /// Klaus Brunner's solarpositioning library (same operations, including fused multiply-add).
    /// </summary>
    /// <param name="jce">Julian ephemeris century</param>
    /// <returns>{Δψ, Δε} in degrees</returns>
    private static double[] Nutation(double jce)
    {
        double[] x = new double[NutationCoeffs.Length];
        for (int i = 0; i < x.Length; i++)
        {
            x[i] = Polynomial(jce, NutationCoeffs[i]);
        }
        double deltaPsi = 0;
        double deltaEpsilon = 0;
        for (int i = 0; i < TermsPe.Length; i++)
        {
            double sum = 0;
            for (int j = 0; j < x.Length; j++)
            {
                sum = Math.FusedMultiplyAdd(x[j], TermsY[i][j], sum);
            }
            double xjYtermSum = JavaMath.ToRadians(sum);
            double[] termsPeI = TermsPe[i];
            deltaPsi = Math.FusedMultiplyAdd(Math.FusedMultiplyAdd(termsPeI[1], jce, termsPeI[0]), Math.Sin(xjYtermSum), deltaPsi);
            deltaEpsilon = Math.FusedMultiplyAdd(Math.FusedMultiplyAdd(termsPeI[3], jce, termsPeI[2]), Math.Cos(xjYtermSum), deltaEpsilon);
        }
        return new double[] { deltaPsi / 36_000_000, deltaEpsilon / 36_000_000 };
    }

    /// <summary>
    /// Polynomial with coefficients in ascending order, evaluated with fused multiply-add (as solarpositioning's MathUtil).
    /// </summary>
    /// <param name="x">the variable</param>
    /// <param name="coeffs">the coefficients, constant term first</param>
    /// <returns>the polynomial's value at x</returns>
    private static double Polynomial(double x, params double[] coeffs)
    {
        int n = coeffs.Length - 1;
        double sum = coeffs[n];
        for (int i = n - 1; i >= 0; i--)
        {
            sum = Math.FusedMultiplyAdd(x, sum, coeffs[i]);
        }
        return sum;
    }

    /// <summary>
    /// Polynomial coefficients of the nutation arguments X0 - X4 (Reda &amp; Andreas, equations 15 - 19).
    /// </summary>
    private static readonly double[][] NutationCoeffs = {
        new double[] {297.85036, 445267.111480, -0.0019142, 1.0 / 189474},
        new double[] {357.52772, 35999.050340, -0.0001603, -1.0 / 300000},
        new double[] {134.96298, 477198.867398, 0.0086972, 1.0 / 56250},
        new double[] {93.27191, 483202.017538, -0.0036825, 1.0 / 327270},
        new double[] {125.04452, -1934.136261, 0.0020708, 1.0 / 450000},
    };

    /// <summary>
    /// Multipliers of X0 - X4 for each of the 63 nutation terms (Reda &amp; Andreas, Table A4.3).
    /// </summary>
    private static readonly double[][] TermsY = {
        new double[] {0, 0, 0, 0, 1},
        new double[] {-2, 0, 0, 2, 2},
        new double[] {0, 0, 0, 2, 2},
        new double[] {0, 0, 0, 0, 2},
        new double[] {0, 1, 0, 0, 0},
        new double[] {0, 0, 1, 0, 0},
        new double[] {-2, 1, 0, 2, 2},
        new double[] {0, 0, 0, 2, 1},
        new double[] {0, 0, 1, 2, 2},
        new double[] {-2, -1, 0, 2, 2},
        new double[] {-2, 0, 1, 0, 0},
        new double[] {-2, 0, 0, 2, 1},
        new double[] {0, 0, -1, 2, 2},
        new double[] {2, 0, 0, 0, 0},
        new double[] {0, 0, 1, 0, 1},
        new double[] {2, 0, -1, 2, 2},
        new double[] {0, 0, -1, 0, 1},
        new double[] {0, 0, 1, 2, 1},
        new double[] {-2, 0, 2, 0, 0},
        new double[] {0, 0, -2, 2, 1},
        new double[] {2, 0, 0, 2, 2},
        new double[] {0, 0, 2, 2, 2},
        new double[] {0, 0, 2, 0, 0},
        new double[] {-2, 0, 1, 2, 2},
        new double[] {0, 0, 0, 2, 0},
        new double[] {-2, 0, 0, 2, 0},
        new double[] {0, 0, -1, 2, 1},
        new double[] {0, 2, 0, 0, 0},
        new double[] {2, 0, -1, 0, 1},
        new double[] {-2, 2, 0, 2, 2},
        new double[] {0, 1, 0, 0, 1},
        new double[] {-2, 0, 1, 0, 1},
        new double[] {0, -1, 0, 0, 1},
        new double[] {0, 0, 2, -2, 0},
        new double[] {2, 0, -1, 2, 1},
        new double[] {2, 0, 1, 2, 2},
        new double[] {0, 1, 0, 2, 2},
        new double[] {-2, 1, 1, 0, 0},
        new double[] {0, -1, 0, 2, 2},
        new double[] {2, 0, 0, 2, 1},
        new double[] {2, 0, 1, 0, 0},
        new double[] {-2, 0, 2, 2, 2},
        new double[] {-2, 0, 1, 2, 1},
        new double[] {2, 0, -2, 0, 1},
        new double[] {2, 0, 0, 0, 1},
        new double[] {0, -1, 1, 0, 0},
        new double[] {-2, -1, 0, 2, 1},
        new double[] {-2, 0, 0, 0, 1},
        new double[] {0, 0, 2, 2, 1},
        new double[] {-2, 0, 2, 0, 1},
        new double[] {-2, 1, 0, 2, 1},
        new double[] {0, 0, 1, -2, 0},
        new double[] {-1, 0, 1, 0, 0},
        new double[] {-2, 1, 0, 0, 0},
        new double[] {1, 0, 0, 0, 0},
        new double[] {0, 0, 1, 2, 0},
        new double[] {0, 0, -2, 2, 2},
        new double[] {-1, -1, 1, 0, 0},
        new double[] {0, 1, 1, 0, 0},
        new double[] {0, -1, 1, 2, 2},
        new double[] {2, -1, -1, 2, 2},
        new double[] {0, 0, 3, 2, 2},
        new double[] {2, -1, 0, 2, 2},
    };

    /// <summary>
    /// The a, b, c, d coefficients of Δψ and Δε for each of the 63 nutation terms (Reda &amp; Andreas, Table A4.3).
    /// </summary>
    private static readonly double[][] TermsPe = {
        new double[] {-171996, -174.2, 92025, 8.9},
        new double[] {-13187, -1.6, 5736, -3.1},
        new double[] {-2274, -0.2, 977, -0.5},
        new double[] {2062, 0.2, -895, 0.5},
        new double[] {1426, -3.4, 54, -0.1},
        new double[] {712, 0.1, -7, 0},
        new double[] {-517, 1.2, 224, -0.6},
        new double[] {-386, -0.4, 200, 0},
        new double[] {-301, 0, 129, -0.1},
        new double[] {217, -0.5, -95, 0.3},
        new double[] {-158, 0, 0, 0},
        new double[] {129, 0.1, -70, 0},
        new double[] {123, 0, -53, 0},
        new double[] {63, 0, 0, 0},
        new double[] {63, 0.1, -33, 0},
        new double[] {-59, 0, 26, 0},
        new double[] {-58, -0.1, 32, 0},
        new double[] {-51, 0, 27, 0},
        new double[] {48, 0, 0, 0},
        new double[] {46, 0, -24, 0},
        new double[] {-38, 0, 16, 0},
        new double[] {-31, 0, 13, 0},
        new double[] {29, 0, 0, 0},
        new double[] {29, 0, -12, 0},
        new double[] {26, 0, 0, 0},
        new double[] {-22, 0, 0, 0},
        new double[] {21, 0, -10, 0},
        new double[] {17, -0.1, 0, 0},
        new double[] {16, 0, -8, 0},
        new double[] {-16, 0.1, 7, 0},
        new double[] {-15, 0, 9, 0},
        new double[] {-13, 0, 7, 0},
        new double[] {-12, 0, 6, 0},
        new double[] {11, 0, 0, 0},
        new double[] {-10, 0, 5, 0},
        new double[] {-8, 0, 3, 0},
        new double[] {7, 0, -3, 0},
        new double[] {-7, 0, 0, 0},
        new double[] {-7, 0, 3, 0},
        new double[] {-7, 0, 3, 0},
        new double[] {6, 0, 0, 0},
        new double[] {6, 0, -3, 0},
        new double[] {6, 0, -3, 0},
        new double[] {-6, 0, 3, 0},
        new double[] {-6, 0, 3, 0},
        new double[] {5, 0, 0, 0},
        new double[] {-5, 0, 3, 0},
        new double[] {-5, 0, 3, 0},
        new double[] {-5, 0, 3, 0},
        new double[] {4, 0, 0, 0},
        new double[] {4, 0, 0, 0},
        new double[] {4, 0, 0, 0},
        new double[] {-4, 0, 0, 0},
        new double[] {-4, 0, 0, 0},
        new double[] {-4, 0, 0, 0},
        new double[] {3, 0, 0, 0},
        new double[] {-3, 0, 0, 0},
        new double[] {-3, 0, 0, 0},
        new double[] {-3, 0, 0, 0},
        new double[] {-3, 0, 0, 0},
        new double[] {-3, 0, 0, 0},
        new double[] {-3, 0, 0, 0},
        new double[] {-3, 0, 0, 0},
    };


    /// <summary>
    /// ΔT (TT - UT) estimate in seconds for a Julian day, as solarpositioning's DeltaT.estimate(LocalDate): the
    /// Espenak &amp; Meeus polynomials with its post-2005 extrapolation, evaluated at the decimal year of the date's month. Beyond
    /// the year 3000, where that library declines to estimate, its last branch is extrapolated.
    /// </summary>
    /// <param name="julianDay">the Julian day (UT)</param>
    /// <returns>ΔT in seconds</returns>
    private static double EstimateDeltaT(double julianDay)
    {
        // java.time's LocalDate.ofEpochDay covers any year, but DateOnly stops at 1..9999 and the rise / set searches can
        // evaluate far from the requested date, so the proleptic Gregorian year and month are computed directly.
        long epochDay = JavaMath.ToLong(Math.Floor(julianDay - 2440587.5));
        if (epochDay < -365243219162L || epochDay > 365241780471L)
        {
            // the range of java.time's LocalDate (years -999,999,999 to 999,999,999); KosherJava throws DateTimeException
            throw new ArgumentOutOfRangeException(nameof(julianDay), julianDay, "The date is outside the supported range of years.");
        }
        (long year, int month) = JavaTime.YearMonthOfEpochDay(epochDay);
        return EstimateDeltaTForYear(year + (month - 0.5) / 12);
    }

    /// <summary>
    /// Estimates ΔT for a decimal year, using the same branches and coefficients as solarpositioning 3.0.
    /// </summary>
    /// <param name="year">the decimal year</param>
    /// <returns>ΔT in seconds</returns>
    private static double EstimateDeltaTForYear(double year)
    {
        double deltaT;

        if (year < -500)
        {
            double u = (year - 1820) / 100.0;
            deltaT = Polynomial(u, -20, 0, 32);
        }
        else if (year < 500)
        {
            double u = year / 100.0;
            deltaT =
                    Polynomial(
                            u, 10583.6, -1014.41, 33.78311, -5.952053, -0.1798452, 0.022174192, 0.0090316521);
        }
        else if (year < 1600)
        {
            double u = (year - 1000) / 100.0;
            deltaT =
                    Polynomial(
                            u, 1574.2, -556.01, 71.23472, 0.319781, -0.8503463, -0.005050998, 0.0083572073);
        }
        else if (year < 1700)
        {
            double t = year - 1600;
            deltaT = Polynomial(t, 120, -0.9808, -0.01532, 1.0 / 7129);
        }
        else if (year < 1800)
        {
            double t = year - 1700;
            deltaT = Polynomial(t, 8.83, 0.1603, -0.0059285, 0.00013336, -1.0 / 1174000);
        }
        else if (year < 1860)
        {
            double t = year - 1800;
            deltaT =
                    Polynomial(
                            t,
                            13.72,
                            -0.332447,
                            0.0068612,
                            0.0041116,
                            -0.00037436,
                            0.0000121272,
                            -0.0000001699,
                            0.000000000875);
        }
        else if (year < 1900)
        {
            double t = year - 1860;
            deltaT = Polynomial(t, 7.62, 0.5737, -0.251754, 0.01680668, -0.0004473624, 1.0 / 233174);
        }
        else if (year < 1920)
        {
            double t = year - 1900;
            deltaT = Polynomial(t, -2.79, 1.494119, -0.0598939, 0.0061966, -0.000197);
        }
        else if (year < 1941)
        {
            double t = year - 1920;
            deltaT = Polynomial(t, 21.20, 0.84493, -0.076100, 0.0020936);
        }
        else if (year < 1961)
        {
            double t = year - 1950;
            deltaT = Polynomial(t, 29.07, 0.407, -1.0 / 233, 1.0 / 2547);
        }
        else if (year < 1986)
        {
            double t = year - 1975;
            deltaT = Polynomial(t, 45.45, 1.067, -1.0 / 260, -1.0 / 718);
        }
        else if (year < 2005)
        {
            double t = year - 2000;
            deltaT = Polynomial(t, 63.86, 0.3345, -0.060374, 0.0017275, 0.000651814, 0.00002373599);
        }
        else if (year < 2015)
        {
            double t = year - 2005;
            deltaT = Polynomial(t, 64.69, 0.2930);
        }
        else if (year < 2026.5)
        {
            double t = year - 2015;
            // Retain full precision; the branches join in value and slope at 2015 and 2026.5.
            deltaT =
                    Polynomial(
                            t, 67.62, 0.2930, 0.08753166427153103, -0.020049795884351372, 0.0009758496126416308);
        }
        else
        {
            double t = year - 2026.5;
            deltaT = Polynomial(t, 69.14, 0.28805287963416737, 0.0057380400318460655); // beyond 3000 this branch is extrapolated
        }

        return deltaT;
    }


    /// <summary>
    /// Evaluate a VSOP87 series Σ<sub>n</sub> (Σ<sub>i</sub> A<sub>i</sub>·cos(B<sub>i</sub> +
    /// C<sub>i</sub>·τ)) · τ<sup>n</sup> with coefficients in 1e-8 of the native unit (radians or AU).
    /// </summary>
    /// <param name="series">indexed <c>[n][term][{A,B,C}]</c>.</param>
    /// <param name="tau">time in Julian millennia from J2000.0 (JME).</param>
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

    // ---------------------------------------------------------------------------------------------------------------
    // Abridged VSOP87 Earth series (Meeus Appendix III). {A, B, C}; value Σ A·cos(B + C·τ), τ in Julian millennia,
    // result in 1e-8 rad (L, B) or 1e-8 AU (R). Validated to ~0.1" against Meeus's worked example.
    // ---------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Earth heliocentric longitude series L0..L5.
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
    /// Two <c>SPACalculator</c> instances are considered equal if their <see cref="ApplyDeltaT"/>, <see cref="DeltaTOverride"/>,
    /// <see cref="Pressure"/> and <see cref="Temperature"/> values are identical.
    /// </summary>
    /// <param name="obj">the reference object with which to compare</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is equal to this instance; otherwise <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (!base.Equals(obj))
        {
            return false;
        }
        SPACalculator other = (SPACalculator)obj; // safe: super.equals() verifies getClass()
        return this.applyDeltaT == other.applyDeltaT
                && Equals(this.deltaTOverride, other.deltaTOverride)
                && JavaMath.Compare(this.pressure, other.pressure) == 0
                && JavaMath.Compare(this.temperature, other.temperature) == 0;
    }

    /// <summary>
    /// This implementation hashes the <c>applyDeltaT</c>, <c>deltaTOverride</c>, <c>pressure</c>, and <c>temperature</c>
    /// properties to maintain the contract with <see cref="Equals"/>.
    /// </summary>
    /// <returns>a hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), applyDeltaT, deltaTOverride, pressure, temperature);
    }
}
