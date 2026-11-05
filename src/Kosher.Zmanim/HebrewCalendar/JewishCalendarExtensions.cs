/*
 * Zmanim .NET API
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
namespace Kosher.Zmanim.HebrewCalendar;

/// <summary>
/// LINQ-friendly extensions for <see cref="JewishDate"/> and <see cref="JewishCalendar"/>. These are additions of the .NET
/// port and have no KosherJava equivalent.
/// </summary>
/// <remarks>
/// <see cref="JewishDate"/> and <see cref="JewishCalendar"/> are mutable (as in KosherJava), so these sequences yield a
/// <b>separate copy</b> for each day. The instance they are called on is never changed.
/// <code>
/// var jc = new JewishCalendar(new DateOnly(2026, 1, 1)) { InIsrael = false };
///
/// // all the yamim tovim of the coming year
/// var yamimTovim = jc.EachDay(385)
///     .Where(day => day.IsYomTov())
///     .Select(day => (day.LocalDate, Name: new HebrewDateFormatter().FormatYomTov(day)));
///
/// // the parshiyos read on Shabbos in Kislev 5787
/// var parshiyos = JewishCalendarExtensions.EachDay(new JewishCalendar(5787, JewishDate.Kislev, 1), 30)
///     .Where(day => day.DayOfWeek == 7)
///     .Select(day => day.GetParshah());
/// </code>
/// </remarks>
public static class JewishCalendarExtensions
{
    /// <summary>
    /// Returns a copy of <paramref name="date"/> (see <see cref="JewishDate.Clone"/>) for each of <paramref name="count"/>
    /// consecutive days, starting with the date itself.
    /// </summary>
    /// <typeparam name="TDate">the type, <see cref="JewishDate"/> or <see cref="JewishCalendar"/>.</typeparam>
    /// <param name="date">the first day. Settings such as <see cref="JewishCalendar.InIsrael"/> are copied. It is not changed.</param>
    /// <param name="count">the number of days.</param>
    /// <returns>a lazily evaluated sequence with one instance per day.</returns>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="count"/> is negative.</exception>
    public static IEnumerable<TDate> EachDay<TDate>(this TDate date, int count) where TDate : JewishDate
    {
        ArgumentNullException.ThrowIfNull(date);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        return Iterate(date, count);

        static IEnumerable<TDate> Iterate(TDate date, int count)
        {
            var current = (TDate)date.Clone();
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    current = (TDate)current.Clone();
                    current.PlusDays(1);
                }
                yield return current;
            }
        }
    }

    /// <summary>
    /// Returns a copy of <paramref name="date"/> (see <see cref="JewishDate.Clone"/>) for each day up to and including the
    /// Gregorian date <paramref name="endInclusive"/>, starting with the date itself.
    /// </summary>
    /// <typeparam name="TDate">the type, <see cref="JewishDate"/> or <see cref="JewishCalendar"/>.</typeparam>
    /// <param name="date">the first day. Settings such as <see cref="JewishCalendar.InIsrael"/> are copied. It is not changed.</param>
    /// <param name="endInclusive">the last Gregorian date. If it is before <paramref name="date"/> the sequence is empty.</param>
    /// <returns>a lazily evaluated sequence with one instance per day.</returns>
    public static IEnumerable<TDate> EachDay<TDate>(this TDate date, DateOnly endInclusive) where TDate : JewishDate
    {
        ArgumentNullException.ThrowIfNull(date);
        int days = endInclusive.DayNumber - date.LocalDate.DayNumber + 1;
        return EachDay(date, Math.Max(days, 0));
    }
}
