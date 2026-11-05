using Kosher.Zmanim;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;
using static Kosher.Zmanim.Tests.JUnitAssert;

namespace Kosher.Zmanim.Tests.HebrewCalendar;

public class YomiCalculatorTest
{
    private static readonly HebrewDateFormatter hdf = new HebrewDateFormatter { IsHebrewFormat = true };

    [Fact]
    public void TestCorrectDaf1()
    {
        JewishCalendar jewishCalendar = new JewishCalendar(5685, JewishDate.Kislev, 12);
        Daf daf = YomiCalculator.GetDafYomiBavli(jewishCalendar);
        AssertEquals(5, daf.MasechtaNumber);
        AssertEquals(2, daf.DafNumber);
    }

    [Fact]
    public void TestCorrectDaf2()
    {
        JewishCalendar jewishCalendar = new JewishCalendar(5736, JewishDate.Elul, 26);
        Daf daf = YomiCalculator.GetDafYomiBavli(jewishCalendar);
        AssertEquals(4, daf.MasechtaNumber);
        AssertEquals(14, daf.DafNumber);
    }

    [Fact]
    public void TestCorrectDaf3()
    {
        JewishCalendar jewishCalendar = new JewishCalendar(5777, JewishDate.Elul, 10);
        Daf daf = YomiCalculator.GetDafYomiBavli(jewishCalendar);
        AssertEquals(23, daf.MasechtaNumber);
        AssertEquals(47, daf.DafNumber);
    }
}
