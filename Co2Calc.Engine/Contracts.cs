namespace Co2Calc.Engine;

public record CalcInput(string Region, long BytesTransferred, double CacheHitRate);
public record CalcResult(decimal Co2eGrams, decimal MinGrams, decimal MaxGrams, IReadOnlyList<string> FactorIds);
public interface IFactorProvider { (decimal value, string id) Get(string category, string region, string unit); }
public interface IMethodology { string Id { get; } Version Version { get; } CalcResult Calculate(CalcInput input, IFactorProvider factors); }
