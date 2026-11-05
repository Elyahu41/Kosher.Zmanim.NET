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
namespace Kosher.Zmanim.Internal;

/// <summary>
/// The <c>java.util.Calendar</c> day-of-week constants (Sunday = 1 through Saturday = 7) that KosherJava uses for
/// <see cref="HebrewCalendar.JewishDate.DayOfWeek"/>.
/// </summary>
internal static class Weekday
{
    public const int Sunday = 1;
    public const int Monday = 2;
    public const int Tuesday = 3;
    public const int Wednesday = 4;
    public const int Thursday = 5;
    public const int Friday = 6;
    public const int Saturday = 7;
}
