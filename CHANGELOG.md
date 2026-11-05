# Changelog

The version numbers of Kosher.Zmanim.NET follow the [KosherJava](https://github.com/KosherJava/zmanim) release each version is a
port of. See the [KosherJava CHANGELOG](https://github.com/KosherJava/zmanim/blob/master/CHANGELOG.md) for the changes in the
calculations themselves.

## Unreleased
* Documentation: replaced the remaining Java terms in the XML documentation (`Instant`, `ZonedDateTime`, `LocalDate`, `Calendar`,
  `IllegalArgumentException`, `String`, Java getter names) with their .NET equivalents, filled in empty `<returns>` tags, documented
  `Clone()`, and converted HTML lists to XML documentation `<list>` elements so they render in IntelliSense.
* `SunTimesCalculator`: the `NotSupportedException` messages no longer contain the Java `{@link}` markup.
* `JewishDate` / `JewishCalendar`: removed `[MethodImpl(MethodImplOptions.Synchronized)]` (a lock on `this` carried over from
  Java's `synchronized`). These classes are mutable and, as in KosherJava, are not meant to be shared between threads without
  external locking; the results are unchanged.

## 3.1.0 (2026-11-05)
Initial release: a complete port of KosherJava 3.1.0 (released 2026-11-02).
* All classes: `AstronomicalCalendar`, `ZmanimCalendar`, `ComprehensiveZmanimCalendar`, `GeoLocation`, the NOAA, SPA, Meeus and
  SunTimes calculators, `Zman`, `ZmanimFormatter`, `JewishDate`, `JewishCalendar`, `HebrewDateFormatter`, `TefilaRules`, `Daf`,
  `YomiCalculator` and `YerushalmiYomiCalculator`, with all of the KosherJava documentation as XML documentation comments.
* Native .NET types only (`DateTimeOffset`, `DateOnly`, `TimeOnly`, `TimeSpan`, `TimeZoneInfo`); no NodaTime or other dependencies.
  Targets .NET 8 and .NET 10.
* All KosherJava unit tests ported to xUnit, plus golden-value tests that compare ~500,000 zmanim and ~1.4 million Hebrew calendar
  values against KosherJava 3.1.0.
* .NET additions: nullable return types for every time and duration, `EachDay()` and `GetZmanim()` LINQ helpers, and comparison
  operators / `IEquatable` for `JewishDate`.
