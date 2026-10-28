using LegacyReportAdapter;

IReportExporter exporter = CompositionRoot.CreateExporter();
var document = new ReportDocument("Состояние оборудования",
    "Всего позиций: 18\nВ ремонте: 3\nСписано: 1");
exporter.Export(document);
