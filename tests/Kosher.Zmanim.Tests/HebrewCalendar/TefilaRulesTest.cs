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
/// Coverage for the <see cref="TefilaRules"/> daily-tefila predicates on two representative dates: an ordinary weekday during
/// the summer, and the first day of Sukkos.
/// </summary>
public class TefilaRulesTest
{

    private TefilaRules rules = new TefilaRules();

    [Fact]
    public void OrdinarySummerWeekday()
    {
        JewishCalendar date = new JewishCalendar(new DateOnly(2023, 8, 21));

        AssertTrue(rules.IsTachanunRecitedShacharis(date));
        AssertTrue(rules.IsTachanunRecitedMincha(date));
        AssertFalse(rules.IsVeseinTalUmatarStartDate(date));
        AssertFalse(rules.IsVeseinTalUmatarStartingTonight(date));
        AssertFalse(rules.IsVeseinTalUmatarRecited(date));
        AssertTrue(rules.IsVeseinBerachaRecited(date));
        AssertFalse(rules.IsMashivHaruachStartDate(date));
        AssertFalse(rules.IsMashivHaruachEndDate(date));
        AssertFalse(rules.IsMashivHaruachRecited(date));
        AssertTrue(rules.IsMoridHatalRecited(date));
        AssertFalse(rules.IsHallelRecited(date));
        AssertFalse(rules.IsHallelShalemRecited(date));
        AssertFalse(rules.IsAlHanissimRecited(date));
        AssertFalse(rules.IsYaalehVeyavoRecited(date));
        AssertTrue(rules.IsMizmorLesodaRecited(date));
    }

    [Fact]
    public void FirstDayOfSukkos()
    {
        JewishCalendar date = new JewishCalendar(new DateOnly(2023, 10, 7));

        AssertFalse(rules.IsTachanunRecitedShacharis(date));
        AssertFalse(rules.IsTachanunRecitedMincha(date));
        AssertFalse(rules.IsVeseinTalUmatarStartDate(date));
        AssertFalse(rules.IsVeseinTalUmatarStartingTonight(date));
        AssertFalse(rules.IsVeseinTalUmatarRecited(date));
        AssertTrue(rules.IsVeseinBerachaRecited(date));
        AssertTrue(rules.IsMashivHaruachStartDate(date));
        AssertFalse(rules.IsMashivHaruachEndDate(date));
        AssertFalse(rules.IsMashivHaruachRecited(date));
        AssertTrue(rules.IsMoridHatalRecited(date));
        AssertTrue(rules.IsHallelRecited(date));
        AssertTrue(rules.IsHallelShalemRecited(date));
        AssertFalse(rules.IsAlHanissimRecited(date));
        AssertTrue(rules.IsYaalehVeyavoRecited(date));
        AssertFalse(rules.IsMizmorLesodaRecited(date));
    }
}
