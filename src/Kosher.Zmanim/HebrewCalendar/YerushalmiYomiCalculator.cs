/*
 * Zmanim Java API
 * Copyright © 2017 - 2026 Eliyahu Hershfeld
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; version 2.1 of the License.
 *
 * This library is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
 * details.
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not, write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA,
 * or connect to: http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html
 */
namespace Kosher.Zmanim.HebrewCalendar;

/// <summary>
/// This class calculates the <a href="https://en.wikipedia.org/wiki/Jerusalem_Talmud">Talmud Yerusalmi</a> <a href=
/// "https://en.wikipedia.org/wiki/Daf_Yomi">Daf Yomi</a> page (<see cref="Daf"/>) for the a given date.
/// </summary>
public class YerushalmiYomiCalculator
{

    /// <summary>
    /// Start date of first Daf Yomi Yerushalmi cycle: 1980-02-02
    /// </summary>
    private static readonly DateOnly DafYomiStartDay = new DateOnly(1980, 2, 2);

    /// <summary>
    /// number of pages (<em>blatt/dafim</em>) in <em>Shas</em>
    /// </summary>
    private const int WholeShasDafs = 1554;

    /// <summary>
    /// number of pages (<em>blatt/dafim</em>) per <em>masechta</em>
    /// </summary>
    private static readonly int[] BlattPerMasechta = {
            68, 37, 34, 44, 31, 59, 26, 33, 28, 20, 13, 92, 65, 71, 22, 22, 42, 26, 26, 33, 34, 22,
            19, 85, 72, 47, 40, 47, 54, 48, 44, 37, 34, 44, 9, 57, 37, 19, 13
    };

    /// <summary>
    /// Default constructor.
    /// </summary>
    public YerushalmiYomiCalculator()
    {
        // No-op
    }

    /// <summary>
    /// Returns the Daf Yomi Yerushalmi page for a given Jewish date.
    /// Returns null on Yom Kippur or Tisha B’Av.
    /// </summary>
    /// <param name="jewishCalendar">the <c>JewishCalendar</c> to set.</param>
    /// <returns>the daf.</returns>
    public static Daf? GetDafYomiYerushalmi(JewishCalendar jewishCalendar)
    {
        DateOnly requested = jewishCalendar.LocalDate;
        int masechta = 0;
        Daf? dafYomi = null;

        // No Daf Yomi on Yom Kippur or Tisha B'Av
        if (jewishCalendar.GetYomTovIndex() == JewishCalendar.YomKippur ||
                jewishCalendar.GetYomTovIndex() == JewishCalendar.TishaBeav)
        {
            return null;
        }

        if (requested.IsBefore(DafYomiStartDay))
        {
            throw new ArgumentException(requested + " is prior to the first Daf Yomi cycle starting on "
                    + DafYomiStartDay);
        }

        // Initialize cycle dates
        DateOnly prevCycle = DafYomiStartDay;
        DateOnly nextCycle = GetNextCycleStart(prevCycle);

        // Loop through cycles until we reach the requested date
        while (!requested.IsBefore(nextCycle))
        {
            prevCycle = nextCycle;
            nextCycle = GetNextCycleStart(prevCycle);
        }

        // Days between start of cycle and requested date
        int dafNo = (int)GetDiffBetweenDays(prevCycle, requested);

        int specialDays = GetNumOfSpecialDays(prevCycle, requested);
        int total = dafNo - specialDays;

        // Find the daf
        foreach (int blatt in BlattPerMasechta)
        {
            if (total < blatt)
            {
                dafYomi = new Daf(masechta, total + 1);
                break;
            }
            total -= blatt;
            masechta++;
        }

        return dafYomi;
    }

    /// <summary>
    /// Returns the start date of the cycle after the one beginning on <c>cycleStart</c>.
    /// The cycle end is inclusive, so skipped days on the tentative end date extend the cycle.
    /// </summary>
    /// <param name="cycleStart">the first day of the current cycle</param>
    /// <returns>the first day of the following cycle</returns>
    private static DateOnly GetNextCycleStart(DateOnly cycleStart)
    {
        DateOnly endDate = cycleStart.AddDays(WholeShasDafs - 1);
        int specialDays = GetNumOfSpecialDays(cycleStart, endDate);

        while (specialDays > 0)
        {
            DateOnly newStart = endDate.AddDays(1);
            endDate = endDate.AddDays(specialDays);
            specialDays = GetNumOfSpecialDays(newStart, endDate);
        }

        return endDate.AddDays(1);
    }

    /// <summary>
    /// Counts the number of "special days" (Yom Kippur, Tisha B’Av) between two dates.
    /// </summary>
    /// <param name="start">the start date for the calculation</param>
    /// <param name="end">the end date for the calculation</param>
    /// <returns>the number of special days</returns>
    private static int GetNumOfSpecialDays(DateOnly start, DateOnly end)
    {

        int startYear = new JewishCalendar(start).JewishYear;
        int endYear = new JewishCalendar(end).JewishYear;

        int specialDays = 0;

        // Loop over each Jewish year in range
        for (int year = startYear; year <= endYear; year++)
        {
            // Create Yom Kippur and Tisha B’Av for that Jewish year
            JewishCalendar yomKippur = new JewishCalendar(year, JewishCalendar.Tishrei, 10);
            JewishCalendar tishaBeav = new JewishCalendar(year, JewishCalendar.Av, 9);
            // Check if Tisha B'Av is a Nidche.
            if (tishaBeav.DayOfWeek == 7)
            {
                tishaBeav.PlusDays(1);
            }
            DateOnly ykDate = yomKippur.LocalDate;
            DateOnly tbDate = tishaBeav.LocalDate;

            if (IsBetween(start, ykDate, end)) specialDays++;
            if (IsBetween(start, tbDate, end)) specialDays++;
        }

        return specialDays;
    }


    /// <summary>
    /// Checks if a date is on or after start and on or before end. A cycle can start on, or be extended to start
    /// on, Yom Kippur or Tisha B'Av, so the start day is counted.
    /// </summary>
    /// <param name="start">the start <c>ZonedDateTime</c></param>
    /// <param name="date">the <c>ZonedDateTime</c> to check</param>
    /// <param name="end">the end <c>ZonedDateTime</c></param>
    /// <returns>if the date is between the two dates</returns>
    private static bool IsBetween(DateOnly start, DateOnly date, DateOnly end)
    {
        return !date.IsBefore(start) && !end.IsBefore(date);
    }

    /// <summary>
    /// Returns the number of full days between two <c>ZonedDateTime</c>s.
    /// </summary>
    /// <param name="start">the start <c>ZonedDateTime</c></param>
    /// <param name="end">the end <c>ZonedDateTime</c></param>
    /// <returns>the number of days between the dates.</returns>
    private static long GetDiffBetweenDays(DateOnly start, DateOnly end)
    {
        return end.DayNumber - start.DayNumber;
    }
}
