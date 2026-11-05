/*
 * Zmanim Java API
 * Copyright © 2011 - 2026 Eliyahu Hershfeld
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

namespace Kosher.Zmanim.HebrewCalendar;

/// <summary>
/// An Object representing a <em>daf</em> (page) in the <a href="https://en.wikipedia.org/wiki/Daf_Yomi">Daf Yomi</a> cycle.
/// </summary>
public class Daf : IEquatable<Daf>
{
    /// <summary>
    /// See <see cref="MasechtaNumber"/> and <see cref="MasechtaNumber"/>.
    /// </summary>
    private int masechtaNumber;

    /// <summary>
    /// See <see cref="Daf"/> and <see cref="Daf"/>.
    /// </summary>
    private int daf;

    /// <summary>
    /// See <see cref="GetMasechtaTransliterated"/> and <see cref="SetMasechtaTransliterated"/>.
    /// </summary>
    private static string[] masechtosBavliTransliterated = { "Berachos", "Shabbos", "Eruvin", "Pesachim", "Shekalim",
            "Yoma", "Sukkah", "Beitzah", "Rosh Hashana", "Taanis", "Megillah", "Moed Katan", "Chagigah", "Yevamos",
            "Kesubos", "Nedarim", "Nazir", "Sotah", "Gitin", "Kiddushin", "Bava Kamma", "Bava Metzia", "Bava Basra",
            "Sanhedrin", "Makkos", "Shevuos", "Avodah Zarah", "Horiyos", "Zevachim", "Menachos", "Chullin", "Bechoros",
            "Arachin", "Temurah", "Kerisos", "Meilah", "Kinnim", "Tamid", "Midos", "Niddah" };

    /// <summary>
    /// See <see cref="GetMasechta"/>.
    /// </summary>
    private static readonly string[] masechtosBavli = { "ברכות", "שבת", "עירובין", "פסחים", "שקלים", "יומא", "סוכה", "ביצה", "ראש השנה",
            "תענית", "מגילה", "מועד קטן", "חגיגה", "יבמות", "כתובות", "נדרים", "נזיר", "סוטה", "גיטין", "קידושין", "בבא קמא", "בבא מציעא",
            "בבא בתרא", "סנהדרין", "מכות", "שבועות", "עבודה זרה", "הוריות", "זבחים", "מנחות", "חולין", "בכורות", "ערכין", "תמורה", "כריתות",
            "מעילה", "קינים", "תמיד", "מידות", "נדה" };

    /// <summary>
    /// See <see cref="GetYerushalmiMasechtaTransliterated"/>.
    /// </summary>
    private static string[] masechtosYerushalmiTransliterated = { "Berachos", "Pe'ah", "Demai", "Kilayim", "Shevi'is",
            "Terumos", "Ma'asros", "Ma'aser Sheni", "Chalah", "Orlah", "Bikurim", "Shabbos", "Eruvin", "Pesachim",
            "Beitzah", "Rosh Hashanah", "Yoma", "Sukah", "Ta'anis", "Shekalim", "Megilah", "Chagigah", "Moed Katan",
            "Yevamos", "Kesuvos", "Sotah", "Nedarim", "Nazir", "Gitin", "Kidushin", "Bava Kama", "Bava Metzia",
            "Bava Basra", "Shevuos", "Makos", "Sanhedrin", "Avodah Zarah", "Horayos", "Nidah", "No Daf Today" };

    /// <summary>
    /// See <see cref="GetYerushalmiMasechta"/>.
    /// </summary>
    private static readonly string[] masechtosYerushalmi = { "ברכות", "פיאה", "דמאי", "כלאים", "שביעית", "תרומות", "מעשרות","מעשר שני",
            "חלה", "עורלה", "ביכורים", "שבת", "עירובין", "פסחים", "ביצה", "ראש השנה", "יומא", "סוכה", "תענית", "שקלים", "מגילה", "חגיגה",
            "מועד קטן", "יבמות", "כתובות", "סוטה", "נדרים", "נזיר", "גיטין", "קידושין", "בבא קמא", "בבא מציעא", "בבא בתרא", "שבועות", "מכות",
            "סנהדרין", "עבודה זרה", "הוריות", "נידה", "אין דף היום" };

    /// <summary>
    /// Gets the <em>masechta</em> number of the currently set <em>Daf</em>. The sequence is: Berachos, Shabbos, Eruvin,
    /// Pesachim, Shekalim, Yoma, Sukkah, Beitzah, Rosh Hashana, Taanis, Megillah, Moed Katan, Chagigah, Yevamos, Kesubos,
    /// Nedarim, Nazir, Sotah, Gitin, Kiddushin, Bava Kamma, Bava Metzia, Bava Basra, Sanhedrin, Makkos, Shevuos, Avodah
    /// Zarah, Horiyos, Zevachim, Menachos, Chullin, Bechoros, Arachin, Temurah, Kerisos, Meilah, Kinnim, Tamid, Midos and
    /// Niddah.
    /// </summary>
    /// <value>the <em>masechta</em> number in the order of the Daf Yomi to set.</value>
    /// <remarks>
    /// Set the <em>masechta</em> number in the order of the Daf Yomi. The sequence is: Berachos, Shabbos, Eruvin, Pesachim,
    /// Shekalim, Yoma, Sukkah, Beitzah, Rosh Hashana, Taanis, Megillah, Moed Katan, Chagigah, Yevamos, Kesubos, Nedarim,
    /// Nazir, Sotah, Gitin, Kiddushin, Bava Kamma, Bava Metzia, Bava Basra, Sanhedrin, Makkos, Shevuos, Avodah Zarah,
    /// Horiyos, Zevachim, Menachos, Chullin, Bechoros, Arachin, Temurah, Kerisos, Meilah, Kinnim, Tamid, Midos and
    /// Niddah.
    /// </remarks>
    /// <seealso cref="MasechtaNumber"/>
    public int MasechtaNumber
    {
        get => masechtaNumber;
        set => masechtaNumber = value;
    }

    /// <summary>
    /// Constructor that creates a Daf setting the <see cref="MasechtaNumber"><em>masechta</em> number</see> and
    /// <see cref="Daf"><em>daf</em> number</see>.
    /// </summary>
    /// <param name="masechtaNumber">the <em>masechta</em> number in the order of the Daf Yomi to set as the current <em>masechta</em>.</param>
    /// <param name="daf">the <em>daf</em> (page) number to set.</param>
    public Daf(int masechtaNumber, int daf)
    {
        this.masechtaNumber = masechtaNumber;
        this.daf = daf;
    }

    /// <summary>
    /// The <em>daf</em> (page) number of the Daf Yomi (<c>getDaf()</c> / <c>setDaf()</c> in KosherJava).
    /// </summary>
    public int DafNumber
    {
        get => daf;
        set => daf = value;
    }

    /// <summary>
    /// Returns the transliterated name of the <em>masechta</em> (tractate) of the Daf Yomi. The list of <em>masechtos</em>
    /// is: Berachos, Shabbos, Eruvin, Pesachim, Shekalim, Yoma, Sukkah, Beitzah, Rosh Hashana, Taanis, Megillah, Moed Katan,
    /// Chagigah, Yevamos, Kesubos, Nedarim, Nazir, Sotah, Gitin, Kiddushin, Bava Kamma, Bava Metzia, Bava Basra, Sanhedrin,
    /// Makkos, Shevuos, Avodah Zarah, Horiyos, Zevachim, Menachos, Chullin, Bechoros, Arachin, Temurah, Kerisos, Meilah,
    /// Kinnim, Tamid, Midos and Niddah.
    /// </summary>
    /// <returns>the transliterated name of the <em>masechta</em> (tractate) of the Daf Yomi such as Berachos.</returns>
    /// <seealso cref="BavliMasechtosTransliterated"/>
    public string GetMasechtaTransliterated()
    {
        return masechtosBavliTransliterated[masechtaNumber];
    }

    /// <summary>
    /// The list of Bavli <em>masechtos</em> transliterated into Latin chars, used by <see cref="GetMasechtaTransliterated"/>.
    /// The default values use Ashkenazi American English transliteration. Set it to override them (for example with a
    /// <em>Sephardi</em> transliteration). The list is shared by all <see cref="Daf"/> instances.
    /// </summary>
    /// <seealso cref="GetMasechtaTransliterated"/>
    public static string[] BavliMasechtosTransliterated
    {
        get => masechtosBavliTransliterated;
        set => masechtosBavliTransliterated = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>The KosherJava setter for <see cref="BavliMasechtosTransliterated"/>.</summary>
    /// <param name="masechtosBavliTransliterated">the list of transliterated Bavli <em>masechtos</em> to set.</param>
    [Obsolete("Use the static BavliMasechtosTransliterated property; SetMasechtaTransliterated is the KosherJava name. This alias will be removed in a future version.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void SetMasechtaTransliterated(string[] masechtosBavliTransliterated)
    {
        BavliMasechtosTransliterated = masechtosBavliTransliterated;
    }

    /// <summary>
    /// Returns the <em>masechta</em> (tractate) of the Daf Yomi in Hebrew. The list is in the following format<br/>
    /// <c>["ברכות", "שבת", "עירובין", "פסחים", "שקלים", "יומא", "סוכה", "ביצה", "ראש השנה", "תענית", "מגילה", "מועד קטן", "חגיגה", "יבמות", "כתובות", "נדרים","נזיר", "סוטה", "גיטין",
    /// "קידושין", "בבא קמא", "בבא מציעא", "בבא בתרא", "סנהדרין", "מכות", "שבועות", "עבודה זרה", "הוריות", "זבחים", "מנחות", "חולין", "בכורות", "ערכין", "תמורה", "כריתות", "מעילה",
    /// "קינים", "תמיד", "מידות", "נדה"]</c>.
    /// </summary>
    /// <returns>the <em>masechta</em> (tractate) of the Daf Yomi in Hebrew. As an example, it will return ברכות for Berachos.</returns>
    public string GetMasechta()
    {
        return masechtosBavli[masechtaNumber];
    }

    /// <summary>
    /// Returns the transliterated name of the <em>masechta</em> (tractate) of the Daf Yomi in Yerushalmi. The list of
    /// <em>masechtos</em> is:
    /// Berachos, Pe'ah, Demai, Kilayim, Shevi'is, Terumos, Ma'asros, Ma'aser Sheni, Chalah, Orlah, Bikurim,
    /// Shabbos, Eruvin, Pesachim, Beitzah, Rosh Hashanah, Yoma, Sukah, Ta'anis, Shekalim, Megilah, Chagigah,
    /// Moed Katan, Yevamos, Kesuvos, Sotah, Nedarim, Nazir, Gitin, Kidushin, Bava Kama, Bava Metzia,
    /// Bava Basra, Shevuos, Makos, Sanhedrin, Avodah Zarah, Horayos, Nidah and No Daf Today.
    /// </summary>
    /// <returns>the transliterated name of the <em>masechta</em> (tractate) of the Daf Yomi such as Berachos.</returns>
    public string GetYerushalmiMasechtaTransliterated()
    {
        return masechtosYerushalmiTransliterated[masechtaNumber];
    }

    /// <summary>
    /// The list of Yerushalmi <em>masechtos</em> transliterated into Latin chars, used by
    /// <see cref="GetYerushalmiMasechtaTransliterated"/>. The default uses Ashkenazi American English transliteration. Set it to
    /// override them. The list is shared by all <see cref="Daf"/> instances.
    /// </summary>
    public static string[] YerushalmiMasechtosTransliterated
    {
        get => masechtosYerushalmiTransliterated;
        set => masechtosYerushalmiTransliterated = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// The Hebrew names of the Yerushalmi <em>masechtos</em> (tractates), used by <see cref="GetYerushalmiMasechta"/>.
    /// </summary>
    public static string[] YerushalmiMasechtos => masechtosYerushalmi;

    /// <summary>The KosherJava setter for <see cref="YerushalmiMasechtosTransliterated"/>.</summary>
    /// <param name="masechtosYerushalmiTransliterated">the list of transliterated Yerushalmi <em>masechtos</em> to set.</param>
    [Obsolete("Use the static YerushalmiMasechtosTransliterated property; SetYerushalmiMasechtaTransliterated is the KosherJava name. This alias will be removed in a future version.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void SetYerushalmiMasechtaTransliterated(string[] masechtosYerushalmiTransliterated)
    {
        YerushalmiMasechtosTransliterated = masechtosYerushalmiTransliterated;
    }

    /// <summary>The KosherJava getter for <see cref="YerushalmiMasechtosTransliterated"/>.</summary>
    /// <returns>the array of transliterated <em>masechta</em> (tractate) names of the Daf Yomi Yerushalmi.</returns>
    [Obsolete("Use the YerushalmiMasechtosTransliterated property; GetYerushalmiMasechtosTransliterated() is the KosherJava name. This alias will be removed in a future version.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static string[] GetYerushalmiMasechtosTransliterated() => YerushalmiMasechtosTransliterated;

    /// <summary>The KosherJava getter for <see cref="YerushalmiMasechtos"/>.</summary>
    /// <returns>the array of Hebrew <em>masechta</em> (tractate) names of the Daf Yomi Yerushalmi.</returns>
    [Obsolete("Use the YerushalmiMasechtos property; GetYerushalmiMasechtos() is the KosherJava name. This alias will be removed in a future version.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static string[] GetYerushalmiMasechtos() => YerushalmiMasechtos;

    /// <summary>
    /// Returns the Yerushalmi <em>masechta</em> (tractate) of the Daf Yomi in Hebrew. As an example, it will return ברכות for Berachos.
    /// </summary>
    /// <returns>the Yerushalmi <em>masechta</em> (tractate) of the Daf Yomi in Hebrew. As an example, it will return ברכות for Berachos.</returns>
    public string GetYerushalmiMasechta()
    {
        return masechtosYerushalmi[masechtaNumber];
    }

    /// <summary>
    /// Indicates whether this <see cref="Daf"/> is for the same <em>masechta</em> and <em>daf</em> as <paramref name="other"/>.
    /// </summary>
    /// <param name="other">the <see cref="Daf"/> to compare with.</param>
    /// <returns><c>true</c> if both refer to the same <em>daf</em>.</returns>
    public bool Equals(Daf? other) => other is not null && masechtaNumber == other.masechtaNumber && daf == other.daf;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Daf);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(masechtaNumber, daf);

    /// <inheritdoc/>
    public override string ToString() => GetMasechtaTransliterated() + " " + daf;
}
