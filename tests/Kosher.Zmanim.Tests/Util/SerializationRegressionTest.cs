using Kosher.Zmanim.Tests;

using Kosher.Zmanim;
using Kosher.Zmanim.Internal;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;
using static Kosher.Zmanim.Tests.JUnitAssert;

namespace Kosher.Zmanim.Tests.Util;

public class SerializationRegressionTest
{

    [Fact]
    public void ZmanToXmlSerializesInstantUsingGeoLocationZone()
    {
        GeoLocation geoLocation = new GeoLocation("Lakewood", 40.0828, -74.2094, TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
        Zman zman = new Zman(DateTimeOffset.Parse("2026-03-23T10:56:01.123Z"), geoLocation, "Sunrise");

        string xml = zman.ToXml();

        AssertTrue(xml.Contains("<Zman>2026-03-23T06:56:01.123</Zman>"));
    }

    [Fact]
    public void ZmanWithoutGeoLocationStillSerializesAndFormats()
    {
        Zman zman = new Zman(DateTimeOffset.Parse("2026-03-23T10:56:01.123Z"), "Sunrise");

        string xml = zman.ToXml();
        string text = zman.ToString();

        AssertTrue(xml.Contains("<Zman>2026-03-23T10:56:01.123</Zman>"));
        AssertTrue(text.Contains("GeoLocation:\tnull"));
    }

    [Fact]
    public void AstronomicalCalendarPreservesSubSecondPrecision()
    {
        AstronomicalCalendar astronomicalCalendar = new AstronomicalCalendar(
                new GeoLocation("Lakewood", 40.0828, -74.2094, TimeZoneInfo.FindSystemTimeZoneById("America/New_York")));

        double utcSunrise = astronomicalCalendar.GetUTCSunrise(AstronomicalCalendar.GeometricZenith);
        long expectedEpochMillis = JavaMath.Round(utcSunrise * 3600000);
        long actualEpochMillis = astronomicalCalendar.GetSunrise()!.Value.ToUnixTimeMilliseconds()
                % (24 * 3600000);

        AssertTrue(expectedEpochMillis % 1000 != 0);
        AssertFalse(actualEpochMillis % 1000 == 0);
        AssertTrue(Math.Abs(expectedEpochMillis - actualEpochMillis) < 1000);
    }
}
