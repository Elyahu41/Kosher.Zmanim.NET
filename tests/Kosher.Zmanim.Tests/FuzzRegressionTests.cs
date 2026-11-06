/*
 * Zmanim .NET API
 * Copyright © 2004-2026 Eliyahu Hershfeld
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; version 2.1 of the License.
 */
using Kosher.Zmanim.Internal;
using Kosher.Zmanim.Util;
using Xunit;

namespace Kosher.Zmanim.Tests;

/// <summary>
/// Regression tests for differences from KosherJava found by differential fuzzing. The expected values are KosherJava 3.1.0's.
/// </summary>
public class FuzzRegressionTests
{
    [Fact]
    public void NonFiniteOffsetsBehaveLikeJavaOnEveryRuntime()
    {
        // Java's (long) cast turns NaN into 0, so a NaN offset acts like 0. On .NET 8 a plain cast gave long.MinValue and a
        // sha'ah zmanis of -8,895 days.
        var geo = new GeoLocation("fuzz", 64.86939028837688, -68.26425927727126, 0.0, 87.30571064376669, 0.0,
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Yerevan"));
        var withNaN = new ComprehensiveZmanimCalendar(geo) { LocalDate = new DateOnly(1977, 5, 3), AteretTorahSunsetOffset = double.NaN };
        var withZero = new ComprehensiveZmanimCalendar(geo) { LocalDate = new DateOnly(1977, 5, 3), AteretTorahSunsetOffset = 0 };
        Assert.Equal(withZero.GetShaahZmanisAteretTorah(), withNaN.GetShaahZmanisAteretTorah());
        Assert.Equal(withZero.GetTzaisAteretTorah(), withNaN.GetTzaisAteretTorah());

        var lakewood = new ComprehensiveZmanimCalendar(TestLocations.Lakewood()) { LocalDate = new DateOnly(2026, 3, 1) };
        Assert.True(double.IsNaN(lakewood.GetSunsetSolarDipFromOffset(double.PositiveInfinity)));
    }

    [Theory]
    [InlineData(double.NaN, 0L)]
    [InlineData(double.PositiveInfinity, long.MaxValue)]
    [InlineData(double.NegativeInfinity, long.MinValue)]
    [InlineData(1e300, long.MaxValue)]
    [InlineData(-1e300, long.MinValue)]
    [InlineData(-2.9, -2L)]
    [InlineData(2.9, 2L)]
    public void ToLongHasJavaCastSemantics(double value, long expected)
    {
        Assert.Equal(expected, JavaMath.ToLong(value));
    }

    [Fact]
    public void SpaDeltaTWorksOutsideDateOnlyRange()
    {
        // the SPA search evaluated ΔT around 3100 BCE, which DateOnly can't represent, and threw ArgumentOutOfRangeException
        var geo = new GeoLocation("fuzz", -19.198649654799723, 20.610354245908866, 371.0443998373833, 0.0, 842.2685869709694,
            TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT-14"));
        var calendar = new ComprehensiveZmanimCalendar(geo)
        {
            AstronomicalCalculator = new SPACalculator { ApplyDeltaT = true, Refraction = 0.322662021764052 },
            LocalDate = new DateOnly(2076, 2, 29),
            AteretTorahSunsetOffset = 163.4156146159324,
        };
        Assert.Equal(4.855873940748249, calendar.GetPercentOfShaahZmanisFromDegrees(118.79076477718678, true), 1e-9);
    }

    [Theory]
    [InlineData(-719468L, 0L, 3)]   // 0000-03-01
    [InlineData(-719469L, 0L, 2)]   // 0000-02-29 (year 0 is a leap year)
    [InlineData(-719528L, 0L, 1)]   // 0000-01-01
    [InlineData(-1853196L, -3104L, 2)]
    [InlineData(2932896L, 9999L, 12)] // 9999-12-31
    public void YearMonthOfEpochDayMatchesJavaTime(long epochDay, long year, int month)
    {
        Assert.Equal((year, month), JavaTime.YearMonthOfEpochDay(epochDay));
    }

    [Fact]
    public void YearMonthOfEpochDayMatchesDateOnly()
    {
        for (int dayNumber = DateOnly.MinValue.DayNumber; dayNumber <= DateOnly.MaxValue.DayNumber; dayNumber += 97)
        {
            DateOnly date = DateOnly.FromDayNumber(dayNumber);
            Assert.Equal(((long)date.Year, date.Month), JavaTime.YearMonthOfEpochDay(dayNumber - 719162L));
        }
    }

    [Fact]
    public void SunTransitBetweenTimesIsInTheLocationsZone()
    {
        var calendar = new ComprehensiveZmanimCalendar(TestLocations.Lakewood()) { LocalDate = new DateOnly(2026, 7, 1) };
        DateTimeOffset sunrise = calendar.GetSeaLevelSunrise()!.Value;
        DateTimeOffset sunset = calendar.GetSeaLevelSunset()!.Value;
        DateTimeOffset? transit = calendar.GetSunTransit(sunrise.ToOffset(TimeSpan.FromHours(-11)), sunset.ToOffset(TimeSpan.FromHours(13)));
        Assert.Equal(calendar.GetSunTransit(sunrise, sunset), transit);
        Assert.Equal(TimeSpan.FromHours(-4), transit!.Value.Offset); // EDT
        Assert.Equal(transit, calendar.GetChatzos(sunrise, sunset));
    }
}
