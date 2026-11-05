/*
 * Zmanim .NET API
 * Copyright © 2004-2026 Eliyahu Hershfeld
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; version 2.1 of the License.
 */
using System.Globalization;
using System.Text.RegularExpressions;
using Xunit;

namespace Kosher.Zmanim.Tests;

/// <summary>
/// Helpers for comparing this port's <see cref="DateTimeOffset"/> / <see cref="TimeSpan"/> results with the nanosecond
/// <c>java.time</c> values hard-coded in the KosherJava tests.
/// </summary>
/// <remarks>
/// .NET times have a resolution of one 100 ns tick, so a KosherJava value such as <c>11:09:11.571783718Z</c> can't be
/// represented exactly. Values are compared within <see cref="ToleranceNanos"/>; see the golden-file tests for the measured
/// agreement with KosherJava.
/// </remarks>
internal static class TestTime
{
    /// <summary>Allowed difference between a KosherJava nanosecond value and the .NET result.</summary>
    public const long ToleranceNanos = 2_000;

    /// <summary>Parses an ISO-8601 instant (<c>Instant.parse</c> format, up to 9 fraction digits) to epoch nanoseconds.</summary>
    public static long ParseEpochNanos(string iso)
    {
        Match m = Regex.Match(iso, @"^(.*T\d\d:\d\d(?::\d\d)?)(?:\.(\d{1,9}))?(Z|[+-]\d\d:\d\d)$");
        if (!m.Success)
        {
            throw new FormatException("Not an ISO instant: " + iso);
        }
        var whole = DateTimeOffset.Parse(m.Groups[1].Value + (m.Groups[3].Value == "Z" ? "+00:00" : m.Groups[3].Value),
            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        long nanos = (whole.UtcTicks - DateTimeOffset.UnixEpoch.UtcTicks) * 100;
        if (m.Groups[2].Success)
        {
            nanos += long.Parse(m.Groups[2].Value.PadRight(9, '0'), CultureInfo.InvariantCulture);
        }
        return nanos;
    }

    /// <summary>Parses an ISO-8601 instant, truncating to a whole tick.</summary>
    public static DateTimeOffset ParseInstant(string iso) => FromEpochNanos(ParseEpochNanos(iso));

    /// <summary>Converts epoch nanoseconds to a UTC <see cref="DateTimeOffset"/> (floor to a tick).</summary>
    public static DateTimeOffset FromEpochNanos(long nanos) =>
        new DateTimeOffset(DateTimeOffset.UnixEpoch.UtcTicks + Math.DivRem(nanos, 100, out long rem) - (rem < 0 ? 1 : 0), TimeSpan.Zero);

    /// <summary>Epoch nanoseconds of a <see cref="DateTimeOffset"/>.</summary>
    public static long EpochNanos(DateTimeOffset time) => (time.UtcTicks - DateTimeOffset.UnixEpoch.UtcTicks) * 100;

    /// <summary>Parses a <c>java.time.Duration</c> ISO-8601 string such as <c>PT55M20.649853956S</c> to nanoseconds.</summary>
    public static long ParseDurationNanos(string iso)
    {
        Match m = Regex.Match(iso, @"^(-)?PT(?:(-?\d+)H)?(?:(-?\d+)M)?(?:(-?\d+)(?:\.(\d{1,9}))?S)?$");
        if (!m.Success)
        {
            throw new FormatException("Not an ISO duration: " + iso);
        }
        long nanos = 0;
        if (m.Groups[2].Success) nanos += long.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture) * 3_600_000_000_000L;
        if (m.Groups[3].Success) nanos += long.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture) * 60_000_000_000L;
        if (m.Groups[4].Success)
        {
            long seconds = long.Parse(m.Groups[4].Value, CultureInfo.InvariantCulture);
            long fraction = m.Groups[5].Success ? long.Parse(m.Groups[5].Value.PadRight(9, '0'), CultureInfo.InvariantCulture) : 0;
            nanos += seconds * 1_000_000_000L + (m.Groups[4].Value.StartsWith('-') ? -fraction : fraction);
        }
        return m.Groups[1].Success ? -nanos : nanos;
    }

    /// <summary>Parses a <c>java.time.Duration</c> ISO-8601 string, truncating to a whole tick.</summary>
    public static TimeSpan ParseDuration(string iso) => TimeSpan.FromTicks(ParseDurationNanos(iso) / 100);

    /// <summary>Asserts that <paramref name="actual"/> is the instant <paramref name="expectedIso"/> (within the tolerance).</summary>
    public static void AssertInstant(string label, string expectedIso, DateTimeOffset? actual)
    {
        Assert.True(actual.HasValue, $"{label}: expected {expectedIso} but was null");
        long diff = Math.Abs(EpochNanos(actual!.Value) - ParseEpochNanos(expectedIso));
        Assert.True(diff <= ToleranceNanos, $"{label}: expected {expectedIso} but was {actual.Value.UtcDateTime:O} ({diff} ns off)");
    }

    /// <summary>Asserts that <paramref name="actual"/> is the duration <paramref name="expectedIso"/> (within the tolerance).</summary>
    public static void AssertDuration(string label, string expectedIso, TimeSpan? actual)
    {
        Assert.True(actual.HasValue, $"{label}: expected {expectedIso} but was null");
        long diff = Math.Abs(actual!.Value.Ticks * 100 - ParseDurationNanos(expectedIso));
        Assert.True(diff <= ToleranceNanos, $"{label}: expected {expectedIso} but was {actual} ({diff} ns off)");
    }

    /// <summary>Asserts two .NET times are the same instant (within the tolerance), or both missing.</summary>
    public static void AssertSameInstant(string label, DateTimeOffset? expected, DateTimeOffset? actual)
    {
        Assert.True(expected.HasValue == actual.HasValue, $"{label}: expected {expected} but was {actual}");
        if (expected.HasValue)
        {
            long diff = Math.Abs(EpochNanos(expected.Value) - EpochNanos(actual!.Value));
            Assert.True(diff <= ToleranceNanos, $"{label}: expected {expected.Value:O} but was {actual.Value:O}");
        }
    }

    /// <summary>A fixed-offset time zone, the equivalent of Java's <c>ZoneOffset.ofHours(hours)</c>.</summary>
    public static TimeZoneInfo FixedOffset(int hours) => hours == 0
        ? TimeZoneInfo.Utc
        : TimeZoneInfo.CreateCustomTimeZone("UTC" + hours.ToString("+00;-00", CultureInfo.InvariantCulture), TimeSpan.FromHours(hours),
            "UTC" + hours.ToString("+00;-00", CultureInfo.InvariantCulture), "UTC" + hours.ToString("+00;-00", CultureInfo.InvariantCulture));

    /// <summary>Looks up an IANA time zone.</summary>
    public static TimeZoneInfo Zone(string id) => TimeZoneInfo.FindSystemTimeZoneById(id);
}
