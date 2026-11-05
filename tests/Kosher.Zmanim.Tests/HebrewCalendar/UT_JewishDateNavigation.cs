/*
 * Copyright (c) 2011. Jay R. Gindin
 */

using Kosher.Zmanim;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;
using static Kosher.Zmanim.Tests.JUnitAssert;

namespace Kosher.Zmanim.Tests.HebrewCalendar;

///
public class UT_JewishDateNavigation
{


    [Fact]
    public void JewishForwardMonthToMonth()
    {

        JewishDate jewishDate = new JewishDate();
        jewishDate.SetJewishDate(5771, 1, 1);
        DateOnly localDate = jewishDate.LocalDate;
        AssertEquals(5, localDate.Day);
        AssertEquals(4, localDate.Month);
        AssertEquals(2011, localDate.Year);
    }


    [Fact]
    public void ComputeRoshHashana5771()
    {

        // At one point, this test was failing as the JewishDate class spun through a never-ending loop...

        JewishDate jewishDate = new JewishDate();
        jewishDate.SetJewishDate(5771, 7, 1);
        DateOnly localDate = jewishDate.LocalDate;
        AssertEquals(9, localDate.Day);
        AssertEquals(9, localDate.Month);
        AssertEquals(2010, localDate.Year);
    }

    [Fact]
    public void AddYearsUsesConfiguredAdarMonthWhenMovingToLeapYear()
    {

        JewishDate adarAlephDate = new JewishDate();
        adarAlephDate.SetJewishDate(5783, JewishDate.Adar, 10);
        adarAlephDate.PlusYears(1, true);
        AssertEquals(5784, adarAlephDate.JewishYear);
        AssertEquals(JewishDate.Adar, adarAlephDate.JewishMonth);
        AssertEquals(10, adarAlephDate.JewishDayOfMonth);

        JewishDate adarBeisDate = new JewishDate();
        adarBeisDate.SetJewishDate(5783, JewishDate.Adar, 10);
        adarBeisDate.PlusYears(1, false);
        AssertEquals(5784, adarBeisDate.JewishYear);
        AssertEquals(JewishDate.AdarII, adarBeisDate.JewishMonth);
        AssertEquals(10, adarBeisDate.JewishDayOfMonth);
    }

    /// <summary>
    /// The molad of Teves 3761 is Tuesday 12 December of Gregorian year 0, which is a leap year.
    /// </summary>
    [Fact]
    public void MoladInGregorianYear0()
    {
        JewishDate molad = new JewishDate(3761, JewishDate.Teves, 20).GetMolad();
        // KosherJava returns LocalDate.of(0, 12, 12). DateOnly can't represent Gregorian year 0, so check the absolute date
        // of December 12, year 0 (proleptic Gregorian) instead, and that LocalDate reports the limitation clearly.
        AssertEquals(-19, molad.AbsDate);
        Assert.Throws<ArgumentOutOfRangeException>(() => molad.LocalDate);
        AssertEquals(3, molad.DayOfWeek); // java.util.Calendar.TUESDAY
        AssertEquals(20, molad.MoladHours);
        AssertEquals(7, molad.MoladMinutes);
        AssertEquals(2, molad.MoladChalakim);
    }


} // End of UT_JewishDateNavigation class
