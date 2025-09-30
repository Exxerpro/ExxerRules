public class LegacyLogger { public void Write(string message) { System.Console.WriteLine(message); } }

public class LoggerAdapter
{
    private readonly LegacyLogger _inner;

    public LoggerAdapter(LegacyLogger inner)
    {
        _inner = inner;
    }

    public void Adapt(string message)
    {
        _inner.Write(message);
    }
}
