using Kosher.Zmanim.Tests.Util;

using Kosher.Zmanim;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;
using static Kosher.Zmanim.Tests.JUnitAssert;

namespace Kosher.Zmanim.Tests;

public class CalendarEqualityRegressionTest
{

    [Fact]
    public void AstronomicalCalendarCloneRemainsEqual()
    {
        AstronomicalCalendar astronomicalCalendar = new AstronomicalCalendar(
                new GeoLocation("Lakewood", 40.0828, -74.2094, TimeZoneInfo.FindSystemTimeZoneById("America/New_York")));
        astronomicalCalendar.AstronomicalCalculator.Refraction = 0.6;
        astronomicalCalendar.AstronomicalCalculator.SolarRadius = 0.3;
        astronomicalCalendar.AstronomicalCalculator.EarthRadius = 6400;

        AstronomicalCalendar clone = (AstronomicalCalendar)astronomicalCalendar.Clone();

        AssertEquals(astronomicalCalendar, clone);
        AssertEquals(astronomicalCalendar.GetHashCode(), clone.GetHashCode());
    }

    [Fact]
    public void ZmanimCalendarEqualityIncludesZmanimSettings()
    {
        ZmanimCalendar zmanimCalendar = new ZmanimCalendar(
                new GeoLocation("Lakewood", 40.0828, -74.2094, TimeZoneInfo.FindSystemTimeZoneById("America/New_York")));
        ZmanimCalendar clone = (ZmanimCalendar)zmanimCalendar.Clone();

        AssertEquals(zmanimCalendar, clone);
        AssertEquals(zmanimCalendar.GetHashCode(), clone.GetHashCode());

        clone.UseElevation = true;
        clone.UseAstronomicalChatzosForOtherZmanim = true;
        clone.CandleLightingOffset = 40;

        AssertFalse(zmanimCalendar.Equals(clone));
    }

    [Fact]
    public void ComprehensiveZmanimCalendarEqualityIncludesAteretTorahOffset()
    {
        ComprehensiveZmanimCalendar comprehensiveZmanimCalendar = new ComprehensiveZmanimCalendar(
                new GeoLocation("Lakewood", 40.0828, -74.2094, TimeZoneInfo.FindSystemTimeZoneById("America/New_York")));
        ComprehensiveZmanimCalendar clone = (ComprehensiveZmanimCalendar)comprehensiveZmanimCalendar.Clone();

        AssertEquals(comprehensiveZmanimCalendar, clone);
        AssertEquals(comprehensiveZmanimCalendar.GetHashCode(), clone.GetHashCode());

        clone.AteretTorahSunsetOffset = 30;

        AssertFalse(comprehensiveZmanimCalendar.Equals(clone));
    }
}
