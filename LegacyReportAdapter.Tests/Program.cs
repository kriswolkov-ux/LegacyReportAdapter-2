using LegacyReportAdapter;
using Adapter = LegacyReportAdapter.LegacyReportAdapter;

var failed = new List<string>();
var checks = 0;

void Check(string name, bool condition)
{
    checks++;
    Console.WriteLine($"{(condition ? "PASS" : "FAIL")} — {name}");
    if (!condition) failed.Add(name);
}

static string Capture(Action action)
{
    var writer = new StringWriter();
    var original = Console.Out;
    try
    {
        Console.SetOut(writer);
        action();
    }
    finally
    {
        Console.SetOut(original);
    }
    return writer.ToString();
}

static Exception? CaptureException(Action action)
{
    try
    {
        action();
        return null;
    }
    catch (Exception exception)
    {
        return exception;
    }
}

var exporter = new Adapter(new LegacyReportPrinter());
var text = Capture(() => exporter.Export(new ReportDocument("Test title", "Test body")));

Check("заголовок передан legacy-принтеру", text.Contains("=== Test title ==="));
Check("тело передано legacy-принтеру", text.Contains("Test body"));
Check("сохранён завершающий маркер", text.Contains("=== END OF REPORT ==="));

var legacyText = Capture(() => new LegacyReportPrinter().PrintReport("Test title", "Test body"));
Check("адаптер не меняет формат legacy-принтера", text == legacyText);

var composedText = Capture(() => CompositionRoot.CreateExporter().Export(new ReportDocument("Test title", "Test body")));
Check("CompositionRoot собирает рабочий IReportExporter", composedText == text);

var emptyTitleText = Capture(() => exporter.Export(new ReportDocument(string.Empty, "Test body")));
var expectedEmptyTitleText = string.Join(
    Environment.NewLine,
    "===  ===",
    "Test body",
    "=== END OF REPORT ===",
    string.Empty);
Check("пустой заголовок передаётся без подстановок", emptyTitleText == expectedEmptyTitleText);

var emptyBodyText = Capture(() => exporter.Export(new ReportDocument("Test title", string.Empty)));
var expectedEmptyBodyText = string.Join(
    Environment.NewLine,
    "=== Test title ===",
    string.Empty,
    "=== END OF REPORT ===",
    string.Empty);
Check("пустое тело передаётся без подстановок", emptyBodyText == expectedEmptyBodyText);

var nullDocumentException = CaptureException(() => exporter.Export(null!));
Check("null-документ отклоняется с ArgumentNullException", nullDocumentException is ArgumentNullException);

var nullTitleException = CaptureException(() => exporter.Export(new ReportDocument(null!, "Test body")));
Check("null-заголовок отклоняется с ArgumentException", nullTitleException is ArgumentException);

var nullBodyException = CaptureException(() => exporter.Export(new ReportDocument("Test title", null!)));
Check("null-тело отклоняется с ArgumentException", nullBodyException is ArgumentException);

var nullPrinterException = CaptureException(() =>
{
    _ = new Adapter(null!);
});
Check("null-зависимость отклоняется с ArgumentNullException", nullPrinterException is ArgumentNullException);

Console.WriteLine($"Провалено проверок: {failed.Count} из {checks}.");
return failed.Count == 0 ? 0 : 1;
