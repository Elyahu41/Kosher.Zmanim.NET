/*
 * Zmanim .NET API
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
using System.Globalization;
using System.Text;

namespace Kosher.Zmanim.Util;

/// <summary>
/// A wrapper class for astronomical times / <em>zmanim</em> that is mostly intended to allow sorting collections of astronomical times.
/// It has fields for both date/time and duration based <em>zmanim</em>, name / labels as well as a longer description or explanation
/// of a <em>zman</em>.
/// <para>Here is an example of various ways of sorting <em>zmanim</em>.</para>
/// <para>First create the calendar for the location you would like to calculate:</para>
/// <code>
/// var location = new GeoLocation("Lakewood, NJ", 40.0828, -74.2094, 20,
///     TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
/// var czc = new ComprehensiveZmanimCalendar(location);
/// var sunset = new Zman(czc.GetSunset(), "Sunset");
/// var shaah16 = new Zman(czc.GetShaahZmanis16Point1Degrees(), "Shaah zmanis 16.1");
/// var sunrise = new Zman(czc.GetSunrise(), "Sunrise");
/// var shaah = new Zman(czc.GetShaahZmanisGRA(), "Shaah zmanis GRA");
/// var zl = new List&lt;Zman&gt; { sunset, shaah16, sunrise, shaah };
/// // will sort sunrise, sunset, shaah, shaah 1.6 (the last 2 are not in any specific order)
/// zl.Sort(Zman.DateOrder);
/// // will sort sunrise, sunset (the first 2 are not in any specific order), shaah GRA, shaah 1.6
/// zl.Sort(Zman.DurationOrder);
/// // will sort shaah 1.6, shaah GRA, sunrise, sunset
/// zl.Sort(Zman.NameOrder);
/// // or with LINQ
/// var ordered = zl.OrderBy(z => z, Zman.DateOrder);
/// </code>
/// </summary>
/// <remarks>
/// <b>To do:</b> Add secondary sorting. As of now the comparers in this class do not sort by secondary order. This means that when
/// sorting a collection of <em>zmanim</em> and using the <see cref="DateOrder"/> comparer will have the duration based
/// <em>zmanim</em> at the end, but they will not be sorted by duration. This should be N/A for label based sorting.
/// </remarks>
public class Zman
{
    /// <summary>
    /// The constructor setting a <see cref="DateTimeOffset"/> based <em>zman</em> and a label. In most cases you will likely want to call
    /// <see cref="Zman(DateTimeOffset?, GeoLocation?, string?)"/> that also sets the location.
    /// </summary>
    /// <param name="instant">the <see cref="DateTimeOffset"/> of the <em>zman</em>.</param>
    /// <param name="label">the label of the <em>zman</em> such as "<em>Sof Zman Krias Shema GRA</em>".</param>
    public Zman(DateTimeOffset? instant, string? label)
        : this(instant, null, label)
    {
    }

    /// <summary>
    /// The constructor setting a <see cref="DateTimeOffset"/> based <em>zman</em> and a label. In most cases you will likely want to call
    /// this constructor that also sets the geo location.
    /// </summary>
    /// <param name="instant">the <see cref="DateTimeOffset"/> of the <em>zman</em>.</param>
    /// <param name="geoLocation">the <see cref="Util.GeoLocation"/> of the <em>zman</em>.</param>
    /// <param name="label">the label of the  <em>zman</em> such as "<em>Sof Zman Krias Shema GRA</em>".</param>
    public Zman(DateTimeOffset? instant, GeoLocation? geoLocation, string? label)
    {
        Time = instant;
        GeoLocation = geoLocation;
        Label = label;
    }

    /// <summary>
    /// The constructor setting a duration based <em>zman</em> such as
    /// <see cref="AstronomicalCalendar.GetTemporalHour()">temporal hour</see> (or the various <em>shaah zmanis</em> times such as
    /// <see cref="ZmanimCalendar.GetShaahZmanisGRA"><em>shaah zmanis GRA</em></see> or
    /// <see cref="ComprehensiveZmanimCalendar.GetShaahZmanis16Point1Degrees"><em>shaah Zmanis 16.1°</em></see>) and label.
    /// </summary>
    /// <param name="duration">a duration based <em>zman</em> such as (<see cref="AstronomicalCalendar.GetTemporalHour()"/></param>
    /// <param name="label">the label of the  <em>zman</em> such as "<em>Shaah Zmanis GRA</em>".</param>
    public Zman(TimeSpan? duration, string? label)
    {
        Label = label;
        Duration = duration;
    }

    /// <summary>
    /// The <see cref="DateTimeOffset"/> of the <em>zman</em> (<c>getZman()</c> in KosherJava).
    /// </summary>
    public DateTimeOffset? Time { get; set; }

    /// <summary>
    /// The location information of the <em>zman</em>.
    /// </summary>
    public GeoLocation? GeoLocation { get; set; }

    /// <summary>
    /// The duration if the <em>zman</em> is  a <see cref="AstronomicalCalendar.GetTemporalHour()">temporal hour</see> (or the various
    /// <em>shaah zmanis</em> base times such as <see cref="ZmanimCalendar.GetShaahZmanisGRA"><em>shaah Zmanis GRA</em></see> or
    /// <see cref="ComprehensiveZmanimCalendar.GetShaahZmanis16Point1Degrees"><em>shaah Zmanis 16.1°</em></see>).
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// The name / label of the <em>zman</em>, such as "<em>Sof Zman Krias Shema GRA</em>".
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// A longer description or explanation of a <em>zman</em>. This would be used to explain the <em>zman</em>, for example the
    /// <em>zman</em> for <em>tzais</em> would explain that it is the time when three medium stars are visible.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// An <see cref="IComparer{T}"/> that will sort <em>zmanim</em> by date/time order. Compares two <em>zmanim</em> by their
    /// <see cref="Time"/>. If both <em>zmanim</em> have a <see cref="Time"/>, they are compared by that. If only one has a
    /// <see cref="Time"/>, it sorts before the other. If neither has a <see cref="Time"/>, they are compared by <see cref="Duration"/>
    /// using the same logic. <c>null</c> <see cref="Zman"/> objects sort last.
    /// </summary>
    public static IComparer<Zman?> DateOrder { get; } = Comparer<Zman?>.Create((z1, z2) =>
    {
        if (ReferenceEquals(z1, z2))
        {
            return 0;
        }
        if (z1 is null)
        {
            return 1;
        }
        if (z2 is null)
        {
            return -1;
        }
        int result = CompareNullableFirst(z1.Time, z2.Time);
        return result != 2 ? result : (CompareNullableFirst(z1.Duration, z2.Duration) is int r && r != 2 ? r : 0);
    });

    /// <summary>
    /// An <see cref="IComparer{T}"/> that will sort <em>zmanim</em> by label order (ordinal comparison, like Java's natural
    /// <c>string</c> ordinal ordering). <c>null</c> <see cref="Zman"/> objects and <c>null</c> labels sort first.
    /// </summary>
    public static IComparer<Zman?> NameOrder { get; } = Comparer<Zman?>.Create((z1, z2) =>
    {
        if (ReferenceEquals(z1, z2))
        {
            return 0;
        }
        if (z1 is null)
        {
            return -1;
        }
        if (z2 is null)
        {
            return 1;
        }
        return string.CompareOrdinal(z1.Label, z2.Label);
    });

    /// <summary>
    /// An <see cref="IComparer{T}"/> that will sort <em>zmanim</em> by duration order. Compares two <em>zmanim</em> by their
    /// <see cref="Duration"/>. If both <em>zmanim</em> have a <see cref="Duration"/>, they are compared by that. If only one has a
    /// <see cref="Duration"/>, it sorts before the other. If neither has a <see cref="Duration"/>, they are compared by
    /// <see cref="Time"/> using the same logic. <c>null</c> <see cref="Zman"/> objects sort last.
    /// </summary>
    public static IComparer<Zman?> DurationOrder { get; } = Comparer<Zman?>.Create((z1, z2) =>
    {
        if (ReferenceEquals(z1, z2))
        {
            return 0;
        }
        if (z1 is null)
        {
            return 1;
        }
        if (z2 is null)
        {
            return -1;
        }
        int result = CompareNullableFirst(z1.Duration, z2.Duration);
        return result != 2 ? result : (CompareNullableFirst(z1.Time, z2.Time) is int r && r != 2 ? r : 0);
    });

    /// <summary>
    /// Compares two nullable values where a present value sorts before a missing one. Returns <c>2</c> when both are missing.
    /// </summary>
    private static int CompareNullableFirst<T>(T? a, T? b) where T : struct, IComparable<T>
    {
        if (a.HasValue && b.HasValue)
        {
            return Math.Sign(a.Value.CompareTo(b.Value));
        }
        if (a.HasValue)
        {
            return -1;
        }
        if (b.HasValue)
        {
            return 1;
        }
        return 2;
    }

    /// <summary>
    /// A method that returns an XML formatted <see cref="string"/> representing the serialized <see cref="Zman"/>. Very similar to
    /// <see cref="ToString"/> but the return value is in an XML format. The format currently used (subject to change) is:
    /// <code>
    /// &lt;Zman&gt;
    ///  &lt;Label&gt;Sof Zman Krias Shema GRA&lt;/Label&gt;
    ///  &lt;Zman&gt;1969-02-08T09:37:56.820&lt;/Zman&gt;
    ///  &lt;GeoLocation&gt;
    ///     &lt;LocationName&gt;Lakewood, NJ&lt;/LocationName&gt;
    ///     &lt;Latitude&gt;40.0828&lt;/Latitude&gt;
    ///     &lt;Longitude&gt;-74.2094&lt;/Longitude&gt;
    ///     &lt;Elevation&gt;0.0 Meters&lt;/Elevation&gt;
    ///     &lt;TimezoneName&gt;America/New_York&lt;/TimezoneName&gt;
    ///     &lt;TimeZoneDisplayName&gt;Eastern Standard Time&lt;/TimeZoneDisplayName&gt;
    ///  &lt;/GeoLocation&gt;
    ///  &lt;Duration&gt;PT1H6M41.7S&lt;/Duration&gt;
    ///  &lt;Description&gt;Sof Zman Krias Shema GRA is 3 sha'os zmaniyos calculated from sunrise to sunset.&lt;/Description&gt;
    /// &lt;/Zman&gt;
    /// </code>
    /// </summary>
    /// <returns>The XML formatted <see cref="string"/>.</returns>
    public string ToXml()
    {
        TimeZoneInfo zone = GeoLocation == null ? TimeZoneInfo.Utc : GeoLocation.TimeZone;
        var sb = new StringBuilder();
        sb.Append("<Zman>\n");
        sb.Append("\t<Label>").Append(ZmanimFormatter.EscapeXml(Label)).Append("</Label>\n");
        sb.Append("\t<Zman>").Append(Time is DateTimeOffset t
            ? JavaTime.InZone(t, zone).ToString("yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.InvariantCulture) : string.Empty).Append("</Zman>\n");
        if (GeoLocation != null)
        {
            sb.Append('\t').Append(GeoLocation.ToXml().Replace("\n", "\n\t"));
        }
        sb.Append("\n\t<Duration>").Append(JavaTime.DurationToString(Duration)).Append("</Duration>\n");
        sb.Append("\t<Description>").Append(ZmanimFormatter.EscapeXml(Description)).Append("</Description>\n");
        sb.Append("</Zman>");
        return sb.ToString();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("\nLabel:\t").Append(JavaTime.NullToString(Label));
        sb.Append("\nZman:\t").Append(JavaTime.InstantToString(Time));
        sb.Append("\nGeoLocation:\t");
        if (GeoLocation == null)
        {
            sb.Append("null");
        }
        else
        {
            sb.Append(GeoLocation.ToString().Replace("\n", "\n\t"));
        }
        sb.Append("\nDuration:\t").Append(JavaTime.DurationToString(Duration));
        sb.Append("\nDescription:\t").Append(JavaTime.NullToString(Description));
        return sb.ToString();
    }
}
