using System.Linq;

public class Sample
{
    protected int _protectedField = values.Sum();

    public double GetAverage(int[] values)
    {
        return _protectedField / (double)values.Length;
    }
}