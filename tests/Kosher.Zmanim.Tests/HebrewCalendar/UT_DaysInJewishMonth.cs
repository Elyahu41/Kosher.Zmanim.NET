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
/// Validate the days in a Hebrew month (in various types of years) are correct.
/// </summary>
public class UT_DaysInJewishMonth
{

    [Fact]
    public void DaysInMonthsInHaserYear()
    {

        AssertHaser(5773);
        AssertHaser(5777);
        AssertHaser(5781);
        AssertHaserLeap(5784);
        AssertHaserLeap(5790);
        AssertHaserLeap(5793);
    }


    [Fact]
    public void DaysInMonthsInQesidrahYear()
    {

        AssertQesidrah(5769);
        AssertQesidrah(5772);
        AssertQesidrah(5778);
        AssertQesidrah(5786);
        AssertQesidrah(5789);
        AssertQesidrah(5792);

        AssertQesidrahLeap(5782);
    }


    [Fact]
    public void DaysInMonthsInShalemYear()
    {

        AssertShalem(5770);
        AssertShalem(5780);
        AssertShalem(5783);
        AssertShalem(5785);
        AssertShalem(5788);
        AssertShalem(5791);
        AssertShalem(5794);

        AssertShalemLeap(5771);
        AssertShalemLeap(5774);
        AssertShalemLeap(5776);
        AssertShalemLeap(5779);
        AssertShalemLeap(5787);
        AssertShalemLeap(5795);
    }


    private void AssertHaser(int year)
    {
        JewishDate jewishDate = new JewishDate();
        jewishDate.JewishYear = year;

        AssertFalse(jewishDate.IsCheshvanLong());
        AssertTrue(jewishDate.IsKislevShort());
    }


    private void AssertHaserLeap(int year)
    {
        JewishDate jewishDate = new JewishDate();
        jewishDate.JewishYear = year;

        AssertHaser(year);
        AssertTrue(jewishDate.IsJewishLeapYear());
    }


    private void AssertQesidrah(int year)
    {
        JewishDate jewishDate = new JewishDate();
        jewishDate.JewishYear = year;

        AssertFalse(jewishDate.IsCheshvanLong());
        AssertFalse(jewishDate.IsKislevShort());
    }


    private void AssertQesidrahLeap(int year)
    {
        JewishDate jewishDate = new JewishDate();
        jewishDate.JewishYear = year;

        AssertQesidrah(year);
        AssertTrue(jewishDate.IsJewishLeapYear());
    }


    private void AssertShalem(int year)
    {
        JewishDate jewishDate = new JewishDate();
        jewishDate.JewishYear = year;

        AssertTrue(jewishDate.IsCheshvanLong());
        AssertFalse(jewishDate.IsKislevShort());
    }


    private void AssertShalemLeap(int year)
    {
        JewishDate jewishDate = new JewishDate();
        jewishDate.JewishYear = year;

        AssertShalem(year);
        AssertTrue(jewishDate.IsJewishLeapYear());
    }

} // End of UT_DaysInJewishMonth class
