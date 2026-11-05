/*
 * Zmanim Java API
 * Copyright © 2004-2026 Eliyahu Hershfeld
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; version 2.1 of the License.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not, write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA,
 * or connect to: https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html
 */
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;

namespace Kosher.Zmanim;

/// <summary>
/// This class extends ZmanimCalendar and provides many more <em>zmanim</em> than available in the ZmanimCalendar. The basis
/// for most <em>zmanim</em> in this class are from the <em>sefer</em> <b><a href="https://hebrewbooks.org/9765">Yisroe
/// Vehazmanim</a></b> by <b><a href="https://en.wikipedia.org/wiki/Yisroel_Dovid_Harfenes">Rabbi Yisrael Dovid Harfenes</a></b>.
/// As an example of the number of different <em>zmanim</em> made available by this class, there are methods to return 18 different
/// calculations for <em>alos</em> (dawn), 18 for <em>plag hamincha</em> and 29 for <em>tzais</em> available in this API. The real
/// power of this API is the ease in calculating <em>zmanim</em> that are not part of the library. The methods for <em>zmanim</em>
/// calculations not present in this class or its superclass  <see cref="ZmanimCalendar"/> are contained in the <see cref="AstronomicalCalendar"/>, the base class of the calendars in our API since they are generic methods for calculating time based on
/// degrees or time before or after <see cref="AstronomicalCalendar.GetSunrise"/> and <see cref="AstronomicalCalendar.GetSunset">sunset</see> and are of interest for calculation
/// beyond <em>zmanim</em> calculations. Here are some examples.
/// <para>First create the calendar for the location you would like to calculate:</para>
/// <code>
/// string locationName = "Lakewood, NJ";
/// double latitude = 40.0828; // Lakewood, NJ
/// double longitude = -74.222; // Lakewood, NJ
/// double elevation = 20; // optional elevation correction in Meters
/// TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
/// var location = new GeoLocation(locationName, latitude, longitude, elevation, timeZone);
/// var czc = new ComprehensiveZmanimCalendar(location);
/// // Optionally set the date or it will default to today's date
/// czc.LocalDate = new DateOnly(1969, 2, 8);
/// </code>
/// <para><b>Note:</b> Time zone rules come from the operating system (or ICU) time zone database. For locations such as Israel
/// where the beginning and end of daylight savings time used to fluctuate from year to year, make sure that database is up to
/// date, or create a custom <see cref="TimeZoneInfo"/> with the known start and end of DST.</para>
/// <para>To get <em>alos</em> calculated as 14° below the horizon (as calculated in the calendars published in Montreal),
/// add <see cref="AstronomicalCalendar.GeometricZenith"/> (90) to the 14° offset to get the desired time:</para>
/// <code>
/// DateTimeOffset? alos14 = czc.GetSunriseOffsetByDegrees(AstronomicalCalendar.GeometricZenith + 14);
/// </code>
/// <para>To get <em>mincha gedola</em> calculated based on the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham
/// (MGA)</a> using a <em>shaah zmanis</em> based on the day starting 16.1° below the horizon (and ending 16.1° after sunset) the
/// following calculation can be used:</para>
/// <code>
/// DateTimeOffset? minchaGedola = AstronomicalCalendar.GetTimeOffset(czc.GetAlos16Point1Degrees(),
///     czc.GetShaahZmanis16Point1Degrees() * 6.5);
/// </code>
/// <para>or even simpler using the included convenience methods</para>
/// <code>
/// DateTimeOffset? minchaGedola = czc.GetMinchaGedola(czc.GetAlos16Point1Degrees(), czc.GetTzais16Point1Degrees(), true);
/// </code>
/// <para>A little more complex example would be calculating <em>zmanim</em> that rely on a <em>shaah zmanis</em> that is
/// not present in this library. While a drop more complex, it is still rather easy. An example would be to calculate
/// the <a href="https://en.wikipedia.org/wiki/Israel_Isserlein">Trumas Hadeshen</a>'s <em>alos</em> to
/// <em>tzais</em> based <em>plag hamincha</em> as calculated in the Machzikei Hadass calendar in Manchester, England.
/// A number of this calendar's <em>zmanim</em> are calculated based on a day starting at <em>alos</em> of 12° before
/// sunrise and ending at <em>tzais</em> of 7.083° after sunset. Be aware that since the <em>alos</em> and <em>tzais</em>
/// do not use identical degree-based offsets, this leads to <em>chatzos</em> being at a time other than the
/// <see cref="AstronomicalCalendar.GetSunTransit()">solar transit</see> (solar midday). To calculate this <em>zman</em>, use the following steps. Note
/// that <em>plag hamincha</em> is 10.75 hours after the start of the day, and the following steps are all that it takes.</para>
/// <code>
/// DateTimeOffset? plag = czc.GetPlagHamincha(czc.GetSunriseOffsetByDegrees(AstronomicalCalendar.GeometricZenith + 12),
///     czc.GetSunsetOffsetByDegrees(AstronomicalCalendar.GeometricZenith + 7.083), true);
/// </code>
/// <para>Something a drop more challenging, but still simple, would be calculating a <em>zman</em> using the same "complex"
/// offset day used in the above-mentioned Manchester calendar, but for a <em>shaos zmaniyos</em> based <em>zman</em> not
/// supported by this library, such as calculating the point that one should be <em>makpid</em>
/// not to eat on <em>erev Shabbos</em> or <em>erev Yom Tov</em>. This is 9 <em>shaos zmaniyos</em> into the day.</para>
/// <list type="number">
///     <item>Calculate the <em>shaah zmanis</em> <see cref="TimeSpan"/> for this day</item>
///     <item>Add 9 of these <em>shaos zmaniyos</em> to <em>alos</em> starting at 12°</item>
/// </list>
/// <code>
/// DateTimeOffset? alos12 = czc.GetSunriseOffsetByDegrees(AstronomicalCalendar.GeometricZenith + 12);
/// TimeSpan? shaahZmanis = czc.GetTemporalHour(alos12,
///     czc.GetSunsetOffsetByDegrees(AstronomicalCalendar.GeometricZenith + 7.083));
/// DateTimeOffset? sofZmanAchila = AstronomicalCalendar.GetTimeOffset(alos12, shaahZmanis * 9);
/// </code>
/// <para>Calculating this <em>sof zman achila</em> according to the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>
/// is simplicity itself.</para>
/// <code>
/// DateTimeOffset? sofZmanAchila = AstronomicalCalendar.GetTimeOffset(czc.GetSunrise(), czc.GetShaahZmanisGRA() * 9);
/// </code>
/// <para>See the documentation of the <see cref="ZmanimCalendar"/> parent class.</para>
/// </summary>
public class ComprehensiveZmanimCalendar : ZmanimCalendar
{

    /// <summary>
    /// The zenith of 3.7° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetTzaisGeonim3Point7Degrees"/>
    protected const double Zenith3Point7 = GeometricZenith + 3.7;

    /// <summary>
    /// The zenith of 3.8° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetTzaisGeonim3Point8Degrees"/>
    protected const double Zenith3Point8 = GeometricZenith + 3.8;

    /// <summary>
    /// The zenith of 5.95° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetTzaisGeonim5Point95Degrees"/>
    protected const double Zenith5Point95 = GeometricZenith + 5.95;

    /// <summary>
    /// The zenith of 7.083° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°). This is often referred to as
    /// 7°5' (7° and 5 minutes).
    /// </summary>
    /// <seealso cref="GetTzaisGeonim7Point083Degrees"/>
    /// <seealso cref="GetBainHashmashosRT13Point5MinutesBefore7Point083Degrees"/>
    protected const double Zenith7Point083 = GeometricZenith + 7 + (5.0 / 60);

    /// <summary>
    /// The zenith of 10.2° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetMisheyakir10Point2Degrees"/>
    protected const double Zenith10Point2 = GeometricZenith + 10.2;

    /// <summary>
    /// The zenith of 11° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetMisheyakir11Degrees"/>
    protected const double Zenith11Degrees = GeometricZenith + 11;

    /// <summary>
    /// The zenith of 11.5° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetMisheyakir11Point5Degrees"/>
    protected const double Zenith11Point5 = GeometricZenith + 11.5;

    /// <summary>
    /// The zenith of 12.85° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetMisheyakir12Point85Degrees"/>
    protected const double Zenith12Point85 = GeometricZenith + 12.85;

    /// <summary>
    /// The zenith of 13.24° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetBainHashmashosRT13Point24Degrees"/>
    protected const double Zenith13Point24 = GeometricZenith + 13.24;

    /// <summary>
    /// The zenith of 19° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetAlos19Degrees"/>
    /// <seealso cref="Zenith19Point8"/>
    protected const double Zenith19Degrees = GeometricZenith + 19;

    /// <summary>
    /// The zenith of 19.8° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetTzais19Point8Degrees"/>
    /// <seealso cref="GetAlos19Point8Degrees"/>
    /// <seealso cref="Zenith19Degrees"/>
    protected const double Zenith19Point8 = GeometricZenith + 19.8;

    /// <summary>
    /// The zenith of 26° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetAlos26Degrees"/>
    /// <seealso cref="GetTzais26Degrees"/>
    /// <seealso cref="GetAlos120Minutes"/>
    /// <seealso cref="GetTzais120Minutes"/>
    protected const double Zenith26Degrees = GeometricZenith + 26.0;

    /// <summary>
    /// The zenith of 4.42° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetTzaisGeonim4Point42Degrees"/>
    protected const double Zenith4Point42 = GeometricZenith + 4.42;

    /// <summary>
    /// The zenith of 4.66° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetTzaisGeonim4Point66Degrees"/>
    protected const double Zenith4Point66 = GeometricZenith + 4.66;

    /// <summary>
    /// The zenith of 4.8° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetTzaisGeonim4Point8Degrees"/>
    protected const double Zenith4Point8 = GeometricZenith + 4.8;

    /// <summary>
    /// The zenith of 16.9° below geometric zenith (90°).
    /// </summary>
    /// <seealso cref="GetAlosBaalHatanya"/>
    protected const double Zenith16Point9 = GeometricZenith + 16.9;

    /// <summary>
    /// The zenith of 6° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetTzaisBaalHatanya"/>
    protected const double Zenith6Degrees = GeometricZenith + 6;

    /// <summary>
    /// The zenith of 6.45° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetTzaisGeonim6Point45Degrees"/>
    protected const double Zenith6Point45 = GeometricZenith + 6.45;

    /// <summary>
    /// The zenith of 7.65° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetMisheyakir7Point65Degrees"/>
    protected const double Zenith7Point65 = GeometricZenith + 7.65;

    /// <summary>
    /// The zenith of 7.67° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetTzaisGeonim7Point67Degrees"/>
    protected const double Zenith7Point67 = GeometricZenith + 7.67;

    /// <summary>
    /// The zenith of 9.3° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetTzaisGeonim9Point3Degrees"/>
    protected const double Zenith9Point3 = GeometricZenith + 9.3;

    /// <summary>
    /// The zenith of 9.5° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetMisheyakir9Point5Degrees"/>
    protected const double Zenith9Point5 = GeometricZenith + 9.5;

    /// <summary>
    /// The zenith of 9.75° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°). This calculation is used for
    /// calculating <em>alos</em> (dawn) and <em>tzais</em> (nightfall) according to some opinions.
    /// </summary>
    /// <seealso cref="GetTzaisGeonim9Point75Degrees"/>
    protected const double Zenith9Point75 = GeometricZenith + 9.75;

    /// <summary>
    /// The zenith of 2.1° <b>above</b> <see cref="AstronomicalCalendar.GeometricZenith"/> (90°). This calculation is used for calculating
    /// the start of <em>bain hashmashos</em> (twilight) of 13.5 minutes before sunset converted to degrees according to the Yereim.
    /// As is traditional with degrees below the horizon, this is calculated without refraction and from the center of the sun. It
    /// would be 0.833° less without this.
    /// </summary>
    /// <seealso cref="GetBainHashmashosYereim2Point1Degrees"/>
    protected const double ZenithMinus2Point1 = GeometricZenith - 2.1;

    /// <summary>
    /// The zenith of 2.8° above <see cref="AstronomicalCalendar.GeometricZenith"/> (90°). This calculation is used for
    /// calculating the start of <em>bain hashmashos</em> (twilight) of 16.875 minutes before sunset converted to degrees
    /// according to the Yereim. As is traditional with degrees below the horizon, this is calculated without refraction
    /// and from the center of the sun. It would be 0.833° less without this.
    /// </summary>
    /// <seealso cref="GetBainHashmashosYereim2Point8Degrees"/>
    protected const double ZenithMinus2Point8 = GeometricZenith - 2.8;

    /// <summary>
    /// The zenith of 3.05° above <see cref="AstronomicalCalendar.GeometricZenith"/> (90°). This calculation is used for
    /// calculating the start of <em>bain hashmashos</em> (twilight) of 18 minutes before sunset converted to degrees
    /// according to the Yereim. As is traditional with degrees below the horizon, this is calculated without refraction
    /// and from the center of the sun. It would be 0.833° less without this.
    /// </summary>
    /// <seealso cref="GetBainHashmashosYereim3Point05Degrees"/>
    protected const double ZenithMinus3Point05 = GeometricZenith - 3.05;

    /// <summary>
    /// The offset in minutes (defaults to 40) after sunset used for <em>tzeit</em> based on calculations of
    /// <em>Chacham</em> Yosef Harari-Raful of Yeshivat Ateret Torah.
    /// </summary>
    /// <seealso cref="GetTzaisAteretTorah"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    private double ateretTorahSunsetOffset = 40;

    /// <summary>
    /// A constructor that takes a <see cref="GeoLocation"/> as a parameter.
    /// </summary>
    /// <param name="location">the location</param>
    /// <seealso cref="ZmanimCalendar.ZmanimCalendar()"/>
    public ComprehensiveZmanimCalendar(GeoLocation location) : base(location)
    {
    }

    /// <summary>
    /// Default constructor will set a default <see cref="GeoLocation.GeoLocation(string, double, double, TimeZoneInfo)"/>, a default
    /// <see cref="AstronomicalCalculator.GetDefault">AstronomicalCalculator</see> and default the calendar to the current date.
    /// </summary>
    /// <seealso cref="AstronomicalCalendar.AstronomicalCalendar()"/>
    /// <seealso cref="ComprehensiveZmanimCalendar(GeoLocation)"/>
    public ComprehensiveZmanimCalendar()
    {
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) calculated using a 19.8° dip. This calculation
    /// divides the day based on the opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen
    /// Avraham (MGA)</a> that the day runs from dawn to dusk. Dawn for this calculation is when the sun is 19.8°
    /// below the eastern geometric horizon before sunrise. Dusk for this is when the sun is 19.8° below the western
    /// geometric horizon after sunset. This day is split into 12 equal parts with each part being a <em>shaah zmanis</em>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may not reach low
    ///         enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed explanation on top
    ///         of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual TimeSpan? GetShaahZmanis19Point8Degrees()
    {
        return GetTemporalHour(GetAlos19Point8Degrees(), GetTzais19Point8Degrees());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) calculated using a 18° dip. This calculation divides
    /// the day based on the opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham
    /// (MGA)</a> that the day runs from dawn to dusk. Dawn for this calculation is when the sun is 18° below the
    /// eastern geometric horizon before sunrise. Dusk for this is when the sun is 18° below the western geometric
    /// horizon after sunset. This day is split into 12 equal parts with each part being a <em>shaah zmanis</em>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may not reach low
    ///         enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed explanation on top
    ///         of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual TimeSpan? GetShaahZmanis18Degrees()
    {
        return GetTemporalHour(GetAlos18Degrees(), GetTzais18Degrees());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) calculated using a dip of 26°. This calculation divides the day based
    /// on the opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> that the day runs from
    /// dawn to dusk. Dawn for this calculation is when the sun is <see cref="GetAlos26Degrees">26°</see> below the eastern geometric horizon
    /// before sunrise. Dusk for this is when the sun is <see cref="GetTzais26Degrees">26°</see> below the western geometric horizon after
    /// sunset. This day is split into 12 equal parts with each part being a <em>shaah zmanis</em>. Since <em>zmanim</em> that use this
    /// method are extremely late or early and at a point when the sky is a long time past the 18° point where the darkest point is
    /// reached, <em>zmanim</em> that use this should only be used <em>lechumra</em>, such as delaying the start of nighttime
    /// <em>mitzvos</em>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as northern and southern
    ///         locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may not reach low enough below
    ///         the horizon for this calculation, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis120Minutes"/>
    public virtual TimeSpan? GetShaahZmanis26Degrees()
    {
        return GetTemporalHour(GetAlos26Degrees(), GetTzais26Degrees());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) calculated using a dip of 16.1°. This calculation divides the day based
    /// on the opinion that the day runs from dawn to dusk. Dawn for this calculation is when the sun is 16.1° below the eastern
    /// geometric horizon before sunrise and dusk is when the sun is 16.1° below the western geometric horizon after sunset. This day is
    /// split into 12 equal parts with each part being a <em>shaah zmanis</em>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/>  of a <em>shaah zmanis</em>. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may not reach low
    ///         enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed explanation on top
    ///         of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetAlos16Point1Degrees"/>
    /// <seealso cref="GetTzais16Point1Degrees"/>
    /// <seealso cref="GetSofZmanShmaMGA16Point1Degrees"/>
    /// <seealso cref="GetSofZmanTfilaMGA16Point1Degrees"/>
    /// <seealso cref="GetMinchaGedola16Point1Degrees"/>
    /// <seealso cref="GetMinchaKetana16Point1Degrees"/>
    /// <seealso cref="GetPlagHamincha16Point1Degrees"/>
    public virtual TimeSpan? GetShaahZmanis16Point1Degrees()
    {
        return GetTemporalHour(GetAlos16Point1Degrees(), GetTzais16Point1Degrees());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (solar hour) according to the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a>. This calculation divides the day based on the opinion
    /// of the MGA that the day runs from dawn to dusk. Dawn for this calculation is 60 minutes before sunrise and dusk is 60 minutes
    /// after sunset. This day is split into 12 equal parts with each part being a <em>shaah zmanis</em>. Alternate methods of
    /// calculating a <em>shaah zmanis</em> are available in the subclass <see cref="ComprehensiveZmanimCalendar"/>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos60Minutes"/>
    /// <seealso cref="GetTzais60Minutes"/>
    /// <seealso cref="GetPlagHamincha60Minutes"/>
    public virtual TimeSpan? GetShaahZmanis60Minutes()
    {
        return GetTemporalHour(GetAlos60Minutes(), GetTzais60Minutes());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) according to the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos72Zmanis">72 minutes <em>zmaniyos</em></see> before <see cref="AstronomicalCalendar.GetSunrise"/> sunrise}. This calculation divides the day based
    /// on the opinion of the MGA that the day runs from dawn to dusk. Dawn for this calculation is 72 minutes <em>zmaniyos</em>
    /// before sunrise and dusk is 72 minutes <em>zmaniyos</em> after sunset. This day is split into 12 equal parts with each part
    /// being a <em>shaah zmanis</em>. This is identical to 1/10th of the day from <see cref="AstronomicalCalendar.GetSunrise"/> to <see cref="AstronomicalCalendar.GetSunset"/>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos72Zmanis"/>
    /// <seealso cref="GetTzais72Zmanis"/>
    public virtual TimeSpan? GetShaahZmanis72MinutesZmanis()
    {
        return GetTemporalHour(GetAlos72Zmanis(), GetTzais72Zmanis());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) calculated using a dip of 90 minutes. This calculation
    /// divides the day based on the opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen
    /// Avraham (MGA)</a> that the day runs from dawn to dusk. Dawn for this calculation is 90 minutes before sunrise
    /// and dusk is 90 minutes after sunset. This day is split into 12 equal parts with each part being a <em>shaah zmanis</em>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual TimeSpan? GetShaahZmanis90Minutes()
    {
        return GetTemporalHour(GetAlos90Minutes(), GetTzais90Minutes());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) according to the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos90Zmanis">90</see> minutes <em>zmaniyos</em> before <see cref="AstronomicalCalendar.GetSunrise"/>. This calculation divides the day
    /// based on the opinion of the MGA that the day runs from dawn to dusk. Dawn is calculated as 90 minutes <em>zmaniyos</em>
    /// before sunrise and dusk is 90 minutes <em>zmaniyos</em> after sunset. This day is split into 12 equal parts with each part
    /// being a <em>shaah zmanis</em>. This is 1/8th of the day from <see cref="AstronomicalCalendar.GetSunrise"/> to <see cref="AstronomicalCalendar.GetSunset">sunset</see>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos90Zmanis"/>
    /// <seealso cref="GetTzais90Zmanis"/>
    public virtual TimeSpan? GetShaahZmanis90MinutesZmanis()
    {
        return GetTemporalHour(GetAlos90Zmanis(), GetTzais90Zmanis());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) according to the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos96Zmanis">96</see> minutes <em>zmaniyos</em> before <see cref="AstronomicalCalendar.GetSunrise"/>. This calculation divides the day based on the opinion of the
    /// MGA that the day runs from dawn to dusk. Dawn is calculated as 96 minutes <em>zmaniyos</em> before sunrise and dusk is 96 minutes
    /// <em>zmaniyos</em> after sunset. This day is split into 12 equal parts with each part being a <em>shaah zmanis</em>. This is
    /// identical to 1/7.5th of the day from <see cref="AstronomicalCalendar.GetSunrise"/> to <see cref="AstronomicalCalendar.GetSunset">sunset</see>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos96Zmanis"/>
    /// <seealso cref="GetTzais96Zmanis"/>
    public virtual TimeSpan? GetShaahZmanis96MinutesZmanis()
    {
        return GetTemporalHour(GetAlos96Zmanis(), GetTzais96Zmanis());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) according to the opinion of the <em>Chacham</em> Yosef Harari-Raful
    /// of Yeshivat Ateret Torah calculated with <em>alos</em> being 1/10th of sunrise to sunset day, or <see cref="GetAlos72Zmanis">72</see>
    /// minutes <em>zmaniyos</em> of such a day before <see cref="AstronomicalCalendar.GetSunrise"/>, and <em>tzais</em> is usually calculated as <see cref="GetTzaisAteretTorah">40 minutes</see> (configurable to any offset via <see cref="AteretTorahSunsetOffset"/>) after <see cref="AstronomicalCalendar.GetSunset">sunset</see>. This day is split into 12 equal parts with each part being a <em>shaah zmanis</em>. Note that with this
    /// system, <em>chatzos</em> (midday) will not be the point that the sun is <see cref="AstronomicalCalendar.GetSunTransit()">halfway across the sky</see>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos72Zmanis"/>
    /// <seealso cref="GetTzaisAteretTorah"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    public virtual TimeSpan? GetShaahZmanisAteretTorah()
    {
        return GetTemporalHour(GetAlos72Zmanis(), GetTzaisAteretTorah());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) used by some <em>zmanim</em> according to the opinion of <a href=
    /// "https://en.wikipedia.org/wiki/Yaakov_Moshe_Hillel">Rabbi Yaakov Moshe Hillel</a> as published in the <em>luach</em> of the
    /// Bais Horaah of Yeshivat Chevrat Ahavat Shalom that is based on a day starting 72 minutes before sunrise in degrees <see cref="ZmanimCalendar.GetAlos16Point1Degrees"><em>alos</em> 16.1°</see> and ending 14 minutes after sunset in degrees <see cref="GetTzaisGeonim3Point8Degrees"><em>tzais</em> 3.8°</see>. This day is split into 12 equal parts with each part being a <em>shaah
    /// zmanis</em>. Note that with this system, <em>chatzos</em> (midday) will not be the point that the sun is <see cref="AstronomicalCalendar.GetSunTransit()">halfway across the sky</see>. These <em>shaos zmaniyos</em> are used for <em>Mincha Ketana</em> and <em>Plag
    /// Hamincha</em>. The 14 minutes are based on 3/4 of an 18 minute <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a>,
    /// with half a minute added for Rav Yosi's <em>Bain Hashmashos</em>. Sources for an asymmetrical day-based calculation can be
    /// seen in the documentation of <see cref="GetSofZmanShmaAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetMinchaKetanaAhavatShalom"/>
    /// <seealso cref="GetPlagAhavatShalom"/>
    /// <seealso cref="GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point7Degrees"/>
    /// <seealso cref="GetShaahZmanisAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>
    public virtual TimeSpan? GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point8Degrees()
    {
        return GetTemporalHour(GetAlos16Point1Degrees(), GetTzaisGeonim3Point8Degrees());
    }


    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) used by some <em>zmanim</em> according to the opinion of <a href=
    /// "https://en.wikipedia.org/wiki/Yaakov_Moshe_Hillel">Rabbi Yaakov Moshe Hillel</a> as published in the <em>luach</em> of the
    /// Bais Horaah of Yeshivat Chevrat Ahavat Shalom that is based on a day starting 72 minutes before sunrise in degrees <see cref="ZmanimCalendar.GetAlos16Point1Degrees"><em>alos</em> 16.1°</see> and ending 13.5 minutes after sunset in degrees <see cref="GetTzaisGeonim3Point7Degrees"><em>tzais</em> 3.7°</see>. This day is split into 12 equal parts with each part being a <em>shaah
    /// zmanis</em>. Note that with this system, <em>chatzos</em> (midday) will not be the point that the sun is <see cref="AstronomicalCalendar.GetSunTransit()">halfway across the sky</see>. These <em>shaos zmaniyos</em> are used for <em>Mincha Gedola</em> calculation.
    /// Sources for an asymmetrical day-based calculation can be seen in the documentation of
    /// <see cref="GetSofZmanShmaAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetMinchaGedolaAhavatShalom"/>
    /// <seealso cref="GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point8Degrees"/>
    /// <seealso cref="GetShaahZmanisAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>
    public virtual TimeSpan? GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point7Degrees()
    {
        return GetTemporalHour(GetAlos16Point1Degrees(), GetTzaisGeonim3Point7Degrees());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) used by some <em>zmanim</em> according to some opinions that is based
    /// on a day starting  at <see cref="ZmanimCalendar.GetAlos16Point1Degrees"><em>alos</em> 16.1°</see> and ending <see cref="GetTzaisGeonim7Point083Degrees"><em>tzais</em> 8.083°</see>. This day is split into 12 equal parts with each part being a <em>shaah
    /// zmanis</em>. Note that with this system, <em>chatzos</em> (midday) will not be the point that the sun is <see cref="AstronomicalCalendar.GetSunTransit()">halfway across the sky</see>. Sources for an asymmetrical day-based calculation can be seen in the documentation
    /// of <see cref="GetSofZmanShmaAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point7Degrees"/>
    /// <seealso cref="GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point8Degrees"/>
    public virtual TimeSpan? GetShaahZmanisAlos16Point1DegreesToTzaisGeonim7Point083Degrees()
    {
        return GetTemporalHour(GetAlos16Point1Degrees(), GetTzaisGeonim7Point083Degrees());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) calculated using a dip of 96 minutes. This calculation divides the
    /// day based on the opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> that the day
    /// runs from dawn to dusk. Dawn for this calculation is 96 minutes before sunrise and dusk is 96 minutes after sunset. This day
    /// is split into 12 equal parts with each part being a <em>shaah zmanis</em>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual TimeSpan? GetShaahZmanis96Minutes()
    {
        return GetTemporalHour(GetAlos96Minutes(), GetTzais96Minutes());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) calculated using a dip of 120 minutes. This calculation divides the
    /// day based on the opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> that the day
    /// runs from dawn to dusk. Dawn for this calculation is 120 minutes before sunrise and dusk is 120 minutes after sunset. This day
    /// is split into 12 equal parts with each part being a <em>shaah zmanis</em>. Since <em>zmanim</em> that use this method are
    /// extremely late or early and at a point when the sky is a long time past the 18° point where the darkest point is reached,
    /// <em>zmanim</em> that use this should only be used <em>lechumra</em> only, such as delaying the start of nighttime
    /// <em>mitzvos</em>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as inin the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis26Degrees"/>
    public virtual TimeSpan? GetShaahZmanis120Minutes()
    {
        return GetTemporalHour(GetAlos120Minutes(), GetTzais120Minutes());
    }

    /// <summary>
    /// Method to return a <em>shaah zmanis</em> (temporal hour) according to the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos120Zmanis">120</see> minutes <em>zmaniyos</em> before <see cref="AstronomicalCalendar.GetSunrise"/>. This calculation divides the day
    /// based on the opinion of the MGA that the day runs from dawn to dusk. Dawn for this calculation is 120 minutes <em>zmaniyos</em>
    /// before sunrise and dusk is 120 minutes <em>zmaniyos</em> after sunset. This day is split into 12 equal parts with each part
    /// being a <em>shaah zmanis</em>. This is identical to 1/6th of the day from <see cref="AstronomicalCalendar.GetSunrise"/> to <see cref="AstronomicalCalendar.GetSunset">sunset</see>. Since <em>zmanim</em> that use this method are extremely late or early and at a point when the sky is a long time
    /// past the 18° point where the darkest point is reached, <em>zmanim</em> that use this should only be used <em>lechumra</em>
    /// such as delaying the start of nighttime <em>mitzvos</em>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos120Zmanis"/>
    /// <seealso cref="GetTzais120Zmanis"/>
    public virtual TimeSpan? GetShaahZmanis120MinutesZmanis()
    {
        return GetTemporalHour(GetAlos120Zmanis(), GetTzais120Zmanis());
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns the time of <em>plag hamincha</em> based on sunrise being 120
    /// minutes <em>zmaniyos</em> or 1/6th of the day before sunrise. This is calculated as 10.75 hours after <see cref="GetAlos120Zmanis">dawn</see>. The formula used is 10.75 * <see cref="GetShaahZmanis120MinutesZmanis"/> after <see cref="GetAlos120Zmanis">dawn</see>. Since the <em>zman</em> based on an extremely early <em>alos</em> and a very late <em>tzais</em>,
    /// it should only be used <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after <em>shkiah</em>),
    ///         and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this method
    ///         from the API, and this deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetShaahZmanis120MinutesZmanis"/>
    /// <seealso cref="GetAlos120Minutes"/>
    /// <seealso cref="GetTzais120Minutes"/>
    /// <seealso cref="GetPlagHamincha26Degrees"/>
    /// <seealso cref="GetPlagHamincha120Minutes"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagHamincha120MinutesZmanis()
    {
        return ToZone(GetPlagHamincha(GetAlos120Zmanis(), GetTzais120Zmanis(), true));
    }

    /// <summary>
    /// This method <em>plag hamincha</em> according to the Magen Avraham with the day starting at <see cref="GetAlos120Minutes"/>. It should be used <em>lechumra</em> only. This is calculated as 10.75 * <see cref="GetShaahZmanis120Minutes"/> after <see cref="GetAlos120Minutes"/>. Since the <em>zman</em> based on an extremely early
    /// <em>alos</em> and a very late <em>tzais</em>, it returns a very late time (often after <em>shkiah</em> and can result in
    /// <em>chillul Shabbos</em> etc.) and should only be used <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after <em>shkiah</em>),
    ///         and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this method
    ///         from the API, and this deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetShaahZmanis120Minutes"/>
    /// <seealso cref="GetPlagHamincha26Degrees"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagHamincha120Minutes()
    {
        return ToZone(GetPlagHamincha(GetAlos120Minutes(), GetTzais120Minutes(), true));
    }

    /// <summary>
    /// Method to return <em>alos</em> (dawn) calculated as 60 minutes before <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting"/> (depends on
    /// the <see cref="ZmanimCalendar.UseElevation"/> setting). This is the time to walk the distance of 4 <a href=
    /// "https://en.wikipedia.org/wiki/Biblical_mile">mil</a> at 15 minutes a mil. This seems to be the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Yair_Bacharach">Chavas Yair</a> in the Mekor Chaim, Orach Chaim Ch. 90, though  the Mekor Chaim
    /// in Ch. 58 and in the <a href="https://hebrewbooks.org/pdfpager.aspx?req=45193&amp;pgnum=214">Chut Hashani Ch. 97</a> states that a
    /// person walks 3 and a 1/3 mil in an hour, or an 18-minute mil. Also see the <a href=
    /// "https://he.wikipedia.org/wiki/%D7%9E%D7%9C%D7%9B%D7%99%D7%90%D7%9C_%D7%A6%D7%91%D7%99_%D7%98%D7%A0%D7%A0%D7%91%D7%95%D7%99%D7%9D"
    /// >Divrei Malkiel</a> <a href="https://hebrewbooks.org/pdfpager.aspx?req=803&amp;pgnum=33">Vol. 4, Ch. 20, page 34</a>) who mentions
    /// the 15 minute mil <em>lechumra</em> by baking matzos. Also see the <a href=
    /// "https://en.wikipedia.org/wiki/Joseph_Colon_Trabotto">Maharik</a> <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=1142&amp;pgnum=216">Ch. 173</a> where the questioner quoting the <a href=
    /// "https://en.wikipedia.org/wiki/Eliezer_ben_Nathan">Ra'avan</a> is of the opinion that the time to walk a mil is 15 minutes (5
    /// mil in a little over an hour). There are many who believe that there is a <em>ta'us sofer</em> (scribe's error) in the Ra'avan,
    /// and it should 4 mil in a little over an hour, or an 18-minute mil. Time based offset calculations are based on the opinions
    /// that the time of the <em>neshef</em> (time between dawn and sunrise) does not vary by the time of year or location but purely
    /// depends on the time it takes to walk the distance of 4* mil. <see cref="GetTzaisGeonim9Point75Degrees"/> is a related
    /// <em>zman</em> that is a degree-based calculation
    /// based on 60 minutes.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic Circle
    ///         where there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetTzais60Minutes"/>
    /// <seealso cref="GetTzaisGeonim9Point75Degrees"/>
    /// <seealso cref="GetPlagHamincha60Minutes"/>
    /// <seealso cref="GetShaahZmanis60Minutes"/>
    public virtual DateTimeOffset? GetAlos60Minutes()
    {
        return ToZone(GetTimeOffset(GetSunriseBasedOnElevationSetting(), JavaTime.OfMinutes(-60)));
    }

    /// <summary>
    /// Method to return <em>alos</em> (dawn) calculated using 72 minutes <em>zmaniyos</em> or 1/10th of the day before sunrise. This
    /// is based on an 18-minute <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a> so the
    /// time for 4 mil is 72 minutes which is 1/10th of a day (12 * 60 = 720) based on the day being from <see cref="AstronomicalCalendar.GetSeaLevelSunrise"/>
    /// to <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> or <see cref="AstronomicalCalendar.GetSunrise"/> to <see cref="AstronomicalCalendar.GetSunset"/> (depending on the
    /// <see cref="ZmanimCalendar.UseElevation"/> setting). The actual calculation is <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting"/> - (<see cref="ZmanimCalendar.GetShaahZmanisGRA"/> * 1.2). This calculation is used in the calendars published by the <a href=
    /// "https://en.wikipedia.org/wiki/Central_Rabbinical_Congress">Hisachdus Harabanim D'Artzos Habris Ve'Canada</a>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic Circle where
    ///         there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c> will
    ///         be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetShaahZmanisGRA"/>
    public virtual DateTimeOffset? GetAlos72Zmanis()
    {
        return ToZone(GetZmanisBasedOffset(-1.2));
    }

    /// <summary>
    /// Method to return <em>alos</em> (dawn) calculated using 96 minutes before <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting"/>
    /// (depends on the <see cref="ZmanimCalendar.UseElevation"/> setting) that is based on the time to walk the distance of 4 <a href=
    /// "https://en.wikipedia.org/wiki/Biblical_mile">mil</a> at 24 minutes a mil. Time based offset calculations for <em>alos</em>
    /// are based on the opinion that the time of the <em>Neshef</em> (time between dawn and sunrise) does not vary by the time of
    /// year or location but purely depends on the time it takes to walk the distance of 4 mil.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic Circle
    ///         where there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual DateTimeOffset? GetAlos96Minutes()
    {
        return ToZone(GetTimeOffset(GetSunriseBasedOnElevationSetting(), JavaTime.OfMinutes(-96)));
    }

    /// <summary>
    /// Method to return <em>alos</em> (dawn) calculated using 90 minutes <em>zmaniyos</em> or 1/8th of the day before <see cref="AstronomicalCalendar.GetSunrise"/> or <see cref="AstronomicalCalendar.GetSeaLevelSunrise"/> (depending on the <see cref="ZmanimCalendar.UseElevation"/> setting). This is based on a
    /// 22.5-minute <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a> so the time for 4 mil is 90 minutes which is 1/8th
    /// of a day (12 * 60) / 8 = 90. The actual calculation used is <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting"/> -
    /// (<see cref="ZmanimCalendar.GetShaahZmanisGRA"/> * 1.5).
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic Circle where
    ///         there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c> will
    ///         be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetShaahZmanisGRA"/>
    public virtual DateTimeOffset? GetAlos90Zmanis()
    {
        return ToZone(GetZmanisBasedOffset(-1.5));
    }

    /// <summary>
    /// This method returns <em>alos</em> (dawn) calculated using 96 minutes <em>zmaniyos</em> or 1/7.5th of the day before <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting"/> (depends on the <see cref="ZmanimCalendar.UseElevation"/> setting). This is based on a 24-minute
    /// <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a> so the time for 4 mil is 96 minutes which is 1/7.5th of a day
    /// (12 * 60 / 7.5 = 96). The day is calculated from <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting"/> to to <see cref="ZmanimCalendar.GetSunsetBasedOnElevationSetting"/> (depends on the <see cref="ZmanimCalendar.UseElevation"/>. The actual calculation used is <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting"/> - (<see cref="ZmanimCalendar.GetShaahZmanisGRA"/> * 1.6).
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic Circle where
    ///         there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetShaahZmanisGRA"/>
    public virtual DateTimeOffset? GetAlos96Zmanis()
    {
        return ToZone(GetZmanisBasedOffset(-1.6));
    }

    /// <summary>
    /// Method to return <em>alos</em> (dawn) calculated using 90 minutes before <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting"/>
    /// (depends on the <see cref="ZmanimCalendar.UseElevation"/> setting) based on the time to walk the distance of 4 <a href=
    /// "https://en.wikipedia.org/wiki/Biblical_mile">mil</a> at 22.5 minutes a mil. Time-based offset calculations for <em>alos</em>
    /// are based on the opinion that the time of the <em>Neshef</em> (time between dawn and sunrise) does not vary by the time of
    /// year or location but purely depends on the time it takes to walk the distance of 4 mil.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic Circle where there
    ///         is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual DateTimeOffset? GetAlos90Minutes()
    {
        return ToZone(GetTimeOffset(GetSunriseBasedOnElevationSetting(), JavaTime.OfMinutes(-90)));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns <em>alos</em> (dawn) calculated using 120 minutes before <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting"/> (that depends on the <see cref="ZmanimCalendar.UseElevation"/> setting) based on the time to walk the
    /// distance of 5 <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a> (<em>Ula</em>) at 24 minutes a mil. Time based
    /// offset calculations for <em>alos</em> are based on the* opinions that the time of the <em>neshef</em> (time between dawn and
    /// sunrise) does not vary by the time of year or location but purely depends on the time it takes to walk the distance of 5 mil
    /// (<em>Ula</em>). Since this time is extremely early, it should only be used <em>lechumra</em>, such as not eating after this
    /// time on a fast day, and not as the start time for <em>mitzvos</em> that can only be performed during the day.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic Circle where
    ///         there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only (such as stopping to eat at this time on a fast day),
    ///         since it returns a very early time, and if used <em>lekula</em> can result in doing <em>mitzvos hayom</em>
    ///         too early according to most opinions. There is no current plan to remove this method from the API, and this
    ///         deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetTzais120Minutes"/>
    /// <seealso cref="GetAlos26Degrees"/>
    [Obsolete("This method should be used lechumra only (such as stopping to eat at this time on a fast day), since it returns a very early time, and if used lekula can result in doing mitzvos hayom too early according to most opinions.")]
    public virtual DateTimeOffset? GetAlos120Minutes()
    {
        return ToZone(GetTimeOffset(GetSunriseBasedOnElevationSetting(), JavaTime.OfMinutes(-120)));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and  method returns <em>alos</em> (dawn) calculated using
    /// 120 minutes <em>zmaniyos</em> or 1/6th of the day before <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting"/> (depends on the <see cref="ZmanimCalendar.UseElevation"/> setting). This is based on a 24-minute <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a> so
    /// the time for 5 mil is 120 minutes which is 1/6th of a day (12 * 60 / 6 = 120). The day is calculated from <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting"/> to <see cref="ZmanimCalendar.GetSunsetBasedOnElevationSetting"/> (depends on the <see cref="ZmanimCalendar.UseElevation"/>.
    /// The actual calculation used is <see cref="AstronomicalCalendar.GetSunset"/> - (<see cref="ZmanimCalendar.GetShaahZmanisGRA"/> * 2). Since this time is extremely early,
    /// it should only be used <em>lechumra</em>, such as not eating after this time on a fast day, and not as the start time for
    /// <em>mitzvos</em> that can only be performed during the day.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic Circle where there
    ///         is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only (such as stopping to eat at this time on a fast day),
    ///         since it returns a very early time, and if used <em>lekula</em> can result in doing <em>mitzvos hayom</em>
    ///         too early according to most opinions. There is no current plan to remove this method from the API, and this
    ///         deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetAlos120Minutes"/>
    /// <seealso cref="GetAlos26Degrees"/>
    [Obsolete("This method should be used lechumra only (such as stopping to eat at this time on a fast day), since it returns a very early time, and if used lekula can result in doing mitzvos hayom too early according to most opinions.")]
    public virtual DateTimeOffset? GetAlos120Zmanis()
    {
        return ToZone(GetZmanisBasedOffset(-2.0));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns <em>alos</em> (dawn) calculated when the sun is <see cref="Zenith26Degrees">26°</see> below the eastern geometric horizon before sunrise. This calculation is based on the same
    /// calculation of <see cref="GetAlos120Minutes">120 minutes</see> but uses a degree-based calculation instead of 120 exact minutes.
    /// This calculation is based on the position of the sun 120 minutes before sunrise in Jerusalem <a href=
    /// "https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a>, which
    /// calculates to 26° below <see cref="AstronomicalCalendar.GeometricZenith"/>. Since this time is extremely early, it should
    /// only be used <em>lechumra</em> only, such as not eating after this time on a fast day, and not as the start time for
    /// <em>mitzvos</em> that can only be performed during the day.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing <em>alos</em>. If the calculation can't be computed such as northern
    ///         and southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun
    ///         may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See
    ///         detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only (such as stopping to eat at this time on a fast day),
    ///         since it returns a very early time, and if used <em>lekula</em> can result in doing <em>mitzvos hayom</em>
    ///         too early according to most opinions. There is no current plan to remove this  method from the API, and this
    ///         deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetAlos120Minutes"/>
    /// <seealso cref="GetTzais120Minutes"/>
    /// <seealso cref="GetTzais26Degrees"/>
    [Obsolete("This method should be used lechumra only (such as stopping to eat at this time on a fast day), since it returns a very early time, and if used lekula can result in doing mitzvos hayom too early according to most opinions.")]
    public virtual DateTimeOffset? GetAlos26Degrees()
    {
        return ToZone(GetSunriseOffsetByDegrees(Zenith26Degrees));
    }

    /// <summary>
    /// A method to return <em>alos</em> (dawn) calculated when the sun is <see cref="AstronomicalCalendar.AstronomicalZenith">18°</see> below the
    /// eastern geometric horizon before sunrise.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing <em>alos</em>. If the calculation can't be computed such as northern
    ///         and southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun
    ///         may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="AstronomicalCalendar.AstronomicalZenith"/>
    public virtual DateTimeOffset? GetAlos18Degrees()
    {
        return ToZone(GetSunriseOffsetByDegrees(AstronomicalZenith));
    }

    /// <summary>
    /// A method to return <em>alos</em> (dawn) calculated when the sun is <see cref="Zenith19Degrees">19°</see> below the
    /// eastern geometric horizon before sunrise. This is the <a href="https://en.wikipedia.org/wiki/Maimonides"
    /// >Rambam</a>'s <em>alos</em> according to Rabbi Moshe Kosower's <a href=
    /// "https://www.worldcat.org/oclc/145454098">Maaglei Tzedek</a>, page 88, <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=33464&amp;pgnum=13">Ayeles Hashachar Vol. I, page 12</a>, <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=55960&amp;pgnum=258">Yom Valayla Shel Torah, Ch. 34, p. 222</a> and
    /// Rabbi Yaakov Shakow's <a href="https://www.worldcat.org/oclc/1043573513">Luach Ikvei Hayom</a>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing <em>alos</em>. If the calculation can't be computed such as northern
    ///         and southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun
    ///         may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual DateTimeOffset? GetAlos19Degrees()
    {
        return ToZone(GetSunriseOffsetByDegrees(Zenith19Degrees));
    }

    /// <summary>
    /// Method to return <em>alos</em> (dawn) calculated when the sun is <see cref="Zenith19Point8">19.8°</see> below the
    /// eastern geometric horizon before sunrise. This calculation is based on the same calculation of
    /// <see cref="GetAlos90Minutes">90 minutes</see> before sunrise, but uses a degree-based calculation instead of 90 exact minutes.
    /// This calculation is based on the position of the sun 90 minutes before sunrise in Jerusalem <a href=
    /// "https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a>, which
    /// calculates to 19.8° below <see cref="AstronomicalCalendar.GeometricZenith"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing <em>alos</em>. If the calculation can't be computed such as northern
    ///         and southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun
    ///         may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See
    ///         detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos90Minutes"/>
    /// <seealso cref="GetTzais19Point8Degrees"/>
    public virtual DateTimeOffset? GetAlos19Point8Degrees()
    {
        return ToZone(GetSunriseOffsetByDegrees(Zenith19Point8));
    }

    /// <summary>
    /// This method returns <em>misheyakir</em> based on the position of the sun <see cref="Zenith12Point85">12.85°</see>
    /// below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°). This is based on the position of the sun slightly
    /// later than 57 minutes before <see cref="AstronomicalCalendar.GetSunrise"/> in Jerusalem <a href=
    /// "https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a>. This
    /// <em>zman</em> is mentioned for use <b><em>bish'as hadchak</em></b> in the Birur Halacha <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=50535&amp;st=&amp;pgnum=88">Tinyana</a> and <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=50537&amp;st=&amp;pgnum=31">Tlisa'ah</a> in  Orach Chaim siman 18 as 12.85°.
    /// Actual calculations show it to be slightly more than 12.9°, but the Birur Halacha indicates that 12.85° is a
    /// slight <em>chumra</em> (on a <em>bedieved</em> time) VS the 12.9° that 57 minutes calculates as (a difference of
    /// about 14 seconds at the equinox/equilux in Jerusalem). The <em>zman</em> of 12.9° is also mentioned in the Piskei
    /// Tshuvos siman 18, page 190 (where a typo indicates that this is the degree equivalent to 60 minutes before sunrise,
    /// when in fact at that point the sun is about 13.5° below the horizon). The 57 minute based time is mentioned by the
    /// Minchas Yitzchak <a href="https://hebrewbooks.org/pdfpager.aspx?req=1601&amp;st=&amp;pgnum=21">vol. 9, siman 9</a> as 15 minutes
    /// before <em>alos hashachar</em> (though he is not clear what location he refers to, and does not mention a degree-based
    /// conversion). The Kaf Hachaim <a href="https://hebrewbooks.org/pdfpager.aspx?req=8140&amp;st=&amp;pgnum=81">vol.1 siman 18, no.
    /// 18</a> states that in Yerushalayim 60 fixed minutes are used year round. Calculations show that 60 fixed minutes in
    /// Yerushalayim ranges from 13.5° at the spring equinox to 11.5° at the summer solstice. 57-minute
    /// <em>misheyakir</em> range from 12.9° at the winter equinox to 11° at the summer solstice.
    /// Analysis of the difference between 12.85° and 12.9°, shows that the maximum difference occurs at the summer
    /// solstice. In Lakewood, NJ at a latitude of 40.096°, the maximum difference throughout the year is 23 seconds.
    /// In the winter where there is the greatest need for very early <em>misheyakir</em> times, the difference is in the 16
    /// second range. Going north to Montreal at latitude 45.5°, the maximum is 29 seconds and is about 18 seconds in the
    /// winter. Moving farther north to the latitude of Vilnius at a latitude of 54.68°, things change. Firstly, around the
    /// summer solstice it will not reach that far below the horizon. On the dates that both can be calculated, the maximum
    /// difference can be pretty high on one or two days of the year (around Jul 8),  with about a week having over a two minute
    /// difference between the two. Even at the latitude of Vilna, from Dec - March, the difference is about 22 seconds.
    /// </summary>
    /// <returns>The <see cref="DateTimeOffset"/> of <em>misheyakir</em>. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may
    ///         not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See
    ///         detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method returns a very early <em>misheyakir</em> time that should only be used <b><em>bish'as
    ///         hadchak</em></b>. <em>Lechatchila</em>, a later <em>zman</em> should be used. There is no current plan to remove
    ///         this method from the API, and this deprecation is intended to notify developers to add an alert to users of
    ///         the risk of using it.
    /// </remarks>
    /// <seealso cref="Zenith12Point85"/>
    [Obsolete("This method returns a very early misheyakir time that should only be used bish'as hadchak.")]
    public virtual DateTimeOffset? GetMisheyakir12Point85Degrees()
    {
        return ToZone(GetSunriseOffsetByDegrees(Zenith12Point85));
    }

    /// <summary>
    /// This method returns <em>misheyakir</em> based on the position of the sun when it is <see cref="Zenith11Degrees">11.5°</see> below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°). This calculation is used for calculating
    /// <em>misheyakir</em> according to some opinions. This calculation is based on the position of the sun 52 minutes
    /// before <see cref="AstronomicalCalendar.GetSunrise"/> in Jerusalem <a href=
    /// "https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a>,
    /// which calculates to 11.5° below <see cref="AstronomicalCalendar.GeometricZenith"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of <em>misheyakir</em>. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may
    ///         not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See
    ///         detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>To do:</b> recalculate.
    /// </remarks>
    /// <seealso cref="Zenith11Point5"/>
    public virtual DateTimeOffset? GetMisheyakir11Point5Degrees()
    {
        return ToZone(GetSunriseOffsetByDegrees(Zenith11Point5));
    }

    /// <summary>
    /// This method returns <em>misheyakir</em> based on the position of the sun when it is <see cref="Zenith11Degrees">11°</see> below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°). This calculation is used for calculating
    /// <em>misheyakir</em> according to some opinions. This calculation is based on the position of the sun 48 minutes
    /// before <see cref="AstronomicalCalendar.GetSunrise"/> in Jerusalem <a href=
    /// "https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a>,
    /// which calculates to 11° below <see cref="AstronomicalCalendar.GeometricZenith"/>.
    /// </summary>
    /// <returns>If the calculation can't be computed such as northern and southern locations even south of the Arctic Circle and north
    ///         of the Antarctic Circle where the sun may not reach low enough below the horizon for this calculation, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="Zenith11Degrees"/>
    public virtual DateTimeOffset? GetMisheyakir11Degrees()
    {
        return ToZone(GetSunriseOffsetByDegrees(Zenith11Degrees));
    }

    /// <summary>
    /// This method returns <em>misheyakir</em> based on the position of the sun when it is <see cref="Zenith10Point2">10.2°</see> below
    /// <see cref="AstronomicalCalendar.GeometricZenith"/> (90°). This calculation is used for calculating
    /// <em>misheyakir</em> according to some opinions. This calculation is based on the position of the sun 45 minutes
    /// before <see cref="AstronomicalCalendar.GetSunrise"/> in Jerusalem <a href=
    /// "https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox</a> which calculates
    /// to 10.2° below <see cref="AstronomicalCalendar.GeometricZenith"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of <em>misheyakir</em>. If the calculation can't be computed such as
    ///         northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where
    ///         the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="Zenith10Point2"/>
    public virtual DateTimeOffset? GetMisheyakir10Point2Degrees()
    {
        return ToZone(GetSunriseOffsetByDegrees(Zenith10Point2));
    }

    /// <summary>
    /// This method returns <em>misheyakir</em> based on the position of the sun when it is <see cref="Zenith7Point65">7.65°</see> below
    /// <see cref="AstronomicalCalendar.GeometricZenith"/> (90°). The degrees are based on a 35/36 minute <em>zman</em> <a href=
    /// "https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a>, when the
    /// <em>neshef</em> (twilight) is the shortest. This time is based on <a href=
    /// "https://en.wikipedia.org/wiki/Moshe_Feinstein">Rabbi Moshe Feinstein</a> who writes in <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=14677&amp;pgnum=7">Ohr Hachaim Vol. 4, Ch. 6</a> that <em>misheyakir</em> in New York
    /// is 35-40 minutes before sunrise, something that is a drop less than 8°. <a href="https://en.wikipedia.org/wiki/Yisroel_Taplin"
    /// >Rabbi Yisroel Taplin</a> in <a href="https://www.worldcat.org/oclc/889556744">Zmanei Yisrael</a> (page 117) notes that
    /// <a href="https://en.wikipedia.org/wiki/Yaakov_Kamenetsky">Rabbi Yaakov Kamenetsky</a> stated that it is not less than 36
    /// minutes before sunrise (maybe it is 40 minutes). Sefer Yisrael Vehazmanim (p. 7) quotes the Tamar Yifrach in the name of the
    /// <a href="https://en.wikipedia.org/wiki/Joel_Teitelbaum">Satmar Rov</a> that one should be stringent not consider
    /// <em>misheyakir</em> before 36 minutes. This is also the accepted <a href="https://en.wikipedia.org/wiki/Minhag">minhag</a> in
    /// <a href="https://en.wikipedia.org/wiki/Lakewood_Township,_New_Jersey">Lakewood</a> that is used in the <a href=
    /// "https://en.wikipedia.org/wiki/Beth_Medrash_Govoha">Yeshiva</a>. This follows the opinion of <a href=
    /// "https://en.wikipedia.org/wiki/Shmuel_Kamenetsky">Rabbi Shmuel Kamenetsky</a> who provided the time of 35/36 minutes, but did
    /// not provide a degree-based time. Since this <em>zman</em> depends on the level of light, Rabbi Yaakov Shakow presented this
    /// degree-based calculations to Rabbi Shmuel Kamenetsky who agreed to them.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of <em>misheyakir</em>. If the calculation can't be computed such as
    ///         northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where
    ///         the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="Zenith7Point65"/>
    /// <seealso cref="GetMisheyakir9Point5Degrees"/>
    public virtual DateTimeOffset? GetMisheyakir7Point65Degrees()
    {
        return ToZone(GetSunriseOffsetByDegrees(Zenith7Point65));
    }

    /// <summary>
    /// This method returns <em>misheyakir</em> based on the position of the sun when it is <see cref="Zenith9Point5">9.5°</see> below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°). This calculation is based on <a href=
    /// "https://en.wikipedia.org/wiki/Dovid_Kronglas">Rabbi Dovid Kronglass's</a> Calculation of 45 minutes in Baltimore
    /// as mentioned in <a href="https://hebrewbooks.org/pdfpager.aspx?req=20287&amp;pgnum=29">Divrei Chachamim No. 24</a>
    /// brought down by the <a href="https://hebrewbooks.org/pdfpager.aspx?req=50535&amp;pgnum=87">Birur Halacha, Tinyana, Ch.
    /// 18</a>. This calculates to 9.5°. Also see <a href="https://en.wikipedia.org/wiki/Jacob_Isaac_Neiman">Rabbi Yaakov
    /// Yitzchok Neiman</a> in Kovetz Eitz Chaim Vol. 9, p. 202 that the Vya'an Yosef did not want to rely on times earlier
    /// than 45 minutes in New York. This <em>zman</em> is also used in the calendars published by Rabbi Hershel Edelstein.
    /// As mentioned in Yisroel Vehazmanim, Rabbi Edelstein who was given the 45 minute <em>zman</em> by Rabbi Bick. The
    /// calendars published by the <em><a href="https://en.wikipedia.org/wiki/Mizrahi_Jews">Edot Hamizrach</a></em> communities
    /// also use this <em>zman</em>. This also follows the opinion of <a href="https://en.wikipedia.org/wiki/Shmuel_Kamenetsky"
    /// >Rabbi Shmuel Kamenetsky</a> who provided the time of 36 and 45 minutes, but did not provide a degree-based time. Since
    /// this <em>zman</em> depends on the level of light, Rabbi Yaakov Shakow presented these degree-based times to Rabbi Shmuel
    /// Kamenetsky who agreed to them.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of <em>misheyakir</em>. If the calculation can't be computed such as
    ///         northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where
    ///         the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="Zenith9Point5"/>
    /// <seealso cref="GetMisheyakir7Point65Degrees"/>
    public virtual DateTimeOffset? GetMisheyakir9Point5Degrees()
    {
        return ToZone(GetSunriseOffsetByDegrees(Zenith9Point5));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite Shema in the morning) according to the
    /// opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on
    /// <em>alos</em> being <see cref="GetAlos19Point8Degrees">19.8°</see> before <see cref="AstronomicalCalendar.GetSunrise"/>. This
    /// time is 3 <see cref="GetShaahZmanis19Point8Degrees"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="GetAlos19Point8Degrees">dawn</see> based on the opinion of the MGA that the day is calculated from dawn to nightfall
    /// with both being 19.8° below sunrise or sunset. This returns the time of 3 *
    /// <see cref="GetShaahZmanis19Point8Degrees"/> after <see cref="GetAlos19Point8Degrees">dawn</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis19Point8Degrees"/>
    /// <seealso cref="GetAlos19Point8Degrees"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA19Point8Degrees()
    {
        return ToZone(GetSofZmanShma(GetAlos19Point8Degrees(), GetTzais19Point8Degrees(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite Shema in the morning) according to the opinion of
    /// the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="ZmanimCalendar.GetAlos16Point1Degrees">16.1°</see> before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 3 <see cref="GetShaahZmanis16Point1Degrees"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="ZmanimCalendar.GetAlos16Point1Degrees">dawn</see> based on
    /// the opinion of the MGA that the day is calculated from dawn to nightfall with both being 16.1° below sunrise or sunset.
    /// This returns the time of 3 * <see cref="GetShaahZmanis16Point1Degrees"/> after <see cref="ZmanimCalendar.GetAlos16Point1Degrees">dawn</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis16Point1Degrees"/>
    /// <seealso cref="ZmanimCalendar.GetAlos16Point1Degrees"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA16Point1Degrees()
    {
        return ToZone(GetSofZmanShma(GetAlos16Point1Degrees(), GetTzais16Point1Degrees(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite Shema in the morning) according to the
    /// opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based
    /// on <em>alos</em> being <see cref="GetAlos18Degrees">18°</see> before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 3
    /// <see cref="GetShaahZmanis18Degrees"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="GetAlos18Degrees">dawn</see>
    /// based on the opinion of the MGA that the day is calculated from dawn to nightfall with both being 18°
    /// below sunrise or sunset. This returns the time of 3 * <see cref="GetShaahZmanis18Degrees"/> after
    /// <see cref="GetAlos18Degrees">dawn</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis18Degrees"/>
    /// <seealso cref="GetAlos18Degrees"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA18Degrees()
    {
        return ToZone(GetSofZmanShma(GetAlos18Degrees(), GetTzais18Degrees(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite <em>Shema</em> in the morning) according
    /// to the opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based
    /// on <em>alos</em> being <see cref="GetAlos72Zmanis">72</see> minutes <em>zmaniyos</em>, or 1/10th of the day before
    /// <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 3 <see cref="GetShaahZmanis72MinutesZmanis"><em>shaos zmaniyos</em></see>
    /// (solar hours) after <see cref="GetAlos72Zmanis">dawn</see> based on the opinion of the MGA that the day is calculated
    /// from a <see cref="GetAlos72Zmanis">dawn</see> of 72 minutes <em>zmaniyos</em>, or 1/10th of the day before
    /// <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> to <see cref="GetTzais72Zmanis">nightfall</see> of 72 minutes
    /// <em>zmaniyos</em> after <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see>. This returns the time of 3 *
    /// <see cref="GetShaahZmanis72MinutesZmanis"/> after <see cref="GetAlos72Zmanis">dawn</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis72MinutesZmanis"/>
    /// <seealso cref="GetAlos72Zmanis"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA72MinutesZmanis()
    {
        return ToZone(GetSofZmanShma(GetAlos72Zmanis(), GetTzais72Zmanis(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite <em>Shema</em> in the morning) according
    /// to the opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on
    /// <em>alos</em> being <see cref="GetAlos90Minutes">90</see> minutes before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 3
    /// <see cref="GetShaahZmanis90Minutes"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="GetAlos90Minutes">dawn</see> based on
    /// the opinion of the MGA that the day is calculated from a <see cref="GetAlos90Minutes">dawn</see> of 90 minutes before sunrise to
    /// <see cref="GetTzais90Minutes">nightfall</see> of 90 minutes after sunset. This returns the time of 3 *
    /// <see cref="GetShaahZmanis90Minutes"/> after <see cref="GetAlos90Minutes">dawn</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where
    ///         it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis90Minutes"/>
    /// <seealso cref="GetAlos90Minutes"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA90Minutes()
    {
        return ToZone(GetSofZmanShma(GetAlos90Minutes(), GetTzais90Minutes(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite Shema in the morning) according to the
    /// opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based
    /// on <em>alos</em> being <see cref="GetAlos90Zmanis">90</see> minutes <em>zmaniyos</em> before <see cref="AstronomicalCalendar.GetSunrise">sunrise</see>. This time is 3 <see cref="GetShaahZmanis90MinutesZmanis"><em>shaos zmaniyos</em></see> (solar hours) after
    /// <see cref="GetAlos90Zmanis">dawn</see> based on the opinion of the MGA that the day is calculated from a <see cref="GetAlos90Zmanis">dawn</see> of 90 minutes <em>zmaniyos</em> before sunrise to <see cref="GetTzais90Zmanis">nightfall</see>
    /// of 90 minutes <em>zmaniyos</em> after sunset. This returns the time of 3 * <see cref="GetShaahZmanis90MinutesZmanis"/>
    /// after <see cref="GetAlos90Zmanis">dawn</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where
    ///         it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis90MinutesZmanis"/>
    /// <seealso cref="GetAlos90Zmanis"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA90MinutesZmanis()
    {
        return ToZone(GetSofZmanShma(GetAlos90Zmanis(), GetTzais90Zmanis(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite Shema in the morning) according to the opinion of
    /// the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos96Minutes">96</see> minutes before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 3 <see cref="GetShaahZmanis96Minutes"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="GetAlos96Minutes">dawn</see> based on the opinion
    /// of the MGA that the day is calculated from a <see cref="GetAlos96Minutes">dawn</see> of 96 minutes before sunrise to <see cref="GetTzais96Minutes">nightfall</see> of 96 minutes after sunset. This returns the time of 3 * <see cref="GetShaahZmanis96Minutes"/>
    /// after <see cref="GetAlos96Minutes">dawn</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where
    ///         it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis96Minutes"/>
    /// <seealso cref="GetAlos96Minutes"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA96Minutes()
    {
        return ToZone(GetSofZmanShma(GetAlos96Minutes(), GetTzais96Minutes(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite Shema in the morning) according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos96Zmanis">96</see> minutes <em>zmaniyos</em> before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 3 <see cref="GetShaahZmanis96MinutesZmanis"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="GetAlos96Zmanis">dawn</see> based on the
    /// opinion of the MGA that the day is calculated from a <see cref="GetAlos96Zmanis">dawn</see> of 96 minutes <em>zmaniyos</em> before
    /// sunrise to <see cref="GetTzais96Zmanis">nightfall</see> of 96 minutes <em>zmaniyos</em> after sunset. This returns the time of 3 *
    /// <see cref="GetShaahZmanis96MinutesZmanis"/> after <see cref="GetAlos96Zmanis">dawn</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where
    ///         it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis96MinutesZmanis"/>
    /// <seealso cref="GetAlos96Zmanis"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA96MinutesZmanis()
    {
        return ToZone(GetSofZmanShma(GetAlos96Zmanis(), GetTzais96Zmanis(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite <em>Shema</em> in the morning) calculated as 3 hours
    /// (regular clock hours and not <em>shaos zmaniyos</em>) before <see cref="ZmanimCalendar.GetChatzosHayom"/>. Generally known as part of the
    /// "Komarno" <em>zmanim</em> after <a href="https://en.wikipedia.org/wiki/Komarno_(Hasidic_dynasty)#Rabbi_Yitzchak_Eisik_Safrin"
    /// >Rav Yitzchak Eizik of Komarno</a>, a proponent of this calculation, it actually predates him a lot. It is the opinion of the
    /// <em>Shach</em> in the Nekudas Hakesef (Yoreh Deah 184), <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=21638&amp;st=&amp;pgnum=30">Rav Moshe Lifshitz</a> in his commentary <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=21638&amp;st=&amp;pgnum=50">Lechem Mishneh on Brachos 1:2</a>. It is next brought down
    /// about 100 years later by the <a href="https://en.wikipedia.org/wiki/Jacob_Emden">Yaavetz</a> (in his <em>siddur</em>,
    /// <a href="https://hebrewbooks.org/pdfpager.aspx?req=7920&amp;st=&amp;pgnum=6">Mor Uktziah Orach
    /// Chaim 1</a>, <a href="https://hebrewbooks.org/pdfpager.aspx?req=22309&amp;st=&amp;pgnum=30">Lechem Shamayim, Brachos 1:2</a> and
    /// <a href="https://hebrewbooks.org/pdfpager.aspx?req=1408&amp;st=&amp;pgnum=69">She'elos Yaavetz vol. 1 no. 40</a>), Rav Yitzchak Eizik
    /// of Komarno in the Ma'aseh Oreg on Mishnayos Brachos 11:2, Shevus Yaakov, Chasan Sofer and others. See Yisrael Vehazmanim
    /// <a href="https://hebrewbooks.org/pdfpager.aspx?req=9765&amp;st=&amp;pgnum=83">vol. 1 7:3, page 55 - 62</a>. If the calculated time
    /// is before sunrise (on short winter days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where
    ///         it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetChatzosHayom"/>
    /// <seealso cref="GetSofZmanTfila2HoursBeforeChatzos"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzos"/>
    public virtual DateTimeOffset? GetSofZmanShma3HoursBeforeChatzos()
    {
        DateTimeOffset? sofZmanShma = GetTimeOffset(GetChatzosHayom(), JavaTime.OfHours(-3));
        DateTimeOffset? sunrise = GetSunriseBasedOnElevationSetting();
        // On a short winter day, 3 hours before chatzos is before sunrise
        if (sofZmanShma != null && sunrise != null && sofZmanShma.IsBefore(sunrise))
        {
            return null;
        }
        return ToZone(sofZmanShma);
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite Shema in the morning) according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos120Minutes">120</see> minutes or 1/6th of the day before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 3 <see cref="GetShaahZmanis120Minutes"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="GetAlos120Minutes">dawn</see> based on the
    /// opinion of the MGA that the day is calculated from a <see cref="GetAlos120Minutes">dawn</see> of 120 minutes before sunrise to <see cref="GetTzais120Minutes">nightfall</see> of 120 minutes after sunset. This returns the time of 3 <see cref="GetShaahZmanis120Minutes"/>
    /// after <see cref="GetAlos120Minutes">dawn</see>. This is an extremely early <em>zman</em> that is very much a <em>chumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where
    ///         it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis120Minutes"/>
    /// <seealso cref="GetAlos120Minutes"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA120Minutes()
    {
        return ToZone(GetSofZmanShma(GetAlos120Minutes(), GetTzais120Minutes(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite <em>Shema</em> in the morning) based on the opinion
    /// that the day starts at <see cref="ZmanimCalendar.GetAlos16Point1Degrees"><em>alos</em> 16.1°</see> and ends at <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see>. This is the opinion of the <a href=
    /// "https://hebrewbooks.org/40357">חידושי וכללות הרז״ה</a> in <a href="https://hebrewbooks.org/pdfpager.aspx?req=40357&amp;pgnum=71"
    /// >סימן ב׳ סע״ג וסע״ד</a> and <a href="https://hebrewbooks.org/pdfpager.aspx?req=40357&amp;pgnum=93">סי׳ ז׳ סע״ב</a> and the <a href=
    /// "https://hebrewbooks.org/14799">מנורה הטהורה</a> as mentioned by <span>ישראל והזמנים</span> <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=9765&amp;pgnum=81">ח״א סי׳ ז׳ פ״ג או׳ ט״ז</a>. Also see <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=67890&amp;pgnum=29">אשר לשלמה, מועד, סי׳ ה׳</a>. Three <em>shaos zmaniyos</em> are calculated
    /// based on this day and added to <see cref="ZmanimCalendar.GetAlos16Point1Degrees"><em>alos</em></see> to reach this time. This time is 3 <em>shaos
    /// zmaniyos</em> (solar hours) after <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/> based on the opinion that the day is calculated from
    /// <em>alos</em> 16.1° to <see cref="AstronomicalCalendar.GetSeaLevelSunset"/>. <b>Note: </b> Based on this calculation <em>chatzos</em> will not be at
    /// midday and <see cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/> will be ignored. Sources for an asymmetrical day-based
    /// calculation can be seen in the documentation of <see cref="GetSofZmanShmaAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em> based on this day. If the calculation can't
    ///         be computed such as northern and southern locations even south of the Arctic Circle and north of the
    ///         Antarctic Circle where the sun may not reach low enough below the horizon for this calculation, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetAlos16Point1Degrees"/>
    /// <seealso cref="AstronomicalCalendar.GetSeaLevelSunset"/>
    public virtual DateTimeOffset? GetSofZmanShmaAlos16Point1ToSunset()
    {
        return ToZone(GetSofZmanShma(GetAlos16Point1Degrees(), GetSunsetBasedOnElevationSetting(), false));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite Shema in the morning) based on the opinion that the
    /// day is an asymmetrical day that starts at <see cref="ZmanimCalendar.GetAlos16Point1Degrees"><em>alos</em> 16.1°</see> and ends at <see cref="GetTzaisGeonim7Point083Degrees"><em>tzais</em> 7.083°</see>. 3 <em>shaos zmaniyos</em> (temporal hours) are calculated based on
    /// this day and added to <see cref="ZmanimCalendar.GetAlos16Point1Degrees"><em>alos</em></see> to reach this time. Sefer <a href=
    /// "https://hebrewbooks.org/9765">Yisroel</a> <a href="https://hebrewbooks.org/9764">Vehazmanim</a> V. III in the explanation of
    /// <a href="https://hebrewbooks.org/pdfpager.aspx?req=9764&amp;st=&amp;pgnum=260"><em>zman</em> no. 22 on page 1015</a> attributes the
    /// origin of this <em>zman</em> to <a href="https://en.wikipedia.org/wiki/Jacob_al-Faraji">Rabbi Yaakov al-Faraji's</a> <a href=
    /// "https://hebrewbooks.org/1151">שו״ת מהרי״ף</a> (who discusses <em>zmanim</em> in various places including chapters 6, 46, 47
    /// and 49), <a href="https://en.wikipedia.org/wiki/Joseph_Schwarz_(geographer)">Rabbi Yehosef Schwarz</a> in the <a href=
    /// "https://hebrewbooks.org/31703">דברי יוסף</a> in the name of <a href="https://en.wikipedia.org/wiki/Nathan_Adler"
    /// >Rabbi Nosson Adler</a>, the <a href="https://en.wikipedia.org/wiki/Yosef_Hayyim">Ben Ish Chai</a> in <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=14054&amp;st=&amp;pgnum=108">שנה א׳ פ׳ ויקהל</a>, <a href=
    /// "https://he.wikipedia.org/wiki/%D7%A0%D7%A1%D7%99%D7%9D_%D7%9B%D7%93%D7%95%D7%A8%D7%99_%D7%97%D7%96%D7%9F">Rabbi Nissim
    /// Kaduri Chazan's</a> <a href="https://www.nli.org.il/he/books/NNL_ALEPH990019649990205171/NLI">מעשה נסים</a> (in the calendar at
    /// the end), <a href="https://he.wikipedia.org/wiki/%D7%9E%D7%A6%D7%9C%D7%99%D7%97_%D7%9E%D7%90%D7%96%D7%95%D7%96">Rabbi Matzliach
    /// Mazuz's</a> שו״ת איש מצליח, in Dr. Baruch (Berthold) Cohn's <em>luach</em> <a href=
    /// "https://sammlungen.ub.uni-frankfurt.de/freimann/content/titleinfo/983088">Tabellen enthaltend die Zeitangaben für den Beginn
    /// der Nacht und des Tages für die Breitengrade + 66 bis -38</a>, שו״ת תשובות והנהגות ח״א <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=20025&amp;st=&amp;pgnum=50">no. 55</a> and <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=20025&amp;st=&amp;pgnum=218">307</a> (<em>lechumra</em> in the name of the <a href=
    /// "https://en.wikipedia.org/wiki/Yehoshua_Leib_Diskin">מהרי״ל דיסקין</a> and others) who calculate <em>shaos zmaniyos</em> based on
    /// an asymmetrical day from an early <em>alos</em> to an early <em>tzais</em> <b>Note: </b> Based on this calculation
    /// <em>chatzos</em> will not be at midday and <see cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/> will be ignored. There are many
    /// variations of this type of zman, with varying start and end times. The Yisroel Vehazmanim gave this as a relatively common one.
    /// Variations using this library are very simple. For example to calculate <em>sof zman krias shema</em> starting the day at the
    /// Rambam's alos of 18° and ending it at 3.7°, use the following code.
    /// <code>
    /// DateTimeOffset? sofZmanKS = czc.GetSofZmanShma(czc.GetAlos18Degrees(), czc.GetTzaisGeonim3Point7Degrees(), false);
    /// </code>
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em> based on this calculation. If the calculation can't be
    ///         computed such as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where
    ///         the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetAlos16Point1Degrees"/>
    /// <seealso cref="GetTzaisGeonim7Point083Degrees"/>
    /// <seealso cref="GetPlagAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>
    /// <seealso cref="GetShaahZmanisAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>
    public virtual DateTimeOffset? GetSofZmanShmaAlos16Point1DegreesToTzaisGeonim7Point083Degrees()
    {
        return ToZone(GetSofZmanShma(GetAlos16Point1Degrees(), GetTzaisGeonim7Point083Degrees(), false));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to recite the morning prayers) according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos19Point8Degrees">19.8°</see> before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 4 <see cref="GetShaahZmanis19Point8Degrees"/> after
    /// <see cref="GetAlos19Point8Degrees"/> based on the opinion that the day is calculated from dawn to nightfall with both being 19.8°
    /// below sunrise or sunset.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis19Point8Degrees"/>
    /// <seealso cref="GetAlos19Point8Degrees"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/>
    public virtual DateTimeOffset? GetSofZmanTfilaMGA19Point8Degrees()
    {
        return ToZone(GetSofZmanTfila(GetAlos19Point8Degrees(), GetTzais19Point8Degrees(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to recite the morning prayers) according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on * <em>alos</em> being <see cref="ZmanimCalendar.GetAlos16Point1Degrees">16.1°</see> before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 4 <see cref="GetShaahZmanis16Point1Degrees"/> after
    /// <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/> based on the opinion that the day is calculated from dawn to nightfall with both being 16.1°
    /// below sunrise or sunset.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis16Point1Degrees"/>
    /// <seealso cref="ZmanimCalendar.GetAlos16Point1Degrees"/>
    public virtual DateTimeOffset? GetSofZmanTfilaMGA16Point1Degrees()
    {
        return ToZone(GetSofZmanTfila(GetAlos16Point1Degrees(), GetTzais16Point1Degrees(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to recite the morning prayers) according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos18Degrees"/> before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 4 <see cref="GetShaahZmanis18Degrees"/> after <see cref="GetAlos18Degrees">dawn</see> based on the opinion that the day is calculated from dawn to nightfall with both being 18° below
    /// sunrise and sunset.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em>. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may not reach low
    ///         enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis18Degrees"/>
    /// <seealso cref="GetAlos18Degrees"/>
    public virtual DateTimeOffset? GetSofZmanTfilaMGA18Degrees()
    {
        return ToZone(GetSofZmanTfila(GetAlos18Degrees(), GetTzais18Degrees(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to the morning prayers) according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em>
    /// being <see cref="GetAlos72Zmanis"/> 72 minutes <em>zmaniyos</em> or a 10th of the day before <see cref="AstronomicalCalendar.GetSunrise"/>. This time
    /// is 4 <see cref="GetShaahZmanis72MinutesZmanis"/> after <see cref="GetAlos72Zmanis"/> based on the opinion that the day is calculated
    /// from a <see cref="GetAlos72Zmanis"/> before sunrise to <see cref="GetTzais72Zmanis"/> of 72 minutes <em>zmaniyos</em> after sunset.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis72MinutesZmanis"/>
    /// <seealso cref="GetAlos72Zmanis"/>
    public virtual DateTimeOffset? GetSofZmanTfilaMGA72MinutesZmanis()
    {
        return ToZone(GetSofZmanTfila(GetAlos72Zmanis(), GetTzais72Zmanis(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to recite the morning prayers) according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos90Minutes"/> minutes before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 4 <see cref="GetShaahZmanis90Minutes"/> after <see cref="GetAlos90Minutes"/> based on the opinion that the day is calculated from a <see cref="GetAlos90Minutes"/> before sunrise to
    /// <see cref="GetTzais90Minutes"/> after sunset.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis90Minutes"/>
    /// <seealso cref="GetAlos90Minutes"/>
    public virtual DateTimeOffset? GetSofZmanTfilaMGA90Minutes()
    {
        return ToZone(GetSofZmanTfila(GetAlos90Minutes(), GetTzais90Minutes(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to the morning prayers) according to the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos90Zmanis"/> 90 minutes <em>zmaniyos</em> or 1/8th of the day before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 4 <see cref="GetShaahZmanis90MinutesZmanis"/> after <see cref="GetAlos90Zmanis"/> based on the opinion that the day is calculated from a <see cref="GetAlos90Zmanis"/> to <see cref="GetTzais90Zmanis"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis90MinutesZmanis"/>
    /// <seealso cref="GetAlos90Zmanis"/>
    public virtual DateTimeOffset? GetSofZmanTfilaMGA90MinutesZmanis()
    {
        return ToZone(GetSofZmanTfila(GetAlos90Zmanis(), GetTzais90Zmanis(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to recite the morning prayers) according to the opinion
    /// of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on
    /// <em>alos</em> being <see cref="GetAlos96Minutes"/> minutes before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 4
    /// <see cref="GetShaahZmanis96Minutes"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="GetAlos96Minutes">dawn</see> based on
    /// the opinion of the MGA that the day is calculated from a <see cref="GetAlos96Minutes">dawn</see> of 96 minutes before
    /// sunrise to <see cref="GetTzais96Minutes">nightfall</see> of 96 minutes after sunset. This returns the time of 4 *
    /// <see cref="GetShaahZmanis96Minutes"/> after <see cref="GetAlos96Minutes">dawn</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em>. If the calculation can't be computed such as in the Arctic Circle
    ///         where there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis96Minutes"/>
    /// <seealso cref="GetAlos96Minutes"/>
    public virtual DateTimeOffset? GetSofZmanTfilaMGA96Minutes()
    {
        return ToZone(GetSofZmanTfila(GetAlos96Minutes(), GetTzais96Minutes(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to the morning prayers) according to the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos96Zmanis"/> 96 minutes <em>zmaniyos</em> before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 4 <see cref="GetShaahZmanis96MinutesZmanis"/> after <see cref="GetAlos96Zmanis"/> based on the opinion that the day is calculated from a
    /// <see cref="GetAlos96Zmanis"/> before sunrise to <see cref="GetTzais96Zmanis"/> after sunset. This returns the time of 4 * <see cref="GetShaahZmanis96MinutesZmanis"/> after <see cref="GetAlos96Zmanis"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis96MinutesZmanis"/>
    /// <seealso cref="GetAlos96Zmanis"/>
    public virtual DateTimeOffset? GetSofZmanTfilaMGA96MinutesZmanis()
    {
        return ToZone(GetSofZmanTfila(GetAlos96Zmanis(), GetTzais96Zmanis(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to recite the morning prayers) according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos120Minutes"/> before <see cref="AstronomicalCalendar.GetSunrise"/> . This time is 4 <see cref="GetShaahZmanis120Minutes"/> after <see cref="GetAlos120Minutes"/> based on the opinion that the day is calculated from a <see cref="GetAlos120Minutes"/>, 120 minutes before
    /// sunrise to <see cref="GetTzais120Minutes"/>, 120 minutes after sunset. This is an extremely early <em>zman</em> that is very
    /// stringent <em>chumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em>. If the calculation can't be computed such
    ///         as in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where
    ///         it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis120Minutes"/>
    /// <seealso cref="GetAlos120Minutes"/>
    public virtual DateTimeOffset? GetSofZmanTfilaMGA120Minutes()
    {
        return ToZone(GetSofZmanTfila(GetAlos120Minutes(), GetTzais120Minutes(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to recite the morning prayers) calculated as 2 hours before <see cref="ZmanimCalendar.GetChatzosHayom"/>. This is based on the opinions that calculate <em>sof zman krias shema</em> as <see cref="GetSofZmanShma3HoursBeforeChatzos"/>. This returns the time of 2 hours before <see cref="ZmanimCalendar.GetChatzosHayom"/>. If the calculated
    /// time is before sunrise (on short winter days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em>. If the calculation can't be computed such
    ///         as in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where
    ///         it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetChatzosHayom"/>
    /// <seealso cref="GetSofZmanShma3HoursBeforeChatzos"/>
    public virtual DateTimeOffset? GetSofZmanTfila2HoursBeforeChatzos()
    {
        DateTimeOffset? sofZmanTfila = GetTimeOffset(GetChatzosHayom(), JavaTime.OfHours(-2));
        DateTimeOffset? sunrise = GetSunriseBasedOnElevationSetting();
        // On a short winter day, 2 hours before chatzos is before sunrise
        if (sofZmanTfila != null && sunrise != null && sofZmanTfila.IsBefore(sunrise))
        {
            return null;
        }
        return ToZone(sofZmanTfila);
    }

    /// <summary>
    /// This method returns <em>mincha gedola</em> calculated as 30 minutes after <see cref="ZmanimCalendar.GetChatzosHayom"><em>chatzos</em></see>
    /// and not 1/2 of a <see cref="ZmanimCalendar.GetShaahZmanisGRA"><em>shaah zmanis</em></see> after <see cref="ZmanimCalendar.GetChatzosHayom"><em>chatzos</em></see> as
    /// calculated by <see cref="ZmanimCalendar.GetMinchaGedola(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>. Some use this time to delay the start of <em>mincha</em> in the winter when
    /// 1/2 of a <see cref="ZmanimCalendar.GetShaahZmanisGRA"><em>shaah zmanis</em></see> is less than 30 minutes. See <see cref="GetMinchaGedolaGreaterThan30"/> for a convenience method that returns the later of the 2 calculations. One should not
    /// use this time to start <em>mincha</em> before the standard <see cref="ZmanimCalendar.GetMinchaGedolaGRA"><em>mincha gedola</em></see>. See Shulchan
    /// Aruch <a href="https://hebrewbooks.org/pdfpager.aspx?req=49624&amp;st=&amp;pgnum=291">Orach Chayim 234:1</a> and the Shaar Hatziyon
    /// <em>seif katan ches</em>. Since this calculation is a fixed 30 minutes of regular clock time after <em>chatzos</em>, even if
    /// <see cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/> is <c>false</c>, this <em>mincha gedola</em> time will be affected by
    /// <see cref="ZmanimCalendar.UseAstronomicalChatzos"/> and not by <see cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/>. If the calculated time is
    /// after sunset (on short Polar days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of 30 minutes after <em>chatzos</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>To do:</b> Consider adjusting this to calculate the time as half an hour after <see cref="ZmanimCalendar.GetChatzosHayom"/> that uses
    ///         <see cref="ZmanimCalendar.UseAstronomicalChatzos"/> to determine the type of <em>chatzos</em> to utilize. that support it,
    ///         based on <see cref="ZmanimCalendar.UseAstronomicalChatzos"/>.
    /// </remarks>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="GetMinchaGedolaGreaterThan30"/>
    /// <seealso cref="ZmanimCalendar.GetChatzosHayom"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzos"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/>
    public virtual DateTimeOffset? GetMinchaGedola30Minutes()
    {
        DateTimeOffset? minchaGedola30 = GetTimeOffset(GetChatzosHayom(), JavaTime.OfMinutes(30));
        DateTimeOffset? sunset = GetSunsetBasedOnElevationSetting();
        if (minchaGedola30 != null && sunset != null && minchaGedola30.IsAfter(sunset))
        { // very short polar day: 30 minutes after chatzos is already night
            return null;
        }
        return ToZone(minchaGedola30);
    }

    /// <summary>
    /// This method returns the time of <em>mincha gedola</em> according to the Magen Avraham with the day starting 72
    /// minutes before sunrise and ending 72 minutes after sunset. This is the earliest time to pray <em>mincha</em>. For
    /// more information on this see the documentation on <see cref="ZmanimCalendar.GetMinchaGedolaGRA"><em>mincha gedola</em></see>. This is
    /// calculated as 6.5 <see cref="AstronomicalCalendar.GetTemporalHour()">solar hours</see> after <em>alos</em>. The calculation used is 6.5 *
    /// <see cref="ZmanimCalendar.GetShaahZmanis72Minutes"/> after <see cref="ZmanimCalendar.GetAlos72Minutes"><em>alos</em></see>. If <see cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/> is set to <c>true</c>, the calculation will be based on 0.5
    /// <see cref="ZmanimCalendar.GetHalfDayBasedShaahZmanis">half-day based <em>sha'ah zmanis</em></see> between
    /// <see cref="ZmanimCalendar.GetChatzosHayom"/> and <see cref="ZmanimCalendar.GetTzais72Minutes"/> after <see cref="ZmanimCalendar.GetChatzosHayom"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>mincha gedola</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetAlos72Minutes"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaKetanaGRA"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="ZmanimCalendar.GetChatzosHayom"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/>
    public virtual DateTimeOffset? GetMinchaGedola72Minutes()
    {
        if (UseAstronomicalChatzosForOtherZmanim)
        {
            return ToZone(GetHalfDayBasedZman(GetChatzosHayom(), GetTzais72Minutes(), 0.5));
        }
        else
        {
            return ToZone(GetMinchaGedola(GetAlos72Minutes(), GetTzais72Minutes(), true));
        }
    }

    /// <summary>
    /// This method returns the time of <em>mincha gedola</em> according to the Magen Avraham with the day starting and ending 16.1°
    /// below the horizon. This is the earliest time to pray <em>mincha</em>. For more information on <em>mincha gedola</em>, see
    /// <see cref="ZmanimCalendar.GetMinchaGedolaGRA"/>. This is calculated as 6.5 <see cref="GetShaahZmanis16Point1Degrees"/> after <em>alos</em>. If
    /// <see cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/> is set to <c>true</c>, the calculation will be based on 0.5 <see cref="ZmanimCalendar.GetHalfDayBasedShaahZmanis">half-day based <em>sha'ah zmanis</em></see> between <see cref="ZmanimCalendar.GetChatzosHayom"/> and
    /// <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/> after <see cref="ZmanimCalendar.GetChatzosHayom"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>mincha gedola</em>. If the calculation can't be computed such as
    ///         northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where
    ///         the sun  may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis16Point1Degrees"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaKetanaGRA"/>
    public virtual DateTimeOffset? GetMinchaGedola16Point1Degrees()
    {
        if (UseAstronomicalChatzosForOtherZmanim)
        {
            return ToZone(GetHalfDayBasedZman(GetChatzosHayom(), GetTzais16Point1Degrees(), 0.5));
        }
        else
        {
            return ToZone(GetMinchaGedola(GetAlos16Point1Degrees(), GetTzais16Point1Degrees(), true));
        }
    }

    /// <summary>
    /// This method returns the time of <em>mincha gedola</em> based on the opinion of <a href=
    /// "https://en.wikipedia.org/wiki/Yaakov_Moshe_Hillel">Rabbi Yaakov Moshe Hillel</a> as published in the <em>luach</em> of the
    /// Bais Horaah of Yeshivat Chevrat Ahavat Shalom that <em>mincha gedola</em> is calculated as half a <em>shaah zmanis</em> after
    /// <em>chatzos</em> with <em>shaos zmaniyos</em> calculated based on a day starting at <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/> and
    /// ending at <see cref="GetTzaisGeonim3Point7Degrees"/>. The later of this time or 30 clock minutes after <em>chatzos</em> is
    /// returned. See <see cref="GetMinchaGedolaGreaterThan30"/> for a way to calculate the later of 30 minutes or this <em>mincha
    /// gedola</em>. For more information about <em>mincha gedola</em> see the documentation on <see cref="ZmanimCalendar.GetMinchaGedolaGRA"><em>mincha gedola</em></see>. Since calculation of this <em>zman</em> involves <em>chatzos</em> that is offset from the center of
    /// the astronomical day, <see cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/> is N/A here. Sources for an asymmetrical day-based
    /// calculation can be seen in the documentation of <see cref="GetSofZmanShmaAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>.
    /// In Polar regions of very short winter days it is possible that Mincha Gedola will occur, but 30 minutes later will be after
    /// sunset. In those cases, Mincha Gedola will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the <em>mincha gedola</em>. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may not
    ///         reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetAlos16Point1Degrees"/>
    /// <seealso cref="GetTzaisGeonim3Point7Degrees"/>
    /// <seealso cref="GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point7Degrees"/>
    /// <seealso cref="GetMinchaGedolaGreaterThan30"/>
    public virtual DateTimeOffset? GetMinchaGedolaAhavatShalom()
    {
        DateTimeOffset? chatzos = GetChatzosHayom();
        TimeSpan? shaahZmanis = GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point7Degrees();
        if (chatzos == null || shaahZmanis == null)
        {
            return null;
        }
        return ToZone(GetMinchaGedolaGreaterThan30(GetTimeOffset(chatzos, shaahZmanis.DividedBy(2))));
    }

    /// <summary>
    /// This is a convenience method that returns the later of the mincha gedola passed in and <see cref="GetMinchaGedola30Minutes"/>. In the winter when 1/2 of a <see cref="ZmanimCalendar.GetShaahZmanisGRA"><em>shaah zmanis</em></see> is less
    /// than 30 minutes <see cref="GetMinchaGedola30Minutes"/> will be returned, otherwise the mincha gedola passed in will be
    /// returned. Since this calculation can be an offset of <em>chatzos</em> (if 30 clock minutes > 1/2 of a <em>shaah
    /// zmanis</em>), even if <see cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/> is <c>false</c>, this <em>mincha</em> time
    /// may be affected by <see cref="ZmanimCalendar.UseAstronomicalChatzos"/>. In Polar regions of very short winter days it is possible that
    /// Mincha Gedola will occur, but 30 minutes later will be after sunset. In those cases, Mincha Gedola will be returned.
    /// </summary>
    /// <param name="minchaGedola">the mincha gedola to be compared to <see cref="GetMinchaGedola30Minutes"/>.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the later of <see cref="ZmanimCalendar.GetMinchaGedolaGRA"/> and <see cref="GetMinchaGedola30Minutes"/>.
    ///         If the calculation can't be computed such as in the Arctic Circle where there is at least one day a year
    ///         where the sun does not rise, and one where it does not set, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="GetMinchaGedola30Minutes"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzos"/>
    public virtual DateTimeOffset? GetMinchaGedolaGreaterThan30(DateTimeOffset? minchaGedola)
    {
        if (minchaGedola == null)
        {
            return null;
        }
        DateTimeOffset? minchaGedola30 = GetMinchaGedola30Minutes();
        // check for polar regions on a very short day where Mincha Gedola can occur, but 30 minutes later will be after sunset
        if (minchaGedola30 == null)
        {
            return ToZone(minchaGedola);
        }
        return ToZone(minchaGedola30.IsAfter(minchaGedola) ? minchaGedola30 : minchaGedola);
    }

    /// <summary>
    /// This is a convenience method that returns the later of <see cref="ZmanimCalendar.GetMinchaGedolaGRA"/> and <see cref="GetMinchaGedola30Minutes"/>.
    /// In the winter when 1/2 of a <see cref="ZmanimCalendar.GetShaahZmanisGRA"><em>shaah zmanis</em></see> is less than 30 minutes <see cref="GetMinchaGedola30Minutes"/> will be returned, otherwise <see cref="ZmanimCalendar.GetMinchaGedolaGRA"/> will be returned. Since this
    /// calculation can be an offset of <em>chatzos</em> (if 30 clock minutes > 1/2 of a <em>shaah zmanis</em>), even if <see cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/> is <c>false</c>, this <em>mincha</em> time may be affected by
    /// <see cref="ZmanimCalendar.UseAstronomicalChatzos"/>. In Polar regions of very short winter days it is possible that Mincha Gedola will occur,
    /// but 30 minutes later will be after sunset. In those cases, Mincha Gedola will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the later of <see cref="ZmanimCalendar.GetMinchaGedolaGRA"/> and <see cref="GetMinchaGedola30Minutes"/>.
    ///         If the calculation can't be computed such as in the Arctic Circle where there is at least one day a year
    ///         where the sun does not rise, and one where it does not set, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>To do:</b> Consider adjusting this to calculate the time as 30 minutes after <see cref="ZmanimCalendar.GetChatzosHayom"/> that uses <see cref="ZmanimCalendar.UseAstronomicalChatzos"/> to determine the type of <em>chatzos</em> to utilize (if the <see cref="AstronomicalCalculator">calculator</see> support astronomical <em>chatzos</em>),
    ///         based on the <see cref="ZmanimCalendar.UseAstronomicalChatzos"/> setting.
    /// </remarks>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="GetMinchaGedola30Minutes"/>
    /// <seealso cref="GetMinchaGedolaGreaterThan30"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzos"/>
    public virtual DateTimeOffset? GetMinchaGedolaGRAGreaterThan30()
    {
        return ToZone(GetMinchaGedolaGreaterThan30(GetMinchaGedolaGRA()));
    }

    /// <summary>
    /// This method returns the time of <em>mincha ketana</em> according to the Magen Avraham with the day starting and ending 16.1°
    /// below the horizon. This is the preferred earliest time to pray <em>mincha</em> according to the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Maimonides">Rambam</a> and others. For more information on <em>mincha ketana</em> see the
    /// documentation on <see cref="ZmanimCalendar.GetMinchaGedolaGRA"/>. This is calculated as 9.5 <see cref="GetShaahZmanis16Point1Degrees"/> after
    /// <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>mincha ketana</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis16Point1Degrees"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaKetanaGRA"/>
    public virtual DateTimeOffset? GetMinchaKetana16Point1Degrees()
    {
        return ToZone(GetMinchaKetana(GetAlos16Point1Degrees(), GetTzais16Point1Degrees(), true));
    }

    /// <summary>
    /// This method returns the time of <em>mincha ketana</em> based on the opinion of <a href=
    /// "https://en.wikipedia.org/wiki/Yaakov_Moshe_Hillel">Rabbi Yaakov Moshe Hillel</a> as published in the <em>luach</em> of the
    /// Bais Horaah of Yeshivat Chevrat Ahavat Shalom that <em>mincha ketana</em> is calculated as 2.5 <em>shaos zmaniyos</em> before
    /// <see cref="GetTzaisGeonim3Point8Degrees"/> with <em>shaos zmaniyos</em> calculated based on a day starting at <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/> and ending at <see cref="GetTzaisGeonim3Point8Degrees"/>. <em>Mincha ketana</em> is the preferred
    /// earliest time to pray <em>mincha</em> according to the opinion of the <a href="https://en.wikipedia.org/wiki/Maimonides"
    /// >Rambam</a> and others. For more information on <em>mincha ketana</em> see the documentation on <see cref="ZmanimCalendar.GetMinchaKetanaGRA"/>. If the calculated time is after sunset (on very short winter days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>mincha ketana</em>. If the calculation can't be computed such as
    ///         northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where the
    ///         sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point8Degrees"/>
    /// <seealso cref="GetMinchaGedolaAhavatShalom"/>
    /// <seealso cref="GetPlagAhavatShalom"/>
    public virtual DateTimeOffset? GetMinchaKetanaAhavatShalom()
    {
        TimeSpan? shaahZmanis = GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point8Degrees();
        if (shaahZmanis == null)
        {
            return null;
        }
        long totalNanos = (long)(-shaahZmanis.ToNanos() * 2.5);
        return ToZone(NullIfAfterSunset(GetTimeOffset(GetTzaisGeonim3Point8Degrees(), JavaTime.DurationOfNanos(totalNanos))));
    }

    /// <summary>
    /// This method returns the time of <em>mincha ketana</em> according to the Magen Avraham with the day starting 72 minutes before
    /// sunrise and ending 72 minutes after sunset. This is the preferred earliest time to pray <em>mincha</em> according to the
    /// opinion of the <a href="https://en.wikipedia.org/wiki/Maimonides">Rambam</a> and others. For more information on this see the
    /// documentation on <see cref="ZmanimCalendar.GetMinchaGedolaGRA"/>. This is calculated as 9.5 <see cref="ZmanimCalendar.GetShaahZmanis72Minutes"/> after
    /// <em>alos</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>mincha ketana</em>. If the calculation can't be computed such as
    ///         in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where it
    ///         does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis16Point1Degrees"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaKetanaGRA"/>
    public virtual DateTimeOffset? GetMinchaKetana72Minutes()
    {
        return ToZone(GetMinchaKetana(GetAlos72Minutes(), GetTzais72Minutes(), true));
    }

    /// <summary>
    /// This method returns the time of <em>plag hamincha</em> according to the Magen Avraham with the day starting 60
    /// minutes before sunrise and ending 60 minutes after sunset. This is calculated as 10.75 <see cref="GetShaahZmanis60Minutes"/>
    /// after <see cref="GetAlos60Minutes"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis60Minutes"/>
    /// <seealso cref="GetAlos60Minutes"/>
    /// <seealso cref="GetTzais60Minutes"/>
    public virtual DateTimeOffset? GetPlagHamincha60Minutes()
    {
        return ToZone(GetPlagHamincha(GetAlos60Minutes(), GetTzais60Minutes(), true));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns the time of <em>plag hamincha</em> according to the Magen
    /// Avraham with the day starting 72 minutes before sunrise and ending 72 minutes after sunset. This is calculated
    /// as 10.75 <see cref="ZmanimCalendar.GetShaahZmanis72Minutes"/> after <see cref="ZmanimCalendar.GetAlos72Minutes"/>. Since <em>plag</em> by this calculation can
    /// occur after sunset, it should only be used <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after <em>shkiah</em>),
    ///         and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this method
    ///         from the API, and this deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="ZmanimCalendar.GetShaahZmanis72Minutes"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagHamincha72Minutes()
    {
        return ToZone(GetPlagHamincha(GetAlos72Minutes(), GetTzais72Minutes(), true));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns the time of <em>plag hamincha</em> according to the Magen
    /// Avraham with the day starting 90 minutes before sunrise and ending 90 minutes after sunset. This is calculated as 10.75 *
    /// <see cref="GetShaahZmanis90Minutes"/> after <see cref="GetAlos90Minutes"/>. Since <em>plag</em> by this calculation can occur after
    /// sunset, it should only be used <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as
    ///         in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where it
    ///         does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after <em>shkiah</em>),
    ///         and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this method
    ///         from the API, and this deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetShaahZmanis90Minutes"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagHamincha90Minutes()
    {
        return ToZone(GetPlagHamincha(GetAlos90Minutes(), GetTzais90Minutes(), true));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns the time of <em>plag hamincha</em> according to the Magen
    /// Avraham with the day starting 96 minutes before sunrise and ending 96 minutes after sunset. This is calculated as 10.75
    /// <see cref="GetShaahZmanis96Minutes"/> after <see cref="GetAlos96Minutes"/>. Since <em>plag</em> by this calculation can occur after
    /// <em>shkiah</em>, it should only be used <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after <em>shkiah</em>),
    ///         and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this method
    ///         from the API, and this deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetShaahZmanis96Minutes"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagHamincha96Minutes()
    {
        return ToZone(GetPlagHamincha(GetAlos96Minutes(), GetTzais96Minutes(), true));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns the time of <em>plag hamincha</em>. This is calculated as
    /// 10.75 * <see cref="GetAlos96Zmanis"/> after <see cref="GetAlos96Zmanis">dawn</see>. Since <em>plag</em> by this calculation can occur
    /// after <em>shkiah</em>), it should only be used <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as
    ///         in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where it
    ///         does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after
    ///         <em>shkiah</em>), and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no
    ///         current plan to remove this method from the API, and this deprecation is intended to alert developers
    ///         of the danger of using it.
    /// </remarks>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagHamincha96MinutesZmanis()
    {
        return ToZone(GetPlagHamincha(GetAlos96Zmanis(), GetTzais96Zmanis(), true));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns the time of <em>plag hamincha</em>. This is calculated
    /// as 10.75 hours after <see cref="GetAlos90Zmanis">dawn</see>. The formula used is 10.75 * <see cref="GetShaahZmanis90MinutesZmanis"/> after <see cref="GetAlos90Zmanis">dawn</see>. Since <em>plag</em> by this calculation can
    /// occur after sunset, it should only be used <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as
    ///         in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where it
    ///         does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after
    ///         <em>shkiah</em>), and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no
    ///         current plan to remove this method from the API, and this deprecation is intended to alert developers
    ///         of the danger of using it.
    /// </remarks>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagHamincha90MinutesZmanis()
    {
        return ToZone(GetPlagHamincha(GetAlos90Zmanis(), GetTzais90Zmanis(), true));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns the time of <em>plag hamincha</em>. This is calculated as
    /// 10.75 hours after <see cref="GetAlos72Zmanis"/>. The formula used is 10.75 * <see cref="GetShaahZmanis72MinutesZmanis"/> after
    /// <see cref="GetAlos72Zmanis">dawn</see>. Since <em>plag</em> by this calculation can occur after sunset, it should only be used
    /// <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as
    ///         in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where it
    ///         does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after
    ///         <em>shkiah</em>), and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no
    ///         current plan to remove this method from the API, and this deprecation is intended to alert developers
    ///         of the danger of using it.
    /// </remarks>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagHamincha72MinutesZmanis()
    {
        return ToZone(GetPlagHamincha(GetAlos72Zmanis(), GetTzais72Zmanis(), true));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns the time of <em>plag hamincha</em> based on the opinion that the
    /// day starts at <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/> and ends at <see cref="GetTzais16Point1Degrees"/>. This is calculated as 10.75 *
    /// <see cref="GetShaahZmanis16Point1Degrees"/> after <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/>. Since <em>plag</em> by this calculation can
    /// occur after <em>shkiah</em>, it should only be used <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as
    ///         northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where
    ///         the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after <em>shkiah</em>),
    ///         and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this method
    ///         from the API, and this deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetShaahZmanis16Point1Degrees"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagHamincha16Point1Degrees()
    {
        return ToZone(GetPlagHamincha(GetAlos16Point1Degrees(), GetTzais16Point1Degrees(), true));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns the time of <em>plag hamincha</em> based on the opinion that the
    /// day starts at <see cref="GetAlos19Point8Degrees"/> and ends at <see cref="GetTzais19Point8Degrees"/>. This is calculated as 10.75 *
    /// <see cref="GetShaahZmanis19Point8Degrees"/> after <see cref="GetAlos19Point8Degrees">dawn</see>. Since <em>plag</em> by this calculation
    /// can occur after <em>shkiah</em>, it should only be used <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as
    ///         northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where
    ///         the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after
    ///         <em>shkiah</em>), and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no
    ///         current plan to remove this method from the API, and this deprecation is intended to alert developers
    ///         of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetShaahZmanis19Point8Degrees"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagHamincha19Point8Degrees()
    {
        return ToZone(GetPlagHamincha(GetAlos19Point8Degrees(), GetTzais19Point8Degrees(), true));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns the time of <em>plag hamincha</em> based on the
    /// opinion that the day starts at <see cref="GetAlos26Degrees"/> and ends at <see cref="GetTzais26Degrees"/>. This is calculated as
    /// 10.75 * <see cref="GetShaahZmanis26Degrees"/> after <see cref="GetAlos26Degrees"/>. Since the <em>zman</em> based on an extremely
    /// early <em>alos</em> and a very late <em>tzais</em>, it should only be used <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may not reach low
    ///         enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after <em>shkiah</em>),
    ///         and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this method
    ///         from the API, and this deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetShaahZmanis26Degrees"/>
    /// <seealso cref="GetPlagHamincha120Minutes"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagHamincha26Degrees()
    {
        return ToZone(GetPlagHamincha(GetAlos26Degrees(), GetTzais26Degrees(), true));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns the time of <em>plag hamincha</em> based on the opinion that the
    /// day starts at <see cref="GetAlos18Degrees"/> and ends at <see cref="GetTzais18Degrees"/>. This is calculated as 10.75* <see cref="GetShaahZmanis18Degrees"/> after <see cref="GetAlos18Degrees"/>. Since <em>plag</em> by this calculation can occur after sunset,
    /// it should only be used <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as
    ///         northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where
    ///         the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after
    ///         <em>shkiah</em>), and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no
    ///         current plan to remove this method from the API, and this deprecation is intended to alert developers
    ///         of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetShaahZmanis18Degrees"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagHamincha18Degrees()
    {
        return ToZone(GetPlagHamincha(GetAlos18Degrees(), GetTzais18Degrees(), true));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns the time of <em>plag hamincha</em> based on the opinion that the
    /// day starts at <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/> and ends at <see cref="ZmanimCalendar.GetSunsetBasedOnElevationSetting"/>. This time is 10.75 *
    /// <em>shaos zmaniyos</em> after <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/>. Since <em>plag</em> by this calculation can occur after
    /// <em>shkiah</em>, it should only be used <em>lechumra</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the <em>plag</em>. If the calculation can't be computed such as northern and southern locations
    ///         even south of the Arctic Circle and north of the Antarctic Circle where the sun may not reach low enough below the
    ///         horizon for this calculation, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time (often after <em>shkiah</em>),
    ///         and if used <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this method
    ///         from the API, and this deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="ZmanimCalendar.GetAlos16Point1Degrees"/>
    /// <seealso cref="AstronomicalCalendar.GetSeaLevelSunset"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time (often after shkiah), and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetPlagAlosToSunset()
    {
        return ToZone(GetPlagHamincha(GetAlos16Point1Degrees(), GetSunsetBasedOnElevationSetting(), false));
    }

    /// <summary>
    /// This method returns the time of <em>plag hamincha</em> based on the opinion that the day starts at <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/> and ends at <see cref="GetTzaisGeonim7Point083Degrees"/>. The time is 10.75 * <see cref="GetShaahZmanisAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/> after <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/> to reach this time.
    /// Sources for an asymmetrical day-based calculation can be seen in the documentation of
    /// <see cref="GetSofZmanShmaAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the <em>plag</em>. If the calculation can't be computed such as northern and southern locations
    ///         even south of the Arctic Circle and north of the Antarctic Circle where the sun may not reach low enough below the
    ///         horizon for this calculation, a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetAlos16Point1Degrees"/>
    /// <seealso cref="GetTzaisGeonim7Point083Degrees"/>
    /// <seealso cref="GetSofZmanShmaAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>
    public virtual DateTimeOffset? GetPlagAlos16Point1DegreesToTzaisGeonim7Point083Degrees()
    {
        return ToZone(GetPlagHamincha(GetAlos16Point1Degrees(), GetTzaisGeonim7Point083Degrees(), false));
    }

    /// <summary>
    /// This method returns the time of <em>plag hamincha</em> (the earliest time that Shabbos can be started) based on the opinion of
    /// <a href="https://en.wikipedia.org/wiki/Yaakov_Moshe_Hillel">Rabbi Yaakov Moshe Hillel</a> as published in the <em>luach</em>
    /// of the Bais Horaah of Yeshivat Chevrat Ahavat Shalom that that <em>plag hamincha</em> is calculated as 1.25 * <see cref="GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point8Degrees"/> before <see cref="GetTzaisGeonim3Point8Degrees"/>. Sources for an
    /// asymmetrical day-based calculation can be seen in the documentation of
    /// <see cref="GetSofZmanShmaAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>. If the calculated time is after sunset (on very short winter days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the <em>plag</em>. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may not
    ///         reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point8Degrees"/>
    /// <seealso cref="GetMinchaGedolaAhavatShalom"/>
    /// <seealso cref="GetMinchaKetanaAhavatShalom"/>
    public virtual DateTimeOffset? GetPlagAhavatShalom()
    {
        TimeSpan? shaahZmanis = GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point8Degrees();
        if (shaahZmanis == null)
        {
            return null;
        }
        long totalNanos = (long)(-shaahZmanis.ToNanos() * 1.25);
        return ToZone(NullIfAfterSunset(GetTimeOffset(GetTzaisGeonim3Point8Degrees(), JavaTime.DurationOfNanos(totalNanos))));
    }

    /// <summary>
    /// Method to return the beginning of <em>bain hashmashos</em> of Rabbeinu Tam calculated when the sun is
    /// <see cref="Zenith13Point24">13.24°</see> below the western <see cref="AstronomicalCalendar.GeometricZenith">geometric horizon</see> (90°)
    /// after sunset. This calculation is based on the same calculation of <see cref="GetBainHashmashosRT58Point5Minutes"><em>bain hashmashos</em> Rabbeinu Tam 58.5 minutes</see> but uses a degree-based calculation instead of 58.5 exact
    /// minutes. This calculation is based on the position of the sun 58.5 minutes after sunset in Jerusalem <a href=
    /// "https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a>,
    /// which calculates to 13.24° below <see cref="AstronomicalCalendar.GeometricZenith"/>.
    /// NOTE: As per Yisrael Vehazmanim Vol. III page 1025, No. 50, a dip of slightly less than 13° should be used.
    /// Calculations show that the proper dip to be 13.2456° (truncated to 13.24° that provides about 1.5 second
    /// earlier (<em>lechumra</em>) time) below the horizon at that time. This makes a difference of 1 minute and 10
    /// seconds in Jerusalem during the Equinox, and 1 minute 29 seconds during the solstice as compared to the proper
    /// 13.24° versus 13°. For NY during the solstice, the difference is 1 minute 56 seconds.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the sun being 13.24° below <see cref="AstronomicalCalendar.GeometricZenith"/>
    ///         (90°). If the calculation can't be computed such as northern and southern locations even south of the
    ///         Arctic Circle and north of the Antarctic Circle where the sun may not reach low enough below the horizon
    ///         for this calculation, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>To do:</b> recalculate the above based on equilux/equinox calculations.
    /// </remarks>
    /// <seealso cref="GetBainHashmashosRT58Point5Minutes"/>
    public virtual DateTimeOffset? GetBainHashmashosRT13Point24Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith13Point24));
    }

    /// <summary>
    /// This method returns the beginning of <em>Bain hashmashos</em> of Rabbeinu Tam calculated as a 58.5-minute offset after sunset.
    /// <em>bain hashmashos</em> is 3/4 of a <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a> before <em>tzais</em> or 3
    /// 1/4 mil after sunset. With a mil calculated as 18 minutes, 3.25 * 18 = 58.5 minutes.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of 58.5 minutes after sunset. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year when the sun does not rise, and one where it does
    ///         not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual DateTimeOffset? GetBainHashmashosRT58Point5Minutes()
    {
        long totalNanos = (long)(58.5 * MinuteNanos);
        return ToZone(GetTimeOffset(GetSunsetBasedOnElevationSetting(), JavaTime.DurationOfNanos(totalNanos)));
    }

    /// <summary>
    /// This method returns the beginning of <em>bain hashmashos</em> based on the calculation of 13.5 minutes (3/4 of an 18-minute
    /// <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a>) before <em>shkiah</em> calculated as
    /// <see cref="GetTzaisGeonim7Point083Degrees"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the <em>bain hashmashos</em> of Rabbeinu Tam in this calculation. If the calculation can't be
    ///         computed such as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where
    ///         the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetTzaisGeonim7Point083Degrees"/>
    public virtual DateTimeOffset? GetBainHashmashosRT13Point5MinutesBefore7Point083Degrees()
    {
        long totalNanos = (long)(-13.5 * MinuteNanos);
        return ToZone(GetTimeOffset(GetTzaisGeonim7Point083Degrees(), JavaTime.DurationOfNanos(totalNanos)));
    }

    /// <summary>
    /// This method returns the beginning of <em>bain hashmashos</em> of Rabbeinu Tam calculated according to the
    /// opinion of the <em>Divrei Yosef</em> (see Yisrael Vehazmanim) calculated 5/18th (27.77%) of the time between
    /// <em>alos</em> (calculated as 19.8° before sunrise) and sunrise. This is added to sunset to arrive at the time
    /// for <em>bain hashmashos</em> of Rabbeinu Tam.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of <em>bain hashmashos</em> of Rabbeinu Tam for this calculation. If the calculation can't be
    ///         computed such as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where
    ///         the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual DateTimeOffset? GetBainHashmashosRT2Stars()
    {
        DateTimeOffset? alos19Point8 = GetAlos19Point8Degrees();
        DateTimeOffset? sunrise = GetSunriseBasedOnElevationSetting();
        if (alos19Point8 == null || sunrise == null)
        {
            return null;
        }
        TimeSpan? alosToSunrise = JavaTime.Between(alos19Point8, sunrise);
        long totalNanos = (long)(alosToSunrise.ToNanos() * (5 / 18d));
        return ToZone(GetTimeOffset(GetSunsetBasedOnElevationSetting(), JavaTime.DurationOfNanos(totalNanos)));
    }

    /// <summary>
    /// This method returns the beginning of <em>bain hashmashos</em> (twilight) according to the <a href=
    /// "https://en.wikipedia.org/wiki/Eliezer_ben_Samuel">Yereim (Rabbi Eliezer of Metz)</a> calculated as 18 minutes or 3/4 of a
    /// 24-minute <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a> before sunset. According to the Yereim, <em>bain
    /// hashmashos</em> starts 3/4 of a mil before sunset and <em>tzais</em> or nightfall starts at sunset. If the calculated time is before <see cref="ZmanimCalendar.GetPlagHaminchaGRA"><em>plag hamincha</em></see>
    /// (on very short winter days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of 18 minutes before sunset. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year when the sun does not rise, and one where it does
    ///         not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetBainHashmashosYereim3Point05Degrees"/>
    public virtual DateTimeOffset? GetBainHashmashosYereim18Minutes()
    {
        return ToZone(NullIfBeforePlagHaminchaGRA(GetTimeOffset(GetSunsetBasedOnElevationSetting(), JavaTime.OfMinutes(-18))));
    }

    /// <summary>
    /// This method returns the beginning of <em>bain hashmashos</em> (twilight) according to the <a href=
    /// "https://en.wikipedia.org/wiki/Eliezer_ben_Samuel">Yereim (Rabbi Eliezer of Metz)</a> calculated as the sun's
    /// position 3.05° above the horizon <a href=
    /// "https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a>,
    /// its position 18 minutes or 3/4 of an 24-minute <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a> before sunset.
    /// According to the Yereim, <em>bain hashmashos</em> starts 3/4 of a mil before sunset and <em>tzais</em> or nightfall starts at
    /// sunset. Note that <em>lechumra</em> (of about 14 seconds) a refraction value of 0.5166° as opposed to the traditional 0.566°
    /// is used. This is more inline with the actual refraction in <em>Eretz Yisrael</em> and is brought down by <a href=
    /// "http://beinenu.com/rabbis/%D7%94%D7%A8%D7%91-%D7%99%D7%93%D7%99%D7%93%D7%99%D7%94-%D7%9E%D7%A0%D7%AA">Rabbi Yedidya Manet</a>
    /// in his <a href="https://www.nli.org.il/en/books/NNL_ALEPH002542826/NLI">Zmanei HaHalacha Lema'aseh</a> (p. 11). That is the
    /// first source that I am aware of that calculates degree-based Yereim <em>zmanim</em>. The 0.5166° refraction is also used by
    /// the <a href="https://zmanim.online/">Luach Itim Lebinah</a>. Calculating the Yereim's <em>bain hashmashos</em> using 18-minute
    /// based degrees is also suggested in the upcoming 8th edition of the zmanim Kehilchasam. For more details, see the article
    /// <a href="https://kosherjava.com/2020/12/07/the-yereims-bein-hashmashos/">The Yereim's <em>Bain Hashmashos</em></a>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the sun's position 3.05° minutes before sunset. If the calculation can't
    ///         be computed such as in the Arctic Circle where there is at least one day a year when the sun does not
    ///         rise, and one where it does not set, a <c>null</c> will be returned. See detailed explanation on
    ///         top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>To do:</b> recalculate based on equinox/equilux
    /// </remarks>
    /// <seealso cref="ZenithMinus3Point05"/>
    /// <seealso cref="GetBainHashmashosYereim18Minutes"/>
    /// <seealso cref="GetBainHashmashosYereim2Point8Degrees"/>
    /// <seealso cref="GetBainHashmashosYereim2Point1Degrees"/>
    public virtual DateTimeOffset? GetBainHashmashosYereim3Point05Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(ZenithMinus3Point05));
    }

    /// <summary>
    /// This method returns the beginning of <em>bain hashmashos</em> (twilight) according to the <a href=
    /// "https://en.wikipedia.org/wiki/Eliezer_ben_Samuel">Yereim (Rabbi Eliezer of Metz)</a> calculated as 16.875 minutes or 3/4 of a
    /// 22.5-minute <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a> before sunset. According to the Yereim, <em>bain
    /// hashmashos</em> starts 3/4 of a mil before sunset and <em>tzais</em> or nightfall starts at sunset. If the calculated time is before <see cref="ZmanimCalendar.GetPlagHaminchaGRA"><em>plag hamincha</em></see>
    /// (on very short winter days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of 16.875 minutes before sunset. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year when the sun does not rise, and one where it does
    ///         not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetBainHashmashosYereim2Point8Degrees"/>
    public virtual DateTimeOffset? GetBainHashmashosYereim16Point875Minutes()
    {
        long totalNanos = (long)(-16.875 * MinuteNanos);
        return ToZone(NullIfBeforePlagHaminchaGRA(GetTimeOffset(GetSunsetBasedOnElevationSetting(), JavaTime.DurationOfNanos(totalNanos))));
    }

    /// <summary>
    /// This method returns the beginning of <em>bain hashmashos</em> (twilight) according to the <a href=
    /// "https://en.wikipedia.org/wiki/Eliezer_ben_Samuel">Yereim (Rabbi Eliezer of Metz)</a> calculated as the sun's position 2.8°
    /// above the horizon <a href="https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox /
    /// equilux</a>, its position 16.875 minutes or 3/4 of an 22.5-minute <a href="https://en.wikipedia.org/wiki/Biblical_mile"
    /// >mil</a> before sunset. According to the Yereim, <em>bain hashmashos</em> starts 3/4 of a mil before sunset and <em>tzais</em>
    /// or nightfall starts at sunset. Details, including how the degrees were calculated can be seen in the documentation of
    /// <see cref="GetBainHashmashosYereim3Point05Degrees"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the sun's position 2.8° minutes before sunset. If the calculation can't
    ///         be computed such as in the Arctic Circle where there is at least one day a year when the sun does not
    ///         rise, and one where it does not set, a <c>null</c> will be returned. See detailed explanation on
    ///         top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZenithMinus2Point8"/>
    /// <seealso cref="GetBainHashmashosYereim16Point875Minutes"/>
    /// <seealso cref="GetBainHashmashosYereim3Point05Degrees"/>
    /// <seealso cref="GetBainHashmashosYereim2Point1Degrees"/>
    public virtual DateTimeOffset? GetBainHashmashosYereim2Point8Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(ZenithMinus2Point8));
    }

    /// <summary>
    /// This method returns the beginning of <em>bain hashmashos</em> (twilight) according to the <a href=
    /// "https://en.wikipedia.org/wiki/Eliezer_ben_Samuel">Yereim (Rabbi Eliezer of Metz)</a> calculated as 13.5 minutes or 3/4 of
    /// an 18-minute <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a> before sunset. According to the Yereim, <em>bain
    /// hashmashos</em> starts 3/4 of a mil before sunset and <em>tzais</em> or nightfall starts at sunset. If the calculated time is before <see cref="ZmanimCalendar.GetPlagHaminchaGRA"><em>plag hamincha</em></see>
    /// (on very short winter days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of 13.5 minutes before sunset. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year when the sun does not rise, and one where it does
    ///         not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetBainHashmashosYereim2Point1Degrees"/>
    public virtual DateTimeOffset? GetBainHashmashosYereim13Point5Minutes()
    {
        long totalNanos = (long)(-13.5 * MinuteNanos);
        return ToZone(NullIfBeforePlagHaminchaGRA(GetTimeOffset(GetSunsetBasedOnElevationSetting(), JavaTime.DurationOfNanos(totalNanos))));
    }

    /// <summary>
    /// Returns the time passed in, or <c>null</c> if it is after <see cref="ZmanimCalendar.GetSunsetBasedOnElevationSetting">sunset</see>, as can happen
    /// on very short winter days for <em>zmanim</em> calculated from a day that extends past sunset.
    /// </summary>
    /// <param name="zman">the time to check</param>
    /// <returns>the time passed in, or <c>null</c> if it is <c>null</c> or after sunset</returns>
    private DateTimeOffset? NullIfAfterSunset(DateTimeOffset? zman)
    {
        DateTimeOffset? sunset = GetSunsetBasedOnElevationSetting();
        if (zman != null && sunset != null && zman.IsAfter(sunset))
        { // very short winter day
            return null;
        }
        return zman;
    }

    /// <summary>
    /// Returns the time passed in, or <c>null</c> if it is before <see cref="ZmanimCalendar.GetPlagHaminchaGRA"><em>plag hamincha</em></see>, as can
    /// happen on very short winter days when a fixed offset before sunset reaches back into the morning.
    /// </summary>
    /// <param name="zman">the time to check</param>
    /// <returns>the time passed in, or <c>null</c> if it is <c>null</c> or before <em>plag hamincha</em></returns>
    private DateTimeOffset? NullIfBeforePlagHaminchaGRA(DateTimeOffset? zman)
    {
        DateTimeOffset? plagHamincha = GetPlagHaminchaGRA();
        if (zman != null && plagHamincha != null && zman.IsBefore(plagHamincha))
        { // very short winter day
            return null;
        }
        return zman;
    }

    /// <summary>
    /// This method returns the beginning of <em>bain hashmashos</em> according to the <a href=
    /// "https://en.wikipedia.org/wiki/Eliezer_ben_Samuel">Yereim (Rabbi Eliezer of Metz)</a> calculated as the sun's position 2.1°
    /// <b>above</b> the horizon <a href="https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the
    /// equinox / equilux</a> in Yerushalayim, its position 13.5 minutes or 3/4 of an 18-minute <a href=
    /// "https://en.wikipedia.org/wiki/Biblical_mile">mil</a> before sunset. According to the Yereim, <em>bain hashmashos</em> starts
    /// 3/4 of a mil before sunset and <em>tzais</em> or nightfall starts at sunset. Details, including how the degrees were
    /// calculated can be seen in the documentation of <see cref="GetBainHashmashosYereim3Point05Degrees"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the sun's position 2.1° minutes before sunset. If the calculation can't be computed such as i
    ///         the Arctic Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZenithMinus2Point1"/>
    /// <seealso cref="GetBainHashmashosYereim13Point5Minutes"/>
    /// <seealso cref="GetBainHashmashosYereim2Point8Degrees"/>
    /// <seealso cref="GetBainHashmashosYereim3Point05Degrees"/>
    public virtual DateTimeOffset? GetBainHashmashosYereim2Point1Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(ZenithMinus2Point1));
    }

    /// <summary>
    /// This method returns the <em>tzais</em> (nightfall) based on the opinion of the <em>Geonim</em> calculated at the sun's position
    /// at <see cref="Zenith3Point7">3.7°</see> below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°), calculated as the position of the
    /// sun 13.5 minutes after sunset, the time it takes to walk 3/4 of a <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a>
    /// at 18 minutes a mil, or 13.5 minutes after sunset. The sun is 3.7° below <see cref="AstronomicalCalendar.GeometricZenith"/> at this
    /// time in Jerusalem <a href="https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox /
    /// equilux</a>. This does not cover the 26.46 it takes to walk 49 amos (the <em>heref ayin</em> of <em>bain hashmashos</em> of
    /// Rav Yosi) at the pace of an 18-minute mil. Rabbi Yedidya Manet in his <a href=
    /// "https://www.nli.org.il/en/books/NNL_ALEPH002542826/NLI">Zmanei HaHalacha Lema'aseh</a> (5th edition, vol. I page 36, no. 22)
    /// lists 3.65° as the value for this <em>tzais</em>, however it is based on sunset calculated using a refreaction value of 31′
    /// (arcminutes) or 0.516° (page 16) based on his calculations of the refraction value in Israel at the autumn equinox (something
    /// he himself questions in vol II, page 36). Even for Israel, most consider that refraction value too low. Regardless, the dip for
    /// <em>tzais</em> must be based on sunset using a specific refraction model (and solar radius), and this library currently uses a
    /// global average refraction value of 34′ (arcminutes) or 0.566°. Based on the global average refraction for sunset, this
    /// <em>tzais</em> calculates as 3.7°.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time when the sun is 3.7° below sea level.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very early time, and if used <em>lekula</em>
    ///         can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this method from the API, and this
    ///         deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="Zenith3Point7"/>
    [Obsolete("This method should be used lechumra only since it returns a very early time, and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetTzaisGeonim3Point7Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith3Point7));
    }

    /// <summary>
    /// This method returns the <em>tzais</em> (nightfall) based on the opinion of the <em>Geonim</em> calculated at the sun's position
    /// at <see cref="Zenith3Point7">3.8°</see> below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°), calculated as the position of the
    /// sun 13.5 minutes after sunset, the time it takes to walk 3/4 of a <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a>
    /// at 18 minutes a mil, plus 30 seconds for the time it takes to walk 49 amos (the <em>heref ayin</em> of <em>bain hashmashos</em>
    /// of Rav Yosi). With this being an 18-minutes mil, 49 amos would take 26.46, rounded to 30 seconds), for a total of 14 minutes
    /// after sunset. The sun is <see cref="Zenith3Point8">3.8°</see> below <see cref="AstronomicalCalendar.GeometricZenith"/> at this time in
    /// Jerusalem <a href="https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time when the sun is 3.8° below sea level.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very early time, and if used <em>lekula</em>
    ///         can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this method from the API, and this
    ///         deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="Zenith3Point8"/>
    [Obsolete("This method should be used lechumra only since it returns a very early time, and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetTzaisGeonim3Point8Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith3Point8));
    }

    /// <summary>
    /// This method returns the <em>tzais</em> (nightfall) based on the opinion of the <em>Geonim</em> calculated at the sun's
    /// position at <see cref="Zenith5Point95">5.95°</see> below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°), calculated as the
    /// position of the sun 24 minutes after sunset in Jerusalem <a href=
    /// "https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a>. The 24 minutes
    /// is based on the Baal Hatanya's calculation of 18 minutes (3/4 of a 24 minute mil) + 4 minutes for <em>shkiah amitis</em> +
    /// 2 minutes for bain hashmashos of Rav Yosi. See Hazmanim Bahalacha vol II, ch. 50, no. 5, p. 512-513, ch. 47, and Yisrael
    /// Vehazmanim <a href="https://hebrewbooks.org/pdfpager.aspx?req=9764&amp;st=&amp;pgnum=266">Vol III, ch. 13, no. 53, p. 1026</a>. Among
    /// sources he mentions for this <em>zman</em> is <a href="https://en.wikipedia.org/wiki/Yehuda_(Leo)_Levi">Rabbi Yehuda (Leo)
    /// Levi's</a> calculations in Jewish Chrononomy and other sources. Calculations show that the time is closer to 5.93° and was
    /// seemingly rounded to 5.95°. Chabad calendars usually use the 6°-based <see cref="GetTzaisBaalHatanya"/> that is built on this
    /// same calculation. Rabbi Yedidya Manet in his <a href="https://www.nli.org.il/en/books/NNL_ALEPH002542826/NLI">Zmanei HaHalacha
    /// Lema'aseh</a> (5th edition, vol. I page 36, no. 24) lists 5.88° as the value for this <em>tzais</em>, however it is based on
    /// sunset calculated using a refreaction value of 31′ (arcminutes) or 0.516° (page 16) based on his calculations of the refraction
    /// value in Israel at the autumn equinox (something he himself questions in vol II, page 36). Even for Israel, most consider that
    /// refraction value too low. Regardless, the dip for <em>tzais</em> must be based on sunset using a specific refraction model
    /// (and solar radius), and this library currently uses a global average refraction value of 34′ (arcminutes) or 0.566°. Based on
    /// the global average refraction for sunset, this <em>tzais</em> calculates as 5.93° (rounded by most calendars to 5.95°).
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time when the sun is 5.95° below sea level. If the calculation can't be computed
    ///         such as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun
    ///         may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetTzaisBaalHatanya"/>
    public virtual DateTimeOffset? GetTzaisGeonim5Point95Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith5Point95));
    }

    /// <summary>
    /// This method returns the <em>tzais</em> (nightfall) based on the opinion of the <em>Geonim</em> calculated as 3/4 of a <a href=
    /// "https://en.wikipedia.org/wiki/Biblical_mile">mil</a> based on a 24-minute mil, or 18 minutes. It is the sun's position at
    /// <see cref="Zenith4Point66">4.66°</see> below the western horizon. This is a very early <em>zman</em> and should not be relied on
    /// without Rabbinical guidance. This does not cover the 35.28 seconds it takes to walk 49 amos (the <em>heref ayin</em> of
    /// <em>bain hashmashos</em> of Rav Yosi) at the pace of a 24-minute mil. See <see cref="GetTzaisGeonim4Point8Degrees"/> for a time
    /// that covers the <em>heref ayin</em>. Rabbi Yedidya Manet in his <a href=
    /// "https://www.nli.org.il/en/books/NNL_ALEPH002542826/NLI">Zmanei HaHalacha Lema'aseh</a> (5th edition, vol. I page 36, no. 24)
    /// lists 4.61° as the value for this <em>tzais</em>, however it is based on sunset calculated using a refreaction value of 31′
    /// (arcminutes) or 0.516° (page 16) based on his calculations of the refraction value in Israel at the autumn equinox (something
    /// he himself questions in vol II, page 36). Even for Israel, most consider that refraction value too low. Regardless, the dip for
    /// <em>tzais</em> must be based on sunset using a specific refraction model (and solar radius), and this library currently uses a
    /// global average refraction value of 34′ (arcminutes) or 0.566°. Based on the global average refraction for sunset, this
    /// <em>tzais</em> calculates as 4.66°.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time when the sun is 4.66° below sea level. If the calculation can't be computed
    ///         such as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where the
    ///         sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very early time, and if used <em>lekula</em>
    ///         can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this method from the API, and this
    ///         deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="Zenith4Point66"/>
    /// <seealso cref="GetTzaisGeonim4Point8Degrees"/>
    [Obsolete("This method should be used lechumra only since it returns a very early time, and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetTzaisGeonim4Point66Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith4Point66));
    }

    /// <summary>
    /// This method returns the <em>tzais</em> (nightfall) based on the opinion of the <em>Geonim</em> calculated as 3/4 of a <a href=
    /// "https://en.wikipedia.org/wiki/Biblical_mile">mil</a>, based on a 22.5-minute mil, or 16.875 minutes. It is the sun's position
    /// at <see cref="Zenith4Point42">4.42°</see> below the western horizon. This is a very early <em>zman</em> and should not be relied on
    /// without Rabbinical guidance. This does not cover the 33.07 seconds it takes to walk 49 amos (the <em>heref ayin</em> of
    /// <em>bain hashmashos</em> of Rav Yosi) at the pace of a 22.5 minute-mil. Rabbi Yedidya Manet in his <a href=
    /// "https://www.nli.org.il/en/books/NNL_ALEPH002542826/NLI">Zmanei HaHalacha Lema'aseh</a> (5th edition, vol. I page 36, no. 23)
    /// lists 4.37° as the value for this <em>tzais</em>, however it is based on sunset calculated using a refreaction value of 31′
    /// (arcminutes) or 0.516° (page 16) based on his calculations of the refraction value in Israel at the autumn equinox (something
    /// he himself questions in vol II, page 36). Even for Israel, most consider that refraction value too low. Regardless, the dip for
    /// <em>tzais</em> must be based on sunset using a specific refraction model (and solar radius), and this library currently uses a
    /// global average refraction value of 34′ (arcminutes) or 0.566°. Based on the global average refraction for sunset, this
    /// <em>tzais</em> calculates as 4.42°.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time when the sun is 4.42° below sea level. If the calculation
    ///         can't be computed such as northern and southern locations even south of the Arctic Circle and north of
    ///         the Antarctic Circle where the sun may not reach low enough below the horizon for this calculation, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very early time, and if used <em>lekula</em>
    ///         can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this method from the API, and this
    ///         deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="Zenith4Point42"/>
    [Obsolete("This method should be used lechumra only since it returns a very early time, and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetTzaisGeonim4Point42Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith4Point42));
    }

    /// <summary>
    /// This method returns the <em>tzais</em> (nightfall) based on the opinion of the <em>Geonim</em> calculated as the sun's
    /// position below the horizon at a time of 18.6 minutes after sunset. This is calculated as 3/4 of a 24-minute <a href=
    /// "https://en.wikipedia.org/wiki/Biblical_mile">mil</a>, plus 0.6 minutes for the time to walk 49 amos for <em>bain
    /// hashmashos</em> of Rav Yosi (with this <em>zman</em> based on a 24-minute mil, 49 amos would take 35.28 seconds to walk), for
    /// a total of 18.6 minutes after sunset. This calculates to the sun's position at <see cref="Zenith4Point8">4.8°</see> below the
    /// western horizon. This is based on <a href=
    /// "https://he.wikipedia.org/wiki/%D7%99%D7%97%D7%99%D7%90%D7%9C_%D7%9E%D7%99%D7%9B%D7%9C_%D7%A9%D7%9C%D7%96%D7%99%D7%A0%D7%92%D7%A8"
    /// >Rav Yechiel Michel Shlezinger's</a> <em>sefer</em> <a href="https://www.nli.org.il/he/books/NNL_ALEPH997010042055805171/NLI"
    /// >Aizehu Bain Hashmashos</a>, <a href="https://en.wikipedia.org/wiki/Yehuda_(Leo)_Levi">Rabbi Yehuda (Leo) Levi's</a>
    /// calculations in <a href="https://www.nli.org.il/en/items/NNL_ALEPH990022548970205171/NLI">Zmanei Hayom BaHalacha</a> p. 37.
    /// At this point, 3 medium sized stars are visible to a non-expert with good vision with effort. An expert knowing where to
    /// look can see the 3 medium stars as early as 15 minutes after sunset. This is explained in detail in Hazmanim Bahalacha vol
    /// II, ch. 41, no. 6 (p. 372-373, ch. 47, no. 11-12 (p. 491-493) where it is clear that medium sized stars would be visible as
    /// early as 14 minutes after sunset (13.5 minutes for 3/4 of an 18 minute Mil, plus 0.5 minutes for <em>bain Hashmashos</em>
    /// of Rav Yosi) to an expert. See more details on this earlier <em>zman</em> at <see cref="GetTzaisGeonim3Point8Degrees"/>.
    /// This is an early <em>zman</em> for <em>tzais</em>and should not be relied on without Rabbinical guidance.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time when the sun is 4.8° below sea level. If the calculation can't be computed
    ///         such as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle where the
    ///         sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="Zenith4Point8"/>
    public virtual DateTimeOffset? GetTzaisGeonim4Point8Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith4Point8));
    }

    /// <summary>
    /// This method returns the <em>tzais</em> (nightfall) based on the opinion of the <em>Geonim</em> as calculated by <a href=
    /// "https://en.wikipedia.org/wiki/Yechiel_Michel_Tucazinsky">Rabbi Yechiel Michel Tucazinsky</a> as the position of
    /// the sun no later than 31 minutes after sea-level sunset in Jerusalem (the Birur halacha shows that Rav Tucazinsky's
    /// calculations for sunset, listed as 28 minutes in this case, were about 3 minutes later than reality), and at the height of
    /// the summer solstice, this <em>zman</em>, calculations show that 30.75 minutes after <em>shkiah</em> computes to 6.45° below
    /// <see cref="AstronomicalCalendar.GeometricZenith"/>. This calculation is found in the <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=50536&amp;st=&amp;pgnum=51">Birur Halacha Yoreh Deah 262</a> and it is the commonly used
    /// <em>zman</em> in Israel. It is also used in the <a href="https://www.worldcat.org/oclc/243303103">Luach Itim Lebinah</a>. it
    /// should be noted that this differs from the 6.1° / 6.2° calculation for Rabbi Tucazinsky's time as calculated by the
    /// Hazmanim Bahalacha Vol II chapter 50:7 (page 515). Calculations show that 6.45° at the equinox is 26.5 minutes after
    /// <em>shkiah</em> <a href="https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox /
    /// equilux</a>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time when the sun is 6.45° below sea level. If the
    ///         calculation can't be computed such as northern and southern locations even south of the Arctic Circle and
    ///         north of the Antarctic Circle where the sun may not reach low enough below the horizon for this
    ///         calculation, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="Zenith6Point45"/>
    public virtual DateTimeOffset? GetTzaisGeonim6Point45Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith6Point45));
    }

    /// <summary>
    /// This method returns the <em>tzais</em> (nightfall) based on the opinion of the <em>Geonim</em> calculated when the sun's
    /// position <see cref="Zenith7Point083">7.083° (or 7° 5′</see>) below the western horizon. This is often referred to as 7° 5′ or 7° and
    /// 5′ minutes. This calculation is based on the observation of 3 medium-sized stars by Dr. Baruch (Berthold) Cohn in his
    /// <em>luach</em> <a href="https://sammlungen.ub.uni-frankfurt.de/freimann/content/titleinfo/983088">Tabellen enthaltend die
    /// Zeitangaben für den Beginn der Nacht und des Tages für die Breitengrade + 66 bis -38</a> published in Strasbourg, France in
    /// 1899. This calendar was very popular in Europe, and many other calendars based their <em>tzais</em> time on it. <a href=
    /// "https://en.wikipedia.org/wiki/David_Zvi_Hoffmann">Rav Dovid Tzvi Hoffman</a> in his <a href="https://hebrewbooks.org/1053"
    /// >שו״ת מלמד להועיל</a> in an exchange of letters with Baruch Cohn in <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=1053&amp;st=&amp;pgnum=37">Orach Chaim 30</a> agreed to this <em>zman</em> (page 36), as
    /// did the שו״ת בני ציון and the <a href="https://hebrewbooks.org/67373">תנובת שדה</a>. It is very close to the time of the <a href=
    /// "https://hebrewbooks.org/22044">מקור חסד</a> of the Sefer chasidim. It is close to the position of the sun 30 minutes after
    /// sunset in Jerusalem <a href="https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox /
    /// equilux</a>, but not Exactly. The actual position of the sun 30 minutes after sunset in Jerusalem at the equilux is 7.205°
    /// and 7.199° at the equinox. See Hazmanim Bahalacha vol 2, pages 520-521 for more details.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time when the sun is 7.083° below sea level. If the calculation can't be
    ///         computed such as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="Zenith7Point083"/>
    public virtual DateTimeOffset? GetTzaisGeonim7Point083Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith7Point083));
    }

    /// <summary>
    /// This method returns <em>tzais</em> (nightfall) based on the opinion of the <em>Geonim</em> calculated as 45 minutes
    /// after sunset during the summer solstice in New York, when the <em>neshef</em> (twilight) is the longest. The sun's
    /// position at this time computes to <see cref="Zenith7Point67">7.67°</see> below the western horizon. See <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=921&amp;pgnum=149">Igros Moshe Even Haezer 4, Ch. 4</a> (regarding
    /// <em>tzais</em> for <em>krias Shema</em>). It is also mentioned in Rabbi Heber's <a href=
    /// "https://hebrewbooks.org/53000">Shaarei Zmanim</a> on in
    /// <a href="https://hebrewbooks.org/pdfpager.aspx?req=53055&amp;pgnum=101">chapter 10 (page 87)</a> and
    /// <a href="https://hebrewbooks.org/pdfpager.aspx?req=53055&amp;pgnum=122">chapter 12 (page 108)</a>. Also see the
    /// time of 45 minutes in <a href="https://en.wikipedia.org/wiki/Simcha_Bunim_Cohen">Rabbi Simcha Bunim Cohen's</a> <a
    /// href="https://www.worldcat.org/oclc/179728985">The radiance of Shabbos</a> as the earliest <em>zman</em> for New York.
    /// This <em>zman</em> is also listed in the <a href="https://hebrewbooks.org/pdfpager.aspx?req=1927&amp;pgnum=90">Divrei
    /// Shalom Vol. III, chapter 75</a>, and <a href="https://hebrewbooks.org/pdfpager.aspx?req=892&amp;pgnum=431">Bais Av"i
    /// Vol. III, chapter 117</a>. This <em>zman</em> is also listed in the Divrei Shalom etc. chapter 177. Since this
    /// <em>zman</em> depends on the level of light, Rabbi Yaakov Shakow presented this degree-based calculation to Rabbi
    /// <a href="https://en.wikipedia.org/wiki/Shmuel_Kamenetsky">Rabbi Shmuel Kamenetsky</a> who agreed to it.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time when the sun is 7.67° below sea level. If the
    ///         calculation can't be computed such as northern and southern locations even south of the Arctic Circle and
    ///         north of the Antarctic Circle where the sun may not reach low enough below the horizon for this
    ///         calculation, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>To do:</b> add hyperlinks to source of Divrei Shalom once it is located.
    /// </remarks>
    /// <seealso cref="Zenith7Point67"/>
    public virtual DateTimeOffset? GetTzaisGeonim7Point67Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith7Point67));
    }

    /// <summary>
    /// This method returns the <em>tzais</em> (nightfall) based on the calculations used in the <a href=
    /// "https://www.worldcat.org/oclc/243303103">Luach Itim Lebinah</a> as the stringent time for <em>tzais</em>. It is calculated
    /// as the sun's position at <see cref="Zenith9Point3">9.3°</see> below the western horizon.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time when the sun is 9.3° below sea level. If the calculation can't be
    ///         computed such as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual DateTimeOffset? GetTzaisGeonim9Point3Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith9Point3));
    }

    /// <summary>
    /// This method returns the <em>tzais</em> (nightfall) based on the opinion of the <em>Geonim</em> calculated as 60 minutes after
    /// sunset <a href="https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a>,
    /// the day that a solar hour is 60 minutes in New York. The sun's position at this time computes to <see cref="Zenith9Point75">9.75°</see> below the western horizon. This is the opinion of <a href="https://en.wikipedia.org/wiki/Yosef_Eliyahu_Henkin"
    /// >Rabbi Eliyahu Henkin</a>. This also follows the opinion of <a href="https://en.wikipedia.org/wiki/Shmuel_Kamenetsky">Rabbi
    /// Shmuel Kamenetsky</a>. Rabbi Yaakov Shakow presented these degree-based times to Rabbi Shmuel Kamenetsky who agreed to them.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time when the sun is 9.75° below sea level. If the calculation can't be
    ///         computed such as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>To do:</b> recalculate based on equinox / equilux.
    /// </remarks>
    /// <seealso cref="GetTzais60Minutes"/>
    public virtual DateTimeOffset? GetTzaisGeonim9Point75Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith9Point75));
    }

    /// <summary>
    /// This method returns the <em>tzais</em> (nightfall) based on the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Yair_Bacharach">Chavas Yair</a> and <a href=
    /// "https://he.wikipedia.org/wiki/%D7%9E%D7%9C%D7%9B%D7%99%D7%90%D7%9C_%D7%A6%D7%91%D7%99_%D7%98%D7%A0%D7%A0%D7%91%D7%95%D7%99%D7%9D"
    /// >Divrei Malkiel</a> that the time to walk the distance of a <a href=
    /// "https://en.wikipedia.org/wiki/Biblical_mile">mil</a> is 15 minutes, for a total of 60 minutes for 4 mil after <see cref="ZmanimCalendar.GetSunsetBasedOnElevationSetting"/>(depends on the <see cref="ZmanimCalendar.UseElevation"/> setting). See detailed documentation explaining
    /// the 60 minute concept at <see cref="GetAlos60Minutes"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing 60 minutes after sea level sunset. If the calculation can't be computed such as
    ///         in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where it does not set,
    ///         a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos60Minutes"/>
    /// <seealso cref="GetPlagHamincha60Minutes"/>
    /// <seealso cref="GetShaahZmanis60Minutes"/>
    public virtual DateTimeOffset? GetTzais60Minutes()
    {
        return ToZone(GetTimeOffset(GetSunsetBasedOnElevationSetting(), JavaTime.OfMinutes(60)));
    }

    /// <summary>
    /// This method returns <em>tzais</em> usually calculated as 40 minutes (configurable to any offset via <see cref="AteretTorahSunsetOffset"/>) after sunset. Please note that <em>Chacham</em> Yosef Harari-Raful of Yeshivat Ateret
    /// Torah who uses this time, does so only for calculating various other <em>zmanei hayom</em> such as <em>Sof Zman Krias
    /// Shema</em> and <em>Plag Hamincha</em>. His calendars do not publish a <em>zman</em> for <em>Tzais</em>. It should also be
    /// noted that <em>Chacham</em> Harari-Raful provided a 25 minute <em>zman</em> for Israel. This API uses 40 minutes year round in
    /// any place on the globe by default. This offset can be changed by calling <see cref="AteretTorahSunsetOffset"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing 40 minutes (configurable via <see cref="AteretTorahSunsetOffset"/>) after sea
    ///         level sunset. If the calculation can't be computed such as in the Arctic Circle where there is at least one day a year
    ///         where the sun does not rise, and one where it does not set, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    public virtual DateTimeOffset? GetTzaisAteretTorah()
    {
        long totalNanos = (long)(AteretTorahSunsetOffset * MinuteNanos);
        return ToZone(GetTimeOffset(GetSunsetBasedOnElevationSetting(), JavaTime.DurationOfNanos(totalNanos)));
    }

    /// <summary>
    /// Returns the offset in minutes after sunset used to calculate <em>tzais</em> based on the calculations of <em>Chacham</em>
    /// Yosef Harari-Raful of Yeshivat Ateret Torah calculations. The default value is 40 minutes. This affects most <em>zmanim</em>,
    /// since almost all zmanim use subset as part of their calculation.
    /// </summary>
    /// <value>the number of minutes after sunset to use as an offset for the Ateret Torah <em>tzais</em></value>
    /// <remarks>
    /// Allows setting the offset in minutes after sunset for the Ateret Torah <em>zmanim</em>. The default if unset is 40 minutes.
    /// <em>Chacham</em> Yosef Harari-Raful of Yeshivat Ateret Torah uses 40 minutes globally with the exception of Israel where a
    /// 25-minute offset is used. This 40-minute (or any other) offset can be overridden by this method. This offset impacts all
    /// Ateret Torah <em>zmanim</em>.
    /// </remarks>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    public double AteretTorahSunsetOffset
    {
        get => ateretTorahSunsetOffset;
        set => ateretTorahSunsetOffset = value;
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite Shema in the morning) based on the calculation of
    /// <em>Chacham</em> Yosef Harari-Raful of Yeshivat Ateret Torah, that the day starts <see cref="GetAlos72Zmanis">1/10th of the day</see>
    /// before sunrise and is usually calculated as ending <see cref="GetTzaisAteretTorah">40 minutes after sunset</see> (configurable to any
    /// offset via <see cref="AteretTorahSunsetOffset"/>). <em>Shaos zmaniyos</em> are calculated based on this day and added to
    /// <see cref="GetAlos72Zmanis"><em>alos</em></see> to reach this time. This time is 3 <see cref="GetShaahZmanisAteretTorah"><em>shaos
    /// zmaniyos</em></see> (temporal hours) after <see cref="GetAlos72Zmanis"><em>alos</em> 72 <em>zmaniyos</em></see>. <b>Note: </b> Based on
    /// this calculation <em>chatzos</em> will not be at midday.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em> based on this calculation. If the calculation can't be
    ///         computed such as in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where
    ///         it does not set, a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation.</returns>
    /// <seealso cref="GetAlos72Zmanis"/>
    /// <seealso cref="GetTzaisAteretTorah"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    /// <seealso cref="GetShaahZmanisAteretTorah"/>
    public virtual DateTimeOffset? GetSofZmanShmaAteretTorah()
    {
        return ToZone(GetSofZmanShma(GetAlos72Zmanis(), GetTzaisAteretTorah(), false));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to recite the morning prayers) based on the calculation of
    /// <em>Chacham</em> Yosef Harari-Raful of Yeshivat Ateret Torah, that the day starts <see cref="GetAlos72Zmanis">1/10th of the day</see>
    /// before sunrise and is usually calculated as ending <see cref="GetTzaisAteretTorah">40 minutes after sunset</see> (configurable to any
    /// offset via <see cref="AteretTorahSunsetOffset"/>). <em>shaos zmaniyos</em> are calculated based on this day and added to
    /// <see cref="GetAlos72Zmanis"/> to reach this time. This time is 4 * <see cref="GetShaahZmanisAteretTorah"/> after <see cref="GetAlos72Zmanis"/>. <b>Note: </b> Based on this calculation <em>chatzos</em> will not be at midday. Sources for an
    /// asymmetrical day-based calculation can be seen in the documentation of
    /// <see cref="GetSofZmanShmaAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em> based on this calculation. If the
    ///         calculation can't be computed such as in the Arctic Circle where there is at least one day a year where
    ///         the sun does not rise, and one where it does not set, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos72Zmanis"/>
    /// <seealso cref="GetTzaisAteretTorah"/>
    /// <seealso cref="GetShaahZmanisAteretTorah"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    public virtual DateTimeOffset? GetSofZmanTfilaAteretTorah()
    {
        return ToZone(GetSofZmanTfila(GetAlos72Zmanis(), GetTzaisAteretTorah(), false));
    }

    /// <summary>
    /// This method returns the time of <em>mincha gedola</em> based on the calculation of <em>Chacham</em> Yosef
    /// Harari-Raful of Yeshivat Ateret Torah, that the day starts <see cref="GetAlos72Zmanis">1/10th of the day</see>
    /// before sunrise and is usually calculated as ending <see cref="GetTzaisAteretTorah">40 minutes after sunset</see>
    /// (configurable to any offset via <see cref="AteretTorahSunsetOffset"/>). This is the preferred earliest
    /// time to pray <em>mincha</em> according to the opinion of the <a href="https://en.wikipedia.org/wiki/Maimonides"
    /// >Rambam</a> and others. For more information on this see the documentation on <see cref="ZmanimCalendar.GetMinchaGedolaGRA"><em>mincha
    /// gedola</em></see>. This is calculated as 6.5 <see cref="GetShaahZmanisAteretTorah">solar hours</see> after <em>alos</em>. The
    /// calculation used is 6.5 * <see cref="GetShaahZmanisAteretTorah"/> after <see cref="GetAlos72Zmanis"><em>alos</em></see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>mincha gedola</em>. If the calculation can't be computed such as
    ///         in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where it
    ///         does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos72Zmanis"/>
    /// <seealso cref="GetTzaisAteretTorah"/>
    /// <seealso cref="GetShaahZmanisAteretTorah"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="GetMinchaKetanaAteretTorah"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    public virtual DateTimeOffset? GetMinchaGedolaAteretTorah()
    {
        return ToZone(GetMinchaGedola(GetAlos72Zmanis(), GetTzaisAteretTorah(), false));
    }

    /// <summary>
    /// This method returns the time of <em>mincha ketana</em> based on the calculation of
    /// <em>Chacham</em> Yosef Harari-Raful of Yeshivat Ateret Torah, that the day starts
    /// <see cref="GetAlos72Zmanis">1/10th of the day</see> before sunrise and is usually calculated as ending
    /// <see cref="GetTzaisAteretTorah">40 minutes after sunset</see> (configurable to any offset via
    /// <see cref="AteretTorahSunsetOffset"/>). This is the preferred earliest time to pray <em>mincha</em>
    /// according to the opinion of the <a href="https://en.wikipedia.org/wiki/Maimonides">Rambam</a> and others.
    /// For more information on this see the documentation on <see cref="ZmanimCalendar.GetMinchaGedolaGRA"><em>mincha gedola</em></see>. This is
    /// calculated as 9.5 <see cref="GetShaahZmanisAteretTorah">solar hours</see> after <see cref="GetAlos72Zmanis"><em>alos</em></see>.
    /// The calculation used is 9.5 * <see cref="GetShaahZmanisAteretTorah"/> after <see cref="GetAlos72Zmanis"><em>alos</em></see>. If the calculated time is after sunset (on very short winter days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>mincha ketana</em>. If the calculation can't be computed such as
    ///         in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where it
    ///         does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos72Zmanis"/>
    /// <seealso cref="GetTzaisAteretTorah"/>
    /// <seealso cref="GetShaahZmanisAteretTorah"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="ZmanimCalendar.GetMinchaKetanaGRA"/>
    public virtual DateTimeOffset? GetMinchaKetanaAteretTorah()
    {
        return ToZone(NullIfAfterSunset(GetMinchaKetana(GetAlos72Zmanis(), GetTzaisAteretTorah(), false)));
    }

    /// <summary>
    /// This method returns the time of <em>plag hamincha</em> based on the calculation of <em>Chacham</em> Yosef Harari-Raful
    /// of Yeshivat Ateret Torah, that the day starts <see cref="GetAlos72Zmanis">1/10th of the day</see> before sunrise and is
    /// usually calculated as ending <see cref="GetTzaisAteretTorah">40 minutes after sunset</see> (configurable to any offset
    /// via <see cref="AteretTorahSunsetOffset"/>). <em>shaos zmaniyos</em> are calculated based on this day and
    /// added to <see cref="GetAlos72Zmanis"><em>alos</em></see> to reach this time. This time is 10.75
    /// <see cref="GetShaahZmanisAteretTorah"><em>shaos zmaniyos</em></see> (temporal hours) after <see cref="GetAlos72Zmanis">dawn</see>. If the calculated time is after sunset (on very short winter days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the <em>plag</em>. If the calculation can't be computed such as in the Arctic Circle
    ///         where there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos72Zmanis"/>
    /// <seealso cref="GetTzaisAteretTorah"/>
    /// <seealso cref="GetShaahZmanisAteretTorah"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    /// <seealso cref="AteretTorahSunsetOffset"/>
    public virtual DateTimeOffset? GetPlagHaminchaAteretTorah()
    {
        return ToZone(NullIfAfterSunset(GetPlagHamincha(GetAlos72Zmanis(), GetTzaisAteretTorah(), false)));
    }

    /// <summary>
    /// Method to return <em>tzais</em> (dusk) calculated as 72 minutes zmaniyos, or 1/10th of the day after <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see>. This is the way that the <a href=
    /// "https://en.wikipedia.org/wiki/Abraham_Cohen_Pimentel">Minchas Cohen</a> in Ma'amar 2:4 calculates Rebbeinu Tam's
    /// time of <em>tzais</em>. It should be noted that this calculation results in the shortest time from sunset to
    /// <em>tzais</em> being during the winter solstice, the longest at the summer solstice and 72 clock minutes at the
    /// equinox. This does not match reality, since there is no direct relationship between the length of the day and
    /// twilight. The shortest twilight is during the equinox, the longest is during the summer solstice, and in the
    /// winter with the shortest daylight, the twilight period is longer than during the equinoxes.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set,
    ///         a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation.</returns>
    /// <seealso cref="GetAlos72Zmanis"/>
    public virtual DateTimeOffset? GetTzais72Zmanis()
    {
        return ToZone(GetZmanisBasedOffset(1.2));
    }

    /// <summary>
    /// Method to return <em>tzais</em> (dusk) calculated using 90 minutes zmaniyos or 1/8th of the day after <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see>. This time is known in Yiddish as the <em>achtel</em> (an eighth)
    /// <em>zman</em> used in various <em>kehilos</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set,
    ///         a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation.</returns>
    /// <seealso cref="GetAlos90Zmanis"/>
    public virtual DateTimeOffset? GetTzais90Zmanis()
    {
        return ToZone(GetZmanisBasedOffset(1.5));
    }

    /// <summary>
    /// Method to return <em>tzais</em> (dusk) calculated using 96 minutes <em>zmaniyos</em> or 1/7.5 of the day after
    /// <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set,
    ///         a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation.</returns>
    /// <seealso cref="GetAlos96Zmanis"/>
    public virtual DateTimeOffset? GetTzais96Zmanis()
    {
        return ToZone(GetZmanisBasedOffset(1.6));
    }

    /// <summary>
    /// Method to return <em>tzais</em> (dusk) calculated as 90 minutes after <see cref="ZmanimCalendar.GetSunsetBasedOnElevationSetting"/> (depends on
    /// the <see cref="ZmanimCalendar.UseElevation"/> setting). This method returns <em>tzais</em> (nightfall) based on the opinion of the Magen
    /// Avraham that the time to walk the distance of a <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a> according to the
    /// <a href="https://en.wikipedia.org/wiki/Maimonides">Rambam</a>'s opinion is 18 minutes, for a total of 90 minutes based on the
    /// opinion of Ula who calculated <em>tzais</em> as 5 mil after elevation adjusted <em>shkiah</em> (sunset). A similar
    /// calculation <see cref="GetTzais19Point8Degrees"/> uses solar position* calculations based on this time.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set,
    ///         a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation.</returns>
    /// <seealso cref="GetTzais19Point8Degrees"/>
    /// <seealso cref="GetAlos90Minutes"/>
    public virtual DateTimeOffset? GetTzais90Minutes()
    {
        return ToZone(GetTimeOffset(GetSunsetBasedOnElevationSetting(), JavaTime.OfMinutes(90)));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns <em>tzais</em> (nightfall) based on the calculations of <a href=
    /// "https://en.wikipedia.org/wiki/Avraham_Chaim_Naeh">Rav Chaim Naeh</a> that the time to walk the distance of a <a href=
    /// "https://en.wikipedia.org/wiki/Biblical_mile">mil</a> according to the <a href="https://en.wikipedia.org/wiki/Maimonides"
    /// >Rambam</a>'s opinion is 2/5 of an hour (24 minutes) for a total of 120* minutes based on the opinion of <em>Ula</em> who
    /// calculated <em>tzais</em> as 5 mil after <see cref="ZmanimCalendar.GetSunsetBasedOnElevationSetting"/> (depends on the <see cref="ZmanimCalendar.UseElevation"/>
    /// setting). A similar calculation <see cref="GetTzais26Degrees"/> uses degree-based calculations based on this 120 minute
    /// calculation Since the <em>zman</em> is extremely late and at a point that is long past the 18° point where the darkest point
    /// is reached, it should only be used <em>lechumra</em>, such as delaying the start of nighttime <em>mitzvos</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set,
    ///         a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>.
    ///         documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time, and if used
    ///         <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this
    ///         method from the API, and this deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetTzais26Degrees"/>
    /// <seealso cref="GetAlos120Minutes"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time, and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetTzais120Minutes()
    {
        return ToZone(GetTimeOffset(GetSunsetBasedOnElevationSetting(), JavaTime.OfMinutes(120)));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns <em>tzais</em> (dusk) calculated using 120 minutes
    /// <em>zmaniyos</em> after <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see>. Since the <em>zman</em>
    /// is extremely late and at a time when the sun is well below the 18° point (scientifically the darkest point) in
    /// most places on the globe, it should only be used <em>lechumra</em>, such as delaying the start of nighttime
    /// <em>mitzvos</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set,
    ///         a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time, and if used
    ///         <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this
    ///         method from the API, and this deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetAlos120Zmanis"/>
    /// <seealso cref="GetTzais120Minutes"/>
    /// <seealso cref="GetTzais26Degrees"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time, and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetTzais120Zmanis()
    {
        return ToZone(GetZmanisBasedOffset(2.0));
    }

    /// <summary>
    /// This calculates the time of <em>tzais</em> at the point when the sun is 16.1° below the horizon. This is
    /// the sun's dip below the horizon 72 minutes after sunset according Rabbeinu Tam's calculation of <em>tzais</em>
    /// <a href=
    /// "https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a> in
    /// Jerusalem. The question of equinox VS equilux is complex, with Rabbi Meir Posen in the <a href=
    /// "https://www.worldcat.org/oclc/956316270">Ohr Meir</a> of the opinion that the equilux should be used. See
    /// Yisrael Vehazmanim vol I, 34:1:4. Rabbi Yedidya Manet in his <a href=
    /// "https://www.nli.org.il/en/books/NNL_ALEPH002542826/NLI">Zmanei HaHalacha Lema'aseh</a> (4th edition part 2, pages
    /// and 22 and 24) and Rabbi Yonah Mertzbuch (in a letter published by Rabbi Manet) are of the opinion that the
    /// astronomical equinox should be used. The difference adds up to about 9 seconds, too trivial to make much of a
    /// difference. For information on how this is calculated see the comments on <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may
    ///         not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See
    ///         detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetTzais72Minutes"/>
    /// <seealso cref="ZmanimCalendar.GetAlos16Point1Degrees"/>
    public virtual DateTimeOffset? GetTzais16Point1Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith16Point1));
    }

    /// <summary>
    /// This method should be used <em>lechumra</em> only and returns <em>tzais</em> based on when the sun is 26°
    /// below the horizon. For information on how this is calculated see the comments on <see cref="GetAlos26Degrees"/>.
    /// Since the <em>zman</em> is extremely late and at a point when it is long past the 18° point where the
    /// darkest point is reached, it should only be used <em>lechumra</em> such as delaying the start of nighttime
    /// <em>mitzvos</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may
    ///         not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See
    ///         detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>Deprecated:</b> This method should be used <em>lechumra</em> only since it returns a very late time, and if used
    ///         <em>lekula</em> can result in <em>chillul Shabbos</em> etc. There is no current plan to remove this
    ///         method from the API, and this deprecation is intended to alert developers of the danger of using it.
    /// </remarks>
    /// <seealso cref="GetTzais120Minutes"/>
    /// <seealso cref="GetAlos26Degrees"/>
    [Obsolete("This method should be used lechumra only since it returns a very late time, and if used lekula can result in chillul Shabbos etc.")]
    public virtual DateTimeOffset? GetTzais26Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith26Degrees));
    }

    /// <summary>
    /// For information on how this is calculated see the comments on <see cref="GetAlos18Degrees"/>
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may
    ///         not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See
    ///         detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos18Degrees"/>
    public virtual DateTimeOffset? GetTzais18Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(AstronomicalZenith));
    }

    /// <summary>
    /// For information on how this is calculated see the comments on <see cref="GetAlos19Point8Degrees"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as northern and
    ///         southern locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may
    ///         not reach low enough below the horizon for this calculation, a <c>null</c> will be returned. See
    ///         detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetTzais90Minutes"/>
    /// <seealso cref="GetAlos19Point8Degrees"/>
    public virtual DateTimeOffset? GetTzais19Point8Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith19Point8));
    }

    /// <summary>
    /// A method to return <em>tzais</em> (dusk) calculated as 96 minutes after <see cref="AstronomicalCalendar.GetSunset">sunset</see> or <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> (depending on the <see cref="ZmanimCalendar.UseElevation"/> setting). For information on how
    /// this is calculated see the comments on <see cref="GetAlos96Minutes"/>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set,
    ///         a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation.</returns>
    /// <seealso cref="GetAlos96Minutes"/>
    public virtual DateTimeOffset? GetTzais96Minutes()
    {
        return ToZone(GetTimeOffset(GetSunsetBasedOnElevationSetting(), JavaTime.OfMinutes(96)));
    }

    /// <summary>
    /// A method that returns the local time for fixed <em>chatzos</em>. This time is noon and adjusted from
    /// standard time to account for the local longitude. The 360° of the globe divided by 24 calculates to 15°
    /// per hour with 4 minutes per degree, so at a longitude of 0 , 15, 30 etc... <em>Chatzos</em> is at exactly 12:00
    /// noon. This is the time of <em>chatzos</em> according to the <a href=
    /// "https://en.wikipedia.org/wiki/Aruch_HaShulchan">Aruch Hashulchan</a> in <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=7705&amp;pgnum=426">Orach Chaim 233:14</a> and <a href=
    /// "https://en.wikipedia.org/wiki/Moshe_Feinstein">Rabbi Moshe Feinstein</a> in Igros Moshe <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=916&amp;st=&amp;pgnum=67">Orach Chaim 1:24</a> and <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=14675&amp;pgnum=191">2:20</a>.
    /// Lakewood, N.J., with a longitude of -74.222, is 0.778 away from the closest multiple of 15 at -75°. This
    /// is multiplied by 4 to yield 3 minutes and 7 seconds for a <em>chatzos</em> of 11:56:53. This method is not tied
    /// to the theoretical 15° time zones, but will adjust to the actual time zone and <a
    /// href="https://en.wikipedia.org/wiki/Daylight_saving_time">Daylight saving time</a>. On very short polar days, fixed local
    /// <em>chatzos</em> can fall before sunrise or after sunset; in those cases a <c>null</c> is returned. Days without a sunrise
    /// or sunset (polar day or night) are not affected.
    /// </summary>
    /// <returns>the Instant representing the local <em>chatzos</em>, or <c>null</c> if it is before sunrise or after sunset.</returns>
    /// <seealso cref="GeoLocation.GetLocalMeanTimeOffset"/>
    /// <seealso cref="AstronomicalCalendar.GetLocalMeanTime"/>
    public virtual DateTimeOffset? GetFixedLocalChatzosHayom()
    {
        DateTimeOffset? fixedLocalChatzos = GetLocalMeanTime(new TimeOnly(12, 0));
        if (fixedLocalChatzos == null)
        {
            return null;
        }
        DateTimeOffset? sunrise = GetSunriseBasedOnElevationSetting();
        DateTimeOffset? sunset = GetSunsetBasedOnElevationSetting();
        if ((sunrise != null && fixedLocalChatzos.IsBefore(sunrise)) || (sunset != null && fixedLocalChatzos.IsAfter(sunset)))
        {
            return null; // very short polar day: fixed local chatzos is outside the day
        }
        return ToZone(fixedLocalChatzos);
    }

    /// <summary>
    /// Returns the latest time of <em>Kidush Levana</em> according to the <a
    /// href="https://en.wikipedia.org/wiki/Yaakov_ben_Moshe_Levi_Moelin">Maharil's</a> opinion that it is calculated as
    /// halfway between <em>molad</em> and <em>molad</em>. This adds half the 29 days, 12 hours and 793 chalakim time
    /// between <em>molad</em> and <em>molad</em> (14 days, 18 hours, 22 minutes and 666 milliseconds) to the month's <em>molad</em>.
    /// If the time of <em>sof zman Kiddush Levana</em> occurs during the day (between the <em>alos</em> and <em>tzais</em> passed in
    /// as parameters), it returns the <em>alos</em> passed in. If a <c>null</c> <em>alos</em> or <em>tzais</em> are passed to
    /// this method, the non-daytime adjusted time will be returned.
    /// </summary>
    /// <param name="alos">the beginning of the Jewish day. If <em>Kidush Levana</em> occurs during the day (starting at <em>alos</em> and
    ///         ending at <em>tzais</em>), the time returned will be <em>alos</em>. If either the <em>alos</em> or <em>tzais</em>
    ///         parameters are <c>null</c>, no daytime adjustment will be made.</param>
    /// <param name="tzais">the end of the Jewish day. If Kidush Levana occurs during the day (starting at <em>alos</em> and ending at
    ///         <em>tzais</em>), the time returned will be <em>alos</em>. If either the <em>alos</em> or <em>tzais</em> parameter
    ///         are <c>null</c>, no daytime adjustment will be made.</param>
    /// <returns>the Instant representing the moment halfway between molad and molad. If the time occurs between
    ///         <em>alos</em> and <em>tzais</em>, <em>alos</em> will be returned. If the <em>zman</em> will not occur on this day, a
    ///         <c>null</c> will be returned.</returns>
    /// <seealso cref="GetSofZmanKidushLevanaBetweenMoldos(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="GetSofZmanKidushLevana15Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="JewishCalendar.GetSofZmanKidushLevanaBetweenMoldos"/>
    public virtual DateTimeOffset? GetSofZmanKidushLevanaBetweenMoldos(DateTimeOffset? alos, DateTimeOffset? tzais)
    {
        JewishCalendar jewishCalendar = new JewishCalendar(LocalDate);

        // Do not calculate for impossible dates, but account for extreme cases. In the extreme case of Rapa Iti in French
        // Polynesia on Dec 2027 when kiddush Levana 3 days can be said on <em>Rosh Chodesh</em>, the sof zman Kiddush Levana
        // will be on the 12th of the Teves. In the case of Anadyr, Russia on Jan, 2071, sof zman Kiddush Levana between the
        // moldos will occur is on the night of 17th of Shevat. See Rabbi Dovid Heber's Shaarei Zmanim chapter 4 (pages 28 and 32).
        if (jewishCalendar.JewishDayOfMonth < 11 || jewishCalendar.JewishDayOfMonth > 16)
        {
            return null;
        }
        return ToZone(GetMoladBasedTime(jewishCalendar.GetSofZmanKidushLevanaBetweenMoldos(), alos, tzais, false));
    }

    /// <summary>
    /// Returns the Instant of the <em>molad</em> based time if it occurs on the current date. Since <em>Kiddush Levana</em>
    /// can only be said during the day, there are parameters to limit it to between <em>alos</em> and <em>tzais</em>. If
    /// the time occurs between <em>alos</em> and <em>tzais</em>, <em>tzais</em> will be returned.
    /// </summary>
    /// <param name="moladBasedTime">the <em>molad</em> based time such as <em>molad</em>, <em>tchilas</em> and <em>sof zman Kiddush
    ///         Levana</em></param>
    /// <param name="alos">optional start of day to limit <em>molad</em> times to the end of the night before or beginning of the next night.
    ///         Ignored if either <em>alos</em> or <em>tzais</em> are <c>null</c>.</param>
    /// <param name="tzais">optional end of day to limit <em>molad</em> times to the end of the night before or beginning of the next night.
    ///         Ignored if either <em>tzais</em> or <em>alos</em> are <c>null</c></param>
    /// <param name="techila">is it the start of <em>Kiddush Levana</em> time or the end? If it is start roll it to the next <em>tzais</em>,
    ///         and if it is the end, return the end of the previous night (<em>alos</em> passed in). Ignored if either
    ///         <em>alos</em> or <em>tzais</em> are <c>null</c>.</param>
    /// <returns>the <em>molad</em> based time. If the <em>zman</em> does not occur during the current date, <c>null</c> will be
    ///         returned.</returns>
    private DateTimeOffset? GetMoladBasedTime(DateTimeOffset? moladBasedTime, DateTimeOffset? alos, DateTimeOffset? tzais, bool techila)
    {
        DateTimeOffset? lastMidnight = GetMidnightLastNight();
        DateTimeOffset? midnightTonight = GetMidnightTonight();
        if (moladBasedTime.IsBefore(lastMidnight) || moladBasedTime.IsAfter(midnightTonight))
        { // Invalid time, bailout
            return null;
        }
        else if (alos == null || tzais == null)
        { // Not enough info to adjust
            return moladBasedTime;
        }
        else
        { // It's the daytime, get the next/prev night
            if (moladBasedTime.IsAfter(alos) && moladBasedTime.IsBefore(tzais))
            {
                if (techila)
                {
                    return tzais;
                }
                else
                {
                    return alos;
                }
            }
            else
            { // It's the night, the provided time is valid
                return moladBasedTime;
            }
        }
    }

    /// <summary>
    /// Returns the latest time of Kiddush Levana according to the <a
    /// href="https://en.wikipedia.org/wiki/Yaakov_ben_Moshe_Levi_Moelin">Maharil's</a> opinion that it is calculated as
    /// halfway between <em>molad</em> and <em>molad</em>. This adds half the 29 days, 12 hours and 793 chalakim time between
    /// <em>molad</em> and <em>molad</em> (14 days, 18 hours, 22 minutes and 666 milliseconds) to the month's <em>molad</em>.
    /// The <em>sof zman Kiddush Levana</em> will be returned even if it occurs during the day. To limit the time to between
    /// <em>tzais</em> and <em>alos</em>, see <see cref="GetSofZmanKidushLevanaBetweenMoldos(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>.
    /// </summary>
    /// <returns>the Instant representing the moment halfway between molad and molad. If the <em>zman</em> will not occur on this
    ///         day, a <c>null</c> will be returned.</returns>
    /// <seealso cref="GetSofZmanKidushLevanaBetweenMoldos(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="GetSofZmanKidushLevana15Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="JewishCalendar.GetSofZmanKidushLevanaBetweenMoldos"/>
    public virtual DateTimeOffset? GetSofZmanKidushLevanaBetweenMoldos()
    {
        return ToZone(GetSofZmanKidushLevanaBetweenMoldos(null, null));
    }

    /// <summary>
    /// Returns the latest time of <em>Kiddush Levana</em> calculated as 15 days after the <em>molad</em>. This is the
    /// opinion brought down in the Shulchan Aruch (Orach Chaim 426). It should be noted that some opinions hold that the
    /// <a href="https://en.wikipedia.org/wiki/Moses_Isserles">Rema</a> who brings down the opinion of the <a
    /// href="https://en.wikipedia.org/wiki/Yaakov_ben_Moshe_Levi_Moelin">Maharil's</a> of calculating
    /// <see cref="GetSofZmanKidushLevanaBetweenMoldos(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})">halfway between <em>molad</em> and <em>molad</em></see> is of
    /// the opinion that the Mechaber agrees to his opinion. Also see the Aruch Hashulchan. For additional details on the subject,
    /// see Rabbi Dovid Heber's very detailed write-up in <em>Siman Daled</em> (chapter 4) of <a href=
    /// "https://hebrewbooks.org/53000">Shaarei Zmanim</a>. If the time of <em>sof zman Kiddush Levana</em> occurs during
    /// the day (between the <em>alos</em> and <em>tzais</em> passed in as parameters), it returns the <em>alos</em> passed in. If a
    /// <c>null</c> <em>alos</em> or <em>tzais</em> are passed to this method, the non-daytime adjusted time will be returned.
    /// </summary>
    /// <param name="alos">the beginning of the Jewish day. If <em>Kidush Levana</em> occurs during the day (starting at <em>alos</em> and
    ///         ending at <em>tzais</em>), the time returned will be <em>alos</em>. If either the <em>alos</em> or <em>tzais</em>
    ///         parameters are <c>null</c>, no daytime adjustment will be made.</param>
    /// <param name="tzais">the end of the Jewish day. If <em>Kidush Levana</em> occurs during the day (starting at <em>alos</em> and ending
    ///         at <em>tzais</em>), the time returned will be <em>alos</em>. If either the <em>alos</em> or <em>tzais</em> parameters
    ///         are <c>null</c>, no daytime adjustment will be made.</param>
    /// <returns>the Instant representing the moment 15 days after the molad. If the time occurs between <em>alos</em> and
    ///         <em>tzais</em>, <em>alos</em> will be returned. If the <em>zman</em> will not occur on this day, a
    ///         <c>null</c> will be returned.</returns>
    /// <seealso cref="GetSofZmanKidushLevanaBetweenMoldos(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="JewishCalendar.GetSofZmanKidushLevana15Days"/>
    public virtual DateTimeOffset? GetSofZmanKidushLevana15Days(DateTimeOffset? alos, DateTimeOffset? tzais)
    {
        JewishCalendar jewishCalendar = new JewishCalendar(LocalDate);

        // Do not calculate for impossible dates, but account for extreme cases. In the extreme case of Rapa Iti in
        // French Polynesia on Dec 2027 when kiddush Levana 3 days can be said on <em>Rosh Chodesh</em>, the sof zman Kiddush
        // Levana will be on the 12th of the Teves. in the case of Anadyr, Russia on Jan, 2071, sof zman kiddush levana will
        // occur after midnight on the 17th of Shevat. See Rabbi Dovid Heber's Shaarei Zmanim chapter 4 (pages 28 and 32).
        if (jewishCalendar.JewishDayOfMonth < 11 || jewishCalendar.JewishDayOfMonth > 17)
        {
            return null;
        }
        return ToZone(GetMoladBasedTime(jewishCalendar.GetSofZmanKidushLevana15Days(), alos, tzais, false));
    }

    /// <summary>
    /// Returns the latest time of <em>Kiddush Levana</em> calculated as 15 days after the molad. This is the opinion of
    /// the Shulchan Aruch (Orach Chaim 426). It should be noted that some opinions hold that the
    /// <a href="https://en.wikipedia.org/wiki/Moses_Isserles">Rema</a> who brings down the opinion of the <a
    /// href="https://en.wikipedia.org/wiki/Yaakov_ben_Moshe_Levi_Moelin">Maharil's</a> of calculating
    /// <see cref="GetSofZmanKidushLevanaBetweenMoldos(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})">halfway between <em>molad</em> and <em>molad</em></see> is of
    /// the opinion that the Mechaber agrees to his opinion. Also see the Aruch Hashulchan. For additional details on the subject,
    /// See Rabbi Dovid Heber's very detailed write-up in Siman Daled (chapter 4) of <a href="https://hebrewbooks.org/53000">Shaarei
    /// Zmanim</a>. The <em>sof zman Kiddush Levana</em> will be returned even if it occurs during the day. To limit the time to
    /// between <em>tzais</em> and <em>alos</em>, see <see cref="GetSofZmanKidushLevana15Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>.
    /// </summary>
    /// <returns>the Instant representing the moment 15 days after the <em>molad</em>. If the <em>zman</em> will not occur on this day,
    ///         a <c>null</c> will be returned.</returns>
    /// <seealso cref="GetSofZmanKidushLevana15Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="GetSofZmanKidushLevanaBetweenMoldos(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="JewishCalendar.GetSofZmanKidushLevana15Days"/>
    public virtual DateTimeOffset? GetSofZmanKidushLevana15Days()
    {
        return ToZone(GetSofZmanKidushLevana15Days(null, null));
    }

    /// <summary>
    /// Returns the earliest time of <em>Kiddush Levana</em> according to <a href=
    /// "https://en.wikipedia.org/wiki/Yonah_Gerondi">Rabbeinu Yonah</a>'s opinion that it can be said 3 days after the
    /// <em>molad</em>. The time will be returned even if it occurs during the day when <em>Kiddush Levana</em> can't be said.
    /// Use <see cref="GetTchilasZmanKidushLevana3Days()"/> if you want to limit the time to night hours.
    /// </summary>
    /// <returns>the Instant representing the moment 3 days after the molad. If the <em>zman</em> will not occur on this day, a
    ///         <c>null</c> will be returned.</returns>
    /// <seealso cref="GetTchilasZmanKidushLevana3Days()"/>
    /// <seealso cref="GetTchilasZmanKidushLevana7Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="JewishCalendar.GetTchilasZmanKidushLevana3Days"/>
    public virtual DateTimeOffset? GetTchilasZmanKidushLevana3Days()
    {
        return ToZone(GetTchilasZmanKidushLevana3Days(null, null));
    }

    /// <summary>
    /// Returns the earliest time of <em>Kiddush Levana</em> according to <a href=
    /// "https://en.wikipedia.org/wiki/Yonah_Gerondi">Rabbeinu Yonah</a>'s opinion that it can be said 3 days after the <em>molad</em>.
    /// If the time of <em>tchilas zman Kiddush Levana</em> occurs during the day (between <em>alos</em> and <em>tzais</em> passed to
    /// this method) it will return the following <em>tzais</em>. If <c>null</c> is passed for either <em>alos</em> or <em>tzais</em>, the
    /// actual <em>tchilas zman Kiddush Levana</em> will be returned, regardless of if it is during the day or not.
    /// </summary>
    /// <param name="alos">the beginning of the Jewish day. If Kidush Levana occurs during the day (starting at <em>alos</em> and ending at
    ///         <em>tzais</em>), the time returned will be <em>tzais</em>. If either the <em>alos</em> or <em>tzais</em> parameters
    ///         are <c>null</c>, no daytime adjustment will be made.</param>
    /// <param name="tzais">the end of the Jewish day. If <em>Kidush Levana</em> occurs during the day (starting at <em>alos</em> and ending
    ///         at <em>tzais</em>), the time returned will be <em>tzais</em>. If either the <em>alos</em> or <em>tzais</em> parameters
    ///         are <c>null</c>, no daytime adjustment will be made.</param>
    /// <returns>the Instant representing the moment 3 days after the molad. If the time occurs between <em>alos</em> and
    ///         <em>tzais</em>, <em>tzais</em> will be returned. If the <em>zman</em> will not occur on this day, a <c>null</c>
    ///         will be returned.</returns>
    /// <seealso cref="GetTchilasZmanKidushLevana3Days()"/>
    /// <seealso cref="GetTchilasZmanKidushLevana7Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="JewishCalendar.GetTchilasZmanKidushLevana3Days"/>
    public virtual DateTimeOffset? GetTchilasZmanKidushLevana3Days(DateTimeOffset? alos, DateTimeOffset? tzais)
    {
        JewishCalendar jewishCalendar = new JewishCalendar(LocalDate);

        // Do not calculate for impossible dates, but account for extreme cases. Tchilas zman kiddush Levana 3 days for
        // the extreme case of Rapa Iti in French Polynesia on Dec 2027 when kiddush Levana 3 days can be said on the evening
        // of the 30th, the second night of Rosh Chodesh. The 3rd day after the <em>molad</em> will be on the 4th of the month.
        // In the case of Anadyr, Russia on Jan, 2071, when sof zman kiddush levana is on the 17th of the month, the 3rd day
        // from the molad will be on the 5th day of Shevat. See Rabbi Dovid Heber's Shaarei Zmanim chapter 4 (pages 28 and 32).
        if (jewishCalendar.JewishDayOfMonth > 5 && jewishCalendar.JewishDayOfMonth < 30)
        {
            return null;
        }

        DateTimeOffset? zman = GetMoladBasedTime(jewishCalendar.GetTchilasZmanKidushLevana3Days(), alos, tzais, true);

        // Get the following month's zman kiddush Levana for the extreme case of Rapa Iti in French Polynesia on Dec 2027 when kiddush
        // Levana can be said on Rosh Chodesh (the evening of the 30th). See Rabbi Dovid Heber's Shaarei Zmanim chapter 4 (page 32)
        if (zman == null && jewishCalendar.JewishDayOfMonth == 30)
        {
            jewishCalendar.PlusMonths(1);
            zman = GetMoladBasedTime(jewishCalendar.GetTchilasZmanKidushLevana3Days(), null, null, true);
        }

        return ToZone(zman);
    }

    /// <summary>
    /// Returns the point in time of <em>Molad</em> as a <see cref="DateTimeOffset"/> Object. For the traditional day of week, hour,
    /// minute and chalakim, <see cref="JewishCalendar.GetMoladAsInstant"/> and the not yet completed
    /// <see cref="HebrewDateFormatter"/> that will have formatting for this.
    /// </summary>
    /// <returns>the Instant representing the moment of the molad. If the <em>molad</em> does not occur on this day, a
    ///         <c>null</c> will be returned.</returns>
    /// <seealso cref="GetTchilasZmanKidushLevana3Days()"/>
    /// <seealso cref="GetTchilasZmanKidushLevana7Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="JewishCalendar.GetMoladAsInstant"/>
    public virtual DateTimeOffset? GetZmanMolad()
    {
        JewishCalendar jewishCalendar = new JewishCalendar(LocalDate);

        // Optimize to not calculate for impossible dates, but account for extreme cases. The molad in the extreme case of Rapa
        // Iti in French Polynesia on Dec 2027 occurs on the night of the 27th of Kislev. In the case of Anadyr, Russia on
        // Jan 2071, the molad will be on the 2nd day of Shevat. See Rabbi Dovid Heber's Shaarei Zmanim chapter 4 (pages 28 and 32).
        if (jewishCalendar.JewishDayOfMonth > 2 && jewishCalendar.JewishDayOfMonth < 27)
        {
            return null;
        }
        DateTimeOffset? molad = GetMoladBasedTime(jewishCalendar.GetMoladAsInstant(), null, null, true);

        // deal with molad that happens on the end of the previous month
        if (molad == null && jewishCalendar.JewishDayOfMonth > 26)
        {
            jewishCalendar.PlusMonths(1);
            molad = GetMoladBasedTime(jewishCalendar.GetMoladAsInstant(), null, null, true);
        }
        return ToZone(molad);
    }

    /// <summary>
    /// Returns the earliest time of <em>Kiddush Levana</em> according to the opinions that it should not be said until 7
    /// days after the <em>molad</em>. If the time of <em>tchilas zman Kiddush Levana</em> occurs during the day (between
    /// <see cref="ZmanimCalendar.GetAlos72Minutes"><em>alos</em></see> and <see cref="ZmanimCalendar.GetTzais72Minutes"><em>tzais</em></see>) it
    /// return the next <em>tzais</em>.
    /// </summary>
    /// <param name="alos">the beginning of the Jewish day. If <em>Kidush Levana</em> occurs during the day (starting at <em>alos</em> and
    ///         ending at <em>tzais</em>), the time returned will be <em>tzais</em>. If either the <em>alos</em> or <em>tzais</em>
    ///         parameters are <c>null</c>, no daytime adjustment will be made.</param>
    /// <param name="tzais">the end of the Jewish day. If <em>Kidush Levana</em> occurs during the day (starting at <em>alos</em> and ending
    ///         at <em>tzais</em>), the time returned will be <em>tzais</em>. If either the <em>alos</em> or <em>tzais</em> parameters
    ///         are <c>null</c>, no daytime adjustment will be made.</param>
    /// <returns>the Instant representing the moment 7 days after the molad. If the time occurs between <em>alos</em> and
    ///         <em>tzais</em>, <em>tzais</em> will be returned. If the <em>zman</em> will not occur on this day, a
    ///         <c>null</c> will be returned.</returns>
    /// <seealso cref="GetTchilasZmanKidushLevana3Days()"/>
    /// <seealso cref="GetTchilasZmanKidushLevana7Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="JewishCalendar.GetTchilasZmanKidushLevana7Days"/>
    public virtual DateTimeOffset? GetTchilasZmanKidushLevana7Days(DateTimeOffset? alos, DateTimeOffset? tzais)
    {
        JewishCalendar jewishCalendar = new JewishCalendar(LocalDate);

        // Optimize to not calculate for impossible dates, but account for extreme cases. Tchilas zman kiddush Levana 7 days for
        // the extreme case of Rapa Iti in French Polynesia on Jan 2028 (when kiddush Levana 3 days can be said on the evening
        // of the 30th, the second night of Rosh Chodesh), the 7th day after the molad will be on the 4th of the month.
        // In the case of Anadyr, Russia on Jan, 2071, when sof zman kiddush levana is on the 17th of the month, the 7th day
        // from the molad will be on the 9th day of Shevat. See Rabbi Dovid Heber's Shaarei Zmanim chapter 4 (pages 28 and 32).
        if (jewishCalendar.JewishDayOfMonth < 4 || jewishCalendar.JewishDayOfMonth > 9)
        {
            return null;
        }

        return ToZone(GetMoladBasedTime(jewishCalendar.GetTchilasZmanKidushLevana7Days(), alos, tzais, true));
    }

    /// <summary>
    /// Returns the earliest time of <em>Kiddush Levana</em> according to the opinions that it should not be said until 7
    /// days after the <em>molad</em>. The time will be returned even if it occurs during the day when <em>Kiddush Levana</em>
    /// can't be recited. Use <see cref="GetTchilasZmanKidushLevana7Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/> if you want to limit the time to night hours.
    /// </summary>
    /// <returns>the Instant representing the moment 7 days after the molad regardless of it is day or night. If the <em>zman</em>
    ///         will not occur on this day, a <c>null</c> will be returned.</returns>
    /// <seealso cref="GetTchilasZmanKidushLevana7Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="JewishCalendar.GetTchilasZmanKidushLevana7Days"/>
    /// <seealso cref="GetTchilasZmanKidushLevana3Days()"/>
    public virtual DateTimeOffset? GetTchilasZmanKidushLevana7Days()
    {
        return ToZone(GetTchilasZmanKidushLevana7Days(null, null));
    }

    /// <summary>
    /// This method returns the latest time one is allowed eating <em>chametz</em> on <em>Erev Pesach</em> according to
    /// the opinion of the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. This time is identical to the <see cref="ZmanimCalendar.GetSofZmanTfilaGRA"><em>Sof zman tfilah</em> GRA</see> and is provided as a convenience method for those who are
    /// unaware how this <em>zman</em> is calculated. This time is 4 hours into the day based on the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a> that the day is calculated from sunrise to sunset.
    /// This returns the time 4 * <see cref="ZmanimCalendar.GetShaahZmanisGRA"/> after <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see>. If it
    /// is not <em>erev Pesach</em>, a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> one is allowed eating <em>chametz</em> on <em>Erev Pesach</em>. If it is not <em>erev
    ///         Pesach</em> or the calculation can't be computed such as in the Arctic Circle where there is at least one
    ///         day a year when the sun does not rise, and one where it does not set, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetShaahZmanisGRA"/>
    /// <seealso cref="ZmanimCalendar.GetSofZmanTfilaGRA"/>
    /// <seealso cref="ZmanimCalendar.GetSofZmanAchilasChametz"/>
    public virtual DateTimeOffset? GetSofZmanAchilasChametzGRA()
    {
        return ToZone(GetSofZmanAchilasChametz(GetSunriseBasedOnElevationSetting(), GetSunsetBasedOnElevationSetting(), true));
    }

    /// <summary>
    /// This method returns the latest time one is allowed eating <em>chametz</em> on <em>Erev Pesach</em> according to the
    /// opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em>
    /// being <see cref="ZmanimCalendar.GetAlos72Minutes">72</see> minutes before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is identical to the
    /// <see cref="ZmanimCalendar.GetSofZmanTfilaMGA72Minutes"><em>Sof zman tfilah</em> MGA 72 minutes</see>. This time is 4 <see cref="ZmanimCalendar.GetShaahZmanis72Minutes"><em>shaos zmaniyos</em></see> (temporal hours) after <see cref="ZmanimCalendar.GetAlos72Minutes">dawn</see> based on the
    /// opinion of the MGA that the day is calculated from a <see cref="ZmanimCalendar.GetAlos72Minutes">dawn</see> of 72 minutes before sunrise to <see cref="ZmanimCalendar.GetTzais72Minutes">nightfall</see> of 72 minutes after sunset. This returns the time of 4 * <see cref="ZmanimCalendar.GetShaahZmanis72Minutes"/>
    /// after <see cref="ZmanimCalendar.GetAlos72Minutes">dawn</see>. If it is not <em>erev Pesach</em>, a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest time of eating <em>chametz</em>. If it is not <em>erev Pesach</em> or the
    ///         calculation can't be computed such as in the Arctic Circle where there is at least one day a year when the sun does
    ///         not rise, and one where it does not set, a <c>null</c> will be returned. See detailed explanation on top of
    ///         the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetShaahZmanis72Minutes"/>
    /// <seealso cref="ZmanimCalendar.GetAlos72Minutes"/>
    /// <seealso cref="ZmanimCalendar.GetSofZmanTfilaMGA72Minutes"/>
    /// <seealso cref="ZmanimCalendar.GetSofZmanAchilasChametz"/>
    public virtual DateTimeOffset? GetSofZmanAchilasChametzMGA72Minutes()
    {
        return ToZone(GetSofZmanAchilasChametz(GetAlos72Minutes(), GetTzais72Minutes(), true));
    }

    /// <summary>
    /// This method returns the latest time one is allowed eating <em>chametz</em> on <em>Erev Pesach</em> according to the opinion
    /// of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos72Zmanis">72 zmaniyos</see> minutes before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is identical to the
    /// <see cref="GetSofZmanTfilaMGA72MinutesZmanis"><em>Sof zman tfilah</em> MGA 72 minutes zmanis</see>. This time is 4 <see cref="GetShaahZmanis72MinutesZmanis"><em>shaos zmaniyos</em></see> (temporal hours) after <see cref="ZmanimCalendar.GetAlos72Minutes">dawn</see> based on the
    /// opinion of the MGA that the day is calculated from a <see cref="GetAlos72Zmanis">dawn</see> of 72 minutes zmanis before sunrise to
    /// <see cref="GetTzais72Zmanis">nightfall</see> of 72 minutes zmanis after sunset. This returns the time of 4 * <see cref="GetShaahZmanis72MinutesZmanis"/> after <see cref="GetAlos72Zmanis">dawn</see>. If it is not <em>erev Pesach</em>, a <c>null</c>
    /// will be
    /// returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest time of eating <em>chametz</em>. If it is not <em>erev Pesach</em> or the
    ///         calculation can't be computed such as in the Arctic Circle where there is at least one day a year when the sun does
    ///         not rise, and one where it does not set, a <c>null</c> will be returned. See detailed explanation on top of
    ///         the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis72MinutesZmanis"/>
    /// <seealso cref="GetAlos72Zmanis"/>
    /// <seealso cref="GetSofZmanTfilaMGA72MinutesZmanis"/>
    /// <seealso cref="ZmanimCalendar.GetSofZmanAchilasChametz"/>
    public virtual DateTimeOffset? GetSofZmanAchilasChametzMGA72MinutesZmanis()
    {
        return ToZone(GetSofZmanAchilasChametz(GetAlos72Zmanis(), GetTzais72Zmanis(), true));
    }

    /// <summary>
    /// This method returns the latest time one is allowed eating <em>chametz</em> on <em>Erev Pesach</em> according to the opinion of
    /// the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="ZmanimCalendar.GetAlos16Point1Degrees">16.1°</see> before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 4 <see cref="GetShaahZmanis16Point1Degrees"/>
    /// after <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/> based on the opinion of the MGA that the day is calculated from dawn to nightfall with
    /// both being 16.1° below sunrise or sunset. This returns the time of 4 <see cref="GetShaahZmanis16Point1Degrees"/> after <see cref="ZmanimCalendar.GetAlos16Point1Degrees">dawn</see>. If it is not <em>erev Pesach</em>, a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest time of eating <em>chametz</em>. If it is not <em>erev Pesach</em> or the
    ///         calculation can't be computed such as northern and southern locations even south of the Arctic Circle and north of
    ///         the Antarctic Circle where the sun may not reach low enough below the horizon for this calculation, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis16Point1Degrees"/>
    /// <seealso cref="ZmanimCalendar.GetAlos16Point1Degrees"/>
    /// <seealso cref="GetSofZmanTfilaMGA16Point1Degrees"/>
    /// <seealso cref="ZmanimCalendar.GetSofZmanAchilasChametz"/>
    public virtual DateTimeOffset? GetSofZmanAchilasChametzMGA16Point1Degrees()
    {
        return ToZone(GetSofZmanAchilasChametz(GetAlos16Point1Degrees(), GetTzais16Point1Degrees(), true));
    }

    /// <summary>
    /// This method returns the latest time for burning <em>chametz</em> on <em>Erev Pesach</em> according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. This returns the time 5 * <see cref="ZmanimCalendar.GetShaahZmanisGRA"/> after
    /// <see cref="AstronomicalCalendar.GetSeaLevelSunrise"/>. If it is not  <em>erev Pesach</em>, a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest time for burning <em>chametz</em> on <em>Erev Pesach</em>. If it is not
    ///         <em>erev Pesach</em> or the calculation can't be computed such as in the Arctic Circle where there is at least
    ///         one day a year when the sun does not rise, and one where it does not set, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetShaahZmanisGRA"/>
    /// <seealso cref="ZmanimCalendar.GetSofZmanBiurChametz"/>
    public virtual DateTimeOffset? GetSofZmanBiurChametzGRA()
    {
        return ToZone(GetSofZmanBiurChametz(GetSunriseBasedOnElevationSetting(), GetSunsetBasedOnElevationSetting(), true));
    }

    /// <summary>
    /// This method returns the latest time for burning <em>chametz</em> on <em>Erev Pesach</em> according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="ZmanimCalendar.GetAlos72Minutes">72</see> minutes before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 5 <see cref="ZmanimCalendar.GetShaahZmanis72Minutes"><em>shaos zmaniyos</em></see> (temporal hours) after <see cref="ZmanimCalendar.GetAlos72Minutes">dawn</see> based on the
    /// opinion of the MGA that the day is calculated from a <see cref="ZmanimCalendar.GetAlos72Minutes">dawn</see> of 72 minutes before sunrise to <see cref="ZmanimCalendar.GetTzais72Minutes">nightfall</see> of 72 minutes after sunset. This returns the time of 5 * <see cref="ZmanimCalendar.GetShaahZmanis72Minutes"/>
    /// after <see cref="ZmanimCalendar.GetAlos72Minutes">dawn</see>. If it is not  <em>erev Pesach</em>, a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest time for burning <em>chametz</em> on <em>Erev Pesach</em>. If it is not
    ///         <em>erev Pesach</em> or the calculation can't be computed such as in the Arctic Circle where there is at
    ///         least one day a year when the sun does not rise, and one where it does not set, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetShaahZmanis72Minutes"/>
    /// <seealso cref="ZmanimCalendar.GetAlos72Minutes"/>
    /// <seealso cref="ZmanimCalendar.GetSofZmanBiurChametz"/>
    public virtual DateTimeOffset? GetSofZmanBiurChametzMGA72Minutes()
    {
        return ToZone(GetSofZmanBiurChametz(GetAlos72Minutes(), GetTzais72Minutes(), true));
    }

    /// <summary>
    /// This method returns the latest time for burning <em>chametz</em> on <em>Erev Pesach</em> according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos72Zmanis">72</see> minutes zmanis before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 5 <see cref="GetShaahZmanis72MinutesZmanis"><em>shaos zmaniyos</em></see> (temporal hours) after <see cref="GetAlos72Zmanis">dawn</see> based on the
    /// opinion of the MGA that the day is calculated from a <see cref="GetAlos72Zmanis">dawn</see> of 72 minutes zmanis before sunrise to
    /// <see cref="GetTzais72Zmanis">nightfall</see> of 72 minutes zmanis after sunset. This returns the time of 5 * <see cref="GetShaahZmanis72MinutesZmanis"/> after <see cref="GetAlos72Zmanis">dawn</see>. If it is not  <em>erev Pesach</em>, a
    /// <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest time for burning <em>chametz</em> on <em>Erev Pesach</em>. If it is not
    ///         <em>erev Pesach</em> or the calculation can't be computed such as in the Arctic Circle where there is at
    ///         least one day a year when the sun does not rise, and one where it does not set, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis72MinutesZmanis"/>
    /// <seealso cref="GetAlos72Zmanis"/>
    /// <seealso cref="ZmanimCalendar.GetSofZmanBiurChametz"/>
    public virtual DateTimeOffset? GetSofZmanBiurChametzMGA72MinutesZmanis()
    {
        return ToZone(GetSofZmanBiurChametz(GetAlos72Zmanis(), GetTzais72Zmanis(), true));
    }

    /// <summary>
    /// This method returns the latest time for burning <em>chametz</em> on <em>Erev Pesach</em> according to the opinion
    /// of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em>
    /// being <see cref="ZmanimCalendar.GetAlos16Point1Degrees">16.1°</see> before <see cref="AstronomicalCalendar.GetSunrise"/>. This time is 5
    /// <see cref="GetShaahZmanis16Point1Degrees"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="ZmanimCalendar.GetAlos16Point1Degrees">dawn</see> based on the opinion of the MGA that the day is calculated from dawn to nightfall with both being 16.1°
    /// below sunrise or sunset. This returns the time of 5 <see cref="GetShaahZmanis16Point1Degrees"/> after
    /// <see cref="ZmanimCalendar.GetAlos16Point1Degrees">dawn</see>. If it is not  <em>erev Pesach</em>, a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest time for burning <em>chametz</em> on <em>Erev Pesach</em>. If it is not
    ///         <em>erev Pesach</em> or the calculation can't be computed such as northern and southern locations even south
    ///         of the Arctic Circle and north of the Antarctic Circle where the sun may not reach low enough below the
    ///         horizon for this calculation, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanis16Point1Degrees"/>
    /// <seealso cref="ZmanimCalendar.GetAlos16Point1Degrees"/>
    /// <seealso cref="ZmanimCalendar.GetSofZmanBiurChametz"/>
    public virtual DateTimeOffset? GetSofZmanBiurChametzMGA16Point1Degrees()
    {
        return ToZone(GetSofZmanBiurChametz(GetAlos16Point1Degrees(), GetTzais16Point1Degrees(), true));
    }

    /// <summary>
    /// A method that returns the <a href="https://en.wikipedia.org/wiki/Shneur_Zalman_of_Liadi">Baal Hatanya</a>'s a <em>shaah
    /// zmanis</em> (<see cref="AstronomicalCalendar.GetTemporalHour()">temporal hour</see>). This forms the base for the Baal Hatanya's day based
    /// calculations that are calculated as a 1.583° dip below the horizon after sunset. According to the Baal Hatanya, <em>shkiah
    /// amiti</em>, true (halachic) sunset, is when the top of the sun's disk disappears from view at an elevation similar to the
    /// mountains of Eretz Yisrael. This time is calculated as the point at which the center of the sun's disk is 1.583° below the
    /// horizon. A method that returns a <em>shaah zmanis</em> (<see cref="AstronomicalCalendar.GetTemporalHour()">temporal hour</see>) calculated
    /// based on the <a href="https://en.wikipedia.org/wiki/Shneur_Zalman_of_Liadi">Baal Hatanya</a>'s <em>netz amiti</em> and
    /// <em>shkiah amiti</em> using a dip of 1.583° below the sea level horizon. This calculation divides the day based on the opinion
    /// of the Baal Hatanya that the day runs from <see cref="ZmanimCalendar.GetSunriseBaalHatanya">netz amiti</see> to <see cref="ZmanimCalendar.GetSunsetBaalHatanya"><em>shkiah amiti</em></see>. The calculations are based on a day from <see cref="ZmanimCalendar.GetSunriseBaalHatanya">sea level <em>netz amiti</em></see>
    /// to <see cref="ZmanimCalendar.GetSunsetBaalHatanya">sea level <em>shkiah amiti</em></see>. The day is split into 12 equal parts with each one being a
    /// <em>shaah zmanis</em>. This method is similar to <see cref="AstronomicalCalendar.GetTemporalHour()"/>, but all calculations are based on a sea level
    /// sunrise and sunset.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em> calculated from <see cref="ZmanimCalendar.GetSunriseBaalHatanya"><em>netz
    ///         amiti</em> (sunrise)</see> to <see cref="ZmanimCalendar.GetSunsetBaalHatanya"><em>shkiah amiti</em> ("real" sunset)</see>. If the calculation
    ///         can't be computed such as in the Arctic Circle where there is at least one day a year when the sun does not rise, and
    ///         one where it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="AstronomicalCalendar.GetTemporalHour()"/>
    /// <seealso cref="ZmanimCalendar.GetSunriseBaalHatanya"/>
    /// <seealso cref="ZmanimCalendar.GetSunsetBaalHatanya"/>
    /// <seealso cref="ZmanimCalendar.Zenith1Point583"/>
    public virtual TimeSpan? GetShaahZmanisBaalHatanya()
    {
        return GetTemporalHour(GetSunriseBaalHatanya(), GetSunsetBaalHatanya());
    }

    /// <summary>
    /// Returns the <a href="https://en.wikipedia.org/wiki/Shneur_Zalman_of_Liadi">Baal Hatanya</a>'s <em>alos</em> (dawn) calculated
    /// as the time when the sun is 16.9° below the eastern <see cref="AstronomicalCalendar.GeometricZenith">geometric horizon</see> before
    /// <see cref="AstronomicalCalendar.GetSunrise"/>. It is based on the calculation that the time between dawn and <em>netz amiti</em> (sunrise) is 72
    /// minutes, the time that is takes to walk 4 mil at 18 minutes a mil (<a href="https://en.wikipedia.org/wiki/Maimonides"
    /// >Rambam</a> and others). The sun's position at 72 minutes before <see cref="ZmanimCalendar.GetSunriseBaalHatanya"><em>netz amiti</em> (sunrise)</see>
    /// in Jerusalem <a href="https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox /
    /// equilux</a> is 16.9° below <see cref="AstronomicalCalendar.GeometricZenith"/>.
    /// </summary>
    /// <returns>The <see cref="DateTimeOffset"/> of dawn. If the calculation can't be computed such as northern and southern
    ///         locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may not reach
    ///         low enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual DateTimeOffset? GetAlosBaalHatanya()
    {
        return ToZone(GetSunriseOffsetByDegrees(Zenith16Point9));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite Shema in the morning). This time is 3 <see cref="GetShaahZmanisBaalHatanya"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="ZmanimCalendar.GetSunriseBaalHatanya"><em>netz amiti</em>
    /// (sunrise)</see> based on the opinion of the Baal Hatanya that the day is calculated from sunrise to sunset. This returns the time
    /// 3 * <see cref="GetShaahZmanisBaalHatanya"/> after <see cref="ZmanimCalendar.GetSunriseBaalHatanya"><em>netz amiti</em> (sunrise)</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman shema</em> according to the Baal Hatanya. If the calculation
    ///         can't be computed such as in the Arctic Circle where there is at least one day a year when the sun does
    ///         not rise, and one where it does not set, a <c>null</c> will be returned. See detailed explanation on
    ///         top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetSofZmanShma(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="GetShaahZmanisBaalHatanya"/>
    public virtual DateTimeOffset? GetSofZmanShmaBaalHatanya()
    {
        return ToZone(GetSofZmanShma(GetSunriseBaalHatanya(), GetSunsetBaalHatanya(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfilah</em> (time to recite the morning prayers). This time is 4 hours into the day
    /// based on the opinion of the Baal Hatanya that the day is calculated from sunrise to sunset. This returns the time 4 *
    /// <see cref="GetShaahZmanisBaalHatanya"/> after <see cref="ZmanimCalendar.GetSunriseBaalHatanya"><em>netz amiti</em> (sunrise)</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfilah</em>. If the calculation can't be computed such as in
    ///         the Arctic Circle where there is at least one day a year when the sun does not rise, and one where it does
    ///         not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetSofZmanTfila(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="GetShaahZmanisBaalHatanya"/>
    public virtual DateTimeOffset? GetSofZmanTfilaBaalHatanya()
    {
        return ToZone(GetSofZmanTfila(GetSunriseBaalHatanya(), GetSunsetBaalHatanya(), true));
    }

    /// <summary>
    /// This method returns the latest time one is allowed eating <em>chametz</em> on <em>Erev Pesach</em> according to the opinion
    /// of the Baal Hatanya. This time is identical to the <see cref="GetSofZmanTfilaBaalHatanya"><em>Sof zman tfilah</em> Baal Hatanya</see>.
    /// This time is 4 hours into the day based on the opinion of the Baal Hatanya that the day is calculated from sunrise to sunset.
    /// This returns the time 4 <see cref="GetShaahZmanisBaalHatanya"/> after <see cref="ZmanimCalendar.GetSunriseBaalHatanya"><em>netz amiti</em>
    /// (sunrise)</see>. If it is not  <em>erev Pesach</em>, a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> one is allowed eating <em>chametz</em> on <em>Erev Pesach</em>. If it is not <em>erev
    ///         Pesach</em> or the  calculation can't be computed such as in the Arctic Circle where there is at least one
    ///         day a year when the sun does not rise, and one where it does not set, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanisBaalHatanya"/>
    /// <seealso cref="GetSofZmanTfilaBaalHatanya"/>
    /// <seealso cref="ZmanimCalendar.GetSofZmanAchilasChametz"/>
    public virtual DateTimeOffset? GetSofZmanAchilasChametzBaalHatanya()
    {
        return ToZone(GetSofZmanAchilasChametz(GetSunriseBaalHatanya(), GetSunsetBaalHatanya(), true));
    }

    /// <summary>
    /// This method returns the latest time for burning <em>chametz</em> on <em>Erev Pesach</em> according to the opinion of the Baal
    /// Hatanya. This time is 5 hours into the day based on the opinion of the Baal Hatanya that the day is calculated from sunrise
    /// to sunset. This returns the time 5 * <see cref="GetShaahZmanisBaalHatanya"/> after <see cref="ZmanimCalendar.GetSunriseBaalHatanya"><em>netz
    /// amiti</em> (sunrise)</see>. If it is not  <em>erev Pesach</em>, a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest time for burning <em>chametz</em> on <em>Erev Pesach</em>.  If it is not
    ///         <em>erev Pesach</em> or the  calculation can't be computed such as in the Arctic Circle where there is at
    ///         least one day a year when the sun does not rise, and one where it does not set, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetShaahZmanisBaalHatanya"/>
    /// <seealso cref="ZmanimCalendar.GetSofZmanBiurChametz"/>
    public virtual DateTimeOffset? GetSofZmanBiurChametzBaalHatanya()
    {
        return ToZone(GetSofZmanBiurChametz(GetSunriseBaalHatanya(), GetSunsetBaalHatanya(), true));
    }

    /// <summary>
    /// This method returns the time of <em>mincha gedola</em>. <em>Mincha gedola</em> is the earliest time one can pray
    /// <em>mincha</em>. The <a href="https://en.wikipedia.org/wiki/Maimonides">Rambam</a> is of the opinion that it is
    /// better to delay <em>mincha</em> until <see cref="GetMinchaKetanaBaalHatanya"><em>mincha ketana</em></see> while the
    /// <a href="https://en.wikipedia.org/wiki/Asher_ben_Jehiel">Ra"sh</a>,
    /// <a href="https://en.wikipedia.org/wiki/Jacob_ben_Asher">Tur</a>, <a href=
    /// "https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a> and others are of the opinion that <em>mincha</em> can be prayed
    /// <em>lechatchila</em> starting at <em>mincha gedola</em>. This is calculated as 6.5 <see cref="GetShaahZmanisBaalHatanya">sea level solar hours</see> after <see cref="ZmanimCalendar.GetSunriseBaalHatanya"><em>netz amiti</em> (sunrise)</see>. This calculation is based
    /// on the opinion of the Baal Hatanya that the day is calculated from sunrise to sunset. This returns the time 6.5
    /// * <see cref="GetShaahZmanisBaalHatanya"/> after <see cref="ZmanimCalendar.GetSunriseBaalHatanya"><em>netz amiti</em> ("real" sunrise)</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>mincha gedola</em> according to the Baal Hatanya. If the calculation
    ///         can't be computed such as in the Arctic Circle where there is at least one day a year when the sun does not rise,
    ///         and one where it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>To do:</b> Consider adjusting this to calculate the time as 30 clock or <em>zmaniyos </em> minutes after either <see cref="AstronomicalCalendar.GetSunTransit()">astronomical <em>chatzos</em></see> or <see cref="ZmanimCalendar.GetChatzosHayomAsHalfDay"><em>chatzos</em> as half a
    ///         day</see> for <see cref="AstronomicalCalculator">calculators</see> that support it, based on <see cref="ZmanimCalendar.UseAstronomicalChatzos"/>.
    /// </remarks>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedola(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="GetShaahZmanisBaalHatanya"/>
    /// <seealso cref="GetMinchaKetanaBaalHatanya"/>
    public virtual DateTimeOffset? GetMinchaGedolaBaalHatanya()
    {
        return ToZone(GetMinchaGedola(GetSunriseBaalHatanya(), GetSunsetBaalHatanya(), true));
    }

    /// <summary>
    /// This method returns the time of <em>mincha ketana</em>. This is the preferred earliest time to pray <em>mincha</em> in the
    /// opinion of the <a href="https://en.wikipedia.org/wiki/Maimonides">Rambam</a> and others. For more information on this see the
    /// documentation on <see cref="GetMinchaGedolaBaalHatanya"><em>mincha gedola</em></see>. This is calculated as 9.5 <see cref="GetShaahZmanisBaalHatanya">sea level solar hours</see> after <see cref="ZmanimCalendar.GetSunriseBaalHatanya"><em>netz amiti</em> (sunrise)</see>.
    /// This calculation is calculated based on the opinion of the Baal Hatanya that the day is calculated from sunrise to sunset.
    /// This returns the time 9.5 * <see cref="GetShaahZmanisBaalHatanya"/> after <see cref="ZmanimCalendar.GetSunriseBaalHatanya"><em>netz amiti</em></see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>mincha ketana</em>. If the calculation can't be computed such as
    ///         in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where it
    ///         does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetMinchaKetana(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="GetShaahZmanisBaalHatanya"/>
    /// <seealso cref="GetMinchaGedolaBaalHatanya"/>
    public virtual DateTimeOffset? GetMinchaKetanaBaalHatanya()
    {
        return ToZone(GetMinchaKetana(GetSunriseBaalHatanya(), GetSunsetBaalHatanya(), true));
    }

    /// <summary>
    /// This method returns the time of <em>plag hamincha</em>. This is calculated as 10.75 hours after sunrise. This calculation
    /// is based on the opinion of the Baal Hatanya that the day is calculated from sunrise to sunset. This returns the time
    /// 10.75 * <see cref="GetShaahZmanisBaalHatanya"/> after <see cref="ZmanimCalendar.GetSunriseBaalHatanya"><em>netz amiti</em> (sunrise)</see>. See
    /// <a href="https://www.chabad.org/library/article_cdo/aid/3209349/jewish/About-Our-Zmanim-Calculations.htm">About Our
    /// <em>Zmanim</em> Calculations @ Chabad.org</a> for more details on this calculation.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as
    ///         in the Arctic Circle where there is at least one day a year when the sun does not rise, and one where it
    ///         does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetPlagHamincha(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    public virtual DateTimeOffset? GetPlagHaminchaBaalHatanya()
    {
        return ToZone(GetPlagHamincha(GetSunriseBaalHatanya(), GetSunsetBaalHatanya(), true));
    }

    /// <summary>
    /// A method that returns <em>tzais</em> (nightfall) when the sun is 6° below the western geometric horizon (90°) after <see cref="AstronomicalCalendar.GetSunset">sunset</see>.  This <em>tzais</em> / nightfall based on the opinion of the  <a href=
    /// "https://en.wikipedia.org/wiki/Shneur_Zalman_of_Liadi">Baal Hatanya</a>. This calculation is based on the position of the sun
    /// about 24 minutes after <see cref="AstronomicalCalendar.GetSeaLevelSunset">sunset</see> in Jerusalem <a href=
    /// "https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a>, which is 6°
    /// below <see cref="AstronomicalCalendar.GeometricZenith"/>. See <a href=
    /// "https://www.chabad.org/library/article_cdo/aid/3209349/jewish/About-Our-Zmanim-Calculations.htm">About Our <em>Zmanim</em>
    /// Calculations @ Chabad.org</a> that is based on <see cref="ZmanimCalendar.GetSunsetBaalHatanya"><em>shkiah amitis</em> as 1.583° below the
    /// horizon</see> calculated around the equinox / equilux that computes 3.516 minutes after sunset. To this, 18 minutes of 3/4 of a
    /// 24-minute mil and two minutes for <em>bain hashmashos</em> of Rav Yosi is added. This calculation computes the sun being
    /// 5.83° below the horizon (very close to the slightly later <see cref="GetTzaisGeonim5Point95Degrees"/> that was calculated based
    /// on 4 fixed minutes) and it is rounded up to 6°.
    /// </summary>
    /// <returns>The <see cref="DateTimeOffset"/> of nightfall. If the calculation can't be computed such as northern and southern
    ///         locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may not reach
    ///         low enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetTzaisGeonim5Point95Degrees"/>
    /// <seealso cref="ZmanimCalendar.GetSunsetBaalHatanya"/>
    public virtual DateTimeOffset? GetTzaisBaalHatanya()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith6Degrees));
    }

    /// <summary>
    /// This method returns <a href="https://en.wikipedia.org/wiki/Moshe_Feinstein">Rav Moshe Feinstein's</a> opinion of the
    /// calculation of <em>sof zman krias shema</em> (latest time to recite <em>Shema</em> in the morning) according to the
    /// opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> that the
    /// day is calculated from dawn to nightfall, but calculated using the first half of the day only. The half a day starts
    /// at <em>alos</em> defined as <see cref="GetAlos18Degrees">18°</see> and ends at <see cref="GetFixedLocalChatzosHayom">fixed local
    /// chatzos</see>. <em>Sof Zman Shema</em> is 3 <em>shaos zmaniyos</em> (solar hours) after <em>alos</em> or half of this half-day.
    /// If fixed local <em>chatzos</em> is before sunrise or after sunset (on very short polar days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos18Degrees"/>
    /// <seealso cref="GetFixedLocalChatzosHayom"/>
    /// <seealso cref="ZmanimCalendar.GetHalfDayBasedZman"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA18DegreesToFixedLocalChatzos()
    {
        return ToZone(GetHalfDayBasedZman(GetAlos18Degrees(), GetFixedLocalChatzosHayom(), 3));
    }

    /// <summary>
    /// This method returns <a href="https://en.wikipedia.org/wiki/Moshe_Feinstein">Rav Moshe Feinstein's</a> opinion of the
    /// calculation of <em>sof zman krias shema</em> (latest time to recite <em>Shema</em> in the morning) according to the
    /// opinion of the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> that the day is calculated
    /// from dawn to nightfall, but calculated using the first half of the day only. The half a day starts at <em>alos</em> defined
    /// as <see cref="ZmanimCalendar.GetAlos16Point1Degrees">16.1°</see> and ends at <see cref="GetFixedLocalChatzosHayom">fixed local chatzos</see>. <em>Sof Zman
    /// Shema</em> is 3 <em>shaos zmaniyos</em> (solar hours) after this <em>alos</em> or half of this half-day. If fixed local
    /// <em>chatzos</em> is before sunrise or after sunset (on very short polar days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetAlos16Point1Degrees"/>
    /// <seealso cref="GetFixedLocalChatzosHayom"/>
    /// <seealso cref="ZmanimCalendar.GetHalfDayBasedZman"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA16Point1DegreesToFixedLocalChatzos()
    {
        return ToZone(GetHalfDayBasedZman(GetAlos16Point1Degrees(), GetFixedLocalChatzosHayom(), 3));
    }

    /// <summary>
    /// This method returns <a href="https://en.wikipedia.org/wiki/Moshe_Feinstein">Rav Moshe Feinstein's</a> opinion of the
    /// calculation of <em>sof zman krias shema</em> (latest time to recite <em>Shema</em> in the morning) according to the opinion of
    /// the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> that the day is calculated from dawn to
    /// nightfall, but calculated using the first half of the day only. The half a day starts at <em>alos</em> defined as <see cref="GetAlos90Minutes">90 minutes before sunrise</see> and ends at <see cref="GetFixedLocalChatzosHayom">fixed local chatzos</see>. <em>Sof
    /// Zman Shema</em> is 3 <em>shaos zmaniyos</em> (solar hours) after this <em>alos</em> or half of this half-day. If fixed local
    /// <em>chatzos</em> is before sunrise or after sunset (on very short polar days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetAlos90Minutes"/>
    /// <seealso cref="GetFixedLocalChatzosHayom"/>
    /// <seealso cref="ZmanimCalendar.GetHalfDayBasedZman"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA90MinutesToFixedLocalChatzos()
    {
        return ToZone(GetHalfDayBasedZman(GetAlos90Minutes(), GetFixedLocalChatzosHayom(), 3));
    }

    /// <summary>
    /// This method returns <a href="https://en.wikipedia.org/wiki/Moshe_Feinstein">Rav Moshe Feinstein's</a> opinion of the
    /// calculation of <em>sof zman krias shema</em> (latest time to recite <em>Shema</em> in the morning) according to the opinion of
    /// the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> that the day is calculated from dawn to
    /// nightfall, but calculated using the first half of the day only. The half a day starts at <em>alos</em> defined as <see cref="ZmanimCalendar.GetAlos72Minutes">72 minutes before sunrise</see> and ends at <see cref="GetFixedLocalChatzosHayom"/>. <em>Sof Zman Shema</em> is 3
    /// <em>shaos zmaniyos</em> (solar hours) after this <em>alos</em> or half of this half-day. If fixed local <em>chatzos</em> is
    /// before sunrise or after sunset (on very short polar days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetAlos72Minutes"/>
    /// <seealso cref="GetFixedLocalChatzosHayom"/>
    /// <seealso cref="ZmanimCalendar.GetHalfDayBasedZman"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA72MinutesToFixedLocalChatzos()
    {
        return ToZone(GetHalfDayBasedZman(GetAlos72Minutes(), GetFixedLocalChatzosHayom(), 3));
    }

    /// <summary>
    /// This method returns <a href="https://en.wikipedia.org/wiki/Moshe_Feinstein">Rav Moshe Feinstein's</a> opinion of the
    /// calculation of <em>sof zman krias shema</em> (latest time to recite <em>Shema</em> in the morning) according to the opinion
    /// of the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a> that the day is calculated from sunrise to sunset, but
    /// calculated using the first half of the day only. The half a day starts at <see cref="AstronomicalCalendar.GetSunrise"/> and
    /// ends at <see cref="GetFixedLocalChatzosHayom">fixed local chatzos</see>. <em>Sof zman Shema</em> is 3 <em>shaos zmaniyos</em> (solar
    /// hours) after sunrise or half of this half-day. If fixed local <em>chatzos</em> is before sunrise or after sunset (on very
    /// short polar days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="AstronomicalCalendar.GetSunrise"/>
    /// <seealso cref="GetFixedLocalChatzosHayom"/>
    /// <seealso cref="ZmanimCalendar.GetHalfDayBasedZman"/>
    public virtual DateTimeOffset? GetSofZmanShmaGRASunriseToFixedLocalChatzos()
    {
        return ToZone(GetHalfDayBasedZman(GetSunriseBasedOnElevationSetting(), GetFixedLocalChatzosHayom(), 3));
    }

    /// <summary>
    /// This method returns <a href="https://en.wikipedia.org/wiki/Moshe_Feinstein">Rav Moshe Feinstein's</a> opinion of the
    /// calculation of <em>sof zman tfila</em> (<em>zman tfilah</em> (the latest time to recite the morning prayers))
    /// according to the opinion of the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a> that the day is
    /// calculated from sunrise to sunset, but calculated using the first half of the day only. The half a day starts at
    /// <see cref="AstronomicalCalendar.GetSunrise"/> and ends at <see cref="GetFixedLocalChatzosHayom">fixed local chatzos</see>. <em>Sof zman
    /// tefila</em> is 4 <em>shaos zmaniyos</em> (solar hours) after sunrise or 2/3 of this half-day. If fixed local
    /// <em>chatzos</em> is before sunrise or after sunset (on very short polar days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="AstronomicalCalendar.GetSunrise"/>
    /// <seealso cref="GetFixedLocalChatzosHayom"/>
    /// <seealso cref="ZmanimCalendar.GetHalfDayBasedZman"/>
    public virtual DateTimeOffset? GetSofZmanTfilaGRASunriseToFixedLocalChatzos()
    {
        return ToZone(GetHalfDayBasedZman(GetSunriseBasedOnElevationSetting(), GetFixedLocalChatzosHayom(), 4));
    }

    /// <summary>
    /// This method returns <a href="https://en.wikipedia.org/wiki/Moshe_Feinstein">Rav Moshe Feinstein's</a> opinion of
    /// the calculation of <em>mincha gedola</em>, the earliest time one can pray <em>mincha</em> according to the <a href=
    /// "https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a> calculated as 30 minutes after <see cref="GetFixedLocalChatzosHayom">fixed
    /// local chatzos</see>. If fixed local <em>chatzos</em> is before sunrise or after sunset, or the calculated time is after sunset (on
    /// very short polar days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>mincha gedola</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="GetFixedLocalChatzosHayom"/>
    /// <seealso cref="GetMinchaKetanaGRAFixedLocalChatzosToSunset"/>
    public virtual DateTimeOffset? GetMinchaGedolaGRAFixedLocalChatzos30Minutes()
    {
        DateTimeOffset? minchaGedola30 = GetTimeOffset(GetFixedLocalChatzosHayom(), JavaTime.OfMinutes(30));
        DateTimeOffset? sunset = GetSunsetBasedOnElevationSetting();
        if (minchaGedola30 != null && sunset != null && minchaGedola30.IsAfter(sunset))
        { // very short polar day
            return null;
        }
        return ToZone(minchaGedola30);
    }

    /// <summary>
    /// This method returns <a href="https://en.wikipedia.org/wiki/Moshe_Feinstein">Rav Moshe Feinstein's</a> opinion
    /// of the calculation of <em>mincha ketana</em> (the preferred time to recite the <em>mincha prayers</em> according to
    /// the opinion of the <a href="https://en.wikipedia.org/wiki/Maimonides">Rambam</a> and others) calculated according
    /// to the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a> that is 3.5 <em>shaos zmaniyos</em> (solar
    /// hours) after <see cref="GetFixedLocalChatzosHayom">fixed local chatzos</see>. If fixed local <em>chatzos</em> is before sunrise
    /// or after sunset (on very short polar days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>mincha ketana</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetMinchaGedolaGRA"/>
    /// <seealso cref="GetFixedLocalChatzosHayom"/>
    /// <seealso cref="GetMinchaGedolaGRAFixedLocalChatzos30Minutes"/>
    /// <seealso cref="ZmanimCalendar.GetHalfDayBasedZman"/>
    public virtual DateTimeOffset? GetMinchaKetanaGRAFixedLocalChatzosToSunset()
    {
        return ToZone(GetHalfDayBasedZman(GetFixedLocalChatzosHayom(), GetSunsetBasedOnElevationSetting(), 3.5));
    }

    /// <summary>
    /// This method returns <a href="https://en.wikipedia.org/wiki/Moshe_Feinstein">Rav Moshe Feinstein's</a> opinion
    /// of the calculation of <em>plag hamincha</em>. This method returns <em>plag hamincha</em> calculated according to the
    /// <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a> that the day ends at sunset and is 4.75 <em>shaos
    /// zmaniyos</em> (solar hours) after <see cref="GetFixedLocalChatzosHayom">fixed local chatzos</see>. If fixed local <em>chatzos</em>
    /// is before sunrise or after sunset (on very short polar days), a <c>null</c> will be returned.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year when the sun does not rise, and one where it does not set, a
    ///         <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetPlagHaminchaGRA"/>
    /// <seealso cref="GetFixedLocalChatzosHayom"/>
    /// <seealso cref="GetMinchaKetanaGRAFixedLocalChatzosToSunset"/>
    /// <seealso cref="GetMinchaGedolaGRAFixedLocalChatzos30Minutes"/>
    /// <seealso cref="ZmanimCalendar.GetHalfDayBasedZman"/>
    public virtual DateTimeOffset? GetPlagHaminchaGRAFixedLocalChatzosToSunset()
    {
        return ToZone(GetHalfDayBasedZman(GetFixedLocalChatzosHayom(), GetSunsetBasedOnElevationSetting(), 4.75));
    }

    /// <summary>
    /// Method to return <em>tzais</em> (dusk) calculated as 50 minutes after <see cref="AstronomicalCalendar.GetSunset">sunset</see> or <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> (depending on the <see cref="ZmanimCalendar.UseElevation"/> setting). This method returns
    /// <em>tzais</em> (nightfall) based on the opinion of Rabbi Moshe Feinstein for the New York area. This time should
    /// not be used for latitudes other than ones similar to the latitude of the NY area.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic Circle where
    ///         there is at least one day a year when the sun does not rise, and one where it does not set, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual DateTimeOffset? GetTzais50Minutes()
    {
        return ToZone(GetTimeOffset(GetSunsetBasedOnElevationSetting(), JavaTime.OfMinutes(50)));
    }

    /// <summary>
    /// A method for calculating <em>samuch lemincha ketana</em>, / near <em>mincha ketana</em> time that is half an hour before
    /// <see cref="ZmanimCalendar.GetMinchaKetanaGRA"/> or is 9 * <em>shaos zmaniyos</em> (solar hours) after the start of
    /// the day, calculated according to the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a> using a day starting at
    /// sunrise and ending at sunset. This is the time that eating or other activity can't begin prior to praying <em>mincha</em>.
    /// The calculation used is 9 * <see cref="ZmanimCalendar.GetShaahZmanisGRA"/> after <see cref="AstronomicalCalendar.GetSunrise"/> or <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting">elevation adjusted sunrise</see> (depending on the <see cref="ZmanimCalendar.UseElevation"/> setting). See the
    /// <a href="https://hebrewbooks.org/pdfpager.aspx?req=60387&amp;st=&amp;pgnum=294">Mechaber and Mishna Berurah 232</a> and <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=60388&amp;pgnum=34">249:2</a>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>samuch lemincha ketana</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be
    ///         returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetShaahZmanisGRA"/>
    /// <seealso cref="ZmanimCalendar.GetSamuchLeMinchaKetana(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="ZmanimCalendar.UseAstronomicalChatzosForOtherZmanim"/>
    public virtual DateTimeOffset? GetSamuchLeMinchaKetanaGRA()
    {
        return ToZone(GetSamuchLeMinchaKetana(GetSunriseBasedOnElevationSetting(), GetSunsetBasedOnElevationSetting(), true));
    }

    /// <summary>
    /// A method for calculating <em>samuch lemincha ketana</em>, / near <em>mincha ketana</em> time that is half an hour before
    /// <see cref="ZmanimCalendar.GetMinchaKetanaGRA"/> or is 9 * <em>shaos zmaniyos</em> (solar hours) after the start of the day, calculated based
    /// on a day from   and ending  a day starting at <see cref="GetMinchaGedola16Point1Degrees"><em>alos</em> 16.1°</see> and ending
    /// at <see cref="ZmanimCalendar.GetTzais72Minutes"><em>tzais</em> 16.1°</see>. This is the time that eating or other activity can't begin prior to
    /// praying <em>mincha</em>.  The calculation used is 9 * <see cref="GetShaahZmanis16Point1Degrees"/> after <see cref="ZmanimCalendar.GetAlos16Point1Degrees"><em>alos</em> 16.1°</see>. See the <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=60387&amp;st=&amp;pgnum=294">Mechaber and Mishna Berurah 232</a> and <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=60388&amp;pgnum=34">249:2</a>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>samuch lemincha ketana</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetSamuchLeMinchaKetana(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="GetShaahZmanis16Point1Degrees"/>
    public virtual DateTimeOffset? GetSamuchLeMinchaKetana16Point1Degrees()
    {
        return ToZone(GetSamuchLeMinchaKetana(GetAlos16Point1Degrees(), GetTzais16Point1Degrees(), true));
    }

    /// <summary>
    /// A method for calculating <em>samuch lemincha ketana</em>, / near <em>mincha ketana</em> time that is half an hour before
    /// <see cref="ZmanimCalendar.GetMinchaKetanaGRA"/> or is 9 * <em>shaos zmaniyos</em> (solar hours) after the start of the day, calculated based
    /// on a day from   and ending  a day starting at <see cref="ZmanimCalendar.GetAlos72Minutes"><em>alos</em> 72 minutes</see> and ending at <see cref="ZmanimCalendar.GetTzais72Minutes"><em>tzais</em> 72 minutes</see>. This is the time that eating or other activity can't begin prior to praying
    /// <em>mincha</em>. The calculation used is 9 * <see cref="ZmanimCalendar.GetShaahZmanis72Minutes"/> after <see cref="ZmanimCalendar.GetAlos72Minutes"><em>alos</em>
    /// 72 minutes</see>. See the <a href="https://hebrewbooks.org/pdfpager.aspx?req=60387&amp;st=&amp;pgnum=294">Mechaber and Mishna Berurah
    /// 232</a> and <a href="https://hebrewbooks.org/pdfpager.aspx?req=60388&amp;pgnum=34">249:2</a>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>samuch lemincha ketana</em>. If the calculation can't be computed such
    ///         as northern and southern locations even south of the Arctic Circle and north of the Antarctic Circle
    ///         where the sun may not reach low enough below the horizon for this calculation, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ZmanimCalendar.GetSamuchLeMinchaKetana(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="ZmanimCalendar.GetShaahZmanis72Minutes"/>
    public virtual DateTimeOffset? GetSamuchLeMinchaKetana72Minutes()
    {
        return ToZone(GetSamuchLeMinchaKetana(GetAlos72Minutes(), GetTzais72Minutes(), true));
    }

    /// <summary>
    /// A method that returns sunset calculated as the time when the sun is directly due west (azimuth 270°) in <a href=
    /// "https://en.wikipedia.org/wiki/Polar_regions_of_Earth">Polar regions</a> on days that there are no <see cref="ZmanimCalendar.GetSunsetBasedOnElevationSetting">sunset</see>. If there is sunset that day, a <c>null</c> will be returned.
    /// <para>In Polar regions (the <a href="https://en.wikipedia.org/wiki/Arctic">Arctic</a> or <a href=
    /// "https://en.wikipedia.org/wiki/Antarctic">Antarctic</a> circles), there are days of no sunrise or sunset, and there are
    /// <em>halachic</em> opinions that during these periods, sunset (the day-night boundary) is reached when the sun is directly due
    /// west (azimuth 270°). Sunrise in this opinion is when the sun is directly due east (azimuth 90°) returned by <see cref="GetPolarSunriseBenIshChai"/>. This is the opinion of <a href="https://en.wikipedia.org/wiki/Joseph_Schwarz_(geographer)">Rabbi Yehosef
    /// Schwarz</a> in his <a href="https://hebrewbooks.org/pdfpager.aspx?req=31703&amp;pgnum=134">דברי יוסף – דרך מבוא השמש</a> and <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=159&amp;pgnum=83">דברי יוסף – תשובות, שאלה ח׳</a>. This is brought down <em>lehalacha</em>
    /// by The <a href="https://en.wikipedia.org/wiki/Yosef_Hayyim">Ben Ish Chai</a> in the <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=1401&amp;pgnum=461">רב פעלים – חלק ב׳, סוד ישרים ס׳ ד׳</a>. This time is close to six hours
    /// after <see cref="AstronomicalCalendar.GetSunTransit()">astronomical <em>chatzos hayom</em></see>, but depending on the time of year and location in the
    ///  Arctic / Antarctic, it can be up to 46 minutes before or after this time.</para>
    /// </summary>
    /// <returns>Sunset calculated as the time the sun is directly due west (azimuth 270°) in Polar regions on days that there is no
    ///         sunset. If there is sunset that day, a <c>null</c> will be returned. While unlikely, it is theoretically possible
    ///         for a <c>null</c> to be returned by the code even in Polar regions.</returns>
    /// <seealso cref="GetPolarSunriseBenIshChai"/>
    /// <seealso cref="AstronomicalCalendar.GetTimeAtAzimuth90Or270"/>
    /// <seealso cref="AstronomicalCalculator.GetTimeAtAzimuth"/>
    public virtual DateTimeOffset? GetPolarSunsetBenIshChai()
    {
        if (GetSunsetBasedOnElevationSetting() == null)
        {
            return ToZone(GetTimeAtAzimuth90Or270(270));
        }
        return null;
    }

    /// <summary>
    /// A method that returns sunrise calculated as the time when the sun is directly due east (azimuth 90°) in <a href=
    /// "https://en.wikipedia.org/wiki/Polar_regions_of_Earth">Polar regions</a> on days that there are no <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting">sunrise</see>. If there is sunrise that day, a <c>null</c> will be returned.
    /// <para>In Polar regions (the <a href="https://en.wikipedia.org/wiki/Arctic">Arctic</a> or <a href=
    /// "https://en.wikipedia.org/wiki/Antarctic">Antarctic</a> circles), there are days of no sunrise or sunset, and there are
    /// <em>halachic</em> opinions that during these periods, sunrise is reached when the sun is directly due east (azimuth 90°).
    /// The day-night boundary (sunset) in these opinions is when the sun is directly due west (azimuth 270°) returned by
    /// <see cref="GetPolarSunsetBenIshChai"/>. This is the opinion of <a href=
    /// "https://en.wikipedia.org/wiki/Joseph_Schwarz_(geographer)">Rabbi Yehosef Schwarz</a> in his <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=31703&amp;pgnum=134">דברי יוסף – דרך מבוא השמש</a> and <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=159&amp;pgnum=83">דברי יוסף – תשובות, שאלה ח׳</a>. This is brought down <em>lehalacha</em>
    /// by The <a href="https://en.wikipedia.org/wiki/Yosef_Hayyim">Ben Ish Chai</a> in the <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=1401&amp;pgnum=461">רב פעלים – חלק ב׳, סוד ישרים ס׳ ד׳</a>. This time is close to six hours
    /// before <see cref="AstronomicalCalendar.GetSunTransit()">astronomical <em>chatzos hayom</em></see>, but depending on the time of year and location in the
    ///  Arctic / Antarctic, it can be up to 46 minutes before or after this time.</para>
    /// <para>All Polar zmanim can be calculated using the Ben Ish Chai's start and end of day using the following sample code.
    /// <code>
    /// var location = new GeoLocation("Thule, Greenland", 79.997, -69.258, 0, TimeZoneInfo.FindSystemTimeZoneById("America/Thule"));
    /// var czc = new ComprehensiveZmanimCalendar(location);
    /// DateTimeOffset? sofZmanKriasShema = czc.GetSofZmanShma(czc.GetPolarSunriseBenIshChai(), czc.GetPolarSunsetBenIshChai(), true);
    /// DateTimeOffset? sofZmanTefila = czc.GetSofZmanTfila(czc.GetPolarSunriseBenIshChai(), czc.GetPolarSunsetBenIshChai(), true);
    /// DateTimeOffset? minchaGedola = czc.GetMinchaGedola(czc.GetPolarSunriseBenIshChai(), czc.GetPolarSunsetBenIshChai(), true);
    /// </code>
    /// Dedicated methods for these zmanim may be added at a future point.</para>
    /// </summary>
    /// <returns>Sunrise calculated as the easternmost solar position (azimuth 90°) in Polar regions on days that there is no sunrise.
    ///         If there is sunrise that day, a <c>null</c> will be returned. While unlikely, it is theoretically possible for a
    ///         <c>null</c> to be returned by the code even in Polar regions.</returns>
    /// <seealso cref="GetPolarSunsetBenIshChai"/>
    /// <seealso cref="AstronomicalCalendar.GetTimeAtAzimuth90Or270"/>
    /// <seealso cref="AstronomicalCalculator.GetTimeAtAzimuth"/>
    public virtual DateTimeOffset? GetPolarSunriseBenIshChai()
    {
        if (GetSunriseBasedOnElevationSetting() == null)
        {
            return ToZone(GetTimeAtAzimuth90Or270(90));
        }
        return null;
    }

    /// <summary>
    /// A method that returns <em>Plag Hamincha</em> in <a href="https://en.wikipedia.org/wiki/Polar_regions_of_Earth">Polar
    /// regions</a> on days that there are no <see cref="ZmanimCalendar.GetSunriseBasedOnElevationSetting">sunrise</see> and <see cref="ZmanimCalendar.GetSunsetBasedOnElevationSetting">sunset</see>, calculated 10.75 <em>shaaos zmaniyos</em> of a day calculated starting at
    /// <see cref="GetPolarSunriseBenIshChai"/> and ending at <see cref="GetPolarSunsetBenIshChai"/>. This is the opinion of <a href=
    /// "https://en.wikipedia.org/wiki/Joseph_Schwarz_(geographer)">Rabbi Yehosef Schwarz</a> in his <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=31703&amp;pgnum=134">דברי יוסף – דרך מבוא השמש</a> and <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=159&amp;pgnum=83">דברי יוסף – תשובות, שאלה ח׳</a>. This is brought down <em>lehalacha</em>
    /// by The <a href="https://en.wikipedia.org/wiki/Yosef_Hayyim">Ben Ish Chai</a> in the <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=1401&amp;pgnum=461">רב פעלים – חלק ב׳, סוד ישרים ס׳ ד׳</a>.
    /// </summary>
    /// <returns><em>Plag Hamincha</em> according to the Ben Ish Chai in Polar regions on days that there is no sunrise or sunset.
    ///         If there is sunrise or sunset that day, a <c>null</c> will be returned.</returns>
    /// <seealso cref="GetPolarSunriseBenIshChai"/>
    /// <seealso cref="GetPolarSunsetBenIshChai"/>
    /// <seealso cref="ZmanimCalendar.GetPlagHamincha(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    public virtual DateTimeOffset? GetPolarPlagHaminchaBenIshChai()
    {
        return ToZone(GetPlagHamincha(GetPolarSunriseBenIshChai(), GetPolarSunsetBenIshChai(), true));
    }

    /// <summary>
    /// In Polar regions during the period where the sun is above the horizon 24-hours a day, <a href=
    /// "https://en.wikipedia.org/wiki/Moshe_Sternbuch">Rav Moshe Sternbuch</a> in <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=19963&amp;st=&amp;pgnum=316">מועדים וזמנים ח״ב ס׳ קנ״ה הע׳ א׳</a> (<span>ח״ד ס׳ ת״א
    /// הע׳ א׳</span> in the new edition) and in תשובות והנהגות <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=20025&amp;st=&amp;pgnum=224">ח״א ס׳ שט״ו,</a> and <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=69067&amp;st=&amp;pgnum=135">ח״ה ס׳ פ״ד</a> ruled that during the Polar summer night-based
    /// <em>mitzvos</em> are not applicable, and during the Polar winter, day-based mitzvos are not applicable. Despite this, the day
    /// of the calendar week changes when the sun is at its lowest position (an instantaneous set and rise) at <see cref="ZmanimCalendar.GetChatzosHalayla"/> in the Polar summer and when it is at its highest position in the Polar winter that occurs at <see cref="ZmanimCalendar.GetChatzosHayom"/>. Rabbi Dovid Heber the author of the <a href="https://hebrewbooks.org/53000">שערי זמנים</a> who <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=53000&amp;st=&amp;pgnum=111">discusses the Moadim Uzmanim's opinion</a> clarified to me
    /// that in the Polar winter this really only applies when there is no <em>alos hashachar</em>. The lack of <em>alos
    /// hashachar</em> only occurs in Antarctica and in uninhabited extreme northern regions. This means almost every northern
    /// inhabited location has an <em>alos hashachar</em> and "day" even on December 21st. This method considers <em>alos</em> to be
    /// <see cref="ZmanimCalendar.GetAlos16Point1Degrees"/>.
    /// <para>It should be noted that in the <span>מועדים וזמנים</span> linked above that was published in 1961, Rav Moshe
    /// Sternbuch mentions that [<span>ולתפלה, היום מתהלק, עד י״ב שעות שחרית,  ומאז מנחה, ומעריב מתפלל סמוך ללילה כרבי יהודה כנל״ד ...</span>].
    /// This comment (in () in the original, and [] in the new edition published in 2023) was not brought down in the
    /// <span>תשובות והנהגות</span> vol. 1 that was published in 1977 nor in vol. 5. published in 2009. His grandson Rav Yonah Sternbuch
    /// confirmed that it is not a retraction. The Moadim Uzmanim lists the end of Shacharis as 12 hours after the start of the day
    /// (either <see cref="ZmanimCalendar.GetChatzosHalayla"/>  in the Polar summer, or <see cref="ZmanimCalendar.GetChatzosHayom"/> in the Polar winter). At this point
    /// the time for <em>mincha</em> starts. The time for <em>maariv</em> in this opinion starts at <see cref="GetPolarPlagHaminchaTeshuvosVehanhagos"/> and ends at the end of the Polar day (the value returned by this method). Regarding
    /// the time for Krias Shema, the מועדים וזמנים states that <span>ורק אני מסתפק בענין ק״ש שתלוי בעצם בשכיבה וקימה ולא ביום ולילה, אולי יש דין ק״ש יום
    /// ולילה גם במקומות אלו...</span>.</para>
    /// <para>FIXME:
    /// <ul>
    ///   <li>Research if an earlier alos such as 18° should be used.</li>
    ///   <li>Consider logic for day with EITHER an <em>alos</em> or a sunset (the day of transit into 24-hour day/night.</li>
    /// </ul></para>
    /// </summary>
    /// <returns>the time of the Jewish calendar transition to the next day in Polar regions according to Rav Moshe Sternbuch.</returns>
    /// <seealso cref="GetPolarPlagHaminchaTeshuvosVehanhagos"/>
    public virtual DateTimeOffset? GetPolarStartOfDayTeshuvosVehanhagos()
    {
        if (GetSunriseBasedOnElevationSetting() == null && GetSunsetBasedOnElevationSetting() == null)
        {
            DateTimeOffset? chatzosHayom = GetChatzosHayom();
            DateTimeOffset? chatzosHalayla = GetChatzosHalayla();
            double chatzosHayomSolarElevation = AstronomicalCalculator.GetSolarElevation(chatzosHayom!.Value, GeoLocation);
            double chatzosHalaylaSolarElevation = AstronomicalCalculator.GetSolarElevation(chatzosHalayla!.Value, GeoLocation);
            double sunriseElevation = AstronomicalCalculator.SolarRadius + AstronomicalCalculator.Refraction;
            if (chatzosHayomSolarElevation < (0 - sunriseElevation) && chatzosHalaylaSolarElevation < (0 - sunriseElevation) && // and the sun is always below the horizon
                    GetAlos16Point1Degrees() == null && GetSunriseBasedOnElevationSetting() == null)
            { // Polar winter in either hemisphere no alos 16.1 or sunrise
                return ToZone(chatzosHayom); // the "sunrise" as the closest to the horizon
            }
            else if (chatzosHayomSolarElevation > (0 - sunriseElevation) && chatzosHalaylaSolarElevation > (0 - sunriseElevation))
            { // Polar summer when both are above the horizon
                return ToZone(chatzosHalayla);
            }
        }
        return null;
    }

    /// <summary>
    /// In Polar regions during the period where the sun is above the horizon 24-hours a day, <a href=
    /// "https://en.wikipedia.org/wiki/Moshe_Sternbuch">Rav Moshe Sternbuch</a> in <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=19963&amp;st=&amp;pgnum=316">מועדים וזמנים ח״ב ס׳ קנ״ה הע׳ א׳</a> (<span>ח״ד ס׳ ת״א
    /// הע׳ א׳</span> in the new edition) mentions the concept of zmanim based on his <see cref="GetPolarStartOfDayTeshuvosVehanhagos"/>.
    /// This calculates <see cref="ZmanimCalendar.GetPlagHamincha(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/> as 10.75 <em>sha'os zmaniyos</em> starting 24 hours prior
    /// to the current start/end of the current day. This will be very close to 2.5 clock hours before the start/end of the day.
    /// </summary>
    /// <returns><em>plag hamincha</em> as 2.5 hours prior to the start of the next day.</returns>
    /// <seealso cref="GetPolarStartOfDayTeshuvosVehanhagos"/>
    /// <seealso cref="ZmanimCalendar.GetPlagHamincha(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    public virtual DateTimeOffset? GetPolarPlagHaminchaTeshuvosVehanhagos()
    {
        DateTimeOffset? polarStartOfDay = GetPolarStartOfDayTeshuvosVehanhagos();
        if (polarStartOfDay == null)
        {
            return null;
        }
        else
        {
            DateTimeOffset? yesterday = polarStartOfDay.Minus(JavaTime.OfDays(1));
            return ToZone(GetPlagHamincha(yesterday, polarStartOfDay, true));
        }

    }

    /// <seealso cref="object.Equals(object)"/>
    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        if (!base.Equals(obj))
        {
            return false;
        }
        ComprehensiveZmanimCalendar that = (ComprehensiveZmanimCalendar)obj;
        return ateretTorahSunsetOffset == that.ateretTorahSunsetOffset;
    }

    /// <seealso cref="object.GetHashCode"/>
    public override int GetHashCode()
    {
        return 37 * base.GetHashCode() + ateretTorahSunsetOffset.GetHashCode();
    }
}
