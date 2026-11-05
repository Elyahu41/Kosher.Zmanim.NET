/*
 * Zmanim Java API
 * Copyright © 2011 - 2026 Eliyahu Hershfeld
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; version 2.1 of the License.
 *
 * This library is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not, write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA,
 * or connect to: http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html
 */
using System.Globalization;
using System.Text;

namespace Kosher.Zmanim.HebrewCalendar;

/// <summary>
/// The HebrewDateFormatter class formats a <see cref="JewishDate"/>. The class formats Jewish dates, numbers, <em>Daf Yomi</em>
/// (<em>Bavli</em> and <em>Yerushalmi</em>), the <em>Omer</em>, <em>Parshas Hashavua</em> (including the special <em>parshiyos</em>
/// of <em>Shekalim</em>, <em>Zachor</em>, <em>Parah</em> and <em>Hachodesh</em>), <em>Yomim Tovim</em> in Hebrew or Latin chars, and
/// has various settings. Sample full date output includes (using various options):
/// <list type="bullet">
/// <item><description>21 Shevat, 5729</description></item>
/// <item><description>כא שבט תשכט</description></item>
/// <item><description>כ״א שבט ה׳תשכ״ט</description></item>
/// <item><description>כ״א שבט תש״פ or כ״א שבט תש״ף</description></item>
/// <item><description>כ׳ שבט ו׳ אלפים</description></item>
/// </list>
/// </summary>
/// <seealso cref="JewishDate"/>
/// <seealso cref="JewishCalendar"/>
public class HebrewDateFormatter
{

    /// <summary>
    /// See <see cref="IsHebrewFormat"/> and <see cref="IsHebrewFormat"/>.
    /// </summary>
    private bool hebrewFormat = false;

    /// <summary>
    /// See <see cref="UseLongHebrewYears"/> and <see cref="UseLongHebrewYears"/>.
    /// </summary>
    private bool useLonghebrewYears = false;

    /// <summary>
    /// See <see cref="UseGershGershayim"/> and <see cref="UseGershGershayim"/>.
    /// </summary>
    private bool useGershGershayim = true;

    /// <summary>
    /// See <see cref="IsLongWeekFormat"/> and <see cref="IsLongWeekFormat"/>.
    /// </summary>
    private bool longWeekFormat = true;

    /// <summary>
    /// See <see cref="UseFinalFormLetters"/> and <see cref="UseFinalFormLetters"/>.
    /// </summary>
    private bool useFinalFormLetters = false;

    /// <summary>
    /// The internal DateFormat. See <see cref="IsLongWeekFormat"/> and <see cref="IsLongWeekFormat"/>.
    /// </summary>
    private string weekFormat;

    /// <summary>
    /// List of transliterated parshiyos using the default <em>Ashkenazi</em> pronunciation. For information on the format, see
    /// <see cref="TransliteratedParshiyosList"/>.
    /// </summary>
    /// <seealso cref="TransliteratedParshiyosList"/>
    /// <seealso cref="HebrewDateFormatter"/>
    private Dictionary<JewishCalendar.Parshah, string> transliteratedParshahMap;

    /// <summary>
    /// An <see cref="Dictionary{TKey, TValue}"/> of Hebrew <em>parshiyos</em>. The list includes double and special <em>parshiyos</em> and contains<br/>
    /// <c>&#x200F;בראשית, נח, לך לך,וירא, חיי שרה,תולדות, ויצא, וישלח,וישב, מקץ, ויגש, ויחי,שמות, וארא, בא, בשלח,יתרו, משפטים, תרומה,תצוה, כי תשא, ויקהל,פקודי,
    /// ויקרא, צו,שמיני, תזריע, מצרע,אחרי מות, קדושים,אמור, בהר, בחקתי,במדבר, נשא, בהעלתך,שלח לך, קרח, חוקת, בלק,פינחס, מטות, מסעי,דברים, ואתחנן, עקב,ראה, שופטים, כי תצא,כי
    /// תבוא, נצבים, וילך,האזינו, וזאת הברכה,ויקהל פקודי, תזריעמצרע, אחרי מותקדושים, בהר בחקתי,חוקת בלק, מטות מסעי,נצבים וילך,
    /// שקלים,זכור, פרה, החדש,שובה,שירה,הגדול,חזון,נחמו</c>
    /// </summary>
    private Dictionary<JewishCalendar.Parshah, string> hebrewParshahMap;

    /// <summary>
    /// Default constructor sets the <see cref="Dictionary{TKey, TValue}"/>s of Hebrew and default transliterated parshiyos.
    /// </summary>
    public HebrewDateFormatter()
    {
        weekFormat = "dddd";
        transliteratedParshahMap = new Dictionary<JewishCalendar.Parshah, string>();
        transliteratedParshahMap[JewishCalendar.Parshah.None] = "";
        transliteratedParshahMap[JewishCalendar.Parshah.Bereshis] = "Bereshis";
        transliteratedParshahMap[JewishCalendar.Parshah.Noach] = "Noach";
        transliteratedParshahMap[JewishCalendar.Parshah.LechLecha] = "Lech Lecha";
        transliteratedParshahMap[JewishCalendar.Parshah.Vayera] = "Vayera";
        transliteratedParshahMap[JewishCalendar.Parshah.ChayeiSara] = "Chayei Sara";
        transliteratedParshahMap[JewishCalendar.Parshah.Toldos] = "Toldos";
        transliteratedParshahMap[JewishCalendar.Parshah.Vayetzei] = "Vayetzei";
        transliteratedParshahMap[JewishCalendar.Parshah.Vayishlach] = "Vayishlach";
        transliteratedParshahMap[JewishCalendar.Parshah.Vayeshev] = "Vayeshev";
        transliteratedParshahMap[JewishCalendar.Parshah.Miketz] = "Miketz";
        transliteratedParshahMap[JewishCalendar.Parshah.Vayigash] = "Vayigash";
        transliteratedParshahMap[JewishCalendar.Parshah.Vayechi] = "Vayechi";
        transliteratedParshahMap[JewishCalendar.Parshah.Shemos] = "Shemos";
        transliteratedParshahMap[JewishCalendar.Parshah.Vaera] = "Vaera";
        transliteratedParshahMap[JewishCalendar.Parshah.Bo] = "Bo";
        transliteratedParshahMap[JewishCalendar.Parshah.Beshalach] = "Beshalach";
        transliteratedParshahMap[JewishCalendar.Parshah.Yisro] = "Yisro";
        transliteratedParshahMap[JewishCalendar.Parshah.Mishpatim] = "Mishpatim";
        transliteratedParshahMap[JewishCalendar.Parshah.Terumah] = "Terumah";
        transliteratedParshahMap[JewishCalendar.Parshah.Tetzaveh] = "Tetzaveh";
        transliteratedParshahMap[JewishCalendar.Parshah.KiSisa] = "Ki Sisa";
        transliteratedParshahMap[JewishCalendar.Parshah.Vayakhel] = "Vayakhel";
        transliteratedParshahMap[JewishCalendar.Parshah.Pekudei] = "Pekudei";
        transliteratedParshahMap[JewishCalendar.Parshah.Vayikra] = "Vayikra";
        transliteratedParshahMap[JewishCalendar.Parshah.Tzav] = "Tzav";
        transliteratedParshahMap[JewishCalendar.Parshah.Shmini] = "Shmini";
        transliteratedParshahMap[JewishCalendar.Parshah.Tazria] = "Tazria";
        transliteratedParshahMap[JewishCalendar.Parshah.Metzora] = "Metzora";
        transliteratedParshahMap[JewishCalendar.Parshah.AchreiMos] = "Achrei Mos";
        transliteratedParshahMap[JewishCalendar.Parshah.Kedoshim] = "Kedoshim";
        transliteratedParshahMap[JewishCalendar.Parshah.Emor] = "Emor";
        transliteratedParshahMap[JewishCalendar.Parshah.Behar] = "Behar";
        transliteratedParshahMap[JewishCalendar.Parshah.Bechukosai] = "Bechukosai";
        transliteratedParshahMap[JewishCalendar.Parshah.Bamidbar] = "Bamidbar";
        transliteratedParshahMap[JewishCalendar.Parshah.Nasso] = "Nasso";
        transliteratedParshahMap[JewishCalendar.Parshah.Behaaloscha] = "Beha'aloscha";
        transliteratedParshahMap[JewishCalendar.Parshah.Shlach] = "Sh'lach";
        transliteratedParshahMap[JewishCalendar.Parshah.Korach] = "Korach";
        transliteratedParshahMap[JewishCalendar.Parshah.Chukas] = "Chukas";
        transliteratedParshahMap[JewishCalendar.Parshah.Balak] = "Balak";
        transliteratedParshahMap[JewishCalendar.Parshah.Pinchas] = "Pinchas";
        transliteratedParshahMap[JewishCalendar.Parshah.Matos] = "Matos";
        transliteratedParshahMap[JewishCalendar.Parshah.Masei] = "Masei";
        transliteratedParshahMap[JewishCalendar.Parshah.Devarim] = "Devarim";
        transliteratedParshahMap[JewishCalendar.Parshah.Vaeschanan] = "Vaeschanan";
        transliteratedParshahMap[JewishCalendar.Parshah.Eikev] = "Eikev";
        transliteratedParshahMap[JewishCalendar.Parshah.Reeh] = "Re'eh";
        transliteratedParshahMap[JewishCalendar.Parshah.Shoftim] = "Shoftim";
        transliteratedParshahMap[JewishCalendar.Parshah.KiSeitzei] = "Ki Seitzei";
        transliteratedParshahMap[JewishCalendar.Parshah.KiSavo] = "Ki Savo";
        transliteratedParshahMap[JewishCalendar.Parshah.Nitzavim] = "Nitzavim";
        transliteratedParshahMap[JewishCalendar.Parshah.Vayeilech] = "Vayeilech";
        transliteratedParshahMap[JewishCalendar.Parshah.Haazinu] = "Ha'Azinu";
        transliteratedParshahMap[JewishCalendar.Parshah.VzosHaberacha] = "Vezos Habracha";
        transliteratedParshahMap[JewishCalendar.Parshah.VayakhelPekudei] = "Vayakhel Pekudei";
        transliteratedParshahMap[JewishCalendar.Parshah.TazriaMetzora] = "Tazria Metzora";
        transliteratedParshahMap[JewishCalendar.Parshah.AchreiMosKedoshim] = "Achrei Mos Kedoshim";
        transliteratedParshahMap[JewishCalendar.Parshah.BeharBechukosai] = "Behar Bechukosai";
        transliteratedParshahMap[JewishCalendar.Parshah.ChukasBalak] = "Chukas Balak";
        transliteratedParshahMap[JewishCalendar.Parshah.MatosMasei] = "Matos Masei";
        transliteratedParshahMap[JewishCalendar.Parshah.NitzavimVayeilech] = "Nitzavim Vayeilech";
        transliteratedParshahMap[JewishCalendar.Parshah.Shkalim] = "Shekalim";
        transliteratedParshahMap[JewishCalendar.Parshah.Zachor] = "Zachor";
        transliteratedParshahMap[JewishCalendar.Parshah.Para] = "Parah";
        transliteratedParshahMap[JewishCalendar.Parshah.Hachodesh] = "Hachodesh";
        transliteratedParshahMap[JewishCalendar.Parshah.Shuva] = "Shuva";
        transliteratedParshahMap[JewishCalendar.Parshah.Shira] = "Shira";
        transliteratedParshahMap[JewishCalendar.Parshah.Hagadol] = "Hagadol";
        transliteratedParshahMap[JewishCalendar.Parshah.Chazon] = "Chazon";
        transliteratedParshahMap[JewishCalendar.Parshah.Nachamu] = "Nachamu";

        hebrewParshahMap = new Dictionary<JewishCalendar.Parshah, string>();
        hebrewParshahMap[JewishCalendar.Parshah.None] = "";
        hebrewParshahMap[JewishCalendar.Parshah.Bereshis] = "בראשית";
        hebrewParshahMap[JewishCalendar.Parshah.Noach] = "נח";
        hebrewParshahMap[JewishCalendar.Parshah.LechLecha] = "לך לך";
        hebrewParshahMap[JewishCalendar.Parshah.Vayera] = "וירא";
        hebrewParshahMap[JewishCalendar.Parshah.ChayeiSara] = "חיי שרה";
        hebrewParshahMap[JewishCalendar.Parshah.Toldos] = "תולדות";
        hebrewParshahMap[JewishCalendar.Parshah.Vayetzei] = "ויצא";
        hebrewParshahMap[JewishCalendar.Parshah.Vayishlach] = "וישלח";
        hebrewParshahMap[JewishCalendar.Parshah.Vayeshev] = "וישב";
        hebrewParshahMap[JewishCalendar.Parshah.Miketz] = "מקץ";
        hebrewParshahMap[JewishCalendar.Parshah.Vayigash] = "ויגש";
        hebrewParshahMap[JewishCalendar.Parshah.Vayechi] = "ויחי";
        hebrewParshahMap[JewishCalendar.Parshah.Shemos] = "שמות";
        hebrewParshahMap[JewishCalendar.Parshah.Vaera] = "וארא";
        hebrewParshahMap[JewishCalendar.Parshah.Bo] = "בא";
        hebrewParshahMap[JewishCalendar.Parshah.Beshalach] = "בשלח";
        hebrewParshahMap[JewishCalendar.Parshah.Yisro] = "יתרו";
        hebrewParshahMap[JewishCalendar.Parshah.Mishpatim] = "משפטים";
        hebrewParshahMap[JewishCalendar.Parshah.Terumah] = "תרומה";
        hebrewParshahMap[JewishCalendar.Parshah.Tetzaveh] = "תצוה";
        hebrewParshahMap[JewishCalendar.Parshah.KiSisa] = "כי תשא";
        hebrewParshahMap[JewishCalendar.Parshah.Vayakhel] = "ויקהל";
        hebrewParshahMap[JewishCalendar.Parshah.Pekudei] = "פקודי";
        hebrewParshahMap[JewishCalendar.Parshah.Vayikra] = "ויקרא";
        hebrewParshahMap[JewishCalendar.Parshah.Tzav] = "צו";
        hebrewParshahMap[JewishCalendar.Parshah.Shmini] = "שמיני";
        hebrewParshahMap[JewishCalendar.Parshah.Tazria] = "תזריע";
        hebrewParshahMap[JewishCalendar.Parshah.Metzora] = "מצרע";
        hebrewParshahMap[JewishCalendar.Parshah.AchreiMos] = "אחרי מות";
        hebrewParshahMap[JewishCalendar.Parshah.Kedoshim] = "קדושים";
        hebrewParshahMap[JewishCalendar.Parshah.Emor] = "אמור";
        hebrewParshahMap[JewishCalendar.Parshah.Behar] = "בהר";
        hebrewParshahMap[JewishCalendar.Parshah.Bechukosai] = "בחקתי";
        hebrewParshahMap[JewishCalendar.Parshah.Bamidbar] = "במדבר";
        hebrewParshahMap[JewishCalendar.Parshah.Nasso] = "נשא";
        hebrewParshahMap[JewishCalendar.Parshah.Behaaloscha] = "בהעלתך";
        hebrewParshahMap[JewishCalendar.Parshah.Shlach] = "שלח לך";
        hebrewParshahMap[JewishCalendar.Parshah.Korach] = "קרח";
        hebrewParshahMap[JewishCalendar.Parshah.Chukas] = "חוקת";
        hebrewParshahMap[JewishCalendar.Parshah.Balak] = "בלק";
        hebrewParshahMap[JewishCalendar.Parshah.Pinchas] = "פינחס";
        hebrewParshahMap[JewishCalendar.Parshah.Matos] = "מטות";
        hebrewParshahMap[JewishCalendar.Parshah.Masei] = "מסעי";
        hebrewParshahMap[JewishCalendar.Parshah.Devarim] = "דברים";
        hebrewParshahMap[JewishCalendar.Parshah.Vaeschanan] = "ואתחנן";
        hebrewParshahMap[JewishCalendar.Parshah.Eikev] = "עקב";
        hebrewParshahMap[JewishCalendar.Parshah.Reeh] = "ראה";
        hebrewParshahMap[JewishCalendar.Parshah.Shoftim] = "שופטים";
        hebrewParshahMap[JewishCalendar.Parshah.KiSeitzei] = "כי תצא";
        hebrewParshahMap[JewishCalendar.Parshah.KiSavo] = "כי תבוא";
        hebrewParshahMap[JewishCalendar.Parshah.Nitzavim] = "נצבים";
        hebrewParshahMap[JewishCalendar.Parshah.Vayeilech] = "וילך";
        hebrewParshahMap[JewishCalendar.Parshah.Haazinu] = "האזינו";
        hebrewParshahMap[JewishCalendar.Parshah.VzosHaberacha] = "וזאת הברכה";
        hebrewParshahMap[JewishCalendar.Parshah.VayakhelPekudei] = "ויקהל פקודי";
        hebrewParshahMap[JewishCalendar.Parshah.TazriaMetzora] = "תזריע מצרע";
        hebrewParshahMap[JewishCalendar.Parshah.AchreiMosKedoshim] = "אחרי מות קדושים";
        hebrewParshahMap[JewishCalendar.Parshah.BeharBechukosai] = "בהר בחקתי";
        hebrewParshahMap[JewishCalendar.Parshah.ChukasBalak] = "חוקת בלק";
        hebrewParshahMap[JewishCalendar.Parshah.MatosMasei] = "מטות מסעי";
        hebrewParshahMap[JewishCalendar.Parshah.NitzavimVayeilech] = "נצבים וילך";
        hebrewParshahMap[JewishCalendar.Parshah.Shkalim] = "שקלים";
        hebrewParshahMap[JewishCalendar.Parshah.Zachor] = "זכור";
        hebrewParshahMap[JewishCalendar.Parshah.Para] = "פרה";
        hebrewParshahMap[JewishCalendar.Parshah.Hachodesh] = "החדש";
        hebrewParshahMap[JewishCalendar.Parshah.Shuva] = "שובה";
        hebrewParshahMap[JewishCalendar.Parshah.Shira] = "שירה";
        hebrewParshahMap[JewishCalendar.Parshah.Hagadol] = "הגדול";
        hebrewParshahMap[JewishCalendar.Parshah.Chazon] = "חזון";
        hebrewParshahMap[JewishCalendar.Parshah.Nachamu] = "נחמו";
    }

    /// <summary>
    /// Returns if the <see cref="FormatDayOfWeek"/> will use the long format such as ראשון or short such as א when formatting
    /// the day of week in <see cref="IsHebrewFormat">Hebrew</see>.
    /// </summary>
    /// <value>the longWeekFormat to set</value>
    /// <remarks>
    /// Setting to control if the <see cref="FormatDayOfWeek"/> will use the long format such as ראשון or short such as א when
    /// formatting the day of week in <see cref="IsHebrewFormat">Hebrew</see>.
    /// </remarks>
    /// <seealso cref="IsLongWeekFormat"/>
    /// <seealso cref="FormatDayOfWeek"/>
    public bool IsLongWeekFormat
    {
        get => longWeekFormat;
        set
        {
            this.longWeekFormat = value;
            if (value)
            {
                weekFormat = "dddd";
            }
            else
            {
                weekFormat = "ddd";
            }

        }
    }

    /// <summary>
    /// The <a href="https://en.wikipedia.org/wiki/Geresh#Punctuation_mark">gersh</a> character is the ׳ char that is similar to a
    /// single quote and is used in formatting Hebrew numbers.
    /// </summary>
    private const string Geresh = "׳";

    /// <summary>
    /// The <a href="https://en.wikipedia.org/wiki/Gershayim#Punctuation_mark">gershyim</a> character is the ״ char that is similar
    /// to a double quote and is used in formatting Hebrew numbers.
    /// </summary>
    private const string Gershayim = "״";

    /// <summary>
    /// Transliterated month names that default to <c>["Nissan", "Iyar", "Sivan", "Tammuz", "Av", "Elul", "Tishrei", "Cheshvan",
    /// "Kislev", "Teves", "Shevat", "Adar", "Adar II", "Adar I" ]</c>.
    /// </summary>
    /// <seealso cref="TransliteratedMonthList"/>
    /// <seealso cref="TransliteratedMonthList"/>
    private string[] transliteratedMonths = { "Nissan", "Iyar", "Sivan", "Tammuz", "Av", "Elul", "Tishrei", "Cheshvan",
            "Kislev", "Teves", "Shevat", "Adar", "Adar II", "Adar I" };

    /// <summary>
    /// The Hebrew omer prefix charachter. It defaults to ב producing בעומר, but can be set to ל to produce לעומר (or any other prefix).
    /// </summary>
    /// <seealso cref="HebrewOmerPrefix"/>
    /// <seealso cref="HebrewOmerPrefix"/>
    private string hebrewOmerPrefix = "ב";

    /// <summary>
    /// The default value for formatting "Shabbos" (Saturday) when transliterated.
    /// </summary>
    /// <seealso cref="TransliteratedShabbosDayOfWeek"/>
    /// <seealso cref="TransliteratedShabbosDayOfWeek"/>
    private string transliteratedShabbosDayOfWeek = "Shabbos";

    /// <summary>
    /// Returns the day of Shabbos transliterated into Latin chars. The default uses Ashkenazi pronunciation "Shabbos". This can be
    /// overwritten using the <see cref="TransliteratedShabbosDayOfWeek"/>. It is uesd by <see cref="FormatDayOfWeek"/>.
    /// </summary>
    /// <value>the transliteratedShabbos to set</value>
    /// <remarks>
    /// Setter to override the default transliterated name of "Shabbos" to alternate spelling such as "Shabbat" used by
    /// the <see cref="FormatDayOfWeek"/>.
    /// </remarks>
    /// <seealso cref="TransliteratedShabbosDayOfWeek"/>
    /// <seealso cref="FormatDayOfWeek"/>
    public string TransliteratedShabbosDayOfWeek
    {
        get => transliteratedShabbosDayOfWeek;
        set => transliteratedShabbosDayOfWeek = value;
    }

    /// <summary>
    /// See <see cref="TransliteratedHolidayList"/> and <see cref="TransliteratedHolidayList"/>.
    /// </summary>
    private string[] transliteratedHolidays = {"Erev Pesach", "Pesach", "Chol Hamoed Pesach", "Pesach Sheni",
            "Erev Shavuos", "Shavuos", "Seventeenth of Tammuz", "Tishah B'Av", "Tu B'Av", "Erev Rosh Hashana",
            "Rosh Hashana", "Fast of Gedalyah", "Erev Yom Kippur", "Yom Kippur", "Erev Succos", "Succos",
            "Chol Hamoed Succos", "Hoshana Rabbah", "Shemini Atzeres", "Simchas Torah", "Erev Chanukah", "Chanukah",
            "Tenth of Teves", "Tu B'Shvat", "Fast of Esther", "Purim", "Shushan Purim", "Purim Katan", "Rosh Chodesh",
            "Yom HaShoah", "Yom Hazikaron", "Yom Ha'atzmaut", "Yom Yerushalayim", "Lag B'Omer", "Shushan Purim Katan",
            "Isru Chag"};

    /// <summary>
    /// Returns the array of <em>Yomim Tovim</em> (holidays) transliterated into Latin chars. This is used by the <see cref="FormatYomTov"/> when formatting the <em>Yom Tov</em> string. The default list of months usesnAshkenazi
    /// pronunciation in typical American English spelling. The default list is currently
    /// <c>["Erev Pesach", "Pesach", "Chol Hamoed Pesach", "Pesach Sheni", "Erev Shavuos", "Shavuos", "Seventeenth of Tammuz",
    /// "Tishah B'Av", "Tu B'Av", "Erev Rosh Hashana", "Rosh Hashana", "Fast of Gedalyah", "Erev Yom Kippur", "Yom Kippur", "Erev
    /// Succos", "Succos", "Chol Hamoed Succos", "Hoshana Rabbah", "Shemini Atzeres", "Simchas Torah", "Erev Chanukah", "Chanukah",
    /// "Tenth of Teves", "Tu B'Shvat", "Fast of Esther", "Purim", "Shushan Purim",m"Purim Katan", "Rosh Chodesh", "Yom HaShoah",
    /// "Yom Hazikaron", "Yom Ha'atzmaut", "Yom Yerushalayim", "Lag B'Omer", "Shushan Purim Katan", "Isru Chag"]</c>.
    /// </summary>
    /// <value>the transliteratedHolidays to set. Ensure that the sequence exactly matches the list returned
    ///         by the default.</value>
    /// <remarks>
    /// Sets the array of <em>Yomim Tovim</em> (holidays) transliterated into Latin chars. This is used by the
    /// <see cref="FormatYomTov"/> when formatting the <em>Yom Tov</em> string. The list uses the following order and uses
    /// the spelling as follows.
    /// <c>["Erev Pesach", "Pesach", "Chol Hamoed Pesach", "Pesach Sheni", "Erev Shavuos", "Shavuos", "Seventeenth of Tammuz",
    /// "Tishah B'Av", "Tu B'Av", "Erev Rosh Hashana", "Rosh Hashana", "Fast of Gedalyah", "Erev Yom Kippur", "Yom Kippur", "Erev
    /// Succos", "Succos", "Chol Hamoed Succos", "Hoshana Rabbah", "Shemini Atzeres", "Simchas Torah", "Erev Chanukah", "Chanukah",
    /// "Tenth of Teves", "Tu B'Shvat", "Fast of Esther", "Purim", "Shushan Purim", "Purim Katan", "Rosh Chodesh", "Yom HaShoah",
    /// "Yom Hazikaron", "Yom Ha'atzmaut", "Yom Yerushalayim", "Lag B'Omer", "Shushan Purim Katan", "Isru Chag"]</c>.
    /// </remarks>
    /// <seealso cref="TransliteratedMonthList"/>
    /// <seealso cref="FormatYomTov"/>
    /// <seealso cref="IsHebrewFormat"/>
    public string[] TransliteratedHolidayList
    {
        get => transliteratedHolidays;
        set => transliteratedHolidays = value;
    }

    /// <summary>
    /// Hebrew <em>Yomim Tovim</em> (holidays) array in the following format.<br/>
    /// <c>&#x200F;["ערב פסח", "פסח", "חול המועד פסח", "פסח שני", "ערב שבועות", "שבועות", "שבעה עשר בתמוז", "תשעה באב", "ט״ו באב", "ערב ראש השנה", "ראש השנה",
    /// "צום גדליה", "ערב יום כיפור", "יום כיפור", "ערב סוכות", "סוכות", "חול המועד סוכות", "הושענא רבה", "שמיני עצרת", "שמחת תורה", "ערב חנוכה", "חנוכה", "עשרה בטבת",
    /// "ט״ו בשבט", "תענית אסתר", "פורים", "שושן פורים", "פורים קטן", "ראש חודש", "יום השואה", "יום הזיכרון", "יום העצמאות", "יום ירושלים", "ל״ג בעומר", "שושן פורים קטן"]</c>
    /// </summary>
    private string[] hebrewHolidays = { "ערב פסח", "פסח",
            "חול המועד פסח",
            "פסח שני", "ערב שבועות",
            "שבועות",
            "שבעה עשר בתמוז",
            "תשעה באב", "ט״ו באב",
            "ערב ראש השנה",
            "ראש השנה", "צום גדליה",
            "ערב יום כיפור",
            "יום כיפור", "ערב סוכות",
            "סוכות",
            "חול המועד סוכות",
            "הושענא רבה",
            "שמיני עצרת",
            "שמחת תורה", "ערב חנוכה",
            "חנוכה", "עשרה בטבת",
            "ט״ו בשבט", "תענית אסתר",
            "פורים", "שושן פורים",
            "פורים קטן", "ראש חודש",
            "יום השואה",
            "יום הזיכרון",
            "יום העצמאות",
            "יום ירושלים",
            "ל״ג בעומר",
            "שושן פורים קטן",
            "אסרו חג"};

    /// <summary>
    /// The transliterated <em>tekufa</em> names.
    /// </summary>
    private string[] transliteratedTekufaNames = new string[] { "Tishrei", "Teves", "Nissan", "Tammuz" };

    /// <summary>
    /// The <em>tekufa</em> names.
    /// </summary>
    private string[] tekufaNames = new string[] { "תשרי", "טבת", "ניסן", "תמוז" };

    /// <summary>
    /// Formats the <em>Yom Tov</em> (holiday) in Hebrew or transliterated Latin characters.
    /// </summary>
    /// <param name="jewishCalendar">the JewishCalendar</param>
    /// <returns>the formatted <em>Yom Tov</em> (holiday) or an empty string if the day is not a <em>Yom Tov</em> (holiday).</returns>
    /// <seealso cref="IsHebrewFormat"/>
    public string FormatYomTov(JewishCalendar jewishCalendar)
    {
        int index = jewishCalendar.GetYomTovIndex();
        if (index == JewishCalendar.Chanukah)
        {
            int dayOfChanukah = jewishCalendar.GetDayOfChanukah();
            return hebrewFormat ? (FormatHebrewNumber(dayOfChanukah) + " " + hebrewHolidays[index])
                    : (transliteratedHolidays[index] + " " + dayOfChanukah);
        }
        return index == -1 ? "" : hebrewFormat ? hebrewHolidays[index] : transliteratedHolidays[index];
    }

    /// <summary>
    /// Formats a day as Rosh Chodesh in the format of in the format of ראש חודש שבט or Rosh Chodesh Shevat. If it is not Rosh Chodesh,
    /// an empty <c>string</c> will be returned.
    /// </summary>
    /// <param name="jewishCalendar">the JewishCalendar</param>
    /// <returns>The formatted <c>string</c> in the format of ראש חודש שבט or Rosh Chodesh Shevat. If it is not Rosh Chodesh, an
    ///         empty <c>string</c> will be returned.</returns>
    public string FormatRoshChodesh(JewishCalendar jewishCalendar)
    {
        if (!jewishCalendar.IsRoshChodesh())
        {
            return "";
        }
        int month = jewishCalendar.JewishMonth;
        if (jewishCalendar.JewishDayOfMonth == 30)
        {
            if (month < JewishCalendar.Adar || (month == JewishCalendar.Adar && jewishCalendar.IsJewishLeapYear()))
            {
                month++;
            }
            else
            { // roll to Nissan
                month = JewishCalendar.Nissan;
            }
        }

        // This method is only about formatting, so we shouldn't make any changes to the params passed in...
        jewishCalendar = jewishCalendar.Clone();
        jewishCalendar.JewishMonth = month;
        string formattedRoshChodesh = hebrewFormat ? hebrewHolidays[JewishCalendar.RoshChodesh]
                : transliteratedHolidays[JewishCalendar.RoshChodesh];
        formattedRoshChodesh += " " + FormatMonth(jewishCalendar);
        return formattedRoshChodesh;
    }

    /// <summary>
    /// Returns if the formatter is set to use Hebrew formatting in the various formatting methods.
    /// </summary>
    /// <value><c>true</c> to format in Hebrew.</value>
    /// <remarks>
    /// Sets the formatter to format in Hebrew in the various formatting methods.
    /// </remarks>
    /// <seealso cref="IsHebrewFormat"/>
    /// <seealso cref="Format"/>
    /// <seealso cref="FormatDayOfWeek"/>
    /// <seealso cref="FormatMonth"/>
    /// <seealso cref="FormatOmer"/>
    /// <seealso cref="FormatYomTov"/>
    public bool IsHebrewFormat
    {
        get => hebrewFormat;
        set => hebrewFormat = value;
    }

    /// <summary>
    /// Returns the Hebrew Omer prefix. By default it is the letter ב producing בעומר, but it can be set to ל to produce לעומר (or any
    /// other prefix) using the <see cref="HebrewOmerPrefix"/>.
    /// </summary>
    /// <value>the hebrewOmerPrefix to set. You can set it to ל to produce to לעומר.</value>
    /// <remarks>
    /// Method to set the Hebrew Omer prefix. By default it is the letter ב producing בעומר, but it can be set to ל to format it לעומר
    /// (or any other prefix).
    /// </remarks>
    /// <seealso cref="HebrewOmerPrefix"/>
    /// <seealso cref="FormatOmer"/>
    public string HebrewOmerPrefix
    {
        get => hebrewOmerPrefix;
        set => hebrewOmerPrefix = value;
    }


    /// <summary>
    /// Returns the Hebrew array of months in the order of<br/><c>&#x200F;["ניסן", "אייר", "סיון", "תמוז", "אב", "אלול", "תשרי", "חשון", "כסלו", "טבת", "שבט", "אדר",
    /// "אדר ב", "אדר א"]</c>. This list has a length of 14 starting with "ניסן" and ending with 3 variations of Adar -
    /// "אדר", "אדר ב", "אדר א".
    /// </summary>
    /// <value>the array of Hebrew months beginning in "ניסן" and ending in "אדר", "אדר ב", "אדר א"</value>
    /// <remarks>
    /// Setter method to allow overriding of the default list of Hebrew month names. This allows changing things such as the default
    /// month name of חשון to מרחשון, etc. This list expects a length of 14 starting with "ניסן" and ending with 3 variations of Adar -
    /// "אדר", "אדר ב", "אדר".
    /// </remarks>
    /// <seealso cref="HebrewMonthList"/>
    /// <seealso cref="HebrewMonthList"/>
    public string[] HebrewMonthList
    {
        get => hebrewMonths;
        set
        {
            if (value.Length != 14)
            {
                throw new ArgumentException("The Hebrew month array must have a length of 14.");
            }
            this.hebrewMonths = value;

        }
    }

    /// <summary>
    /// Returns the array of months transliterated into Latin chars. The default list of months uses Ashkenazi pronunciation in
    /// typical American English spelling. This list has a length of 14 with 3 variations for Adar - "Adar", "Adar II", "Adar I".
    /// The array of months beginn in Nissan and end in "Adar", "Adar II", "Adar I". The default list is
    /// <c>["Nissan", "Iyar", "Sivan", "Tammuz", "Av", "Elul", "Tishrei", "Cheshvan", "Kislev", "Teves", "Shevat", "Adar",
    /// "Adar II", "Adar I"]</c>.
    /// </summary>
    /// <value>the array of 14 month names beginning in Nissan and ending in "Adar", "Adar II", "Adar I".</value>
    /// <remarks>
    /// Setter method to allow overriding of the default list of months transliterated into Latin chars. The default list uses
    /// Ashkenazi American English transliteration. The array of 14 transliterated month names begin in "Nissan" and end in the
    /// 3 Adar variations - "Adar", "Adar II", "Adar I". The default list is
    /// <c>["Nissan", "Iyar", "Sivan", "Tammuz", "Av", "Elul", "Tishrei", "Cheshvan", "Kislev", "Teves", "Shevat", "Adar",
    /// "Adar II", "Adar I"]</c>.
    /// </remarks>
    /// <seealso cref="TransliteratedMonthList"/>
    public string[] TransliteratedMonthList
    {
        get => transliteratedMonths;
        set
        {
            if (value.Length != 14)
            {
                throw new ArgumentException("The transliterated month array must have a length of 14.");
            }
            this.transliteratedMonths = value;

        }
    }

    /// <summary>
    /// List of Hebrew months. The list has* a length of 14 starting with "ניסן" and ending with the 3 variations of Adar -
    /// "אדר", "אדר ב", "אדר א".
    /// </summary>
    /// <seealso cref="HebrewMonthList"/>
    /// <seealso cref="HebrewMonthList"/>
    /// <seealso cref="FormatMonth"/>
    private string[] hebrewMonths = { "ניסן", "אייר",
            "סיון", "תמוז", "אב", "אלול",
            "תשרי", "חשון", "כסלו",
            "טבת", "שבט", "אדר", "אדר ב",
            "אדר א" };

    /// <summary>
    /// Unicode list of Hebrew days of week in the format of <c>&#x200F;["ראשון", "שני", "שלישי", "רביעי", "חמישי", "ששי", "שבת"]</c>
    /// </summary>
    private static readonly string[] hebrewDaysOfWeek = { "ראשון", "שני", "שלישי", "רביעי", "חמישי", "ששי", "שבת" };

    /// <summary>
    /// Formats the day of week. If <see cref="IsHebrewFormat">Hebrew formatting</see> is set, it will display in the format ראשון etc. If
    /// Hebrew formatting is not in use it will return it in the format of Sunday etc. There are various formatting options that will
    /// affect the output.
    /// </summary>
    /// <param name="jewishDate">the JewishDate Object</param>
    /// <returns>the formatted day of week</returns>
    /// <seealso cref="IsHebrewFormat"/>
    /// <seealso cref="IsLongWeekFormat"/>
    public string FormatDayOfWeek(JewishDate jewishDate)
    {
        if (hebrewFormat)
        {
            if (IsLongWeekFormat)
            {
                return hebrewDaysOfWeek[jewishDate.DayOfWeek - 1];
            }
            else
            {
                if (jewishDate.DayOfWeek == 7)
                {
                    return FormatHebrewNumber(300);
                }
                else
                {
                    return FormatHebrewNumber(jewishDate.DayOfWeek);
                }
            }
        }
        else
        {
            if (jewishDate.DayOfWeek == 7)
            {
                if (IsLongWeekFormat)
                {
                    return TransliteratedShabbosDayOfWeek;
                }
                else
                {
                    return TransliteratedShabbosDayOfWeek.Substring(0, 3);
                }
            }
            else
            {
                return jewishDate.LocalDate.ToString(weekFormat, CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Returns whether the class is set to use the Geresh ׳ and Gershayim ״ in formatting Hebrew dates and numbers. When true and
    /// output would look like כ״א שבט תש״כ (or כ״א שבט תש״ך). When set to false, this output would display as כא שבט תשכ.
    /// </summary>
    /// <value>set this to false to omit the Geresh ׳ and Gershayim ״ in formatting</value>
    /// <remarks>
    /// Sets whether to use the Geresh ׳ and Gershayim ״ in formatting Hebrew dates and numbers. The default value is true and output
    /// would look like כ״א שבט תש״כ (or כ״א שבט תש״ך). When set to false, this output would display as כא שבט תשכ (or כא שבט תשך).
    /// Single digit days or month or years such as כ׳ שבט ו׳ אלפים show the use of the Geresh.
    /// </remarks>
    public bool UseGershGershayim
    {
        get => useGershGershayim;
        set => useGershGershayim = value;
    }

    /// <summary>
    /// Returns whether the class is set to use the מנצפ״ך letters when formatting years ending in 20, 40, 50, 80 and 90 to produce
    /// תש״פ if false or תש״ף if true. Traditionally non-final form letters are used, so the year 5780 would be formatted as תש״פ if
    /// the default false is used here. If this returns true, the format תש״ף would be used.
    /// </summary>
    /// <value>Set this to true to use final form letters when formatting Hebrew years.</value>
    /// <remarks>
    /// When formatting a Hebrew Year, traditionally years ending in 20, 40, 50, 80 and 90 are formatted using non-final form letters
    /// for example תש״פ for the year 5780. Setting this to true (the default is false) will use the final form letters for מנצפ״ך and
    /// will format the year 5780 as תש״ף.
    /// </remarks>
    public bool UseFinalFormLetters
    {
        get => useFinalFormLetters;
        set => useFinalFormLetters = value;
    }

    /// <summary>
    /// Returns whether the class is set to use the thousands digit when formatting a Hebrew Year. Traditionally the thousands digit
    /// is omitted and output for a year such as 5729 (1969 Gregorian) would be calculated as 729 and formatted as תשכ״ט. When set to
    /// true the long format year such,  as ה׳ תשכ״ט for 5729/1969 is returned.
    /// </summary>
    /// <value>Set this to true to use the long formatting</value>
    /// <remarks>
    /// When formatting a Hebrew Year, traditionally the thousands digit is omitted and output for a year such as 5729 (1969
    /// Gregorian) would be calculated for 729 and format as תשכ״ט. This method allows setting this to true to return the long format
    /// year such as ה׳ תשכ״ט for 5729/1969.
    /// </remarks>
    public bool UseLongHebrewYears
    {
        get => useLonghebrewYears;
        set => useLonghebrewYears = value;
    }

    /// <summary>
    /// Formats the Jewish date. If the formatter is set to Hebrew, it will format in the form, "day Month year" for example
    /// כ״א שבט תשכ״ט, and the format "21 Shevat, 5729" if not.
    /// </summary>
    /// <param name="jewishDate">the JewishDate to be formatted</param>
    /// <returns>the formatted date. If the formatter is set to Hebrew, it will format in the form, "day Month year" as כ״א שבט תשכ״ט,
    ///         and "21 Shevat, 5729" if not.</returns>
    public string Format(JewishDate jewishDate)
    {
        if (IsHebrewFormat)
        {
            return FormatHebrewNumber(jewishDate.JewishDayOfMonth) + " " + FormatMonth(jewishDate) + " "
                    + FormatHebrewNumber(jewishDate.JewishYear);
        }
        else
        {
            return jewishDate.JewishDayOfMonth + " " + FormatMonth(jewishDate) + ", " + jewishDate.JewishYear;
        }
    }

    /// <summary>
    /// Returns a string of the current Hebrew month formatted as "אדר ב׳" or "Adar II" depending on how <see cref="IsHebrewFormat"/>
    /// is set.
    /// </summary>
    /// <param name="jewishDate">the JewishDate to format</param>
    /// <returns>the formatted month name formatted as "אדר ב׳" or "Adar II" depending on how <see cref="IsHebrewFormat"/> is set.</returns>
    /// <seealso cref="IsHebrewFormat"/>
    /// <seealso cref="IsHebrewFormat"/>
    /// <seealso cref="TransliteratedMonthList"/>
    /// <seealso cref="TransliteratedMonthList"/>
    public string FormatMonth(JewishDate jewishDate)
    {
        int month = jewishDate.JewishMonth;
        if (IsHebrewFormat)
        {
            if (jewishDate.IsJewishLeapYear() && month == JewishDate.Adar)
            {
                return hebrewMonths[JewishDate.AdarII] + (useGershGershayim ? Geresh : ""); // return Adar I, not Adar in a leap year
            }
            else if (jewishDate.IsJewishLeapYear() && month == JewishDate.AdarII)
            {
                return hebrewMonths[JewishDate.Adar] + (useGershGershayim ? Geresh : "");
            }
            else
            {
                return hebrewMonths[month - 1];
            }
        }
        else
        {
            if (jewishDate.IsJewishLeapYear() && month == JewishDate.Adar)
            {
                return transliteratedMonths[JewishDate.AdarII]; // return Adar I, not Adar in a leap year
            }
            else
            {
                return transliteratedMonths[month - 1];
            }
        }
    }

    /// <summary>
    /// Returns a string of the Omer day in the form ל״ג בעומר if Hebrew Format is set, or "Omer X" or "Lag B'Omer" if not. An empty
    /// string if there is no Omer this day.
    /// </summary>
    /// <param name="jewishCalendar">the JewishCalendar to be formatted</param>
    /// <returns>a string of the Omer day in the form or an empty string if there is no Omer this day. The default formatting has a
    /// ב prefix that would output בעומר, but this can be set via the <see cref="HebrewOmerPrefix"/> method to use a ל and
    ///         output ל״ג לעומר.</returns>
    /// <seealso cref="IsHebrewFormat"/>
    /// <seealso cref="HebrewOmerPrefix"/>
    /// <seealso cref="HebrewOmerPrefix"/>
    public string FormatOmer(JewishCalendar jewishCalendar)
    {
        int omer = jewishCalendar.GetDayOfOmer();
        if (omer == -1)
        {
            return "";
        }
        if (hebrewFormat)
        {
            return FormatHebrewNumber(omer) + " " + hebrewOmerPrefix + "עומר";
        }
        else
        {
            if (omer == 33)
            { // if Lag B'Omer
                return transliteratedHolidays[33];
            }
            else
            {
                return "Omer " + omer;
            }
        }
    }

    /// <summary>
    /// Returns the kviah in the traditional 3 letter Hebrew format where the first letter represents the day of week of Rosh Hashana,
    /// the second letter represents the lengths of Cheshvan and Kislev (<see cref="JewishDate.Shelaimim">Shelaimim</see> , <see cref="JewishDate.Kesidran">Kesidran</see> or <see cref="JewishDate.Chaserim">Chaserim</see>) and the 3rd letter represents the day of week of Pesach.
    /// For example 5729 (1969) would return בשה (Rosh Hashana on Monday, Shelaimim, and Pesach on Thursday), while 5771 (2011) would
    /// return השג (Rosh Hashana on Thursday, Shelaimim, and Pesach on Tuesday).
    /// </summary>
    /// <param name="jewishYear">the Jewish year</param>
    /// <returns>the Hebrew string such as בשה for 5729 (1969) and השג for 5771 (2011).</returns>
    public string GetFormattedKviah(int jewishYear)
    {
        JewishDate jewishDate = new JewishDate(jewishYear, JewishDate.Tishrei, 1); // set date to Rosh Hashana
        int kviah = jewishDate.GetCheshvanKislevKviah();
        int roshHashanaDayOfWeek = jewishDate.DayOfWeek;
        string returnValue = FormatHebrewNumber(roshHashanaDayOfWeek);
        returnValue += (kviah == JewishDate.Chaserim ? "ח" : kviah == JewishDate.Shelaimim ? "ש" : "כ");
        jewishDate.SetJewishDate(jewishYear, JewishDate.Nissan, 15); // set to Pesach of the given year
        int pesachDayOfWeek = jewishDate.DayOfWeek;
        returnValue += FormatHebrewNumber(pesachDayOfWeek);
        returnValue = returnValue.Replace(Geresh, "");// geresh is never used in the kviah format
        // boolean isLeapYear = JewishDate.isJewishLeapYear(jewishYear);
        // for efficiency we can avoid the expensive recalculation of the pesach day of week by adding 1 day to Rosh
        // Hashana for a 353-day year, 2 for a 354-day year, 3 for a 355 or 383-day year, 4 for a 384-day year and 5 for
        // a 385-day year
        return returnValue;
    }

    /// <summary>
    /// Formats the <a href="https://en.wikipedia.org/wiki/Daf_Yomi">Daf Yomi</a> Bavli in the format of "עירובין נ״ב" if <see cref="IsHebrewFormat"/> is set to <c>true</c>, or the transliterated format of "Eruvin 52" if set to <c>false</c>.
    /// </summary>
    /// <param name="daf">the Daf to be formatted.</param>
    /// <returns>the formatted daf.</returns>
    public string FormatDafYomiBavli(Daf daf)
    {
        if (hebrewFormat)
        {
            return daf.GetMasechta() + " " + FormatHebrewNumber(daf.DafNumber);
        }
        else
        {
            return daf.GetMasechtaTransliterated() + " " + daf.DafNumber;
        }
    }

    /// <summary>
    /// Formats the <a href="https://en.wikipedia.org/wiki/Jerusalem_Talmud#Daf_Yomi_Yerushalmi">Daf Yomi Yerushalmi</a> in the format
    /// of "עירובין נ״ב" in <see cref="IsHebrewFormat"/> is set to <c>true</c>, or the transliterated format of "Eruvin 52" if set to
    /// <c>false</c>.
    /// </summary>
    /// <param name="daf">the Daf to be formatted.</param>
    /// <returns>the formatted daf.</returns>
    public string FormatDafYomiYerushalmi(Daf? daf)
    {
        if (daf == null)
        {
            if (hebrewFormat)
            {
                return Daf.GetYerushalmiMasechtos()[39];
            }
            else
            {
                return Daf.GetYerushalmiMasechtosTransliterated()[39];
            }
        }
        if (hebrewFormat)
        {
            return daf.GetYerushalmiMasechta() + " " + FormatHebrewNumber(daf.DafNumber);
        }
        else
        {
            return daf.GetYerushalmiMasechtaTransliterated() + " " + daf.DafNumber;
        }
    }

    /// <summary>
    /// Returns a Hebrew formatted string of a number. The method can calculate from 0 to 9999.
    /// <list type="bullet">
    /// <item><description>Single digit numbers such as 3, 30 and 100 will be returned with a ׳ (<a
    /// href="http://en.wikipedia.org/wiki/Geresh">Geresh</a>) appended as at the end. For example ג׳, ל׳ and ק׳</description></item>
    /// <item><description>multi digit numbers such as 21 and 769 will be returned with a ״ (<a
    /// href="http://en.wikipedia.org/wiki/Gershayim">Gershayim</a>) between the second to last and last letters. For
    /// example כ״א, תשכ״ט</description></item>
    /// <item><description>15 and 16 will be returned as ט״ו and ט״ז</description></item>
    /// <item><description>Single digit numbers (years assumed) such as 6000 (%1000=0) will be returned as ו׳אלפים</description></item>
    /// <item><description>0 will return אפס</description></item>
    /// </list>
    /// </summary>
    /// <param name="number">the number to be formatted. An <see cref="ArgumentException"/> is thrown if the number is &lt; 0 or &gt; 9999.</param>
    /// <returns>the Hebrew formatted number such as תשכ״ט</returns>
    /// <seealso cref="UseFinalFormLetters"/>
    /// <seealso cref="UseGershGershayim"/>
    /// <seealso cref="IsHebrewFormat"/>
    public string FormatHebrewNumber(int number)
    {
        if (number < 0)
        {
            throw new ArgumentException("negative numbers can't be formatted");
        }
        else if (number > 9999)
        {
            throw new ArgumentException("numbers > 9999 can't be formatted");
        }

        string Alafim = "אלפים";
        string Efes = "אפס";

        string[] jHundreds = new string[] { "", "ק", "ר", "ש", "ת", "תק", "תר", "תש", "תת", "תתק" };
        string[] jTens = new string[] { "", "י", "כ", "ל", "מ", "נ", "ס", "ע", "פ", "צ" };
        string[] jTenEnds = new string[] { "", "י", "ך", "ל", "ם", "ן", "ס", "ע", "ף", "ץ" };
        string[] tavTaz = new string[] { "טו", "טז" };
        string[] jOnes = new string[] { "", "א", "ב", "ג", "ד", "ה", "ו", "ז", "ח", "ט" };

        if (number == 0)
        { // do we really need this? Should it be applicable to a date?
            return Efes;
        }
        int shortNumber = number % 1000; // discard thousands
        // next check for all possible single Hebrew digit years
        bool singleDigitNumber = (shortNumber < 11 || (shortNumber < 100 && shortNumber % 10 == 0) ||
                (shortNumber <= 400 && shortNumber % 100 == 0));
        int thousands = number / 1000; // get # thousands
        StringBuilder sb = new StringBuilder();
        // append thousands to String
        if (number % 1000 == 0)
        { // in year is 5000, 4000 etc
            sb.Append(jOnes[thousands]);
            if (UseGershGershayim)
            {
                sb.Append(Geresh);
            }
            sb.Append(" ");
            sb.Append(Alafim); // add # of thousands plus the word "thousand" (override alafim boolean)
            return sb.ToString();
        }
        else if (useLonghebrewYears && number >= 1000)
        { // if alafim boolean display thousands
            sb.Append(jOnes[thousands]);
            if (UseGershGershayim)
            {
                sb.Append(Geresh); // append thousands quote
            }
            sb.Append(" ");
        }
        number = number % 1000; // remove 1000s
        int hundreds = number / 100; // # of hundreds
        sb.Append(jHundreds[hundreds]); // add hundreds to String
        number = number % 100; // remove 100s
        if (number == 15)
        { // special case 15
            sb.Append(tavTaz[0]);
        }
        else if (number == 16)
        { // special case 16
            sb.Append(tavTaz[1]);
        }
        else
        {
            int tens = number / 10;
            if (number % 10 == 0)
            { // if evenly divisible by 10
                if (!singleDigitNumber)
                {
                    if (UseFinalFormLetters)
                    {
                        sb.Append(jTenEnds[tens]); // years like 5780 will end with a final form ף
                    }
                    else
                    {
                        sb.Append(jTens[tens]); // years like 5780 will end with a regular פ
                    }
                }
                else
                {
                    sb.Append(jTens[tens]); // standard letters so years like 5050 will end with a regular nun
                }
            }
            else
            {
                sb.Append(jTens[tens]);
                number = number % 10;
                sb.Append(jOnes[number]);
            }
        }
        if (UseGershGershayim)
        {
            if (singleDigitNumber)
            {
                sb.Append(Geresh); // append single quote
            }
            else
            { // append double quote before last digit
                sb.Insert(sb.Length - 1, Gershayim);
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// The map of transliterated parshiyos used by this formatter. This list using the default <em>Ashkenazi</em>
    /// pronunciation. This list can be overridden (for <em>Sephardi</em> English transliteration for example) by setting this
    /// property. The list includes double and special <em>parshiyos</em> in the following
    /// order and spelling "<em>Bereshis, Noach, Lech Lecha, Vayera, Chayei Sara, Toldos, Vayetzei, Vayishlach, Vayeshev, Miketz,
    /// Vayigash, Vayechi, Shemos, Vaera, Bo, Beshalach, Yisro, Mishpatim, Terumah, Tetzaveh, Ki Sisa, Vayakhel, Pekudei, Vayikra,
    /// Tzav, Shmini, Tazria, Metzora, Achrei Mos, Kedoshim, Emor, Behar, Bechukosai, Bamidbar, Nasso, Beha'aloscha, Sh'lach,
    /// Korach, Chukas, Balak, Pinchas, Matos, Masei, Devarim, Vaeschanan, Eikev, Re'eh, Shoftim, Ki Seitzei, Ki Savo, Nitzavim,
    /// Vayeilech, Ha'Azinu, Vezos Habracha, Vayakhel Pekudei, Tazria Metzora, Achrei Mos Kedoshim, Behar Bechukosai, Chukas Balak,
    /// Matos Masei, Nitzavim Vayeilech, Shekalim, Zachor, Parah, Hachodesh,Shuva, Shira, Hagadol, Chazon, Nachamu</em>".
    /// </summary>
    /// <seealso cref="FormatParshah(JewishCalendar)"/>
    public Dictionary<JewishCalendar.Parshah, string> TransliteratedParshiyosList
    {
        get => transliteratedParshahMap;
        set => transliteratedParshahMap = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>The KosherJava getter for <see cref="TransliteratedParshiyosList"/>.</summary>
    /// <returns>the map of transliterated Parshiyos</returns>
    [Obsolete("Use the TransliteratedParshiyosList property; GetTransliteratedParshiyosList() is the KosherJava name. This alias will be removed in a future version.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public Dictionary<JewishCalendar.Parshah, string> GetTransliteratedParshiyosList() => TransliteratedParshiyosList;

    /// <summary>The KosherJava setter for <see cref="TransliteratedParshiyosList"/>.</summary>
    /// <param name="transliteratedParshahMap">the transliterated Parshiyos to set</param>
    [Obsolete("Use the TransliteratedParshiyosList property; SetTransliteratedParshiyosList is the KosherJava name. This alias will be removed in a future version.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void SetTransliteratedParshiyosList(Dictionary<JewishCalendar.Parshah, string> transliteratedParshahMap) =>
        TransliteratedParshiyosList = transliteratedParshahMap;

    /// <summary>
    /// Returns a string with the name of the current parshah(iyos). This method gets the current <em>parshah</em> by calling <see cref="JewishCalendar.GetParshah"/> that does not return a <em>parshah</em> for any non-<em>Shabbos</em> or a <em>Shabbos</em> that
    /// occurs on a <em>Yom Tov</em>, and will return an empty <c>string</c> in those cases. If the class <see cref="IsHebrewFormat">is set to format in Hebrew</see> it will return a <c>string</c> of the current parshah(iyos) in Hebrew for
    /// example בראשית or נצבים וילך for a double parshah, or an empty string will be returned if there is not parshah that week. If not set
    /// to Hebrew, it returns a string of the parshah(iyos) transliterated into Latin chars. The default uses Ashkenazi pronunciation
    /// in typical American English spelling, for example Bereshis, Nitzavim Vayeilech for a double parshah, An empty string if there
    /// are none.
    /// </summary>
    /// <param name="jewishCalendar">the JewishCalendar Object</param>
    /// <returns>today's parshah(iyos) in Hebrew for example, if the formatter is set to format in Hebrew, returns a string of the current
    ///         parshah(iyos) in Hebrew for example בראשית or נצבים וילך, for a double parshah or an empty <c>string</c> if there is
    ///         no parshah that week. If not set to Hebrew, it returns a string of the parshah(iyos) transliterated into Latin chars. The
    ///         default uses Ashkenazi pronunciation in typical American English spelling, for example Bereshis, Nitzavim Vayeilech for
    ///         a double parshah, or an empty <c>string</c> if there are none.</returns>
    /// <seealso cref="FormatParshah(JewishCalendar)"/>
    /// <seealso cref="IsHebrewFormat"/>
    /// <seealso cref="JewishCalendar.GetParshah"/>
    public string FormatParshah(JewishCalendar jewishCalendar)
    {
        JewishCalendar.Parshah parshah = jewishCalendar.GetParshah();
        return FormatParshah(parshah);
    }

    /// <summary>
    /// Returns a <c>string</c> with the name of the current parshah(iyos). This method overloads <see cref="FormatParshah(JewishCalendar)"/> and unlike that method, it will format the <em>parshah</em> passed to this method regardless of
    /// the day of week. This is the way to format a <em>parshah</em> retrieved from calling
    /// <see cref="JewishCalendar.GetUpcomingParshah"/>.
    /// </summary>
    /// <param name="parshah">a JewishCalendar.Parshah object</param>
    /// <returns>today's parshah(iyos) in Hebrew for example, if the formatter is set to format in Hebrew, returns a <c>string</c>
    ///         of the current parshah(iyos) in Hebrew for example בראשית or נצבים וילך for a double parshah, or an empty <c>string</c>
    ///         if there is no parshah that week. If not set to Hebrew, it returns a string of the parshah(iyos) transliterated into
    ///         Latin chars. The default uses Ashkenazi pronunciation in typical American English spelling, for example Bereshis,  or
    ///         Nitzavim Vayeilech for a double parshah, or an empty string if there are none.</returns>
    /// <seealso cref="FormatParshah(JewishCalendar)"/>
    /// <seealso cref="JewishCalendar.GetUpcomingParshah"/>
    public string FormatParshah(JewishCalendar.Parshah parshah)
    {
        return hebrewFormat ? hebrewParshahMap[parshah] : transliteratedParshahMap[parshah];
    }

    /// <summary>
    /// Returns a string with the name of the current special parshah of Shekalim, Zachor, Parah or Hachodesh or an empty string for a
    /// non-special parshah. If the formatter is set to format in Hebrew, it returns a string of the current special parshah in Hebrew,
    /// for example שקלים, זכור, פרה or החדש, or an empty <c>string</c> if the date is not a special parshah. If not set to Hebrew,
    /// it returns a string of the special parshah transliterated into Latin chars. The default uses Ashkenazi pronunciation in typical
    /// American English spelling Shekalim, Zachor, Parah or Hachodesh.
    /// </summary>
    /// <param name="jewishCalendar">the JewishCalendar Object</param>
    /// <returns>today's special parshah. If the formatter is set to format in Hebrew, returns a string of the current special parshah in
    ///         Hebrew for in the format of שקלים, זכור, פרה or החדש or an empty string if there are none. If not set to Hebrew, it
    ///         returns a string of the special parshah transliterated into Latin chars. The default uses Ashkenazi pronunciation in
    ///         typical American English spelling of Shekalim, Zachor, Parah or Hachodesh. An empty string if there are none.</returns>
    public string FormatSpecialParshah(JewishCalendar jewishCalendar)
    {
        JewishCalendar.Parshah specialParshah = jewishCalendar.GetSpecialShabbos();
        return hebrewFormat ? hebrewParshahMap[specialParshah] : transliteratedParshahMap[specialParshah];
    }

    /// <summary>
    /// Returns a the formatted <em>tekufa</em> name if it is the day of the <em>tekufa</em> event, or an empty <see cref="string"/> if it
    /// is not.
    /// </summary>
    /// <param name="jewishCalendar">the <c>JewishCalendar</c> to format the <em>tekufa</em> name for.</param>
    /// <returns>a <see cref="string"/> with the name of the upcoming tekufa/season, in the format of "תקופת תשרי" if <see cref="IsHebrewFormat"/>
    ///         is set to <c>true</c>, or "Tekufas Tishrei" if set to <c>false</c> or an empty string on a day without a
    ///         <em>tekufa</em> event.</returns>
    public string FormatTekufaName(JewishCalendar jewishCalendar)
    {
        double InitialTekufaOffset = 12.625;  // the number of days Tekufas Tishrei occurs before JEWISH_EPOCH
        double days = JewishDate.GetJewishCalendarElapsedDays(jewishCalendar.JewishYear) + jewishCalendar.GetDaysSinceStartOfJewishYear() + InitialTekufaOffset - 1;  // total days since first Tekufas Tishrei event

        double solarDaysElapsed = days % 365.25;  // total days elapsed since start of solar year
        int currentTekufaNumber = (int)(solarDaysElapsed / 91.3125);  // the current quarter of the solar year
        double tekufaDaysElapsed = solarDaysElapsed % 91.3125;  // the number of days that have passed since a tekufa event
        if (tekufaDaysElapsed > 0 && tekufaDaysElapsed <= 1)
        {  // if the tekufa happens in the upcoming 24 hours
            return IsHebrewFormat ? "תקופת " + tekufaNames[currentTekufaNumber] : "Tekufas " + transliteratedTekufaNames[currentTekufaNumber];//0 for Tishrei, 1 for Tevet, 2, for Nissan, 3 for Tammuz
        }
        else
        {
            return "";
        }
    }
}
