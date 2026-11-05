/*
 * Zmanim .NET API
 * Copyright © 2004-2026 Eliyahu Hershfeld
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; version 2.1 of the License.
 */
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;

namespace Kosher.Zmanim.Tests;

/// <summary>
/// The code samples from README.md, compiled and run so the documentation can't drift from the API.
/// </summary>
public class ReadmeSamplesTests
{
    [Fact]
    public void JewishCalendarSample()
    {
        var jewishCalendar = new JewishCalendar(); // by default it is set to today's date
        int month = jewishCalendar.JewishMonth;   // Nissan is 1. See the JewishDate month constants
        Assert.InRange(month, 1, 13);

        // To check for holidays / yamim tovim, use the built-in methods of the JewishCalendar:
        jewishCalendar = new JewishCalendar(5786, JewishDate.Nissan, 15);
        Assert.True(jewishCalendar.IsPesach());
        // or check it yourself with GetYomTovIndex() just like in KosherJava:
        Assert.Equal(JewishCalendar.Pesach, jewishCalendar.GetYomTovIndex());
    }

    [Fact]
    public void SunriseSample()
    {
        var geoLocation = new GeoLocation("Lakewood, NJ", 40.08213, -74.20970,
            TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
        var cal = new AstronomicalCalendar(geoLocation);
        // To use another calculator:
        cal.AstronomicalCalculator = new SPACalculator();
        DateTimeOffset? sunrise = cal.GetSunrise(); // null if the sun does not rise that day
        Assert.NotNull(sunrise);
        Assert.Equal(geoLocation.TimeZone.GetUtcOffset(sunrise!.Value), sunrise.Value.Offset);
    }

    [Fact]
    public void ZmanimSample()
    {
        var geoLocation = new GeoLocation("Lakewood, NJ", 40.08213, -74.20970,
            TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
        var czc = new ComprehensiveZmanimCalendar(geoLocation)
        {
            LocalDate = new DateOnly(2026, 1, 1),
        };
        DateTimeOffset? sofZmanShmaGra = czc.GetSofZmanShmaGRA();
        TimeSpan? shaahZmanis = czc.GetShaahZmanisGRA();
        Assert.True(sofZmanShmaGra > czc.GetSunrise());
        Assert.InRange(shaahZmanis!.Value, TimeSpan.FromMinutes(45), TimeSpan.FromMinutes(50));
        // with elevation
        var withElevation = new GeoLocation("Jerusalem", 31.778, 35.2354, 754,
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Jerusalem"));
        var jerusalem = new ComprehensiveZmanimCalendar(withElevation) { UseElevation = true };
        Assert.True(jerusalem.GetSunrise() < jerusalem.GetSeaLevelSunrise());
    }

    [Fact]
    public void FormatterAndTefilaRulesSample()
    {
        var jewishCalendar = new JewishCalendar(5786, JewishDate.Nissan, 15);
        var hebrewDateFormatter = new HebrewDateFormatter();
        Assert.Equal("Nissan", hebrewDateFormatter.FormatMonth(jewishCalendar));
        Assert.Equal("15 Nissan, 5786", hebrewDateFormatter.Format(jewishCalendar));
        hebrewDateFormatter.IsHebrewFormat = true;
        Assert.Equal("ניסן", hebrewDateFormatter.FormatMonth(jewishCalendar));
        Assert.Equal("ט״ו ניסן תשפ״ו", hebrewDateFormatter.Format(jewishCalendar));

        var tefilaRules = new TefilaRules();
        Assert.False(tefilaRules.IsVeseinTalUmatarRecited(jewishCalendar));
        Assert.True(tefilaRules.IsHallelShalemRecited(jewishCalendar));
    }

    [Fact]
    public void ZmanAndFormatterSample()
    {
        DateTimeOffset early = DateTimeOffset.Now;
        DateTimeOffset later = early.AddHours(1);
        var list = new List<Zman> { new Zman(later, "Later"), new Zman(early, "Earlier") };
        List<Zman> sorted = list.OrderBy(z => z, Zman.DateOrder).ToList();
        Assert.Equal("Earlier", sorted[0].Label);

        var formatter = new ZmanimFormatter(TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
        Assert.Equal("1:02:03", formatter.Format(new TimeSpan(1, 2, 3)));
    }

    [Fact]
    public void CustomZmanimSample()
    {
        var czc = new MyZmanimCalendar(TestLocations.Lakewood()) { LocalDate = new DateOnly(2026, 1, 1) };
        Assert.Equal(AstronomicalCalendar.GetTimeOffset(czc.GetSeaLevelSunset(), TimeSpan.FromMinutes(45)), czc.GetTzais45()); // UseElevation is false by default
        Assert.Equal(czc.GetSunsetOffsetByDegrees(AstronomicalCalendar.GeometricZenith + 8.7), czc.GetTzais8Point7Degrees());
        Assert.Equal(AstronomicalCalendar.GetTimeOffset(czc.GetSeaLevelSunrise(), czc.GetShaahZmanisGRA() * 2), czc.GetTwoSeasonalHoursIntoTheDay());
        Assert.Contains(czc.GetZmanim(), z => z.Label == "Tzais45"); // custom zmanim are picked up by GetZmanim() and ToXml()
    }
}

/// <summary>The "making your own zmanim" sample from README.md.</summary>
public class MyZmanimCalendar : ComprehensiveZmanimCalendar
{
    public MyZmanimCalendar(GeoLocation location) : base(location)
    {
    }

    // for regular minutes
    public DateTimeOffset? GetTzais45() =>
        ToZone(GetTimeOffset(GetSunsetBasedOnElevationSetting(), TimeSpan.FromMinutes(45)));

    // for degrees
    public DateTimeOffset? GetTzais8Point7Degrees() => GetSunsetOffsetByDegrees(GeometricZenith + 8.7);

    // for seasonal hours (GetTimeOffset handles null values)
    public DateTimeOffset? GetTwoSeasonalHoursIntoTheDay() =>
        ToZone(GetTimeOffset(GetSunriseBasedOnElevationSetting(), GetShaahZmanisGRA() * 2));
}
