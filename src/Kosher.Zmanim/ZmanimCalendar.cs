/*
 * Zmanim Java API
 * Copyright © 2004-2026 Eliyahu Hershfeld
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; either version 2.1 of the License.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
 * details.
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not, write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA,
 * or connect to: https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html
 */
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;

namespace Kosher.Zmanim;

/// <summary>
/// The ZmanimCalendar is a specialized calendar that can calculate sunrise, sunset and Jewish <em>zmanim</em> (religious times)
/// for prayers and other Jewish religious duties. This class contains the main functionality of the <em>Zmanim</em> library. For
/// a much more extensive list of <em>zmanim</em>, use the <see cref="ComprehensiveZmanimCalendar"/> that extends this class. See
/// documentation for the <see cref="ComprehensiveZmanimCalendar"/> and <see cref="AstronomicalCalendar"/> for simple examples on using the API.
/// <b>Elevation based <em>zmanim</em> (even sunrise and sunset) should not be used <em>lekula</em> without the guidance
/// of a <em>posek</em></b>. According to Rabbi Dovid Yehudah Bursztyn in his <a href="https://www.worldcat.org/oclc/1158574217"
/// >Zmanim Kehilchasam, 7th edition</a> chapter 2, section 7 (pages 181-182) and section 9 (pages 186-187), no <em>zmanim</em>
/// besides sunrise and sunset should use elevation. However, Rabbi Yechiel Avrahom Zilber in the <a href=
/// "https://hebrewbooks.org/51654">Birur Halacha Vol. 6</a> Ch. 58 Pages <a href=
/// "https://hebrewbooks.org/pdfpager.aspx?req=51654&amp;pgnum=42">34</a> and <a href=
/// "https://hebrewbooks.org/pdfpager.aspx?req=51654&amp;pgnum=50">42</a> is of the opinion that elevation should be accounted for
/// in <em>zmanim</em> calculations. Related to this, Rabbi Yaakov Karp in <a href="https://www.worldcat.org/oclc/919472094"
/// >Shimush Zekeinim</a>, Ch. 1, page 17 states that obstructing horizons should be factored into <em>zmanim</em> calculations. The
/// setting defaults to false (elevation will not be used for <em>zmanim</em> calculations besides sunrise and sunset), unless the
/// setting is changed to true in <see cref="UseElevation"/>. This will impact sunrise and sunset-based <em>zmanim</em> such as
/// <see cref="AstronomicalCalendar.GetSunrise"/>, <see cref="AstronomicalCalendar.GetSunset"/>, <see cref="GetSofZmanShmaGRA"/>, <em>alos</em>-based
/// <em>zmanim</em> such as <see cref="GetSofZmanShmaMGA72Minutes"/> that are based on a fixed offset of sunrise or sunset and
/// <em>zmanim</em> based on a percentage of the day such as <see cref="ComprehensiveZmanimCalendar.GetSofZmanShmaMGA90MinutesZmanis"/>
/// that are based on sunrise and sunset. Even when set to true it will not impact <em>zmanim</em> that are a degree-based offset of
/// sunrise and sunset, such as <see cref="ComprehensiveZmanimCalendar.GetSofZmanShmaMGA16Point1Degrees"/> or <see cref="ComprehensiveZmanimCalendar.GetSofZmanShmaBaalHatanya"/> since these <em>zmanim</em> are not linked to sunrise or sunset times
/// (the calculations are based on the astronomical definition of sunrise and sunset calculated in a vacuum with the solar radius
/// above the horizon), and are therefore not impacted by the use of elevation. For additional information on the <em>halachic</em>
/// impact of elevation on <em>zmanim</em> see:
/// <ul>
/// <li><a href="https://www.nli.org.il/en/books/NNL_ALEPH002542826/NLI">Zmanei Halacha Lema'aseh</a> 4th edition by <a href=
/// "http://beinenu.com/rabbis/%D7%94%D7%A8%D7%91-%D7%99%D7%93%D7%99%D7%93%D7%99%D7%94-%D7%9E%D7%A0%D7%AA">Rabbi Yedidya Manat</a>.
/// See section 1, pages 11-12 for a very concise write-up, with details in section 2, pages 37 - 63 and 133 - 151.</li>
/// <li><a href="https://www.worldcat.org/oclc/1158574217">Zmanim Kehilchasam</a> 7th edition, by Rabbi Dovid Yehuda Burstein,  vol 1,
/// chapter 2, pages 95 - 188.</li>
/// <li><a href="https://www.worldcat.org/oclc/36089452">Hazmanim Bahalacha</a> by Rabbi Chaim Benish , perek 7, pages 53 - 63.</li>
/// </ul>
/// <para><b>Note:</b> It is important to read the technical notes on top of the <see cref="AstronomicalCalculator"/> documentation
/// before using this code.</para>
/// <para>I would like to thank <a href="https://www.worldcat.org/search?q=au%3AShakow%2C+Yaakov">Rabbi Yaakov Shakow</a>, the
/// author of Luach Ikvei Hayom who spent a considerable amount of time reviewing, correcting and making suggestions on the
/// documentation in this library.
/// &lt;h2>Disclaimer:&lt;/h2> I did my best to get accurate results, but please double-check before relying on these
/// <em>zmanim</em> for <em>halacha lema'aseh</em>.</para>
/// </summary>
public class ZmanimCalendar : AstronomicalCalendar
{

    /// <summary>
    /// Is elevation factored in for some <em>zmanim</em> (see <see cref="UseElevation"/> for additional information).
    /// </summary>
    /// <seealso cref="UseElevation"/>
    /// <seealso cref="UseElevation"/>
    private bool useElevation;

    /// <summary>
    /// Is elevation above sea level calculated for times besides sunrise and sunset. According to Rabbi Dovid Yehuda Bursztyn in
    /// his <a href="https://www.worldcat.org/oclc/659793988">Zmanim Kehilchasam (second edition published in 2007)</a> chapter 2
    /// (pages 186-187) no <em>zmanim</em> besides sunrise and sunset should use elevation. However, Rabbi Yechiel Avrahom Zilber
    /// in the <a href="https://hebrewbooks.org/51654">Birur Halacha Vol. 6</a> Ch. 58 Pages <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=51654&amp;pgnum=42">34</a> and <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=51654&amp;pgnum=50">42</a> is of the opinion that elevation should be accounted
    /// for in <em>zmanim</em> calculations. Related to this, Rabbi Yaakov Karp in <a href=
    /// "https://www.worldcat.org/oclc/919472094">Shimush Zekeinim</a>, Ch. 1, page 17 states that obstructing horizons should be
    /// factored into <em>zmanim</em> calculations.The setting defaults to false (elevation will not be used for <em>zmanim</em>
    /// calculations), unless the setting is changed to true in <see cref="UseElevation"/>. This will impact sunrise and
    /// sunset based <em>zmanim</em> such as  <see cref="GetSofZmanShmaGRA"/>, alos based <em>zmanim</em> such as <see cref="GetSofZmanShmaMGA72Minutes"/> that are based on a fixed offset of sunrise or sunset and <em>zmanim</em> based on a
    /// percentage of the day such as <see cref="ComprehensiveZmanimCalendar.GetSofZmanShmaMGA90MinutesZmanis"/> that are based on
    /// sunrise and sunset. It will not impact <em>zmanim</em> that are a degree based offset of sunrise and sunset, such as <see cref="ComprehensiveZmanimCalendar.GetSofZmanShmaMGA16Point1Degrees"/> or <see cref="ComprehensiveZmanimCalendar.GetSofZmanShmaBaalHatanya"/>.
    /// </summary>
    /// <value>set to true to use elevation in <em>zmanim</em> calculations</value>
    /// <remarks>
    /// Sets whether elevation above sea level is factored into <em>zmanim</em> calculations for times besides sunrise and sunset.
    /// See <see cref="UseElevation"/> for more details.
    /// </remarks>
    /// <seealso cref="UseElevation"/>
    public bool UseElevation
    {
        get => useElevation;
        set => useElevation = value;
    }


    /// <summary>
    /// Is astronomical <em>chatzos</em> used for <em>zmanim</em> calculations. The default value of <c>true</c> will
    /// keep the standard astronomical <em>chatzos</em> calculation, while setting it to <c>false</c> will use half of
    /// a solar day / night calculation for <em>chatzos</em>.
    /// </summary>
    /// <seealso cref="UseAstronomicalChatzos"/>
    /// <seealso cref="UseAstronomicalChatzosForOtherZmanim"/>
    /// <seealso cref="UseAstronomicalChatzos"/>
    /// <seealso cref="GetChatzosHayom"/>
    /// <seealso cref="GetChatzosHalayla"/>
    /// <seealso cref="AstronomicalCalendar.GetSunTransit()"/>
    /// <seealso cref="GetChatzosHayomAsHalfDay"/>
    private bool useAstronomicalChatzos = true;

    /// <summary>
    /// Is <see cref="AstronomicalCalendar.GetSunTransit()">astronomical <em>chatzos</em></see> used for <see cref="GetChatzosHayom"/> and <see cref="GetChatzosHalayla"/>
    /// for enhanced accuracy. For example as the day lengthens, the second half of the day is longer than the first and astronomical
    /// <em>chatzos</em> would be a drop earlier than half of the time between sunrise and sunset.
    /// </summary>
    /// <value>set to true to use astronomical in <em>chatzos</em> in <em>zmanim</em> calculations.</value>
    /// <remarks>
    /// Sets if astronomical <em>chatzos</em> should be used in calculations of other <em>zmanim</em> for enhanced accuracy.
    /// </remarks>
    /// <seealso cref="UseAstronomicalChatzos"/>
    /// <seealso cref="GetChatzosHayom"/>
    /// <seealso cref="GetChatzosHalayla"/>
    /// <seealso cref="AstronomicalCalendar.GetSunTransit()"/>
    /// <seealso cref="GetChatzosHayomAsHalfDay"/>
    /// <seealso cref="UseAstronomicalChatzosForOtherZmanim"/>
    public bool UseAstronomicalChatzos
    {
        get => useAstronomicalChatzos;
        set => useAstronomicalChatzos = value;
    }


    /// <summary>
    /// Is astronomical <em>chatzos</em> used for <em>zmanim</em> calculations besides <em>chatzos</em> itself for enhanced
    /// accuracy. The default value of <c>false</c> will keep the standard start to end of day calculations, while setting
    /// it to <c>true</c> will use half of a solar day calculation for <em>zmanim</em>.
    /// </summary>
    /// <seealso cref="UseAstronomicalChatzosForOtherZmanim"/>
    /// <seealso cref="UseAstronomicalChatzosForOtherZmanim"/>
    /// <seealso cref="UseAstronomicalChatzos"/>
    /// <seealso cref="UseAstronomicalChatzos"/>
    /// <seealso cref="GetChatzosHayom"/>
    /// <seealso cref="GetChatzosHalayla"/>
    private bool useAstronomicalChatzosForOtherZmanim = false;

    /// <summary>
    /// Is astronomical <em>chatzos</em> used for <em>zmanim</em> calculations besides <em>chatzos</em> itself for enhanced
    /// accuracy. For example as the day is lengthening (as we approach spring season), the second half of the day is longer than
    /// the first and astronomical <em>chatzos</em> would be a drop earlier than half of the time between sunrise and sunset.
    /// Conversely, the second half of the day would be shorter in the autumn as the days start getting shorter.
    /// </summary>
    /// <value>set to true to use astronomical in <em>chatzos</em> in <em>zmanim</em> calculations.</value>
    /// <remarks>
    /// Sets if astronomical <em>chatzos</em> should be used in calculations of other <em>zmanim</em> for enhanced accuracy.
    /// </remarks>
    /// <seealso cref="UseAstronomicalChatzosForOtherZmanim"/>
    /// <seealso cref="UseAstronomicalChatzos"/>
    public bool UseAstronomicalChatzosForOtherZmanim
    {
        get => useAstronomicalChatzosForOtherZmanim;
        set => useAstronomicalChatzosForOtherZmanim = value;
    }

    /// <summary>
    /// The zenith of 16.1° below geometric zenith (90°).
    /// </summary>
    /// <seealso cref="GetAlos16Point1Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetTzais16Point1Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSofZmanShmaMGA16Point1Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSofZmanTfilaMGA16Point1Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetMinchaGedola16Point1Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetMinchaKetana16Point1Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetPlagHamincha16Point1Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSofZmanShmaAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetPlagAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSofZmanShmaAlos16Point1ToSunset"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetShaahZmanis16Point1Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point7Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetShaahZmanisAlos16Point1DegreesToTzaisGeonim3Point8Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetShaahZmanisAlos16Point1DegreesToTzaisGeonim7Point083Degrees"/>
    protected const double Zenith16Point1 = GeometricZenith + 16.1;

    /// <summary>
    /// The zenith of 8.5° below geometric zenith (90°).
    /// </summary>
    /// <seealso cref="GetTzaisGeonim8Point5Degrees"/>
    protected const double Zenith8Point5 = GeometricZenith + 8.5;

    /// <summary>
    /// The zenith of 1.583° below <see cref="AstronomicalCalendar.GeometricZenith"/> (90°).
    /// </summary>
    /// <seealso cref="GetSunriseBaalHatanya"/>
    /// <seealso cref="GetSunsetBaalHatanya"/>
    protected const double Zenith1Point583 = GeometricZenith + 1.583;

    /// <summary>
    /// The default <em>Shabbos</em> candle lighting offset is 18 minutes. This can be changed via the
    /// <see cref="CandleLightingOffset"/> and retrieved by the <see cref="CandleLightingOffset"/>.
    /// </summary>
    private double candleLightingOffset = 18;

    /// <summary>
    /// This method will return <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> if <see cref="UseElevation"/> is false (the default),
    /// or elevation adjusted <see cref="AstronomicalCalendar.GetSunrise"/> if it is true. This allows relevant <em>zmanim</em> in this and
    /// extending classes (such as the <see cref="ComprehensiveZmanimCalendar"/>) to automatically adjust to the elevation setting.
    /// </summary>
    /// <returns><see cref="AstronomicalCalendar.GetSeaLevelSunrise"/> if <see cref="UseElevation"/> is false (the default), or elevation adjusted
    ///         <see cref="AstronomicalCalendar.GetSunrise"/> if it is true.</returns>
    /// <seealso cref="AstronomicalCalendar.GetSunrise"/>
    protected virtual DateTimeOffset? GetSunriseBasedOnElevationSetting()
    {
        if (UseElevation)
        {
            return base.GetSunrise();
        }
        return GetSeaLevelSunrise();
    }

    /// <summary>
    /// This method will return <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> if <see cref="UseElevation"/> is false (the default), or
    /// elevation adjusted <see cref="AstronomicalCalendar.GetSunset"/> if it is true. This allows relevant <em>zmanim</em>in this and extending classes (such
    /// as the <see cref="ComprehensiveZmanimCalendar"/>) to automatically adjust to the elevation setting.
    /// </summary>
    /// <returns><see cref="AstronomicalCalendar.GetSeaLevelSunset"/> if <see cref="UseElevation"/> is false (the default), or elevation adjusted
    ///         <see cref="AstronomicalCalendar.GetSunset"/> if it is true.</returns>
    /// <seealso cref="AstronomicalCalendar.GetSunset"/>
    protected virtual DateTimeOffset? GetSunsetBasedOnElevationSetting()
    {
        if (UseElevation)
        {
            return base.GetSunset();
        }
        return GetSeaLevelSunset();
    }

    /// <summary>
    /// A method that returns <em>tzais</em> (nightfall) when the sun is <see cref="Zenith8Point5">8.5°</see> below the <see cref="AstronomicalCalendar.GeometricZenith"/> (90°) after <see cref="AstronomicalCalendar.GetSunset">sunset</see>, a time that Rabbi Meir Posen in his the <em><a
    /// href="https://www.worldcat.org/oclc/29283612">Ohr Meir</a></em> calculated that 3 small stars are visible, which is later than
    /// the required 3 medium stars. This calculation is based on the sun's position below the horizon 36 minutes after <see cref="AstronomicalCalendar.GetSeaLevelSunset">sunset</see> in Jerusalem <a href="https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/"
    /// >around the equinox / equilux</a>, which is 8.5° below <see cref="AstronomicalCalendar.GeometricZenith"/>.
    /// </summary>
    /// <returns>The <see cref="DateTimeOffset"/> of nightfall. If the calculation can't be computed such as northern and southern locations
    ///         even south of the Arctic Circle and north of the Antarctic Circle where the sun may not reach low enough below the
    ///         horizon for this calculation, a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.
    /// (the same method) that returns an identical time to the old generic <em>tzais</em></returns>
    public virtual DateTimeOffset? GetTzaisGeonim8Point5Degrees()
    {
        return ToZone(GetSunsetOffsetByDegrees(Zenith8Point5));
    }

    /// <summary>
    /// Returns <em>alos</em> (dawn) based on the time when the sun is <see cref="Zenith16Point1">16.1°</see> below the eastern <see cref="AstronomicalCalendar.GeometricZenith"/> before <see cref="AstronomicalCalendar.GetSunrise">sunrise</see>. This is based on the calculation that the time
    /// between dawn and sunrise (and sunset to nightfall) is 72 minutes, the time that is takes to walk 4 <a href=
    /// "https://en.wikipedia.org/wiki/Biblical_mile">mil</a> at 18 minutes a mil (<a href="https://en.wikipedia.org/wiki/Maimonides"
    /// >Rambam</a> and others). The sun's position below the horizon 72 minutes before <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> in Jerusalem on
    /// the <a href="https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">around the equinox / equilux</a> is
    /// 16.1° below <see cref="AstronomicalCalendar.GeometricZenith"/>.
    /// </summary>
    /// <returns>The <see cref="DateTimeOffset"/> of dawn. If the calculation can't be computed such as northern and southern
    ///         locations even south of the Arctic Circle and north of the Antarctic Circle where the sun may not reach
    ///         low enough below the horizon for this calculation, a <c>null</c> will be returned. See detailed
    ///         explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetTzais16Point1Degrees"/>
    public virtual DateTimeOffset? GetAlos16Point1Degrees()
    {
        return ToZone(GetSunriseOffsetByDegrees(Zenith16Point1));
    }

    /// <summary>
    /// Method to return <em>alos</em> (dawn) calculated as 72 minutes before <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> or <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> (depending on the <see cref="UseElevation"/> setting). This time
    /// is based on the time to walk the distance of 4 <a href="https://en.wikipedia.org/wiki/Biblical_mile">mil</a> at 18 minutes a mil. The
    /// 72-minute time (but not the concept of fixed minutes) is based on the opinion that the time of the <em>Neshef</em>
    /// (twilight between dawn and sunrise) does not vary by the time of year or location but depends on the time it takes
    /// to walk the distance of 4 mil.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year where the sun does not rise, and one where it does not set,
    ///         a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation.</returns>
    public virtual DateTimeOffset? GetAlos72Minutes()
    {
        return ToZone(GetTimeOffset(GetSunriseBasedOnElevationSetting(), JavaTime.OfMinutes(-72)));
    }

    /// <summary>
    /// This method returns <see cref="AstronomicalCalendar.GetSunTransit()">Astronomical <em>chatzos hayom</em></see> if the
    /// <see cref="AstronomicalCalculator">calculator</see> class used supports it and
    /// <see cref="UseAstronomicalChatzos">isUseAstronomicalChatzos()</see> is set to <c>true</c> or the <see cref="GetChatzosHayomAsHalfDay">halfway point between sunrise and sunset</see> if it does not support it, or it is not configured to use it. There are currently
    /// four <see cref="AstronomicalCalculator">calculators</see> available in the API. The default <see cref="NOAACalculator">NOAA calculator</see>, the <see cref="SPACalculator">SPA calculator</see>
    /// and the <see cref="MeeusCalculator">Meeus calculator</see> calculate it as <see cref="AstronomicalCalendar.GetSunTransit()">astronomical
    /// <em>chatzos</em></see>, while the <see cref="SunTimesCalculator">USNO calculator</see> calculates <em>chatzos</em>
    /// as halfway between sunrise and sunset (identical to six <em>shaos zmaniyos</em> after sunrise). See <a href=
    /// "https://kosherjava.com/2020/07/02/definition-of-chatzos/">The Definition of <em>Chatzos</em></a> for a detailed explanation of
    /// the ways to calculate <em>Chatzos</em>. Since half-day <em>chatzos</em> can be <c>null</c> in the Arctic on a day when
    /// either sunrise or sunset did not happen and astronomical <em>chatzos</em> can be calculated even in the Arctic, if half-day
    /// <em>chatzos</em> calculates as <c>null</c> and astronomical <em>chatzos</em> is supported by the calculator, astronomical
    /// <em>chatzos</em> will be returned to avoid returning a <c>null</c>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of <em>chatzos</em>. If the calculation can't be computed such as in the Arctic Circle
    ///         where there is at least one day where the sun does not rise, and one where it does not set, and the calculator does not
    ///         support astronomical calculations (that will never report a <c>null</c>) a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="AstronomicalCalendar.GetSunTransit()"/>
    /// <seealso cref="GetChatzosHayomAsHalfDay"/>
    /// <seealso cref="UseAstronomicalChatzos"/>
    /// <seealso cref="UseAstronomicalChatzos"/>
    public virtual DateTimeOffset? GetChatzosHayom()
    {
        if (UseAstronomicalChatzos)
        {
            return ToZone(GetSunTransit()); // can be null if the calculator does not support astronomical chatzos
        }
        else
        {
            DateTimeOffset? halfDayChatzos = GetChatzosHayomAsHalfDay();
            if (halfDayChatzos == null)
            {
                return ToZone(GetSunTransit()); // can be null if the calculator does not support astronomical chatzos
            }
            else
            {
                return ToZone(halfDayChatzos);
            }
        }
    }

    /// <summary>
    /// A method that returns <em>chatzos halayla</em> at the <b>end of the day</b> (the last <em>zman</em> of the day
    /// returned by the calendar, that may actually be after midnight of the day it is being calculated for). For example, if
    /// calculating it for the date of <em>Erev Pesach</em>, the calculation will be for <em>Lail Pesach</em> to allow you to use the
    /// <em>zman</em> as <em>sof zman achilas afikoman</em>. <see cref="AstronomicalCalendar.GetSolarMidnight">Astronomical <em>chatzos halayla</em></see> will be
    /// returned if the <see cref="AstronomicalCalculator">calculator</see> class used supports it and <see cref="UseAstronomicalChatzos">isUseAstronomicalChatzos()</see> is set to <c>true</c>. Otherwise the <see cref="GetChatzos">halfway point</see> between sunset and the following day's sunrise, if it does not support it, or it is not configured to
    /// use it. There are currently four <see cref="AstronomicalCalculator">calculators</see> available in the API.
    /// The default <see cref="NOAACalculator">NOAA calculator</see>, the <see cref="SPACalculator">SPA calculator</see> and the <see cref="MeeusCalculator">Meeus
    /// calculator</see> calculate it as <see cref="AstronomicalCalendar.GetSolarMidnight">astronomical <em>chatzos halayla</em></see>, while the <see cref="SunTimesCalculator">USNO calculator</see> calculates <em>chatzos halayla</em> as halfway between sunset
    /// and the following day's sunrise (identical to six <em>shaos zmaniyos</em> after sunset). See <a href=
    /// "https://kosherjava.com/2020/07/02/definition-of-chatzos/">The Definition of <em>Chatzos</em></a> for a detailed explanation
    /// of the ways to calculate <em>Chatzos</em>. Since half-night <em>chatzos</em> can be <c>null</c> in the Arctic on a day
    /// when either sunset or the following day's sunrise did not happen and astronomical <em>chatzos halayla</em> can be calculated
    /// even in the Arctic, if half-day <em>chatzos</em> calculates as <c>null</c> and astronomical <em>chatzos</em> is
    /// supported by the calculator, astronomical <em>chatzos</em> will be returned to avoid returning a <c>null</c>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of <em>chatzos halayla</em> at the <b>end of the current day</b>. If the calculation can't be
    ///         computed such as in the Arctic Circle where there is at least one day where the sun does not rise, and one where it
    ///         does not set, and the calculator does not support astronomical calculations (that will never report a
    ///         <c>null</c>) a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="AstronomicalCalendar.GetSolarMidnight"/>
    /// <seealso cref="GetChatzos"/>
    /// <seealso cref="UseAstronomicalChatzos"/>
    /// <seealso cref="UseAstronomicalChatzos"/>
    public virtual DateTimeOffset? GetChatzosHalayla()
    {
        if (UseAstronomicalChatzos)
        {
            return ToZone(GetSolarMidnight()); // can be null if the calculator does not support astronomical chatzos
        }
        else
        {
            ZmanimCalendar clonedCalendar = (ZmanimCalendar)this.Clone();
            clonedCalendar.LocalDate = LocalDate.AddDays(1);
            DateTimeOffset? halfNightChatzos = GetChatzos(GetSeaLevelSunset(), clonedCalendar.GetSeaLevelSunrise());
            if (halfNightChatzos == null)
            {
                return ToZone(GetSolarMidnight()); // can be null if the calculator does not support astronomical chatzos
            }
            else
            {
                return ToZone(halfNightChatzos);
            }
        }
    }

    /// <summary>
    /// A method that returns <em>chatzos</em> (<em>hayom</em> or <em>layla</em>) calculated as halfway between the begin and end
    /// times passed in. If sunrise and sunset (or sunset and the following sunrise for <em>chatzos halayla</em>) are passed in,
    /// the <em>zman</em> returned will be close to, but not exactly, when the Sun is <a href=
    /// "https://en.wikipedia.org/wiki/Transit_%28astronomy%29">transiting</a> the <a href=
    /// "https://en.wikipedia.org/wiki/Meridian_%28astronomy%29">celestial meridian</a> due to changes in declination (the
    /// lengthening or shortening day). See <a href="https://kosherjava.com/2020/07/02/definition-of-chatzos/">The Definition of
    /// <em>Chatzos</em></a> for a detailed explanation of the ways to calculate <em>Chatzos</em>. This method is a convenience
    /// method that calls the parent class's <see cref="AstronomicalCalendar.GetSunTransit()"/>. A practical example of using this would be
    /// calculating <em>chatzos halayla</em> for the end of <em>zman achilas afikoman</em> on <em>Pesach</em> night, where
    /// <a href="https://en.wikipedia.org/wiki/Shlomo_Zalman_Auerbach">Rav Shlomo Zalman Auerbach</a> in the Halichos Shlomo,
    /// Moadim Nisan - Av, ch. 9, no. 44, pages 289-292, questioned the common practice of considering <em>chatzos halayla</em> as
    /// astronomical <em>chatzos halayla</em>, and felt that for this it should be at <em>chatzi halayla</em>, halfway between sunset
    /// and <em>alos</em> or <em>bedieved</em> from an early <em>tzais</em> that is lower (degree-wise) than <em>alos</em> the next
    /// morning, thus making this <em>zman</em> significantly earlier. <a href="https://en.wikipedia.org/wiki/Moshe_Sternbuch">Rav
    /// Moshe Sternbuch</a> in <a href="https://hebrewbooks.org/pdfpager.aspx?req=69091&amp;pgnum=15">Teshuvos Vehanhagos v. VII, ch 1, p.
    /// 3</a>, agreed to this calculation for <em>chatzos halayla</em> on <em>Pesach</em> night.
    /// </summary>
    /// <param name="begin">the beginning of day or night for calculating <em>chatzos</em>. For <em>chatzos hayom</em>, this can be sea level
    ///         sunrise, or any arbitrary sunrise or <em>alos</em> used as the beginning of the day, and for <em>chatzos halayla</em>
    ///         this can be sea level sunset, or any arbitrary sunset or <em>tzais</em> used as the beginning of the night.</param>
    /// <param name="end">the end of day or night for calculating <em>chatzos</em>. For <em>chatzos hayom</em>, this can be sea level sunset,
    ///         or any arbitrary sunset or <em>tzais</em> used as the end of day, and for <em>chatzos halayla</em> this can be sea
    ///         level sunrise, or any arbitrary sunrise or <em>alos</em> used as the end of the night.</param>
    /// <returns>the <see cref="DateTimeOffset"/> representing <em>chatzos</em>. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year where the sun does not rise, and one where it does
    ///         not set, <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    /// <seealso cref="AstronomicalCalendar.GetSunTransit()"/>
    /// <seealso cref="GetChatzosHayomAsHalfDay"/>
    /// <seealso cref="GetChatzosHayom"/>
    /// <seealso cref="GetChatzosHalayla"/>
    public virtual DateTimeOffset? GetChatzos(DateTimeOffset? begin, DateTimeOffset? end)
    {
        return ToZone(GetSunTransit(begin, end));
    }

    /// <summary>
    /// Returns <em>chatzos</em> calculated as halfway between sunrise and sunset. Many are of the opinion that
    /// <em>chatzos</em> is calculated as the midpoint between <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> and
    /// <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see>, despite it not being the most accurate way to calculate it. A day
    /// starting at <em>alos</em> and ending at <em>tzais</em> using the same time or degree offset will also return
    /// the same time. In reality due to lengthening or shortening of day, this is not necessarily the exact midpoint of
    /// the day, but it is very close. There are currently four <see cref="AstronomicalCalculator">calculators</see> available in the API. The default <see cref="NOAACalculator">NOAA calculator</see>, the
    /// <see cref="SPACalculator">SPA calculator</see> and the <see cref="MeeusCalculator">Meeus calculator</see> calculate <em>chatzos</em> as <see cref="AstronomicalCalendar.GetSunTransit()">astronomical <em>chatzos</em></see>, which is slightly
    /// more accurate, while the <see cref="SunTimesCalculator">USNO calculator</see> calculates it as halfway
    /// between sunrise and sunset (identical to six <em>shaos zmaniyos</em> after sunrise). This method allows you to use any
    /// calculator and still calculate <em>chatzos</em> as six <em>shaos zmaniyos</em> after sunrise. See <a href=
    /// "https://kosherjava.com/2020/07/02/definition-of-chatzos/">The Definition of <em>Chatzos</em></a> for a detailed explanation
    /// of the ways to calculate <em>Chatzos</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>chatzos</em>. If the calculation can't be computed such
    ///         as in the Arctic Circle where there is at least one day a year where the sun does not rise, and one where
    ///         it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="NOAACalculator.GetUTCNoon"/>
    /// <seealso cref="SunTimesCalculator.GetUTCNoon"/>
    /// <seealso cref="AstronomicalCalculator.GetUTCNoon"/>
    /// <seealso cref="AstronomicalCalendar.GetSunTransit()"/>
    /// <seealso cref="GetChatzosHayom"/>
    /// <seealso cref="GetChatzosHalayla"/>
    /// <seealso cref="GetChatzos"/>
    /// <seealso cref="AstronomicalCalendar.GetSunTransit()"/>
    /// <seealso cref="UseAstronomicalChatzos"/>
    public virtual DateTimeOffset? GetChatzosHayomAsHalfDay()
    {
        return ToZone(GetChatzos(GetSeaLevelSunrise(), GetSeaLevelSunset()));
    }

    /// <summary>
    /// A generic method for calculating the latest <em>zman krias shema</em> (time to recite shema in the morning) that is 3 *
    /// <em>shaos zmaniyos</em> (temporal hours) after the start of the day, calculated using the start and end of the day passed
    /// to this method. The time from the start of day to the end of day are divided into 12 <em>shaos zmaniyos</em> (temporal
    /// hours), and the latest <em>zman krias shema</em> is calculated as 3 of those <em>shaos zmaniyos</em> after the beginning of
    /// the day. If <see cref="UseAstronomicalChatzosForOtherZmanim"/> is <c>true</c>, the 3 <em>shaos zmaniyos</em> will be
    /// based on 1/6 of the time between sunrise and <see cref="AstronomicalCalendar.GetSunTransit()">astronomical <em>chatzos</em></see>. As an example, passing
    /// <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> and <see cref="AstronomicalCalendar.GetSunset">sunset</see> or <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level
    /// sunrise</see> and <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> to this method (or <see cref="GetSunriseBasedOnElevationSetting"/> and
    /// <see cref="GetSunsetBasedOnElevationSetting"/> that is driven off the <see cref="UseElevation"/> setting) will return <em>sof zman
    /// krias shema</em> according to the opinion of the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. In cases where
    /// the start and end dates are not synchronous such as in <c>getSofZmanShmaAlos16Point1ToTzaisGeonim7Point083Degrees()</c> (removed in KosherJava) <c>false</c> should be passed to the synchronous parameter
    /// to ensure that <see cref="UseAstronomicalChatzosForOtherZmanim"/> will not be used.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>zman krias shema</em>. This can be sunrise or any <em>alos</em> passed
    ///         to this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>zman krias shema</em>. This can be sunset or any <em>tzais</em> passed to
    ///         this method.</param>
    /// <param name="synchronous">If the <em>zman</em> has a synchronous start and end of the day. If this is <c>false</c>, using a
    ///         <see cref="UseAstronomicalChatzosForOtherZmanim"/> makes no sense and will be ignored even if set to true, since by
    ///         definition <em>chatzos</em> will not be the middle of the day for the <em>zman</em>.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman shema</em> based on the start and end of day times passed to this
    ///         method. If the calculation can't be computed such as in the Arctic Circle where there is at least one day
    ///         a year where the sun does not rise, and one where it does not set, a <c>null</c> will be returned. See
    ///         detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="UseAstronomicalChatzosForOtherZmanim"/>
    public virtual DateTimeOffset? GetSofZmanShma(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay, bool synchronous)
    {
        if (UseAstronomicalChatzosForOtherZmanim && synchronous)
        {
            return ToZone(GetHalfDayBasedZman(startOfDay, GetChatzosHayom(), 3));
        }
        else
        {
            return ToZone(GetShaahZmanisBasedZman(startOfDay, endOfDay, 3));
        }
    }

    /// <summary>
    /// A generic method for calculating the latest <em>zman krias shema</em> that calls <see cref="GetSofZmanShma(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// passing <c>false</c> to the synchronous parameter since there is no way to know if the start and end of the day are
    /// synchronous. Passing true when they are not synchronous is too much of a risk. See information on that method for more details.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>zman krias shema</em>. This can be sunrise or any <em>alos</em> passed
    ///         to this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>zman krias shema</em>. This can be sunset or any <em>tzais</em> passed to
    ///         this method.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman shema</em> based on the start and end of day times passed to this
    ///         method. If the calculation can't be computed such as in the Arctic Circle where there is at least one day
    ///         a year where the sun does not rise, and one where it does not set, a <c>null</c> will be returned. See
    ///         detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetSofZmanShma(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    public virtual DateTimeOffset? GetSofZmanShma(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay)
    {
        return ToZone(GetSofZmanShma(startOfDay, endOfDay, false));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite shema in the morning) that is 3 *
    /// <see cref="GetShaahZmanisGRA"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> or
    /// <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> (depending on the <see cref="UseElevation"/> setting), according
    /// to the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>.
    ///  The day is calculated from <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> to <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level
    ///  sunset</see> or from <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> to <see cref="AstronomicalCalendar.GetSunset">sunset</see> (depending on the
    ///  <see cref="UseElevation"/> setting).
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman shema</em> according to the GRA. If the calculation can't be
    ///         computed such as in the Arctic Circle where there is at least one day a year where the sun does not rise,
    ///         and one where it does not set, a <c>null</c> will be returned. See the detailed explanation on top
    ///         of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetSofZmanShma(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="GetShaahZmanisGRA"/>
    /// <seealso cref="UseElevation"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSofZmanShmaBaalHatanya"/>
    public virtual DateTimeOffset? GetSofZmanShmaGRA()
    {
        return ToZone(GetSofZmanShma(GetSunriseBasedOnElevationSetting(), GetSunsetBasedOnElevationSetting(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman krias shema</em> (time to recite Shema in the morning) according to the opinion of
    /// the <a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a> based on <em>alos</em> being <see cref="GetAlos72Minutes">72</see> minutes before <see cref="AstronomicalCalendar.GetSunrise">sunrise</see>. This time is 3 <see cref="GetShaahZmanis72Minutes"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="GetAlos72Minutes">dawn</see> based on the opinion
    /// of the MGA that the day is calculated from a <see cref="GetAlos72Minutes">dawn</see> of 72 minutes before sunrise to
    /// <see cref="GetTzais72Minutes">nightfall</see> of 72 minutes after sunset. This returns the time of 3 * <see cref="GetShaahZmanis72Minutes"/> after <see cref="GetAlos72Minutes">dawn</see>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman krias shema</em>. If the calculation can't be computed such
    ///         as in the Arctic Circle where there is at least one day a year where the sun does not rise, and one where
    ///         it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="UseAstronomicalChatzosForOtherZmanim"/>
    /// <seealso cref="GetShaahZmanis72Minutes"/>
    /// <seealso cref="GetAlos72Minutes"/>
    /// <seealso cref="GetSofZmanShmaMGA72Minutes"/>
    /// <seealso cref="GetSofZmanShma(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    public virtual DateTimeOffset? GetSofZmanShmaMGA72Minutes()
    {
        return ToZone(GetSofZmanShma(GetAlos72Minutes(), GetTzais72Minutes(), true));
    }

    /// <summary>
    /// This method returns the <em>tzais</em> (nightfall) based on the opinion of <em>Rabbeinu Tam</em> that <em>tzais hakochavim</em>
    /// is calculated as 72 minutes after sunset, the time it takes to walk 4 <a href=
    /// "https://en.wikipedia.org/wiki/Biblical_mile">mil</a> at 18 minutes a mil. According to the <a href=
    /// "https://en.wikipedia.org/wiki/Samuel_Loew">Machtzis Hashekel</a> in Orach Chaim 235:3, the <a href=
    /// "https://en.wikipedia.org/wiki/Joseph_ben_Meir_Teomim">Pri Megadim</a> in Orach Chaim 261:2 (see the Biur Halacha) and others
    /// (see Hazmanim Bahalacha 17:3 and 17:5) the 72 minutes are standard clock minutes any time of the year in any location.
    /// Depending on the <see cref="UseElevation"/> setting, a 72-minute offset from  either <see cref="AstronomicalCalendar.GetSunset">sunset</see> or
    /// <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> is used.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing 72 minutes after sunset. If the calculation can't be
    ///         computed such as in the Arctic Circle where there is at least one day a year where the sun does not rise,
    ///         and one where it does not set, a <c>null</c> will be returned See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetTzais16Point1Degrees"/>
    public virtual DateTimeOffset? GetTzais72Minutes()
    {
        return ToZone(GetTimeOffset(GetSunsetBasedOnElevationSetting(), JavaTime.OfMinutes(72)));
    }

    /// <summary>
    /// A method to return candle lighting time, calculated as <see cref="CandleLightingOffset"/> minutes before <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see>. This will return the time for any day of the week, since it can be used to calculate
    /// candle lighting time for <em>Yom Tov</em> (mid-week holidays) as well. Elevation adjustments are intentionally not performed
    /// by this method, but you can calculate it by passing the elevation adjusted sunset to <see cref="AstronomicalCalendar.GetTimeOffset"/>. If the calculated time is before <see cref="GetPlagHaminchaGRA"><em>plag hamincha</em></see> (on very short winter days),
    /// a <c>null</c> will be returned.
    /// </summary>
    /// <returns>candle lighting time. If the calculation can't be computed such as in the Arctic Circle where there is at
    ///         least one day a year where the sun does not rise, and one where it does not set, or if it is before <see cref="GetPlagHaminchaGRA"><em>plag hamincha</em></see> on very short winter days, a <c>null</c> will be returned.
    ///         See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="AstronomicalCalendar.GetSeaLevelSunset"/>
    /// <seealso cref="CandleLightingOffset"/>
    /// <seealso cref="CandleLightingOffset"/>
    public virtual DateTimeOffset? GetCandleLighting()
    {
        long totalNanos = (long)(-CandleLightingOffset * MinuteNanos);
        DateTimeOffset? candleLighting = GetTimeOffset(GetSeaLevelSunset(), JavaTime.DurationOfNanos(totalNanos));
        DateTimeOffset? plagHamincha = GetPlagHaminchaGRA();
        if (candleLighting != null && plagHamincha != null && candleLighting.IsBefore(plagHamincha))
        { // very short winter day
            return null;
        }
        return ToZone(candleLighting);
    }

    /// <summary>
    /// A generic method for calculating the latest <em>zman tfilah</em> (time to recite the morning prayers) that is 4 * <em>shaos
    /// zmaniyos</em> (temporal hours) after the start of the day, calculated using the start and end of the day passed to this method.
    /// The time from the start of day to the end of day are divided into 12 <em>shaos zmaniyos</em> (temporal hours), and <em>sof
    /// zman tfila</em> is calculated as 4 of those <em>shaos zmaniyos</em> after the beginning of the day. As an example, passing
    /// <see cref="GetSunriseBasedOnElevationSetting"/> and <see cref="GetSunsetBasedOnElevationSetting"/> (depends on the <see cref="UseElevation"/> elevation setting) to this method will return <em>zman tfilah</em> according to the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. This method's synchronous parameter indicates if the start and end of day
    /// for the calculation are synchronous, having the same offset. This is typically the case, but some <em>zmanim</em> calculations
    /// are based on a start and end at different offsets from the real start and end of the day, such as starting the day at
    /// <em>alos</em> and an ending it at <em>tzais Geonim</em> or some other variant. If the day is not synchronous a <see cref="GetHalfDayBasedZman">half-day based calculations</see> will be bypassed. It would be illogical to use a
    /// half-day based calculation that start/end at <em>chatzos</em> when the two "halves" of the day are not equal, and the halfway
    /// point between them is not at <em>chatzos</em>.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>zman tfilah</em>. This can be sunrise or any <em>alos</em> passed
    ///         to this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>zman tfilah</em>. This can be sunset or any <em>tzais</em> passed
    ///         to this method.</param>
    /// <param name="synchronous">If the <em>zman</em> has a synchronous start and end of the day. If this is <c>false</c>, using a
    ///         <see cref="UseAstronomicalChatzosForOtherZmanim"/> makes no sense and will be ignored even if set to true, since by
    ///         definition <em>chatzos</em> will not be the middle of the day for the <em>zman</em>.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfilah</em> based on the start and end of day times passed to this
    ///         method. If the calculation can't be computed such as in the Arctic Circle where there is at least one day a year where
    ///         the sun does not rise, and one where it does not set, a <c>null</c> will be returned. See detailed explanation
    ///         on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual DateTimeOffset? GetSofZmanTfila(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay, bool synchronous)
    {
        if (UseAstronomicalChatzosForOtherZmanim && synchronous)
        {
            return ToZone(GetHalfDayBasedZman(startOfDay, GetChatzosHayom(), 4));
        }
        else
        {
            return ToZone(GetShaahZmanisBasedZman(startOfDay, endOfDay, 4));
        }
    }

    /// <summary>
    /// A generic method for calculating <em>sof zman biur chametz</em> or the latest time one is allowed burning
    /// <em>chametz</em> on <em>Erev Pesach</em> that is 5 * <em>shaos zmaniyos</em> (temporal hours) after the start of the
    /// day, calculated using the start and end of the day passed to this method. If the date is not <em>erev Pesach</em>, a
    /// <c>null</c> will be returned. The time from the start of day to the end of day is divided into 12 <em>shaos zmaniyos</em>
    /// (temporal hours), and <em>sof zman biur chametz</em> is calculated as 5 of those <em>shaos zmaniyos</em> after the
    /// beginning of the day. As an example, passing <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> and <see cref="AstronomicalCalendar.GetSunset">sunset</see> to this
    /// method will return <em>sof zman biur chametz</em> according to the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. This method's synchronous parameter indicates if the start
    /// and end of day for the calculation are synchronous, having the same offset. This is typically the case, but some
    /// <em>zmanim</em> calculations are based on a start and end at different offsets from the real start and end of the day,
    /// such as starting the day at <em>alos</em> and an ending it at <em>tzais Geonim</em> or some other variant. If the day
    /// is not synchronous a <see cref="GetHalfDayBasedZman">half-day based calculations</see> will be bypassed.
    /// It would be illogical to use a half-day based calculation that start/end at <em>chatzos</em> when the two "halves" of
    /// the day are not equal, and the halfway point between them is not at <em>chatzos</em>.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>sof zman biur chametz</em>. This can be sunrise or any <em>alos</em>
    ///         passed to this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>sof zman biur chametz</em>. This can be sunset or any <em>tzais</em>
    ///         passed to this method.</param>
    /// <param name="synchronous">If the <em>zman</em> has a synchronous start and end of the day. If this is <c>false</c>, using a
    ///         <see cref="UseAstronomicalChatzosForOtherZmanim"/> makes no sense and will be ignored even if set to true, since by
    ///         definition <em>chatzos</em> will not be the middle of the day for the <em>zman</em>.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the <em>sof zman biur chametz</em> based on the start and end of day times passed
    ///         to this method. If the date is not <em>Erev Pesach</em> or if the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year where the sun does not rise, and one where it does not set,
    ///         a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation.</returns>
    public virtual DateTimeOffset? GetSofZmanBiurChametz(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay, bool synchronous)
    {
        JewishCalendar jewishCalendar = new JewishCalendar(LocalDate);
        if (jewishCalendar.JewishMonth == JewishCalendar.Nissan && jewishCalendar.JewishDayOfMonth == 14)
        {
            if (UseAstronomicalChatzosForOtherZmanim && synchronous)
            {
                return ToZone(GetHalfDayBasedZman(startOfDay, GetChatzosHayom(), 5));
            }
            else
            {
                return ToZone(GetShaahZmanisBasedZman(startOfDay, endOfDay, 5));
            }
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// A generic method for calculating <em>sof zman achilas chametz</em> or the latest time one is allowed eating
    /// <em>chametz</em> on <em>Erev Pesach</em> that is 4 * <em>shaos zmaniyos</em> (temporal hours) after the start of the
    /// day, calculated using the start and end of the day passed to this method. If the date is not <em>erev Pesach</em>, a
    /// <c>null</c> will be returned. The time from the start of day to the end of day is divided into 12 <em>shaos zmaniyos</em>
    /// (temporal hours), and <em>sof zman achilas chametz</em> is calculated as 4 of those <em>shaos zmaniyos</em> after the
    /// beginning of the day. As an example, passing <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> and <see cref="AstronomicalCalendar.GetSunset">sunset</see> to this
    /// method will return <em>sof zman achilas chametz</em> according to the opinion of the <a href=
    /// "https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. This method's synchronous parameter indicates if the start
    /// and end of day for the calculation are synchronous, having the same offset. This is typically the case, but some
    /// <em>zmanim</em> calculations are based on a start and end at different offsets from the real start and end of the day,
    /// such as starting the day at <em>alos</em> and an ending it at <em>tzais Geonim</em> or some other variant. If the day
    /// is not synchronous a <see cref="GetHalfDayBasedZman">half-day based calculations</see> will be bypassed.
    /// It would be illogical to use a half-day based calculation that start/end at <em>chatzos</em> when the two "halves" of
    /// the day are not equal, and the halfway point between them is not at <em>chatzos</em>.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>sof zman achilas chametz</em>. This can be sunrise or any <em>alos</em>
    ///         passed to this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>sof zman achilas chametz</em>. This can be sunset or any <em>tzais</em>
    ///         passed to this method.</param>
    /// <param name="synchronous">If the <em>zman</em> has a synchronous start and end of the day. If this is <c>false</c>, using a <see cref="UseAstronomicalChatzosForOtherZmanim"/> makes no sense and will be ignored even if set to true, since by definition
    ///         <em>chatzos</em> will not be the middle of the day for the <em>zman</em>.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the <em>sof zman achilas chametz</em> based on the start and end of day times passed
    ///         to this method. If the date is not <em>Erev Pesach</em> or if the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year where the sun does not rise, and one where it does not set,
    ///         a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation.</returns>
    public virtual DateTimeOffset? GetSofZmanAchilasChametz(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay, bool synchronous)
    {
        JewishCalendar jewishCalendar = new JewishCalendar(LocalDate);
        if (jewishCalendar.JewishMonth == JewishCalendar.Nissan && jewishCalendar.JewishDayOfMonth == 14)
        {
            return ToZone(GetSofZmanTfila(startOfDay, endOfDay, synchronous));
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// A generic method for calculating the latest <em>zman tfila</em> that calls <see cref="GetSofZmanTfila(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// passing <c>false</c> to the synchronous parameter since there is no way to know if the start and end of the day are
    /// synchronous. Passing true when they are not synchronous is too much of a risk. See information on that method for more details.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>zman tfilah</em>. This can be sunrise or any <em>alos</em> passed
    ///         to this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>zman tfilah</em>. This can be sunset or any <em>tzais</em> passed to
    ///         this method.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfilah</em> based on the start and end of day times passed to this
    ///         method. If the calculation can't be computed such as in the Arctic Circle where there is at least one day
    ///         a year where the sun does not rise, and one where it does not set, a <c>null</c> will be returned. See
    ///         detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetSofZmanShma(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    public virtual DateTimeOffset? GetSofZmanTfila(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay)
    {
        return ToZone(GetSofZmanTfila(startOfDay, endOfDay, false));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to recite the morning prayers) that is 4 *
    /// <see cref="GetShaahZmanisGRA"><em>shaos zmaniyos</em></see>(solar hours) after <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> or
    /// <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> (depending on the <see cref="UseElevation"/> setting), according
    /// to the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>.
    /// The day is calculated from <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> to <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level
    /// sunset</see> or from <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> to <see cref="AstronomicalCalendar.GetSunset">sunset</see> (depending on the
    /// <see cref="UseElevation"/> setting).
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfilah</em>. If the calculation can't be computed such as in
    ///         the Arctic Circle where there is at least one day a year where the sun does not rise, and one where it
    ///         does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetSofZmanTfila(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="GetShaahZmanisGRA"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSofZmanTfilaBaalHatanya"/>
    public virtual DateTimeOffset? GetSofZmanTfilaGRA()
    {
        return ToZone(GetSofZmanTfila(GetSunriseBasedOnElevationSetting(), GetSunsetBasedOnElevationSetting(), true));
    }

    /// <summary>
    /// This method returns the latest <em>zman tfila</em> (time to recite the morning prayers) that is 4 * <see cref="GetShaahZmanis72Minutes"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="GetAlos72Minutes"/>, according to the
    /// <em><a href="https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a></em>. The day is calculated
    /// from 72 minutes before <see cref="GetSunriseBasedOnElevationSetting"/> to 72 minutes after <see cref="GetSunsetBasedOnElevationSetting"/>. The use of elevation depends on the <see cref="UseElevation"/> setting).
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>zman tfila</em>. If the calculation can't be computed such as in
    ///         the Arctic Circle where there is at least one day a year where the sun does not rise, and one where it
    ///         does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetSofZmanTfila(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="GetShaahZmanis72Minutes"/>
    /// <seealso cref="GetAlos72Minutes"/>
    public virtual DateTimeOffset? GetSofZmanTfilaMGA72Minutes()
    {
        return ToZone(GetSofZmanTfila(GetAlos72Minutes(), GetTzais72Minutes(), true));
    }

    /// <summary>
    /// A generic method for calculating <em>mincha gedola</em> (the earliest time to recite the <em>mincha</em> prayers) that
    /// is 6.5 * <em>shaos zmaniyos</em> (temporal hours) after the start of the day, calculated using the start and end of the
    /// day passed to this method. The time from the start of day to the end of day are divided into 12 <em>shaos zmaniyos</em>
    /// (temporal hours), and <em>mincha gedola</em> is calculated as 6.5 of those <em>shaos zmaniyos</em> after the beginning
    /// of the day. As an example, passing <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> and <see cref="AstronomicalCalendar.GetSunset">sunset</see> or
    /// <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> and <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> (depending on the <see cref="UseElevation"/> elevation setting) to this method will return <em>mincha gedola</em> according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. Alternatively, this method uses <see cref="UseAstronomicalChatzosForOtherZmanim"/> to control if the time is based on 6.5 <em>shaos zmaniyos</em> into the day
    /// mentioned above, or as half an hour <em>zmaniyos</em> based on the second half of the day after <em>chatzos</em> (<see cref="AstronomicalCalendar.GetSunTransit()">astronomical <em>chatzos</em></see> if supported by the <see cref="AstronomicalCalculator">calculator</see> and <see cref="UseAstronomicalChatzos">configured</see> or <see cref="GetChatzosHayomAsHalfDay"><em>chatzos</em> as half a day</see> if not. This
    /// method's synchronous parameter indicates if the start and end of day for the calculation are synchronous, having the same
    /// offset. This is typically the case, but some <em>zmanim</em> calculations are based on a start and end at different offsets
    /// from the real start and end of the day, such as starting the day at <em>alos</em> and an ending it at <em>tzais Geonim</em>
    /// or some other variant. If the day is not synchronous a <see cref="GetHalfDayBasedZman">half-day based
    /// calculations</see> will be bypassed. It would be illogical to use a half-day based calculation that start/end at <em>chatzos</em>
    /// when the two "halves" of the day are not equal, and the halfway point between them is not at <em>chatzos</em>.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>Mincha gedola</em>. This can be sunrise or any <em>alos</em> passed
    ///         to this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>Mincha gedola</em>. This can be sunset or any <em>tzais</em> passed
    ///         to this method.</param>
    /// <param name="synchronous">If the <em>zman</em> has a synchronous start and end of the day. If this is <c>false</c>, using a
    ///         <see cref="UseAstronomicalChatzosForOtherZmanim"/> makes no sense and will be ignored even if set to true, since by
    ///         definition <em>chatzos</em> will not be the middle of the day for the <em>zman</em>.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>Mincha gedola</em> based on the start and end of day times
    ///         passed to this method. If the calculation can't be computed such as in the Arctic Circle where there is
    ///         at least one day a year where the sun does not rise, and one where it does not set, a <c>null</c> will
    ///         be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="AstronomicalCalendar.GetSunTransit()"/>
    /// <seealso cref="GetChatzosHayomAsHalfDay"/>
    /// <seealso cref="GetChatzosHayom"/>
    /// <seealso cref="GetChatzosHalayla"/>
    /// <seealso cref="UseAstronomicalChatzos"/>
    /// <seealso cref="UseAstronomicalChatzosForOtherZmanim"/>
    public virtual DateTimeOffset? GetMinchaGedola(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay, bool synchronous)
    {
        if (UseAstronomicalChatzosForOtherZmanim && synchronous)
        {
            return ToZone(GetHalfDayBasedZman(GetChatzosHayom(), endOfDay, 0.5));
        }
        else
        {
            return ToZone(GetShaahZmanisBasedZman(startOfDay, endOfDay, 6.5));
        }
    }

    /// <summary>
    /// A generic method for calculating <em>mincha gedola</em> that calls <see cref="GetMinchaGedola(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/> passing
    /// <c>false</c> to the synchronous parameter since there is no way to know if the start and end of the day are
    /// synchronous. Passing true when they are not synchronous is too much of a risk. See information on that method for more
    /// details.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>Mincha gedola</em>. This can be sunrise or any <em>alos</em> passed
    ///         to this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>Mincha gedola</em>. This can be sunset or any <em>tzais</em> passed to
    ///         this method.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the latest <em>Mincha gedola</em> based on the start and end of day times passed to this
    ///         method. If the calculation can't be computed such as in the Arctic Circle where there is at least one day
    ///         a year where the sun does not rise, and one where it does not set, a <c>null</c> will be returned. See
    ///         detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetMinchaGedola(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    public virtual DateTimeOffset? GetMinchaGedola(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay)
    {
        return ToZone(GetMinchaGedola(startOfDay, endOfDay, false));
    }

    /// <summary>
    /// This method returns the latest <em>mincha gedola</em>,the earliest time one can pray <em>mincha</em> that is 6.5 *
    /// <see cref="GetShaahZmanisGRA"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> or
    /// <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> (depending on the <see cref="UseElevation"/> setting), according
    /// to the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. <em>Mincha gedola</em> is the earliest
    /// time one can pray <em>mincha</em>. The Ramba"m is of the opinion that it is better to delay <em>mincha</em> until
    /// <see cref="GetMinchaKetanaGRA"><em>mincha ketana GRA</em></see> while the Ra"sh, Tur, GRA and others are of the
    /// opinion that <em>mincha</em> can be prayed <em>lechatchila</em> starting at <em>mincha gedola</em>.
    /// The day is calculated from <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> to <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level
    /// sunset</see> or <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> to <see cref="AstronomicalCalendar.GetSunset">sunset</see> (depending on the <see cref="UseElevation"/>
    /// setting).
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of mincha gedola. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year where the sun does not rise, and one where it does
    ///         not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <remarks>
    /// <b>To do:</b> Consider adjusting this to calculate the time as half an hour <em>zmaniyos</em> after <see cref="GetChatzosHayom"/>
    ///         that uses <see cref="UseAstronomicalChatzos"/> to determine the type of <em>chatzos</em> to utilize (if the
    ///         <see cref="AstronomicalCalculator">calculator</see> support astronomical <em>chatzos</em>),
    ///         based on the <see cref="UseAstronomicalChatzos"/> setting.
    /// </remarks>
    /// <seealso cref="GetMinchaGedola(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="GetShaahZmanisGRA"/>
    /// <seealso cref="GetMinchaKetanaGRA"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetMinchaGedolaBaalHatanya"/>
    public virtual DateTimeOffset? GetMinchaGedolaGRA()
    {
        return ToZone(GetMinchaGedola(GetSunriseBasedOnElevationSetting(), GetSunsetBasedOnElevationSetting(), true));
    }

    /// <summary>
    /// A generic method for calculating <em>samuch lemincha ketana</em>, / near <em>mincha ketana</em> time that is half
    /// an hour before <see cref="GetMinchaKetana(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>  or 9 * <em>shaos zmaniyos</em> (temporal hours) after the
    /// start of the day, calculated using the start and end of the day passed to this method.
    /// The time from the start of day to the end of day are divided into 12 <em>shaos zmaniyos</em> (temporal hours), and
    /// <em>samuch lemincha ketana</em> is calculated as 9 of those <em>shaos zmaniyos</em> after the beginning of the day.
    /// For example, passing <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> and <see cref="AstronomicalCalendar.GetSunset">sunset</see> or <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea
    /// level sunrise</see> and <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> (depending on the <see cref="UseElevation"/> elevation
    /// setting) to this method will return <em>samuch lemincha ketana</em> according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. See the <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=60387&amp;st=&amp;pgnum=294">Mechaber and Mishna Berurah 232</a> and <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=60388&amp;pgnum=34">249:2</a>.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>samuch lemincha ketana</em>. This can be sunrise or any <em>alos</em>
    ///         passed to this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>samuch lemincha ketana</em>. This can be sunset or any <em>tzais</em>
    ///         passed to this method.</param>
    /// <param name="synchronous">If the <em>zman</em> has a synchronous start and end of the day. If this is <c>false</c>, using a
    ///         <see cref="UseAstronomicalChatzosForOtherZmanim"/> makes no sense and will be ignored even if set to true, since by
    ///         definition <em>chatzos</em> will not be the middle of the day for the <em>zman</em>.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>Mincha ketana</em> based on the start and end of day times
    ///         passed to this method. If the calculation can't be computed such as in the Arctic Circle where there is
    ///         at least one day a year where the sun does not rise, and one where it does not set, a <c>null</c> will
    ///         be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSamuchLeMinchaKetanaGRA"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSamuchLeMinchaKetana16Point1Degrees"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSamuchLeMinchaKetana72Minutes"/>
    public virtual DateTimeOffset? GetSamuchLeMinchaKetana(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay, bool synchronous)
    {
        if (UseAstronomicalChatzosForOtherZmanim && synchronous)
        {
            return ToZone(GetHalfDayBasedZman(GetChatzosHayom(), endOfDay, 3));
        }
        else
        {
            return ToZone(GetShaahZmanisBasedZman(startOfDay, endOfDay, 9));
        }
    }

    /// <summary>
    /// A generic method for calculating <em>samuch lemincha ketana</em> that calls <see cref="GetSamuchLeMinchaKetana(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// passing <c>false</c> to the synchronous parameter since there is no way to know if the start and end of the day are
    /// synchronous. Passing true when they are not synchronous is too much of a risk. See information on that method for more details.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>samuch lemincha ketana</em>. This can be sunrise or any <em>alos</em>
    ///         passed to this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>samuch lemincha ketana</em>. This can be sunset or any <em>tzais</em>
    ///         passed to this method.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>samuch lemincha ketana</em> based on the start and end of day times
    ///         passed to this method. If the calculation can't be computed such as in the Arctic Circle where there is
    ///         at least one day a year where the sun does not rise, and one where it does not set, a <c>null</c> will
    ///         be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetSamuchLeMinchaKetana(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    public virtual DateTimeOffset? GetSamuchLeMinchaKetana(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay)
    {
        return ToZone(GetSamuchLeMinchaKetana(startOfDay, endOfDay, false));
    }

    /// <summary>
    /// A generic method for calculating <em>mincha ketana</em>, (the preferred time to recite the mincha prayers in
    /// the opinion of the <em><a href="https://en.wikipedia.org/wiki/Maimonides">Rambam</a></em> and others) that is
    /// 9.5 * <em>shaos zmaniyos</em> (temporal hours) after the start of the day, calculated using the start and end
    /// of the day passed to this method.
    /// The time from the start of day to the end of day are divided into 12 <em>shaos zmaniyos</em> (temporal hours), and
    /// <em>mincha ketana</em> is calculated as 9.5 of those <em>shaos zmaniyos</em> after the beginning of the day. As an
    /// example, passing <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> and <see cref="AstronomicalCalendar.GetSunset">sunset</see> or <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea
    /// level sunrise</see> and <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> (depending on the <see cref="UseElevation"/>
    /// elevation setting) to this method will return <em>mincha ketana</em> according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. This method's synchronous parameter indicates if the start
    /// and end of day for the calculation are synchronous, having the same offset. This is typically the case, but some
    /// <em>zmanim</em> calculations are based on a start and end at different offsets from the real start and end of the day,
    /// such as starting the day at <em>alos</em> and an ending it at <em>tzais Geonim</em> or some other variant. If the day
    /// is not synchronous a <see cref="GetHalfDayBasedZman">half-day based calculations</see> will be bypassed.
    /// It would be illogical to use a half-day based calculation that start/end at <em>chatzos</em> when the two "halves" of
    /// the day are not equal, and the halfway point between them is not at <em>chatzos</em>.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>Mincha ketana</em>. This can be sunrise or any <em>alos</em> passed
    ///         to this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>Mincha ketana</em>. This can be sunset or any <em>tzais</em> passed to
    ///         this method.</param>
    /// <param name="synchronous">If the <em>zman</em> has a synchronous start and end of the day. If this is <c>false</c>, using a
    ///         <see cref="UseAstronomicalChatzosForOtherZmanim"/> makes no sense and will be ignored even if set to true, since by
    ///         definition <em>chatzos</em> will not be the middle of the day for the <em>zman</em>.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>Mincha ketana</em> based on the start and end of day times
    ///         passed to this method. If the calculation can't be computed such as in the Arctic Circle where there is
    ///         at least one day a year where the sun does not rise, and one where it does not set, a <c>null</c> will
    ///         be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual DateTimeOffset? GetMinchaKetana(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay, bool synchronous)
    {
        if (UseAstronomicalChatzosForOtherZmanim && synchronous)
        {
            return ToZone(GetHalfDayBasedZman(GetChatzosHayom(), endOfDay, 3.5));
        }
        else
        {
            return ToZone(GetShaahZmanisBasedZman(startOfDay, endOfDay, 9.5));
        }
    }

    /// <summary>
    /// A generic method for calculating <em>mincha ketana</em> that calls <see cref="GetMinchaKetana(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/> passing
    /// <c>false</c> to the synchronous parameter since there is no way to know if the start and end of the day are synchronous.
    /// Passing true when they are not synchronous is too much of a risk. See information on that method for more details.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>Mincha ketana</em>. This can be sunrise or any <em>alos</em> passed
    ///         to this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>Mincha ketana</em>. This can be sunset or any <em>tzais</em> passed to
    ///         this method.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>Mincha ketana</em> based on the start and end of day times
    ///         passed to this method. If the calculation can't be computed such as in the Arctic Circle where there is
    ///         at least one day a year where the sun does not rise, and one where it does not set, a <c>null</c> will
    ///         be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetMinchaKetana(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    public virtual DateTimeOffset? GetMinchaKetana(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay)
    {
        return ToZone(GetMinchaKetana(startOfDay, endOfDay, false));
    }

    /// <summary>
    /// This method returns <em>mincha ketana</em>,the preferred earliest time to pray <em>mincha</em> in the
    /// opinion of the <em><a href="https://en.wikipedia.org/wiki/Maimonides">Rambam</a></em> and others, that is 9.5
    /// * <see cref="GetShaahZmanisGRA"><em>shaos zmaniyos</em></see> (solar hours) after <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> or
    /// <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> (depending on the <see cref="UseElevation"/> setting), according
    /// to the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. For more information on this see the
    /// documentation on <see cref="GetMinchaGedolaGRA"><em>mincha gedola</em></see>.
    /// The day is calculated from <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> to <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level
    /// sunset</see> or from <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> to <see cref="AstronomicalCalendar.GetSunset">sunset</see> (depending on the <see cref="UseElevation"/>
    /// setting.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of mincha ketana. If the calculation can't be computed such as in the
    ///         Arctic Circle where there is at least one day a year where the sun does not rise, and one where it does
    ///         not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetMinchaKetana(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="GetShaahZmanisGRA"/>
    /// <seealso cref="GetMinchaGedolaGRA"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetMinchaKetanaBaalHatanya"/>
    public virtual DateTimeOffset? GetMinchaKetanaGRA()
    {
        return ToZone(GetMinchaKetana(GetSunriseBasedOnElevationSetting(), GetSunsetBasedOnElevationSetting(), true));
    }

    /// <summary>
    /// A generic method for calculating <em>plag hamincha</em> (the earliest time that Shabbos can be started) that is
    /// 10.75 hours after the start of the day, (or 1.25 hours before the end of the day) based on the start and end of
    /// the day passed to the method.
    /// The time from the start of day to the end of day are divided into 12 <em>shaos zmaniyos</em> (temporal hours), and
    /// <em>plag hamincha</em> is calculated as 10.75 of those <em>shaos zmaniyos</em> after the beginning of the day. As an
    /// example, passing <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> and <see cref="AstronomicalCalendar.GetSunset">sunset</see> or <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level
    /// sunrise</see> and <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> (depending on the <see cref="UseElevation"/> elevation
    /// setting) to this method will return <em>plag mincha</em> according to the opinion of the
    /// <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. This method's synchronous parameter indicates if the start
    /// and end of day for the calculation are synchronous, having the same offset. This is typically the case, but some
    /// <em>zmanim</em> calculations are based on a start and end at different offsets from the real start and end of the day,
    /// such as starting the day at <em>alos</em> and an ending it at <em>tzais Geonim</em> or some other variant. If the day
    /// is not synchronous a <see cref="GetHalfDayBasedZman">half-day based calculations</see> will be bypassed. It
    /// would be illogical to use a half-day based calculation that start/end at <em>chatzos</em> when the two "halves" of the
    /// day are not equal, and the halfway point between them is not at <em>chatzos</em>.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>plag hamincha</em>. This can be sunrise or any <em>alos</em> passed to
    ///         this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>plag hamincha</em>. This can be sunset or any <em>tzais</em> passed to
    ///         this method.</param>
    /// <param name="synchronous">If the <em>zman</em> has a synchronous start and end of the day. If this is <c>false</c>, using a
    ///         <see cref="UseAstronomicalChatzosForOtherZmanim"/> makes no sense and will be ignored even if set to true, since by
    ///         definition <em>chatzos</em> will not be the middle of the day for the <em>zman</em>.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em> based on the start and end of day times
    ///         passed to this method. If the calculation can't be computed such as in the Arctic Circle where there is
    ///         at least one day a year where the sun does not rise, and one where it does not set, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual DateTimeOffset? GetPlagHamincha(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay, bool synchronous)
    {
        if (UseAstronomicalChatzosForOtherZmanim && synchronous)
        {
            return ToZone(GetHalfDayBasedZman(GetChatzosHayom(), endOfDay, 4.75));
        }
        else
        {
            return ToZone(GetShaahZmanisBasedZman(startOfDay, endOfDay, 10.75));
        }
    }

    /// <summary>
    /// A generic method for calculating <em>plag hamincha</em> that calls <see cref="GetPlagHamincha(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/> passing
    /// <c>false</c> to the synchronous parameter since there is no way to know if the start and end of the day are synchronous.
    /// Passing true when they are not synchronous is too much of a risk. See information on that method for more details.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating <em>plag hamincha</em>. This can be sunrise or any <em>alos</em> passed to
    ///         this method.</param>
    /// <param name="endOfDay">the end of day for calculating <em>plag hamincha</em>. This can be sunset or any <em>tzais</em> passed to this
    ///         method.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em> based on the start and end of day times
    ///         passed to this method. If the calculation can't be computed such as in the Arctic Circle where there is
    ///         at least one day a year where the sun does not rise, and one where it does not set, a <c>null</c>
    ///         will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetPlagHamincha(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    public virtual DateTimeOffset? GetPlagHamincha(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay)
    {
        return ToZone(GetPlagHamincha(startOfDay, endOfDay, false));
    }

    /// <summary>
    /// This method returns <em>plag hamincha</em>, that is 10.75 * <see cref="GetShaahZmanisGRA"><em>shaos zmaniyos</em></see>
    /// (solar hours) after <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> or <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> (depending on
    /// the <see cref="UseElevation"/> setting), according to the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon"
    /// >GRA</a>. <em>Plag hamincha</em> is the earliest time that <em>Shabbos</em> can be started.
    /// The day is calculated from <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> to <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level
    /// sunset</see> or <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> to <see cref="AstronomicalCalendar.GetSunset">sunset</see> (depending on the <see cref="UseElevation"/>
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>plag hamincha</em>. If the calculation can't be computed such as
    ///         in the Arctic Circle where there is at least one day a year where the sun does not rise, and one where it
    ///         does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="GetPlagHamincha(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="GetPlagHamincha(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetPlagHaminchaBaalHatanya"/>
    public virtual DateTimeOffset? GetPlagHaminchaGRA()
    {
        return ToZone(GetPlagHamincha(GetSunriseBasedOnElevationSetting(), GetSunsetBasedOnElevationSetting(), true));
    }

    /// <summary>
    /// A method that returns a <em>shaah zmanis</em> (<see cref="AstronomicalCalendar.GetTemporalHour()">temporal hour</see>) according to
    /// the opinion of the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>. This calculation divides the day
    /// based on the opinion of the <em>GRA</em> that the day runs from <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> to
    /// <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> or <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> to <see cref="AstronomicalCalendar.GetSunset">sunset</see> (depending
    /// on the <see cref="UseElevation"/> setting). The day is split into 12 equal parts with each one being a <em>shaah zmanis</em>.
    /// This method is similar to <see cref="AstronomicalCalendar.GetTemporalHour()"/>, but can account for elevation.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em> calculated from sunrise to sunset. If the calculation can't be
    ///         computed such as in the Arctic Circle where there is at least one day a year where the sun does not rise, and one
    ///         where it does not set, a <c>null</c> will be returned. See  detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="AstronomicalCalendar.GetTemporalHour()"/>
    /// <seealso cref="AstronomicalCalendar.GetSeaLevelSunrise"/>
    /// <seealso cref="AstronomicalCalendar.GetSeaLevelSunset"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetShaahZmanisBaalHatanya"/>
    public virtual TimeSpan? GetShaahZmanisGRA()
    {
        return GetTemporalHour(GetSunriseBasedOnElevationSetting(), GetSunsetBasedOnElevationSetting());
    }

    /// <summary>
    /// A utility method to return <em>alos</em> (dawn) or <em>tzais</em> (dusk) based on a fractional day offset. As an
    /// example passing 1.5 to this method as done in the <see cref="ComprehensiveZmanimCalendar.GetTzais90Zmanis"/> will return
    /// the time 90 minutes <em>zmaniyos</em> after <see cref="GetSunsetBasedOnElevationSetting"/>, a <em>zman</em> known as
    /// the <em>achtel zman</em> or 1/8th of the length of the day (12 * 60 = 720-minute day / 8 = 90 or 1.5 hours
    /// <em>zmaniyos</em>) after sunset.
    /// </summary>
    /// <param name="hours">the number of <em>shaos zmaniyos</em> (temporal hours) before sunrise or after sunset that defines dawn
    ///        or dusk. If a negative number is passed in, it will return the time of <em>alos</em> (dawn) (subtracting the
    ///        time from sunrise) and if a positive number is passed in, it will return the time of <em>tzais</em> (dusk)
    ///        (adding the time to sunset). If 0 is passed in, a <c>null</c> will be returned (since we can't tell if it
    ///        is sunrise or sunset based).</param>
    /// <returns>the <see cref="DateTimeOffset"/> representing the time. If the calculation can't be computed such as in the Arctic
    ///         Circle where there is at least one day a year where the sun does not rise, and one where it does not set,
    ///         a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation. A <c>null</c> will also be returned if 0 is passed in, since we can't tell if it is a
    ///         sunrise or sunset based <em>zman</em>.</returns>
    /// <remarks>
    /// See the <c>...Zmanis()</c> methods of <see cref="ComprehensiveZmanimCalendar"/>.
    /// based <em>zmanim</em>.
    /// </remarks>
    public virtual DateTimeOffset? GetZmanisBasedOffset(double hours)
    {
        TimeSpan? shaahZmanis = GetShaahZmanisGRA();
        if (shaahZmanis == null || hours == 0)
        {
            return null;
        }

        long totalNanos = (long)(shaahZmanis.ToNanos() * hours);
        TimeSpan? offsetDuration = JavaTime.DurationOfNanos(totalNanos);

        if (hours > 0)
        {
            return ToZone(GetTimeOffset(GetSunsetBasedOnElevationSetting(), offsetDuration));
        }
        else
        {
            return ToZone(GetTimeOffset(GetSunriseBasedOnElevationSetting(), offsetDuration));
        }
    }

    /// <summary>
    /// A method that returns a <em>shaah zmanis</em> (temporal hour) according to the opinion of the <em><a href=
    /// "https://en.wikipedia.org/wiki/Avraham_Gombiner">Magen Avraham (MGA)</a></em> based on a 72-minute <em>alos</em>
    /// and <em>tzais</em>. This calculation divides the day that runs from dawn to dusk (for <em>sof zman krias shema</em> and
    /// <em>tfila</em>). Dawn for this calculation is 72 minutes before <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> or <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> (depending on the <see cref="UseElevation"/> elevation setting) and dusk is 72 minutes after <see cref="AstronomicalCalendar.GetSunset">sunset</see> or <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> (depending on the <see cref="UseElevation"/> elevation
    /// setting). This day is split into 12 equal parts with each part being a <em>shaah zmanis</em>. Alternate methods of calculating
    /// a <em>shaah zmanis</em> according to the Magen Avraham (MGA) are available in the subclass <see cref="ComprehensiveZmanimCalendar"/>.
    /// </summary>
    /// <returns>the <see cref="TimeSpan"/> of a <em>shaah zmanis</em>. If the calculation can't be computed
    ///         such as in the Arctic Circle where there is at least one day a year where the sun does not rise, and one
    ///         where it does not set, a <c>null</c> will be returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual TimeSpan? GetShaahZmanis72Minutes()
    {
        return GetTemporalHour(GetAlos72Minutes(), GetTzais72Minutes());
    }

    /// <summary>
    /// Default constructor will set a default <see cref="GeoLocation.GeoLocation(string, double, double, TimeZoneInfo)"/>, a default
    /// <see cref="AstronomicalCalculator.GetDefault">AstronomicalCalculator</see> and default the calendar to the current date.
    /// </summary>
    /// <seealso cref="AstronomicalCalendar.AstronomicalCalendar()"/>
    public ZmanimCalendar()
    {
    }

    /// <summary>
    /// A constructor that takes a <see cref="GeoLocation"/> as a parameter.
    /// </summary>
    /// <param name="location">the location</param>
    public ZmanimCalendar(GeoLocation location) : base(location)
    {
    }

    /// <summary>
    /// A method to get the offset in minutes before <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> which
    /// is used in calculating candle lighting time. The default time used is 18 minutes before sea level sunset. Some
    /// calendars use 15 minutes, while the custom in Jerusalem is to use a 40-minute offset. Please check the local custom
    /// for candle lighting time.
    /// </summary>
    /// <value>The candle lighting offset to set in minutes.</value>
    /// <remarks>
    /// A method to set the offset in minutes before <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> that is
    /// used in calculating candle lighting time. The default time used is 18 minutes before sunset. Some calendars use 15
    /// minutes, while the custom in Jerusalem is to use a 40-minute offset.
    /// </remarks>
    /// <seealso cref="GetCandleLighting"/>
    /// <seealso cref="CandleLightingOffset"/>
    public double CandleLightingOffset
    {
        get => candleLightingOffset;
        set => candleLightingOffset = value;
    }


    /// <summary>
    /// This is a utility method to determine if the current Instant passed in has a <em>melacha</em> (work) prohibition.
    /// Since there are many opinions on the time of <em>tzais</em>, the <em>tzais</em> for the current day has to be passed to this
    /// class. Sunset is the classes current day's <see cref="GetSunsetBasedOnElevationSetting">elevation adjusted sunset</see> that observes the
    /// <see cref="UseElevation"/> settings. The <see cref="JewishCalendar.InIsrael"/> will be set by the inIsrael parameter. Since there
    /// is no consensus on the day boundaries in polar regions, if a boundary needed for the date (sunset on <em>erev Shabbos</em> or
    /// <em>Yom Tov</em>, <em>tzais</em> on <em>Shabbos</em> or <em>Yom Tov</em>) can't be determined, an <see cref="InvalidOperationException"/>
    /// is thrown rather than guessing.
    /// </summary>
    /// <param name="currentTime">the current time</param>
    /// <param name="tzais">the time of tzais</param>
    /// <param name="inIsrael">whether to use Israel holiday scheme or not</param>
    /// <returns>true if <em>melacha</em> is prohibited or false if it is not.</returns>
    /// <exception cref="InvalidOperationException">if sunset (on <em>erev Shabbos</em> or <em>Yom Tov</em>) or <em>tzais</em> (on <em>Shabbos</em>
    ///         or <em>Yom Tov</em>) is needed but can't be determined, such as in polar regions.</exception>
    /// <seealso cref="JewishCalendar.IsAssurBemelacha"/>
    /// <seealso cref="JewishCalendar.HasCandleLighting"/>
    /// <seealso cref="JewishCalendar.InIsrael"/>
    public virtual bool IsAssurBemelacha(DateTimeOffset? currentTime, DateTimeOffset? tzais, bool inIsrael)
    {
        JewishCalendar jewishCalendar = new JewishCalendar();
        jewishCalendar.SetGregorianDate(LocalDate);

        jewishCalendar.InIsrael = inIsrael;

        bool needsSunset = jewishCalendar.HasCandleLighting(); // erev shabbos, YT or YT sheni
        bool needsTzais = jewishCalendar.IsAssurBemelacha(); // shabbos or YT
        DateTimeOffset? sunset = needsSunset ? GetSunsetBasedOnElevationSetting() : null;

        bool noSunset = needsSunset && sunset == null;
        bool noTzais = needsTzais && tzais == null;
        if (noSunset || (noTzais && (sunset == null || currentTime.IsBefore(sunset))))
        { // tzais is irrelevant once past shkiah on erev Shabbos/Yom Tov
            string missing = noSunset && noTzais ? "Sunset and tzais" : noSunset ? "Sunset" : "Tzais";
            throw new InvalidOperationException(missing + " can't be determined for " + LocalDate
                    + " at this location; whether melacha is prohibited can't be determined");
        }

        if (needsSunset && !currentTime.IsBefore(sunset))
        { // after shkiah
            return true;
        }
        return needsTzais && !currentTime.IsAfter(tzais); // before tzais
    }

    /// <summary>
    /// A method that returns the <a href="https://en.wikipedia.org/wiki/Shneur_Zalman_of_Liadi">Baal Hatanya</a>'s <em>netz amiti</em>
    /// (sunrise) without <see cref="AstronomicalCalculator.GetElevationAdjustment"/>. This forms the base for the Baal Hatanya's
    /// dawn-based calculations that are calculated as a dip below the horizon before sunrise. According to the Baal Hatanya, <em>netz
    /// amiti</em>, or true (halachic) sunrise, is when the top of the sun's disk is visible at an elevation similar to the mountains
    /// of Eretz Yisrael. The time is calculated as the point at which the center of the sun's disk is 1.583° below the horizon. This
    /// degree-based calculation can be found in Rabbi Shalom DovBer Levine's commentary on The <a href=
    /// "https://www.chabadlibrary.org/books/pdf/Seder-Hachnosas-Shabbos.pdf">Baal Hatanya's Seder Hachnasas Shabbos</a>. From an
    /// elevation of 546 meters, the top of <a href="https://en.wikipedia.org/wiki/Mount_Carmel">Har Hacarmel</a>, the sun disappears
    /// when it is 1° 35' or 1.583° below the sea level horizon. This in turn is based on the Gemara <a href=
    /// "https://hebrewbooks.org/shas.aspx?mesechta=2&amp;daf=35">Shabbos 35a</a>. There are other opinions brought down by Rabbi Levine,
    /// including Rabbi Yosef Yitzchok Feigelstock who calculates it as the degrees below the horizon 4 minutes after sunset in
    /// Yerushalayim (on the equinox). That is brought down as 1.583°. This is identical to the 1° 35' <em>zman</em> and is probably a
    /// typo and should be 1.683°. These calculations are used by most <a href="https://en.wikipedia.org/wiki/Chabad">Chabad</a>
    /// calendars that use the Baal Hatanya's <em>zmanim</em>. See
    /// <a href="https://www.chabad.org/library/article_cdo/aid/3209349/jewish/About-Our-Zmanim-Calculations.htm">About Our
    /// <em>Zmanim</em> Calculations @ Chabad.org</a>.
    ///
    /// Note: <em>netz amiti</em> is used only for calculating certain <em>zmanim</em>, and is intentionally unpublished. For
    /// practical purposes, daytime <em>mitzvos</em> like <em>shofar</em> and <em>lulav</em> should not be done until after the
    /// published time for <em>netz</em> / sunrise.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the exact sea level <em>netz amiti</em> (sunrise) time. If the calculation can't
    ///         be computed such as in the Arctic Circle where there is at least one day a year where the sun does not rise, and one
    ///         where it does not set, a <c>null</c> will be returned. See detailed explanation on top of the page.</returns>
    /// <seealso cref="AstronomicalCalendar.GetSunrise"/>
    /// <seealso cref="AstronomicalCalendar.GetSeaLevelSunrise"/>
    /// <seealso cref="GetSunsetBaalHatanya"/>
    /// <seealso cref="Zenith1Point583"/>
    protected virtual DateTimeOffset? GetSunriseBaalHatanya()
    {
        return GetSunriseOffsetByDegrees(Zenith1Point583);
    }

    /// <summary>
    /// A method that returns the <a href="https://en.wikipedia.org/wiki/Shneur_Zalman_of_Liadi">Baal Hatanya</a>'s <em>shkiah
    /// amiti</em> (sunset) without <see cref="AstronomicalCalculator.GetElevationAdjustment">elevation adjustment</see>. This forms
    /// the base for the Baal Hatanya's dusk-based calculations that are calculated as a dip below the horizon after sunset. According
    /// to the Baal Hatanya, <em>shkiah amiti</em>, true (<em>halachic</em>) sunset, is when the top of the sun's disk disappears from
    /// view at an elevation similar to the mountains of <em>Eretz Yisrael</em>. This time is calculated as the point at which the
    /// center of the sun's disk is 1.583° below the horizon.
    ///
    /// Note: <em>shkiah amiti</em> is used only for calculating certain <em>zmanim</em>, and is intentionally unpublished. For
    /// practical purposes, all daytime mitzvos should be completed before the published time for <em>shkiah</em> / sunset. For further
    /// explanation of the calculations used for the Baal Hatanya's <em>zmanim</em> in this library, see <a href=
    /// "https://www.chabad.org/library/article_cdo/aid/3209349/jewish/About-Our-Zmanim-Calculations.htm">About Our <em>Zmanim</em>
    /// Calculations @ Chabad.org</a>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the exact sea level <em>shkiah amiti</em> (sunset) time. If the calculation can't
    ///         be computed such as in the Arctic Circle where there is at least one day a year where the sun does not rise, and one
    ///         where it does not set, a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="AstronomicalCalendar.GetSunset"/>
    /// <seealso cref="AstronomicalCalendar.GetSeaLevelSunset"/>
    /// <seealso cref="GetSunriseBaalHatanya"/>
    /// <seealso cref="Zenith1Point583"/>
    protected virtual DateTimeOffset? GetSunsetBaalHatanya()
    {
        return GetSunsetOffsetByDegrees(Zenith1Point583);
    }

    /// <summary>
    /// A generic utility method for calculating any <em>shaah zmanis</em> (temporal hour) based <em>zman</em> with the day
    /// defined as the start and end of day (or night) and the number of <em>shaos zmaniyos</em> passed to the method. This
    /// simplifies the code in other methods such as <see cref="GetPlagHamincha(Nullable{DateTimeOffset}, Nullable{DateTimeOffset}, bool)"/> and cuts down on code replication.
    /// As an example, passing <see cref="AstronomicalCalendar.GetSunrise">sunrise</see> and <see cref="AstronomicalCalendar.GetSunset">sunset</see> or <see cref="AstronomicalCalendar.GetSeaLevelSunrise">sea level sunrise</see> and <see cref="AstronomicalCalendar.GetSeaLevelSunset">sea level sunset</see> (depending on the <see cref="UseElevation"/> elevation setting) and 10.75 hours to this method will return <em>plag mincha</em> according to the
    /// opinion of the <a href="https://en.wikipedia.org/wiki/Vilna_Gaon">GRA</a>.
    /// </summary>
    /// <param name="startOfDay">the start of day for calculating the <em>zman</em>. This can be sunrise or any <em>alos</em> passed to
    ///         this method.</param>
    /// <param name="endOfDay">the end of day for calculating the <em>zman</em>. This can be sunset or any <em>tzais</em> passed to this
    ///         method.</param>
    /// <param name="hours">the number of <em>shaos zmaniyos</em> (temporal hours) to offset from the start of day</param>
    /// <returns>the <see cref="DateTimeOffset"/> of the time of <em>zman</em> with the <em>shaos zmaniyos</em> (temporal hours)
    ///         in the day offset from the start of day passed to this method. If the calculation can't be computed such
    ///         as in the Arctic Circle where there is at least one day a year where the sun does not rise, and one
    ///         where it does not set, a <c>null</c> will be  returned. See detailed explanation on top of the
    ///         <see cref="AstronomicalCalendar"/> documentation.</returns>
    public virtual DateTimeOffset? GetShaahZmanisBasedZman(DateTimeOffset? startOfDay, DateTimeOffset? endOfDay, double hours)
    {
        TimeSpan? shaahZmanis = GetTemporalHour(startOfDay, endOfDay);
        if (shaahZmanis == null)
        { //defensive, should not be needed
            return null;
        }

        long totalNanos = (long)(shaahZmanis.ToNanos() * hours);
        return ToZone(GetTimeOffset(startOfDay, JavaTime.DurationOfNanos(totalNanos)));
    }


    /// <summary>
    /// A utility method that returns the percentage of a <em>shaah zmanis</em> after sunset (or before sunrise) for a given degree
    /// offset. For the <a href="https://kosherjava.com/2022/01/12/equinox-vs-equilux-zmanim-calculations/">equilux</a> where there
    /// is a 720-minute day, passing 16.1° for the location of Jerusalem will return about 1.2. This will work for any location or
    /// date, but will typically only be of interest at the equinox/equilux to calculate the percentage of a <em>shaah zmanis</em> for
    /// those who want to use the <a href="https://en.wikipedia.org/wiki/Abraham_Cohen_Pimentel">Minchas Cohen</a> in Ma'amar 2:4 and
    /// the <a href="https://en.wikipedia.org/wiki/Hezekiah_da_Silva">Pri Chadash</a> who calculate <em>tzais</em> as a percentage of
    /// the day after sunset. While the Minchas Cohen only applies this to 72 minutes or a 1/10 of the day around the world (based on
    /// the equinox / equilux in Israel), this method allows calculations for any degree level for any location.
    /// </summary>
    /// <param name="degrees">the number of degrees below the horizon after sunset.</param>
    /// <param name="sunset">if <c>true</c> the calculation is in degrees after sunset, otherwise it is degrees before sunrise.</param>
    /// <returns>the <c>double</c> percentage of a <em>sha'ah zmanis</em> for a given set of degrees below the astronomical horizon
    ///         for the current calendar.  If the calculation can't be computed <see cref="double.Epsilon"/> (the smallest positive <see cref="double"/>, Java's <c>Double.MIN_VALUE</c>) will be returned. See detailed
    ///         explanation on top of the page.</returns>
    public virtual double GetPercentOfShaahZmanisFromDegrees(double degrees, bool sunset)
    {
        DateTimeOffset? seaLevelSunrise = GetSeaLevelSunrise();
        DateTimeOffset? seaLevelSunset = GetSeaLevelSunset();
        DateTimeOffset? twilight;
        if (sunset)
        {
            twilight = GetSunsetOffsetByDegrees(GeometricZenith + degrees);
        }
        else
        {
            twilight = GetSunriseOffsetByDegrees(GeometricZenith + degrees);
        }
        if (seaLevelSunrise == null || seaLevelSunset == null || twilight == null)
        {
            return double.Epsilon;
        }
        double shaahZmanis = (seaLevelSunset.ToEpochMilli() - seaLevelSunrise.ToEpochMilli()) / 12.0;
        long riseSetToTwilight;
        if (sunset)
        {
            riseSetToTwilight = twilight.ToEpochMilli() - seaLevelSunset.ToEpochMilli();
        }
        else
        {
            riseSetToTwilight = seaLevelSunrise.ToEpochMilli() - twilight.ToEpochMilli();
        }
        return riseSetToTwilight / shaahZmanis;
    }

    /// <summary>
    /// A utility method to calculate <em>zmanim</em> based on <a href="https://en.wikipedia.org/wiki/Moshe_Feinstein">Rav Moshe
    /// Feinstein</a> and others as calculated in <a href="https://en.wikipedia.org/wiki/Mesivtha_Tifereth_Jerusalem">MTJ</a>, <a href=
    /// "https://en.wikipedia.org/wiki/Mesivtha_Tifereth_Jerusalem">Yeshiva of Staten Island</a>, and Camp Yeshiva
    /// of Staten Island and other calendars. The day is split in two, from <em>alos</em> / sunrise to <em>chatzos</em>, and the
    /// second half of the day, from <em>chatzos</em> to sunset / <em>tzais</em>. Morning based times are calculated. based on the first
    /// 6 hours of the day, and afternoon times based on the second half of the day. As an example, passing 0.5, a start of
    /// <em>chatzos</em> and an end of day as sunset will return the time of <em>mincha gedola</em> GRA as half an hour <em>zmanis</em>
    /// based on the second half of the day. Some <em>zmanim</em> calculations can be based on subtracting <em>shaos zmaniyos</em>
    /// from the end of the day, and that is supported by passing a negative hour to this method.
    /// </summary>
    /// <param name="startOfHalfDay">The start of the half day. This would be <em>alos</em> or sunrise for morning based times such as
    ///         <em>sof zman krias shema</em> and <em>chatzos</em> for afternoon based times such as <em>mincha gedola</em>.</param>
    /// <param name="endOfHalfDay">The end of the half day. This would be <em>chatzos</em> for morning based times  such as <em>sof zman
    ///         krias shema</em>  and sunset or <em>tzais</em> for afternoon based times such as <em>mincha gedola</em>.</param>
    /// <param name="hours">The number of <em>shaos zmaniyos</em> (hours) to offset the beginning of the first or second half of the day. For
    ///         example, 3 for <em>sof zman Shma</em>, 0.5 for <em>mincha gedola</em> (half an hour after <em>chatzos</em>) and 4.75
    ///         for <em>plag hamincha</em>. If the number of hours is negative, it will subtract the number of <em>shaos zmaniyos</em>
    ///         from the end of the day.</param>
    /// <returns>the <see cref="DateTimeOffset"/> of <em>zman</em> based on calculation of the first or second half of the day. If the
    ///         calculation can't be computed such as in the Arctic Circle where there is at least one day a year where the
    ///         sun does not rise, and one where it does not set, a <c>null</c> will be returned. See detailed explanation
    ///         on top of the <see cref="AstronomicalCalendar"/> documentation.</returns>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetFixedLocalChatzosHayom"/>
    public virtual DateTimeOffset? GetHalfDayBasedZman(DateTimeOffset? startOfHalfDay, DateTimeOffset? endOfHalfDay, double hours)
    {
        if (startOfHalfDay == null || endOfHalfDay == null)
        {
            return null;
        }
        TimeSpan? shaahZmanis = GetHalfDayBasedShaahZmanis(startOfHalfDay, endOfHalfDay);
        if (shaahZmanis == null)
        { //defensive, should not be needed
            return null;
        }

        long totalNanos = (long)(shaahZmanis.ToNanos() * hours);
        TimeSpan? offsetDuration = JavaTime.DurationOfNanos(totalNanos);

        if (hours >= 0)
        { // forward from start of day
            return ToZone(GetTimeOffset(startOfHalfDay, offsetDuration));
        }
        else
        { // subtract from end of day
            return ToZone(GetTimeOffset(endOfHalfDay, offsetDuration));
        }
    }

    /// <summary>
    /// A utility method to calculate the length of a <em>sha'ah zmanis</em> based on 1/6 of a 6-hour day.
    /// </summary>
    /// <param name="startOfHalfDay">The start of the half-day. This would be <em>alos</em> or sunrise for the first half of the day, or
    ///         <em>chatzos</em> for the second half of the day.</param>
    /// <param name="endOfHalfDay">The end of the half-day. This would be <em>chatzos</em> for the first half of the day, or sunset or
    ///         <em>tzais</em> for the second half of the day.</param>
    /// <returns>The <see cref="TimeSpan"/> of a <em>shaah zmanis</em> based on 1/6 of a half-day. If the calculation can't be computed
    ///         such as in the Arctic Circle where there is at least one day a year where the sun does not rise, and one where it does
    ///         not set, a <c>null</c> will be returned. See detailed explanation on top of the <see cref="AstronomicalCalendar"/>
    ///         documentation.</returns>
    /// <remarks>
    /// <b>To do:</b> Consider adjusting various shaah zmanis times to use this.
    /// </remarks>
    /// <seealso cref="GetHalfDayBasedZman"/>
    /// <seealso cref="UseAstronomicalChatzosForOtherZmanim"/>
    public virtual TimeSpan? GetHalfDayBasedShaahZmanis(DateTimeOffset? startOfHalfDay, DateTimeOffset? endOfHalfDay)
    {
        if (startOfHalfDay == null || endOfHalfDay == null)
        {
            return null;
        }
        return JavaTime.Between(startOfHalfDay, endOfHalfDay).DividedBy(6);
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
        ZmanimCalendar that = (ZmanimCalendar)obj;
        return useElevation == that.useElevation
                && useAstronomicalChatzos == that.useAstronomicalChatzos
                && useAstronomicalChatzosForOtherZmanim == that.useAstronomicalChatzosForOtherZmanim
                && candleLightingOffset == that.candleLightingOffset;
    }

    /// <seealso cref="object.GetHashCode"/>
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), useElevation, useAstronomicalChatzos,
                useAstronomicalChatzosForOtherZmanim, candleLightingOffset.GetHashCode());
    }
}
