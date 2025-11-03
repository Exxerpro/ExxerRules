using System.Collections.Generic;
public class Outer
{
    public class Inner { }
    public List<Inner> MakeList()
    {
        return Target.MakeList();
    }

    public int CountList(List<Inner> items)
    {
        return Target.CountList(items);
    }
}
public class Target { }