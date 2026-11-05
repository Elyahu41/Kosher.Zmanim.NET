/*
 * Zmanim Java API
 * Copyright © 2004-2026 Eliyahu Hershfeld
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; version 2.1 of the License.
 *
 * This library is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not, write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA,
 * or connect to: https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html
 */
using Kosher.Zmanim;
using Kosher.Zmanim.HebrewCalendar;
using Kosher.Zmanim.Util;
using Xunit;
using static Kosher.Zmanim.Tests.JUnitAssert;

namespace Kosher.Zmanim.Tests.Util;

/// <summary>
/// Coverage for <see cref="GeoLocation"/>. The geodesic (Vincenty) and rhumb-line results are a <b>Tier-A independently
/// verifiable</b> anchor: the coordinates are the classic Ordnance Survey reference points (Cornwall and Scotland). The
/// rest is regression / behavioral coverage of construction, validation, the local-mean-time / antimeridian helpers, and
/// <c>equals</c>/<c>hashCode</c>/<c>clone</c>/<c>toXML</c>.
/// </summary>
public class GeoLocationTest
{

    private static readonly TimeZoneInfo Ny = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
    private static readonly DateTimeOffset Jan12017 = DateTimeOffset.Parse("2017-01-01T00:00:00Z");

    [Fact]
    public void ConstructWithDefaults()
    {
        GeoLocation geoLocation = new GeoLocation();
        AssertEquals(51.4772, geoLocation.Latitude, 0);
        AssertEquals(0.0, geoLocation.Longitude, 0);
        AssertEquals("GMT", geoLocation.TimeZone.Id);
        AssertEquals(0.0, geoLocation.Elevation, 0);
        AssertEquals("Greenwich, England", geoLocation.LocationName);
    }

    [Fact]
    public void ConstructWithData()
    {
        GeoLocation geoLocation = new GeoLocation("Lakewood, NJ", 40.0828, -74.2094, 20, Ny);
        AssertEquals(40.0828, geoLocation.Latitude, 0);
        AssertEquals(-74.2094, geoLocation.Longitude, 0);
        AssertEquals(Ny, geoLocation.TimeZone);
        AssertEquals(20.0, geoLocation.Elevation, 0);
        AssertEquals("Lakewood, NJ", geoLocation.LocationName);
    }

    [Fact]
    public void SettersUpdateValues()
    {
        GeoLocation geoLocation = new GeoLocation();
        geoLocation.Latitude = 41.1181036;
        geoLocation.Longitude = -74.2094;
        geoLocation.TimeZone = Ny;
        geoLocation.Elevation = 20;
        geoLocation.LocationName = "Lakewood, NJ";

        AssertEquals(41.1181036, geoLocation.Latitude, 0);
        AssertEquals(-74.2094, geoLocation.Longitude, 0);
        AssertEquals(Ny, geoLocation.TimeZone);
        AssertEquals(20.0, geoLocation.Elevation, 0);
        AssertEquals("Lakewood, NJ", geoLocation.LocationName);
    }

    [Fact]
    public void SetLatitudeRejectsOutOfRange()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new GeoLocation().Latitude = 90.1;
        });
    }

    [Fact]
    public void SetLatitudeRejectsNaN()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new GeoLocation().Latitude = double.NaN;
        });
    }

    [Fact]
    public void SetLongitudeRejectsOutOfRange()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new GeoLocation().Longitude = -180.1;
        });
    }

    [Fact]
    public void SetLongitudeRejectsNaN()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new GeoLocation().Longitude = double.NaN;
        });
    }

    [Fact]
    public void SetElevationRejectsNegative()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new GeoLocation().Elevation = -1;
        });
    }

    [Fact]
    public void SetElevationRejectsInfinite()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new GeoLocation().Elevation = double.PositiveInfinity;
        });
    }

    [Fact]
    public void ConstructorValidatesLatitude()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new GeoLocation("bad", 100, 0, TimeZoneInfo.FindSystemTimeZoneById("GMT"));
        });
    }

    [Fact]
    public void GetLocalMeanTimeOffset()
    {
        AssertEquals(0L, new GeoLocation().GetLocalMeanTimeOffset(Jan12017));

        GeoLocation lakewood = new GeoLocation("Lakewood, NJ", 40.0828, -74.2094, 20, Ny);
        AssertEquals(189744L, lakewood.GetLocalMeanTimeOffset(Jan12017));
    }

    [Fact]
    public void AntimeridianAdjustment()
    {
        AssertEquals(0, new GeoLocation().GetAntimeridianAdjustment(Jan12017));

        GeoLocation ny = new GeoLocation("Lakewood, NJ", 40.0828, -74.2094, 20, Ny);
        AssertEquals(0, ny.GetAntimeridianAdjustment(Jan12017));

        // Apia, Samoa (UTC+14 in January) is well east of its longitude-implied zone: roll the date back a day.
        GeoLocation apia = new GeoLocation("Apia, Samoa", -13.8599098, -171.8031745, 1858, TimeZoneInfo.FindSystemTimeZoneById("Pacific/Apia"));
        AssertEquals(-1, apia.GetAntimeridianAdjustment(Jan12017));

        // A location east of the dateline forced onto a UTC-12 zone: roll the date forward a day.
        GeoLocation westOfLine = new GeoLocation("GMT +12", -13.8599098, 179, 0, TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT+12"));
        AssertEquals(1, westOfLine.GetAntimeridianAdjustment(Jan12017));
    }

    /*
     * Tier-A: Vincenty and rhumb-line results against the classic Ordnance Survey reference points.
     */
    [Fact]
    public void VincentyFormulae()
    {
        GeoLocation pointA = new GeoLocation("A", 50.06632222222222, -5.71475, TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT+12"));
        GeoLocation pointB = new GeoLocation("B", 58.64402222222222, -3.0700944444444445, TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT+12"));

        AssertEquals(9.14186191, pointA.GetGeodesicInitialBearing(pointB), 1e-8);
        AssertEquals(11.29720127, pointA.GetGeodesicFinalBearing(pointB), 1e-8);
        AssertEquals(969954.11445043, pointA.GetGeodesicDistance(pointB), 1e-5);
    }

    [Fact]
    public void GeodesicDistanceForCoincidentPointsIsZero()
    {
        GeoLocation point = new GeoLocation("Lakewood, NJ", 40.0828, -74.2094, Ny);
        AssertEquals(0.0, point.GetGeodesicDistance(point), 0);
    }

    [Fact]
    public void RhumbLine()
    {
        GeoLocation pointA = new GeoLocation("A", 50.06638888888889, -5.714722222222222, TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT+12"));
        GeoLocation pointB = new GeoLocation("B", 58.64388888888889, -3.07, TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT+12"));

        AssertEquals(10.14069288, pointA.GetRhumbLineBearing(pointB), 1e-8);
        AssertEquals(969995.8368008, pointA.GetRhumbLineDistance(pointB), 1e-5);
    }

    [Fact]
    public void EqualsAndHashCode()
    {
        GeoLocation a = new GeoLocation("Lakewood, NJ", 40.0828, -74.2094, 20, Ny);
        GeoLocation b = new GeoLocation("Lakewood, NJ", 40.0828, -74.2094, 20, Ny);
        AssertEquals(a, b);
        AssertEquals(a.GetHashCode(), b.GetHashCode());

        GeoLocation c = new GeoLocation("Lakewood, NJ", 41.0, -74.2094, 20, Ny);
        AssertNotEquals(a, c);
    }

    [Fact]
    public void CloneProducesIndependentCopy()
    {
        GeoLocation geoLocation = new GeoLocation("Lakewood, NJ", 40.0828, -74.2094, 20, Ny);
        GeoLocation copy = (GeoLocation)geoLocation.Clone();

        AssertNotSame(geoLocation, copy);
        AssertEquals(geoLocation, copy);

        copy.Latitude = 10;
        AssertEquals(40.0828, geoLocation.Latitude, 0);
        AssertEquals(10.0, copy.Latitude, 0);
    }

    [Fact]
    public void ToXmlContainsFields()
    {
        GeoLocation geoLocation = new GeoLocation("Lakewood, NJ", 40.0828, -74.2094, 20, Ny);
        string xml = geoLocation.ToXml();
        AssertTrue(xml.Contains("<LocationName>Lakewood, NJ</LocationName>"));
        AssertTrue(xml.Contains("<Latitude>40.0828</Latitude>"));
        AssertTrue(xml.Contains("<Longitude>-74.2094</Longitude>"));
        AssertTrue(xml.Contains("<Elevation>20.0 Meters</Elevation>"));
        AssertTrue(xml.Contains("<TimezoneName>America/New_York</TimezoneName>"));
    }
}
