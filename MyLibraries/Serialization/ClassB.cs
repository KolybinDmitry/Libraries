public class ClassB : ClassA
{
    public ClassB() { }

    public ClassB(int ValueA, string ValueB) : base(ValueA)
    {
        this.ValueB = ValueB;
    }

    public string ValueB { get; set; }
}
