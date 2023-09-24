public class ClassC : ClassB
{
    public ClassC() { }

    public ClassC(int ValueA, string ValueB, bool ValueC) : base(ValueA, ValueB)
    {
        this.ValueC = ValueC;
    }

    public bool ValueC { get; set; }
}