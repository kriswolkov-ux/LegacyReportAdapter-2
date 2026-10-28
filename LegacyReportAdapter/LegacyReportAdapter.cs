namespace LegacyReportAdapter;

/// <summary>
/// Адаптер между современным <see cref="IReportExporter"/> и legacy-принтером.
/// </summary>
public sealed class LegacyReportAdapter : IReportExporter
{
    private readonly LegacyReportPrinter _printer;

    /// <exception cref="ArgumentNullException">
    /// <paramref name="printer"/> равен <see langword="null"/>.
    /// </exception>
    public LegacyReportAdapter(LegacyReportPrinter printer)
    {
        _printer = printer ?? throw new ArgumentNullException(nameof(printer));
    }

    /// <summary>
    /// Передаёт заголовок и тело документа в <see cref="LegacyReportPrinter.PrintReport"/>.
    /// Пустые строки передаются без подстановок: адаптер не меняет содержимое отчёта.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="document"/> равен <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <see cref="ReportDocument.Title"/> или <see cref="ReportDocument.Body"/> равен <see langword="null"/>.
    /// </exception>
    public void Export(ReportDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (document.Title is null)
        {
            throw new ArgumentException(
                "Заголовок отчёта не может быть null.",
                nameof(document));
        }

        if (document.Body is null)
        {
            throw new ArgumentException(
                "Тело отчёта не может быть null.",
                nameof(document));
        }

        _printer.PrintReport(document.Title, document.Body);
    }
}
