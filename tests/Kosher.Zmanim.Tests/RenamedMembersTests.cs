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
/// Members renamed to .NET names keep their KosherJava names as <see cref="ObsoleteAttribute"/> aliases. These tests make sure
/// the aliases still return the same results as the new names.
/// </summary>
public class RenamedMembersTests
{
    [Fact]
    public void MoladAndTekufaAliases()
    {
        var jewishCalendar = new JewishCalendar(new DateOnly(2026, 4, 7)); // a day with a tekufa (Nissan 5786)
        Assert.NotNull(jewishCalendar.GetMoladAsDateTimeOffset());
        Assert.Equal(jewishCalendar.GetMoladAsDateTimeOffset(), jewishCalendar.GetMoladAsInstant());
        Assert.Equal(jewishCalendar.GetTekufaAsDateTimeOffset(false), jewishCalendar.GetTekufaAsInstant(false));
        Assert.Equal(jewishCalendar.GetTekufaAsDateTimeOffset(true), jewishCalendar.GetTekufaAsInstant(true));
    }

    [Fact]
    public void ZmanimFormatterAliases()
    {
        TimeZoneInfo zone = TestLocations.Lakewood().TimeZone;
        var formatter = new ZmanimFormatter(zone);
        var time = new DateTimeOffset(2026, 1, 1, 12, 7, 8, TimeSpan.Zero);
        Assert.Equal("7:07:08", formatter.Format(time));
        Assert.Equal(formatter.Format(time, zone), formatter.FormatInstant(time, zone));
        Assert.Equal(formatter.FormatXsdDateTime(time), formatter.FormatXSDateTime(time));
        Assert.Equal("PT1H30M", formatter.FormatXsdDurationTime(TimeSpan.FromMinutes(90)));
        Assert.Equal("PT1H30M", formatter.FormatXSDDurationTime(TimeSpan.FromMinutes(90)));

        Assert.Equal(ZmanimFormatter.SexagesimalSecondsFormat, formatter.TimeFormat);
        formatter.TimeFormat = ZmanimFormatter.SexagesimalFormat;
        Assert.Equal("1:30", formatter.Format(TimeSpan.FromMinutes(90)));
        formatter.SetTimeFormat(ZmanimFormatter.XsdDurationFormat);
        Assert.Equal(ZmanimFormatter.XsdDurationFormat, formatter.TimeFormat);
        Assert.Throws<ArgumentException>(() => formatter.TimeFormat = 99);
    }

    [Fact]
    public void TransliterationProperties()
    {
        var hebrewDateFormatter = new HebrewDateFormatter();
        Assert.Same(hebrewDateFormatter.TransliteratedParshiyosList, hebrewDateFormatter.GetTransliteratedParshiyosList());
        Assert.Equal("Bereshis", hebrewDateFormatter.TransliteratedParshiyosList[JewishCalendar.Parshah.Bereshis]);

        Assert.Same(Daf.YerushalmiMasechtosTransliterated, Daf.GetYerushalmiMasechtosTransliterated());
        Assert.Same(Daf.YerushalmiMasechtos, Daf.GetYerushalmiMasechtos());
        Assert.Equal("Berachos", Daf.BavliMasechtosTransliterated[0]);

        string[] original = Daf.BavliMasechtosTransliterated;
        try
        {
            string[] sephardi = (string[])original.Clone();
            sephardi[0] = "Berakhot";
            Daf.BavliMasechtosTransliterated = sephardi;
            Assert.Equal("Berakhot", new Daf(0, 2).GetMasechtaTransliterated());
        }
        finally
        {
            Daf.BavliMasechtosTransliterated = original;
        }
    }
}
