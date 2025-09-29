public class cA { public int Value => 1; public int Get() { return B.Get(this); } public int Add(int x) { return x + Value; } }
public class B { }
