using Adapter = LegacyReportAdapter.LegacyReportAdapter;

namespace LegacyReportAdapter;

/// <summary>
/// Точка сборки приложения: только здесь создаётся конкретный legacy-принтер,
/// чтобы вызывающий код зависел от <see cref="IReportExporter"/>, а не от legacy-класса.
/// </summary>
public static class CompositionRoot
{
    public static IReportExporter CreateExporter()
        => new Adapter(new LegacyReportPrinter());
}
