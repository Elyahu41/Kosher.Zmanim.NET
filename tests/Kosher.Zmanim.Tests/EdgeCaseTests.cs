/*
 * Zmanim .NET API
 * Copyright © 2004-2026 Eliyahu Hershfeld
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; version 2.1 of the License.
 */
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;

namespace Kosher.Zmanim.Tests;

/// <summary>
/// Edge cases where KosherJava 3.1.0 misbehaves (fixed here and proposed upstream), the candle lighting change, and the
/// methods added from KosherSwift.
/// </summary>
public class EdgeCaseTests
{
    private static readonly GeoLocation Lakewood5017m =
        new("Lakewood, NJ", 40.0721087, -74.2400243, 5017, TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));

    private static readonly DateOnly Date = new(2026, 1, 1);

    private static double Timed(Func<double> calculation)
    {
        var task = Task.Run(calculation);
        Assert.True(task.Wait(TimeSpan.FromSeconds(10)), "the calculation didn't finish in 10 seconds");
        return task.Result;
    }

    [Fact]
    public void SpaReturnsNaNPromptlyForANaNTargetZenith()
    {
        Assert.True(double.IsNaN(Timed(() => new SPACalculator().GetUTCSunrise(Date, Lakewood5017m, double.PositiveInfinity, false))));
        Assert.True(double.IsNaN(Timed(() => new SPACalculator().GetUTCSunset(Date, Lakewood5017m, double.NaN, false))));
        Assert.True(double.IsNaN(Timed(() => new SPACalculator { EarthRadius = -6371 }.GetUTCSunrise(Date, Lakewood5017m, 90, true))));
        Assert.True(double.IsNaN(Timed(() => new SPACalculator { Refraction = double.NaN }.GetUTCSunset(Date, Lakewood5017m, 90, false))));
        Assert.True(double.IsNaN(Timed(() => new SPACalculator { SolarRadius = double.PositiveInfinity }.GetUTCSunrise(Date, Lakewood5017m, 90, false))));
    }

    private const string SpecialName = "Ta\"l & <Ma'alot> \\ x";

    [Fact]
    public void XmlOutputIsWellFormedWithSpecialCharacters()
    {
        var geo = new GeoLocation(SpecialName, 40.0828, -74.2094, TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
        var calendar = new ComprehensiveZmanimCalendar(geo) { LocalDate = Date };
        XDocument xml = XDocument.Parse(ZmanimFormatter.ToXml(calendar));
        Assert.Equal(SpecialName, xml.Root!.Attribute("location")!.Value);
        Assert.Equal(SpecialName, XDocument.Parse(geo.ToXml()).Root!.Element("LocationName")!.Value);

        var zman = new Zman(calendar.GetSunrise(), geo, "Alos & <Tzais>") { Description = "16.1° \"degrees\"" };
        XDocument zmanXml = XDocument.Parse(zman.ToXml());
        Assert.Equal("Alos & <Tzais>", zmanXml.Root!.Element("Label")!.Value);
    }

    [Fact]
    public void JsonOutputIsValidWithSpecialCharacters()
    {
        var geo = new GeoLocation(SpecialName + "\n", 40.0828, -74.2094, TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
        using JsonDocument json = JsonDocument.Parse(ZmanimFormatter.ToJson(new ComprehensiveZmanimCalendar(geo) { LocalDate = Date }));
        Assert.Equal(SpecialName + "\n", json.RootElement.GetProperty("metadata").GetProperty("location").GetString());
    }

    [Fact]
    public void NegativeDurationsAreValidXsdDurations()
    {
        var formatter = new ZmanimFormatter(TimeZoneInfo.Utc);
        Assert.Equal("PT1H30M", formatter.FormatXsdDurationTime(TimeSpan.FromMinutes(90)));
        Assert.Equal("-PT1H15M3S", formatter.FormatXsdDurationTime(TimeSpan.FromMinutes(-75) - TimeSpan.FromSeconds(3)));
        Assert.Equal("-PT0.5S", formatter.FormatXsdDurationTime(TimeSpan.FromMilliseconds(-500)));
        TimeSpan negative = -(TimeSpan.FromHours(1) + TimeSpan.FromMinutes(15) + TimeSpan.FromMilliseconds(250));
        Assert.Equal(negative, XmlConvert.ToTimeSpan(formatter.FormatXsdDurationTime(negative)));
        formatter.TimeFormat = ZmanimFormatter.XsdDurationFormat;
        Assert.Equal("-PT30M", formatter.Format(TimeSpan.FromMinutes(-30)));
    }

    [Fact]
    public void HebrewNumbersAbove9999()
    {
        var formatter = new HebrewDateFormatter();
        Assert.Equal("ה׳ אלפים", formatter.FormatHebrewNumber(5000));
        Assert.Equal("תשפ״ו", formatter.FormatHebrewNumber(5786));
        Assert.Equal("י׳ אלפים", formatter.FormatHebrewNumber(10_000));
        Assert.Equal("שמ״ה", formatter.FormatHebrewNumber(12_345));
        formatter.UseLongHebrewYears = true;
        Assert.Equal("ה׳ תשפ״ו", formatter.FormatHebrewNumber(5786));
        Assert.Equal("ט׳ תתקצ״ט", formatter.FormatHebrewNumber(9999));
        Assert.Equal("יב׳ שמ״ה", formatter.FormatHebrewNumber(12_345));
        Assert.Equal("טו׳ א׳", formatter.FormatHebrewNumber(15_001));
        Assert.Equal("תתקצט׳ תתקצ״ט", formatter.FormatHebrewNumber(999_999));
        formatter.UseGershGershayim = false;
        Assert.Equal("יב שמה", formatter.FormatHebrewNumber(12_345));
        Assert.Throws<ArgumentException>(() => formatter.FormatHebrewNumber(1_000_000));

        var jewishCalendar = new JewishCalendar(new DateOnly(6500, 1, 1));
        Assert.Equal(10_260, jewishCalendar.JewishYear);
        Assert.EndsWith(" ר״ס", new HebrewDateFormatter { IsHebrewFormat = true }.Format(jewishCalendar));
    }

    [Fact]
    public void CandleLightingIsNeverBeforePlagHamincha()
    {
        // on a very short day near the Arctic Circle, 18 minutes before sunset is before plag hamincha
        var geo = new GeoLocation("Bodø area", 66.2, 14.4, TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo"));
        var calendar = new ComprehensiveZmanimCalendar(geo) { LocalDate = new DateOnly(2026, 12, 21) };
        DateTimeOffset plag = calendar.GetPlagHaminchaGRA()!.Value;
        DateTimeOffset sunsetMinusOffset = calendar.GetSeaLevelSunset()!.Value.AddMinutes(-calendar.CandleLightingOffset);
        Assert.True(sunsetMinusOffset < plag, "precondition: the test day must be short enough");
        Assert.Equal(plag, calendar.GetCandleLighting());
        Assert.Equal(plag.Offset, calendar.GetCandleLighting()!.Value.Offset);

        // on a normal day it is still the offset before sea level sunset
        var lakewood = new ComprehensiveZmanimCalendar(TestLocations.Lakewood()) { LocalDate = new DateOnly(2026, 12, 21) };
        Assert.Equal(lakewood.GetSeaLevelSunset()!.Value.AddMinutes(-18), lakewood.GetCandleLighting());
    }

    [Theory]
    [InlineData(5782, true)]
    [InlineData(5786, false)]
    [InlineData(5789, true)]
    [InlineData(5775, true)]
    public void IsShmitaYear(int year, bool expected)
    {
        Assert.Equal(expected, new JewishCalendar(year, JewishDate.Tishrei, 1).IsShmitaYear());
    }

    [Fact]
    public void IsRegularTaanisExcludesYomKippur()
    {
        var yomKippur = new JewishCalendar(5786, JewishDate.Tishrei, 10);
        Assert.True(yomKippur.IsTaanis());
        Assert.False(yomKippur.IsRegularTaanis());

        foreach (var (month, day) in new[] { (JewishDate.Tammuz, 17), (JewishDate.Av, 9), (JewishDate.Teves, 10) })
        {
            var fast = new JewishCalendar(5785, month, day);
            Assert.Equal(fast.IsTaanis(), fast.IsRegularTaanis());
        }
        Assert.True(new JewishCalendar(new DateOnly(2026, 3, 2)).IsRegularTaanis()); // Ta'anis Esther 5786
        Assert.False(new JewishCalendar(new DateOnly(2026, 3, 3)).IsRegularTaanis()); // Purim
    }
}
