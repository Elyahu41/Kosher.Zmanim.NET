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
/// Helpers that map the <c>java.time</c> operations used by KosherJava onto the native .NET date and time types.
/// </summary>
/// <remarks>
/// <c>java.time</c> has nanosecond resolution while <see cref="DateTimeOffset"/> and <see cref="TimeSpan"/> have 100 nanosecond
/// ticks. Nanosecond values are converted with floor division, so a time truncated to milliseconds is always identical to
/// <c>Instant.toEpochMilli()</c> in KosherJava.
/// </remarks>
internal static class JavaTime
{
    /// <summary>Nanoseconds in one tick.</summary>
    public const long NanosPerTick = 100;

    /// <summary>The nanosecond-of-second field, as <c>ZonedDateTime.getNano()</c> returns it.</summary>
    public static int GetNano(DateTimeOffset time) => (int)(time.UtcTicks % TimeSpan.TicksPerSecond * NanosPerTick);

    /// <summary>Converts nanoseconds to ticks (floor division).</summary>
    public static long NanosToTicks(long nanos) => JavaMath.FloorDiv(nanos, NanosPerTick);

    /// <summary>Equivalent of <c>Duration.ofNanos(long)</c>.</summary>
    public static TimeSpan DurationOfNanos(long nanos) => TimeSpan.FromTicks(NanosToTicks(nanos));

    /// <summary>Equivalent of <c>Duration.ofSeconds(long)</c>.</summary>
    public static TimeSpan OfSeconds(long seconds) => TimeSpan.FromTicks(checked(seconds * TimeSpan.TicksPerSecond));

    /// <summary>Equivalent of <c>Duration.ofMinutes(long)</c>.</summary>
    public static TimeSpan OfMinutes(long minutes) => TimeSpan.FromTicks(checked(minutes * TimeSpan.TicksPerMinute));

    /// <summary>Equivalent of <c>Duration.ofHours(long)</c>.</summary>
    public static TimeSpan OfHours(long hours) => TimeSpan.FromTicks(checked(hours * TimeSpan.TicksPerHour));

    /// <summary>Equivalent of <c>Duration.ofDays(long)</c>.</summary>
    public static TimeSpan OfDays(long days) => TimeSpan.FromTicks(checked(days * TimeSpan.TicksPerDay));

    /// <summary>Equivalent of <c>Duration.between(Instant, Instant)</c>.</summary>
    public static TimeSpan Between(DateTimeOffset? start, DateTimeOffset? end) => end!.Value - start!.Value;

    /// <summary>Equivalent of <c>Duration.ofMillis(long)</c>.</summary>
    public static TimeSpan DurationOfMillis(long millis) => TimeSpan.FromTicks(millis * TimeSpan.TicksPerMillisecond);

    /// <summary>Equivalent of <c>Duration.toMillis()</c>: whole milliseconds, rounded toward negative infinity.</summary>
    public static long ToMillis(TimeSpan duration) => JavaMath.FloorDiv(duration.Ticks, TimeSpan.TicksPerMillisecond);

    /// <summary>Equivalent of <c>Instant.toEpochMilli()</c>: whole milliseconds, rounded toward negative infinity.</summary>
    public static long ToEpochMilli(DateTimeOffset time) => time.ToUnixTimeMilliseconds();

    /// <summary>Midnight UTC at the start of <paramref name="date"/>.</summary>
    public static DateTimeOffset AtStartOfDayUtc(DateOnly date) =>
        new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified), TimeSpan.Zero);

    /// <summary>
    /// Equivalent of <c>ZonedDateTime.of(date, time, zone)</c>: the instant at which the clocks in <paramref name="zone"/> show
    /// <paramref name="date"/> and <paramref name="time"/>, returned with that zone's offset. Like <c>java.time</c>, a local time
    /// in a DST gap is moved forward by the length of the gap, and an ambiguous local time in an overlap uses the earlier
    /// (summer) offset.
    /// </summary>
    public static DateTimeOffset AtZone(DateOnly date, TimeOnly time, TimeZoneInfo zone)
    {
        DateTime local = date.ToDateTime(time, DateTimeKind.Unspecified);
        if (zone.IsInvalidTime(local))
        {
            // java.time: "the local date-time is adjusted to be later by the length of the gap"
            TimeSpan before = zone.GetUtcOffset(local.AddHours(-12));
            DateTimeOffset utc = new DateTimeOffset(local, before).ToUniversalTime();
            TimeSpan after = zone.GetUtcOffset(utc.UtcDateTime);
            return new DateTimeOffset(local + (after - before), after);
        }
        if (zone.IsAmbiguousTime(local))
        {
            TimeSpan[] offsets = zone.GetAmbiguousTimeOffsets(local);
            TimeSpan earlier = offsets.Max();
            return new DateTimeOffset(local, earlier);
        }
        return new DateTimeOffset(local, zone.GetUtcOffset(local));
    }

    /// <summary>Returns <paramref name="time"/> expressed in <paramref name="zone"/>'s offset at that instant.</summary>
    public static DateTimeOffset InZone(DateTimeOffset time, TimeZoneInfo zone) => TimeZoneInfo.ConvertTime(time, zone);

    /// <summary>Equivalent of <c>LocalDate.now(zone)</c>.</summary>
    public static DateOnly Today(TimeZoneInfo zone) => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, zone).DateTime);

    /// <summary>The calendar date of <paramref name="time"/> in <paramref name="zone"/>.</summary>
    public static DateOnly LocalDateIn(DateTimeOffset time, TimeZoneInfo zone) => DateOnly.FromDateTime(InZone(time, zone).DateTime);

    /// <summary>Equivalent of <c>LocalDate.ofEpochDay(long)</c>.</summary>
    public static DateOnly DateOfEpochDay(long epochDay) => DateOnly.FromDayNumber(checked((int)(epochDay + 719162)));

    /// <summary>Equivalent of <c>LocalDate.toEpochDay()</c>.</summary>
    public static long ToEpochDay(DateOnly date) => date.DayNumber - 719162L;

    /// <summary>Equivalent of <c>Duration.dividedBy(long)</c> (truncates toward zero).</summary>
    public static TimeSpan Divide(TimeSpan duration, long divisor) => TimeSpan.FromTicks(duration.Ticks / divisor);

    /// <summary>Equivalent of <c>Duration.multipliedBy(long)</c>.</summary>
    public static TimeSpan Multiply(TimeSpan duration, long multiplier) => TimeSpan.FromTicks(checked(duration.Ticks * multiplier));
    /// <summary>Equivalent of Java's <c>String.valueOf(Object)</c>: <c>"null"</c> for a null reference.</summary>
    public static string NullToString(string? value) => value ?? "null";

    /// <summary>Equivalent of <c>Instant.toString()</c> (ISO-8601 in UTC, for example <c>2026-11-05T12:34:56.789Z</c>).</summary>
    public static string InstantToString(DateTimeOffset? time)
    {
        if (time is not DateTimeOffset t)
        {
            return "null";
        }
        DateTimeOffset utc = t.ToUniversalTime();
        var sb = new System.Text.StringBuilder(utc.ToString("yyyy-MM-dd'T'HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
        AppendFraction(sb, GetNano(utc));
        return sb.Append('Z').ToString();
    }

    /// <summary>Equivalent of <c>Duration.toString()</c> (ISO-8601 seconds based, for example <c>PT1H6M41.7S</c>).</summary>
    public static string DurationToString(TimeSpan? duration)
    {
        if (duration is not TimeSpan d)
        {
            return "null";
        }
        if (d == TimeSpan.Zero)
        {
            return "PT0S";
        }
        long totalNanos = d.Ticks * NanosPerTick;
        long effectiveTotalSecs = JavaMath.FloorDiv(totalNanos, 1_000_000_000L);
        int nanos = (int)JavaMath.FloorMod(totalNanos, 1_000_000_000L);
        if (effectiveTotalSecs < 0 && nanos > 0)
        {
            effectiveTotalSecs++;
        }
        long hours = effectiveTotalSecs / 3600;
        int minutes = (int)((effectiveTotalSecs % 3600) / 60);
        int secs = (int)(effectiveTotalSecs % 60);
        var buf = new System.Text.StringBuilder("PT");
        if (hours != 0)
        {
            buf.Append(hours).Append('H');
        }
        if (minutes != 0)
        {
            buf.Append(minutes).Append('M');
        }
        if (secs == 0 && nanos == 0 && buf.Length > 2)
        {
            return buf.ToString();
        }
        long seconds = JavaMath.FloorDiv(totalNanos, 1_000_000_000L);
        if (seconds < 0 && nanos > 0)
        {
            if (secs == 0)
            {
                buf.Append("-0");
            }
            else
            {
                buf.Append(secs);
            }
        }
        else
        {
            buf.Append(secs);
        }
        if (nanos > 0)
        {
            int pos = buf.Length;
            if (seconds < 0)
            {
                buf.Append(2 * 1_000_000_000L - nanos);
            }
            else
            {
                buf.Append(nanos + 1_000_000_000L);
            }
            while (buf[buf.Length - 1] == '0')
            {
                buf.Length--;
            }
            buf[pos] = '.';
        }
        buf.Append('S');
        return buf.ToString();
    }

    /// <summary>Appends a fraction of a second the way <c>DateTimeFormatter.ISO_INSTANT</c> does (3, 6 or 9 digits).</summary>
    private static void AppendFraction(System.Text.StringBuilder sb, int nano)
    {
        if (nano == 0)
        {
            return;
        }
        if (nano % 1_000_000 == 0)
        {
            sb.Append('.').Append((nano / 1_000_000).ToString("000", System.Globalization.CultureInfo.InvariantCulture));
        }
        else if (nano % 1000 == 0)
        {
            sb.Append('.').Append((nano / 1000).ToString("000000", System.Globalization.CultureInfo.InvariantCulture));
        }
        else
        {
            sb.Append('.').Append(nano.ToString("000000000", System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
