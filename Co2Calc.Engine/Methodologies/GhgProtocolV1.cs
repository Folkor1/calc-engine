namespace Co2Calc.Engine;

public sealed class GhgProtocolV1 : IMethodology
{
    public string Id => "ghg-protocol";
    public Version Version => new(1, 0, 0);

    // Упрощённая заглушка: энергозатраты на передачу 1 ГБ
    // ВНИМАНИЕ: это демо-константа; позже заменим на реальную модель/параметры.
    private const decimal EnergyPerGbKWh = 0.05m; // 0.05 кВт⋅ч/ГБ (пример)

    public CalcResult Calculate(CalcInput input, IFactorProvider factors)
    {
        if (input.BytesTransferred < 0)
            throw new ArgumentOutOfRangeException(nameof(input.BytesTransferred), "bytes must be >= 0");
        if (input.CacheHitRate is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(input.CacheHitRate), "cacheHitRate must be [0..1]");

        // 1) Переводим байты в ГБ
        var gb = (decimal)input.BytesTransferred / (1024m * 1024m * 1024m);

        // 2) Оцениваем кВт⋅ч на просмотр (учитываем кэш — чем выше hit, тем меньше энергия)
        var kWh = gb * EnergyPerGbKWh * (1m - 0.5m * (decimal)input.CacheHitRate);

        // 3) Берём сетевой фактор региона (gCO2e/kWh)
        var (gridFactor, factorId) = factors.Get("grid.electricity", input.Region, "g_per_kWh");

        // 4) Умножаем: кВт⋅ч × g/кВт⋅ч = gCO2e
        var mean = kWh * gridFactor;

        // 5) Диапазон (пока ±10% как заглушка)
        var min = mean * 0.9m;
        var max = mean * 1.1m;

        return new CalcResult(
            Co2eGrams: decimal.Round(mean, 6),
            MinGrams:  decimal.Round(min, 6),
            MaxGrams:  decimal.Round(max, 6),
            FactorIds: new [] { factorId }
        );
    }
}
