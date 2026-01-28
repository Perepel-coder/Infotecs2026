namespace Infotecs2026.Share.Errors;

public enum StatusCode
{
    // Общие ошибки (0-99)
    Ok = 0,
    UnknownError = 1,
    ValidationFailed = 2,

    // Ошибки файлов (100-199)
    FileNotFound = 100,
    FileEmpty = 101,
    FileTooLarge = 102,
    InvalidFileFormat = 103,
    FileCorrupted = 104,

    // Ошибки CSV (200-299)
    CsvHeaderInvalid = 200,
    CsvParsingError = 201,
    CsvFormatError = 202,
    InvalidDateFormat = 203,
    InvalidDateRange = 204,
    InvalidNumberFormat = 205,
    InvalidNumberValue = 206,
}