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

namespace Kosher.Zmanim.Tests.HebrewCalendar;

/// <summary>
/// Coverage for <see cref="JewishCalendar"/> holiday / special-day predicates and <see cref="HebrewDateFormatter.FormatParshah"/>.
/// These are deterministic halachic classifications asserted against known dates.
/// </summary>
public class JewishCalendarHolidayTest
{

    [Fact]
    public void RoshHashanaAndTzomGedaliah()
    {
        JewishCalendar date = new JewishCalendar(new DateOnly(2023, 9, 16));
        AssertTrue(date.IsRoshHashana());

        date.PlusDays(1);
        AssertTrue(date.IsRoshHashana());

        date.PlusDays(1); // Tzom Gedaliah (3 Tishrei)
        AssertFalse(date.IsRoshHashana());
        AssertTrue(date.IsTaanis());
    }

    [Fact]
    public void Parshah()
    {
        HebrewDateFormatter formatter = new HebrewDateFormatter();
        AssertEquals("Bereshis", formatter.FormatParshah(new JewishCalendar(new DateOnly(2023, 10, 14))));

        HebrewDateFormatter hebrew = new HebrewDateFormatter();
        hebrew.IsHebrewFormat = true;
        AssertEquals("בראשית", hebrew.FormatParshah(new JewishCalendar(new DateOnly(2023, 10, 14))));

        AssertEquals("Terumah", formatter.FormatParshah(new JewishCalendar(new DateOnly(2024, 2, 17))));
    }

    [Fact]
    public void PurimAndShushanPurim()
    {
        JewishCalendar date = new JewishCalendar();

        // 14 Adar in a non-leap year: Purim in unwalled cities, but Shushan Purim (15th) in walled cities.
        date.SetJewishDate(5783, JewishDate.Adar, 14);
        AssertTrue(date.IsPurim());
        date.IsMukafChoma = true;
        AssertFalse(date.IsPurim());

        // 15 Adar: Shushan Purim, observed as Purim in walled cities.
        date.PlusDays(1);
        AssertTrue(date.IsPurim());
        date.IsMukafChoma = false;
        AssertFalse(date.IsPurim());

        // In a leap year Purim falls in Adar II, so Adar (I) 14 is not Purim.
        date.SetJewishDate(5784, JewishDate.Adar, 14);
        AssertFalse(date.IsPurim());
    }

    [Fact]
    public void AssurBemelacha()
    {
        // Yom Kippur (10 Tishrei) is assur bemelacha.
        JewishCalendar yomKippur = new JewishCalendar(5784, JewishDate.Tishrei, 10);
        AssertTrue(yomKippur.IsAssurBemelacha());

        // An ordinary weekday is not.
        JewishCalendar weekday = new JewishCalendar(5784, JewishDate.Cheshvan, 5);
        AssertFalse(weekday.IsAssurBemelacha());
    }
}
