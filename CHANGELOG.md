# Changelog

The version numbers of Kosher.Zmanim.NET follow the [KosherJava](https://github.com/KosherJava/zmanim) release each version is a
port of. See the [KosherJava CHANGELOG](https://github.com/KosherJava/zmanim/blob/master/CHANGELOG.md) for the changes in the
calculations themselves.

## Unreleased
Fixes for differences from KosherJava found by differential fuzzing (random inputs replayed against KosherJava 3.1.0):
* .NET 8: non-finite or out of range values (for example `AteretTorahSunsetOffset = double.NaN`, or
  `GetSunsetSolarDipFromOffset(double.PositiveInfinity)`) gave wrong results, such as a *sha'ah zmanis* of −8,895 days. Converting
  a `double` to `long` now follows Java's rules (NaN is 0, out of range values saturate) on every runtime; .NET 9 and later
  already behaved this way.
* `SPACalculator`: the ΔT estimate converted the Julian day through `DateOnly`, so a rise / set search that evaluated a date
  before year 1 threw `ArgumentOutOfRangeException` where KosherJava returns a result (for example
  `GetPercentOfShaahZmanisFromDegrees(118.79, true)` on 2076-02-29 in Etc/GMT-14). The year and month are now computed
  directly, for the same range of years as `java.time`.
* `GetSunTransit(DateTimeOffset?, DateTimeOffset?)` returned the right instant but in the offset of its `startOfDay` argument;
  it is now in the location's time zone like every other zman.

Behavior changes and additions:
* `GetCandleLighting()` returns *plag hamincha* (`GetPlagHaminchaGRA()`) when candle lighting would be before it (very short
  winter days near the Arctic Circle), instead of `null` like KosherJava. It still returns `null` when there is no sunset.
* New `JewishCalendar.IsShmitaYear()` and `JewishCalendar.IsRegularTaanis()` (a fast day other than Yom Kippur), from KosherSwift.

Fixes for KosherJava bugs (also proposed upstream):
* `SPACalculator` returns `NaN` right away when the target zenith is `NaN` (a `NaN` / infinite zenith, refraction or solar
  radius, or a negative earth radius) instead of searching for hours.
* `ZmanimFormatter.ToXml()` / `ToJson()`, `GeoLocation.ToXml()` and `Zman.ToXml()` escape text, so a location name, label or
  description with `"`, `&`, `<` or `\` no longer produces invalid XML or JSON.
* `ZmanimFormatter.FormatXsdDurationTime()` writes negative durations as a valid `xs:duration` (`-PT1H30M`, not `PT-1H-30M`).
* `HebrewDateFormatter.FormatHebrewNumber()` formats numbers up to 999,999 (it threw above 9999), so dates whose Jewish year is
  10,000 or more (Gregorian 6240 on) can be formatted in Hebrew.

## 3.1.1 (2026-11-05)
* Renamed members whose names referred to Java types or used Java casing, keeping the old names as `[Obsolete]` aliases so
  existing code still compiles:
  * `JewishCalendar.GetMoladAsInstant()` → `GetMoladAsDateTimeOffset()`, `GetTekufaAsInstant(bool)` → `GetTekufaAsDateTimeOffset(bool)`
  * `AstronomicalCalendar.GetInstantFromTime(...)` (protected) → `GetDateTimeOffsetFromTime(...)`
  * `ZmanimFormatter.FormatInstant(...)` → `Format(DateTimeOffset, TimeZoneInfo)` (plus a new `Format(DateTimeOffset)` that uses
    the formatter's `TimeZone`), `FormatXSDateTime` → `FormatXsdDateTime`, `FormatXSDDurationTime` → `FormatXsdDurationTime`,
    `SetTimeFormat(int)` → `TimeFormat` property
  * `HebrewDateFormatter.Get/SetTransliteratedParshiyosList` → `TransliteratedParshiyosList` property
  * `Daf.SetMasechtaTransliterated` / `SetYerushalmiMasechtaTransliterated` / `GetYerushalmiMasechtosTransliterated()` /
    `GetYerushalmiMasechtos()` → static `BavliMasechtosTransliterated`, `YerushalmiMasechtosTransliterated` and
    `YerushalmiMasechtos` properties
  * Parameters named `zonedDateTime` are now `dateTime`.
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
