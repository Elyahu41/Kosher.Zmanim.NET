/*
 * Zmanim Java API
 * Copyright © 2011-2026 Eliyahu Hershfeld
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
/// This class calculates the Daf Yomi Bavli page (daf) for a given date. To calculate Daf Yomi Yerushalmi
/// use the <see cref="YerushalmiYomiCalculator"/>. The library may cover Mishna Yomi etc. at some point in the future.
/// </summary>
public class YomiCalculator
{

    /// <summary>
    /// The start date of the first Daf Yomi Bavli cycle of September 11, 1923 / Rosh Hashana 5684.
    /// </summary>
    private static readonly DateOnly dafYomiStartDay = new DateOnly(1923, 9, 11);
    /// <summary>
    /// The start date of the first Daf Yomi Bavli cycle in the Julian calendar. Used internally for calculations.
    /// </summary>
    private static readonly int dafYomiJulianStartDay = GetJulianDay(dafYomiStartDay);
    /// <summary>
    /// The date that the pagination for the Daf Yomi <em>Maseches Shekalim</em> changed to use the commonly used Vilna
    /// Shas pagination from the no longer commonly available Zhitomir / Slavuta Shas used by Rabbi Meir Shapiro.
    /// </summary>
    private static readonly DateOnly shekalimChangeDay = new DateOnly(1975, 6, 24);

    /// <summary>
    /// The Julian date that the cycle for Shekalim changed.
    /// </summary>
    /// <seealso cref="GetDafYomiBavli"/>
    private static readonly int shekalimJulianChangeDay = GetJulianDay(shekalimChangeDay);

    /// <summary>
    /// Default constructor.
    /// </summary>
    public YomiCalculator()
    {
        // nothing here
    }

    /// <summary>
    /// Returns the <a href="http://en.wikipedia.org/wiki/Daf_yomi">Daf Yomi</a> <a
    /// href="http://en.wikipedia.org/wiki/Talmud">Bavli</a> <see cref="Daf"/> for a given date. The first Daf Yomi cycle
    /// started on Rosh Hashana 5684 (September 11, 1923) and calculations prior to this date will result in an
    /// IllegalArgumentException thrown. For historical calculations (supported by this method), it is important to note
    /// that a change in length of the cycle was instituted starting in the eighth Daf Yomi cycle beginning on June 24,
    /// 1975. The Daf Yomi Bavli cycle has a single masechta of the Talmud Yerushalmi - Shekalim as part of the cycle.
    /// Unlike the Bavli where the number of daf per masechta was standardized since the original <a
    /// href="http://en.wikipedia.org/wiki/Daniel_Bomberg">Bomberg Edition</a> published from 1520 - 1523, there is no
    /// uniform page length in the Yerushalmi. The early cycles had the Yerushalmi Shekalim length of 13 days following the
    /// <a href=
    /// "https://he.wikipedia.org/wiki/%D7%93%D7%A4%D7%95%D7%A1_%D7%A1%D7%9C%D7%90%D7%95%D7%95%D7%99%D7%98%D7%90">Slavuta/Zhytomyr</a>
    /// Shas used by <a href="http://en.wikipedia.org/wiki/Meir_Shapiro">Rabbi Meir Shapiro</a>. With the start of the eighth Daf Yomi
    /// cycle beginning on June 24, 1975, the length of the Yerushalmi Shekalim was changed from 13 to 22 daf to follow
    /// the <a href="https://en.wikipedia.org/wiki/Vilna_Edition_Shas">Vilna Shas</a> that is in common use today.
    /// </summary>
    /// <param name="jewishCalendar">The JewishCalendar date for calculation. TODO: this can be changed to use a regular GregorianCalendar since
    ///            there is nothing specific to the JewishCalendar in this class.</param>
    /// <returns>the <see cref="Daf"/>.</returns>
    /// <exception cref="ArgumentException">if the date is prior to the September 11, 1923, the start date of the first Daf Yomi cycle.</exception>
    public static Daf GetDafYomiBavli(JewishCalendar jewishCalendar)
    {
        /*
         * The number of daf per masechta. Since the number of blatt in Shekalim changed on the 8th Daf Yomi cycle
         * beginning on June 24, 1975, from 13 to 22, the actual calculation for blattPerMasechta[4] will later be
         * adjusted based on the cycle.
         */
        int[] blattPerMasechta = { 64, 157, 105, 121, 22, 88, 56, 40, 35, 31, 32, 29, 27, 122, 112, 91, 66, 49, 90, 82,
                119, 119, 176, 113, 24, 49, 76, 14, 120, 110, 142, 61, 34, 34, 28, 22, 4, 9, 5, 73 };
        DateOnly date = jewishCalendar.LocalDate;

        Daf? dafYomi = null;
        int julianDay = GetJulianDay(date);
        int cycleNo;
        int dafNo;
        if (date.IsBefore(dafYomiStartDay))
        {
            // TODO: should we return a null or throw an IllegalArgumentException?
            throw new ArgumentException(date + " is prior to organized Daf Yomi Bavli cycles that started on "
                    + dafYomiStartDay);
        }
        if (date.Equals(shekalimChangeDay) || date.IsAfter(shekalimChangeDay))
        {
            cycleNo = 8 + ((julianDay - shekalimJulianChangeDay) / 2711);
            dafNo = ((julianDay - shekalimJulianChangeDay) % 2711);
        }
        else
        {
            cycleNo = 1 + ((julianDay - dafYomiJulianStartDay) / 2702);
            dafNo = ((julianDay - dafYomiJulianStartDay) % 2702);
        }

        int total = 0;
        int masechta = -1;
        int blatt;

        // Fix Shekalim for old cycles.
        if (cycleNo <= 7)
        {
            blattPerMasechta[4] = 13;
        }

        // Finally find the daf.
        foreach (int i in blattPerMasechta)
        {
            masechta++;
            total = total + i - 1;
            if (dafNo < total)
            {
                blatt = 1 + i - (total - dafNo);
                // Fiddle with the weird ones near the end.
                if (masechta == 36)
                {
                    blatt += 21;
                }
                else if (masechta == 37)
                {
                    blatt += 24;
                }
                else if (masechta == 38)
                {
                    blatt += 32;
                }
                dafYomi = new Daf(masechta, blatt);
                break;
            }
        }

        return dafYomi!;
    }

    /// <summary>
    /// Return the <a href="http://en.wikipedia.org/wiki/Julian_day">Julian day</a> from a Java Calendar.
    /// </summary>
    /// <param name="localDate">The Java <see cref="DateOnly"/> of the date to be calculated</param>
    /// <returns>the Julian day number corresponding to the date</returns>
    private static int GetJulianDay(DateOnly localDate)
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
        return (int)(Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + b - 1524.5);
    }
}
