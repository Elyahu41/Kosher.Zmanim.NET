/*
 * Zmanim Java API
 * Copyright © 2011 - 2026 Eliyahu Hershfeld
 * Copyright © September 2002 Avrom Finkelstein
 * Copyright © 2019 - 2022 Y Paritcher
 * Copyright © 2026 Moshe Dicker
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

namespace Kosher.Zmanim.HebrewCalendar;


/// <summary>
/// The JewishCalendar extends the JewishDate class and adds calendar methods.
///
/// This open source code (originally Java) was originally ported by <a href="https://www.linkedin.com/in/avromf/">Avrom Finkelstein</a>
/// from his C++ code. It was refactored to fit the KosherJava Zmanim API with simplification of the code, enhancements
/// and some bug fixing. The class allows setting whether the holiday and <em>parshah</em> scheme follows the Israel scheme
/// or outside Israel scheme. The default is the outside Israel scheme.
/// The parshah code was ported by Y. Paritcher from his <a href="https://github.com/yparitcher/libzmanim">libzmanim</a> code.
/// </summary>
/// <remarks>
/// <b>To do:</b> Some do not belong in this class, but here is a partial list of what should still be implemented in some form:
/// <list type="number">
/// <item><description>Mishna yomis etc</description></item>
/// </list>
/// </remarks>
public class JewishCalendar : JewishDate
{
    /// <summary>
    /// The 14th day of Nissan, the day before Pesach (Passover).
    /// </summary>
    public const int ErevPesach = 0;
    /// <summary>
    /// The holiday of Pesach (Passover) on the 15th (and 16th out of Israel) day of Nissan.
    /// </summary>
    public const int Pesach = 1;
    /// <summary>
    /// Chol Hamoed (interim days) of Pesach (Passover)
    /// </summary>
    public const int CholHamoedPesach = 2;
    /// <summary>
    /// Pesach Sheni, the 14th day of Iyar, a minor holiday.
    /// </summary>
    public const int PesachSheni = 3;
    /// <summary>
    /// Erev Shavuos (the day before Shavuos), the 5th of Sivan
    /// </summary>
    public const int ErevShavuos = 4;
    /// <summary>
    /// Shavuos (Pentecost), the 6th of Sivan
    /// </summary>
    public const int Shavuos = 5;
    /// <summary>
    /// The fast of the 17th day of Tammuz
    /// </summary>
    public const int SeventeenOfTammuz = 6;
    /// <summary>
    /// The fast of the 9th of Av
    /// </summary>
    public const int TishaBeav = 7;
    /// <summary>
    /// The 15th day of Av, a minor holiday
    /// </summary>
    public const int TuBeav = 8;
    /// <summary>
    /// Erev Rosh Hashana (the day before Rosh Hashana), the 29th of Elul
    /// </summary>
    public const int ErevRoshHashana = 9;
    /// <summary>
    /// Rosh Hashana, the first and second days of Tishrei.
    /// </summary>
    public const int RoshHashana = 10;
    /// <summary>
    /// The fast of Gedalyah, the 3rd of Tishrei.
    /// </summary>
    public const int FastOfGedalyah = 11;
    /// <summary>
    /// The 9th day of Tishrei, the day before of Yom Kippur.
    /// </summary>
    public const int ErevYomKippur = 12;
    /// <summary>
    /// The holiday of Yom Kippur, the 10th day of Tishrei
    /// </summary>
    public const int YomKippur = 13;
    /// <summary>
    /// The 14th day of Tishrei, the day before of Succos/Sukkos (Tabernacles).
    /// </summary>
    public const int ErevSuccos = 14;
    /// <summary>
    /// The holiday of Succos/Sukkos (Tabernacles), the 15th (and 16th out of Israel) day of Tishrei
    /// </summary>
    public const int Succos = 15;
    /// <summary>
    /// Chol Hamoed (interim days) of Succos/Sukkos (Tabernacles)
    /// </summary>
    public const int CholHamoedSuccos = 16;
    /// <summary>
    /// Hoshana Rabba, the 7th day of Succos/Sukkos that occurs on the 21st of Tishrei.
    /// </summary>
    public const int HoshanaRabba = 17;
    /// <summary>
    /// Shmini Atzeres, the 8th day of Succos/Sukkos is an independent holiday that occurs on the 22nd of Tishrei.
    /// </summary>
    public const int SheminiAtzeres = 18;
    /// <summary>
    /// Simchas Torah, the 9th day of Succos/Sukkos, or the second day of Shmini Atzeres that is celebrated
    /// <see cref="InIsrael">out of Israel</see> on the 23rd of Tishrei.
    /// </summary>
    public const int SimchasTorah = 19;
    // public static final int EREV_CHANUKAH = 20;// probably remove this
    /// <summary>
    /// The holiday of Chanukah. 8 days starting on the 25th day Kislev.
    /// </summary>
    public const int Chanukah = 21;
    /// <summary>
    /// The fast of the 10th day of Teves.
    /// </summary>
    public const int TenthOfTeves = 22;
    /// <summary>
    /// Tu Bishvat on the 15th day of Shevat, a minor holiday.
    /// </summary>
    public const int TuBeshvat = 23;
    /// <summary>
    /// The fast of Esther, usually on the 13th day of Adar (or Adar II on leap years). It is earlier on some years.
    /// </summary>
    public const int FastOfEsther = 24;
    /// <summary>
    /// The holiday of Purim on the 14th day of Adar (or Adar II on leap years).
    /// </summary>
    public const int Purim = 25;
    /// <summary>
    /// The holiday of Shushan Purim on the 15th day of Adar (or Adar II on leap years).
    /// </summary>
    public const int ShushanPurim = 26;
    /// <summary>
    /// The holiday of Purim Katan on the 14th day of Adar I on a leap year when Purim is on Adar II, a minor holiday.
    /// </summary>
    public const int PurimKatan = 27;
    /// <summary>
    /// Rosh Chodesh, the new moon on the first day of the Jewish month, and the 30th day of the previous month in the
    /// case of a month with 30 days.
    /// </summary>
    public const int RoshChodesh = 28;
    /// <summary>
    /// Yom HaShoah, Holocaust Remembrance Day, usually held on the 27th of Nissan. If it falls on a Friday, it is moved
    /// to the 26th, and if it falls on a Sunday it is moved to the 28th. A <see cref="UseModernHolidays">modern holiday</see>.
    /// </summary>
    public const int YomHashoah = 29;
    /// <summary>
    /// Yom HaZikaron, Israeli Memorial Day, held a day before Yom Ha'atzmaut.  A <see cref="UseModernHolidays">modern holiday</see>.
    /// </summary>
    public const int YomHazikaron = 30;

    /// <summary>
    /// Yom Ha'atzmaut, Israel Independence Day, the 5th of Iyar, but if it occurs on a Friday or Saturday, the holiday is
    /// moved back to Thursday, the 3rd of 4th of Iyar, and if it falls on a Monday, it is moved forward to Tuesday the
    /// 6th of Iyar.  A <see cref="UseModernHolidays">modern holiday</see>.
    /// </summary>
    public const int YomHaatzmaut = 31;
    /// <summary>
    /// Yom Yerushalayim or Jerusalem Day, on 28 Iyar. A <see cref="UseModernHolidays">modern holiday</see>.
    /// </summary>
    public const int YomYerushalayim = 32;

    /// <summary>
    /// The 33rd day of the Omer, the 18th of Iyar, a minor holiday.
    /// </summary>
    public const int LagBaomer = 33;

    /// <summary>
    /// The holiday of Purim Katan on the 15th day of Adar I on a leap year when Purim is on Adar II, a minor holiday.
    /// </summary>
    public const int ShushanPurimKatan = 34;

    /// <summary>
    /// The day following the last day of Pesach, Shavuos and Sukkos.
    /// </summary>
    public const int IsruChag = 35;

    /// <summary>
    /// The day before <em>Rosh Chodesh</em> (moved to Thursday if <em>Rosh Chodesh</em> is on a Friday or <em>Shabbos</em>) in most months.
    /// This constant is not actively in use.
    /// </summary>
    /// <seealso cref="IsYomKippurKatan"/>
    public const int YomKippurKatan = 36;

    /// <summary>
    /// The Monday, Thursday and Monday after the first <em>Shabbos</em> after <em>Rosh Chodesh Cheshvan</em> and <em>Iyar</em>) are BeHab
    /// days. This constant is not actively in use.
    /// </summary>
    /// <seealso cref="IsBeHaB"/>
    public const int Behab = 37;

    /// <summary>
    /// Is the calendar set to Israel, where some holidays have different rules.
    /// </summary>
    /// <seealso cref="InIsrael"/>
    /// <seealso cref="InIsrael"/>
    private bool inIsrael = false;

    /// <summary>
    /// Is the calendar set to have Purim <em>demukafim</em>, where Purim is celebrated on Shushan Purim.
    /// </summary>
    /// <seealso cref="IsMukafChoma"/>
    /// <seealso cref="IsMukafChoma"/>
    private bool isMukafChoma = false;

    /// <summary>
    /// Is the calendar set to use modern Israeli holidays such as Yom Haatzmaut.
    /// </summary>
    /// <seealso cref="UseModernHolidays"/>
    /// <seealso cref="UseModernHolidays"/>
    private bool useModernHolidays = false;

    /// <summary>
    /// List of <em>parshiyos</em> or special <em>Shabasos</em>. <see cref="None"/> indicates a week without a <em>parshah</em>, while the enum for
    /// the <em>parshah</em> of <see cref="VzosHaberacha"/> exists for consistency, but is not currently used. The special <em>Shabasos</em> of
    /// Shekalim, Zachor, Para, Hachodesh, as well as Shabbos Shuva, Shira, Hagadol, Chazon and Nachamu are also represented in this collection
    /// of <em>parshiyos</em>.
    /// </summary>
    /// <seealso cref="GetSpecialShabbos"/>
    /// <seealso cref="GetParshah"/>
    public enum Parshah
    {
        /// <summary>
        /// NONE A week without any <em>parshah</em> such as <em>Shabbos Chol Hamoed</em>
        /// </summary>
        None,
        /// <summary>
        /// BERESHIS
        /// </summary>
        Bereshis,
        /// <summary>
        /// NOACH
        /// </summary>
        Noach,
        /// <summary>
        /// LECH_LECHA
        /// </summary>
        LechLecha,
        /// <summary>
        /// VAYERA
        /// </summary>
        Vayera,
        /// <summary>
        /// CHAYEI_SARA
        /// </summary>
        ChayeiSara,
        /// <summary>
        /// TOLDOS
        /// </summary>
        Toldos,
        /// <summary>
        /// VAYETZEI
        /// </summary>
        Vayetzei,
        /// <summary>
        /// VAYISHLACH
        /// </summary>
        Vayishlach,
        /// <summary>
        /// VAYESHEV
        /// </summary>
        Vayeshev,
        /// <summary>
        /// MIKETZ
        /// </summary>
        Miketz,
        /// <summary>
        /// VAYIGASH
        /// </summary>
        Vayigash,
        /// <summary>
        /// VAYECHI
        /// </summary>
        Vayechi,
        /// <summary>
        /// SHEMOS
        /// </summary>
        Shemos,
        /// <summary>
        /// VAERA
        /// </summary>
        Vaera,
        /// <summary>
        /// BO
        /// </summary>
        Bo,
        /// <summary>
        /// BESHALACH
        /// </summary>
        Beshalach,
        /// <summary>
        /// YISRO
        /// </summary>
        Yisro,
        /// <summary>
        /// MISHPATIM
        /// </summary>
        Mishpatim,
        /// <summary>
        /// TERUMAH
        /// </summary>
        Terumah,
        /// <summary>
        /// TETZAVEH
        /// </summary>
        Tetzaveh,
        /// <summary>
        /// KI_SISA
        /// </summary>
        KiSisa,
        /// <summary>
        /// VAYAKHEL
        /// </summary>
        Vayakhel,
        /// <summary>
        /// PEKUDEI
        /// </summary>
        Pekudei,
        /// <summary>
        /// VAYIKRA
        /// </summary>
        Vayikra,
        /// <summary>
        /// TZAV
        /// </summary>
        Tzav,
        /// <summary>
        /// SHMINI
        /// </summary>
        Shmini,
        /// <summary>
        /// TAZRIA
        /// </summary>
        Tazria,
        /// <summary>
        /// METZORA
        /// </summary>
        Metzora,
        /// <summary>
        /// ACHREI_MOS
        /// </summary>
        AchreiMos,
        /// <summary>
        /// KEDOSHIM
        /// </summary>
        Kedoshim,
        /// <summary>
        /// EMOR
        /// </summary>
        Emor,
        /// <summary>
        /// BEHAR
        /// </summary>
        Behar,
        /// <summary>
        /// BECHUKOSAI
        /// </summary>
        Bechukosai,
        /// <summary>
        /// BAMIDBAR
        /// </summary>
        Bamidbar,
        /// <summary>
        /// NASSO
        /// </summary>
        Nasso,
        /// <summary>
        /// BEHAALOSCHA
        /// </summary>
        Behaaloscha,
        /// <summary>
        /// SHLACH
        /// </summary>
        Shlach,
        /// <summary>
        /// KORACH
        /// </summary>
        Korach,
        /// <summary>
        /// CHUKAS
        /// </summary>
        Chukas,
        /// <summary>
        /// BALAK
        /// </summary>
        Balak,
        /// <summary>
        /// PINCHAS
        /// </summary>
        Pinchas,
        /// <summary>
        /// MATOS
        /// </summary>
        Matos,
        /// <summary>
        /// MASEI
        /// </summary>
        Masei,
        /// <summary>
        /// DEVARIM
        /// </summary>
        Devarim,
        /// <summary>
        /// VAESCHANAN
        /// </summary>
        Vaeschanan,
        /// <summary>
        /// EIKEV
        /// </summary>
        Eikev,
        /// <summary>
        /// REEH
        /// </summary>
        Reeh,
        /// <summary>
        /// SHOFTIM
        /// </summary>
        Shoftim,
        /// <summary>
        /// KI_SEITZEI
        /// </summary>
        KiSeitzei,
        /// <summary>
        /// KI_SAVO
        /// </summary>
        KiSavo,
        /// <summary>
        /// NITZAVIM
        /// </summary>
        Nitzavim,
        /// <summary>
        /// VAYEILECH
        /// </summary>
        Vayeilech,
        /// <summary>
        /// HAAZINU
        /// </summary>
        Haazinu,
        /// <summary>
        /// VZOS_HABERACHA
        /// </summary>
        VzosHaberacha,
        /// <summary>
        /// The double parshah of Vayakhel &amp; Pekudei
        /// </summary>
        VayakhelPekudei,
        /// <summary>
        /// The double <em>parshah</em> of Tazria
        /// &amp; Metzora
        /// </summary>
        TazriaMetzora,
        /// <summary>
        /// The double <em>parshah</em> of Achrei Mos &amp; Kedoshim
        /// </summary>
        AchreiMosKedoshim,
        /// <summary>
        /// The double <em>parshah</em>
        /// of Behar &amp; Bechukosai
        /// </summary>
        BeharBechukosai,
        /// <summary>
        /// The double <em>parshah</em> of Chukas &amp; Balak
        /// </summary>
        ChukasBalak,
        /// <summary>
        /// The double
        /// <em>parshah</em> of Matos &amp; Masei
        /// </summary>
        MatosMasei,
        /// <summary>
        /// The double <em>parshah</em> of Nitzavim &amp; Vayelech
        /// </summary>
        NitzavimVayeilech,
        /// <summary>
        /// The special <em>parshah</em> of Shekalim
        /// </summary>
        Shkalim,
        /// <summary>
        /// The special <em>parshah</em> of Zachor
        /// </summary>
        Zachor,
        /// <summary>
        /// The special <em>parshah</em> of
        /// Para
        /// </summary>
        Para,
        /// <summary>
        /// The special <em>parshah</em> of Hachodesh
        /// </summary>
        Hachodesh,
        /// <summary>
        /// <em>Shabbos</em> Shuva
        /// </summary>
        Shuva,
        /// <summary>
        /// <em>Shabbos</em> Shira
        /// </summary>
        Shira,
        /// <summary>
        /// <em>Shabbos</em> Hagadol
        /// </summary>
        Hagadol,
        /// <summary>
        /// <em>Shabbos</em> Chazon
        /// </summary>
        Chazon,
        /// <summary>
        /// <em>Shabbos</em> Nachamu
        /// </summary>
        Nachamu
    };

    /// <summary>
    /// An array of <em>parshiyos</em> in the 17 possible combinations.
    /// </summary>
    public static readonly Parshah[][] parshahList = {
        new Parshah[] {Parshah.None, Parshah.Vayeilech, Parshah.Haazinu, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.VayakhelPekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.None, Parshah.Shmini, Parshah.TazriaMetzora, Parshah.AchreiMosKedoshim, Parshah.Emor, Parshah.BeharBechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.NitzavimVayeilech},
        new Parshah[] {Parshah.None, Parshah.Vayeilech, Parshah.Haazinu, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.VayakhelPekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.None, Parshah.Shmini, Parshah.TazriaMetzora, Parshah.AchreiMosKedoshim, Parshah.Emor, Parshah.BeharBechukosai, Parshah.Bamidbar, Parshah.None, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.ChukasBalak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.NitzavimVayeilech},
        new Parshah[] {Parshah.None, Parshah.Haazinu, Parshah.None, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.VayakhelPekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.None, Parshah.None, Parshah.Shmini, Parshah.TazriaMetzora, Parshah.AchreiMosKedoshim, Parshah.Emor, Parshah.BeharBechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.Nitzavim},
        new Parshah[] {Parshah.None, Parshah.Haazinu, Parshah.None, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.Vayakhel, Parshah.Pekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.None, Parshah.Shmini, Parshah.TazriaMetzora, Parshah.AchreiMosKedoshim, Parshah.Emor, Parshah.BeharBechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.Nitzavim},
        new Parshah[] {Parshah.None, Parshah.None, Parshah.Haazinu, Parshah.None, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.VayakhelPekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.None, Parshah.Shmini, Parshah.TazriaMetzora, Parshah.AchreiMosKedoshim, Parshah.Emor, Parshah.BeharBechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.Nitzavim},
        new Parshah[] {Parshah.None, Parshah.None, Parshah.Haazinu, Parshah.None, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.VayakhelPekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.None, Parshah.Shmini, Parshah.TazriaMetzora, Parshah.AchreiMosKedoshim, Parshah.Emor, Parshah.BeharBechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.NitzavimVayeilech},
        new Parshah[] {Parshah.None, Parshah.Vayeilech, Parshah.Haazinu, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.Vayakhel, Parshah.Pekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.Shmini, Parshah.Tazria, Parshah.Metzora, Parshah.None, Parshah.AchreiMos, Parshah.Kedoshim, Parshah.Emor, Parshah.Behar, Parshah.Bechukosai, Parshah.Bamidbar, Parshah.None, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.ChukasBalak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.NitzavimVayeilech},
        new Parshah[] {Parshah.None, Parshah.Vayeilech, Parshah.Haazinu, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.Vayakhel, Parshah.Pekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.Shmini, Parshah.Tazria, Parshah.Metzora, Parshah.None, Parshah.None, Parshah.AchreiMos, Parshah.Kedoshim, Parshah.Emor, Parshah.Behar, Parshah.Bechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.Nitzavim},
        new Parshah[] {Parshah.None, Parshah.Haazinu, Parshah.None, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.Vayakhel, Parshah.Pekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.Shmini, Parshah.Tazria, Parshah.Metzora, Parshah.AchreiMos, Parshah.None, Parshah.Kedoshim, Parshah.Emor, Parshah.Behar, Parshah.Bechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.Matos, Parshah.Masei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.Nitzavim},
        new Parshah[] {Parshah.None, Parshah.Haazinu, Parshah.None, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.Vayakhel, Parshah.Pekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.Shmini, Parshah.Tazria, Parshah.Metzora, Parshah.AchreiMos, Parshah.None, Parshah.Kedoshim, Parshah.Emor, Parshah.Behar, Parshah.Bechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.Matos, Parshah.Masei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.NitzavimVayeilech},
        new Parshah[] {Parshah.None, Parshah.None, Parshah.Haazinu, Parshah.None, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.Vayakhel, Parshah.Pekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.Shmini, Parshah.Tazria, Parshah.Metzora, Parshah.None, Parshah.AchreiMos, Parshah.Kedoshim, Parshah.Emor, Parshah.Behar, Parshah.Bechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.NitzavimVayeilech},
        new Parshah[] {Parshah.None, Parshah.None, Parshah.Haazinu, Parshah.None, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.Vayakhel, Parshah.Pekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.Shmini, Parshah.Tazria, Parshah.Metzora, Parshah.None, Parshah.AchreiMos, Parshah.Kedoshim, Parshah.Emor, Parshah.Behar, Parshah.Bechukosai, Parshah.Bamidbar, Parshah.None, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.ChukasBalak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.NitzavimVayeilech},
        new Parshah[] {Parshah.None, Parshah.Vayeilech, Parshah.Haazinu, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.VayakhelPekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.None, Parshah.Shmini, Parshah.TazriaMetzora, Parshah.AchreiMosKedoshim, Parshah.Emor, Parshah.BeharBechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.NitzavimVayeilech},
        new Parshah[] {Parshah.None, Parshah.Haazinu, Parshah.None, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.VayakhelPekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.None, Parshah.Shmini, Parshah.TazriaMetzora, Parshah.AchreiMosKedoshim, Parshah.Emor, Parshah.Behar, Parshah.Bechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.Nitzavim},
        new Parshah[] {Parshah.None, Parshah.Vayeilech, Parshah.Haazinu, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.Vayakhel, Parshah.Pekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.Shmini, Parshah.Tazria, Parshah.Metzora, Parshah.None, Parshah.AchreiMos, Parshah.Kedoshim, Parshah.Emor, Parshah.Behar, Parshah.Bechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.NitzavimVayeilech},
        new Parshah[] {Parshah.None, Parshah.Vayeilech, Parshah.Haazinu, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.Vayakhel, Parshah.Pekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.Shmini, Parshah.Tazria, Parshah.Metzora, Parshah.None, Parshah.AchreiMos, Parshah.Kedoshim, Parshah.Emor, Parshah.Behar, Parshah.Bechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.Matos, Parshah.Masei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.Nitzavim},
        new Parshah[] {Parshah.None, Parshah.None, Parshah.Haazinu, Parshah.None, Parshah.None, Parshah.Bereshis, Parshah.Noach, Parshah.LechLecha, Parshah.Vayera, Parshah.ChayeiSara, Parshah.Toldos, Parshah.Vayetzei, Parshah.Vayishlach, Parshah.Vayeshev, Parshah.Miketz, Parshah.Vayigash, Parshah.Vayechi, Parshah.Shemos, Parshah.Vaera, Parshah.Bo, Parshah.Beshalach, Parshah.Yisro, Parshah.Mishpatim, Parshah.Terumah, Parshah.Tetzaveh, Parshah.KiSisa, Parshah.Vayakhel, Parshah.Pekudei, Parshah.Vayikra, Parshah.Tzav, Parshah.Shmini, Parshah.Tazria, Parshah.Metzora, Parshah.None, Parshah.AchreiMos, Parshah.Kedoshim, Parshah.Emor, Parshah.Behar, Parshah.Bechukosai, Parshah.Bamidbar, Parshah.Nasso, Parshah.Behaaloscha, Parshah.Shlach, Parshah.Korach, Parshah.Chukas, Parshah.Balak, Parshah.Pinchas, Parshah.MatosMasei, Parshah.Devarim, Parshah.Vaeschanan, Parshah.Eikev, Parshah.Reeh, Parshah.Shoftim, Parshah.KiSeitzei, Parshah.KiSavo, Parshah.NitzavimVayeilech}
    };

    /// <summary>
    /// Is this calendar set to return modern Israeli national holidays. By default, this value is false. The holidays
    /// are <see cref="YomHashoah"><em>Yom HaShoah</em></see>, <see cref="YomHazikaron"><em>Yom Hazikaron</em></see>, <see cref="YomHaatzmaut"><em>Yom Ha'atzmaut</em></see> and <see cref="YomYerushalayim"><em>Yom Yerushalayim</em></see>.
    /// </summary>
    /// <value>the useModernHolidays to set</value>
    /// <remarks>
    /// Sets the calendar to return modern Israeli national holidays. By default, this value is false. The holidays are:
    /// <see cref="YomHashoah"><em>Yom HaShoah</em></see>, <see cref="YomHazikaron"><em>Yom Hazikaron</em></see>, <see cref="YomHaatzmaut"><em>Yom Ha'atzmaut</em></see> and <see cref="YomYerushalayim"><em>Yom Yerushalayim</em></see>.
    /// </remarks>
    /// <seealso cref="UseModernHolidays"/>
    public bool UseModernHolidays
    {
        get => useModernHolidays;
        set => useModernHolidays = value;
    }

    /// <summary>
    /// Default constructor will set a default date to the current system date.
    /// </summary>
    public JewishCalendar()
    {
    }

    /// <summary>
    /// A constructor that initializes the date to the <see cref="DateTimeOffset"/> parameter.
    /// </summary>
    /// <param name="dateTime">the <see cref="DateTimeOffset"/> whose local date (in its own offset) the calendar is set to</param>
    public JewishCalendar(DateTimeOffset dateTime) : base(dateTime)
    {
    }

    /// <summary>
    /// A constructor that initializes the date to the <see cref="DateOnly"/> parameter.
    /// </summary>
    /// <param name="localDate">the <see cref="DateOnly"/> to set the calendar to</param>
    public JewishCalendar(DateOnly localDate) : base(localDate)
    {
    }

    /// <summary>
    /// Creates a Jewish date based on a Jewish year, month and day of month.
    /// </summary>
    /// <param name="jewishYear">the Jewish year</param>
    /// <param name="jewishMonth">the Jewish month. The method expects a 1 for Nissan ... 12 for Adar and 13 for Adar II. Use the
    ///            constants <see cref="JewishDate.Nissan"/> ... <see cref="JewishDate.Adar"/> (or <see cref="JewishDate.AdarII"/> for a leap year Adar II) to avoid any
    ///            confusion.</param>
    /// <param name="jewishDayOfMonth">the Jewish day of month. If 30 is passed in for a month with only 29 days (for example <see cref="JewishDate.Iyar"/>,
    ///            or <see cref="JewishDate.Kislev"/> in a year that <see cref="JewishDate.IsKislevShort(int)"/>), the 29th (last valid date of the month)
    ///            will be set</param>
    /// <exception cref="ArgumentException">if the day of month is &lt; 1 or &gt; 30, or a year of &lt; 0 is passed in.</exception>
    public JewishCalendar(int jewishYear, int jewishMonth, int jewishDayOfMonth) : base(jewishYear, jewishMonth, jewishDayOfMonth)
    {
    }

    /// <summary>
    /// Creates a Jewish date based on a Jewish date and whether in Israel
    /// </summary>
    /// <param name="jewishYear">the Jewish year</param>
    /// <param name="jewishMonth">the Jewish month. The method expects a 1 for <em>Nissan</em> ... 12 for <em>Adar</em> and 13 for
    ///            <em>Adar II</em>. Use the constants <see cref="JewishDate.Nissan"/> ... <see cref="JewishDate.Adar"/> (or <see cref="JewishDate.AdarII"/> for a
    ///            leap year Adar II) to avoid any confusion.</param>
    /// <param name="jewishDayOfMonth">the Jewish day of month. If 30 is passed in for a month with only 29 days (for example <see cref="JewishDate.Iyar"/>,
    ///            or <see cref="JewishDate.Kislev"/> in a year that <see cref="JewishDate.IsKislevShort(int)"/>), the 29th (last valid date of the month)
    ///            will be set.</param>
    /// <param name="inIsrael">whether in Israel. This affects <em>Yom Tov</em> calculations</param>
    public JewishCalendar(int jewishYear, int jewishMonth, int jewishDayOfMonth, bool inIsrael) : base(jewishYear, jewishMonth, jewishDayOfMonth)
    {
        InIsrael = inIsrael;
    }

    /// <summary>
    /// Gets whether Israel holiday scheme is used or not. The default (if not set) is false.
    /// </summary>
    /// <value>set to true for calculations for Israel</value>
    /// <remarks>
    /// Sets whether to use Israel holiday scheme or not. Default is false.
    /// </remarks>
    /// <seealso cref="InIsrael"/>
    public bool InIsrael
    {
        get => inIsrael;
        set => inIsrael = value;
    }

    /// <summary>
    /// Returns if the city is set as a city surrounded by a wall from the time of Yehoshua, and Shushan Purim
    /// should be celebrated as opposed to regular Purim.
    /// </summary>
    /// <value>is the city surrounded by a wall from the time of Yehoshua.</value>
    /// <remarks>
    /// Sets if the location is surrounded by a wall from the time of Yehoshua, and Shushan Purim should be
    /// celebrated as opposed to regular Purim. This should be set for Yerushalayim, Shushan and other cities.
    /// </remarks>
    /// <seealso cref="IsMukafChoma"/>
    public bool IsMukafChoma
    {
        get => isMukafChoma;
        set => isMukafChoma = value;
    }


    /// <summary>
    /// <a href="https://en.wikipedia.org/wiki/Birkat_Hachama">Birkas Hachamah</a> is recited every 28 years based on
    /// <em>Tekufas Shmuel</em> (Julian years) that a year is 365.25 days. The <a href="https://en.wikipedia.org/wiki/Maimonides"
    /// >Rambam</a> in <a href="http://hebrewbooks.org/pdfpager.aspx?req=14278&amp;st=&amp;pgnum=323">Hilchos Kiddush Hachodesh 9:3</a>
    /// states that <em>tekufas Nissan</em> of year 1 was 7 days + 9 hours before <em>molad Nissan</em>. This is calculated as every
    /// 10,227 days (28 * 365.25).
    /// </summary>
    /// <returns>true for a day that <em>Birkas Hachamah</em> is recited.</returns>
    public virtual bool IsBirkasHachamah()
    {
        int elapsedDays = GetJewishCalendarElapsedDays(JewishYear); //elapsed days since molad ToHu
        elapsedDays = elapsedDays + GetDaysSinceStartOfJewishYear(); //elapsed days to the current calendar date

        /* Molad Nissan year 1 was 177 days after molad tohu of Tishrei. We multiply 29.5 days * 6 months from Tishrei
         * to Nissan = 177. Subtract 7 days since tekufas Nissan was 7 days and 9 hours before the molad as stated in the Rambam
         * and we are now at 170 days. Because getJewishCalendarElapsedDays and getDaysSinceStartOfJewishYear use the value for
         * Rosh Hashana as 1, we have to add 1 day for a total of 171. To this add a day since the tekufah is on a Tuesday
         * night, and we push off the bracha to Wednesday morning resulting in the 172 used in the calculation.
         */
        // Pure integer math prevents implicit promotion to double. 28 * 365.25 is exactly 10227 days.
        // Using "return elapsedDays % (28 * 365.25) == 172" would break with an integer overflow in Jewish year >= 5,879,101 😀
        return elapsedDays % 10227 == 172; // 28 years of 365.25 days + the offset from molad tohu mentioned above
    }

    /// <summary>
    /// Return the type of year for <em>parshah</em> calculations. The algorithm follows the
    /// <a href="http://hebrewbooks.org/pdfpager.aspx?req=14268&amp;st=&amp;pgnum=222">Luach Arba'ah Shearim</a> in the Tur Ohr Hachaim.
    /// </summary>
    /// <returns>the type of year for <em>parshah</em> calculations.</returns>
    private int GetParshahYearType()
    {
        int roshHashanaDayOfWeek = (GetJewishCalendarElapsedDays(JewishYear) + 1) % 7; // plus one to the original Rosh Hashana of year 1 to get a week starting on Sunday
        if (roshHashanaDayOfWeek == 0)
        {
            roshHashanaDayOfWeek = 7; // convert 0 to 7 for Shabbos for readability
        }
        if (IsJewishLeapYear())
        {
            switch (roshHashanaDayOfWeek)
            {
                case Weekday.Monday:
                    if (IsKislevShort())
                    { //BaCh
                        if (InIsrael)
                        {
                            return 14;
                        }
                        return 6;
                    }
                    if (IsCheshvanLong())
                    { //BaSh
                        if (InIsrael)
                        {
                            return 15;
                        }
                        return 7;
                    }
                    break;
                case Weekday.Tuesday: //Gak
                    if (InIsrael)
                    {
                        return 15;
                    }
                    return 7;
                case Weekday.Thursday:
                    if (IsKislevShort())
                    { //HaCh
                        return 8;
                    }
                    if (IsCheshvanLong())
                    { //HaSh
                        return 9;
                    }
                    break;
                case Weekday.Saturday:
                    if (IsKislevShort())
                    { //ZaCh
                        return 10;
                    }
                    if (IsCheshvanLong())
                    { //ZaSh
                        if (InIsrael)
                        {
                            return 16;
                        }
                        return 11;
                    }
                    break;
            }
        }
        else
        { //not a leap year
            switch (roshHashanaDayOfWeek)
            {
                case Weekday.Monday:
                    if (IsKislevShort())
                    { //BaCh
                        return 0;
                    }
                    if (IsCheshvanLong())
                    { //BaSh
                        if (InIsrael)
                        {
                            return 12;
                        }
                        return 1;
                    }
                    break;
                case Weekday.Tuesday: //GaK
                    if (InIsrael)
                    {
                        return 12;
                    }
                    return 1;
                case Weekday.Thursday:
                    if (IsCheshvanLong())
                    { //HaSh
                        return 3;
                    }
                    if (!IsKislevShort())
                    { //Hak
                        if (InIsrael)
                        {
                            return 13;
                        }
                        return 2;
                    }
                    break;
                case Weekday.Saturday:
                    if (IsKislevShort())
                    { //ZaCh
                        return 4;
                    }
                    if (IsCheshvanLong())
                    { //ZaSh
                        return 5;
                    }
                    break;
            }
        }
        return -1; //keep the compiler happy
    }

    /// <summary>
    /// Returns this week's <see cref="Parshah"><em>Parshah</em></see> if it is <em>Shabbos</em>. It returns <see cref="Parshah.None"/> if the date
    /// is a weekday or if there is no <em>parshah</em> that week (for example <em>Yom Tov</em> that falls on a <em>Shabbos</em>).
    /// </summary>
    /// <returns>the current <em>parshah</em>.</returns>
    public virtual Parshah GetParshah()
    {
        if (DayOfWeek != Weekday.Saturday)
        {
            return Parshah.None;
        }

        int yearType = GetParshahYearType();
        int roshHashanaDayOfWeek = GetJewishCalendarElapsedDays(JewishYear) % 7;
        int day = roshHashanaDayOfWeek + GetDaysSinceStartOfJewishYear();

        if (yearType >= 0)
        { // negative year should be impossible, but let's cover all bases
            return parshahList[yearType][day / 7];
        }
        return Parshah.None; //keep the compiler happy
    }

    /// <summary>
    /// Returns the upcoming <see cref="Parshah"><em>Parshah</em></see> regardless of if it is the weekday or <em>Shabbos</em> (where next
    /// Shabbos's <em>Parshah</em> will be returned. This is unlike <see cref="GetParshah"/> that returns <see cref="Parshah.None"/> if
    /// the date is not <em>Shabbos</em>. If the upcoming <em>Shabbos</em> is a <em>Yom Tov</em> and has no <em>Parshah</em>, the
    /// following week's <em>parshah</em> will be returned.
    /// </summary>
    /// <returns>the upcoming <em>parshah</em>.</returns>
    public virtual Parshah GetUpcomingParshah()
    {
        JewishCalendar clone = (JewishCalendar)Clone();
        int daysToShabbos = (Weekday.Saturday - DayOfWeek + 7) % 7;
        if (DayOfWeek != Weekday.Saturday)
        {
            clone.PlusDays(daysToShabbos);
        }
        else
        {
            clone.PlusDays(7);
        }
        while (clone.GetParshah() == Parshah.None)
        { //Yom Kippur / Sukkos or Pesach with 2 potential non-parshah Shabbosim in a row
            clone.PlusDays(7);
        }
        return clone.GetParshah();
    }

    /// <summary>
    /// Returns a <see cref="Parshah"><em>Parshah</em></see> enum if the <em>Shabbos</em> is one of the four <em>parshiyos</em> of <see cref="Parshah.Shkalim"><em>Shkalim</em></see>, <see cref="Parshah.Zachor"><em>Zachor</em></see>, <see cref="Parshah.Para"><em>Para</em></see>, <see cref="Parshah.Hachodesh"><em>Hachodesh</em></see>, or five other special <em>Shabbasos</em> of <see cref="Parshah.Hagadol"><em>Hagadol</em></see>,
    /// <see cref="Parshah.Chazon"><em>Chazon</em></see>, <see cref="Parshah.Nachamu"><em>Nachamu</em></see>, <see cref="Parshah.Shuva"><em>Shuva</em></see>,
    /// <see cref="Parshah.Shira"><em>Shira</em></see>, or <see cref="Parshah.None">Parshah.NONE</see> for a regular <em>Shabbos</em> (or any weekday).
    /// </summary>
    /// <returns>one of the four <em>parshiyos</em> of <see cref="Parshah.Shkalim"><em>Shkalim</em></see>, <see cref="Parshah.Zachor"><em>Zachor</em></see>,
    ///         <see cref="Parshah.Para"><em>Para</em></see>, <see cref="Parshah.Hachodesh"><em>Hachodesh</em></see>, or five other special <em>Shabbasos</em>
    ///         of <see cref="Parshah.Hagadol"><em>Hagadol</em></see>, <see cref="Parshah.Chazon"><em>Chazon</em></see>, <see cref="Parshah.Nachamu"><em>Nachamu</em></see>,
    ///         <see cref="Parshah.Shuva"><em>Shuva</em></see>, <see cref="Parshah.Shira"><em>Shira</em></see>, or <see cref="Parshah.None">Parshah.NONE</see> for a regular
    ///         <em>Shabbos</em> (or any weekday).</returns>
    public virtual Parshah GetSpecialShabbos()
    {
        if (DayOfWeek == Weekday.Saturday)
        {
            if ((JewishMonth == Shevat && !IsJewishLeapYear()) || (JewishMonth == Adar && IsJewishLeapYear()))
            {
                if (JewishDayOfMonth == 25 || JewishDayOfMonth == 27 || JewishDayOfMonth == 29)
                {
                    return Parshah.Shkalim;
                }
            }
            if ((JewishMonth == Adar && !IsJewishLeapYear()) || JewishMonth == AdarII)
            {
                if (JewishDayOfMonth == 1)
                {
                    return Parshah.Shkalim;
                }
                if (JewishDayOfMonth == 8 || JewishDayOfMonth == 9 || JewishDayOfMonth == 11 || JewishDayOfMonth == 13)
                {
                    return Parshah.Zachor;
                }
                if (JewishDayOfMonth == 18 || JewishDayOfMonth == 20 || JewishDayOfMonth == 22 || JewishDayOfMonth == 23)
                {
                    return Parshah.Para;
                }
                if (JewishDayOfMonth == 25 || JewishDayOfMonth == 27 || JewishDayOfMonth == 29)
                {
                    return Parshah.Hachodesh;
                }
            }
            if (JewishMonth == Nissan)
            {
                if (JewishDayOfMonth == 1)
                {
                    return Parshah.Hachodesh;
                }
                if (JewishDayOfMonth >= 8 && JewishDayOfMonth <= 14)
                {
                    return Parshah.Hagadol;
                }
            }
            if (JewishMonth == Av)
            {
                if (JewishDayOfMonth >= 4 && JewishDayOfMonth <= 9)
                {
                    return Parshah.Chazon;
                }
                if (JewishDayOfMonth >= 10 && JewishDayOfMonth <= 16)
                {
                    return Parshah.Nachamu;
                }
            }
            if (JewishMonth == Tishrei)
            {
                if (JewishDayOfMonth >= 3 && JewishDayOfMonth <= 8)
                {
                    return Parshah.Shuva;
                }

            }
            if (GetParshah() == Parshah.Beshalach)
            {
                return Parshah.Shira;
            }
        }
        return Parshah.None;
    }

    /// <summary>
    /// Returns an index of the Jewish holiday or fast day for the current day, or a -1 if there is no holiday for this day.
    /// There are constants in this class representing each <em>Yom Tov</em>. Formatting of the <em>Yomim tovim</em> is done
    /// in the <see cref="HebrewDateFormatter.FormatYomTov"/>.
    /// </summary>
    /// <returns>the index of the holiday such as the constant <see cref="LagBaomer"/> or <see cref="YomKippur"/> or a -1 if it is not a holiday.</returns>
    /// <remarks>
    /// <b>To do:</b> Consider using enums instead of the constant ints.
    /// </remarks>
    /// <seealso cref="HebrewDateFormatter.FormatYomTov"/>
    public virtual int GetYomTovIndex()
    {
        int day = JewishDayOfMonth;
        int dayOfWeek = DayOfWeek;

        // check by month (starting from Nissan)
        switch (JewishMonth)
        {
            case Nissan:
                if (day == 14)
                {
                    return ErevPesach;
                }
                if (day == 15 || day == 21
                        || (!inIsrael && (day == 16 || day == 22)))
                {
                    return Pesach;
                }
                if (day >= 17 && day <= 20 || day == 16)
                {
                    return CholHamoedPesach;
                }
                if ((day == 22 && inIsrael) || (day == 23 && !inIsrael))
                {
                    return IsruChag;
                }
                if (UseModernHolidays
                        && ((day == 26 && dayOfWeek == Weekday.Thursday)
                                || (day == 28 && dayOfWeek == Weekday.Monday)
                                || (day == 27 && dayOfWeek != Weekday.Sunday && dayOfWeek != Weekday.Friday)))
                {
                    return YomHashoah;
                }
                break;
            case Iyar:
                if (UseModernHolidays
                        && ((day == 4 && dayOfWeek == Weekday.Tuesday)
                                || ((day == 3 || day == 2) && dayOfWeek == Weekday.Wednesday) || (day == 5 && dayOfWeek == Weekday.Monday)))
                {
                    return YomHazikaron;
                }
                // if 5 Iyar falls on Wed, Yom Haatzmaut is that day. If it falls on Friday or Shabbos, it is moved back to
                // Thursday. If it falls on Monday it is moved to Tuesday
                if (UseModernHolidays
                        && ((day == 5 && dayOfWeek == Weekday.Wednesday)
                                || ((day == 4 || day == 3) && dayOfWeek == Weekday.Thursday) || (day == 6 && dayOfWeek == Weekday.Tuesday)))
                {
                    return YomHaatzmaut;
                }
                if (day == 14)
                {
                    return PesachSheni;
                }
                if (day == 18)
                {
                    return LagBaomer;
                }
                if (UseModernHolidays && day == 28)
                {
                    return YomYerushalayim;
                }
                break;
            case Sivan:
                if (day == 5)
                {
                    return ErevShavuos;
                }
                if (day == 6 || (day == 7 && !inIsrael))
                {
                    return Shavuos;
                }
                if ((day == 7 && inIsrael) || (day == 8 && !inIsrael))
                {
                    return IsruChag;
                }
                break;
            case Tammuz:
                // push off the fast day if it falls on Shabbos
                if ((day == 17 && dayOfWeek != Weekday.Saturday)
                        || (day == 18 && dayOfWeek == Weekday.Sunday))
                {
                    return SeventeenOfTammuz;
                }
                break;
            case Av:
                // if Tisha B'av falls on Shabbos, push off until Sunday
                if ((dayOfWeek == Weekday.Sunday && day == 10)
                        || (dayOfWeek != Weekday.Saturday && day == 9))
                {
                    return TishaBeav;
                }
                if (day == 15)
                {
                    return TuBeav;
                }
                break;
            case Elul:
                if (day == 29)
                {
                    return ErevRoshHashana;
                }
                break;
            case Tishrei:
                if (day == 1 || day == 2)
                {
                    return RoshHashana;
                }
                if ((day == 3 && dayOfWeek != Weekday.Saturday) || (day == 4 && dayOfWeek == Weekday.Sunday))
                {
                    // push off Tzom Gedalia if it falls on Shabbos
                    return FastOfGedalyah;
                }
                if (day == 9)
                {
                    return ErevYomKippur;
                }
                if (day == 10)
                {
                    return YomKippur;
                }
                if (day == 14)
                {
                    return ErevSuccos;
                }
                if (day == 15 || (day == 16 && !inIsrael))
                {
                    return Succos;
                }
                if (day >= 16 && day <= 20)
                {
                    return CholHamoedSuccos;
                }
                if (day == 21)
                {
                    return HoshanaRabba;
                }
                if (day == 22)
                {
                    return SheminiAtzeres;
                }
                if (day == 23 && !inIsrael)
                {
                    return SimchasTorah;
                }
                if ((day == 23 && inIsrael) || (day == 24 && !inIsrael))
                {
                    return IsruChag;
                }
                break;
            case Kislev: // no yomtov in CHESHVAN
                if (day >= 25)
                {
                    return Chanukah;
                }
                break;
            case Teves:
                if (day == 1 || day == 2
                        || (day == 3 && IsKislevShort()))
                {
                    return Chanukah;
                }
                if (day == 10)
                {
                    return TenthOfTeves;
                }
                break;
            case Shevat:
                if (day == 15)
                {
                    return TuBeshvat;
                }
                break;
            case Adar:
                if (!IsJewishLeapYear())
                {
                    // if 13th Adar falls on Friday or Shabbos, push back to Thursday
                    if (((day == 11 || day == 12) && dayOfWeek == Weekday.Thursday)
                            || (day == 13 && !(dayOfWeek == Weekday.Friday || dayOfWeek == Weekday.Saturday)))
                    {
                        return FastOfEsther;
                    }
                    if (day == 14)
                    {
                        return Purim;
                    }
                    if (day == 15)
                    {
                        return ShushanPurim;
                    }
                }
                else
                { // else if a leap year
                    if (day == 14)
                    {
                        return PurimKatan;
                    }
                    if (day == 15)
                    {
                        return ShushanPurimKatan;
                    }
                }
                break;
            case AdarII:
                // if 13th Adar falls on Friday or Shabbos, push back to Thursday
                if (((day == 11 || day == 12) && dayOfWeek == Weekday.Thursday)
                        || (day == 13 && !(dayOfWeek == Weekday.Friday || dayOfWeek == Weekday.Saturday)))
                {
                    return FastOfEsther;
                }
                if (day == 14)
                {
                    return Purim;
                }
                if (day == 15)
                {
                    return ShushanPurim;
                }
                break;
        }
        // if we get to this stage, then there are no holidays for the given date return -1
        return -1;
    }

    /// <summary>
    /// Returns true if the current day is <em>Yom Tov</em>. The method returns true even for holidays such as <see cref="Chanukah"/>
    /// and minor ones such as <see cref="TuBeav"/> and <see cref="PesachSheni"/>. <em>Erev Yom Tov</em> (with the exception of
    /// <see cref="HoshanaRabba"/> and <em>erev</em> the second days of <see cref="Pesach"/>) returns false, as do <see cref="IsTaanis">fast
    /// days</see> besides <see cref="YomKippur"/>. Use <see cref="IsAssurBemelacha"/> to find the days that have a prohibition of work.
    /// </summary>
    /// <returns>true if the current day is a Yom Tov</returns>
    /// <seealso cref="GetYomTovIndex"/>
    /// <seealso cref="IsErevYomTov"/>
    /// <seealso cref="IsErevYomTovSheni"/>
    /// <seealso cref="IsTaanis"/>
    /// <seealso cref="IsAssurBemelacha"/>
    /// <seealso cref="IsCholHamoed"/>
    public virtual bool IsYomTov()
    {
        int holidayIndex = GetYomTovIndex();
        if ((IsErevYomTov() && !(holidayIndex == HoshanaRabba || holidayIndex == CholHamoedPesach))
                || (IsTaanis() && holidayIndex != YomKippur) || holidayIndex == IsruChag)
        {
            return false;
        }
        return GetYomTovIndex() != -1;
    }

    /// <summary>
    /// Returns true if the <em>Yom Tov</em> day has a <em>melacha</em> (work)  prohibition. This method will return false for a
    /// non-<em>Yom Tov</em> day, even if it is <em>Shabbos</em>.
    /// </summary>
    /// <returns>if the <em>Yom Tov</em> day has a <em>melacha</em> (work)  prohibition.</returns>
    public virtual bool IsYomTovAssurBemelacha()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == Pesach || holidayIndex == Shavuos || holidayIndex == Succos || holidayIndex == SheminiAtzeres ||
                holidayIndex == SimchasTorah || holidayIndex == RoshHashana || holidayIndex == YomKippur;
    }

    /// <summary>
    /// Returns true if it is <em>Shabbos</em> or if it is a <em>Yom Tov</em> day that has a <em>melacha</em> (work)  prohibition.
    /// </summary>
    /// <returns>if the day is a <em>Yom Tov</em> that is <em>assur bemlacha</em> or <em>Shabbos</em></returns>
    public virtual bool IsAssurBemelacha()
    {
        return DayOfWeek == Weekday.Saturday || IsYomTovAssurBemelacha();
    }

    /// <summary>
    /// Returns if <em>melacha</em> is permissible at night at the end of a day that was <see cref="IsAssurBemelacha"/> (i.e. that the
    /// following day is not <em>assur bemelacha</em>). If it is a case of a <em>Yom Tov</em> night that falls on a <em>motzai
    /// Shabbos</em> that has <em>havdalah</em> in <em>kiddush</em>, false will be returned. Don't rely on this to determine if
    /// <em>havdalah</em> should be recited.
    /// </summary>
    /// <returns><c>true</c> if <em>melacha</em> is permissible at night at the end of a day that was <see cref="IsAssurBemelacha"/>.</returns>
    /// <seealso cref="IsAssurBemelacha"/>
    public virtual bool IsTonightMutarBemelacha()
    {
        if (!IsAssurBemelacha())
        {
            return false;
        }
        JewishCalendar tomorrow = (JewishCalendar)Clone();
        tomorrow.PlusDays(1);
        return !tomorrow.IsAssurBemelacha();
    }

    /// <summary>
    /// Returns true if the day has candle lighting. This will return true on <em>Erev Shabbos</em>, <em>Erev Yom Tov</em>, the
    /// first day of <em>Rosh Hashana</em> and the first days of <em>Yom Tov</em> out of Israel. It is identical
    /// to calling <see cref="IsTomorrowShabbosOrYomTov"/>.
    /// </summary>
    /// <returns>if the day has candle lighting.</returns>
    /// <seealso cref="IsTomorrowShabbosOrYomTov"/>
    public virtual bool HasCandleLighting()
    {
        return IsTomorrowShabbosOrYomTov();
    }

    /// <summary>
    /// Returns true if tomorrow is <em>Shabbos</em> or <em>Yom Tov</em>. This will return true on <em>Erev Shabbos</em>,
    /// <em>Erev Yom Tov</em>, the first day of <em>Rosh Hashana</em> and <em>erev</em> the first days of <em>Yom Tov</em>
    /// out of Israel. It is identical to calling <see cref="HasCandleLighting"/>.
    /// </summary>
    /// <returns>will return if the next day is <em>Shabbos</em> or <em>Yom Tov</em>.</returns>
    /// <seealso cref="HasCandleLighting"/>
    public virtual bool IsTomorrowShabbosOrYomTov()
    {
        return DayOfWeek == Weekday.Friday || IsErevYomTov() || IsErevYomTovSheni();
    }

    /// <summary>
    /// Returns true if the day is the second day of <em>Yom Tov</em>. This impacts the second day of <em>Rosh Hashana</em> everywhere and
    /// the second days of Yom Tov in <em>chutz laaretz</em> (out of Israel).
    /// </summary>
    /// <returns>if the day is the second day of <em>Yom Tov</em>.</returns>
    public virtual bool IsErevYomTovSheni()
    {
        return (JewishMonth == Tishrei && (JewishDayOfMonth == 1))
        || (!InIsrael
                && ((JewishMonth == Nissan && (JewishDayOfMonth == 15 || JewishDayOfMonth == 21))
                || (JewishMonth == Tishrei && (JewishDayOfMonth == 15 || JewishDayOfMonth == 22))
                || (JewishMonth == Sivan && JewishDayOfMonth == 6)));
    }

    /// <summary>
    /// Returns true if the current day is <em>Aseres Yemei Teshuva</em>.
    /// </summary>
    /// <returns>if the current day is <em>Aseres Yemei Teshuva</em></returns>
    public virtual bool IsAseresYemeiTeshuva()
    {
        return JewishMonth == Tishrei && JewishDayOfMonth <= 10;
    }

    /// <summary>
    /// Returns true if the current day is <em>Pesach</em> (either  the <em>Yom Tov</em> of <em>Pesach</em> or<em>Chol Hamoed Pesach</em>).
    /// </summary>
    /// <returns>true if the current day is <em>Pesach</em> (either  the <em>Yom Tov</em> of <em>Pesach</em> or<em>Chol Hamoed Pesach</em>).</returns>
    /// <seealso cref="IsYomTov"/>
    /// <seealso cref="IsCholHamoedPesach"/>
    /// <seealso cref="Pesach"/>
    /// <seealso cref="CholHamoedPesach"/>
    public virtual bool IsPesach()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == Pesach || holidayIndex == CholHamoedPesach;
    }

    /// <summary>
    /// Returns true if the current day is <em>Chol Hamoed</em> of <em>Pesach</em>.
    /// </summary>
    /// <returns>true if the current day is <em>Chol Hamoed</em> of <em>Pesach</em></returns>
    /// <seealso cref="IsYomTov"/>
    /// <seealso cref="IsPesach"/>
    /// <seealso cref="CholHamoedPesach"/>
    public virtual bool IsCholHamoedPesach()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == CholHamoedPesach;
    }

    /// <summary>
    /// Returns true if the current day is <em>Shavuos</em>.
    /// </summary>
    /// <returns>true if the current day is <em>Shavuos</em>.</returns>
    /// <seealso cref="IsYomTov"/>
    /// <seealso cref="Shavuos"/>
    public virtual bool IsShavuos()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == Shavuos;
    }

    /// <summary>
    /// Returns true if the current day is <em>Rosh Hashana</em>.
    /// </summary>
    /// <returns>true if the current day is <em>Rosh Hashana</em>.</returns>
    /// <seealso cref="IsYomTov"/>
    /// <seealso cref="RoshHashana"/>
    public virtual bool IsRoshHashana()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == RoshHashana;
    }

    /// <summary>
    /// Returns true if the current day is <em>Yom Kippur</em>.
    /// </summary>
    /// <returns>true if the current day is <em>Yom Kippur</em>.</returns>
    /// <seealso cref="IsYomTov"/>
    /// <seealso cref="YomKippur"/>
    public virtual bool IsYomKippur()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == YomKippur;
    }

    /// <summary>
    /// Returns true if the current day is <em>Succos</em> (either  the <em>Yom Tov</em> of <em>Succos</em> or<em>Chol Hamoed Succos</em>).
    /// It will return false for <see cref="IsShminiAtzeres">Shmini Atzeres</see> and <see cref="IsSimchasTorah">Simchas Torah</see>.
    /// </summary>
    /// <returns>true if the current day is <em>Succos</em> (either  the <em>Yom Tov</em> of <em>Succos</em> or<em>Chol Hamoed Succos</em>.</returns>
    /// <seealso cref="IsYomTov"/>
    /// <seealso cref="IsCholHamoedSuccos"/>
    /// <seealso cref="IsHoshanaRabba"/>
    /// <seealso cref="Succos"/>
    /// <seealso cref="CholHamoedSuccos"/>
    /// <seealso cref="HoshanaRabba"/>
    public virtual bool IsSuccos()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == Succos || holidayIndex == CholHamoedSuccos || holidayIndex == HoshanaRabba;
    }

    /// <summary>
    /// Returns true if the current day is <em>Hoshana Rabba</em>.
    /// </summary>
    /// <returns>true if the current day is <em>Hoshana Rabba</em>.</returns>
    /// <seealso cref="IsYomTov"/>
    /// <seealso cref="HoshanaRabba"/>
    public virtual bool IsHoshanaRabba()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == HoshanaRabba;
    }

    /// <summary>
    /// Returns true if the current day is <em>Shmini Atzeres</em>.
    /// </summary>
    /// <returns>true if the current day is <em>Shmini Atzeres</em>.</returns>
    /// <seealso cref="IsYomTov"/>
    /// <seealso cref="SheminiAtzeres"/>
    public virtual bool IsShminiAtzeres()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == SheminiAtzeres;
    }

    /// <summary>
    /// Returns true if the current day is <em>Simchas Torah</em>. This will always return false if <see cref="InIsrael">in Israel</see>
    /// </summary>
    /// <returns>true if the current day is <em>Shmini Atzeres</em>.</returns>
    /// <seealso cref="IsYomTov"/>
    /// <seealso cref="SimchasTorah"/>
    public virtual bool IsSimchasTorah()
    {
        int holidayIndex = GetYomTovIndex();
        //if in Israel, Holiday index of SIMCHAS_TORAH will not be returned by getYomTovIndex()
        return holidayIndex == SimchasTorah;
    }

    /// <summary>
    /// Returns true if the current day is <em>Chol Hamoed</em> of <em>Succos</em>.
    /// </summary>
    /// <returns>true if the current day is <em>Chol Hamoed</em> of <em>Succos</em></returns>
    /// <seealso cref="IsYomTov"/>
    /// <seealso cref="CholHamoedSuccos"/>
    public virtual bool IsCholHamoedSuccos()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == CholHamoedSuccos || holidayIndex == HoshanaRabba;
    }

    /// <summary>
    /// Returns true if the current day is <em>Chol Hamoed</em> of <em>Pesach</em> or <em>Succos</em>.
    /// </summary>
    /// <returns>true if the current day is <em>Chol Hamoed</em> of <em>Pesach</em> or <em>Succos</em></returns>
    /// <seealso cref="IsYomTov"/>
    /// <seealso cref="CholHamoedPesach"/>
    /// <seealso cref="CholHamoedSuccos"/>
    public virtual bool IsCholHamoed()
    {
        return IsCholHamoedPesach() || IsCholHamoedSuccos();
    }

    /// <summary>
    /// Returns true if the current day is <em>Erev Yom Tov</em>. The method returns true for <em>Erev</em> - <em>Pesach</em>
    /// (first and last days), <em>Shavuos</em>, <em>Rosh Hashana</em>, <em>Yom Kippur</em>, <em>Succos</em> and <em>Hoshana
    /// Rabba</em>.
    /// </summary>
    /// <returns>true if the current day is <em>Erev</em> - <em>Pesach</em>, <em>Shavuos</em>, <em>Rosh Hashana</em>, <em>Yom
    /// Kippur</em>, <em>Succos</em> and <em>Hoshana Rabba</em>.</returns>
    /// <seealso cref="IsYomTov"/>
    /// <seealso cref="IsErevYomTovSheni"/>
    public virtual bool IsErevYomTov()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == ErevPesach || holidayIndex == ErevShavuos || holidayIndex == ErevRoshHashana
                || holidayIndex == ErevYomKippur || holidayIndex == ErevSuccos || holidayIndex == HoshanaRabba
                || (holidayIndex == CholHamoedPesach && JewishDayOfMonth == 20);
    }

    /// <summary>
    /// Returns true if the current day is <em>Erev Rosh Chodesh</em>. Returns false for <em>Erev Rosh Hashana</em>.
    /// </summary>
    /// <returns>true if the current day is <em>Erev Rosh Chodesh</em>. Returns false for <em>Erev Rosh Hashana</em>.</returns>
    /// <seealso cref="IsRoshChodesh"/>
    public virtual bool IsErevRoshChodesh()
    {
        // Erev Rosh Hashana is not Erev Rosh Chodesh.
        return (JewishDayOfMonth == 29 && JewishMonth != Elul);
    }


    /// <summary>
    /// Returns true if the current day is <em>Yom Kippur Katan</em>. Returns false for <em>Erev Rosh Hashana</em>,
    /// <em>Erev Rosh Chodesh Cheshvan</em>, <em>Teves</em> and <em>Iyyar</em>. If <em>Erev Rosh Chodesh</em> occurs
    /// on a Friday or <em>Shabbos</em>, <em>Yom Kippur Katan</em> is moved back to Thursday.
    /// </summary>
    /// <returns>true if the current day is <em>Erev Rosh Chodesh</em>. Returns false for <em>Erev Rosh Hashana</em>.</returns>
    /// <seealso cref="IsRoshChodesh"/>
    public virtual bool IsYomKippurKatan()
    {
        int dayOfWeek = DayOfWeek;
        int month = JewishMonth;
        int day = JewishDayOfMonth;
        if (month == JewishDate.Elul || month == JewishDate.Tishrei || month == JewishDate.Kislev || month == JewishDate.Nissan)
        {
            return false;
        }

        if (day == 29 && dayOfWeek != Weekday.Friday && dayOfWeek != Weekday.Saturday)
        {
            return true;
        }
        return (day == 27 || day == 28) && dayOfWeek == Weekday.Thursday;
    }

    /// <summary>
    /// The Monday, Thursday and Monday after the first <em>Shabbos</em> after <see cref="IsRoshChodesh"><em>Rosh Chodesh</em></see>
    /// <see cref="JewishDate.Cheshvan"><em>Cheshvan</em></see> and <see cref="JewishDate.Iyar"><em>Iyar</em></see> are <a href=
    /// "https://outorah.org/p/41334/"> <em>BeHaB</em></a> days. If the last Monday of Iyar's BeHaB coincides with <see cref="PesachSheni"><em>Pesach Sheni</em></see>, the method currently considers it both <em>Pesach Sheni</em> and <em>BeHaB</em>.
    /// As seen in an Ohr Sameach  article on the subject <a href="https://ohr.edu/this_week/insights_into_halacha/9340">The
    /// unknown Days: BeHaB Vs. Pesach Sheini?</a> there are some customs that delay the day to various points in the future.
    /// </summary>
    /// <returns>true if the day is <em>BeHaB</em>.</returns>
    public virtual bool IsBeHaB()
    {
        int dayOfWeek = DayOfWeek;
        int month = JewishMonth;
        int day = JewishDayOfMonth;

        if (month == JewishDate.Cheshvan || month == JewishDate.Iyar)
        {
            return (dayOfWeek == Weekday.Monday && day > 4 && day < 18)
                    || (dayOfWeek == Weekday.Thursday && day > 7 && day < 14);
        }
        return false;
    }

    /// <summary>
    /// Return true if the day is a Taanis (fast day). Return true for <em>17 of Tammuz</em>, <em>Tisha B'Av</em>,
    /// <em>Yom Kippur</em>, <em>Fast of Gedalyah</em>, <em>10 of Teves</em> and the <em>Fast of Esther</em>.
    /// </summary>
    /// <returns>true if today is a fast day</returns>
    public virtual bool IsTaanis()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == SeventeenOfTammuz || holidayIndex == TishaBeav || holidayIndex == YomKippur
                || holidayIndex == FastOfGedalyah || holidayIndex == TenthOfTeves || holidayIndex == FastOfEsther;
    }

    /// <summary>
    /// Return true if the day is <em>Taanis Bechoros</em> (on <em>Erev Pesach</em>). It will return true for the 14th
    /// of <em>Nissan</em> if it is not on <em>Shabbos</em>, or if the 12th of <em>Nissan</em> occurs on a Thursday.
    /// </summary>
    /// <returns>true if today is <em>Taanis Bechoros</em>.</returns>
    public virtual bool IsTaanisBechoros()
    {
        int day = JewishDayOfMonth;
        int dayOfWeek = DayOfWeek;
        // on 14 Nissan unless that is Shabbos where the fast is moved back to Thursday
        return JewishMonth == Nissan && ((day == 14 && dayOfWeek != Weekday.Saturday) ||
                (day == 12 && dayOfWeek == Weekday.Thursday));
    }

    /// <summary>
    /// Returns the day of <em>Chanukah</em> or -1 if it is not <em>Chanukah</em>.
    /// </summary>
    /// <returns>the day of <em>Chanukah</em> or -1 if it is not <em>Chanukah</em>.</returns>
    /// <seealso cref="IsChanukah"/>
    public virtual int GetDayOfChanukah()
    {
        int day = JewishDayOfMonth;
        if (IsChanukah())
        {
            if (JewishMonth == Kislev)
            {
                return day - 24;
            }
            else
            { // teves
                return IsKislevShort() ? day + 5 : day + 6;
            }
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// Returns true if the current day is one of the 8 days of <em>Chanukah</em>.
    /// </summary>
    /// <returns>if the current day is one of the 8 days of <em>Chanukah</em>.</returns>
    /// <seealso cref="GetDayOfChanukah"/>
    public virtual bool IsChanukah()
    {
        return GetYomTovIndex() == Chanukah;
    }

    /// <summary>
    /// Returns if the day is Purim (<a href="https://en.wikipedia.org/wiki/Purim#Shushan_Purim">Shushan Purim</a>
    /// in a mukaf choma and regular Purim in a non-mukaf choma).
    /// </summary>
    /// <returns>if the day is Purim (Shushan Purim in a mukaf choma and regular Purim in a non-mukaf choma)</returns>
    /// <seealso cref="IsMukafChoma"/>
    /// <seealso cref="IsMukafChoma"/>
    public virtual bool IsPurim()
    {
        if (isMukafChoma)
        {
            return GetYomTovIndex() == ShushanPurim;
        }
        else
        {
            return GetYomTovIndex() == Purim;
        }
    }

    /// <summary>
    /// Returns if the day is Rosh Chodesh. Rosh Hashana will return false
    /// </summary>
    /// <returns>true if it is Rosh Chodesh. Rosh Hashana will return false</returns>
    public virtual bool IsRoshChodesh()
    {
        // Rosh Hashana is not rosh chodesh. Elul never has 30 days
        return (JewishDayOfMonth == 1 && JewishMonth != Tishrei) || JewishDayOfMonth == 30;
    }

    /// <summary>
    /// Returns if the day is <em>Shabbos</em> and Sunday is <em>Rosh Chodesh</em>.
    /// </summary>
    /// <returns>true if it is <em>Shabbos</em> and Sunday is <em>Rosh Chodesh</em>.</returns>
    /// <remarks>
    /// <b>To do:</b> There is more to tweak in this method (it does not cover all cases and opinions), and it may be removed.
    /// </remarks>
    public virtual bool IsMacharChodesh()
    {
        return (DayOfWeek == Weekday.Saturday && (JewishDayOfMonth == 30 || JewishDayOfMonth == 29));
    }

    /// <summary>
    /// Returns if the day is <em>Shabbos Mevorchim</em>.
    /// </summary>
    /// <returns>true if it is <em>Shabbos Mevorchim</em>.</returns>
    public virtual bool IsShabbosMevorchim()
    {
        return (DayOfWeek == Weekday.Saturday && JewishDayOfMonth >= 23 && JewishDayOfMonth <= 29 && JewishMonth != Elul);
    }

    /// <summary>
    /// Returns the int value of the <em>Omer</em> day or -1 if the day is not in the <em>Omer</em>.
    /// </summary>
    /// <returns>The <em>Omer</em> count as an int or -1 if it is not a day of the <em>Omer</em>.</returns>
    public virtual int GetDayOfOmer()
    {
        int omer = -1; // not a day of the Omer
        int month = JewishMonth;
        int day = JewishDayOfMonth;

        // if Nissan and second day of Pesach and on
        if (month == Nissan && day >= 16)
        {
            omer = day - 15;
            // if Iyar
        }
        else if (month == Iyar)
        {
            omer = day + 15;
            // if Sivan and before Shavuos
        }
        else if (month == Sivan && day < 6)
        {
            omer = day + 44;
        }
        return omer;
    }

    /// <summary>
    /// Returns if the day is Tisha Be'Av (the 9th of Av).
    /// </summary>
    /// <returns>if the day is Tisha Be'Av (the 9th of Av).</returns>
    public virtual bool IsTishaBav()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == TishaBeav;
    }

    /// <summary>
    /// Returns the <em>molad</em> in Standard Time in Yerushalayim as a <see cref="DateTimeOffset"/>. The traditional calculation uses local time.
    /// This method subtracts 20.94 minutes (20 minutes and 56.496 seconds) from the local time (of <em>Har Habayis</em>
    /// with a longitude of 35.2354° is 5.2354° away from the %15 timezone longitude) to get to standard time. This
    /// method intentionally uses standard time and not daylight savings time. The returned value has a UTC offset of zero; use
    /// <see cref="TimeZoneInfo.ConvertTime(DateTimeOffset, TimeZoneInfo)"/> to display it in another time zone.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the moment of the <em>molad</em> in Yerushalayim standard time (GMT + 2), expressed in UTC</returns>
    public virtual DateTimeOffset? GetMoladAsDateTimeOffset()
    {
        JewishDate molad = GetMolad();

        // Standard time offset for Jerusalem: GMT+2
        // The raw point in time must be generated using standard time. Using "Asia/Jerusalem" timezone will result in the time
        // being incorrectly off by an hour in the summer due to DST. Proper adjustment for the actual time in DST will be done
        // by the formatter class used to display the Instant.
        TimeSpan jerusalemStandardOffset = TimeSpan.FromHours(2);

        double moladSeconds = molad.MoladChalakim * 10.0 / 3.0; // Compute molad seconds from chalakim
        int seconds = (int)moladSeconds;
        int nanos = (int)((moladSeconds - seconds) * 1_000_000_000); // convert remainder to nanos

        TimeOnly time = new TimeOnly(molad.MoladHours, molad.MoladMinutes, seconds);

        DateTimeOffset moladZdt = new DateTimeOffset(molad.LocalDate.ToDateTime(time, DateTimeKind.Unspecified), jerusalemStandardOffset)
                .AddTicks(JavaTime.NanosToTicks(nanos));

        // Har Habayis at a longitude of 35.2354 offset vs longitude 35 in standard time, so we subtract the time difference
        // of 20.94 minutes (20 minutes and 56 seconds and 496 millis) to get to Standard time from local mean time
        TimeSpan jerusalemStandardTimeOffset = JavaTime.OfMinutes(20) + JavaTime.OfSeconds(56) + JavaTime.DurationOfMillis(496);

        return (moladZdt - jerusalemStandardTimeOffset).ToUniversalTime();
    }

    /// <summary>
    /// Returns the earliest time of <em>Kiddush Levana</em> calculated as 3 days after the molad. This method returns the time
    /// even if it is during the day when <em>Kiddush Levana</em> can't be said. Callers of this method should consider
    /// displaying the next <em>tzais</em> if the <em>zman</em> is between <em>alos</em> and <em>tzais</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the moment 3 days after the molad.</returns>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetTchilasZmanKidushLevana3Days()"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetTchilasZmanKidushLevana3Days()"/>
    public virtual DateTimeOffset? GetTchilasZmanKidushLevana3Days()
    {
        return GetMoladAsDateTimeOffset().Plus(JavaTime.OfHours(72)); // 3 days after the molad
    }

    /// <summary>
    /// Returns the earliest time of <em>Kiddush Levana</em> calculated as 7 days after the <em>molad</em> as mentioned
    /// by the <a href="http://en.wikipedia.org/wiki/Yosef_Karo">Mechaber</a>. See the <a
    /// href="http://en.wikipedia.org/wiki/Yoel_Sirkis">Bach's</a> opinion on this time. This method returns the time
    /// even if it is during the day when <em>Kiddush Levana</em> can't be said. Callers of this method should consider
    /// displaying the next <em>tzais</em> if the <em>zman</em> is between <em>alos</em> and <em>tzais</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the moment 7 days after the molad.</returns>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetTchilasZmanKidushLevana7Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetTchilasZmanKidushLevana7Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    public virtual DateTimeOffset? GetTchilasZmanKidushLevana7Days()
    {
        return GetMoladAsDateTimeOffset().Plus(JavaTime.OfHours(168)); // 7 days after the molad
    }

    /// <summary>
    /// Returns the latest time of Kiddush Levana according to the <a
    /// href="http://en.wikipedia.org/wiki/Yaakov_ben_Moshe_Levi_Moelin">Maharil's</a> opinion that it is calculated as
    /// halfway between <em>molad</em> and <em>molad</em>. This adds half the 29 days, 12 hours and 793 <em>chalakim</em>
    /// time between <em>molad</em> and <em>molad</em> (14 days, 18 hours, 22 minutes and 666 milliseconds) to the month's
    /// <em>molad</em>. This method returns the time even if it is during the day when <em>Kiddush Levana</em> can't be
    /// recited. Callers of this method should consider displaying <em>alos</em> before this time if the <em>zman</em> is
    /// between <em>alos</em> and <em>tzais</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the moment halfway between <em>molad</em> and <em>molad</em>.</returns>
    /// <seealso cref="GetSofZmanKidushLevana15Days"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSofZmanKidushLevanaBetweenMoldos(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSofZmanKidushLevanaBetweenMoldos(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    public virtual DateTimeOffset? GetSofZmanKidushLevanaBetweenMoldos()
    {
        DateTimeOffset? molad = GetMoladAsDateTimeOffset();

        // Duration for half of the lunar month:
        // 14 days, 18 hours, 22 minutes, 1 second, 666 milliseconds
        TimeSpan? halfLunarMonth = JavaTime.OfDays(14)
                + JavaTime.OfHours(18)
                + JavaTime.OfMinutes(22)
                + JavaTime.OfSeconds(1)
                + JavaTime.DurationOfMillis(666);

        return molad.Plus(halfLunarMonth);
    }

    /// <summary>
    /// Returns the latest time of <em>Kiddush Levana</em> calculated as 15 days after the <em>molad.</em> This is the
    /// opinion brought down in the Shulchan Aruch (Orach Chaim 426). It should be noted that some opinions hold that
    /// the <a href="http://en.wikipedia.org/wiki/Moses_Isserles">Rema</a> who brings down the <a
    /// href="http://en.wikipedia.org/wiki/Yaakov_ben_Moshe_Levi_Moelin">Maharil's</a> opinion of calculating it as
    /// <see cref="GetSofZmanKidushLevanaBetweenMoldos">half way between <em>molad</em> and <em>molad</em></see> is of the
    /// opinion of the Mechaber as well. Also see the Aruch Hashulchan. For additional details on the subject, See Rabbi
    /// Dovid Heber's very detailed writeup in Siman Daled (chapter 4) of <a
    /// href="http://www.worldcat.org/oclc/461326125">Shaarei Zmanim</a>. This method returns the time even if it is during
    /// the day when <em>Kiddush Levana</em> can't be said. Callers of this method should consider displaying <em>alos</em>
    /// before this time if the <em>zman</em> is between <em>alos</em> and <em>tzais</em>.
    /// </summary>
    /// <returns>the <see cref="DateTimeOffset"/> representing the moment 15 days after the <em>molad</em>.</returns>
    /// <seealso cref="GetSofZmanKidushLevanaBetweenMoldos"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSofZmanKidushLevana15Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    /// <seealso cref="ComprehensiveZmanimCalendar.GetSofZmanKidushLevana15Days(Nullable{DateTimeOffset}, Nullable{DateTimeOffset})"/>
    public virtual DateTimeOffset? GetSofZmanKidushLevana15Days()
    {
        return GetMoladAsDateTimeOffset().Plus(JavaTime.OfHours(24 * 15));
    }

    /// <summary>
    /// Returns the <em>Daf Yomi (Bavli)</em> for the date that the calendar is set to. See the
    /// <see cref="HebrewDateFormatter.FormatDafYomiBavli"/> for the ability to format the <em>daf</em> in
    /// Hebrew or transliterated <em>masechta</em> names.
    /// </summary>
    /// <returns>the daf as a <see cref="Daf"/></returns>
    public virtual Daf GetDafYomiBavli()
    {
        return YomiCalculator.GetDafYomiBavli(this);
    }
    /// <summary>
    /// Returns the <em>Daf Yomi (Yerushalmi)</em> for the date that the calendar is set to. See the
    /// <see cref="HebrewDateFormatter.FormatDafYomiYerushalmi"/> for the ability to format the <em>daf</em>
    /// in Hebrew or transliterated <em>masechta</em> names.
    /// </summary>
    /// <returns>the daf as a <see cref="Daf"/></returns>
    public virtual Daf? GetDafYomiYerushalmi()
    {
        return YerushalmiYomiCalculator.GetDafYomiYerushalmi(this);
    }

    /// <summary>
    /// Returns the elapsed days since <em>Tekufas Tishrei</em>. This uses <em>Tekufas Shmuel</em> (identical to the <a href=
    /// "https://en.wikipedia.org/wiki/Julian_year_(astronomy)">Julian Year</a> with a solar year length of 365.25 days).
    /// The notation used below is D = days, H = hours and C = chalakim. <em><a href="https://en.wikipedia.org/wiki/Molad"
    /// >Molad</a> BaHaRad</em> was 2D,5H,204C or 5H,204C from the start of <em>Rosh Hashana</em> year 1. For <em>molad
    /// Nissan</em> add 177D, 4H and 438C (6 * 29D, 12H and 793C), or 177D,9H,642C after <em>Rosh Hashana</em> year 1.
    /// <em>Tekufas Nissan</em> was 7D, 9H and 642C before <em>molad Nissan</em> according to the Rambam, or 170D, 0H and
    /// 0C after <em>Rosh Hashana</em> year 1. <em>Tekufas Tishrei</em> was 182D and 3H (365.25 / 2) before <em>tekufas
    /// Nissan</em>, or 12D and 15H before <em>Rosh Hashana</em> of year 1. Outside of Israel we start reciting <em>Tal
    /// Umatar</em> in <em>Birkas Hashanim</em> from 60 days after <em>tekufas Tishrei</em>. The 60 days include the day of
    /// the <em>tekufah</em> and the day we start reciting <em>Tal Umatar</em>. 60 days from the tekufah == 47D and 9H
    /// from <em>Rosh Hashana</em> year 1.
    /// </summary>
    /// <returns>the number of elapsed days since <em>tekufas Tishrei</em>.</returns>
    /// <seealso cref="TefilaRules.IsVeseinTalUmatarStartDate"/>
    /// <seealso cref="TefilaRules.IsVeseinTalUmatarStartingTonight"/>
    /// <seealso cref="TefilaRules.IsYaalehVeyavoRecited"/>
    /// <seealso cref="GetTekufa"/>
    /// <seealso cref="GetTekufaAsDateTimeOffset"/>
    public virtual int GetTekufasTishreiElapsedDays()
    {
        // Days since Rosh Hashana year 1. Add 1/2 day as the first tekufas tishrei was 9 hours into the day. This allows all
        // 4 years of the secular leap year cycle to share 47 days. Truncate 47D and 9H to 47D for simplicity.
        double days = GetJewishCalendarElapsedDays(JewishYear) + (GetDaysSinceStartOfJewishYear() - 1) + 0.5;
        // days of completed solar years
        double solar = (JewishYear - 1) * 365.25;
        return (int)Math.Floor(days - solar);
    }

    /// <summary>
    /// Calculates the hours for when the <a href="https://en.wikipedia.org/wiki/Tekufah"><em>Tekufa</em></a> (season) changes.
    /// There are 4 <em>tekufos</em> a year: Nissan/Spring, Tammuz/Summer, Tishri/Fall, and Teves/Winter. This calculation is
    /// according to <a href="https://en.wikipedia.org/wiki/Samuel_of_Nehardea">Shmuel</a> in <a href=
    /// "https://hebrewbooks.org/shas.aspx?mesechta=3&amp;daf=56">Eruvin 56a</a>, which is a more rounded up version of <a href=
    /// "https://en.wikipedia.org/wiki/Adda_bar_Ahavah">Rav Adda's</a> calculation. This calculation follows the Levush in the end of Siman 428,
    /// who writes that the hours to calculate the tekufa are based on regular clock hours and not proportional hours, and that the time
    /// for the tekufa are based on 6 hours after the previous day's Midday. The <a href=
    /// "https://en.wikipedia.org/wiki/Moses_Isserles">Rama</a> writes in <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=67715&amp;st=&amp;pgnum=218">Yoreh De'ah 116:5</a> that one should not drink water during
    /// the <em>tekufa</em> change. <a href="https://en.wikipedia.org/wiki/Ovadia_Yosef">Rabbi Ovadia Yosef</a> (Halichot Olam,
    /// Chelek 7, Page 183) recommends to abstain from drinking water for a range of 1 hour (30 minutes before and after the
    /// <em>tekufa</em> event).
    /// </summary>
    /// <returns>the number of hours into the Hebrew day that the <em>tekufa</em> (season) change takes place as a {code Double}, or
    ///         <c>null</c> if the <em>tekufa</em> does not occur on current day. For example: 19.5 would mean that the
    ///         <em>tekufa</em> occurs 19 hours and a half into the Hebrew date, or 13 and a half hours (-6 hours) into the Gregorian
    ///         date.</returns>
    private double? GetTekufa()
    {
        double InitialTekufaOffset = 12.625;  // the number of days Tekufas Tishrei occurs before JEWISH_EPOCH

        double days = GetJewishCalendarElapsedDays(JewishYear) + GetDaysSinceStartOfJewishYear() + InitialTekufaOffset - 1;  // total days since first Tekufas Tishrei event

        double solarDaysElapsed = days % 365.25;  // total days elapsed since the start of solar year
        double tekufaDaysElapsed = solarDaysElapsed % 91.3125;  // the number of days that have passed since a tekufa event
        if (tekufaDaysElapsed > 0 && tekufaDaysElapsed <= 1)
        {  // if the tekufa happens in the upcoming 24 hours
            return ((1.0 - tekufaDaysElapsed) * 24.0) % 24;// rationalize the tekufa event to number of hours since start of jewish day
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns an <see cref="DateTimeOffset"/> if the current day has a <a href="https://en.wikipedia.org/wiki/Tekufah"><em>Tekufa</em></a>
    /// (season) change. The <see cref="DateTimeOffset"/> will contain the time that the <em>tekufa</em> (season) is arriving. If this method is
    /// called on a day without a <em>tekufa</em> change, it will return a <c>null</c>. The default implementation of this method
    /// will return the <em>tekufa</em> change according to the calculations of the <a href="https://zmanim.online/">Luach Itim
    /// Lebinah</a> following <a href="https://en.wikipedia.org/wiki/Yechiel_Michel_Tucazinsky">Rabbi Yechiel Michel Tucazinsky</a>.
    /// However, there is also the opinion of <a href=
    /// "https://he.wikipedia.org/wiki/%D7%99%D7%95%D7%A0%D7%94_%D7%9E%D7%A8%D7%A6%D7%91%D7%9A">Rabbi Yonah Merzbach</a>, who
    /// calculates the <em>tekufa</em> based on Local Mean Time (LMT) in Israel which causes a ~21-minute difference. It should be
    /// noted that this method only covers two opinions of how to calculate the tekufa (based on 6 hours after Midday of the day
    /// before the tekufa). The <a href="https://en.wikipedia.org/wiki/Asher_ben_Jehiel">Rosh</a> (Tosafos HaRosh <a href=
    /// "https://www.sefaria.org/Tosafot_HaRosh_on_Berakhot.3b.6?lang=he">Berachos 3b</a>) is of the opinion that the calculation
    /// starts at sunset and to use proportional hours. While the <a href="https://en.wikipedia.org/wiki/Simeon_ben_Zemah_Duran"
    /// >Tashbetz</a> (Shut Tashbetz <a href="https://hebrewbooks.org/pdfpager.aspx?req=1381&amp;st=&amp;pgnum=91">Chelek 1, Siman 109</a>)
    /// is also of the opinion that the calculation starts at sunset, however, his opinion is to use regular hours.
    /// </summary>
    /// <param name="useLocalMeanTime">if true, removes ~21 minutes from the time of the <em>tekufa</em> calculated by Rabbi Yechiel Michel
    ///         Tucazinsky, and will follow the opinion of Rabbi Yonah Merzbach.</param>
    /// <returns>a <see cref="DateTimeOffset"/> with the time that the <em>tekufa</em> (season) changes or a <c>null</c> on a day with no <em>tekufa</em>
    ///         change.</returns>
    /// <seealso cref="GetTekufa"/>
    public virtual DateTimeOffset? GetTekufaAsDateTimeOffset(bool useLocalMeanTime)
    {
        double? tekufa = GetTekufa();
        if (tekufa is not double hours)
        {
            return null;
        }
        // The tekufa Date must be generated using standard time. Using "Asia/Jerusalem" timezone will result in the time being
        // incorrectly off by an hour in the summer due to DST. Proper adjustment for the actual time in DST will be done by the
        // formatter class used to display the Instant.
        TimeSpan yerushalayimStandardTZ = TimeSpan.FromHours(2);

        hours -= 6; // minus 6 hours because the hebrew date starts at sunset which is at 6PM

        DateOnly tekufaDate = LocalDate;
        if (hours < 0)
        {
            hours += 24;
            tekufaDate = tekufaDate.AddDays(-1);
        }

        int minutes = (int)((hours - (int)hours) * 60);

        TimeOnly time = new TimeOnly((int)hours, minutes);
        DateTimeOffset tekufaZDT = new DateTimeOffset(tekufaDate.ToDateTime(time, DateTimeKind.Unspecified), yerushalayimStandardTZ);

        // Har Habayis at a longitude of 35.2354 offset vs longitude 35 in standard time, so we subtract the time difference
        // of 20.94 minutes (20 minutes and 56 seconds and 496 millis) to get to Standard time from local mean time
        if (useLocalMeanTime)
        {
            TimeSpan jerusalemStandardTimeOffset = JavaTime.OfMinutes(20) + JavaTime.OfSeconds(56) + JavaTime.DurationOfMillis(496);
            return (tekufaZDT - jerusalemStandardTimeOffset).ToUniversalTime();
        }
        else
        {
            return tekufaZDT.ToUniversalTime();
        }
    }

    /// <summary>
    /// Returns true if the current day is <em>Isru Chag</em>. The method returns true for the day following <em>Pesach</em>
    /// <em>Shavuos</em> and <em>Succos</em>. It utilizes <see cref="InIsrael"/> to return the proper date.
    /// </summary>
    /// <returns>true if the current day is <em>Isru Chag</em>. The method returns true for the day following <em>Pesach</em>
    /// <em>Shavuos</em> and <em>Succos</em>. It utilizes <see cref="InIsrael"/> to return the proper date.</returns>
    public virtual bool IsIsruChag()
    {
        int holidayIndex = GetYomTovIndex();
        return holidayIndex == IsruChag;
    }

    /// <summary>The KosherJava name of <see cref="GetMoladAsDateTimeOffset"/>.</summary>
    /// <returns>the result of <see cref="GetMoladAsDateTimeOffset"/>.</returns>
    [Obsolete("Use GetMoladAsDateTimeOffset(); GetMoladAsInstant is the KosherJava name. This alias will be removed in a future version.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public DateTimeOffset? GetMoladAsInstant() => GetMoladAsDateTimeOffset();

    /// <summary>The KosherJava name of <see cref="GetTekufaAsDateTimeOffset"/>.</summary>
    /// <param name="useLocalMeanTime">see <see cref="GetTekufaAsDateTimeOffset"/>.</param>
    /// <returns>the result of <see cref="GetTekufaAsDateTimeOffset"/>.</returns>
    [Obsolete("Use GetTekufaAsDateTimeOffset(bool); GetTekufaAsInstant is the KosherJava name. This alias will be removed in a future version.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public DateTimeOffset? GetTekufaAsInstant(bool useLocalMeanTime) => GetTekufaAsDateTimeOffset(useLocalMeanTime);

    /// <summary>
    /// Creates a copy of this <see cref="JewishCalendar"/>, including its <see cref="InIsrael"/>, <see cref="IsMukafChoma"/> and
    /// <see cref="UseModernHolidays"/> settings.
    /// </summary>
    /// <returns>a copy of this calendar.</returns>
    public override JewishCalendar Clone()
    {
        return (JewishCalendar)base.Clone();
    }

    /// <summary>
    /// Indicates whether some other object is "equal to" this one.
    /// </summary>
    /// <seealso cref="object.Equals(object)"/>
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
        JewishCalendar jewishCalendar = (JewishCalendar)obj;
        return AbsDate == jewishCalendar.AbsDate && InIsrael == jewishCalendar.InIsrael;
    }

    /// <summary>
    /// Overrides <see cref="object.GetHashCode"/>.
    /// </summary>
    /// <seealso cref="object.GetHashCode"/>
    public override int GetHashCode()
    {
        int result = AbsDate.GetHashCode();
        result = 31 * result + InIsrael.GetHashCode();
        return result;
    }
}
