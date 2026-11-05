using Kosher.Zmanim;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;
using static Kosher.Zmanim.Tests.JUnitAssert;

namespace Kosher.Zmanim.Tests.HebrewCalendar;

public class UT_YerushalmiTest
{
    private static readonly HebrewDateFormatter hdf = new HebrewDateFormatter { IsHebrewFormat = true };

    [Fact]
    public void TestCorrectDaf1()
    {
        JewishCalendar jewishCalendar = new JewishCalendar(5777, JewishCalendar.Elul, 10);
        AssertEquals(8, jewishCalendar.GetDafYomiYerushalmi().DafNumber);
        AssertEquals(29, jewishCalendar.GetDafYomiYerushalmi().MasechtaNumber);
    }

    [Fact]
    public void TestCorrectDaf2()
    {
        JewishCalendar jewishCalendar = new JewishCalendar(5744, JewishCalendar.Kislev, 1);
        AssertEquals(26, jewishCalendar.GetDafYomiYerushalmi().DafNumber);
        AssertEquals(32, jewishCalendar.GetDafYomiYerushalmi().MasechtaNumber);
    }

    [Fact]
    public void TestCorrectDaf3()
    {
        JewishCalendar jewishCalendar = new JewishCalendar(5782, JewishCalendar.Sivan, 1);
        AssertEquals(15, jewishCalendar.GetDafYomiYerushalmi().DafNumber);
        AssertEquals(33, jewishCalendar.GetDafYomiYerushalmi().MasechtaNumber);
    }

    [Fact]
    public void TestCorrectSpecialDate()
    {
        JewishCalendar jewishCalendar = new JewishCalendar(5775, JewishCalendar.Tishrei, 10);
        AssertNull(jewishCalendar.GetDafYomiYerushalmi());

        jewishCalendar = new JewishCalendar(5783, JewishCalendar.Av, 9);
        AssertNull(jewishCalendar.GetDafYomiYerushalmi());

        jewishCalendar = new JewishCalendar(5775, JewishCalendar.Av, 10);// 9 Av delayed to Sunday 10 Av
        AssertNull(jewishCalendar.GetDafYomiYerushalmi());
    }

    /// <summary>
    /// The cycle ending in 2172 is extended for Tisha B'Av 5932. It still has every daf, ending with Niddah 13 on
    /// 7 August 2172.
    /// </summary>
    [Fact]
    public void TestCycleExtendedForTishaBav()
    {
        AssertDaf(new DateOnly(2172, 8, 6), 38, 12);
        AssertDaf(new DateOnly(2172, 8, 7), 38, 13);
        AssertDaf(new DateOnly(2172, 8, 8), 0, 1);
    }

    /// <summary>
    /// Tisha B'Av 6240 falls the day before the last daf of a cycle. The cycle still has every daf, ending with
    /// Niddah 13 on 16 August 2480, and the next one starts with Berachos 1.
    /// </summary>
    [Fact]
    public void TestCycleWithTishaBavBeforeItsLastDaf()
    {
        AssertDaf(new DateOnly(2480, 8, 14), 38, 12);
        AssertNull(new JewishCalendar(new DateOnly(2480, 8, 15)).GetDafYomiYerushalmi());
        AssertDaf(new DateOnly(2480, 8, 16), 38, 13);
        AssertDaf(new DateOnly(2480, 8, 17), 0, 1);
    }

    private static void AssertDaf(DateOnly date, int masechtaNumber, int daf)
    {
        Daf dafYomi = new JewishCalendar(date).GetDafYomiYerushalmi();
        AssertEquals(date.ToString(), masechtaNumber, dafYomi.MasechtaNumber);
        AssertEquals(date.ToString(), daf, dafYomi.DafNumber);
    }

}
