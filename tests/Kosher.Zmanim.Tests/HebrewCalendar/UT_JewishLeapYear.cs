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
/// Verify correct calculations of when a Hebrew leap year occurs.
/// </summary>
public class UT_JewishLeapYear
{


    [Fact]
    public void IsLeapYear()
    {

        ShouldBeLeapYear(5160);
        ShouldNotBeLeapYear(5536);

        ShouldNotBeLeapYear(5770);
        ShouldBeLeapYear(5771);
        ShouldNotBeLeapYear(5772);
        ShouldNotBeLeapYear(5773);
        ShouldBeLeapYear(5774);
        ShouldNotBeLeapYear(5775);
        ShouldBeLeapYear(5776);
        ShouldNotBeLeapYear(5777);
        ShouldNotBeLeapYear(5778);
        ShouldBeLeapYear(5779);
        ShouldNotBeLeapYear(5780);
        ShouldNotBeLeapYear(5781);
        ShouldBeLeapYear(5782);
        ShouldNotBeLeapYear(5783);
        ShouldBeLeapYear(5784);
        ShouldNotBeLeapYear(5785);
        ShouldNotBeLeapYear(5786);
        ShouldBeLeapYear(5787);
        ShouldNotBeLeapYear(5788);
        ShouldNotBeLeapYear(5789);
        ShouldBeLeapYear(5790);
        ShouldNotBeLeapYear(5791);
        ShouldNotBeLeapYear(5792);
        ShouldBeLeapYear(5793);
        ShouldNotBeLeapYear(5794);
        ShouldBeLeapYear(5795);
    }


    private void ShouldBeLeapYear(int year)
    {
        JewishDate jewishDate = new JewishDate();
        jewishDate.JewishYear = year;

        AssertTrue(jewishDate.IsJewishLeapYear());
    }


    private void ShouldNotBeLeapYear(int year)
    {
        JewishDate jewishDate = new JewishDate();
        jewishDate.JewishYear = year;

        AssertFalse(jewishDate.IsJewishLeapYear());
    }

} // End of UT_JewishLeapYear class
