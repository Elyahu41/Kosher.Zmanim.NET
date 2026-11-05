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
/// A class that contains location information such as latitude and longitude required for astronomical calculations. The
/// elevation field may not be used by some calculation engines and would be ignored if set. Check the documentation for
/// specific implementations of the <see cref="AstronomicalCalculator"/> to see if elevation is calculated as part of the
/// algorithm.
/// </summary>
public class GeoLocation : IEquatable<GeoLocation>
{
    /// <summary>
    /// The latitude, for example 40.096 for Lakewood, NJ.
    /// </summary>
    /// <seealso cref="Latitude"/>
    /// <seealso cref="Latitude"/>
    private double latitude;

    /// <summary>
    /// The longitude, for example -74.222 for Lakewood, NJ.
    /// </summary>
    /// <seealso cref="Longitude"/>
    /// <seealso cref="Longitude"/>
    private double longitude;

    /// <summary>
    /// The location name used for display, for example "Lakewood, NJ".
    /// </summary>
    /// <seealso cref="LocationName"/>
    /// <seealso cref="LocationName"/>
    private string? locationName;

    /// <summary>
    /// The location's zoneId
    /// </summary>
    /// <seealso cref="GeoLocation.TimeZone"/>
    /// <seealso cref="GeoLocation.TimeZone"/>
    private TimeZoneInfo zoneId = TimeZoneInfo.Utc;

    /// <summary>
    /// The elevation in Meters <b>above</b> sea level.
    /// </summary>
    /// <seealso cref="Elevation"/>
    /// <seealso cref="Elevation"/>
    private double elevation;

    /// <summary>
    /// The elevation in Meters above sea level of the horizon at the point where the sun rises that day.
    /// This point is not simply due east and shifts through the year as the sunrise azimuth changes with the season.
    /// Defaults to 0.
    /// </summary>
    /// <seealso cref="HorizonElevationAtSunrise"/>
    /// <seealso cref="HorizonElevationAtSunrise"/>
    /// <seealso cref="SunriseElevation"/>
    private double horizonElevationAtSunrise = 0;

    /// <summary>
    /// The elevation in Meters above sea level of the horizon at the point where the sun sets that day.
    /// This point is not simply due west and shifts through the year as the sunset azimuth changes with the season.
    /// Defaults to 0.
    /// </summary>
    /// <seealso cref="HorizonElevationAtSunset"/>
    /// <seealso cref="HorizonElevationAtSunset"/>
    /// <seealso cref="SunsetElevation"/>
    private double horizonElevationAtSunset = 0;

    /// <summary>
    /// Constant for a distance type calculation.
    /// </summary>
    /// <seealso cref="GetGeodesicDistance"/>
    private const int Distance = 0;

    /// <summary>
    /// Constant for an initial bearing type calculation.
    /// </summary>
    /// <seealso cref="GetGeodesicInitialBearing"/>
    private const int InitialBearing = 1;

    /// <summary>
    /// Constant for a final bearing type calculation.
    /// </summary>
    /// <seealso cref="GetGeodesicFinalBearing"/>
    private const int FinalBearing = 2;

    /// <summary>
    /// constant for milliseconds in a minute (60,000)
    /// </summary>
    private const long MinuteMillis = 60 * 1000;

    /// <summary>
    /// constant for milliseconds in an hour (3,600,000)
    /// </summary>
    private const long HourMillis = MinuteMillis * 60;

    /// <summary>
    /// Method to return the elevation in Meters <b>above</b> sea level.
    /// </summary>
    /// <value>The elevation to set in Meters. An <see cref="ArgumentException"/> will be thrown if the value is a negative,
    ///         <see cref="double.IsNaN"/>  or <see cref="double.IsInfinity"/>.</value>
    /// <remarks>
    /// Method to set the elevation in Meters <b>above</b> sea level. Locations with an elevation below sea level should use a sea level
    /// value, since the delay in sunrise and sunset is due to obstructions (natural in this case) on the horizon that should be discounted.
    /// Natural obstructions such as the walls of the Grand Canyon that block visibility to the sun to a person at the bottom of the canyon
    /// (that is actually above sea level) for 3 to 4 hours after sunrise would not mean that <em>halachic</em> sunrise would be 3 hours after
    /// sea level sunrise and sunset 3 hours before sea level sunset at that location.
    /// </remarks>
    /// <seealso cref="Elevation"/>
    public double Elevation
    {
        get => elevation;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Elevation cannot be negative");
            }
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException("Elevation cannot be NaN or infinite");
            }
            this.elevation = value;

        }
    }

    /// <summary>
    /// Method to return the elevation in Meters above sea level of the horizon at the point where the sun rises that day.
    /// </summary>
    /// <value>the horizon elevation at sunrise in Meters. An <see cref="ArgumentException"/> will be thrown
    ///         if the value is a negative, <see cref="double.IsNaN"/> or <see cref="double.IsInfinity"/>.</value>
    /// <remarks>
    /// Method to set the elevation in Meters above sea level of the horizon at the point where the sun rises that day.
    /// This point is not simply due east and shifts through the year as the sunrise azimuth changes with the season.
    /// Sunrise is calculated with the observer's height above this horizon instead of above sea level.
    /// </remarks>
    /// <seealso cref="HorizonElevationAtSunrise"/>
    /// <seealso cref="SunriseElevation"/>
    public double HorizonElevationAtSunrise
    {
        get => horizonElevationAtSunrise;
        set
        {
            ValidateHorizonElevation(value);
            this.horizonElevationAtSunrise = value;

        }
    }

    /// <summary>
    /// Method to return the elevation in Meters above sea level of the horizon at the point where the sun sets that day.
    /// </summary>
    /// <value>the horizon elevation at sunset in Meters. An <see cref="ArgumentException"/> will be thrown
    ///         if the value is a negative, <see cref="double.IsNaN"/> or <see cref="double.IsInfinity"/>.</value>
    /// <remarks>
    /// Method to set the elevation in Meters above sea level of the horizon at the point where the sun sets that day.
    /// This point is not simply due west and shifts through the year as the sunset azimuth changes with the season.
    /// Sunset is calculated with the observer's height above this horizon instead of above sea level.
    /// </remarks>
    /// <seealso cref="HorizonElevationAtSunset"/>
    /// <seealso cref="SunsetElevation"/>
    public double HorizonElevationAtSunset
    {
        get => horizonElevationAtSunset;
        set
        {
            ValidateHorizonElevation(value);
            this.horizonElevationAtSunset = value;

        }
    }

    /// <summary>
    /// Throws if a horizon elevation is not a usable height in Meters above sea level.
    /// </summary>
    /// <param name="horizonElevation">the value to check.</param>
    private static void ValidateHorizonElevation(double horizonElevation)
    {
        if (horizonElevation < 0)
        {
            throw new ArgumentException("Horizon elevation cannot be negative");
        }
        if (double.IsNaN(horizonElevation) || double.IsInfinity(horizonElevation))
        {
            throw new ArgumentException("Horizon elevation cannot be NaN or infinite");
        }
    }

    /// <summary>
    /// The height in Meters of the observer above the <see cref="HorizonElevationAtSunrise">sunrise horizon</see>.
    /// A higher horizon returns 0, since <em>halachically</em> land blocking the sun does not delay sunrise.
    /// </summary>
    /// <returns>the observer's height above the sunrise horizon in Meters, never negative. With the default sea-level horizon
    ///         this is the same as <see cref="Elevation"/>.</returns>
    public double SunriseElevation => Math.Max(elevation - horizonElevationAtSunrise, 0);

    /// <summary>
    /// The height in Meters of the observer above the <see cref="HorizonElevationAtSunset">sunset horizon</see>.
    /// A higher horizon returns 0, since <em>halachically</em> land blocking the sun does not make sunset earlier.
    /// </summary>
    /// <returns>the observer's height above the sunset horizon in Meters, never negative. With the default sea-level horizon
    ///         this is the same as <see cref="Elevation"/>.</returns>
    public double SunsetElevation => Math.Max(elevation - horizonElevationAtSunset, 0);

    /// <summary>
    /// GeoLocation constructor with parameters for all required fields.
    /// </summary>
    /// <param name="name">The location name for display, for example "Lakewood, NJ".</param>
    /// <param name="latitude">the latitude as a <c>double</c>, for example 40.096 for Lakewood, NJ. <b>Note:</b> For latitudes south of
    ///         the equator, a negative value should be used.</param>
    /// <param name="longitude">the longitude as a <c>double</c>, for example -74.222 for Lakewood, NJ. <b>Note:</b> For longitudes west
    ///         of the <a href="https://en.wikipedia.org/wiki/Prime_Meridian">Prime Meridian</a> (Greenwich), a negative value should
    ///         be used.</param>
    /// <param name="zoneId">the <see cref="TimeZoneInfo"/> for the location.</param>
    public GeoLocation(string? name, double latitude, double longitude, TimeZoneInfo zoneId)
        : this(name, latitude, longitude, 0, zoneId)
    {
    }

    /// <summary>
    /// GeoLocation constructor with parameters for all required fields.
    /// </summary>
    /// <param name="name">The location name for display, for example "Lakewood, NJ".</param>
    /// <param name="latitude">the latitude as a <c>double</c>, for example 40.096 for Lakewood, NJ. <b>Note:</b> For latitudes south of
    ///         the equator, a negative value should be used.</param>
    /// <param name="longitude">the longitude as a <c>double</c>, for example -74.222 for Lakewood, NJ. <b>Note:</b> For longitudes west
    ///         of the <a href="https://en.wikipedia.org/wiki/Prime_Meridian">Prime Meridian</a> (Greenwich), a negative value should
    ///         be used.</param>
    /// <param name="elevation">the elevation above sea level in Meters.</param>
    /// <param name="zoneId">the <see cref="TimeZoneInfo"/> for the location.</param>
    public GeoLocation(string? name, double latitude, double longitude, double elevation, TimeZoneInfo zoneId)
    {
        LocationName = name;
        Latitude = latitude;
        Longitude = longitude;
        Elevation = elevation;
        TimeZone = zoneId;
    }

    /// <summary>
    /// GeoLocation constructor for a location whose horizon at sunrise or sunset is not at sea level.
    /// </summary>
    /// <param name="name">The location name for display, for example "Lakewood, NJ".</param>
    /// <param name="latitude">the latitude as a <c>double</c>, for example 40.096 for Lakewood, NJ. <b>Note:</b> For latitudes south of
    ///         the equator, a negative value should be used.</param>
    /// <param name="longitude">the longitude as a <c>double</c>, for example -74.222 for Lakewood, NJ. <b>Note:</b> For longitudes west
    ///         of the <a href="https://en.wikipedia.org/wiki/Prime_Meridian">Prime Meridian</a> (Greenwich), a negative value should
    ///         be used.</param>
    /// <param name="elevation">the elevation above sea level in Meters.</param>
    /// <param name="horizonElevationAtSunrise">the elevation above sea level in Meters of the horizon at the point where the sun rises
    ///         that day.</param>
    /// <param name="horizonElevationAtSunset">the elevation above sea level in Meters of the horizon at the point where the sun sets
    ///         that day.</param>
    /// <param name="zoneId">the <see cref="TimeZoneInfo"/> for the location.</param>
    /// <seealso cref="HorizonElevationAtSunrise"/>
    public GeoLocation(string? name, double latitude, double longitude, double elevation, double horizonElevationAtSunrise,
            double horizonElevationAtSunset, TimeZoneInfo zoneId) : this(name, latitude, longitude, elevation, zoneId)
    {
        HorizonElevationAtSunrise = horizonElevationAtSunrise;
        HorizonElevationAtSunset = horizonElevationAtSunset;
    }

    /// <summary>
    /// Default GeoLocation constructor will set location to the Prime Meridian at Greenwich, England and a <see cref="TimeZoneInfo"/>
    /// of GMT. The longitude will be set to 0 and the latitude will be 51.4772 to match the location of the <a
    /// href="https://www.rmg.co.uk/royal-observatory">Royal Observatory, Greenwich</a>. No daylight savings time will be used.
    /// </summary>
    public GeoLocation()
    {
        LocationName = "Greenwich, England";
        Longitude = 0; // added for clarity
        Latitude = 51.4772;
        TimeZone = Gmt;
    }

    /// <summary>
    /// The fixed <c>GMT</c> time zone (UTC offset 0, no daylight saving time) used by the default constructor, the equivalent of
    /// Java's <c>ZoneId.of("GMT")</c>.
    /// </summary>
    private static readonly TimeZoneInfo Gmt = TimeZoneInfo.CreateCustomTimeZone("GMT", TimeSpan.Zero, "Greenwich Mean Time",
            "Greenwich Mean Time");

    /// <summary>
    /// Method to return the latitude as a <c>double</c>, for example 40.096 for Lakewood, NJ.
    /// </summary>
    /// <value>The degrees of latitude to set. The values should be between -90° and 90°. For example 40.096 would be
    ///         used for Lakewood, NJ. <b>Note:</b> For latitudes south of the equator, a negative value should be used. An
    ///         <see cref="ArgumentException"/> will be thrown if the value exceeds the limits.</value>
    /// <remarks>
    /// Method to set the latitude as a <c>double</c>, for example 40.096 for Lakewood, NJ.
    /// </remarks>
    /// <exception cref="ArgumentException">if the latitude is not between -90 and 90.</exception>
    public double Latitude
    {
        get => latitude;
        set
        {
            if (value > 90 || value < -90 || double.IsNaN(value))
            {
                throw new ArgumentException("Latitude must be between -90 and  90");
            }
            this.latitude = value;

        }
    }

    /// <summary>
    /// Method to return the longitude as a <c>double</c>, for example -74.222 for Lakewood, NJ.
    /// </summary>
    /// <value>The degrees of longitude to set as a <c>double</c> between -180.0° and 180.0°. For example
    ///         -74.222 would be used for Lakewood, NJ. Note: for longitudes west of the <a href=
    ///         "https://en.wikipedia.org/wiki/Prime_Meridian">Prime Meridian</a> (Greenwich) a negative value should be used. An
    ///         <see cref="ArgumentException"/> will be thrown if the value exceeds the limit.</value>
    /// <remarks>
    /// Method to set the longitude as a <c>double</c>, for example -74.222 for Lakewood, NJ
    /// </remarks>
    /// <exception cref="ArgumentException">if the longitude is not between -180 and 180.</exception>
    public double Longitude
    {
        get => longitude;
        set
        {
            if (value > 180 || value < -180 || double.IsNaN(value))
            {
                throw new ArgumentException("Longitude must be between -180 and  180");
            }
            this.longitude = value;

        }
    }

    /// <summary>
    /// Method to return the location name (for display), for example "Lakewood, NJ".
    /// </summary>
    /// <value>the display name to be set.</value>
    /// <remarks>
    /// Setter for the location name (for display), for example "Lakewood, NJ".
    /// </remarks>
    public string? LocationName
    {
        get => locationName;
        set => locationName = value;
    }


    /// <summary>
    /// Method to return the <see cref="TimeZoneInfo"/>.
    /// </summary>
    /// <value>The time zone to set.</value>
    /// <remarks>
    /// The calendars convert every time they return to this zone when it is calculated, so changing the time zone of a
    /// <see cref="GeoLocation"/> already set in an <see cref="AstronomicalCalendar"/> takes effect immediately. Note that
    /// <see cref="AstronomicalCalendar.Clone"/> creates a copy of the <see cref="GeoLocation"/>.
    /// </remarks>
    public TimeZoneInfo TimeZone
    {
        get => zoneId;
        set => zoneId = value;
    }

    /// <summary>
    /// A method that will return the location's local mean time offset in milliseconds from the local clock time defined by the time
    /// zone offset in effect for the supplied <see cref="DateTimeOffset"/>. The globe is split into 360°, with 15° per hour of the day. For
    /// a local that is at a longitude that is evenly divisible by 15 (longitude % 15 == 0), at solar <see cref="AstronomicalCalendar.GetSunTransit()">noon</see> (with adjustment for the <a
    /// href="https://en.wikipedia.org/wiki/Equation_of_time">equation of time</a>) the sun should be directly overhead, so a user who
    /// is 1° west of this will have noon at 4 minutes after local clock noon, and conversely, a user who is 1° east of the 15°
    /// longitude will have noon at 11:56 AM local clock time. Lakewood, N.J., whose longitude is -74.222, is 0.778 away from the
    /// closest multiple of 15 at -75°. This is multiplied by 4 to yield 3 minutes and 10 seconds earlier than the local clock tim
    ///  derived from the zone offset in effect for the supplied instant, including any applicable <a href=
    ///  "https://en.wikipedia.org/wiki/Daylight_saving_time">Daylight saving time</a> adjustment.
    /// </summary>
    /// <param name="instant">the <see cref="DateTimeOffset"/> used to calculate the local mean offset for the date in question.</param>
    /// <returns>the offset in milliseconds relative to the time zone offset in effect at the supplied instant. A positive value will
    ///         be returned East of the 15° timezone line, and a negative value West of it.</returns>
    public long GetLocalMeanTimeOffset(DateTimeOffset instant)
    {
        long timezoneOffsetMillis = (long)zoneId.GetUtcOffset(instant).TotalSeconds * 1000L;
        return (long)(Longitude * 4 * MinuteMillis - timezoneOffsetMillis);
    }

    /// <summary>
    /// Adjust the date for <a href="https://en.wikipedia.org/wiki/180th_meridian">antimeridian</a> crossover. This is
    /// needed to deal with edge cases such as Samoa that use a different calendar date than expected based on their
    /// geographic location. The actual Time Zone offset may deviate from the expected offset based on the longitude. Since the
    /// "absolute time" calculations are always based on longitudinal offset from UTC for a given date, the date is presumed to only
    /// increase East of the Prime Meridian, and to only decrease West of it. For Time Zones that cross the antimeridian, the date
    /// will be artificially adjusted before calculation to conform with this presumption. For example, Apia, Samoa with a longitude
    /// of -171.75 uses a local offset of +14:00.  When calculating sunrise for 2018-02-03, the calculator should operate using
    /// 2018-02-02 since the expected zone is -11.  After determining the UTC time, the local DST offset of <a href=
    /// "https://en.wikipedia.org/wiki/UTC%2B14:00">UTC+14:00</a> should be applied to bring the date back to 2018-02-03.
    /// </summary>
    /// <param name="instant">the <see cref="DateTimeOffset"/> required for the local mean time offset calculation</param>
    /// <returns>the number of days to adjust the date This will typically be 0 unless the date crosses the antimeridian</returns>
    public int GetAntimeridianAdjustment(DateTimeOffset instant)
    {
        double localHoursOffset = GetLocalMeanTimeOffset(instant) / (double)HourMillis;

        if (localHoursOffset >= 20)
        {// if the offset is 20 hours or more in the future (never expected anywhere other
         // than a location using a timezone across the antimeridian to the east such as Samoa)
            return 1; // roll the date forward a day
        }
        else if (localHoursOffset <= -20)
        {    // if the offset is 20 hours or more in the past (no current location is known
             //that crosses the antimeridian to the west, but better safe than sorry)
            return -1; // roll the date back a day
        }
        return 0; //99.999% of the world will have no adjustment
    }

    /// <summary>
    /// Calculate the initial <a href="https://en.wikipedia.org/wiki/Great_circle">geodesic</a> bearing between this and another
    /// <c>GeoLocation</c> passed to this method using <a href="https://en.wikipedia.org/wiki/Thaddeus_Vincenty">Thaddeus
    /// Vincenty's</a> inverse formula See T Vincenty, <a href="https://www.ngs.noaa.gov/PUBS_LIB/inverse.pdf">"Direct and Inverse
    /// Solutions of Geodesics on the Ellipsoid with application of nested equations"</a>, Survey Review, vol. XXII no 176, 1975
    /// </summary>
    /// <param name="location">the destination location</param>
    /// <returns>the initial bearing</returns>
    public double GetGeodesicInitialBearing(GeoLocation location)
    {
        return VincentyInverseFormula(location, InitialBearing);
    }

    /// <summary>
    /// Calculate the final <a href="https://en.wikipedia.org/wiki/Great_circle">geodesic</a> bearing between this and another
    /// <c>GeoLocation</c> passed to this method using <a href="https://en.wikipedia.org/wiki/Thaddeus_Vincenty">Thaddeus
    /// Vincenty's</a> inverse formula See T Vincenty, <a href="https://www.ngs.noaa.gov/PUBS_LIB/inverse.pdf">"Direct and Inverse
    /// Solutions of Geodesics on the Ellipsoid with application of nested equations"</a>, Survey Review, vol. XXII no 176, 1975
    /// </summary>
    /// <param name="location">the destination location</param>
    /// <returns>the final bearing</returns>
    public double GetGeodesicFinalBearing(GeoLocation location)
    {
        return VincentyInverseFormula(location, FinalBearing);
    }

    /// <summary>
    /// Calculate <a href="https://en.wikipedia.org/wiki/Great-circle_distance">geodesic distance</a> in Meters between
    /// this Object and a second Object passed to this method using <a
    /// href="https://en.wikipedia.org/wiki/Thaddeus_Vincenty">Thaddeus Vincenty's</a> inverse formula See T Vincenty, "<a
    /// href="https://www.ngs.noaa.gov/PUBS_LIB/inverse.pdf">Direct and Inverse Solutions of Geodesics on the Ellipsoid
    /// with application of nested equations</a>", Survey Review, vol XXII no 176, 1975
    /// </summary>
    /// <param name="location">the destination location</param>
    /// <returns>the geodesic distance in Meters</returns>
    /// <seealso cref="VincentyInverseFormula"/>
    public double GetGeodesicDistance(GeoLocation location)
    {
        return VincentyInverseFormula(location, Distance);
    }

    /// <summary>
    /// Calculate <a href="https://en.wikipedia.org/wiki/Great-circle_distance">geodesic distance</a> in Meters between
    /// this Object and a second Object passed to this method using <a
    /// href="https://en.wikipedia.org/wiki/Thaddeus_Vincenty">Thaddeus Vincenty's</a> inverse formula See T Vincenty, "<a
    /// href="https://www.ngs.noaa.gov/PUBS_LIB/inverse.pdf">Direct and Inverse Solutions of Geodesics on the Ellipsoid
    /// with application of nested equations</a>", Survey Review, vol XXII no 176, 1975
    /// </summary>
    /// <param name="location">the destination location</param>
    /// <param name="formula">This formula calculates initial bearing (<see cref="InitialBearing"/>), final bearing (<see cref="FinalBearing"/>) and
    ///         distance (<see cref="Distance"/>).</param>
    /// <returns>geodesic distance in Meters</returns>
    private double VincentyInverseFormula(GeoLocation location, int formula)
    {
        double majorSemiAxis = 6378137;
        double minorSemiAxis = 6356752.3142;
        double f = 1 / 298.257223563; // WGS-84 ellipsoid
        double L = JavaMath.ToRadians(location.Longitude - Longitude);
        double U1 = Math.Atan((1 - f) * Math.Tan(JavaMath.ToRadians(Latitude)));
        double U2 = Math.Atan((1 - f) * Math.Tan(JavaMath.ToRadians(location.Latitude)));
        double sinU1 = Math.Sin(U1), cosU1 = Math.Cos(U1);
        double sinU2 = Math.Sin(U2), cosU2 = Math.Cos(U2);

        double lambda = L;
        double lambdaP = 2 * Math.PI;
        double iterLimit = 20;
        double sinLambda = 0;
        double cosLambda = 0;
        double sinSigma = 0;
        double cosSigma = 0;
        double sigma = 0;
        double sinAlpha;
        double cosSqAlpha = 0;
        double cos2SigmaM = 0;
        double C;
        while (Math.Abs(lambda - lambdaP) > 1e-12 && --iterLimit > 0)
        {
            sinLambda = Math.Sin(lambda);
            cosLambda = Math.Cos(lambda);
            sinSigma = Math.Sqrt((cosU2 * sinLambda) * (cosU2 * sinLambda)
                    + (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda) * (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda));
            if (sinSigma == 0)
                return 0; // co-incident points
            cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
            sigma = Math.Atan2(sinSigma, cosSigma);
            sinAlpha = cosU1 * cosU2 * sinLambda / sinSigma;
            cosSqAlpha = 1 - sinAlpha * sinAlpha;
            cos2SigmaM = cosSigma - 2 * sinU1 * sinU2 / cosSqAlpha;
            if (double.IsNaN(cos2SigmaM))
                cos2SigmaM = 0; // equatorial line: cosSqAlpha=0 (§6)
            C = f / 16 * cosSqAlpha * (4 + f * (4 - 3 * cosSqAlpha));
            lambdaP = lambda;
            lambda = L + (1 - C) * f * sinAlpha
                    * (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));
        }
        if (iterLimit == 0)
            return double.NaN; // formula failed to converge

        double uSq = cosSqAlpha * (majorSemiAxis * majorSemiAxis - minorSemiAxis * minorSemiAxis) / (minorSemiAxis * minorSemiAxis);
        double A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
        double B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
        double deltaSigma = B
                * sinSigma
                * (cos2SigmaM + B
                        / 4
                        * (cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM) - B / 6 * cos2SigmaM
                                * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaM * cos2SigmaM)));
        double distance = minorSemiAxis * A * (sigma - deltaSigma);

        // initial bearing
        double fwdAz = JavaMath.ToDegrees(Math.Atan2(cosU2 * sinLambda, cosU1 * sinU2 - sinU1 * cosU2 * cosLambda));
        // final bearing
        double revAz = JavaMath.ToDegrees(Math.Atan2(cosU1 * sinLambda, -sinU1 * cosU2 + cosU1 * sinU2 * cosLambda));
        if (formula == Distance)
        {
            return distance;
        }
        else if (formula == InitialBearing)
        {
            return fwdAz;
        }
        else if (formula == FinalBearing)
        {
            return revAz;
        }
        else
        { // should never happen
            return double.NaN;
        }
    }

    /// <summary>
    /// Returns the <a href="https://en.wikipedia.org/wiki/Rhumb_line">rhumb line</a> bearing from the current location to
    /// the GeoLocation passed in.
    /// </summary>
    /// <param name="location">destination location</param>
    /// <returns>the bearing in degrees</returns>
    public double GetRhumbLineBearing(GeoLocation location)
    {
        double dLon = JavaMath.ToRadians(location.Longitude - Longitude);
        double dPhi = Math.Log(Math.Tan(JavaMath.ToRadians(location.Latitude) / 2 + Math.PI / 4)
                / Math.Tan(JavaMath.ToRadians(Latitude) / 2 + Math.PI / 4));
        if (Math.Abs(dLon) > Math.PI)
            dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
        return JavaMath.ToDegrees(Math.Atan2(dLon, dPhi));
    }

    /// <summary>
    /// Returns the <a href="https://en.wikipedia.org/wiki/Rhumb_line">rhumb line</a> distance from the current location
    /// to the GeoLocation passed in.
    /// </summary>
    /// <param name="location">the destination location</param>
    /// <returns>the distance in Meters</returns>
    public double GetRhumbLineDistance(GeoLocation location)
    {
        double earthRadius = 6378137; // Earth's radius in meters (WGS-84)
        double dLat = JavaMath.ToRadians(location.Latitude) - JavaMath.ToRadians(Latitude);
        double dLon = Math.Abs(JavaMath.ToRadians(location.Longitude) - JavaMath.ToRadians(Longitude));
        double dPhi = Math.Log(Math.Tan(JavaMath.ToRadians(location.Latitude) / 2 + Math.PI / 4)
                / Math.Tan(JavaMath.ToRadians(Latitude) / 2 + Math.PI / 4));
        double q = dLat / dPhi;

        if (!(Math.Abs(q) <= double.MaxValue))
        {
            q = Math.Cos(JavaMath.ToRadians(Latitude));
        }
        // if dLon over 180° take shorter rhumb across 180° meridian:
        if (dLon > Math.PI)
        {
            dLon = 2 * Math.PI - dLon;
        }
        double d = Math.Sqrt(dLat * dLat + q * q * dLon * dLon);
        return d * earthRadius;
    }

    /// <summary>
    /// A method that returns an XML formatted <see cref="string"/> representing the serialized <see cref="object"/>. Very
    /// similar to the toString method but the return value is in an xml format. The format currently used (subject to
    /// change) is:
    ///
    /// <c>&lt;GeoLocation&gt;
    ///     &lt;LocationName&gt;Lakewood, NJ&lt;/LocationName&gt;
    ///        &lt;Latitude&gt;40.096°&lt;/Latitude&gt;
    ///     &lt;Longitude&gt;-74.222°&lt;/Longitude&gt;
    ///     &lt;Elevation&gt;0 Meters&lt;/Elevation&gt;
    ///        &lt;TimeZoneName&gt;America/New_York&lt;/TimeZoneName&gt;
    ///        &lt;TimeZoneDisplayName&gt;Eastern Standard Time&lt;/TimeZoneDisplayName&gt;
    /// &lt;/GeoLocation&gt;</c>
    /// </summary>
    /// <returns>The XML formatted <see cref="string"/>.</returns>
    public string ToXml()
    {
        return "<GeoLocation>\n" +
                "\t<LocationName>" + LocationName + "</LocationName>\n" +
                "\t<Latitude>" + JavaMath.ToJavaString(Latitude) + "</Latitude>\n" +
                "\t<Longitude>" + JavaMath.ToJavaString(Longitude) + "</Longitude>\n" +
                "\t<Elevation>" + JavaMath.ToJavaString(Elevation) + " Meters" + "</Elevation>\n" +
                "\t<HorizonElevationAtSunrise>" + JavaMath.ToJavaString(HorizonElevationAtSunrise) + " Meters" + "</HorizonElevationAtSunrise>\n" +
                "\t<HorizonElevationAtSunset>" + JavaMath.ToJavaString(HorizonElevationAtSunset) + " Meters" + "</HorizonElevationAtSunset>\n" +
                "\t<TimezoneName>" + TimeZone.Id + "</TimezoneName>\n" +
                "\t<TimeZoneDisplayName>" + TimeZone.StandardName + "</TimeZoneDisplayName>\n" +
                "</GeoLocation>";
    }

    /// <seealso cref="object.Equals(object)"/>
    public override bool Equals(object? obj) => Equals(obj as GeoLocation);

    /// <summary>
    /// Indicates whether this <see cref="GeoLocation"/> has the same latitude, longitude, elevations, name and time zone as
    /// <paramref name="other"/>.
    /// </summary>
    /// <param name="other">the location to compare with.</param>
    /// <returns><c>true</c> if both locations are equal.</returns>
    public bool Equals(GeoLocation? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        if (other is null || GetType() != other.GetType())
        {
            return false;
        }
        return latitude.Equals(other.latitude)
            && longitude.Equals(other.longitude)
            && elevation.Equals(other.elevation)
            && horizonElevationAtSunrise.Equals(other.horizonElevationAtSunrise)
            && horizonElevationAtSunset.Equals(other.horizonElevationAtSunset)
            && string.Equals(locationName, other.locationName, StringComparison.Ordinal)
            && string.Equals(zoneId.Id, other.zoneId.Id, StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), latitude, longitude, elevation, horizonElevationAtSunrise,
            horizonElevationAtSunset, locationName, zoneId.Id);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return "\nLocation Name:\t\t\t" + LocationName +
            "\nLatitude:\t\t\t" + JavaMath.ToJavaString(Latitude) + "\u00B0" +
            "\nLongitude:\t\t\t" + JavaMath.ToJavaString(Longitude) + "\u00B0" +
            "\nElevation:\t\t\t" + JavaMath.ToJavaString(Elevation) + " Meters" +
            "\nHorizon Elevation At Sunrise:\t" + JavaMath.ToJavaString(HorizonElevationAtSunrise) + " Meters" +
            "\nHorizon Elevation At Sunset:\t" + JavaMath.ToJavaString(HorizonElevationAtSunset) + " Meters" +
            "\nTimezone ID:\t\t\t" + TimeZone.Id +
            "\nTimezone Display Name:\t\t" + TimeZone.StandardName;
    }

    /// <summary>
    /// Creates a copy of this <see cref="GeoLocation"/>. All of its fields are values or immutable objects, so the copy is
    /// independent of the original.
    /// </summary>
    public GeoLocation Clone()
    {
        return (GeoLocation)MemberwiseClone();
    }
}
