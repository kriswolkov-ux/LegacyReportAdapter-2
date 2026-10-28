# Практика № 1 — Подключение старого принтера отчётов через адаптер

**Время:** 32 минуты. **Уровень:** продолжающие.

## Сценарий
Старое приложение использует `LegacyReportPrinter`. Его нельзя изменять: класс поставляется как внешняя библиотека. Новый код должен работать с `IReportExporter`.

## Задания
1. Проследите вызовы от `Program` к `IReportExporter` и `LegacyReportPrinter`.
2. Реализуйте `LegacyReportAdapter`, который реализует `IReportExporter` и делегирует работу старому классу. Старый класс не изменять.
3. Преобразуйте `ReportDocument` в параметры старого API: заголовок и тело.
4. Определите поведение при `null`, пустом заголовке и пустом теле. Не используйте пустой `catch`.
5. Запустите тесты и добавьте минимум один собственный граничный тест.
6. Сохраните прежний формат вывода legacy-принтера.

## Ограничения
- Не переименовывайте методы старого класса.
- Не добавляйте зависимость от конкретного legacy-класса в `Program`.
- Адаптер — единственное место преобразования API.
- Обоснуйте выбранный контракт.

## Трассировка вызовов
```text
Program
  → CompositionRoot.CreateExporter()
      → new LegacyReportPrinter()
      → new LegacyReportAdapter(printer) : IReportExporter
  → IReportExporter.Export(document)
      → LegacyReportAdapter.Export(document)
          → проверка document / Title / Body
          → LegacyReportPrinter.PrintReport(document.Title, document.Body)
              → Console.WriteLine("=== {title} ===")
              → Console.WriteLine(body)
              → Console.WriteLine("=== END OF REPORT ===")
```
`Program` не создаёт `LegacyReportPrinter` и не вызывает `PrintReport`: конкретный legacy-класс собирается только в `CompositionRoot`.

## Контракт адаптера
- `new LegacyReportAdapter(null)` → `ArgumentNullException`: без принтера экспорт невозможен.
- `Export(null)` → `ArgumentNullException`: отсутствие документа — ошибка вызывающего кода, а не «пустой отчёт».
- `Title == null` или `Body == null` → `ArgumentException`: значения нарушают non-nullable контракт `ReportDocument`; адаптер не передаёт заведомо некорректные данные в legacy-код.
- Пустые `Title` и `Body` (`string.Empty`) допустимы и передаются в legacy-принтер как есть. Адаптер преобразует только API и не придумывает содержимое, поэтому формат `=== ... ===`, тело и `=== END OF REPORT ===` остаются форматом самого принтера.
- Исключения legacy-принтера не перехватываются: адаптер не скрывает ошибки старого API.

## Критерии готовности
- [x] Реализован `IReportExporter`.
- [x] `LegacyReportPrinter` не изменён.
- [x] Основной сценарий работает.
- [x] Тесты проходят.
- [x] Проверены два граничных сценария.

## Запуск
```powershell
dotnet run --project LegacyReportAdapter.Tests/LegacyReportAdapter.Tests.csproj
dotnet run --project LegacyReportAdapter/LegacyReportAdapter.csproj
```
