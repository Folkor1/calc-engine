using System.Collections.Generic;

namespace Co2Calc.Engine;

public sealed class BasicFactorProvider : IFactorProvider
{
    private readonly Dictionary<(string category, string region, string unit), (decimal value, string id)> _map;

    public BasicFactorProvider(Dictionary<(string,string,string),(decimal,string)>? seed = null)
    {
        _map = seed ?? new Dictionary<(string, string, string), (decimal, string)>(
            new Dictionary<(string, string, string), (decimal, string)>.Comparer
        );
        if (seed is null)
        {
            // Демо-факторы (gCO2e/kWh). Позже будут реальные DEFRA/EPA/IPCC.
            _map[("grid.electricity","IE","g_per_kWh")] = (275m, "def:placeholder:IE:2025");
            _map[("grid.electricity","EU","g_per_kWh")] = (300m, "def:placeholder:EU:2025");
            _map[("grid.electricity","US","g_per_kWh")] = (400m, "def:placeholder:US:2025");
            _map[("grid.electricity","DEFAULT","g_per_kWh")] = (350m, "def:placeholder:DEFAULT:2025");
        }
    }

    public (decimal value, string id) Get(string category, string region, string unit)
    {
        if (_map.TryGetValue((category, region, unit), out var v)) return v;
        if (_map.TryGetValue((category, "DEFAULT", unit), out v)) return v;
        throw new KeyNotFoundException($"No factor for {category}/{region}/{unit}");
    }
}
