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
/// Tests for the LINQ-friendly additions of the .NET port (<see cref="CalendarExtensions"/>,
/// <see cref="JewishCalendarExtensions"/> and the <see cref="JewishDate"/> comparison operators).
/// </summary>
public class LinqExtensionsTests
{
    [Fact]
    public void EachDayYieldsIndependentCopiesForEachDate()
    {
        var czc = new ComprehensiveZmanimCalendar(TestLocations.Lakewood()) { LocalDate = new DateOnly(2026, 1, 1) };
        czc.CandleLightingOffset = 40;
        List<ComprehensiveZmanimCalendar> days = czc.EachDay(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 31)).ToList();

        Assert.Equal(31, days.Count);
        Assert.Equal(new DateOnly(2026, 1, 1), czc.LocalDate); // the source calendar is not changed
        for (int i = 0; i < days.Count; i++)
        {
            Assert.Equal(new DateOnly(2026, 3, 1).AddDays(i), days[i].LocalDate);
            Assert.Equal(40, days[i].CandleLightingOffset);
            Assert.NotSame(czc.GeoLocation, days[i].GeoLocation);
            var expected = new ComprehensiveZmanimCalendar(TestLocations.Lakewood()) { LocalDate = days[i].LocalDate };
            Assert.Equal(expected.GetSunrise(), days[i].GetSunrise());
        }
        // buffering and re-querying is safe because each element is its own calendar
        Assert.Equal(days.Select(d => d.GetSunset()), days.Select(d => d.GetSunset()).ToList());
    }

    [Fact]
    public void EachDayHandlesEmptyAndInvalidRanges()
    {
        var calendar = new AstronomicalCalendar(TestLocations.Lakewood());
        Assert.Empty(calendar.EachDay(new DateOnly(2026, 3, 2), new DateOnly(2026, 3, 1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => calendar.EachDay(new DateOnly(2026, 3, 1), -1));
    }

    [Fact]
    public void LinqQueriesOverDaysWork()
    {
        var calendar = new ZmanimCalendar(TestLocations.Lakewood());
        TimeSpan? latestSunset = calendar.EachDay(new DateOnly(2026, 1, 1), 365).Max(day => day.GetSunset()?.TimeOfDay);
        Assert.NotNull(latestSunset);
        // polar location: count the days without a sunrise in December
        var norway = new AstronomicalCalendar(TestLocations.Norway(TestTime.Zone("Europe/Oslo")));
        int polarNights = norway.EachDay(new DateOnly(2026, 12, 1), 31).Count(day => day.GetSunrise() == null);
        Assert.InRange(polarNights, 20, 31);
    }

    [Fact]
    public void GetZmanimReturnsEveryZmanWithItsLabel()
    {
        var czc = new ComprehensiveZmanimCalendar(TestLocations.Lakewood()) { LocalDate = new DateOnly(2017, 10, 17) };
        List<Zman> zmanim = czc.GetZmanim().ToList();

        Zman sunrise = Assert.Single(zmanim, z => z.Label == "Sunrise");
        Assert.Equal(czc.GetSunrise(), sunrise.Time);
        Assert.Same(czc.GeoLocation, sunrise.GeoLocation);
        Zman shaah = Assert.Single(zmanim, z => z.Label == "ShaahZmanisGRA");
        Assert.Equal(czc.GetShaahZmanisGRA(), shaah.Duration);
        // the zmanim KosherJava deprecates as lechumra-only are skipped
        Assert.DoesNotContain(zmanim, z => z.Label == "PlagHamincha120Minutes");
        Assert.Contains(zmanim, z => z.Label == "SofZmanShmaAlos16Point1ToSunset");
        Assert.True(zmanim.Count > 150);

        List<Zman> ordered = zmanim.Where(z => z.Time != null).OrderBy(z => z, Zman.DateOrder).ToList();
        for (int i = 1; i < ordered.Count; i++)
        {
            Assert.True(ordered[i - 1].Time <= ordered[i].Time);
        }
    }

    [Fact]
    public void JewishCalendarEachDayCopiesSettings()
    {
        var jc = new JewishCalendar(5787, JewishDate.Tishrei, 1) { InIsrael = true, UseModernHolidays = true };
        List<JewishCalendar> days = jc.EachDay(30).ToList();

        Assert.Equal(30, days.Count);
        Assert.Equal(1, jc.JewishDayOfMonth); // the source is not changed
        Assert.All(days, d => Assert.True(d.InIsrael));
        Assert.Equal(Enumerable.Range(1, 30), days.Select(d => d.JewishDayOfMonth));
        // in Israel Shemini Atzeres / Simchas Torah is on 22 Tishrei and Isru Chag on the 23rd
        Assert.Equal(JewishCalendar.SheminiAtzeres, days[21].GetYomTovIndex());
        Assert.Equal(JewishCalendar.IsruChag, days[22].GetYomTovIndex());
    }

    [Fact]
    public void JewishCalendarEachDayUntilGregorianDate()
    {
        var jc = new JewishCalendar(new DateOnly(2026, 1, 1));
        Assert.Equal(365, jc.EachDay(new DateOnly(2026, 12, 31)).Count());
        Assert.Equal(new DateOnly(2026, 12, 31), jc.EachDay(new DateOnly(2026, 12, 31)).Last().LocalDate);
    }

    [Fact]
    public void JewishDateComparisonOperatorsAndSorting()
    {
        var a = new JewishDate(5786, JewishDate.Nissan, 15);
        var b = new JewishDate(5786, JewishDate.Iyar, 1);
        Assert.True(a < b);
        Assert.True(b > a);
        Assert.True(a <= new JewishDate(5786, JewishDate.Nissan, 15));
        Assert.True(a >= new JewishDate(5786, JewishDate.Nissan, 15));
        Assert.True(a.Equals(new JewishDate(5786, JewishDate.Nissan, 15)));
        Assert.Equal(new[] { a, b }, new[] { b, a }.OrderBy(d => d).ToArray());
        Assert.Single(new[] { a, new JewishDate(5786, JewishDate.Nissan, 15) }.Distinct());
    }
}
