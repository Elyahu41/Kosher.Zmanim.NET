# Golden values

`GoldenGenerator.java` runs [KosherJava](https://github.com/KosherJava/zmanim) itself and writes the values the .NET tests compare
against (`tests/Kosher.Zmanim.Tests/Golden/Data/*.tsv.gz`):

* `zmanim.tsv.gz` - every zman of `ComprehensiveZmanimCalendar` (NOAA), the core astronomical times with the SPA, Meeus and
  SunTimes calculators, and raw calculator output (UTC rise/set at many zeniths, solar elevation/azimuth, time at azimuth) for 12
  locations on ~70 dates from 1900 to 2099. Times are stored as nanoseconds since the epoch.
* `hebrew.tsv.gz` - every parameterless getter of `JewishCalendar`, every `TefilaRules` rule and `HebrewDateFormatter` output (Hebrew
  and English), in and outside of Israel, daily for 2024-2027 and weekly from 1924 to 2100.

To regenerate them for a new KosherJava release (Java 17+ and the KosherJava sources or jar are needed):

```shell
git clone https://github.com/KosherJava/zmanim.git kosherjava
javac -encoding UTF-8 -d build $(find kosherjava/src/main -name '*.java')
javac -encoding UTF-8 -cp build -d build tools/golden/GoldenGenerator.java
java -cp build GoldenGenerator tests/Kosher.Zmanim.Tests/Golden/Data
```

Then run `dotnet test`. The tests also report the largest deviation found.
