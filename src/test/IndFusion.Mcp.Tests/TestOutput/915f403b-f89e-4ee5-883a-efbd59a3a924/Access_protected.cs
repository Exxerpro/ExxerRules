using System.Linq;

public class Sample
{
    public double GetAverage(int[] values)
    {
        return values.Sum() / (double)values.Length;
    }
}