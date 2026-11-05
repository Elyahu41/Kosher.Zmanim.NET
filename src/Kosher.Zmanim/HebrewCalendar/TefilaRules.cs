/*
 * Zmanim Java API
 * Copyright © 2019 - 2026 Eliyahu Hershfeld
 * Copyright © 2019 - 2021 Y Paritcher
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
/// Tefila Rules is a utility class that covers the various <em>halachos</em> and <em>minhagim</em> regarding
/// changes to daily <em>tefila</em> / prayers, based on the Jewish calendar. This is mostly useful for use in
/// developing <em>siddur</em> type applications, but it is also valuable for <em>shul</em> calendars that set
/// <em>tefila</em> times based on if <a href="https://en.wikipedia.org/wiki/Tachanun"><em>tachanun</em></a> is
/// recited that day. There are many settings in this class to cover the vast majority of <em>minhagim</em>, but
/// there are likely some not covered here. The source for many of the <em>chasidishe minhagim</em> can be found
/// in the <a href="https://www.nli.org.il/he/books/NNL_ALEPH001141272/NLI">Minhag Yisrael Torah</a> on Orach
/// Chaim 131.
/// Dates used in specific communities such as specific <em>yahrzeits</em> or a holidays like Purim Mezhbizh
/// (Medzhybizh) celebrated on 11 <see cref="JewishDate.Teves"><em>Teves</em></see> or <a href=
/// "https://en.wikipedia.org/wiki/Second_Purim#Purim_Saragossa_(18_Shevat)">Purim Saragossa</a> celebrated on
/// the (17th or) 18th of <see cref="JewishDate.Shevat"><em>Shevat</em></see> are not (and likely will not be) supported by
/// this class.
/// <para>Sample code:
/// <code>
/// var tr = new TefilaRules();
/// var jewishCalendar = new JewishCalendar();
/// var hdf = new HebrewDateFormatter();
/// jewishCalendar.SetJewishDate(5783, JewishDate.Tishrei, 1); // Rosh Hashana
/// Console.WriteLine(hdf.Format(jewishCalendar) + ": " + tr.IsTachanunRecitedShacharis(jewishCalendar));
/// jewishCalendar.SetJewishDate(5783, JewishDate.Adar, 17);
/// Console.WriteLine(hdf.Format(jewishCalendar) + ": " + tr.IsTachanunRecitedShacharis(jewishCalendar));
/// tr.IsTachanunRecitedWeekOfPurim = false;
/// Console.WriteLine(hdf.Format(jewishCalendar) + ": " + tr.IsTachanunRecitedShacharis(jewishCalendar));</code></para>
/// </summary>
/// <remarks>
/// <b>To do:</b> The following items may be added at a future date.
/// <ol>
/// <li><em>Lamnatzaiach</em></li>
/// <li><em>Selichos</em></li>
/// <li>...</li>
/// </ol>
/// </remarks>
public class TefilaRules
{
    /// <summary>
    /// The default value is <c>true</c>.
    /// </summary>
    /// <seealso cref="IsTachanunRecitedEndOfTishrei"/>
    /// <seealso cref="IsTachanunRecitedEndOfTishrei"/>
    private bool tachanunRecitedEndOfTishrei = true;

    /// <summary>
    /// The default value is <c>false</c>.
    /// </summary>
    /// <seealso cref="IsTachanunRecitedWeekAfterShavuos"/>
    /// <seealso cref="IsTachanunRecitedWeekAfterShavuos"/>
    private bool tachanunRecitedWeekAfterShavuos = false;

    /// <summary>
    /// The default value is <c>true</c>.
    /// </summary>
    /// <seealso cref="IsTachanunRecited13SivanOutOfIsrael"/>
    /// <seealso cref="IsTachanunRecited13SivanOutOfIsrael"/>
    private bool tachanunRecited13SivanOutOfIsrael = true;

    /// <summary>
    /// The default value is <c>false</c>.
    /// </summary>
    /// <seealso cref="IsTachanunRecitedPesachSheni"/>
    /// <seealso cref="IsTachanunRecitedPesachSheni"/>
    private bool tachanunRecitedPesachSheni = false;

    /// <summary>
    /// The default value is <c>true</c>.
    /// </summary>
    /// <seealso cref="IsTachanunRecited15IyarOutOfIsrael"/>
    /// <seealso cref="IsTachanunRecited15IyarOutOfIsrael"/>
    private bool tachanunRecited15IyarOutOfIsrael = true;

    /// <summary>
    /// The default value is <c>false</c>.
    /// </summary>
    /// <seealso cref="IsTachanunRecitedMinchaErevLagBaomer"/>
    /// <seealso cref="IsTachanunRecitedMinchaErevLagBaomer"/>
    private bool tachanunRecitedMinchaErevLagBaomer = false;

    /// <summary>
    /// The default value is <c>true</c>.
    /// </summary>
    /// <seealso cref="IsTachanunRecitedShivasYemeiHamiluim"/>
    /// <seealso cref="IsTachanunRecitedShivasYemeiHamiluim"/>
    private bool tachanunRecitedShivasYemeiHamiluim = true;

    /// <summary>
    /// The default value is <c>true</c>.
    /// </summary>
    /// <seealso cref="IsTachanunRecitedWeekOfHod"/>
    /// <seealso cref="IsTachanunRecitedWeekOfHod"/>
    private bool tachanunRecitedWeekOfHod = true;

    /// <summary>
    /// The default value is <c>true</c>.
    /// </summary>
    /// <seealso cref="IsTachanunRecitedWeekOfPurim"/>
    /// <seealso cref="IsTachanunRecitedWeekOfPurim"/>
    private bool tachanunRecitedWeekOfPurim = true;

    /// <summary>
    /// The default value is <c>true</c>.
    /// </summary>
    /// <seealso cref="IsTachanunRecitedFridays"/>
    /// <seealso cref="IsTachanunRecitedFridays"/>
    private bool tachanunRecitedFridays = true;

    /// <summary>
    /// The default value is <c>true</c>.
    /// </summary>
    /// <seealso cref="IsTachanunRecitedSundays"/>
    /// <seealso cref="IsTachanunRecitedSundays"/>
    private bool tachanunRecitedSundays = true;

    /// <summary>
    /// The default value is <c>true</c>.
    /// </summary>
    /// <seealso cref="IsTachanunRecitedMinchaAllYear"/>
    /// <seealso cref="IsTachanunRecitedMinchaAllYear"/>
    private bool tachanunRecitedMinchaAllYear = true;

    /// <summary>
    /// The default value is <c>false</c>.
    /// </summary>
    /// <seealso cref="IsMizmorLesodaRecited"/>
    /// <seealso cref="IsMizmorLesodaRecitedErevYomKippurAndPesach"/>
    private bool mizmorLesodaRecitedErevYomKippurAndPesach = false;

    /// <summary>
    /// Default constructor of the TefilaRules. It currently does not have any parameters.
    /// </summary>
    public TefilaRules()
    {
        // nothing here
    }

    /// <summary>
    /// Returns if <em>tachanun</em> is recited during <em>shacharis</em> on the day in question. There are the many
    /// <em>minhagim</em> based settings that are available in this class that control what days are set for
    /// <em>tachanun</em> recital.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>if <em>tachanun</em> is recited during <em>shacharis</em>.</returns>
    /// <seealso cref="IsTachanunRecitedMincha"/>
    /// <seealso cref="IsTachanunRecitedSundays"/>
    /// <seealso cref="IsTachanunRecitedFridays"/>
    /// <seealso cref="IsTachanunRecitedEndOfTishrei"/>
    /// <seealso cref="IsTachanunRecitedWeekAfterShavuos"/>
    /// <seealso cref="IsTachanunRecited13SivanOutOfIsrael"/>
    /// <seealso cref="IsTachanunRecitedPesachSheni"/>
    /// <seealso cref="IsTachanunRecited15IyarOutOfIsrael"/>
    /// <seealso cref="IsTachanunRecitedShivasYemeiHamiluim"/>
    /// <seealso cref="IsTachanunRecitedWeekOfPurim"/>
    /// <seealso cref="IsTachanunRecitedWeekOfHod"/>
    public bool IsTachanunRecitedShacharis(JewishCalendar jewishCalendar)
    {
        int holidayIndex = jewishCalendar.GetYomTovIndex();
        int day = jewishCalendar.JewishDayOfMonth;
        int month = jewishCalendar.JewishMonth;

        return jewishCalendar.DayOfWeek != Weekday.Saturday
                && (tachanunRecitedSundays || jewishCalendar.DayOfWeek != Weekday.Sunday)
                && (tachanunRecitedFridays || jewishCalendar.DayOfWeek != Weekday.Friday)
                && month != JewishDate.Nissan
                && (month != JewishDate.Tishrei || ((tachanunRecitedEndOfTishrei || day <= 8)
                        && (!tachanunRecitedEndOfTishrei || (day <= 8 || day >= 22))))
                && (month != JewishDate.Sivan
                        || ((!tachanunRecitedWeekAfterShavuos || day >= 7) && (tachanunRecitedWeekAfterShavuos
                                || day >= (!jewishCalendar.InIsrael && !tachanunRecited13SivanOutOfIsrael ? 14
                                        : 13))))
                && !jewishCalendar.IsErevYomTov() && (!jewishCalendar.IsYomTov() || (jewishCalendar.IsTaanis()
                        && (tachanunRecitedPesachSheni || holidayIndex != JewishCalendar.PesachSheni)))
                && (jewishCalendar.InIsrael || tachanunRecitedPesachSheni || tachanunRecited15IyarOutOfIsrael
                        || jewishCalendar.JewishMonth != JewishDate.Iyar || day != 15)
                && holidayIndex != JewishCalendar.TishaBeav && !jewishCalendar.IsIsruChag()
                && !jewishCalendar.IsRoshChodesh()
                && (tachanunRecitedShivasYemeiHamiluim
                        || ((jewishCalendar.IsJewishLeapYear() || month != JewishDate.Adar)
                                && (!jewishCalendar.IsJewishLeapYear() || month != JewishDate.AdarII))
                        || day <= 22)
                && (tachanunRecitedWeekOfPurim
                        || ((jewishCalendar.IsJewishLeapYear() || month != JewishDate.Adar)
                                && (!jewishCalendar.IsJewishLeapYear() || month != JewishDate.AdarII))
                        || day <= 10 || day >= 18)
                && (!jewishCalendar.UseModernHolidays || (holidayIndex != JewishCalendar.YomHaatzmaut
                        && holidayIndex != JewishCalendar.YomYerushalayim))
                && (tachanunRecitedWeekOfHod || month != JewishDate.Iyar || day <= 13 || day >= 21);
    }

    /// <summary>
    /// Returns if <em>tachanun</em> is recited during <em>mincha</em> on the day in question.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>if <em>tachanun</em> is recited during <em>mincha</em>.</returns>
    /// <seealso cref="IsTachanunRecitedShacharis"/>
    public bool IsTachanunRecitedMincha(JewishCalendar jewishCalendar)
    {
        JewishCalendar tomorrow = (JewishCalendar)jewishCalendar.Clone();
        tomorrow.PlusDays(1);

        return tachanunRecitedMinchaAllYear && jewishCalendar.DayOfWeek != Weekday.Friday
                && IsTachanunRecitedShacharis(jewishCalendar)
                && (IsTachanunRecitedShacharis(tomorrow)
                        || tomorrow.GetYomTovIndex() == JewishCalendar.ErevRoshHashana
                        || tomorrow.GetYomTovIndex() == JewishCalendar.ErevYomKippur
                        || tomorrow.GetYomTovIndex() == JewishCalendar.PesachSheni)
                && (tachanunRecitedMinchaErevLagBaomer || tomorrow.GetYomTovIndex() != JewishCalendar.LagBaomer);
    }

    /// <summary>
    /// Returns if it is the Jewish day (starting the evening before) to start reciting <em>Vesein Tal Umatar Livracha</em>
    /// (<em>Sheailas Geshamim</em>). In Israel this is the 7th day of <see cref="JewishDate.Cheshvan"><em>Marcheshvan</em></see>.
    /// Outside Israel recitation starts on the evening of December 4th (or 5th if it is the year before a civil leap year)
    /// in the 21st century and shifts a day forward every century not evenly divisible by 400. This method will return true
    /// if <em>vesein tal umatar</em> on the current Jewish date that starts on the previous night, so Dec 5/6 will be
    /// returned by this method in the 21st century. <em>vesein tal umatar</em> is not recited on <em>Shabbos</em> and the
    /// start date will be delayed a day when the start day is on a <em>Shabbos</em> (this can only occur out of Israel).
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>true if it is the first Jewish day (starting the prior evening of reciting <em>Vesein Tal Umatar Livracha</em>
    ///         (<em>Sheailas Geshamim</em>).</returns>
    /// <seealso cref="IsVeseinTalUmatarStartingTonight"/>
    /// <seealso cref="IsVeseinTalUmatarRecited"/>
    public bool IsVeseinTalUmatarStartDate(JewishCalendar jewishCalendar)
    {
        if (jewishCalendar.InIsrael)
        {
            // The 7th Cheshvan can't occur on Shabbos, so always return true for 7 Cheshvan
            return jewishCalendar.JewishMonth == JewishDate.Cheshvan && jewishCalendar.JewishDayOfMonth == 7;
        }
        else
        {
            if (jewishCalendar.DayOfWeek == Weekday.Saturday)
            { //Not recited on Friday night
                return false;
            }
            if (jewishCalendar.DayOfWeek == Weekday.Sunday)
            { // When starting on Sunday, it can be the start date or delayed from Shabbos
                return jewishCalendar.GetTekufasTishreiElapsedDays() == 48 || jewishCalendar.GetTekufasTishreiElapsedDays() == 47;
            }
            else
            {
                return jewishCalendar.GetTekufasTishreiElapsedDays() == 47;
            }
        }
    }

    /// <summary>
    /// Returns true if tonight is the first night to start reciting <em>Vesein Tal Umatar Livracha</em> (
    /// <em>Sheailas Geshamim</em>). In Israel this is the 7th day of <see cref="JewishDate.Cheshvan"><em>Marcheshvan</em></see> (so the 6th will return true). Outside Israel recitation starts on the evening
    /// of December 4th (or 5th if it is the year before a civil leap year) in the 21st century and shifts a
    /// day forward every century not evenly divisible by 400. <em>Vesein tal umatar</em> is not recited on
    /// <em>Shabbos</em> and the start date will be delayed a day when the start day is on a <em>Shabbos</em>
    /// (this can only occur out of Israel).
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>true if it is the first Jewish day (starting the prior evening of reciting <em>Vesein Tal Umatar
    ///         Livracha</em> (<em>Sheailas Geshamim</em>).</returns>
    /// <seealso cref="IsVeseinTalUmatarStartDate"/>
    /// <seealso cref="IsVeseinTalUmatarRecited"/>
    public bool IsVeseinTalUmatarStartingTonight(JewishCalendar jewishCalendar)
    {
        if (jewishCalendar.InIsrael)
        {
            // The 7th Cheshvan can't occur on Shabbos, so always return true for 6 Cheshvan
            return jewishCalendar.JewishMonth == JewishDate.Cheshvan && jewishCalendar.JewishDayOfMonth == 6;
        }
        else
        {
            if (jewishCalendar.DayOfWeek == Weekday.Friday)
            { //Not recited on Friday night
                return false;
            }
            if (jewishCalendar.DayOfWeek == Weekday.Saturday)
            { // When starting on motzai Shabbos, it can be the start date or delayed from Friday night
                return jewishCalendar.GetTekufasTishreiElapsedDays() == 47 || jewishCalendar.GetTekufasTishreiElapsedDays() == 46;
            }
            else
            {
                return jewishCalendar.GetTekufasTishreiElapsedDays() == 46;
            }
        }
    }

    /// <summary>
    /// Returns if <em>Vesein Tal Umatar Livracha</em> (<em>Sheailas Geshamim</em>) is recited. This will return
    /// true for the entire season, even on <em>Shabbos</em> when it is not recited.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>true if <em>Vesein Tal Umatar Livracha</em> (<em>Sheailas Geshamim</em>) is recited.</returns>
    /// <seealso cref="IsVeseinTalUmatarStartDate"/>
    /// <seealso cref="IsVeseinTalUmatarStartingTonight"/>
    public bool IsVeseinTalUmatarRecited(JewishCalendar jewishCalendar)
    {
        int month = jewishCalendar.JewishMonth;
        int day = jewishCalendar.JewishDayOfMonth;

        if (month == JewishDate.Nissan && day >= 15)
        { // Not recited from 15 Nissan (Pesach) through end of summer months
            return false;
        }
        if (month > JewishDate.Nissan && month < JewishDate.Cheshvan)
        {
            return false;
        }
        // Recited from 7 Cheshvan (Israel) or tekufas Tishrei day 47 (outside Israel) through 14 Nissan
        if (jewishCalendar.InIsrael)
        {
            return month != JewishDate.Cheshvan || day >= 7;
        }
        else
        {
            return jewishCalendar.GetTekufasTishreiElapsedDays() >= 47;
        }
    }

    /// <summary>
    /// Returns if <em>Vesein Beracha</em> is recited. It is recited from 15 <see cref="JewishDate.Nissan"><em>Nissan</em></see> to the
    /// point that <see cref="IsVeseinTalUmatarRecited"><em>vesein tal umatar</em> is recited</see>.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>true if <em>Vesein Beracha</em> is recited.</returns>
    /// <seealso cref="IsVeseinTalUmatarRecited"/>
    public bool IsVeseinBerachaRecited(JewishCalendar jewishCalendar)
    {
        return !IsVeseinTalUmatarRecited(jewishCalendar);
    }

    /// <summary>
    /// Returns if the date is the start date for reciting <em>Mashiv Haruach Umorid Hageshem</em>. The date is 22
    /// <see cref="JewishDate.Tishrei"><em>Tishrei</em></see>.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>true if the date is the start date for reciting <em>Mashiv Haruach Umorid Hageshem</em>.</returns>
    /// <seealso cref="IsMashivHaruachEndDate"/>
    /// <seealso cref="IsMashivHaruachRecited"/>
    public bool IsMashivHaruachStartDate(JewishCalendar jewishCalendar)
    {
        return jewishCalendar.JewishMonth == JewishDate.Tishrei && jewishCalendar.JewishDayOfMonth == 22;
    }

    /// <summary>
    /// Returns if the date is the end date for reciting <em>Mashiv Haruach Umorid Hageshem</em>. The date is 15
    /// <see cref="JewishDate.Nissan"><em>Nissan</em></see>.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>true if the date is the end date for reciting <em>Mashiv Haruach Umorid Hageshem</em>.</returns>
    /// <seealso cref="IsMashivHaruachStartDate"/>
    /// <seealso cref="IsMashivHaruachRecited"/>
    public bool IsMashivHaruachEndDate(JewishCalendar jewishCalendar)
    {
        return jewishCalendar.JewishMonth == JewishDate.Nissan && jewishCalendar.JewishDayOfMonth == 15;
    }

    /// <summary>
    /// Returns if <em>Mashiv Haruach Umorid Hageshem</em> is recited. This period starts on 22 <see cref="JewishDate.Tishrei"><em>Tishrei</em></see> and ends on the 15th day of <see cref="JewishDate.Nissan"><em>Nissan</em></see>.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>true if <em>Mashiv Haruach Umorid Hageshem</em> is recited.</returns>
    /// <seealso cref="IsMashivHaruachStartDate"/>
    /// <seealso cref="IsMashivHaruachEndDate"/>
    public bool IsMashivHaruachRecited(JewishCalendar jewishCalendar)
    {
        JewishDate startDate = new JewishDate(jewishCalendar.JewishYear, JewishDate.Tishrei, 22);
        JewishDate endDate = new JewishDate(jewishCalendar.JewishYear, JewishDate.Nissan, 15);
        return jewishCalendar.CompareTo(startDate) > 0 && jewishCalendar.CompareTo(endDate) < 0;
    }

    /// <summary>
    /// Returns if <em>Morid Hatal</em> (or the lack of reciting <em>Mashiv Haruach</em> following <em>nussach Ashkenaz</em>) is
    /// recited. This period starts on the 15th day of <see cref="JewishDate.Nissan"><em>Nissan</em></see> and ends on 22 <see cref="JewishDate.Tishrei"><em>Tishrei</em></see>.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>true if <em>Morid Hatal</em> (or the lack of reciting <em>Mashiv Haruach</em> following <em>nussach Ashkenaz</em>) is recited.</returns>
    public bool IsMoridHatalRecited(JewishCalendar jewishCalendar)
    {
        return !IsMashivHaruachRecited(jewishCalendar) || IsMashivHaruachStartDate(jewishCalendar) || IsMashivHaruachEndDate(jewishCalendar);
    }

    /// <summary>
    /// Returns if <em>Hallel</em> is recited on the day in question. This will return true for both <em>Hallel shalem</em>
    /// and <em>Chatzi Hallel</em>. See <see cref="IsHallelShalemRecited"/> to know if the complete <em>Hallel</em>
    /// is recited.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>if <em>Hallel</em> is recited.</returns>
    /// <seealso cref="IsHallelShalemRecited"/>
    public bool IsHallelRecited(JewishCalendar jewishCalendar)
    {
        int day = jewishCalendar.JewishDayOfMonth;
        int month = jewishCalendar.JewishMonth;
        int holidayIndex = jewishCalendar.GetYomTovIndex();
        bool inIsrael = jewishCalendar.InIsrael;

        if (jewishCalendar.IsRoshChodesh())
        { //RH returns false for RC
            return true;
        }
        if (jewishCalendar.IsChanukah())
        {
            return true;
        }
        switch (month)
        {
            case JewishDate.Nissan:
                if (day >= 15 && ((inIsrael && day <= 21) || (!inIsrael && day <= 22)))
                {
                    return true;
                }
                break;
            case JewishDate.Iyar: // modern holidays
                if (jewishCalendar.UseModernHolidays && (holidayIndex == JewishCalendar.YomHaatzmaut
                        || holidayIndex == JewishCalendar.YomYerushalayim))
                {
                    return true;
                }
                break;
            case JewishDate.Sivan:
                if (day == 6 || (!inIsrael && (day == 7)))
                {
                    return true;
                }
                break;
            case JewishDate.Tishrei:
                if (day >= 15 && (day <= 22 || (!inIsrael && (day <= 23))))
                {
                    return true;
                }
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns if <em>hallel shalem</em> is recited on the day in question. This will always return false if <see cref="IsHallelRecited"/> returns false.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>if <em>hallel shalem</em> is recited.</returns>
    /// <seealso cref="IsHallelRecited"/>
    public bool IsHallelShalemRecited(JewishCalendar jewishCalendar)
    {
        int day = jewishCalendar.JewishDayOfMonth;
        int month = jewishCalendar.JewishMonth;
        bool inIsrael = jewishCalendar.InIsrael;
        if (IsHallelRecited(jewishCalendar))
        {
            return (!jewishCalendar.IsRoshChodesh() || jewishCalendar.IsChanukah())
                    && (month != JewishDate.Nissan || ((!inIsrael || day <= 15) && (inIsrael || day <= 16)));
        }
        return false;
    }

    /// <summary>
    /// Returns if <a href="https://en.wikipedia.org/wiki/Al_HaNissim"><em>Al HaNissim</em></a> is recited on the day in question.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>if <em>al hanissim</em> is recited.</returns>
    /// <seealso cref="JewishCalendar.IsChanukah"/>
    /// <seealso cref="JewishCalendar.IsPurim"/>
    /// <seealso cref="JewishCalendar.IsMukafChoma"/>
    public bool IsAlHanissimRecited(JewishCalendar jewishCalendar)
    {
        return jewishCalendar.IsPurim() || jewishCalendar.IsChanukah();
    }

    /// <summary>
    /// Returns if <em>Yaaleh Veyavo</em> is recited on the day in question.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>if <em>Yaaleh Veyavo</em> is recited.</returns>
    /// <seealso cref="JewishCalendar.IsPesach"/>
    /// <seealso cref="JewishCalendar.IsShavuos"/>
    /// <seealso cref="JewishCalendar.IsRoshHashana"/>
    /// <seealso cref="JewishCalendar.IsYomKippur"/>
    /// <seealso cref="JewishCalendar.IsSuccos"/>
    /// <seealso cref="JewishCalendar.IsShminiAtzeres"/>
    /// <seealso cref="JewishCalendar.IsSimchasTorah"/>
    /// <seealso cref="JewishCalendar.IsRoshChodesh"/>
    public bool IsYaalehVeyavoRecited(JewishCalendar jewishCalendar)
    {
        return jewishCalendar.IsPesach() || jewishCalendar.IsShavuos() || jewishCalendar.IsRoshHashana()
                || jewishCalendar.IsYomKippur() || jewishCalendar.IsSuccos() || jewishCalendar.IsShminiAtzeres()
                || jewishCalendar.IsSimchasTorah() || jewishCalendar.IsRoshChodesh();
    }

    /// <summary>
    /// Returns if Is <em>Mizmor Lesoda</em> is recited on the day in question.
    /// </summary>
    /// <param name="jewishCalendar">the Jewish calendar day.</param>
    /// <returns>if <em>Mizmor Lesoda</em> is recited.</returns>
    /// <seealso cref="IsMizmorLesodaRecitedErevYomKippurAndPesach"/>
    public bool IsMizmorLesodaRecited(JewishCalendar jewishCalendar)
    {
        if (jewishCalendar.IsAssurBemelacha())
        {
            return false;
        }

        int holidayIndex = jewishCalendar.GetYomTovIndex();
        return IsMizmorLesodaRecitedErevYomKippurAndPesach || (holidayIndex != JewishCalendar.ErevYomKippur
                && holidayIndex != JewishCalendar.ErevPesach && !jewishCalendar.IsCholHamoedPesach());
    }

    /// <summary>
    /// Is <em>tachanun</em> set to be recited during the week of Purim, from the 11th through the 17th of <see cref="JewishDate.Adar"><em>Adar</em></see> (on a non-leap year, or <see cref="JewishDate.AdarII"><em>Adar II</em></see> on a leap year). Some
    /// <em>chasidishe</em> communities do not recite <em>tachanun</em> during this period. See the <a href=
    /// "https://www.nli.org.il/he/books/NNL_ALEPH001141272/NLI">Minhag Yisrael Torah</a> 131 and <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=4692&amp;st=&amp;pgnum=70">Darkei Chaim Veshalom 191</a>who discuss the
    /// <em>minhag</em> not to recite <em>tachanun</em>. Also see the <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=8944&amp;st=&amp;pgnum=160">Mishmeres Shalom (Hadras Shalom)</a> who discusses the
    /// <em>minhag</em> of not reciting it on the 16th and 17th.
    /// </summary>
    /// <value>Sets if <em>tachanun</em> is to recited during the week of Purim from the 11th
    ///         through the 17th of <see cref="JewishDate.Adar"><em>Adar</em></see> (on a non-leap year), or <see cref="JewishDate.AdarII"><em>Adar II</em></see> (on a leap year). Some <em>chasidishe</em> communities do not recite <em>tachanun</em>
    ///         during this period.</value>
    /// <remarks>
    /// Sets if <em>tachanun</em> should be recited during the week of Purim from the 11th through the 17th of <see cref="JewishDate.Adar"><em>Adar</em></see> (on a non-leap year), or <see cref="JewishDate.AdarII"><em>Adar II</em></see> (on a leap year).
    /// </remarks>
    /// <seealso cref="IsTachanunRecitedWeekOfPurim"/>
    public bool IsTachanunRecitedWeekOfPurim
    {
        get => tachanunRecitedWeekOfPurim;
        set => tachanunRecitedWeekOfPurim = value;
    }

    /// <summary>
    /// Is <em>tachanun</em> set to be recited during the <em>sefira</em> week of <em>Hod</em> (14 - 20 <see cref="JewishDate.Iyar"><em>Iyar</em></see>, or the 29th - 35th of the <see cref="JewishCalendar.GetDayOfOmer"><em>Omer</em></see>). Some
    /// <em>chasidishe</em> communities do not recite <em>tachanun</em> during this week. See Minhag Yisrael Torah 131:Iyar.
    /// </summary>
    /// <value>Sets if <em>tachanun</em> should be recited during the <em>sefira</em> week of
    /// <em>Hod</em>.</value>
    /// <remarks>
    /// Sets if <em>tachanun</em> should be recited during the <em>sefira</em> week of <em>Hod</em> (14 - 20
    /// <see cref="JewishDate.Iyar"><em>Iyar</em></see>, or the 29th - 35th of the <see cref="JewishCalendar.GetDayOfOmer"><em>Omer</em></see>).
    /// </remarks>
    /// <seealso cref="IsTachanunRecitedWeekOfHod"/>
    public bool IsTachanunRecitedWeekOfHod
    {
        get => tachanunRecitedWeekOfHod;
        set => tachanunRecitedWeekOfHod = value;
    }

    /// <summary>
    /// Is <em>tachanun</em> set to be recited at the end Of <see cref="JewishDate.Tishrei"><em>Tishrei</em></see>.The Magen Avraham 669:1 and
    /// the Pri Chadash 131:7 state that some places to not recite <em>tachanun</em> during this period. The Sh"UT Chasam Sofer on
    /// Choshen Mishpat 77 writes that this is the <em>minhag</em> in Ashkenaz. The Shaarei Teshuva 131:19 quotes the Sheyarie Kneses
    /// Hagdola who also states that it should not be recited. The Aderes wanted to institute saying <em>tachanun</em> during this
    /// period, but was dissuaded from this by Rav Shmuel Salant who did not want to change the <em>minhag</em> in Yerushalayim.
    /// The Aruch Hashulchan is of the opinion that this <em>minhag</em> is incorrect, and it should be recited, and The Chazon
    /// Ish also recited <em>tachanun</em> during this period. See the Dirshu edition of the Mishna Berurah for details.
    /// </summary>
    /// <value>is <em>tachanun</em> recited at the end of <see cref="JewishDate.Tishrei"><em>Tishrei</em></see>.</value>
    /// <remarks>
    /// Sets if <em>tachanun</em> should be recited at the end of <see cref="JewishDate.Tishrei"><em>Tishrei</em></see>.
    /// </remarks>
    /// <seealso cref="IsTachanunRecitedEndOfTishrei"/>
    public bool IsTachanunRecitedEndOfTishrei
    {
        get => tachanunRecitedEndOfTishrei;
        set => tachanunRecitedEndOfTishrei = value;
    }


    /// <summary>
    /// Is <em>tachanun</em> set to be recited during the week after <em>Shavuos</em>. This is the opinion of the Pri
    /// Megadim quoted by the Mishna Berurah. This is since <em>karbanos</em> of <em>Shavuos</em> have <em>tashlumim</em>
    /// for 7 days, it is still considered like a <em>Yom Tov</em>. The Chazon Ish quoted in the Orchos Rabainu vol. 1
    /// page 68 recited <em>tachanun</em> during this week.
    /// </summary>
    /// <value>is <em>tachanun</em> recited during the week after Shavuos.</value>
    /// <remarks>
    /// Sets if <em>tachanun</em> should be recited during the week after <em>Shavuos</em>.
    /// </remarks>
    /// <seealso cref="IsTachanunRecitedWeekAfterShavuos"/>
    public bool IsTachanunRecitedWeekAfterShavuos
    {
        get => tachanunRecitedWeekAfterShavuos;
        set => tachanunRecitedWeekAfterShavuos = value;
    }


    /// <summary>
    /// Is <em>tachanun</em> is set to be recited on the 13th of <see cref="JewishDate.Sivan"><em>Sivan</em></see> (<a href=
    /// "https://en.wikipedia.org/wiki/Yom_tov_sheni_shel_galuyot"><em>Yom Tov Sheni shel Galuyos</em></a> of the 7th
    /// day) outside Israel. This is brought down by the Shaarie Teshuva 131:19 quoting the <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=41295&amp;st=&amp;pgnum=39">Sheyarei Kneses Hagedola 131:12</a>that
    /// <em>tachanun</em> should not be recited on this day. Rav Shlomo Zalman Orbach in Halichos Shlomo on
    /// Shavuos 12:16:25 is of the opinion that even in <em>chutz laaretz</em> it should be recited since the <em>yemei
    /// Tashlumin</em> are counted based on Israel since that is where the <em>karbanos</em> are brought. Both
    /// <see cref="IsTachanunRecitedShacharis"/> and <see cref="IsTachanunRecitedMincha"/>
    /// only return false if the location is not set to <see cref="JewishCalendar.InIsrael">Israel</see> and both
    /// <c>TachanunRecitedWeekAfterShavuos</c> and <see cref="IsTachanunRecited13SivanOutOfIsrael"/> are set to false.
    /// </summary>
    /// <value>sets if <em>tachanun</em> should be recited on the 13th of <see cref="JewishDate.Sivan"><em>Sivan</em></see> out of Israel. Both <see cref="IsTachanunRecitedShacharis"/> and
    ///         <see cref="IsTachanunRecitedMincha"/> only return false if the location is not set to <see cref="JewishCalendar.InIsrael">Israel</see> and both <c>TachanunRecitedWeekAfterShavuos</c> and
    ///         <see cref="IsTachanunRecited13SivanOutOfIsrael"/> are set to false.</value>
    /// <remarks>
    /// Sets if <em>tachanun</em> should be recited on the 13th of <see cref="JewishDate.Sivan"><em>Sivan</em></see> (<a href=
    /// "https://en.wikipedia.org/wiki/Yom_tov_sheni_shel_galuyot"><em>Yom Tov Sheni shel Galuyos</em></a> of the 7th
    /// day) outside Israel.
    /// </remarks>
    /// <seealso cref="IsTachanunRecited13SivanOutOfIsrael"/>
    /// <seealso cref="IsTachanunRecitedWeekAfterShavuos"/>
    public bool IsTachanunRecited13SivanOutOfIsrael
    {
        get => tachanunRecited13SivanOutOfIsrael;
        set => tachanunRecited13SivanOutOfIsrael = value;
    }


    /// <summary>
    /// Is <em>tachanun</em> set to be recited on <see cref="JewishCalendar.PesachSheni"><em>Pesach Sheni</em></see>. The Pri Chadash 131:7
    /// states that <em>tachanun</em> should not be recited. The Aruch Hashulchan states that this is the minhag of the
    /// <em>sephardim</em>. The Shaarei Efraim 10:27 also mentions that it is not recited, as does the Siddur Yaavetz (Shaar Hayesod,
    /// Chodesh Iyar). The Pri Megadim (Mishbetzes Hazahav 131:15) and the Chazon Ish (Erev Pesach Shechal Beshabbos, page 203 in
    /// <a href="https://he.wikipedia.org/wiki/%D7%A9%D7%A8%D7%99%D7%94_%D7%93%D7%91%D7%9C%D7%99%D7%A6%D7%A7%D7%99">Rav Sheraya
    /// Devlitzky's</a> comments).
    /// </summary>
    /// <value>sets if <em>tachanun</em> should be recited on <em>Pesach Sheni</em>.</value>
    /// <remarks>
    /// Sets if <em>tachanun</em> should be recited on <see cref="JewishCalendar.PesachSheni"><em>Pesach Sheni</em></see>.
    /// </remarks>
    /// <seealso cref="IsTachanunRecitedPesachSheni"/>
    public bool IsTachanunRecitedPesachSheni
    {
        get => tachanunRecitedPesachSheni;
        set => tachanunRecitedPesachSheni = value;
    }


    /// <summary>
    /// Is <em>tachanun</em> set to be recited on 15 <see cref="JewishDate.Iyar"><em>Iyar</em></see> (<em>sfaika deyoma</em> of <see cref="JewishCalendar.PesachSheni"><em>Pesach Sheni</em></see>) out of Israel. If <see cref="IsTachanunRecitedPesachSheni"/> is
    /// <c>true</c> this will be ignored even if <c>false</c>.
    /// </summary>
    /// <value>if <em>tachanun</em> should be recited on the 15th of <see cref="JewishDate.Iyar"><em>Iyar</em></see> (<em>sfaika deyoma</em> of <see cref="JewishCalendar.PesachSheni"><em>Pesach Sheni</em></see>) out of Israel.</value>
    /// <remarks>
    /// Sets if <em>tachanun</em> should be recited on the 15th of <see cref="JewishDate.Iyar"><em>Iyar</em></see> (<a href=
    /// "https://en.wikipedia.org/wiki/Yom_tov_sheni_shel_galuyot"><em>Yom Tov Sheni shel Galuyos</em></a> of
    /// <see cref="JewishCalendar.PesachSheni"><em>Pesach Sheni</em></see>) out of Israel. Ignored if <see cref="IsTachanunRecitedPesachSheni"/> is <c>true</c>.
    /// </remarks>
    /// <seealso cref="IsTachanunRecited15IyarOutOfIsrael"/>
    /// <seealso cref="IsTachanunRecitedPesachSheni"/>
    public bool IsTachanunRecited15IyarOutOfIsrael
    {
        get => tachanunRecited15IyarOutOfIsrael;
        set => tachanunRecited15IyarOutOfIsrael = value;
    }


    /// <summary>
    /// Is <em>tachanun</em> set to be recited on <em>mincha</em> on <em>erev <see cref="JewishCalendar.LagBaomer">Lag
    /// Baomer</see></em>.
    /// </summary>
    /// <value>sets if <em>tachanun</em> should be recited on <em>mincha</em>
    ///         of <em>erev <see cref="JewishCalendar.LagBaomer">Lag Baomer</see></em>.</value>
    /// <remarks>
    /// Sets if <em>tachanun</em> should be recited on <em>erev <see cref="JewishCalendar.LagBaomer">Lag Baomer</see></em>.
    /// </remarks>
    /// <seealso cref="IsTachanunRecitedMinchaErevLagBaomer"/>
    public bool IsTachanunRecitedMinchaErevLagBaomer
    {
        get => tachanunRecitedMinchaErevLagBaomer;
        set => tachanunRecitedMinchaErevLagBaomer = value;
    }


    /// <summary>
    /// Is <em>tachanun</em> set to be recited during the <em>Shivas Yemei Hamiluim</em>, from the 23 of <see cref="JewishDate.Adar"><em>Adar</em></see> on a non-leap-year or <see cref="JewishDate.AdarII"><em>Adar II</em></see> on a
    /// leap year to the end of the month. Some <em>chasidishe</em> communities do not say <em>tachanun</em>
    /// during this week. See <a href="https://hebrewbooks.org/pdfpager.aspx?req=4692&amp;st=&amp;pgnum=70">Darkei
    /// Chaim Veshalom 191</a>.
    /// </summary>
    /// <value>sets if <em>tachanun</em> should be recited during the <em>Shivas Yemei Hamiluim</em>.</value>
    /// <remarks>
    /// Sets if <em>tachanun</em> should be recited during the <em>Shivas Yemei Hamiluim</em>, from the 23 of
    /// <see cref="JewishDate.Adar"><em>Adar</em></see> on a non-leap-year or <see cref="JewishDate.AdarII"><em>Adar II</em></see>
    /// on a leap year to the end of the month.
    /// </remarks>
    /// <seealso cref="IsTachanunRecitedShivasYemeiHamiluim"/>
    public bool IsTachanunRecitedShivasYemeiHamiluim
    {
        get => tachanunRecitedShivasYemeiHamiluim;
        set => tachanunRecitedShivasYemeiHamiluim = value;
    }

    /// <summary>
    /// Is <em>tachanun</em> set to be recited on Fridays. Some <em>chasidishe</em> communities do not recite
    /// <em>tachanun</em> on Fridays. See <a href="https://hebrewbooks.org/pdfpager.aspx?req=41190&amp;st=&amp;pgnum=10">Likutei
    /// Maharich Vol 2 Seder Hanhagos Erev Shabbos</a>. This is also the <em>minhag</em> in Satmar.
    /// </summary>
    /// <value>sets if <em>tachanun</em> should be recited on Fridays. Some <em>chasidishe</em> communities
    ///         do not recite <em>tachanun</em> on Fridays.</value>
    /// <remarks>
    /// Sets if <em>tachanun</em> should be recited on Fridays.
    /// </remarks>
    /// <seealso cref="IsTachanunRecitedFridays"/>
    public bool IsTachanunRecitedFridays
    {
        get => tachanunRecitedFridays;
        set => tachanunRecitedFridays = value;
    }

    /// <summary>
    /// Is <em>tachanun</em> set to be recited on Sundays. Some <em>chasidishe</em> communities do not recite <em>tachanun</em> on
    /// Sundays. See <a href="https://hebrewbooks.org/pdfpager.aspx?req=41190&amp;st=&amp;pgnum=10">Likutei Maharich Vol 2 Seder Hanhagos
    /// Erev Shabbos</a>.
    /// </summary>
    /// <value>sets if <em>tachanun</em> should be recited on Sundays. Some <em>chasidishe</em> communities
    ///         do not recite <em>tachanun</em> on Sundays.</value>
    /// <remarks>
    /// Sets if <em>tachanun</em> should be recited on Sundays.
    /// </remarks>
    /// <seealso cref="IsTachanunRecitedSundays"/>
    public bool IsTachanunRecitedSundays
    {
        get => tachanunRecitedSundays;
        set => tachanunRecitedSundays = value;
    }


    /// <summary>
    /// Is <em>tachanun</em> set to be recited in <em>Mincha</em> the entire year. Some <em>chasidishe</em> communities do
    /// not recite <em>tachanun</em> by <em>Mincha</em> all year round. See <a href=
    /// "https://hebrewbooks.org/pdfpager.aspx?req=4751&amp;st=&amp;pgnum=105">Nemukei Orach Chaim 131:3</a>.
    /// </summary>
    /// <value>sets if <em>tachanun</em> should be recited by <em>mincha</em> all year. If set
    ///         to false, <see cref="IsTachanunRecitedMincha"/> will always return false. If set to true (the default),
    ///         it will use the regular rules.</value>
    /// <remarks>
    /// Sets if <em>tachanun</em> should be recited in <em>Mincha</em> the entire year.
    /// </remarks>
    /// <seealso cref="IsTachanunRecitedMinchaAllYear"/>
    public bool IsTachanunRecitedMinchaAllYear
    {
        get => tachanunRecitedMinchaAllYear;
        set => tachanunRecitedMinchaAllYear = value;
    }


    /// <summary>
    /// Is <em>Mizmor Lesoda</em> set to be recited on <em>Erev Yom Kippur</em>, <em>Erev Pesach</em> and <em>Chol Hamoed Pesach</em>.
    /// Ashkenazi congregations do not recite it on these days, while Sephardi congregations do. The default value is
    /// <c>false</c>.
    /// </summary>
    /// <value>Sets if <em>Mizmor Lesoda</em> should be recited on <em>Erev Yom Kippur</em>,
    ///         <em>Erev Pesach</em> and <em>Chol Hamoed Pesach</em>. If set to true (the default value is <c>false</c>).</value>
    /// <remarks>
    /// Sets if <em>Mizmor Lesoda</em> should be recited on <em>Erev Yom Kippur</em>, <em>Erev Pesach</em> and <em>Chol
    /// Hamoed Pesach</em>. Ashkenazi congregations do not recite it on these days, while Sephardi congregations do. The
    /// default value is <c>false</c>.
    /// </remarks>
    /// <seealso cref="IsMizmorLesodaRecited"/>
    /// <seealso cref="IsTachanunRecitedMinchaAllYear"/>
    public bool IsMizmorLesodaRecitedErevYomKippurAndPesach
    {
        get => mizmorLesodaRecitedErevYomKippurAndPesach;
        set => mizmorLesodaRecitedErevYomKippurAndPesach = value;
    }
}
