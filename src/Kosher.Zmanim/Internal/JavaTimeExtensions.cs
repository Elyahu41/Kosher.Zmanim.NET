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
/// Internal extension methods with the names and semantics of the <c>java.time</c> methods used by KosherJava, so the ported
/// algorithms read the same as the Java originals. Like Java, calling one of these on a missing (<c>null</c>) value is a
/// programming error and throws.
/// </summary>
internal static class JavaTimeExtensions
{
    /// <summary><c>Instant.isBefore(Instant)</c>.</summary>
    public static bool IsBefore(this DateTimeOffset? time, DateTimeOffset? other) => time!.Value < other!.Value;

    /// <summary><c>Instant.isAfter(Instant)</c>.</summary>
    public static bool IsAfter(this DateTimeOffset? time, DateTimeOffset? other) => time!.Value > other!.Value;

    /// <summary><c>Instant.isBefore(Instant)</c>.</summary>
    public static bool IsBefore(this DateTimeOffset time, DateTimeOffset other) => time < other;

    /// <summary><c>Instant.isAfter(Instant)</c>.</summary>
    public static bool IsAfter(this DateTimeOffset time, DateTimeOffset other) => time > other;

    /// <summary><c>Instant.plus(Duration)</c>.</summary>
    public static DateTimeOffset Plus(this DateTimeOffset? time, TimeSpan? duration) => time!.Value + duration!.Value;

    /// <summary><c>Instant.minus(Duration)</c>.</summary>
    public static DateTimeOffset Minus(this DateTimeOffset? time, TimeSpan? duration) => time!.Value - duration!.Value;

    /// <summary><c>Instant.plusSeconds(long)</c>.</summary>
    public static DateTimeOffset PlusSeconds(this DateTimeOffset? time, long seconds) => time!.Value.AddTicks(seconds * TimeSpan.TicksPerSecond);

    /// <summary><c>Instant.toEpochMilli()</c> (floors to whole milliseconds).</summary>
    public static long ToEpochMilli(this DateTimeOffset? time) => time!.Value.ToUnixTimeMilliseconds();

    /// <summary><c>Instant.toEpochMilli()</c> (floors to whole milliseconds).</summary>
    public static long ToEpochMilli(this DateTimeOffset time) => time.ToUnixTimeMilliseconds();

    /// <summary><c>Duration.toNanos()</c>.</summary>
    public static long ToNanos(this TimeSpan? duration) => duration!.Value.Ticks * JavaTime.NanosPerTick;

    /// <summary><c>Duration.toNanos()</c>.</summary>
    public static long ToNanos(this TimeSpan duration) => duration.Ticks * JavaTime.NanosPerTick;

    /// <summary><c>Duration.toMillis()</c> (floors to whole milliseconds).</summary>
    public static long ToMillis(this TimeSpan? duration) => JavaTime.ToMillis(duration!.Value);

    /// <summary><c>Duration.dividedBy(long)</c> (truncates).</summary>
    public static TimeSpan DividedBy(this TimeSpan? duration, long divisor) => JavaTime.Divide(duration!.Value, divisor);

    /// <summary><c>Duration.multipliedBy(long)</c>.</summary>
    public static TimeSpan MultipliedBy(this TimeSpan? duration, long multiplier) => JavaTime.Multiply(duration!.Value, multiplier);

    /// <summary><c>Duration.dividedBy(long)</c> (truncates).</summary>
    public static TimeSpan DividedBy(this TimeSpan duration, long divisor) => JavaTime.Divide(duration, divisor);

    /// <summary><c>Duration.multipliedBy(long)</c>.</summary>
    public static TimeSpan MultipliedBy(this TimeSpan duration, long multiplier) => JavaTime.Multiply(duration, multiplier);

    /// <summary><c>Duration.toMillis()</c> (floors to whole milliseconds).</summary>
    public static long ToMillis(this TimeSpan duration) => JavaTime.ToMillis(duration);

    /// <summary><c>Instant.plus(Duration)</c>.</summary>
    public static DateTimeOffset Plus(this DateTimeOffset time, TimeSpan duration) => time + duration;

    /// <summary><c>Instant.minus(Duration)</c>.</summary>
    public static DateTimeOffset Minus(this DateTimeOffset time, TimeSpan duration) => time - duration;

    /// <summary><c>LocalDate.isBefore(LocalDate)</c>.</summary>
    public static bool IsBefore(this DateOnly date, DateOnly other) => date < other;

    /// <summary><c>LocalDate.isAfter(LocalDate)</c>.</summary>
    public static bool IsAfter(this DateOnly date, DateOnly other) => date > other;

    /// <summary><c>Duration.abs()</c>.</summary>
    public static TimeSpan Abs(this TimeSpan? duration) => duration!.Value.Duration();
}
