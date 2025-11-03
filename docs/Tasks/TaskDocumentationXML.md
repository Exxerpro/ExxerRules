You are responsible for writing **complete XML documentation** for all public C# types and members in this project. 
F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\src

There is a Check list for you to keep a record of all public types to document these is the minimun expected,
XMLExxerRules_Checklist.md , be advice is a very large file please process on chunks
fill after documenting  each one , inspect all in order to check if they follow the rules of dotnet and if there are deviattion correct to a standard.

Your output must:


1. Strictly follow the **.NET XML documentation standard**, including:
   - `<summary>`, `<param>`, `<returns>`, `<remarks>` as appropriate
   - Correct `<typeparam>` for generics
   - Special tags for exceptions, references, examples if relevant

2. Match member visibility:
   - All public protected and private members are required
   - Internal and private members alsbo must documented for clarity and if the need arise to convert to public

3. Use clear, descriptive, and technically accurate language:
   - Avoid generic phrases like "Gets or sets the value."
   - Clarify purpose, behavior, units, constraints, or side effects.

4. Output must:
   - Include XML doc above each method/property/class/interface
   - If there is any atribute the Xml Doc must be above the atributes also
   - Match C# formatting conventions (triple slashes)
   - Be ready for `docfx`, `Visual Studio IntelliSense`, or `DocFX` ingestion

5. You must document every:
   - Public class/interface/enum/struct
   - Public or private method, property, field, constructor
   - Delegate/event if present



Do not skip over undocumented items. If information is missing, use placeholder summaries but flag as `TODO:`.

You are not summarizing — you are writing structured documentation for consumption by tools, developers, and future maintainers.

✅ Correct XML Documentation Highlights
Member	Description
<summary>	Describes purpose and usage, not repetition of type name
<typeparam>	Used for TKey and TValue, even if the types are obvious
<param>	Describes usage, not just "The X"
<returns>	Explains what the return value means
&lt;, &gt;	Used in <see> and <typeparamref> for generic mentions
✅ Example Inline Use
/// <returns>
/// A <see cref="Dictionary&lt;TKey, TValue&gt;"/> that contains all cache entries.
/// </returns>

❌ What to Avoid
Mistake	Example
Vague summary	/// <summary>Gets or sets something.</summary>
Skipping generics	Omitting <typeparam>
Raw < and >	List<string> instead of List&lt;string&gt;
Repetition	/// <summary>Represents a GenericCache class.</summary>
Empty docs	/// <summary></summary> or missing tags

| Character | Escape in XML Doc |
| --------- | ----------------- |
| `<`       | `&lt;`            |
| `>`       | `&gt;`            |
| `&`       | `&amp;`           |
| `"`       | `&quot;`          |
| `'`       | `&apos;`          |
Sample for scaping
List<Dictionary<string, List<int>>>

<see cref="List&lt;Dictionary&lt;string, List&lt;int&gt;&gt;&gt;"/>


## ✅ Sample Class: `GenericCache<TKey, TValue>`

```csharp
/// <summary>
/// Represents a simple in-memory cache with generic key-value support.
/// </summary>
/// <typeparam name="TKey">The type used for keys.</typeparam>
/// <typeparam name="TValue">The type used for values.</typeparam>
public class GenericCache<TKey, TValue>
{
    /// <summary>
    /// Stores the items internally.
    /// </summary>
    private readonly Dictionary<TKey, TValue> _store = new();

    /// <summary>
    /// Adds or replaces the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to store.</param>
    /// <param name="value">The value to associate with the key.</param>
    public void AddOrUpdate(TKey key, TValue value)
    {
        _store[key] = value;
    }

    /// <summary>
    /// Tries to get the value for the specified key.
    /// </summary>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <param name="value">When this method returns, contains the value if found; otherwise, the default value.</param>
    /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
    public bool TryGet(TKey key, out TValue value)
    {
        return _store.TryGetValue(key, out value);
    }

    /// <summary>
    /// Clears all items from the cache.
    /// </summary>
    public void Clear()
    {
        _store.Clear();
    }
}
