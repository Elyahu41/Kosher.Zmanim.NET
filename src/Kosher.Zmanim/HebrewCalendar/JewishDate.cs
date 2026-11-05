/*
 * Zmanim Java API
 * Copyright © 2011 - 2026 Eliyahu Hershfeld
 * Copyright © September 2002 Avrom Finkelstein
 * Copyright © 2026 Moshe Dicker
 *
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
 * Public License as published by the Free Software Foundation; version 2.1 of the License.
 *
 * This library is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
 * details.
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not, write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA,
 * or connect to: http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html
 */
using System.Runtime.CompilerServices;

namespace Kosher.Zmanim.HebrewCalendar;

/// <summary>
/// The JewishDate is the base calendar class, that supports maintenance of a <see cref="DateOnly"/>
/// instance along with the corresponding Jewish date. This class can use the standard Java Date and Calendar
/// classes for setting and maintaining the dates, but it does not subclass these classes or use them internally
/// in any calculations. This class also does not have a concept of a time (which the Date class does). Please
/// note that the calendar does not currently support dates prior to 1/1/1 Gregorian. Also keep in mind that the
/// Gregorian calendar started on October 15, 1582, so any calculations prior to that are suspect (at least from
/// a Gregorian perspective). While 1/1/1 Gregorian and forward are technically supported, any calculations prior to <a
/// href="http://en.wikipedia.org/wiki/Hillel_II">Hillel II's (Hakatan's</a>) calendar (4119 in the Jewish Calendar / 359
/// CE Julian as recorded by <a href="http://en.wikipedia.org/wiki/Hai_Gaon">Rav Hai Gaon</a>) would be just an
/// approximation.
///
/// This open source Java code was written by <a href="https://www.linkedin.com/in/avromf/">Avrom Finkelstein</a> from his C++
/// code. It was refactored to fit the KosherJava Zmanim API with simplification of the code, enhancements and some bug
/// fixing.
///
/// Some of Avrom's original C++ code was translated from
/// <a href="https://web.archive.org/web/20120124134148/http://emr.cs.uiuc.edu/~reingold/calendar.C">C/C++ code</a> in
/// <a href="http://www.calendarists.com">Calendrical Calculations</a> by Nachum Dershowitz and Edward M.
/// Reingold, Software-- Practice &amp; Experience, vol. 20, no. 9 (September, 1990), pp. 899- 928. Any method with the mark
/// "ND+ER" indicates that the method was taken from this source with minor modifications.
///
/// If you are looking for a class that implements a Jewish calendar version of the Calendar class, one is available from
/// the <a href="http://site.icu-project.org/" >ICU (International Components for Unicode)</a> project, formerly part of
/// IBM's DeveloperWorks.
/// </summary>
/// <seealso cref="JewishCalendar"/>
/// <seealso cref="HebrewDateFormatter"/>
public class JewishDate : IComparable<JewishDate>, IEquatable<JewishDate>
{
    /// <summary>
    /// Value of the month field indicating Nissan, the first numeric month of the year in the Jewish calendar. With the year
    /// starting at <see cref="Tishrei"/>, it would actually be the 7th (or 8th in a <see cref="IsJewishLeapYear(int)">leap year</see>) month of
    /// the year.
    /// </summary>
    public const int Nissan = 1;

    /// <summary>
    /// Value of the month field indicating Iyar, the second numeric month of the year in the Jewish calendar. With the year starting
    /// at <see cref="Tishrei"/>, it would actually be the 8th (or 9th in a <see cref="IsJewishLeapYear(int)">leap year</see>) month of the year.
    /// </summary>
    public const int Iyar = 2;

    /// <summary>
    /// Value of the month field indicating Sivan, the third numeric month of the year in the Jewish calendar. With the year starting
    /// at <see cref="Tishrei"/>, it would actually be the 9th (or 10th in a <see cref="IsJewishLeapYear(int)">leap year</see>) month of the year.
    /// </summary>
    public const int Sivan = 3;

    /// <summary>
    /// Value of the month field indicating Tammuz, the fourth numeric month of the year in the Jewish calendar. With the year
    /// starting at <see cref="Tishrei"/>, it would actually be the 10th (or 11th in a <see cref="IsJewishLeapYear(int)">leap year</see>) month of the
    /// year.
    /// </summary>
    public const int Tammuz = 4;

    /// <summary>
    /// Value of the month field indicating Av, the fifth numeric month of the year in the Jewish calendar. With the year starting at
    /// <see cref="Tishrei"/>, it would actually be the 11th (or 12th in a <see cref="IsJewishLeapYear(int)">leap year</see>) month of the year.
    /// </summary>
    public const int Av = 5;

    /// <summary>
    /// Value of the month field indicating Elul, the sixth numeric month of the year in the Jewish calendar. With the year starting
    /// at <see cref="Tishrei"/>, it would actually be the 12th (or 13th in a <see cref="IsJewishLeapYear(int)">leap year</see>) month of the year.
    /// </summary>
    public const int Elul = 6;

    /// <summary>
    /// Value of the month field indicating Tishrei, the seventh numeric month of the year in the Jewish calendar. With the year
    /// starting at this month, it would actually be the 1st month of the year.
    /// </summary>
    public const int Tishrei = 7;

    /// <summary>
    /// Value of the month field indicating Cheshvan/marcheshvan, the eighth numeric month of the year in the Jewish calendar. With
    /// the year starting at <see cref="Tishrei"/>, it would actually be the 2nd month of the year.
    /// </summary>
    public const int Cheshvan = 8;

    /// <summary>
    /// Value of the month field indicating Kislev, the ninth numeric month of the year in the Jewish calendar. With the year starting
    /// at <see cref="Tishrei"/>, it would actually be the 3rd month of the year.
    /// </summary>
    public const int Kislev = 9;

    /// <summary>
    /// Value of the month field indicating Teves, the tenth numeric month of the year in the Jewish calendar. With the
    /// year starting at <see cref="Tishrei"/>, it would actually be the 4th month of the year.
    /// </summary>
    public const int Teves = 10;

    /// <summary>
    /// Value of the month field indicating Shevat, the eleventh numeric month of the year in the Jewish calendar. With
    /// the year starting at <see cref="Tishrei"/>, it would actually be the 5th month of the year.
    /// </summary>
    public const int Shevat = 11;

    /// <summary>
    /// Value of the month field indicating Adar (or Adar I in a <see cref="IsJewishLeapYear(int)">leap year</see>), the twelfth
    /// numeric month of the year in the Jewish calendar. With the year starting at <see cref="Tishrei"/>, it would actually
    /// be the 6th month of the year.
    /// </summary>
    public const int Adar = 12;

    /// <summary>
    /// Value of the month field indicating Adar II, the leap (intercalary or embolismic) thirteenth (Undecimber) numeric
    /// month of the year added in Jewish <see cref="IsJewishLeapYear(int)">leap year</see>). The leap years are years 3, 6, 8, 11,
    /// 14, 17 and 19 of a 19-year cycle. With the year starting at <see cref="Tishrei"/>, it would actually be the 7th month
    /// of the year.
    /// </summary>
    public const int AdarII = 13;

    /// <summary>
    /// the Jewish epoch using the RD (Rata Die/Fixed Date or Reingold Dershowitz) day used in Calendrical Calculations.
    /// Day 1 is January 1, 0001 of the Gregorian calendar
    /// </summary>
    private const int JewishEpoch = -1373429;

    /// <summary>
    /// The number  of <em>chalakim</em> (18) in a minute.
    /// </summary>
    private const int ChalakimPerMinute = 18;
    /// <summary>
    /// The number  of <em>chalakim</em> (1080) in an hour.
    /// </summary>
    private const int ChalakimPerHour = 1080;
    /// <summary>
    /// The number of <em>chalakim</em> (25,920) in a 24-hour day .
    /// </summary>
    private const long ChalakimPerDay = 25920; // 24 * 1080
    /// <summary>
    /// The number  of <em>chalakim</em> in an average Jewish month. A month has 29 days, 12 hours and 793
    /// <em>chalakim</em> (44 minutes and 3.3 seconds) for a total of 765,433 <em>chalakim</em>
    /// </summary>
    private const long ChalakimPerMonth = 765433; // (29 * 24 + 12) * 1080 + 793
    /// <summary>
    /// Days from the beginning of Sunday till <em>molad BaHaRaD</em>. Calculated as 1 day, 5 hours and 204 <em>chalakim</em> =
    /// (24 + 5) * 1080 + 204 = 31524
    /// </summary>
    private const int ChalakimMoladTohu = 31524;

    /// <summary>
    /// A short year where both <see cref="Cheshvan"/> and <see cref="Kislev"/> are 29 days.
    /// </summary>
    /// <seealso cref="GetCheshvanKislevKviah"/>
    /// <seealso cref="HebrewDateFormatter.GetFormattedKviah"/>
    public const int Chaserim = 0;

    /// <summary>
    /// An ordered year where <see cref="Cheshvan"/> is 29 days and <see cref="Kislev"/> is 30 days.
    /// </summary>
    /// <seealso cref="GetCheshvanKislevKviah"/>
    /// <seealso cref="HebrewDateFormatter.GetFormattedKviah"/>
    public const int Kesidran = 1;

    /// <summary>
    /// A long year where both <see cref="Cheshvan"/> and <see cref="Kislev"/> are 30 days.
    /// </summary>
    /// <seealso cref="GetCheshvanKislevKviah"/>
    /// <seealso cref="HebrewDateFormatter.GetFormattedKviah"/>
    public const int Shelaimim = 2;

    /// <summary>
    /// the internal Jewish month.
    /// </summary>
    private int jewishMonth;
    /// <summary>
    /// the internal Jewish day.
    /// </summary>
    private int jewishDay;
    /// <summary>
    /// the internal Jewish year.
    /// </summary>
    private int jewishYear;
    /// <summary>
    /// the internal count of <em>molad</em> hours.
    /// </summary>
    private int moladHours;
    /// <summary>
    /// the internal count of <em>molad</em> minutes.
    /// </summary>
    private int moladMinutes;
    /// <summary>
    /// the internal count of <em>molad chalakim</em>.
    /// </summary>
    private int moladChalakim;

    /// <summary>
    /// Returns the <em>molad</em> hours. Only a JewishDate object populated with <see cref="GetMolad"/>,
    /// <see cref="SetJewishDate(int, int, int)"/> or <see cref="MoladHours"/> will have this field
    /// populated. A regular JewishDate object will have this field set to 0.
    /// </summary>
    /// <value>the <em>molad</em> hours to set</value>
    /// <remarks>
    /// Sets the <em>molad</em> hours.
    /// </remarks>
    /// <seealso cref="MoladHours"/>
    /// <seealso cref="GetMolad"/>
    /// <seealso cref="SetJewishDate(int, int, int)"/>
    public int MoladHours
    {
        get => moladHours;
        set => moladHours = value;
    }

    /// <summary>
    /// Returns the <em>molad</em> minutes. Only an object populated with <see cref="GetMolad"/>,
    /// <see cref="SetJewishDate(int, int, int)"/> or <see cref="MoladMinutes"/> will have these fields populated.
    /// A regular JewishDate object will have this field set to 0.
    /// </summary>
    /// <value>the molad minutes to set</value>
    /// <remarks>
    /// Sets the <em>molad</em> minutes. The expectation is that the traditional minute-less <em>chalakim</em> will be broken out to
    /// minutes and <see cref="MoladChalakim"><em>chalakim</em> / parts</see> , so 793 (TaShTZaG) parts would have the minutes set to
    /// 44 and <em>chalakim</em> to 1.
    /// </remarks>
    /// <seealso cref="MoladMinutes"/>
    /// <seealso cref="GetMolad"/>
    /// <seealso cref="SetJewishDate(int, int, int)"/>
    /// <seealso cref="MoladChalakim"/>
    public int MoladMinutes
    {
        get => moladMinutes;
        set => moladMinutes = value;
    }

    /// <summary>
    /// Returns the <em>molad chalakim</em> / parts. Only an object populated with <see cref="GetMolad"/>,
    /// <see cref="SetJewishDate(int, int, int)"/> or <see cref="MoladChalakim"/> will have these fields
    /// populated. A regular JewishDate object will have this field set to 0.
    /// </summary>
    /// <value>the <em>molad chalakim</em> / parts to set</value>
    /// <remarks>
    /// Sets the <em>molad</em> chalakim/parts. The expectation is that the traditional minute-less <em>chalakim</em> will be broken
    /// out to <see cref="MoladMinutes">minutes</see> and chalakim, so 793 (TaShTZaG) parts would have the minutes set to 44 and
    /// <em>chalakim</em> to 1.
    /// </remarks>
    /// <seealso cref="MoladChalakim"/>
    /// <seealso cref="GetMolad"/>
    /// <seealso cref="SetJewishDate(int, int, int)"/>
    /// <seealso cref="MoladMinutes"/>
    public int MoladChalakim
    {
        get => moladChalakim;
        set => moladChalakim = value;
    }

    /// <summary>
    /// 1 == Sunday, 2 == Monday, etc...
    /// </summary>
    private int dayOfWeek;

    /// <summary>
    /// Returns the absolute date (days since January 1, 0001 of the Gregorian calendar).
    /// </summary>
    /// <seealso cref="AbsDate"/>
    /// <seealso cref="AbsDate"/>
    private int gregorianAbsDate;

    /// <summary>
    /// Returns the number of days in a given month in a given year and month.
    /// </summary>
    /// <param name="year">the year (only impacts February)</param>
    /// <param name="month">the month. As with other cases in this class, this is 1-based, not zero-based.</param>
    /// <returns>the number of days in the month in the given year</returns>
    private static int GetLastDayOfGregorianMonth(int year, int month)
    {
        return DateTime.DaysInMonth(year, month);
    }

    /// <summary>
    /// Computes the Gregorian date from the absolute date. ND+ER
    /// </summary>
    /// <param name="absDate">the absolute date</param>
    /// <returns>the <see cref="DateOnly"/>.</returns>
    private static DateOnly AbsDateToDate(int absDate)
    {
        int year = JavaMath.FloorDiv(absDate, 366); // Search forward year by year from approximate year
        while (absDate >= GregorianDateToAbsDate(year + 1, 1, 1))
        {
            year++;
        }

        int month = 1; // Search forward month by month from January
        while (absDate > GregorianDateToAbsDate(year, month, GetLastDayOfGregorianMonth(year, month)))
        {
            month++;
        }

        int dayOfMonth = absDate - GregorianDateToAbsDate(year, month, 1) + 1;
        if (year < 1)
        {
            // java.time can represent the proleptic Gregorian year 0 (for example the molad of Teves 3761), DateOnly can not
            throw new ArgumentOutOfRangeException(nameof(absDate), "The Gregorian date " + year + "-" + month + "-" + dayOfMonth
                    + " is before January 1, 0001 and can't be represented by a DateOnly.");
        }
        return new DateOnly(year, month, dayOfMonth);
    }

    /// <summary>
    /// Returns the absolute date (days since January 1, 0001 of the Gregorian calendar).
    /// </summary>
    /// <value>the number of days since January 1, 1</value>
    public int AbsDate => gregorianAbsDate;

    /// <summary>
    /// Computes the absolute date from a Gregorian date. ND+ER
    /// </summary>
    /// <param name="year">the Gregorian year</param>
    /// <param name="month">the Gregorian month. Unlike the Java Calendar where January has the value of 0,This expects a 1 for January</param>
    /// <param name="dayOfMonth">the day of the month (1st, 2nd, etc...)</param>
    /// <returns>the absolute Gregorian day</returns>
    private static int GregorianDateToAbsDate(int year, int month, int dayOfMonth)
    {
        int absDate = dayOfMonth;
        for (int m = month - 1; m > 0; m--)
        {
            absDate += GetLastDayOfGregorianMonth(year, m); // days in prior months of the year
        }

        int y = year - 1;
        // Floor division, so that year 0, a leap year, gets its leap day
        return (absDate // days this year
                + 365 * y // days in previous years ignoring leap days
                + JavaMath.FloorDiv(y, 4) // Julian leap days before this year
                - JavaMath.FloorDiv(y, 100) // minus prior century years
                + JavaMath.FloorDiv(y, 400)); // plus prior years divisible by 400
    }

    /// <summary>
    /// Returns if the year is a Jewish leap year. Years 3, 6, 8, 11, 14, 17 and 19 in the 19-year cycle are leap years.
    /// </summary>
    /// <param name="year">the Jewish year.</param>
    /// <returns>true if it is a leap year</returns>
    /// <seealso cref="IsJewishLeapYear(int)"/>
    public static bool IsJewishLeapYear(int year)
    {
        return ((7 * year) + 1) % 19 < 7;
    }

    /// <summary>
    /// Returns if the year the calendar is set to is a Jewish leap year. Years 3, 6, 8, 11, 14, 17 and 19 in the 19-year
    /// cycle are leap years.
    /// </summary>
    /// <returns>true if it is a leap year</returns>
    /// <seealso cref="IsJewishLeapYear(int)"/>
    public virtual bool IsJewishLeapYear()
    {
        return IsJewishLeapYear(JewishYear);
    }

    /// <summary>
    /// Returns the last month of a given Jewish year. This will be 12 on a non <see cref="IsJewishLeapYear(int)">leap year</see>
    /// or 13 on a leap year.
    /// </summary>
    /// <param name="year">the Jewish year.</param>
    /// <returns>12 on a non leap year or 13 on a leap year</returns>
    /// <seealso cref="IsJewishLeapYear(int)"/>
    private static int GetLastMonthOfJewishYear(int year)
    {
        return IsJewishLeapYear(year) ? AdarII : Adar;
    }

    /// <summary>
    /// Returns the number of days elapsed from the Sunday prior to the start of the Jewish calendar to the mean
    /// conjunction of Tishri of the Jewish year.
    /// </summary>
    /// <param name="year">the Jewish year</param>
    /// <returns>the number of days elapsed from prior to the <em>molad Tohu BaHaRaD</em> (Be = Monday, <em>Ha</em> = 5
    ///         hours and <em>RaD</em> = 204 <em>chalakim</em> / parts) prior to the start of the Jewish calendar, to
    ///         the mean conjunction of Tishri of the Jewish year. BeHaRaD is 23:11:20 on Sunday night(5 hours 204/1080
    ///         <em>chalakim</em> after sunset on Sunday evening).</returns>
    public static int GetJewishCalendarElapsedDays(int year)
    {
        long chalakimSince = GetChalakimSinceMoladTohu(year, Tishrei);
        int moladDay = (int)(chalakimSince / ChalakimPerDay);
        int moladParts = (int)(chalakimSince - moladDay * ChalakimPerDay);
        // delay Rosh Hashana for the 4 dechiyos
        return AddDechiyos(year, moladDay, moladParts);
    }

    /// <summary>
    /// Adds the 4 dechiyos for <em>molad</em> Tishrei. These are:
    /// <ol>
    /// <li><em>Lo ADU Rosh</em> - Rosh Hashana can't fall on a Sunday, Wednesday or Friday. If the <em>molad</em> fell on one
    /// of these days, Rosh Hashana is delayed to the following day.</li>
    /// <li><em>Molad Zaken</em> - If the <em>molad</em> of Tishrei falls after 12 noon, Rosh Hashana is delayed to the following
    /// day. If the following day is <em>ADU</em>, it will be delayed an additional day.</li>
    /// <li><em>GaTRaD</em> - If on a non leap year the <em>molad</em> of Tishrei falls on a Tuesday (Ga) on or after 9 hours
    /// (<em>T</em>) and (<em>RaD</em> 204 <em>chalakim</em> it is delayed till Thursday (one day delay, plus one day for
    /// <em>Lo ADU Rosh</em>)</li>
    /// <li><em>BeTuTaKPaT</em> - if the year following a leap year falls on a Monday (<em>Be</em>) on or after 15 hours
    /// (<em>Tu</em>) and 589 <em>chalakim</em> (<em>TaKPaT</em>) it is delayed till Tuesday</li>
    /// </ol>
    /// </summary>
    /// <param name="year">the year</param>
    /// <param name="moladDay">the <em>molad</em> day</param>
    /// <param name="moladParts">the <em>molad</em> parts</param>
    /// <returns>the number of elapsed days in the JewishCalendar adjusted for the 4 dechiyos.</returns>
    private static int AddDechiyos(int year, int moladDay, int moladParts)
    {
        int roshHashanaDay = moladDay; // if no dechiyos
        // delay Rosh Hashana for the dechiyos of the Molad - new moon 1 - Molad Zaken, 2- GaTRaD 3- BeTuTaKPaT
        if ((moladParts >= 19440) // Dechiya of Molad Zaken - molad is >= midday (18 hours * 1080 chalakim)
                || (((moladDay % 7) == 2) // start Dechiya of GaTRaD - Ga = is a Tuesday
                && (moladParts >= 9924) // TRaD = 9 hours, 204 parts or later (9 * 1080 + 204)
                && !IsJewishLeapYear(year)) // of a non-leap year - end Dechiya of GaTRaD
                || (((moladDay % 7) == 1) // start Dechiya of BeTuTaKPaT - Be = is on a Monday
                && (moladParts >= 16789) // TUTaKPaT part of BeTuTaKPaT = 15 hours, 589 parts or later (15 * 1080 + 589)
                && (IsJewishLeapYear(year - 1))))
        { // in a year following a leap year - end Dechiya of BeTuTaKPaT
            roshHashanaDay += 1; // Then postpone Rosh HaShanah one day
        }
        // start 4th Dechiya - Lo ADU Rosh - Rosh Hashana can't occur on A- sunday, D- Wednesday, U - Friday
        if (((roshHashanaDay % 7) == 0)// If Rosh HaShanah would occur on Sunday,
                || ((roshHashanaDay % 7) == 3) // or Wednesday,
                || ((roshHashanaDay % 7) == 5))
        { // or Friday - end 4th Dechiya - Lo ADU Rosh
            roshHashanaDay = roshHashanaDay + 1; // Then postpone it one (more) day
        }
        return roshHashanaDay;
    }

    /// <summary>
    /// Returns the number of <em>chalakim</em> (parts - 1080 to the hour) from the original hypothetical <em>Molad Tohu</em>
    /// to the year and month passed in.
    /// </summary>
    /// <param name="year">the Jewish year</param>
    /// <param name="month">the Jewish month the Jewish month, with the month numbers starting from Nissan. Use the JewishDate constants such
    ///         as <see cref="JewishDate.Tishrei"/>.</param>
    /// <returns>the number of <em>chalakim</em> (parts - 1080 to the hour) from the original hypothetical <em>Molad Tohu</em></returns>
    private static long GetChalakimSinceMoladTohu(int year, int month)
    {
        int y = year - 1;
        // Jewish lunar month = 29 days, 12 hours and 793 chalakim
        // chalakim since Molad Tohu BeHaRaD - 1 day, 5 hours and 204 chalakim
        int monthOfYear = GetJewishMonthOfYear(year, month);
        int monthsElapsed = (235 * (y / 19)) // Months in complete 19-year lunar (Metonic) cycles so far
                 + (12 * (y % 19)) // Regular months in this cycle
                 + ((7 * (y % 19) + 1) / 19) // Leap months this cycle
                 + (monthOfYear - 1); // add elapsed months till the start of the molad of the month
                                      // return chalakim prior to BeHaRaD + number of chalakim since
        return ChalakimMoladTohu + (ChalakimPerMonth * monthsElapsed);
    }

    /// <summary>
    /// Returns the number of <em>chalakim</em> (parts - 1080 to the hour) from the original hypothetical <em>Molad Tohu</em>
    /// to the Jewish year and month that this Object is set to.
    /// </summary>
    /// <returns>the number of <em>chalakim</em> (parts - 1080 to the hour) from the original hypothetical <em>Molad Tohu</em></returns>
    public virtual long GetChalakimSinceMoladTohu()
    {
        return GetChalakimSinceMoladTohu(jewishYear, jewishMonth);
    }

    /// <summary>
    /// Converts the <see cref="JewishDate.Nissan"/> based constants used by this class to numeric month starting from
    /// <see cref="JewishDate.Tishrei"/>. This is required for <em>molad</em> calculations.
    /// </summary>
    /// <param name="year">The Jewish year</param>
    /// <param name="month">The Jewish Month</param>
    /// <returns>the Jewish month of the year starting with Tishrei</returns>
    private static int GetJewishMonthOfYear(int year, int month)
    {
        bool isLeapYear = IsJewishLeapYear(year);
        return (month + (isLeapYear ? 6 : 5)) % (isLeapYear ? 13 : 12) + 1;
    }

    /// <summary>
    /// Validates the components of a Jewish date for validity. It will throw an <see cref="ArgumentException"/> if the Jewish
    /// date is earlier than 18 Teves, 3761 (1/1/1 Gregorian), a month &lt; 1 or &gt; 12 (or 13 on a <see cref="IsJewishLeapYear(int)">leap year</see>), the day of month is &lt; 1 or &gt; 30, an hour &lt; 0 or &gt; 23, a minute &lt; 0 or &gt; 59 or
    /// <em>chalakim</em> &lt; 0 or &gt; 17. For larger a larger number of <em>chalakim</em> such as 793 (TaShTzaG) break the
    /// <em>chalakim</em> into minutes (18 <em>chalakim</em> per minutes, so it would be 44 minutes and 1 <em>chelek</em> in the
    /// case of 793 / <em>TaShTzaG</em>).
    /// </summary>
    /// <param name="year">the Jewish year to validate. It will reject any year &lt;= 3761 (lower than the year 1 Gregorian).</param>
    /// <param name="month">the Jewish month to validate. It will reject a month &lt; 1 or &gt; 12 (or 13 on a leap year) .</param>
    /// <param name="dayOfMonth">the day of the Jewish month to validate. It will reject any value &lt; 1 or &gt; 30 TODO: check calling
    ///         methods to see if there is any reason that the class can validate that 30 is invalid for some months.</param>
    /// <param name="hours">the hours (for <em>molad</em> calculations). It will reject an hour &lt; 0 or &gt; 23</param>
    /// <param name="minutes">the minutes (for <em>molad</em> calculations). It will reject a minute &lt; 0 or &gt; 59</param>
    /// <param name="chalakim">the <em>chalakim</em> / parts (for <em>molad</em> calculations). It will reject a <em>chalakim</em> &lt; 0 or
    ///         &gt; 17. For larger numbers such as 793 (<em>TaShTzaG</em>) break the <em>chalakim</em> into minutes (18
    ///         <em>chalakim</em> per minutes, so it would be 44  minutes and 1 <em>chelek</em> in the case of 793 / <em>TaShTzaG</em>)</param>
    /// <exception cref="ArgumentException">if a Jewish date earlier than 18 Teves, 3761 (1/1/1 Gregorian), a month &lt; 1 or &gt; 12 (or
    ///         13 on a leap year), the day of month is &lt; 1 or &gt; 30, an hour &lt; 0 or &gt; 23, a minute &lt; 0 or &gt; 59 or
    ///         <em>chalakim</em> &lt; 0 or &gt; 17. For larger a larger number of <em>chalakim</em> such as 793 (<em>TaShTzaG</em>)
    ///         break the <em>chalakim</em> into minutes (18 <em>chalakim</em> per minutes, so it would be 44 minutes and 1
    ///         <em>chelek</em> in the case of 793 (<em>TaShTzaG</em>).</exception>
    private static void ValidateJewishDate(int year, int month, int dayOfMonth, int hours, int minutes, int chalakim)
    {
        if (month < Nissan || month > GetLastMonthOfJewishYear(year))
        {
            throw new ArgumentException("The Jewish month has to be between 1 and 12 (or 13 on a leap year). "
                    + month + " is invalid for the year " + year + ".");
        }
        int monthLength = GetDaysInJewishMonth(year, month);
        if (dayOfMonth < 1 || dayOfMonth > monthLength)
        {
            HebrewDateFormatter hdf = new HebrewDateFormatter();
            string monthName = hdf.TransliteratedMonthList[month];
            throw new ArgumentException(
                    "The Jewish day of month can't be < 1 or > " + monthLength +
                            " for the month index " + monthName + ", day  " + dayOfMonth + " is invalid.");
        }
        // reject dates prior to 18 Teves, 3761 (1/1/1 AD). This restriction can be relaxed if the date coding is
        // changed/corrected
        if ((year < 3761) || (year == 3761 && (month >= Tishrei && month < Teves))
                || (year == 3761 && month == Teves && dayOfMonth < 18))
        {
            throw new ArgumentException(
                    "A Jewish date earlier than 18 Teves, 3761 (1/1/1 Gregorian) can't be set. " + year + ", " + month
                            + ", " + dayOfMonth + " is invalid.");
        }
        if (hours < 0 || hours > 23)
        {
            throw new ArgumentException("Hours < 0 or > 23 can't be set. " + hours + " is invalid.");
        }

        if (minutes < 0 || minutes > 59)
        {
            throw new ArgumentException("Minutes < 0 or > 59 can't be set. " + minutes + " is invalid.");
        }

        if (chalakim < 0 || chalakim > 17)
        {
            throw new ArgumentException(
                    "Chalakim/parts < 0 or > 17 can't be set. "
                            + chalakim
                            + " is invalid. For larger numbers such as 793 (TaShTzaG) break the <em>chalakim</em> into minutes (18 chalakim per minutes, so it would be 44 minutes and 1 chelek in the case of 793 (TaShTzaG)");
        }
    }

    /// <summary>
    /// Returns the number of days for a given Jewish year. ND+ER
    /// </summary>
    /// <param name="year">the Jewish year</param>
    /// <returns>the number of days for a given Jewish year.</returns>
    /// <seealso cref="IsCheshvanLong(int)"/>
    /// <seealso cref="IsKislevShort(int)"/>
    public static int GetDaysInJewishYear(int year)
    {
        return GetJewishCalendarElapsedDays(year + 1) - GetJewishCalendarElapsedDays(year);
    }

    /// <summary>
    /// Returns the number of days for the current year that the calendar is set to.
    /// </summary>
    /// <returns>the number of days for the Object's current Jewish year.</returns>
    /// <seealso cref="IsCheshvanLong(int)"/>
    /// <seealso cref="IsKislevShort(int)"/>
    /// <seealso cref="IsJewishLeapYear(int)"/>
    public virtual int GetDaysInJewishYear()
    {
        return GetDaysInJewishYear(JewishYear);
    }

    /// <summary>
    /// Returns if Cheshvan is long in a given Jewish year. The method name isLong is done since in a Kesidran (ordered)
    /// year Cheshvan is short. ND+ER
    /// </summary>
    /// <param name="year">the year</param>
    /// <returns>true if Cheshvan is long in Jewish year.</returns>
    /// <seealso cref="IsCheshvanLong(int)"/>
    /// <seealso cref="GetCheshvanKislevKviah"/>
    private static bool IsCheshvanLong(int year)
    {
        return GetDaysInJewishYear(year) % 10 == 5;
    }

    /// <summary>
    /// Returns if Cheshvan is long (30 days VS 29 days) for the current year that the calendar is set to. The method
    /// name isLong is done since in a Kesidran (ordered) year Cheshvan is short.
    /// </summary>
    /// <returns>true if Cheshvan is long for the current year that the calendar is set to</returns>
    /// <seealso cref="IsCheshvanLong(int)"/>
    public virtual bool IsCheshvanLong()
    {
        return IsCheshvanLong(JewishYear);
    }

    /// <summary>
    /// Returns if Kislev is short (29 days VS 30 days) in a given Jewish year. The method name isShort is done since in
    /// a Kesidran (ordered) year Kislev is long. ND+ER
    /// </summary>
    /// <param name="year">the Jewish year</param>
    /// <returns>true if Kislev is short for the given Jewish year.</returns>
    /// <seealso cref="IsKislevShort(int)"/>
    /// <seealso cref="GetCheshvanKislevKviah"/>
    private static bool IsKislevShort(int year)
    {
        return GetDaysInJewishYear(year) % 10 == 3;
    }

    /// <summary>
    /// Returns if the Kislev is short for the year that this class is set to. The method name isShort is done since in a
    /// Kesidran (ordered) year Kislev is long.
    /// </summary>
    /// <returns>true if Kislev is short for the year that this class is set to</returns>
    public virtual bool IsKislevShort()
    {
        return IsKislevShort(JewishYear);
    }

    /// <summary>
    /// Returns the Cheshvan and Kislev kviah (whether a Jewish year is short, regular or long). It will return
    /// <see cref="Shelaimim"/> if both cheshvan and kislev are 30 days, <see cref="Kesidran"/> if Cheshvan is 29 days and Kislev
    /// is 30 days and <see cref="Chaserim"/> if both are 29 days.
    /// </summary>
    /// <returns><see cref="Shelaimim"/> if both cheshvan and kislev are 30 days, <see cref="Kesidran"/> if Cheshvan is 29 days and
    ///        Kislev is 30 days and <see cref="Chaserim"/> if both are 29 days.</returns>
    /// <seealso cref="IsCheshvanLong(int)"/>
    /// <seealso cref="IsKislevShort(int)"/>
    public virtual int GetCheshvanKislevKviah()
    {
        if (IsCheshvanLong() && !IsKislevShort())
        {
            return Shelaimim;
        }
        else if (!IsCheshvanLong() && IsKislevShort())
        {
            return Chaserim;
        }
        else
        {
            return Kesidran;
        }
    }

    /// <summary>
    /// Returns the number of days of a Jewish month for a given month and year.
    /// </summary>
    /// <param name="year">the Jewish Year</param>
    /// <param name="month">the Jewish month</param>
    /// <returns>the number of days for a given Jewish month</returns>
    private static int GetDaysInJewishMonth(int year, int month)
    {
        if ((month == Iyar) || (month == Tammuz) || (month == Elul) || ((month == Cheshvan) && !(IsCheshvanLong(year)))
                || ((month == Kislev) && IsKislevShort(year)) || (month == Teves)
                || ((month == Adar) && !(IsJewishLeapYear(year))) || (month == AdarII))
        {
            return 29;
        }
        else
        {
            return 30;
        }
    }

    /// <summary>
    /// Returns the number of days of the Jewish month that the calendar is currently set to.
    /// </summary>
    /// <returns>the number of days for the Jewish month that the calendar is currently set to.</returns>
    public virtual int GetDaysInJewishMonth()
    {
        return GetDaysInJewishMonth(JewishYear, JewishMonth);
    }

    /// <summary>
    /// Computes and sets the Jewish date fields based on the provided absolute (Gregorian) date. The date may be before
    /// January 1, 0001, as the <em>molad</em> of Teves 3761 is, on December 12, 0000.
    /// </summary>
    /// <param name="gregorianAbsDate">the Gregorian absolute date.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    private void SetAbsDate(int gregorianAbsDate)
    {
        this.gregorianAbsDate = gregorianAbsDate;
        // Approximation from below
        jewishYear = (gregorianAbsDate - JewishEpoch) / 366;
        // Search forward for year from the approximation
        while (gregorianAbsDate >= JewishDateToAbsDate(jewishYear + 1, Tishrei, 1))
        {
            jewishYear++;
        }
        // Search forward for month from either Tishri or Nissan.
        if (gregorianAbsDate < JewishDateToAbsDate(jewishYear, Nissan, 1))
        {
            jewishMonth = Tishrei;// Start at Tishri
        }
        else
        {
            jewishMonth = Nissan;// Start at Nissan
        }
        while (gregorianAbsDate > JewishDateToAbsDate(jewishYear, jewishMonth, GetDaysInJewishMonth()))
        {
            jewishMonth++;
        }
        // Calculate the day by subtraction
        jewishDay = gregorianAbsDate - JewishDateToAbsDate(jewishYear, jewishMonth, 1) + 1;

        // day of week, with floorMod so that a date before January 1, 0001 gets the right day
        dayOfWeek = JavaMath.FloorMod(gregorianAbsDate, 7) + 1;
    }

    /// <summary>
    /// Returns the absolute date of Jewish date. ND+ER
    /// </summary>
    /// <param name="year">the Jewish year. The year can't be negative</param>
    /// <param name="month">the Jewish month starting with Nissan. Nissan expects a value of 1 etc. until Adar with a value of 12. For a
    ///         leap year, 13 will be the expected value for Adar II. Use the constants <see cref="JewishDate.Nissan"/> etc.</param>
    /// <param name="dayOfMonth">the Jewish day of month. valid values are 1-30. If the day of month is set to 30 for a month that only
    ///         has 29 days, the day will be set as 29.</param>
    /// <returns>the absolute date of the Jewish date.</returns>
    private static int JewishDateToAbsDate(int year, int month, int dayOfMonth)
    {
        int elapsed = GetDaysSinceStartOfJewishYear(year, month, dayOfMonth);
        // add elapsed days this year + Days in prior years + Days elapsed before absolute year 1
        return elapsed + GetJewishCalendarElapsedDays(year) + JewishEpoch;
    }

    /// <summary>
    /// Returns the <em>molad</em> for a given year and month. Returns a JewishDate <see cref="object"/> set to the date of the <em>molad</em>
    /// with the <see cref="MoladHours">hours</see>, <see cref="MoladMinutes">minutes</see> and <see cref="MoladChalakim">chalakim</see> set. In the current implementation, it sets the <em>molad</em> time based on a midnight date rollover. This
    /// means that Rosh Chodesh Adar II, 5771 with a <em>molad</em> of 7 <em>chalakim</em> past midnight on Shabbos 29 Adar I / March 5,
    /// 2011 12:00 AM and 7 chalakim, will have the following values: hours: 0, minutes: 0, Chalakim: 7.
    /// </summary>
    /// <returns>a JewishDate <see cref="object"/> set to the date of the <em>molad</em> with the <see cref="MoladHours">hours</see>,
    ///         <see cref="MoladMinutes">minutes</see> and <see cref="MoladChalakim"><em>chalakim</em></see> set.</returns>
    public virtual JewishDate GetMolad()
    {
        JewishDate moladDate = new JewishDate(GetChalakimSinceMoladTohu());
        if (moladDate.MoladHours >= 6)
        {
            moladDate.PlusDays(1);
        }
        moladDate.MoladHours = (moladDate.MoladHours + 18) % 24;
        return moladDate;
    }

    /// <summary>
    /// Returns the number of days from the Jewish epoch from the number of <em>chalakim</em> from the epoch passed in.
    /// </summary>
    /// <param name="chalakim">the number of <em>chalakim</em> since the beginning of Sunday prior to BaHaRaD</param>
    /// <returns>the number of days from the Jewish epoch</returns>
    private static int MoladToAbsDate(long chalakim)
    {
        return (int)(chalakim / ChalakimPerDay) + JewishEpoch;
    }

    /// <summary>
    /// Constructor that creates a JewishDate based on a <em>molad</em> passed in. The <em>molad</em> would be the number of
    /// <em>chalakim</em> / parts starting at the beginning of Sunday prior to the <em>Molad Tohu BeHaRaD</em> (<em>Be</em> =
    /// Monday, <em>Ha</em> = 5 hours and <em>RaD</em> = 204 <em>chalakim</em> / parts) - prior to the start of the Jewish
    /// calendar. <em>BeHaRaD</em> is 23:11:20 on Sunday night(5 hours 204/1080 <em>chalakim</em> after sunset on Sunday evening).
    /// </summary>
    /// <param name="molad">the number of <em>chalakim</em> since the beginning of Sunday prior to BaHaRaD</param>
    public JewishDate(long molad)
    {
        SetAbsDate(MoladToAbsDate(molad));
        int conjunctionDay = (int)(molad / ChalakimPerDay);
        int chalakim = (int)(molad - conjunctionDay * ChalakimPerDay);
        MoladHours = chalakim / ChalakimPerHour;
        chalakim = chalakim - (MoladHours * ChalakimPerHour);
        MoladMinutes = chalakim / ChalakimPerMinute;
        MoladChalakim = chalakim - moladMinutes * ChalakimPerMinute;
    }

    /// <summary>
    /// returns the number of days from Rosh Hashana of the date passed in, to the full date passed in.
    /// </summary>
    /// <param name="year">the Jewish year</param>
    /// <param name="month">the Jewish month</param>
    /// <param name="dayOfMonth">the day in the Jewish month</param>
    /// <returns>the number of days</returns>
    private static int GetDaysSinceStartOfJewishYear(int year, int month, int dayOfMonth)
    {
        int elapsedDays = dayOfMonth;
        // Before Tishrei (from Nissan to Tishrei), add days in prior months
        if (month < Tishrei)
        {
            // this year before and after Nissan.
            for (int m = Tishrei; m <= GetLastMonthOfJewishYear(year); m++)
            {
                elapsedDays += GetDaysInJewishMonth(year, m);
            }
            for (int m = Nissan; m < month; m++)
            {
                elapsedDays += GetDaysInJewishMonth(year, m);
            }
        }
        else
        { // Add days in prior months this year
            for (int m = Tishrei; m < month; m++)
            {
                elapsedDays += GetDaysInJewishMonth(year, m);
            }
        }
        return elapsedDays;
    }

    /// <summary>
    /// returns the number of days from Rosh Hashana of the date passed in, to the full date passed in.
    /// </summary>
    /// <returns>the number of days</returns>
    public virtual int GetDaysSinceStartOfJewishYear()
    {
        return GetDaysSinceStartOfJewishYear(JewishYear, JewishMonth, JewishDayOfMonth);
    }

    /// <summary>
    /// Creates a Jewish date based on a Jewish year, month and day of month.
    /// </summary>
    /// <param name="jewishYear">the Jewish year</param>
    /// <param name="jewishMonth">the Jewish month. The method expects a 1 for Nissan ... 12 for Adar and 13 for Adar II. Use the constants
    ///         <see cref="Nissan"/> ... <see cref="Adar"/> (or <see cref="AdarII"/> for a leap year Adar II) to avoid any confusion.</param>
    /// <param name="jewishDayOfMonth">the Jewish day of month. If 30 is passed in for a month with only 29 days (for example <see cref="Iyar"/>,
    ///         or <see cref="Kislev"/> in a year that <see cref="IsKislevShort(int)"/>), the 29th (last valid date of the month) will be set.</param>
    /// <exception cref="ArgumentException">if the day of month is &lt; 1 or &gt; 30, or a year of &lt; 0 is passed in.</exception>
    public JewishDate(int jewishYear, int jewishMonth, int jewishDayOfMonth)
    {
        SetJewishDate(jewishYear, jewishMonth, jewishDayOfMonth);
    }

    /// <summary>
    /// Default constructor will set a default date to the current system date.
    /// </summary>
    public JewishDate()
    {
        ResetDate();
    }

    /// <summary>
    /// A constructor that initializes the date to the <see cref="DateTimeOffset"/> parameter.
    /// </summary>
    /// <param name="zonedDateTime">the <c>ZonedDateTime</c> to set the calendar to</param>
    public JewishDate(DateTimeOffset zonedDateTime)
    {
        SetGregorianDate(zonedDateTime);
    }

    /// <summary>
    /// A constructor that initializes the date to the <see cref="DateOnly"/> parameter.
    /// </summary>
    /// <param name="localDate">the <c>LocalDate</c> to set the calendar to</param>
    public JewishDate(DateOnly localDate)
    {
        SetGregorianDate(localDate);
    }

    /// <summary>
    /// Sets the date based on a <see cref="DateTimeOffset"/> object. Modifies the Jewish date as well.
    /// </summary>
    /// <param name="zonedDateTime">the <see cref="DateTimeOffset"/> to set the calendar to</param>
    public virtual void SetGregorianDate(DateTimeOffset zonedDateTime)
    {
        SetGregorianDate(DateOnly.FromDateTime(zonedDateTime.DateTime));
    }

    /// <summary>
    /// Sets the date based on a <see cref="DateOnly"/> object. Modifies the Jewish date as well.
    /// </summary>
    /// <param name="localDate">the <c>LocalDate</c> to set the calendar to</param>
    /// <exception cref="ArgumentException">if the date would fall prior to the year 1 AD</exception>
    public virtual void SetGregorianDate(DateOnly localDate)
    {
        int absDate = GregorianDateToAbsDate(localDate.Year, localDate.Month, localDate.Day);
        if (absDate <= 0)
        {
            throw new ArgumentException("Dates in the BC era are not supported");
        }
        SetAbsDate(absDate); // convert to Jewish date
    }

    /// <summary>
    /// Sets the Jewish Date and updates the Gregorian date accordingly.
    /// </summary>
    /// <param name="year">the Jewish year. The year can't be negative</param>
    /// <param name="month">the Jewish month starting with Nissan. A value of 1 is expected for Nissan ... 12 for Adar and 13 for Adar II.
    ///         Use the constants <see cref="Nissan"/> ... <see cref="Adar"/> (or <see cref="AdarII"/> for a leap year Adar II) to avoid any
    ///         confusion.</param>
    /// <param name="dayOfMonth">the Jewish day of month. valid values are 1-30. If the day of month is set to 30 for a month that only
    ///         has 29 days, the day will be set as 29.</param>
    /// <exception cref="ArgumentException">if a Jewish date earlier than 18 Teves, 3761 (1/1/1 Gregorian), a month &lt; 1 or &gt; 12
    ///         (or 13 on a leap year) or the day of month is &lt; 1 or &gt; 30 is passed in</exception>
    public virtual void SetJewishDate(int year, int month, int dayOfMonth)
    {
        SetJewishDate(year, month, dayOfMonth, MoladHours, MoladMinutes, MoladChalakim);
    }

    /// <summary>
    /// Sets the Jewish Date and updates the Gregorian date accordingly.
    /// </summary>
    /// <param name="year">the Jewish year. The year can't be negative</param>
    /// <param name="month">the Jewish month starting with Nissan. A value of 1 is expected for Nissan ... 12 for Adar and 13 for Adar II.
    ///         Use the constants <see cref="Nissan"/> ... <see cref="Adar"/> (or <see cref="AdarII"/> for a leap year Adar II) to avoid any
    ///         confusion.</param>
    /// <param name="dayOfMonth">the Jewish day of month. valid values are 1-30. If the day of month is set to 30 for a month that only
    ///         has 29 days, the day will be set as 29.</param>
    /// <param name="hours">the hour of the day. Used for <em>molad</em> calculations</param>
    /// <param name="minutes">the minutes. Used for <em>molad</em> calculations</param>
    /// <param name="chalakim">the <em>chalakim</em> / parts. Used for <em>molad</em> calculations. The <em>chalakim</em> should not exceed
    ///         17. Minutes should be used for larger numbers.</param>
    /// <exception cref="ArgumentException">if a Jewish date earlier than 18 Teves, 3761 (1/1/1 Gregorian), a month &lt; 1 or &gt; 12 (or
    ///         13 on a leap year), the day of month is &lt; 1 or &gt; 30, an hour &lt; 0 or &gt; 23, a minute &lt; 0 &gt; 59 or
    ///         <em>chalakim</em> &lt; 0 &gt; 17. For larger a larger number of <em>chalakim</em> such as 793 (<em>TaShTzaG</em>)
    ///         break the <em>chalakim</em> into minutes (18 <em>chalakim</em> per minutes, so it would be 44 minutes and 1
    ///         <em>chelek</em> in the case of 793 (<em>TaShTzaG</em>).</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void SetJewishDate(int year, int month, int dayOfMonth, int hours, int minutes, int chalakim)
    {

        ValidateJewishDate(year, month, dayOfMonth, hours, minutes, chalakim);

        jewishYear = year;
        jewishMonth = month;
        jewishDay = dayOfMonth;
        moladHours = hours;
        moladMinutes = minutes;
        moladChalakim = chalakim;

        gregorianAbsDate = JewishDateToAbsDate(jewishYear, jewishMonth, jewishDay); // reset Gregorian date

        dayOfWeek = Math.Abs(gregorianAbsDate % 7) + 1; // reset day of week
    }





    /// <summary>
    /// Returns this object's date as a <see cref="DateOnly"/> object.
    /// </summary>
    /// <returns>The <see cref="DateOnly"/></returns>
    public DateOnly LocalDate => AbsDateToDate(AbsDate);

    /// <summary>
    /// Resets this date to the current system date.
    /// </summary>
    public virtual void ResetDate()
    {
        DateOnly localDate = DateOnly.FromDateTime(DateTime.Now);
        SetGregorianDate(localDate);
    }

    /// <summary>
    /// Subtracts the number of days passed in from the currently set date.
    /// </summary>
    /// <param name="days">the number of days to subtract.</param>
    /// <seealso cref="JewishDate.PlusDays"/>
    public virtual void MinusDays(int days)
    {
        if (days < 1)
        {
            throw new ArgumentException("The number of days to subtract must be greater than zero.");
        }
        int absDate = AbsDate - days;
        if (absDate <= 0)
        {
            throw new ArgumentException("Dates in the BC era are not supported");
        }
        SetAbsDate(absDate);

    }

    /// <summary>
    /// Add the number of days passed in to the currently set date.
    /// </summary>
    /// <param name="days">the number of days to add.</param>
    /// <seealso cref="JewishDate.MinusDays"/>
    public virtual void PlusDays(int days)
    {
        if (days < 1)
        {
            throw new ArgumentException("The number of days to add must be greater than zero. Use minusDays(int) to subtract days.");
        }
        SetAbsDate(AbsDate + days);
    }

    /// <summary>
    /// Add the number of months passed in to the currently set date. If the day of the month prior to addition is the 30th, and
    /// the target month only has 29 days, the date will be clamped to the 29th.
    /// </summary>
    /// <param name="months">the number of months to add.</param>
    /// <seealso cref="JewishDate.MinusMonths"/>
    public virtual void PlusMonths(int months)
    {
        if (months < 1)
        {
            throw new ArgumentException("The number of months to add must be greater than zero. Use minusMonths(int) to subtract months.");
        }
        int year = JewishYear;
        int month = JewishMonth;
        for (int i = 0; i < months; i++)
        {
            if (month == Elul)
            {
                month = Tishrei;
                year++;
            }
            else if ((!IsJewishLeapYear(year) && month == Adar)
                    || (IsJewishLeapYear(year) && month == AdarII))
            {
                month = Nissan;
            }
            else
            {
                month++;
            }
        }
        int day = Math.Min(JewishDayOfMonth, GetDaysInJewishMonth(year, month));
        SetJewishDate(year, month, day);
    }

    /// <summary>
    /// Subtracts the number of months passed in to the currently set date. If the day of the month prior to subtraction
    /// is the 30th, and the target month only has 29 days, the date will be clamped to the 29th.
    /// </summary>
    /// <param name="months">the number of months to subtract.</param>
    /// <seealso cref="JewishDate.PlusMonths"/>
    public virtual void MinusMonths(int months)
    {
        if (months < 1)
        {
            throw new ArgumentException("The number of months to subtract must be greater than zero.");
        }
        int year = JewishYear;
        int month = JewishMonth;
        for (int i = 0; i < months; i++)
        {
            if (month == Tishrei)
            {
                month = Elul;
                year--;
            }
            else if (month == Nissan)
            {
                month = GetLastMonthOfJewishYear(year);
            }
            else if (!IsJewishLeapYear(year) && month == Adar)
            {
                month = Shevat;
            }
            else
            {
                month--;
            }
        }
        int day = Math.Min(JewishDayOfMonth, GetDaysInJewishMonth(year, month));
        SetJewishDate(year, month, day);
    }

    /// <summary>
    /// Add the number of years passed in to the currently set date. If the current month is Adar on a non-leap year,
    /// and the year after the addition will be a leap year, passing <c>true</c> to the useAdarAlephForLeapYear
    /// parameter will set the month to Adar I, and passing <c>false</c> will forward it to Adar II. The
    /// useAdarAlephForLeapYear will be ignored if the month is not Adar. If the current year is a leap year and it is
    /// currently Adar I or Adar II and the year it is being increased to is also a leap year, the same Adar will be used.
    /// If it is being increased to a non-leap year, the month will be set to Adar. It is important to keep in mind when
    /// calculating <em>yahrzits</em> that are on the 30th and the target year only has 29 days, that the date will be set
    /// to the 29th, something that may nt be desirable.
    /// </summary>
    /// <param name="years">the number of years to add</param>
    /// <param name="useAdarAlephForLeapYear">if set to true and the current month is Adar on a non-leap year and it is being moved
    ///         forward to a leap year, it will be set to Adar I, and if set to false it will set to Adar II. This will be ignored if
    ///         the month is not set to Adar.</param>
    /// <seealso cref="JewishDate.MinusYears"/>
    public virtual void PlusYears(int years, bool useAdarAlephForLeapYear)
    {
        if (years < 1)
        {
            throw new ArgumentException("The number of years to add has to be greater than zero. Use minusYears(int, boolean) to subtract years.");
        }
        int targetYear = JewishYear + years;
        // If we are in the month of Adar in a non-leap year and we are skipping
        // to a year that is a leap year, we will use useAdarAlephInYear do
        // decide which month to skip to.
        int month;
        if (JewishMonth == JewishDate.Adar && !IsJewishLeapYear(JewishYear) && IsJewishLeapYear(targetYear))
        {
            if (useAdarAlephForLeapYear)
            {
                month = JewishDate.Adar;
            }
            else
            {
                month = JewishDate.AdarII;
            }
        }
        else
        {
            // If we are in JewishDate.ADAR_II, this will clamp the month to 12 (JewishDate.ADAR)
            month = Math.Min(JewishMonth, GetLastMonthOfJewishYear(targetYear));
        }
        // Clamp to final day of the month
        int day = Math.Min(JewishDayOfMonth, GetDaysInJewishMonth(targetYear, month));
        SetJewishDate(targetYear, month, day);
    }

    /// <summary>
    /// Subtract the number of years passed in to the currently set date. If the current month is Adar on a non-leap year,
    /// and the year after the subtraction will be a leap year, passing <c>true</c> to the useAdarAlephForLeapYear
    /// parameter will set the month to Adar I, and passing <c>false</c> will set it to Adar II. The
    /// useAdarAlephForLeapYear will be ignored if the current month is not Adar on a non-leap year. If the current year
    /// is a leap year and it is currently Adar I or Adar II and the year it is being decreased to is also a leap year,
    /// the same Adar will be used. If it is being decreased to a non-leap year, the month will be set to Adar.
    /// </summary>
    /// <param name="years">the number of years to subtract</param>
    /// <param name="useAdarAlephForLeapYear">if set to true and the current month is Adar on a non-leap year and it is being
    ///         subtracted to a leap year, it will be set to Adar I, and if set to false it will set to Adar II. This will
    ///         be ignored if the month is not set to Adar.</param>
    /// <seealso cref="JewishDate.PlusYears"/>
    public virtual void MinusYears(int years, bool useAdarAlephForLeapYear)
    {
        if (years < 1)
        {
            throw new ArgumentException("The number of years to subtract has to be greater than zero.");
        }
        int targetYear = JewishYear - years;
        // If we are in the month of Adar in a non-leap year and we are skipping
        // to a year that is a leap year, we will use useAdarAlephInYear do
        // decide which month to skip to.
        int month;
        if (JewishMonth == JewishDate.Adar && !IsJewishLeapYear(JewishYear) && IsJewishLeapYear(targetYear))
        {
            if (useAdarAlephForLeapYear)
            {
                month = JewishDate.Adar;
            }
            else
            {
                month = JewishDate.AdarII;
            }
        }
        else
        {
            // If we are in JewishDate.ADAR_II, this will clamp the month to 12 (JewishDate.ADAR)
            month = Math.Min(JewishMonth, GetLastMonthOfJewishYear(targetYear));
        }
        // Clamp to final day of the month
        int day = Math.Min(JewishDayOfMonth, GetDaysInJewishMonth(targetYear, month));
        SetJewishDate(targetYear, month, day);
    }

    /// <summary>
    /// Returns a string containing the Jewish date in the form, "day Month, year" e.g. "21 Shevat, 5729". For more
    /// complex formatting, use the formatter classes.
    /// </summary>
    /// <returns>the Jewish date in the form "day Month, year" e.g. "21 Shevat, 5729"</returns>
    /// <seealso cref="HebrewDateFormatter.Format"/>
    public override string ToString()
    {
        return new HebrewDateFormatter().Format(this);
    }

    /// <summary>
    /// Indicates whether some other object is "equal to" this one.
    /// </summary>
    /// <seealso cref="object.Equals(object)"/>
    public virtual bool Equals(JewishDate? other)
    {
        return other is not null && GetType() == other.GetType() && gregorianAbsDate == other.AbsDate;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        JewishDate jewishDate = (JewishDate)obj;
        return gregorianAbsDate == jewishDate.AbsDate;
    }

    /// <summary>
    /// Compares two dates as per the compareTo() method in the Comparable interface. Returns a value less than 0 if this
    /// date is "less than" (before) the date, greater than 0 if this date is "greater than" (after) the date, or 0 if
    /// they are equal.
    /// </summary>
    public virtual int CompareTo(JewishDate? jewishDate)
    {
        return jewishDate is null ? 1 : gregorianAbsDate.CompareTo(jewishDate.AbsDate);
    }

    /// <summary>
    /// Returns the Jewish month 1-12 (or 13 years in a leap year). The month count starts with 1 for Nissan and goes to
    /// 13 for Adar II
    /// </summary>
    /// <value>the Jewish month from 1 to 12 (or 13 years in a leap year). The month count starts with 1 for Nissan and
    ///         goes to 13 for Adar II</value>
    /// <remarks>Setting: Setter for the Jewish month that is passed in. If the day of month is currently the 30th and the month is being set to a month that only has 29 days, the day of month will be <a href="https://en.wikipedia.org/wiki/Clamp_(function)" >clamped</a> to the 29th of the month.</remarks>
    public int JewishMonth
    {
        get => jewishMonth;
        set
        {
            int year = JewishYear;
            int day = Math.Min(GetDaysInJewishMonth(year, value), JewishDayOfMonth);
            SetJewishDate(year, value, day);
        }
    }

    /// <summary>
    /// Returns the Jewish day of month.
    /// </summary>
    /// <value>the Jewish day of the month</value>
    /// <remarks>Setting: Setter for the Jewish day of the month that will be <a href="https://en.wikipedia.org/wiki/Clamp_(function)" >clamped</a> to the lesser of the number passed in or the max number of days in the month.</remarks>
    public int JewishDayOfMonth
    {
        get => jewishDay;
        set
        {
            SetJewishDate(JewishYear, JewishMonth, value);
        }
    }

    /// <summary>
    /// Returns the Jewish year.
    /// </summary>
    /// <value>the Jewish year</value>
    /// <remarks>Setting: Setter for the Jewish year of the passed in that will <a href="https://en.wikipedia.org/wiki/Clamp_(function)" >clamp</a> the day to the month to the lesser of the current day and the max number of days in the month (if set to the 30th).  Note that if you are using this for a yahrzeit (or any other reason)on the 30th of the month that will not always have 30 days, such as <see cref="Cheshvan"/> or <see cref="Kislev"/> or <see cref="Adar">ADAR I</see> on a leap year and the next year is a non-leap year, you must clone your date or once it is set to the 29th, the next time you forward it to a year that has 30 days, the calendar will incorrectly be forwarded a year from the 29th to the 29th and not the 30th that you may expect.</remarks>
    public int JewishYear
    {
        get => jewishYear;
        set
        {
            int month = Math.Min(JewishMonth, GetLastMonthOfJewishYear(value));
            int day = Math.Min(JewishDayOfMonth, GetDaysInJewishMonth(value, month));
            SetJewishDate(value, month, day);
        }
    }

    /// <summary>
    /// Returns the day of the week as a number between 1-7.
    /// </summary>
    /// <value>the day of the week as a number between 1-7.</value>
    public int DayOfWeek => dayOfWeek;

    /// <summary>
    /// A method that creates a <a href="http://en.wikipedia.org/wiki/Object_copy#Deep_copy">deep copy</a> of the object.
    /// </summary>
    public virtual JewishDate Clone()
    {
        return (JewishDate)MemberwiseClone();
    }

    /// <summary>
    /// Overrides <see cref="object.GetHashCode"/>.
    /// </summary>
    /// <seealso cref="object.GetHashCode"/>
    public override int GetHashCode()
    {
        return gregorianAbsDate.GetHashCode();
    }

    /// <summary>Returns whether <paramref name="left"/> is before <paramref name="right"/> (a <c>null</c> sorts first).</summary>
    /// <param name="left">the first date.</param>
    /// <param name="right">the second date.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is earlier.</returns>
    public static bool operator <(JewishDate? left, JewishDate? right) => Compare(left, right) < 0;

    /// <summary>Returns whether <paramref name="left"/> is after <paramref name="right"/> (a <c>null</c> sorts first).</summary>
    /// <param name="left">the first date.</param>
    /// <param name="right">the second date.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is later.</returns>
    public static bool operator >(JewishDate? left, JewishDate? right) => Compare(left, right) > 0;

    /// <summary>Returns whether <paramref name="left"/> is on or before <paramref name="right"/> (a <c>null</c> sorts first).</summary>
    /// <param name="left">the first date.</param>
    /// <param name="right">the second date.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is the same day or earlier.</returns>
    public static bool operator <=(JewishDate? left, JewishDate? right) => Compare(left, right) <= 0;

    /// <summary>Returns whether <paramref name="left"/> is on or after <paramref name="right"/> (a <c>null</c> sorts first).</summary>
    /// <param name="left">the first date.</param>
    /// <param name="right">the second date.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is the same day or later.</returns>
    public static bool operator >=(JewishDate? left, JewishDate? right) => Compare(left, right) >= 0;

    private static int Compare(JewishDate? left, JewishDate? right) =>
        left is null ? (right is null ? 0 : -1) : left.CompareTo(right);
}
