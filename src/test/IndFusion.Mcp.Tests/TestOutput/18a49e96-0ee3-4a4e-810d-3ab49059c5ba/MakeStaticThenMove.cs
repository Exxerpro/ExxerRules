public class SourceClass
{
    public string Value = "x";
    public static string GetValueWithSuffix(SourceClass source, string suffix)
    {
        return source.Value + suffix;
    }
}

public class NewMathUtils { }