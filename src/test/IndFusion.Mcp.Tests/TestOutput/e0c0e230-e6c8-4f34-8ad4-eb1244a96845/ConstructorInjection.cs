class C
{
    private readonly int _x;

    int M() { return _x + 1; }
    void Call() { M(); }

    public C(int x)
    {
        _x = x;
    }
}
