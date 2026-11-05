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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Kosher.Zmanim.Util;

namespace Kosher.Zmanim;

/// <summary>
/// LINQ-friendly extensions for the zmanim calendars. These are additions of the .NET port and have no KosherJava equivalent.
/// </summary>
/// <remarks>
/// The calendar classes are mutable (as in KosherJava): setting <see cref="AstronomicalCalendar.LocalDate"/> changes the
/// results of every method. The sequences returned here therefore yield a <b>separate copy</b> of the calendar for each day, so
/// they are safe to buffer, sort, group or query in parallel with LINQ. The calendar they are called on is never changed.
/// <code>
/// var czc = new ComprehensiveZmanimCalendar(location);
///
/// // the earliest and latest candle lighting over the next year
/// var candleLighting = czc.EachDay(DateOnly.FromDateTime(DateTime.Today), 365)
///     .Select(day => day.GetCandleLighting())
///     .OfType&lt;DateTimeOffset&gt;()
///     .ToList();
///
/// // every day in June where tzais 8.5° can't be calculated
/// var noTzais = czc.EachDay(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30))
///     .Where(day => day.GetTzaisGeonim8Point5Degrees() == null)
///     .Select(day => day.LocalDate);
///
/// // today's zmanim in time order
/// foreach (Zman zman in czc.GetZmanim().Where(z => z.Time != null).OrderBy(z => z, Zman.DateOrder))
/// {
///     Console.WriteLine($"{zman.Label}: {zman.Time:t}");
/// }
/// </code>
/// </remarks>
public static class CalendarExtensions
{
    /// <summary>
    /// Returns a copy of <paramref name="calendar"/> (see <see cref="AstronomicalCalendar.Clone"/>) set to each day from
    /// <paramref name="start"/> through <paramref name="endInclusive"/>.
    /// </summary>
    /// <typeparam name="TCalendar">the calendar type, such as <see cref="ComprehensiveZmanimCalendar"/>.</typeparam>
    /// <param name="calendar">the calendar whose location, calculator and settings are used. It is not changed.</param>
    /// <param name="start">the first date.</param>
    /// <param name="endInclusive">the last date. If it is before <paramref name="start"/> the sequence is empty.</param>
    /// <returns>a lazily evaluated sequence with one calendar per day.</returns>
    public static IEnumerable<TCalendar> EachDay<TCalendar>(this TCalendar calendar, DateOnly start, DateOnly endInclusive)
        where TCalendar : AstronomicalCalendar
    {
        ArgumentNullException.ThrowIfNull(calendar);
        int days = endInclusive.DayNumber - start.DayNumber + 1;
        return EachDay(calendar, start, Math.Max(days, 0));
    }

    /// <summary>
    /// Returns a copy of <paramref name="calendar"/> (see <see cref="AstronomicalCalendar.Clone"/>) set to each of
    /// <paramref name="count"/> consecutive days starting at <paramref name="start"/>.
    /// </summary>
    /// <typeparam name="TCalendar">the calendar type, such as <see cref="ComprehensiveZmanimCalendar"/>.</typeparam>
    /// <param name="calendar">the calendar whose location, calculator and settings are used. It is not changed.</param>
    /// <param name="start">the first date.</param>
    /// <param name="count">the number of days.</param>
    /// <returns>a lazily evaluated sequence with one calendar per day.</returns>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="count"/> is negative.</exception>
    public static IEnumerable<TCalendar> EachDay<TCalendar>(this TCalendar calendar, DateOnly start, int count)
        where TCalendar : AstronomicalCalendar
    {
        ArgumentNullException.ThrowIfNull(calendar);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        return Iterate(calendar, start, count);

        static IEnumerable<TCalendar> Iterate(TCalendar calendar, DateOnly start, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var day = (TCalendar)calendar.Clone();
                day.LocalDate = start.AddDays(i);
                yield return day;
            }
        }
    }

    /// <summary>
    /// Returns every <em>zman</em> of the calendar for its <see cref="AstronomicalCalendar.LocalDate"/>: the result of each public,
    /// parameterless <c>Get…()</c> method that returns a <see cref="DateTimeOffset"/> (such as
    /// <see cref="AstronomicalCalendar.GetSunrise"/>) or a <see cref="TimeSpan"/> (such as
    /// <see cref="ZmanimCalendar.GetShaahZmanisGRA"/>), as a <see cref="Zman"/> whose <see cref="Zman.Label"/> is the method name
    /// without the <c>Get</c> prefix (for example <c>"SofZmanShmaGRA"</c>). <em>Zmanim</em> that do not occur that day are
    /// included with a <c>null</c> <see cref="Zman.Time"/>. The order is unspecified; sort with <see cref="Zman.DateOrder"/>,
    /// <see cref="Zman.DurationOrder"/> or <see cref="Zman.NameOrder"/>. This is the same set of values serialized by
    /// <see cref="ZmanimFormatter.ToXml"/>, except that methods marked <see cref="ObsoleteAttribute"/> (the <em>zmanim</em> that
    /// KosherJava deprecates as <em>lechumra</em> only, or as slow) are skipped.
    /// </summary>
    /// <param name="calendar">the calendar.</param>
    /// <returns>the <em>zmanim</em> of the day.</returns>
    [RequiresUnreferencedCode("Discovers zmanim methods with reflection.")]
    public static IEnumerable<Zman> GetZmanim(this AstronomicalCalendar calendar)
    {
        ArgumentNullException.ThrowIfNull(calendar);
        foreach (MethodInfo method in calendar.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            if (method.GetParameters().Length != 0 || method.IsSpecialName || !method.Name.StartsWith("Get", StringComparison.Ordinal)
                || method.IsDefined(typeof(ObsoleteAttribute)))
            {
                continue;
            }
            string label = method.Name.Substring(3);
            if (method.ReturnType == typeof(DateTimeOffset?) || method.ReturnType == typeof(DateTimeOffset))
            {
                yield return new Zman((DateTimeOffset?)method.Invoke(calendar, null), calendar.GeoLocation, label);
            }
            else if (method.ReturnType == typeof(TimeSpan?) || method.ReturnType == typeof(TimeSpan))
            {
                yield return new Zman((TimeSpan?)method.Invoke(calendar, null), label) { GeoLocation = calendar.GeoLocation };
            }
        }
    }
}
