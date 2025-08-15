using Xunit;

namespace Co2Calc.Engine.Tests;

public class GhgProtocolV1Tests
{
    private static readonly Co2Calc.Engine.GhgProtocolV1 M = new();
    private static readonly Co2Calc.Engine.BasicFactorProvider F = new();

    [Fact]
    public void CalculatesPositiveValue()
    {
        var input = new Co2Calc.Engine.CalcInput("IE", 100_000_000, 0.2); // ~100MB
        var res = M.Calculate(input, F);

        Assert.True(res.Co2eGrams > 0, "CO2 should be positive");
        Assert.Contains("def:placeholder:IE", res.FactorIds[0]);
        Assert.True(res.MinGrams < res.Co2eGrams && res.Co2eGrams < res.MaxGrams);
    }

    [Fact]
    public void DeterministicForSameInput()
    {
        var input = new Co2Calc.Engine.CalcInput("EU", 500_000_000, 0.5);
        var r1 = M.Calculate(input, F);
        var r2 = M.Calculate(input, F);
        Assert.Equal(r1.Co2eGrams, r2.Co2eGrams);
        Assert.Equal(r1.MinGrams,  r2.MinGrams);
        Assert.Equal(r1.MaxGrams,  r2.MaxGrams);
    }

    [Fact]
    public void ValidatesInputs()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            M.Calculate(new Co2Calc.Engine.CalcInput("US", -1, 0), F));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            M.Calculate(new Co2Calc.Engine.CalcInput("US", 1,  1.5), F));
    }
}
