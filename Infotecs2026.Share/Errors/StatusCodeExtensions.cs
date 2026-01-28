namespace Infotecs2026.Share.Errors;

public static class StatusCodeExtensions
{
    private static readonly Dictionary<StatusCode, string> ErrorMessages = new()
    {
        // Общие
        [StatusCode.Ok] = "Успешно",
        [StatusCode.UnknownError] = "Неизвестная ошибка",
        [StatusCode.ValidationFailed] = "Ошибка валидации",

        // Файлы
        [StatusCode.FileNotFound] = "Файл не найден",
        [StatusCode.FileEmpty] = "Файл '{0}' пуст",
        [StatusCode.FileTooLarge] = "Файл '{0}' содержит более 10000 строк",
        [StatusCode.InvalidFileFormat] = "Неподдерживаемый формат файла. Ожидается: '{0}'",

        // CSV
        [StatusCode.CsvHeaderInvalid] = "Неверный формат заголовков CSV",
        [StatusCode.CsvParsingError] = "Ошибка парсинга CSV",
        [StatusCode.InvalidDateFormat] = "Неверный формат даты в колонке '{0}'",
        [StatusCode.InvalidDateRange] = "Дата не попадает в требуемый промежуток",
        [StatusCode.InvalidNumberFormat] = "Неверный числовой формат в колонке '{0}'"
    };

    public static string GetMessage(this StatusCode errorCode, params object[] args) => 
        ErrorMessages.TryGetValue(errorCode, out var message) ?
        args.Length > 0 ? string.Format(message, args) : message :
        ErrorMessages[StatusCode.UnknownError];
}