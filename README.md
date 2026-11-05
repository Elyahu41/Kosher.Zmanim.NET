Kosher.Zmanim.NET API
=====================

The Zmanim library is an API for a specialized calendar that can calculate different astronomical
times including sunrise and sunset and Jewish _zmanim_ or religious times for prayers and other
Jewish religious duties.

This is a complete .NET port of the [KosherJava Zmanim API](https://github.com/KosherJava/zmanim) (version 3.1.0), written with
native .NET types only (`DateTimeOffset`, `DateOnly`, `TimeOnly`, `TimeSpan` and `TimeZoneInfo` - no NodaTime). For non religious
astronomical / solar calculations use the [AstronomicalCalendar](./src/Kosher.Zmanim/AstronomicalCalendar.cs).

The ZmanimCalendar contains the most common zmanim or religious time calculations. For a much more
extensive list of _zmanim_ use the ComprehensiveZmanimCalendar.
This class contains the main functionality of the Zmanim library.

For a basic set of instructions on the use of the API, see [How to Use the Zmanim API](https://kosherjava.com/zmanim-project/how-to-use-the-zmanim-api/), [zmanim code samples](https://kosherjava.com/tag/code-sample/) and the [KosherJava FAQ](https://kosherjava.com/tag/faq/). See the <a href="https://kosherjava.com">KosherJava Zmanim site</a> for additional information.

# Get Started
Add Kosher.Zmanim.NET as a dependency to your project. It supports .NET 8 and .NET 10.

#### .NET CLI
```shell
dotnet add package Kosher.Zmanim
```

#### PackageReference
Add the following to your `.csproj` file:
```xml
<PackageReference Include="Kosher.Zmanim" Version="3.1.0" />
```

How To Use This API
-------
Import the namespaces you need:
```csharp
using Kosher.Zmanim;                 // AstronomicalCalendar, ZmanimCalendar, ComprehensiveZmanimCalendar
using Kosher.Zmanim.Util;            // GeoLocation, the calculators, Zman, ZmanimFormatter
using Kosher.Zmanim.HebrewCalendar;  // JewishDate, JewishCalendar, HebrewDateFormatter, TefilaRules, Daf
```
Then, it is as simple as instantiating a class and calling its methods:
```csharp
var jewishCalendar = new JewishCalendar(); // by default it is set to today's date
Console.WriteLine(jewishCalendar.JewishMonth); // the Jewish month as a number, so Nissan will be 1. See the JewishDate constants

// To check for holidays/yom tovs, you can use the built in methods of the JewishCalendar:
if (jewishCalendar.IsPesach())
{
    Console.WriteLine("Today is Passover!");
}
// or you can check it yourself with the GetYomTovIndex() method just like in KosherJava:
if (jewishCalendar.GetYomTovIndex() == JewishCalendar.Pesach)
{
    Console.WriteLine("Today is Passover!");
}
```
For sunrise and sunset times, you will need a GeoLocation object. Then, you will need to pass that into the AstronomicalCalendar class like so:
```csharp
var geoLocation = new GeoLocation("Lakewood, NJ", 40.08213, -74.20970,
    TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
// The location name can be null if you'd like. There is also a constructor that takes the elevation in meters.
// There are four sunrise/sunset calculators in this API: NOAACalculator (the default), SPACalculator, MeeusCalculator and the
// legacy SunTimesCalculator.
var cal = new AstronomicalCalendar(geoLocation);
// To use the NREL Solar Position Algorithm (the most accurate):
// cal.AstronomicalCalculator = new SPACalculator();
DateTimeOffset? sunrise = cal.GetSunrise(); // This will return a nullable DateTimeOffset (null when the sun does not rise)
```
For zmanim, you will do the same thing, just use the ComprehensiveZmanimCalendar or ZmanimCalendar classes:
```csharp
var czc = new ComprehensiveZmanimCalendar(geoLocation);
DateTimeOffset? sofZmanShmaGra = czc.GetSofZmanShmaGRA(); // This will return a nullable DateTimeOffset
TimeSpan? shaahZmanis = czc.GetShaahZmanisGRA();            // durations are nullable TimeSpans
```
If you want to calculate a zman for a certain date, set the `LocalDate` property:
```csharp
var czc = new ComprehensiveZmanimCalendar(geoLocation)
{
    LocalDate = new DateOnly(2023, 1, 1),
};
DateTimeOffset? sunrise = czc.GetSunrise();
```
Every `DateTimeOffset` returned already carries the UTC offset of the location's time zone at that instant (including daylight
saving time), so `sunrise.Value.ToString("h:mm:ss tt")` or `sunrise.Value.DateTime` give the local clock time with no extra conversion.
Time zone IDs can be IANA (`"America/New_York"`) or Windows (`"Eastern Standard Time"`) IDs on any operating system (.NET converts
between them using ICU).

For converting any of the data in this API to hebrew, look at the HebrewDateFormatter class:
```csharp
var jewishCalendar = new JewishCalendar(5786, JewishDate.Nissan, 15);
var hebrewDateFormatter = new HebrewDateFormatter();
Console.WriteLine(hebrewDateFormatter.FormatMonth(jewishCalendar)); // prints "Nissan"
Console.WriteLine(hebrewDateFormatter.Format(jewishCalendar));      // prints "15 Nissan, 5786"
hebrewDateFormatter.IsHebrewFormat = true;
Console.WriteLine(hebrewDateFormatter.FormatMonth(jewishCalendar)); // prints "ניסן"
Console.WriteLine(hebrewDateFormatter.Format(jewishCalendar));      // prints "ט״ו ניסן תשפ״ו"
```
Look at the TefilaRules class for methods that use the JewishCalendar to tell you which prayers to say:
```csharp
var tefilaRules = new TefilaRules();
tefilaRules.IsVeseinTalUmatarRecited(jewishCalendar); // returns true or false depending on the DAY of the year
```
There are also the convenience classes from KosherJava:
```csharp
var formatter = new ZmanimFormatter(TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
formatter.Format(new TimeSpan(1, 2, 3)); // "1:02:03". See the ZmanimFormatter constructor for the other formats
formatter.Format(sunrise.Value); // in the formatter's TimeZone, "h:mm:ss" by default, set DateTimeFormat to customize
czc.ToString(); // all the zmanim of the day as XML. czc.ToJson() for JSON

var zman = new Zman(DateTimeOffset.Now, "Sunrise"); // A wrapper class for zmanim that is mostly intended to allow sorting collections of astronomical times.
zman.Description = "This is the time for sunrise.";

// or use it to sort your list of zmanim like so:
DateTimeOffset early = DateTimeOffset.Now;
DateTimeOffset later = early.AddHours(1);
var list = new List<Zman> { new Zman(later, "Later"), new Zman(early, "Earlier") };
list.Sort(Zman.DateOrder);
```

Using LINQ
-------
Every time and duration is nullable (`DateTimeOffset?` / `TimeSpan?`), exactly where KosherJava may return `null`, so results
compose naturally with LINQ. The port also adds a few LINQ-friendly helpers. Since the calendars are mutable (just like in
KosherJava), these return a separate copy for every day, so the results are safe to buffer, sort and group:
```csharp
// every candle lighting time of the coming year
var candleLighting = czc.EachDay(DateOnly.FromDateTime(DateTime.Today), 365)
    .Where(day => day.LocalDate.DayOfWeek == DayOfWeek.Friday)
    .Select(day => day.GetCandleLighting())
    .OfType<DateTimeOffset>();

// all of today's zmanim (each public Get...() zman of the calendar) in time order
foreach (Zman z in czc.GetZmanim().Where(z => z.Time != null).OrderBy(z => z, Zman.DateOrder))
{
    Console.WriteLine($"{z.Label}: {z.Time:h:mm:ss tt}");
}

// the parshiyos and yamim tovim of the year
var yamimTovim = new JewishCalendar(5787, JewishDate.Tishrei, 1).EachDay(385)
    .Where(day => day.IsYomTov())
    .Select(day => (day.LocalDate, Name: hebrewDateFormatter.FormatYomTov(day)));
```
`JewishDate` also implements `IComparable<JewishDate>` and `IEquatable<JewishDate>` and supports `<`, `>`, `<=` and `>=`.

Making Your Own Methods
-------
If you want to add onto what is already available, subclass the calendar just like in KosherJava (subclasses can use protected
helpers such as `GetSunriseBasedOnElevationSetting()`). C# extension methods work too for anything built on the public API.
```csharp
public class MyZmanimCalendar : ComprehensiveZmanimCalendar
{
    public MyZmanimCalendar(GeoLocation location) : base(location)
    {
    }

    // for regular minutes
    public DateTimeOffset? GetTzais45() =>
        ToZone(GetTimeOffset(GetSunsetBasedOnElevationSetting(), TimeSpan.FromMinutes(45)));

    // for degrees
    public DateTimeOffset? GetTzais8Point7Degrees() => GetSunsetOffsetByDegrees(GeometricZenith + 8.7);

    // for seasonal hours (GetTimeOffset handles null values)
    public DateTimeOffset? GetTwoSeasonalHoursIntoTheDay() =>
        ToZone(GetTimeOffset(GetSunriseBasedOnElevationSetting(), GetShaahZmanisGRA() * 2));
}
```
`ToZone()` returns the time in the location's time zone, like all of the built-in methods. Public parameterless `Get...()`
methods you add are also picked up by `GetZmanim()`, `ToString()` and `ToJson()`.

Things To Keep In Mind
-------
This port follows KosherJava as closely as possible, so code and documentation written for KosherJava translate directly, with
the usual .NET naming conventions applied:
* Methods are PascalCase (`getSunrise()` is `GetSunrise()`), and constants are PascalCase (`GEOMETRIC_ZENITH` is
  `GeometricZenith`, `JewishDate.ADAR_II` is `JewishDate.AdarII`, `Parshah.LECH_LECHA` is `Parshah.LechLecha`).
* Simple getter / setter pairs are properties (`setLocalDate(date)` is `LocalDate = date`, `isUseElevation()` is `UseElevation`,
  `getJewishYear()` is `JewishYear`, `setHebrewFormat(true)` is `IsHebrewFormat = true`). A few names had to change because .NET does
  not allow a member to have the name of its class: `Zman.getZman()` is `Zman.Time` and `Daf.getDaf()` is `Daf.DafNumber`;
  `GeoLocation.getZoneId()` is `GeoLocation.TimeZone`.
* Names that refer to Java types use the .NET type instead: `getMoladAsInstant()` is `GetMoladAsDateTimeOffset()`,
  `getTekufaAsInstant()` is `GetTekufaAsDateTimeOffset()`, `ZmanimFormatter.formatInstant()` is `ZmanimFormatter.Format()`, and
  `formatXSDateTime()` / `formatXSDDurationTime()` are `FormatXsdDateTime()` / `FormatXsdDurationTime()`. The KosherJava names
  still compile but are marked `[Obsolete]`.
* `java.time.Instant` is `DateTimeOffset?` (in the location's offset), `Duration` is `TimeSpan?`, `LocalDate` is `DateOnly`,
  `LocalTime` is `TimeOnly` and `ZoneId` is `TimeZoneInfo`.
* `JewishDate.getDayOfWeek()` keeps the KosherJava numbering (1 = Sunday through 7 = Shabbos).
* Members that KosherJava marks `@Deprecated` are marked `[Obsolete]`, with the same explanation (for example the
  _zmanim_ that should only be used _lechumra_).
* `java.time` has nanosecond resolution and .NET has 100 nanosecond ticks, so results can differ from KosherJava by up to about a
  microsecond. DateOnly can't represent dates before January 1, 0001, so `JewishDate.LocalDate` throws for the handful of
  molad dates in Gregorian year 0. Before 1970 the time zone database of your operating system may differ from Java's for some
  locations.

Accuracy
-------
Every calculation is tested against KosherJava itself. Besides all of the KosherJava unit tests (ported to xUnit), the test suite
compares this port with values generated by KosherJava 3.1.0 ([tools/golden](./tools/golden)):
* about 500,000 _zmanim_ - every method of `ComprehensiveZmanimCalendar` with the NOAA calculator, the core times with the SPA,
  Meeus and SunTimes calculators, and the raw calculator output at many zeniths, plus solar elevation and azimuth - for 12 locations
  (both hemispheres, the equator, the date line, the Arctic, horizon elevations) on dates from 1900 to 2099. The largest
  difference is 1.3 microseconds.
* about 1.4 million Hebrew calendar values - the Jewish date, holidays, parshah, daf yomi, molad, kiddush levana, every
  `TefilaRules` rule and the `HebrewDateFormatter` output in Hebrew and English, in and outside of Israel, from 1924 to 2100. They
  all match exactly.

License
-------
The library is released under the [LGPL 2.1 license](https://kosherjava.com/2011/05/09/kosherjava-zmanim-api-released-under-the-lgpl-license/).

Ports to Other Languages
------------------------
The KosherJava Zmanim API has been ported to:
* Java (original repo) - https://github.com/KosherJava/zmanim/
* .NET - https://github.com/Elyahu41/Kosher.Zmanim.NET (You are here) & https://github.com/Yitzchok/Zmanim
* C - https://github.com/yparitcher/libzmanim
* Dart / Flutter - https://github.com/yakir8/kosher_dart
* Go - https://github.com/vlipovetskii/go-zmanim
* JavaScript - https://github.com/yparitcher/zmanJS
* Kotlin - https://github.com/elfifo4/KosherKotlin / [Maven Central](https://central.sonatype.com/artifact/io.github.elfifo4/kosherkotlin) (an up-to-date fork of [Sternbach-Software's port](https://github.com/Sternbach-Software/KosherKotlin)).
* Monkey C - https://github.com/5E7EN/Garmin-ZmanIQ/tree/rebuild-zmaniq/source/utils/zmanim
* Objective-C - https://github.com/MosheBerman/KosherCocoa
* PHP - https://github.com/zachweix/PhpZmanim/
* Python - https://github.com/pinnymz/python-zmanim & https://pypi.org/project/zmanim/
* Ruby - https://github.com/pinnymz/ruby-zmanim
* Rust - https://github.com/dickermoshe/rust-zmanim-project & https://github.com/YSCohen/rust-zmanim (WIP)
* Scala - https://github.com/nafg/jewish-date
* SQL - https://github.com/BehindTheMath/zmanim-sql (alpha quality).
* Swift - https://github.com/Elyahu41/KosherSwift & https://github.com/DanielSmith1239/KosherSwift
* TypeScript - https://github.com/BehindTheMath/KosherZmanim
* VB6/VBA (Visual Basic) - https://github.com/diaphone1/vbzmanim

ZmanCode Desktop App
------------------------
The earlier .NET port was used to create a desktop app that is available at https://github.com/NykUser/MyZman.

Disclaimer:
-----------
__While I did my best to get accurate results, please double check before relying on these zmanim for <em>halacha lemaaseh</em>__.

------------------------
[![NuGet](https://img.shields.io/nuget/v/Kosher.Zmanim?color=eed6af&logo=nuget&cacheSeconds=3600)](https://www.nuget.org/packages/Kosher.Zmanim)
[![GitHub](https://img.shields.io/github/license/Elyahu41/Kosher.Zmanim.NET?color=eed6af&logo=gnu)](https://github.com/Elyahu41/Kosher.Zmanim.NET/blob/main/LICENSE)
[![GitHub last commit](https://img.shields.io/github/last-commit/Elyahu41/Kosher.Zmanim.NET?logo=github)](https://github.com/Elyahu41/Kosher.Zmanim.NET/commits/main)
[![Build](https://github.com/Elyahu41/Kosher.Zmanim.NET/actions/workflows/build.yml/badge.svg)](https://github.com/Elyahu41/Kosher.Zmanim.NET/actions/workflows/build.yml)
