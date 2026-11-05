# Changelog

The version numbers of Kosher.Zmanim.NET follow the [KosherJava](https://github.com/KosherJava/zmanim) release each version is a
port of. See the [KosherJava CHANGELOG](https://github.com/KosherJava/zmanim/blob/master/CHANGELOG.md) for the changes in the
calculations themselves.

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
