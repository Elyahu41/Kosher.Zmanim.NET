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
using Kosher.Zmanim.Util;

using System.Diagnostics.CodeAnalysis;

namespace Kosher.Zmanim;

/// <summary>
/// A calendar that calculates astronomical times such as <see cref="GetSunrise">sunrise</see>, <see cref="GetSunset">sunset</see> and twilight
/// times. The date is set with the <see cref="LocalDate"/> property (a <see cref="DateOnly"/>), and every time returned is a
/// <see cref="DateTimeOffset"/> in the offset of the location's <see cref="Util.GeoLocation.TimeZone"/> at that instant. The calculation engine used to calculate the astronomical times can be changed to a different implementation by
/// implementing the abstract <see cref="AstronomicalCalculator"/> which is configured via
/// <see cref="AstronomicalCalculator"/>. A number of different calculation engine implementations are included
/// in the util package.
/// <b>Note:</b> There are times when the algorithms can't calculate proper values for sunrise, sunset and twilight. This is usually
/// caused by trying to calculate times for areas either very far North or South, where sunrise / sunset never happen on that date.
/// This is common when calculating deep twilight angles at high northern latitudes, such as London. The sun never reaches this dip at
/// certain times of the year. When the calculations encounter this condition a <c>null</c> will be returned when a <see cref="DateTimeOffset"/> or <see cref="TimeSpan"/> is expected. The reason that exceptions are not thrown in these cases is
/// because the lack of a rise/set or twilight is not an exception, but an expected condition in many parts of the world.
/// <para>Here is a simple example of how to use the API to calculate sunrise.
/// First create the AstronomicalCalendar for the location you would like to calculate sunrise or sunset times for:</para>
/// <code>
/// string locationName = "Lakewood, NJ";
/// double latitude = 40.0828; // Lakewood, NJ
/// double longitude = -74.2094; // Lakewood, NJ
/// double elevation = 20; // optional elevation correction in Meters
/// // any IANA (or Windows) time zone ID accepted by TimeZoneInfo.FindSystemTimeZoneById
/// TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
/// var location = new GeoLocation(locationName, latitude, longitude, elevation, timeZone);
/// var ac = new AstronomicalCalendar(location);
/// </code>
/// <para>To get the time of sunrise, first set the date you want (if not set, the date will default to today):</para>
/// <code>
/// ac.LocalDate = new DateOnly(1969, 2, 8);
/// DateTimeOffset? sunrise = ac.GetSunrise(); // null if the sun does not rise that day
/// </code>
/// </summary>
public class AstronomicalCalendar
{

    /// <summary>
    /// 90° below the vertical. Used as a basis for most calculations since the location of the sun is 90° below the horizon
    /// at sunrise and sunset.
    /// <b>Note </b>: it is important to note that for sunrise and sunset the <see cref="AstronomicalCalculator.AdjustZenith">adjusted zenith</see> is required to account for the radius of the sun and refraction. The adjusted zenith should
    /// not be used for calculations above or below 90° since they are usually calculated as an offset to 90°.
    /// </summary>
    public const double GeometricZenith = 90;

    /// <summary>
    /// Sun's zenith at civil twilight (96°).
    /// </summary>
    public const double CivilZenith = 96;

    /// <summary>
    /// Sun's zenith at nautical twilight (102°).
    /// </summary>
    public const double NauticalZenith = 102;

    /// <summary>
    /// Sun's zenith at astronomical twilight (108°).
    /// </summary>
    public const double AstronomicalZenith = 108;

    /// <summary>
    /// constant for nanoseconds in a minute (60 billion / 60,000,000,000)
    /// </summary>
    public const long MinuteNanos = 60_000_000_000L;

    /// <summary>
    /// constant for nanoseconds in an hour (3.6 trillion / 3,600,000,000,000)
    /// </summary>
    public const long HourNanos = 3_600_000_000_000L;

    /// <summary>
    /// The <see cref="DateOnly"/> encapsulated by this class to track the current date used by the class
    /// </summary>
    private DateOnly localDate;

    /// <summary>
    /// the <see cref="GeoLocation"/> used for calculations.
    /// </summary>
    private GeoLocation geoLocation;

    /// <summary>
    /// the internal <see cref="AstronomicalCalculator"/> used for calculating solar based times.
    /// </summary>
    private AstronomicalCalculator astronomicalCalculator;

    /// <summary>
    /// The GetSunrise method returns a <see cref="DateTimeOffset"/> representing the <see cref="AstronomicalCalculator.GetElevationAdjustment">elevation adjusted</see> sunrise time. The zenith used for the calculation uses <see cref="GeometricZenith">geometric zenith</see> of 90° plus <see cref="AstronomicalCalculator.GetElevationAdjustment"/>. This is
    /// adjusted by the <see cref="AstronomicalCalculator"/> to add approximately 50/60 of a degree to account for 34 arcminutes of
    /// refraction and 16 arcminutes for the sun's radius for a total of <see cref="AstronomicalCalculator.AdjustZenith">90.83333°</see>.
    /// See documentation for the specific implementation of the <see cref="AstronomicalCalculator"/> that you are using.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the exact sunrise time. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year where the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    /// <seealso cref="AstronomicalCalculator.AdjustZenith"/>
    /// <seealso cref="GetSeaLevelSunrise"/>
    /// <seealso cref="GetUTCSunrise"/>
    public virtual DateTimeOffset? GetSunrise()
    {
        double sunrise = GetUTCSunrise(GeometricZenith);
        if (double.IsNaN(sunrise))
        {
            return null;
        }
        else
        {
            return GetDateTimeOffsetFromTime(sunrise, SolarEvent.Sunrise);
        }
    }

    /// <summary>
    /// A method that returns the sunrise without <see cref="AstronomicalCalculator.GetElevationAdjustment">elevation
    /// adjustment</see>. Non-sunrise and sunset calculations such as dawn and dusk, depend on the amount of visible light,
    /// something that is not affected by elevation. This method returns sunrise calculated at sea level. This forms the
    /// base for dawn calculations that are calculated as a dip below the horizon before sunrise.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the exact sea-level sunrise time. If the calculation can't be computed
    ///         such as in the Arctic Circle where there is at least one day a year where the sun does not rise, and one
    ///         where it does not set, a <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    /// <seealso cref="GetSunrise"/>
    /// <seealso cref="GetUTCSeaLevelSunrise"/>
    /// <seealso cref="GetSeaLevelSunset"/>
    public virtual DateTimeOffset? GetSeaLevelSunrise()
    {
        double sunrise = GetUTCSeaLevelSunrise(GeometricZenith);
        if (double.IsNaN(sunrise))
        {
            return null;
        }
        else
        {
            return GetDateTimeOffsetFromTime(sunrise, SolarEvent.Sunrise);
        }
    }

    /// <summary>
    /// A method that returns the beginning of <a href="https://en.wikipedia.org/wiki/Twilight#Civil_twilight">civil twilight</a>
    /// (dawn) using a zenith of <see cref="CivilZenith">96°</see>.
    /// </summary>
    /// <returns>The <see cref="DateTimeOffset"/> of the beginning of civil twilight using a zenith of 96°. If the calculation
    ///         can't be computed, <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    public virtual DateTimeOffset? GetBeginCivilTwilight()
    {
        return GetSunriseOffsetByDegrees(CivilZenith);
    }

    /// <summary>
    /// A method that returns the beginning of <a href="https://en.wikipedia.org/wiki/Twilight#Nautical_twilight">nautical twilight</a>
    /// using a zenith of <see cref="NauticalZenith">102°</see>.
    /// </summary>
    /// <returns>The <see cref="DateTimeOffset"/> of the beginning of nautical twilight using a zenith of 102°. If the calculation
    ///         can't be computed <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    public virtual DateTimeOffset? GetBeginNauticalTwilight()
    {
        return GetSunriseOffsetByDegrees(NauticalZenith);
    }

    /// <summary>
    /// A method that returns the beginning of <a href="https://en.wikipedia.org/wiki/Twilight#Astronomical_twilight">astronomical
    /// twilight</a> using a zenith of <see cref="AstronomicalZenith">108°</see>.
    /// </summary>
    /// <returns>The <see cref="DateTimeOffset"/> of the beginning of astronomical twilight using a zenith of 108°. If the calculation
    ///         can't be computed, <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    public virtual DateTimeOffset? GetBeginAstronomicalTwilight()
    {
        return GetSunriseOffsetByDegrees(AstronomicalZenith);
    }

    /// <summary>
    /// The GetSunset method returns a <see cref="DateTimeOffset"/> representing the
    /// <see cref="AstronomicalCalculator.GetElevationAdjustment">elevation adjusted</see> sunset time. The zenith used for the
    /// calculation uses <see cref="GeometricZenith">geometric zenith</see> of 90° plus <see cref="AstronomicalCalculator.GetElevationAdjustment"/>. This is adjusted by the <see cref="AstronomicalCalculator"/> to add approximately 50/60 of a
    /// degree to account for 34 arcminutes of refraction and 16 arcminutes for the sun's radius for a total of <see cref="AstronomicalCalculator.AdjustZenith">90.83333°</see>. See documentation for the specific implementation of the
    /// <see cref="AstronomicalCalculator"/> that you are using.
    /// Note: In certain cases the calculates sunset will occur before sunrise. This will typically happen when a time zone other than
    /// the local timezone is used (calculating Los Angeles sunset using a GMT time zone for example). In this case the sunset date
    /// will be incremented to the following date.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the exact sunset time. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year where the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    /// <remarks>
    /// <b>To do:</b> update documentation for solar radius changes.
    /// </remarks>
    /// <seealso cref="AstronomicalCalculator.AdjustZenith"/>
    /// <seealso cref="GetSeaLevelSunset"/>
    /// <seealso cref="GetUTCSunset"/>
    public virtual DateTimeOffset? GetSunset()
    {
        double sunset = GetUTCSunset(GeometricZenith);
        if (double.IsNaN(sunset))
        {
            return null;
        }
        else
        {
            return GetDateTimeOffsetFromTime(sunset, SolarEvent.Sunset);
        }
    }

    /// <summary>
    /// A method that returns the sunset without <see cref="AstronomicalCalculator.GetElevationAdjustment">elevation adjustment</see>.
    /// Non-sunrise and sunset calculations such as dawn and dusk, depend on the amount of visible light, something that is not
    /// affected by elevation. This method returns sunset calculated at sea level. This forms the base for dusk calculations that are
    /// calculated as a dip below the horizon after sunset.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the exact sea-level sunset time. If the calculation can't be computed
    ///         such as in the Arctic Circle where there is at least one day a year where the sun does not rise, and one
    ///         where it does not set, a <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    /// <seealso cref="GetSunset"/>
    /// <seealso cref="GetUTCSeaLevelSunset"/>
    public virtual DateTimeOffset? GetSeaLevelSunset()
    {
        double sunset = GetUTCSeaLevelSunset(GeometricZenith);
        if (double.IsNaN(sunset))
        {
            return null;
        }
        else
        {
            return GetDateTimeOffsetFromTime(sunset, SolarEvent.Sunset);
        }
    }

    /// <summary>
    /// A method that returns the end of <a href="https://en.wikipedia.org/wiki/Twilight#Civil_twilight">civil twilight</a>
    /// using a zenith of <see cref="CivilZenith">96°</see>.
    /// </summary>
    /// <returns>The <see cref="DateTimeOffset"/> of the end of civil twilight using a zenith of <see cref="CivilZenith">96°</see>. If the
    ///         calculation can't be computed, <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    public virtual DateTimeOffset? GetEndCivilTwilight()
    {
        return GetSunsetOffsetByDegrees(CivilZenith);
    }

    /// <summary>
    /// A method that returns the end of nautical twilight using a zenith of <see cref="NauticalZenith">102°</see>.
    /// </summary>
    /// <returns>The <see cref="DateTimeOffset"/> of the end of nautical twilight using a zenith of <see cref="NauticalZenith">102°</see>. If the
    ///         calculation can't be computed, <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    public virtual DateTimeOffset? GetEndNauticalTwilight()
    {
        return GetSunsetOffsetByDegrees(NauticalZenith);
    }

    /// <summary>
    /// A method that returns the end of astronomical twilight using a zenith of <see cref="AstronomicalZenith">108°</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the end of astronomical twilight using a zenith of <see cref="AstronomicalZenith">108°</see>. If
    ///         the calculation can't be computed, <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    public virtual DateTimeOffset? GetEndAstronomicalTwilight()
    {
        return GetSunsetOffsetByDegrees(AstronomicalZenith);
    }

    /// <summary>
    /// A utility method that returns an <see cref="DateTimeOffset"/> offset by the offset time passed in. Please note that the level of light
    /// during twilight is not affected by elevation, so if this is being used to calculate an offset before sunrise or after sunset
    /// with the intent of getting a rough "level of light" calculation, the sunrise or sunset time passed to this method should be
    /// sea level sunrise and sunset.
    /// </summary>
    /// <param name="time">the start time</param>
    /// <param name="offset">the <see cref="TimeSpan"/> of the offset to add to the time.</param>
    /// <returns>the <see cref="DateTimeOffset"/> with the offset of the <see cref="TimeSpan"/> added to it, or <c>null</c> if either
    ///         parameter is <c>null</c></returns>
    public static DateTimeOffset? GetTimeOffset(DateTimeOffset? time, TimeSpan? offset)
    {
        if (time == null || offset == null)
        {
            return null;
        }
        return time.Plus(offset);
    }

    /// <summary>
    /// A utility method that returns the time of an offset by degrees below or above the horizon of <see cref="GetSunrise">sunrise</see>.
    /// Note that the degree offset is from the vertical, so for a calculation of 14° before sunrise, an offset of 14
    /// + <see cref="GeometricZenith"/> = 104 would have to be passed as a parameter.
    /// </summary>
    /// <param name="offsetZenith">the degrees before <see cref="GetSunrise"/> to use in the calculation. For time after sunrise use negative
    ///         numbers. Note that the degree offset is from the vertical, so for a calculation of 14° before sunrise, an offset
    ///         of 14 + <see cref="GeometricZenith"/> = 104 would have to be passed as a parameter.</param>
    /// <returns>The <see cref="DateTimeOffset"/> of the offset after (or before) <see cref="GetSunrise"/>. If the calculation
    ///         can't be computed such as in the Arctic Circle where there is at least one day a year where the sun does
    ///         not rise, and one where it does not set, a <c>null</c> will be returned. See detailed explanation
    ///         on top of the page.</returns>
    public virtual DateTimeOffset? GetSunriseOffsetByDegrees(double offsetZenith)
    {
        double dawn = GetUTCSunrise(offsetZenith);
        return double.IsNaN(dawn) ? null : GetDateTimeOffsetFromTime(dawn, SolarEvent.Sunrise);
    }

    /// <summary>
    /// A utility method that returns the time of an offset by degrees below or above the horizon of <see cref="GetSunset">sunset</see>. Note that the degree offset is from the vertical, so for a calculation of 14° after sunset, an offset of 14 +
    /// <see cref="GeometricZenith"/> = 104 would have to be passed as a parameter.
    /// </summary>
    /// <param name="offsetZenith">the degrees after <see cref="GetSunset"/> to use in the calculation. For time before sunset use negative
    ///         numbers. Note that the degree offset is from the vertical, so for a calculation of 14° after sunset, an offset
    ///         of 14 + <see cref="GeometricZenith"/> = 104 would have to be passed as a parameter.</param>
    /// <returns>The <see cref="DateTimeOffset"/> of the offset after (or before) <see cref="GetSunset"/>. If the calculation
    ///         can't be computed such as in the Arctic Circle where there is at least one day a year where the sun does not rise, and
    ///         and one where it does not set, a <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    public virtual DateTimeOffset? GetSunsetOffsetByDegrees(double offsetZenith)
    {
        double sunset = GetUTCSunset(offsetZenith);
        return double.IsNaN(sunset) ? null : GetDateTimeOffsetFromTime(sunset, SolarEvent.Sunset);
    }

    /// <summary>
    /// Default constructor will set a default <see cref="GeoLocation.GeoLocation(string, double, double, TimeZoneInfo)"/>, a default <see cref="AstronomicalCalculator.GetDefault">AstronomicalCalculator</see> and default the <see cref="DateOnly"/> to the current date.
    /// </summary>
    public AstronomicalCalendar() : this(new GeoLocation())
    {
    }

    /// <summary>
    /// A constructor that takes in <a href="https://en.wikipedia.org/wiki/Geolocation">geolocation</a> information as a parameter.
    /// The default <see cref="AstronomicalCalculator.GetDefault">AstronomicalCalculator</see> used for solar calculations is the more
    /// accurate <see cref="NOAACalculator"/>.
    /// </summary>
    /// <param name="geoLocation">The location information used for calculating astronomical solar times.</param>
    /// <seealso cref="AstronomicalCalendar.AstronomicalCalculator"/>
    public AstronomicalCalendar(GeoLocation geoLocation)
    {
        ArgumentNullException.ThrowIfNull(geoLocation);
        localDate = JavaTime.Today(geoLocation.TimeZone);
        this.geoLocation = geoLocation;
        astronomicalCalculator = AstronomicalCalculator.GetDefault();
    }

    /// <summary>
    /// A method that returns the sunrise in UTC time without correction for time zone offset from GMT and without using daylight
    /// savings time.
    /// </summary>
    /// <param name="zenith">the degrees below the horizon. For time after sunrise use negative numbers.</param>
    /// <returns>The time in the format: 18.75 for 18:45:00 UTC/GMT. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year where the sun does not rise, and one where it does
    ///         not set, <see cref="double.NaN"/> will be returned. See detailed explanation on top of the page.</returns>
    public virtual double GetUTCSunrise(double zenith)
    {
        return AstronomicalCalculator.GetCachedUTCSunrise(GetAdjustedLocalDate(), GeoLocation, zenith, true);
    }

    /// <summary>
    /// A method that returns the sunrise in UTC time without correction for time zone offset from GMT and without using
    /// daylight savings time. Non-sunrise and sunset calculations such as dawn and dusk, depend on the amount of visible
    /// light, something that is not affected by elevation. This method returns UTC sunrise calculated at sea level. This
    /// forms the base for dawn calculations that are calculated as a dip below the horizon before sunrise.
    /// </summary>
    /// <param name="zenith">the degrees below the horizon. For time after sunrise use negative numbers.</param>
    /// <returns>The time in the format: 18.75 for 18:45:00 UTC/GMT. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year where the sun does not rise, and one where it does
    ///         not set, <see cref="double.NaN"/> will be returned. See detailed explanation on top of the page.</returns>
    /// <seealso cref="GetUTCSunrise"/>
    /// <seealso cref="GetUTCSeaLevelSunset"/>
    public virtual double GetUTCSeaLevelSunrise(double zenith)
    {
        return AstronomicalCalculator.GetCachedUTCSunrise(GetAdjustedLocalDate(), GeoLocation, zenith, false);
    }

    /// <summary>
    /// A method that returns the sunset in UTC time without correction for time zone offset from GMT and without using
    /// daylight savings time.
    /// </summary>
    /// <param name="zenith">the degrees below the horizon. For time after sunset use negative numbers.</param>
    /// <returns>The time in the format: 18.75 for 18:45:00 UTC/GMT. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year where the sun does not rise, and one where it does
    ///         not set, <see cref="double.NaN"/> will be returned. See detailed explanation on top of the page.</returns>
    /// <seealso cref="GetUTCSeaLevelSunset"/>
    public virtual double GetUTCSunset(double zenith)
    {
        return AstronomicalCalculator.GetCachedUTCSunset(GetAdjustedLocalDate(), GeoLocation, zenith, true);
    }

    /// <summary>
    /// A method that returns the sunset in UTC time without correction for elevation, time zone offset from GMT and without using
    /// daylight savings time. Non-sunrise and sunset calculations such as dawn and dusk, depend on the amount of visible light,
    /// something that is not affected by elevation. This method returns UTC sunset calculated at sea level. This forms the base for
    /// dusk calculations that are calculated as a dip below the horizon after sunset.
    /// </summary>
    /// <param name="zenith">the degrees below the horizon. For time before sunset use negative numbers.</param>
    /// <returns>The time in the format: 18.75 for 18:45:00 UTC/GMT. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year where the sun does not rise, and one where it does
    ///         not set, <see cref="double.NaN"/> will be returned. See detailed explanation on top of the page.</returns>
    /// <seealso cref="GetUTCSunset"/>
    /// <seealso cref="GetUTCSeaLevelSunrise"/>
    public virtual double GetUTCSeaLevelSunset(double zenith)
    {
        return AstronomicalCalculator.GetCachedUTCSunset(GetAdjustedLocalDate(), GeoLocation, zenith, false);
    }

    /// <summary>
    /// A method that returns a sea-level based temporal (solar) hour. The day from <see cref="GetSeaLevelSunrise">sea-level sunrise</see> to
    /// <see cref="GetSeaLevelSunset">sea-level sunset</see> is split into 12 equal parts with each one being a temporal hour.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of the temporal hour. If the calculation can't be computed a <c>null</c> will be
    ///         returned. See detailed explanation on top of the page.</returns>
    /// <seealso cref="GetSeaLevelSunrise"/>
    /// <seealso cref="GetSeaLevelSunset"/>
    /// <seealso cref="GetTemporalHour()"/>
    public virtual TimeSpan? GetTemporalHour()
    {
        return GetTemporalHour(GetSeaLevelSunrise(), GetSeaLevelSunset());
    }

    /// <summary>
    /// A utility method that will allow the calculation of a temporal (solar) hour based on the sunrise and sunset passed as
    /// parameters to this method. An example of the use of this method would be the calculation of a elevation adjusted temporal
    /// hour by passing in <see cref="GetSunrise">sunrise</see> and <see cref="GetSunset">sunset</see> as parameters.
    /// </summary>
    /// <param name="startOfDay">The start of the day.</param>
    /// <param name="endOfDay">The end of the day.</param>
    /// <returns>the <see cref="TimeSpan"/> of the temporal hour. If the calculation can't be computed a <c>null</c> will be
    ///         returned. See detailed explanation on top of the page.</returns>
    /// <seealso cref="GetTemporalHour()"/>
    public virtual TimeSpan? GetTemporalHour(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay)
    {
        if (startOfDay == null || endOfDay == null)
        {
            return null;
        }

        return JavaTime.Between(startOfDay, endOfDay).DividedBy(12);
    }

    /// <summary>
    /// A method that returns sundial or solar noon. It occurs when the Sun is <a href=
    /// "https://en.wikipedia.org/wiki/Transit_%28astronomy%29">transiting</a> the <a
    /// href="https://en.wikipedia.org/wiki/Meridian_%28astronomy%29">celestial meridian</a>. The calculations used by this class
    /// depend on the <see cref="AstronomicalCalculator"/> used. If this calendar instance is <see cref="AstronomicalCalculator"/> is set to use the <see cref="NOAACalculator"/> (the default) it will calculate
    /// astronomical noon. If the calendar instance is  to use the <see cref="SunTimesCalculator"/>, that does
    /// not have code to calculate astronomical noon, the sun transit is calculated as halfway between sea level sunrise and sea level
    /// sunset, which can be slightly off the real transit time due to changes in declination (the lengthening or shortening day). See
    /// <a href="https://kosherjava.com/2020/07/02/definition-of-chatzos/">The Definition of Chatzos</a> for details on the proper
    /// definition of solar noon / midday.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing Sun's transit. If the calculation can't be computed such as when using the <see cref="SunTimesCalculator">USNO calculator</see> that does not support getting solar noon for the Arctic
    ///         Circle (where there is at least one day a year where the sun does not rise, and one where it does not set), a
    ///         <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    /// <seealso cref="GetSunTransit()"/>
    /// <seealso cref="GetTemporalHour()"/>
    /// <seealso cref="NOAACalculator.GetUTCNoon"/>
    /// <seealso cref="SunTimesCalculator.GetUTCNoon"/>
    public virtual DateTimeOffset? GetSunTransit()
    {
        double noon = AstronomicalCalculator.GetCachedUTCNoon(GetAdjustedLocalDate(), GeoLocation);
        return GetDateTimeOffsetFromTime(noon, SolarEvent.Noon);
    }

    /// <summary>
    /// A method that returns solar midnight as the <b>end of the day</b> (that may actually be after midnight of the  day it is
    /// being calculated for). For example calculating solar midnight for February 8, will calculate it for midnight between February
    /// 8 and February 9. It occurs when the Sun is <a href="https://en.wikipedia.org/wiki/Transit_%28astronomy%29">transiting</a> the
    /// lower <a href="https://en.wikipedia.org/wiki/Meridian_%28astronomy%29">celestial meridian</a>, or when the sun is at it's
    /// <a href="https://en.wikipedia.org/wiki/Nadir">nadir</a>. The calculations used by this class depend on the <see cref="AstronomicalCalculator"/> used. If this calendar instance is <see cref="AstronomicalCalculator">set</see> to
    /// use the <see cref="NOAACalculator"/> (the default) it will calculate astronomical midnight. If the
    /// calendar instance is to use the <see cref="SunTimesCalculator">USNO Calculator</see>, that does not have code
    /// to calculate astronomical noon, midnight is calculated as 12 hours after halfway between sea level sunrise and sea level sunset
    /// of that day. This can be slightly off the real transit time due to changes in declination (the lengthening or shortening day).
    /// See <a href="https://kosherjava.com/2020/07/02/definition-of-chatzos/">The Definition of Chatzos</a> for details on the proper
    /// definition of solar noon / midday.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing Sun's lower transit at the <b>end of the current day</b>. If the calculation
    ///         can't be computed such as when using the <see cref="SunTimesCalculator">USNO calculator</see> that does
    ///         not support getting solar noon or midnight for the Arctic Circle (where there is at least one day a year where the sun
    ///         does not rise, and one where it does not set), a <c>null</c> will be returned. This is not relevant when using the
    ///         <see cref="NOAACalculator">NOAA Calculator</see> that is never expected to return <c>null</c>.
    ///         See the detailed explanation on top of the page.</returns>
    /// <seealso cref="GetSunTransit()"/>
    /// <seealso cref="NOAACalculator.GetUTCNoon"/>
    /// <seealso cref="SunTimesCalculator.GetUTCNoon"/>
    public virtual DateTimeOffset? GetSolarMidnight()
    {
        double noon = AstronomicalCalculator.GetCachedUTCMidnight(GetAdjustedLocalDate(), GeoLocation);
        return GetDateTimeOffsetFromTime(noon, SolarEvent.Midnight);
    }

    /// <summary>
    /// A method that returns sundial or solar noon (or midnight) calculated as halfway between the times passed in. It is close to,
    /// but not exactly  occurs when the Sun is <a href="https://en.wikipedia.org/wiki/Transit_%28astronomy%29">transiting</a> the
    /// <a href="https://en.wikipedia.org/wiki/Meridian_%28astronomy%29">celestial meridian</a>. It will not exactly match the
    /// astronomical transit, due to changes in declination (the lengthening or shortening day).
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating the sun's transit. This can be sea level sunrise, visual sunrise (or any
    ///         arbitrary start of day) passed to this method.</param>
    /// <param name="endOfDay">the end of day for calculating the sun's transit. This can be sea level sunset, visual sunset (or any arbitrary
    ///         end of day) passed to this method.</param>
    /// <returns>the <see cref="DateTimeOffset"/> representing Sun's transit. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year where the sun does not rise, and one where it does
    ///         not set, <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    public virtual DateTimeOffset? GetSunTransit(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay)
    {
        TimeSpan? temporalHour = GetTemporalHour(startOfDay, endOfDay);
        if (temporalHour == null)
        {
            return null;
        }
        return ToZone(GetTimeOffset(startOfDay, temporalHour.MultipliedBy(6)));
    }

    /// <summary>
    /// An <c>enum</c> to indicate what type of solar event is being calculated.
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
    /// Return the time at either an azimuth 90° (directly east) or 270° (directly west).
    /// </summary>
    /// <param name="azimuth">the azimuth that you want to get the time of day for.</param>
    /// <returns>the time that the azimuth will be reached. There are cases where this azimuth will never be reached for the date and
    ///         location, and a null will be returned in that case.</returns>
    /// <exception cref="ArgumentException">if the azimuth is not 90° or 270°.</exception>
    /// <remarks>
    /// <b>To do:</b> Once a reliable implementation to get any azimuth at any date for any latitude is implemented, make this method more
    ///         generic.
    /// </remarks>
    /// <seealso cref="AstronomicalCalculator.GetTimeAtAzimuth"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetPolarSunriseBenIshChai"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetPolarSunsetBenIshChai"/>
    public virtual DateTimeOffset? GetTimeAtAzimuth90Or270(double azimuth)
    {
        double rawAzimuth = AstronomicalCalculator.GetTimeAtAzimuth(GetAdjustedLocalDate(), GeoLocation, azimuth);
        return GetDateTimeOffsetFromTime(rawAzimuth, (azimuth == 90) ? SolarEvent.Sunrise : SolarEvent.Sunset);
    }

    /// <summary>
    /// A method that returns an <see cref="DateTimeOffset"/> from the <c>double</c> passed in as a parameter.
    /// </summary>
    /// <param name="time">The time to be set as the time for the <see cref="DateTimeOffset"/>. The time expected is in the format: 18.75 for 6:45:00 PM.</param>
    /// <param name="solarEvent">the type of <see cref="SolarEvent"/></param>
    /// <returns>The <see cref="DateTimeOffset"/> representation of the time double, or <c>null</c> if it is <see cref="double.NaN"/></returns>
    protected virtual DateTimeOffset? GetDateTimeOffsetFromTime(double time, SolarEvent solarEvent)
    {
        if (double.IsNaN(time))
        {
            return null;
        }

        DateOnly date = GetAdjustedLocalDate();
        double localTimeHours = (GeoLocation.Longitude / 15) + time;

        // The calculator gives only the UTC time of day, a number from 0 to 24. It doesn't say which UTC day the event
        // is on, so start from the calculation date and correct it.
        //
        // localTimeHours is that time in local mean time, which is UTC plus the longitude's hours. It can come out below 0 or
        // above 24, which means the event is really on the day before or the day after the date. Each event has a
        // window of local mean time that belongs to the date, and these checks move the date by a day when the event
        // falls outside it.
        //
        // - Sunrise belongs to the date from 18:00 the evening before through 18:00 of the date. After 18:00 it is
        //   really the evening before (an Arctic sunrise just after midnight, say), so go back a day. At -6:00 or
        //   earlier the UTC time wrapped around midnight, so go forward a day. For example, just west of the date line
        //   local mean time is about 12 hours behind UTC. A sunrise at noon local mean time is then 24:00 UTC, which
        //   arrives as 0:00 and reads as -12:00 local mean time.
        // - Sunset belongs from 06:00 of the date through 06:00 the next morning. Before 06:00 go forward a day. At
        //   30:00, which is 06:00 the next morning, or later the UTC time wrapped the other way, so go back a day.
        // - Solar midnight is the one after the date's noon, so one before 12:00 moves forward a day.
        // - Solar noon belongs between 0:00 and 24:00. Before 0:00 go forward a day, and after 24:00 go back a day.
        if (solarEvent == SolarEvent.Sunrise && localTimeHours > 18)
        {
            date = date.AddDays(-1);
        }
        else if (solarEvent == SolarEvent.Sunrise && localTimeHours <= -6)
        {
            date = date.AddDays(1);
        }
        else if (solarEvent == SolarEvent.Sunset && localTimeHours < 6)
        {
            date = date.AddDays(1);
        }
        else if (solarEvent == SolarEvent.Sunset && localTimeHours >= 30)
        {
            date = date.AddDays(-1);
        }
        else if (solarEvent == SolarEvent.Midnight && localTimeHours < 12)
        {
            date = date.AddDays(1);
        }
        else if (solarEvent == SolarEvent.Noon)
        {
            if (localTimeHours < 0)
            {
                date = date.AddDays(1);
            }
            else if (localTimeHours > 24)
            {
                date = date.AddDays(-1);
            }
        }

        DateTimeOffset? dateTime = JavaTime.AtStartOfDayUtc(date).AddTicks(JavaTime.NanosToTicks(JavaMath.Round(time * HourNanos)));

        // The computed time is in UTC fractional hours; anchor in UTC before converting to the location's time zone.
        return ToZone(dateTime);
    }

    /// <summary>
    /// Returns the sun's elevation (number of degrees) below the horizon before sunrise that matches the offset minutes
    /// on passed in as a parameter. For example passing in 72 minutes for a calendar set to the equinox in Jerusalem
    /// returns a value close to 16.1°.
    /// </summary>
    /// <param name="minutes">minutes before sunrise</param>
    /// <returns>the degrees below the horizon before <see cref="GetSeaLevelSunrise"/> that match the offset in minutes passed in as a
    ///         parameter. If the calculation can't be computed (no sunrise occurs on this day) a <see cref="double.NaN"/> will be returned.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method is slow and inefficient and should NEVER be used in a loop. This method should be replaced by calls to
    ///         <see cref="AstronomicalCalculator.GetSolarElevation"/>. That method will efficiently return the solar
    ///         elevation (the sun's position in degrees below (or above) the horizon) at the given time even in the arctic when there
    ///         is no sunrise.
    /// </remarks>
    /// <seealso cref="AstronomicalCalculator.GetSolarElevation"/>
    /// <seealso cref="GetSunsetSolarDipFromOffset"/>
    [Obsolete("This method is slow and inefficient and should NEVER be used in a loop.")]
    public virtual double GetSunriseSolarDipFromOffset(double minutes)
    {
        if (minutes == 0.0)
        {
            return 0.0;
        }
        if (double.IsNaN(minutes))
        {
            return double.NaN;
        }

        DateTimeOffset? seaLevelSunrise = GetSeaLevelSunrise();
        if (seaLevelSunrise == null)
        {
            return double.NaN;
        }

        TimeSpan? offsetDuration = JavaTime.DurationOfNanos(JavaMath.ToLong(-minutes * MinuteNanos));
        DateTimeOffset? offsetByTime = GetTimeOffset(seaLevelSunrise, offsetDuration);
        long offsetByTimeMilli = offsetByTime.ToEpochMilli();
        double degrees = 0.0;
        double incrementor = 0.0001;
        DateTimeOffset? offsetByDegrees;

        do
        {
            if (minutes > 0.0)
            {
                degrees += incrementor;
            }
            else
            {
                degrees -= incrementor;
            }

            offsetByDegrees = GetSunriseOffsetByDegrees(GeometricZenith + degrees);

            if (offsetByDegrees == null || Math.Abs(degrees) > 30.0)
            {
                return double.NaN;
            }

        } while ((minutes > 0.0 && offsetByDegrees.ToEpochMilli() > offsetByTimeMilli) ||
                (minutes < 0.0 && offsetByDegrees.ToEpochMilli() < offsetByTimeMilli));

        return degrees;
    }

    /// <summary>
    /// Returns the sun's elevation (number of degrees) below the horizon after sunset that matches the offset minutes
    /// passed in as a parameter. For example passing in 72 minutes for a date set to the equinox in Jerusalem
    /// returns a value close to 16.1°.
    /// </summary>
    /// <param name="minutes">minutes after sunset</param>
    /// <returns>the degrees below the horizon after sunset that match the offset in minutes passed it as a parameter. If the
    ///         calculation can't be computed (no sunset occurs on this day) a <see cref="double.NaN"/> will be returned.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method is slow and inefficient and should NEVER be used in a loop. This method should be replaced by calls to
    ///         <see cref="AstronomicalCalculator.GetSolarElevation"/>. That method will efficiently return the solar
    ///         elevation (the sun's position in degrees below (or above) the horizon) at the given time even in the arctic when there
    ///         is no sunrise.
    /// </remarks>
    /// <seealso cref="AstronomicalCalculator.GetSolarElevation"/>
    /// <seealso cref="GetSunriseSolarDipFromOffset"/>
    [Obsolete("This method is slow and inefficient and should NEVER be used in a loop.")]
    public virtual double GetSunsetSolarDipFromOffset(double minutes)
    {
        if (minutes == 0.0)
        {
            return 0.0;
        }
        if (double.IsNaN(minutes))
        {
            return double.NaN;
        }

        DateTimeOffset? seaLevelSunset = GetSeaLevelSunset();
        if (seaLevelSunset == null)
        {
            return double.NaN;
        }

        TimeSpan? offsetDuration = JavaTime.DurationOfNanos(JavaMath.ToLong(minutes * MinuteNanos));
        DateTimeOffset? offsetByTime = GetTimeOffset(seaLevelSunset, offsetDuration);
        long offsetByTimeMilli = offsetByTime.ToEpochMilli();
        double degrees = 0.0;
        double incrementor = 0.0001;
        DateTimeOffset? offsetByDegrees;

        do
        {
            if (minutes > 0.0)
            {
                degrees += incrementor;
            }
            else
            {
                degrees -= incrementor;
            }

            offsetByDegrees = GetSunsetOffsetByDegrees(GeometricZenith + degrees);

            if (offsetByDegrees == null || Math.Abs(degrees) > 30.0)
            {
                return double.NaN;
            }

        } while ((minutes > 0.0 && offsetByDegrees.ToEpochMilli() < offsetByTimeMilli) ||
                (minutes < 0.0 && offsetByDegrees.ToEpochMilli() > offsetByTimeMilli));

        return degrees;
    }

    /// <summary>
    /// A method that returns <a href="https://en.wikipedia.org/wiki/Local_mean_time">local mean time (LMT)</a> for a <see cref="TimeOnly"/>
    /// passed to this method. This time is adjusted from standard time to account for the local longitude. The 360° of the globe
    /// divided by 24 calculates to 15° per hour with 4 minutes per degree, so at a longitude of 0 , 15, 30 etc... noon is at exactly
    /// 12:00 PM. Lakewood, NJ, with a longitude of -74.222, is 0.7906 away from the closest multiple of 15 at -75°. This is
    /// multiplied by 4 clock minutes (per degree) to yield 3 minutes and 7 seconds for a noon time of 11:56:53 AM. This method is not
    /// tied to the theoretical 15° time zones, but will adjust to the actual time zone and <a href=
    /// "https://en.wikipedia.org/wiki/Daylight_saving_time">Daylight saving time</a> to return LMT.
    /// </summary>
    /// <param name="localTime">the local wall-clock time (such as 12:00 for noon and 00:00 for midnight) to calculate as LMT.</param>
    /// <returns>the <see cref="DateTimeOffset"/> representing the local mean time (LMT) for the time passed in. In Lakewood, NJ, passing noon
    ///         will return 11:56:50 AM.</returns>
    /// <seealso cref="GeoLocation.GetLocalMeanTimeOffset"/>
    public virtual DateTimeOffset? GetLocalMeanTime(TimeOnly localTime)
    {
        DateTimeOffset? localMeanTime = new DateTimeOffset(GetAdjustedLocalDate().ToDateTime(localTime, DateTimeKind.Unspecified), TimeSpan.Zero);
        long totalNanos = JavaMath.ToLong(GeoLocation.Longitude * 4 * MinuteNanos);
        return ToZone(GetTimeOffset(localMeanTime, JavaTime.DurationOfNanos(-totalNanos)));
    }

    /// <summary>
    /// Adjusts the <see cref="DateOnly"/> to deal with edge cases where the location crosses the antimeridian.
    /// </summary>
    /// <returns>the adjusted <see cref="DateOnly"/></returns>
    /// <seealso cref="GeoLocation.GetAntimeridianAdjustment"/>
    protected virtual DateOnly GetAdjustedLocalDate()
    {
        int offset = GeoLocation.GetAntimeridianAdjustment(GetMidnightLastNight());
        return offset == 0 ? LocalDate : LocalDate.AddDays(offset);
    }

    /// <summary>
    /// Used by Molad based <em>zmanim</em> to determine if <em>zmanim</em> occur during the current day. This is also used as the
    /// anchor for current timezone-offset calculations.
    /// </summary>
    /// <returns>midnight at the start of the current local date in the configured <see cref="GeoLocation.TimeZone"/>.</returns>
    protected virtual DateTimeOffset GetMidnightLastNight()
    {
        return JavaTime.AtZone(LocalDate, TimeOnly.MinValue, GeoLocation.TimeZone);
    }

    /// <summary>
    /// Used by Molad based <em>zmanim</em> to determine if <em>zmanim</em> occur during the current day.
    /// </summary>
    /// <returns>following midnight</returns>
    protected virtual DateTimeOffset GetMidnightTonight()
    {
        return JavaTime.AtZone(LocalDate.AddDays(1), TimeOnly.MinValue, GeoLocation.TimeZone);
    }

    /// <summary>
    /// Returns an XML formatted representation of the class using the default output of the
    ///         <see cref="ZmanimFormatter.ToXml">toXML</see> method.
    /// </summary>
    /// <returns>an XML formatted representation of the class. It returns the default output of the
    ///         <see cref="ZmanimFormatter.ToXml">toXML</see> method.</returns>
    /// <seealso cref="ZmanimFormatter.ToXml"/>
    /// <seealso cref="object.ToString"/>
    public override string ToString()
    {
        return ZmanimFormatter.ToXml(this);
    }

    /// <summary>
    /// Returns a JSON formatted representation of the class using the default output of the
    ///         <see cref="ZmanimFormatter.ToJson">toJSON</see> method.
    /// </summary>
    /// <returns>a JSON formatted representation of the class. It returns the default output of the
    ///         <see cref="ZmanimFormatter.ToJson">toJSON</see> method.</returns>
    /// <seealso cref="ZmanimFormatter.ToJson"/>
    /// <seealso cref="object.ToString"/>
    [RequiresUnreferencedCode("Discovers zmanim methods with reflection.")]
    public virtual string ToJson()
    {
        return ZmanimFormatter.ToJson(this);
    }

    /// <summary>
    /// Two <c>AstronomicalCalendar</c> instances are considered equal if their <see cref="LocalDate"/>, <see cref="GeoLocation"/>
    /// and <see cref="AstronomicalCalculator"/> values are identical.
    /// </summary>
    /// <param name="obj">the reference object with which to compare</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is equal to this instance; otherwise <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        AstronomicalCalendar aCal = (AstronomicalCalendar)obj;
        return Equals(LocalDate, aCal.LocalDate)
                && Equals(GeoLocation, aCal.GeoLocation)
                && Equals(AstronomicalCalculator, aCal.AstronomicalCalculator);
    }

    /// <summary>
    /// This implementation hashes the runtime type, <see cref="LocalDate"/>, <see cref="GeoLocation"/> and
    /// <see cref="AstronomicalCalculator"/> properties to maintain the contract with <see cref="Equals"/>.
    /// </summary>
    /// <returns>a hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), LocalDate, GeoLocation, AstronomicalCalculator);
    }

    /// <summary>
    /// A method that returns the currently set <see cref="GeoLocation"/> which contains location information used for the
    /// astronomical calculations.
    /// </summary>
    /// <value>The geoLocation to set.</value>
    /// <remarks>
    /// Sets the <see cref="GeoLocation"/> <see cref="object"/> to be used for astronomical calculations.
    /// </remarks>
    public GeoLocation GeoLocation
    {
        get => geoLocation;
        set => geoLocation = value;
    }

    /// <summary>
    /// A method that returns the currently set AstronomicalCalculator.
    /// </summary>
    /// <value>The <c>AstronomicalCalculator</c> to set.</value>
    /// <remarks>
    /// A method to set the <see cref="AstronomicalCalculator"/> used for astronomical calculations. The Zmanim package ships
    /// with a number of different implementations of the <c>abstract</c> <see cref="AstronomicalCalculator"/> based on
    /// different algorithms, including the default <see cref="NOAACalculator"/> based on <a href=
    /// "https://noaa.gov">NOAA's</a> implementation of Jean Meeus's algorithms as well as <see cref="SunTimesCalculator"/> based on the <a href = "https://www.cnmoc.usff.navy.mil/usno/">US
    /// Naval Observatory's</a> algorithm. This allows easy runtime switching and comparison of different algorithms.
    /// </remarks>
    /// <seealso cref="AstronomicalCalculator"/>
    public AstronomicalCalculator AstronomicalCalculator
    {
        get => astronomicalCalculator;
        set => astronomicalCalculator = value;
    }


    /// <summary>
    /// The date the zmanim are calculated for. Defaults to today in the location's time zone when the calendar is created.
    /// </summary>
    /// <value>The <see cref="DateOnly"/> to set.</value>
    public DateOnly LocalDate
    {
        get => localDate;
        set => localDate = value;
    }

    /// <summary>
    /// Creates a copy of this calendar. The <see cref="GeoLocation"/> and <see cref="AstronomicalCalculator"/> are copied too, so
    /// changing the copy (for example its <see cref="LocalDate"/> or location) does not affect this instance.
    /// </summary>
    /// <returns>a copy of this calendar, of the same runtime type.</returns>
    public virtual AstronomicalCalendar Clone()
    {
        AstronomicalCalendar clone = (AstronomicalCalendar)MemberwiseClone();
        clone.GeoLocation = GeoLocation.Clone(); // consider converting the GeoLocation class to be immutable to avoid the deep copy
        clone.AstronomicalCalculator = AstronomicalCalculator.Clone(); // likely not needed
        return clone;
    }

    /// <summary>The KosherJava name of <see cref="GetDateTimeOffsetFromTime"/>.</summary>
    /// <param name="time">see <see cref="GetDateTimeOffsetFromTime"/>.</param>
    /// <param name="solarEvent">see <see cref="GetDateTimeOffsetFromTime"/>.</param>
    /// <returns>the result of <see cref="GetDateTimeOffsetFromTime"/>.</returns>
    [Obsolete("Use GetDateTimeOffsetFromTime(double, SolarEvent); GetInstantFromTime is the KosherJava name. This alias will be removed in a future version.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    protected DateTimeOffset? GetInstantFromTime(double time, SolarEvent solarEvent) => GetDateTimeOffsetFromTime(time, solarEvent);

    /// <summary>
    /// Returns <paramref name="time"/> expressed in the offset of <see cref="GeoLocation"/>'s <see cref="Util.GeoLocation.TimeZone"/>
    /// in effect at that instant. All <see cref="DateTimeOffset"/> values returned by the calendar classes are normalized with this
    /// method, so their <see cref="DateTimeOffset.Offset"/> is always the location's local offset (including daylight saving time).
    /// </summary>
    /// <param name="time">the time to convert. A <c>null</c> value is returned as <c>null</c>.</param>
    /// <returns>the same instant in the location's time zone, or <c>null</c>.</returns>
    protected DateTimeOffset? ToZone(DateTimeOffset? time)
    {
        return time is DateTimeOffset t ? TimeZoneInfo.ConvertTime(t, GeoLocation.TimeZone) : null;
    }
}
