/*
 * Copyright (c) 2011. Jay R. Gindin
 */

using Kosher.Zmanim;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;
using static Kosher.Zmanim.Tests.JUnitAssert;

namespace Kosher.Zmanim.Tests.HebrewCalendar;

/// <summary>
/// Checks that we can roll forward &amp; backward the gregorian dates...
/// </summary>
public class UT_GregorianDateNavigation
{

    [Fact]
    public void GregorianForwardMonthToMonth()
    {

        DateOnly localDate = new DateOnly(2011, 1, 31);

        JewishDate hebrewDate = new JewishDate(localDate);
        AssertEquals(5771, hebrewDate.JewishYear);
        AssertEquals(11, hebrewDate.JewishMonth);
        AssertEquals(26, hebrewDate.JewishDayOfMonth);

        hebrewDate.PlusDays(1);
        AssertEquals(2, hebrewDate.LocalDate.Month);
        AssertEquals(1, hebrewDate.LocalDate.Day);
        AssertEquals(11, hebrewDate.JewishMonth);
        AssertEquals(27, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2011, 2, 28);
        hebrewDate = new JewishDate(localDate);
        AssertEquals(2, hebrewDate.LocalDate.Month);
        AssertEquals(28, hebrewDate.LocalDate.Day);
        AssertEquals(12, hebrewDate.JewishMonth);
        AssertEquals(24, hebrewDate.JewishDayOfMonth);

        hebrewDate.PlusDays(1);
        AssertEquals(3, hebrewDate.LocalDate.Month);
        AssertEquals(1, hebrewDate.LocalDate.Day);
        AssertEquals(12, hebrewDate.JewishMonth);
        AssertEquals(25, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2011, 3, 31);
        hebrewDate = new JewishDate(localDate);
        hebrewDate.PlusDays(1);
        AssertEquals(4, hebrewDate.LocalDate.Month);
        AssertEquals(1, hebrewDate.LocalDate.Day);
        AssertEquals(13, hebrewDate.JewishMonth);
        AssertEquals(26, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2011, 4, 30);
        hebrewDate = new JewishDate(localDate);
        hebrewDate.PlusDays(1);
        AssertEquals(5, hebrewDate.LocalDate.Month);
        AssertEquals(1, hebrewDate.LocalDate.Day);
        AssertEquals(1, hebrewDate.JewishMonth);
        AssertEquals(27, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2011, 5, 31);
        hebrewDate = new JewishDate(localDate);
        hebrewDate.PlusDays(1);
        AssertEquals(6, hebrewDate.LocalDate.Month);
        AssertEquals(1, hebrewDate.LocalDate.Day);
        AssertEquals(2, hebrewDate.JewishMonth);
        AssertEquals(28, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2011, 6, 30);
        hebrewDate = new JewishDate(localDate);
        hebrewDate.PlusDays(1);
        AssertEquals(7, hebrewDate.LocalDate.Month);
        AssertEquals(1, hebrewDate.LocalDate.Day);
        AssertEquals(3, hebrewDate.JewishMonth);
        AssertEquals(29, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2011, 7, 31);
        hebrewDate = new JewishDate(localDate);
        hebrewDate.PlusDays(1);
        AssertEquals(8, hebrewDate.LocalDate.Month);
        AssertEquals(1, hebrewDate.LocalDate.Day);
        AssertEquals(5, hebrewDate.JewishMonth);
        AssertEquals(1, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2011, 8, 31);
        hebrewDate = new JewishDate(localDate);
        hebrewDate.PlusDays(1);
        AssertEquals(9, hebrewDate.LocalDate.Month);
        AssertEquals(1, hebrewDate.LocalDate.Day);
        AssertEquals(6, hebrewDate.JewishMonth);
        AssertEquals(2, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2011, 9, 30);
        hebrewDate = new JewishDate(localDate);
        hebrewDate.PlusDays(1);
        AssertEquals(10, hebrewDate.LocalDate.Month);
        AssertEquals(1, hebrewDate.LocalDate.Day);
        AssertEquals(7, hebrewDate.JewishMonth);
        AssertEquals(3, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2011, 10, 31);
        hebrewDate = new JewishDate(localDate);
        hebrewDate.PlusDays(1);
        AssertEquals(11, hebrewDate.LocalDate.Month);
        AssertEquals(1, hebrewDate.LocalDate.Day);
        AssertEquals(5772, hebrewDate.JewishYear);
        AssertEquals(8, hebrewDate.JewishMonth);
        AssertEquals(4, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2011, 11, 30);
        hebrewDate = new JewishDate(localDate);
        hebrewDate.PlusDays(1);
        AssertEquals(12, hebrewDate.LocalDate.Month);
        AssertEquals(1, hebrewDate.LocalDate.Day);
        AssertEquals(9, hebrewDate.JewishMonth);
        AssertEquals(5, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2011, 12, 31);
        hebrewDate = new JewishDate(localDate);
        hebrewDate.PlusDays(1);
        AssertEquals(2012, hebrewDate.LocalDate.Year);
        AssertEquals(1, hebrewDate.LocalDate.Month);
        AssertEquals(1, hebrewDate.LocalDate.Day);
        AssertEquals(10, hebrewDate.JewishMonth);
        AssertEquals(6, hebrewDate.JewishDayOfMonth);
    }


    [Fact]
    public void GregorianBackwardMonthToMonth()
    {

        DateOnly localDate = new DateOnly(2011, 1, 1);
        JewishDate hebrewDate = new JewishDate(localDate);
        hebrewDate.MinusDays(1);
        AssertEquals(2010, hebrewDate.LocalDate.Year);
        AssertEquals(12, hebrewDate.LocalDate.Month);
        AssertEquals(31, hebrewDate.LocalDate.Day);
        AssertEquals(10, hebrewDate.JewishMonth);
        AssertEquals(24, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2010, 12, 1);
        hebrewDate.SetGregorianDate(localDate);
        hebrewDate.MinusDays(1);
        AssertEquals(11, hebrewDate.LocalDate.Month);
        AssertEquals(30, hebrewDate.LocalDate.Day);
        AssertEquals(9, hebrewDate.JewishMonth);
        AssertEquals(23, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2010, 11, 1);
        hebrewDate.SetGregorianDate(localDate);
        hebrewDate.MinusDays(1);
        AssertEquals(10, hebrewDate.LocalDate.Month);
        AssertEquals(31, hebrewDate.LocalDate.Day);
        AssertEquals(8, hebrewDate.JewishMonth);
        AssertEquals(23, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2010, 10, 1);
        hebrewDate.SetGregorianDate(localDate);
        hebrewDate.MinusDays(1);
        AssertEquals(9, hebrewDate.LocalDate.Month);
        AssertEquals(30, hebrewDate.LocalDate.Day);
        AssertEquals(7, hebrewDate.JewishMonth);
        AssertEquals(22, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2010, 9, 1);
        hebrewDate.SetGregorianDate(localDate);
        hebrewDate.MinusDays(1);
        AssertEquals(8, hebrewDate.LocalDate.Month);
        AssertEquals(31, hebrewDate.LocalDate.Day);
        AssertEquals(5770, hebrewDate.JewishYear);
        AssertEquals(6, hebrewDate.JewishMonth);
        AssertEquals(21, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2010, 8, 1);
        hebrewDate.SetGregorianDate(localDate);
        hebrewDate.MinusDays(1);
        AssertEquals(7, hebrewDate.LocalDate.Month);
        AssertEquals(31, hebrewDate.LocalDate.Day);
        AssertEquals(5, hebrewDate.JewishMonth);
        AssertEquals(20, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2010, 7, 1);
        hebrewDate.SetGregorianDate(localDate);
        hebrewDate.MinusDays(1);
        AssertEquals(6, hebrewDate.LocalDate.Month);
        AssertEquals(30, hebrewDate.LocalDate.Day);
        AssertEquals(4, hebrewDate.JewishMonth);
        AssertEquals(18, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2010, 6, 1);
        hebrewDate.SetGregorianDate(localDate);
        hebrewDate.MinusDays(1);
        AssertEquals(5, hebrewDate.LocalDate.Month);
        AssertEquals(31, hebrewDate.LocalDate.Day);
        AssertEquals(3, hebrewDate.JewishMonth);
        AssertEquals(18, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2010, 5, 1);
        hebrewDate.SetGregorianDate(localDate);
        hebrewDate.MinusDays(1);
        AssertEquals(4, hebrewDate.LocalDate.Month);
        AssertEquals(30, hebrewDate.LocalDate.Day);
        AssertEquals(2, hebrewDate.JewishMonth);
        AssertEquals(16, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2010, 4, 1);
        hebrewDate.SetGregorianDate(localDate);
        hebrewDate.MinusDays(1);
        AssertEquals(3, hebrewDate.LocalDate.Month);
        AssertEquals(31, hebrewDate.LocalDate.Day);
        AssertEquals(1, hebrewDate.JewishMonth);
        AssertEquals(16, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2010, 3, 1);
        hebrewDate.SetGregorianDate(localDate);
        hebrewDate.MinusDays(1);
        AssertEquals(2, hebrewDate.LocalDate.Month);
        AssertEquals(28, hebrewDate.LocalDate.Day);
        AssertEquals(12, hebrewDate.JewishMonth);
        AssertEquals(14, hebrewDate.JewishDayOfMonth);

        localDate = new DateOnly(2010, 2, 1);
        hebrewDate.SetGregorianDate(localDate);
        hebrewDate.MinusDays(1);
        AssertEquals(1, hebrewDate.LocalDate.Month);
        AssertEquals(31, hebrewDate.LocalDate.Day);
        AssertEquals(11, hebrewDate.JewishMonth);
        AssertEquals(16, hebrewDate.JewishDayOfMonth);

    }



} // End of UT_GregorianDateNavigation class
