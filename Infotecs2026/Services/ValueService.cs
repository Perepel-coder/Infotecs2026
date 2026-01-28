using CsvHelper;
using Infotecs2026.Share.Application.Interfaces;
using Infotecs2026.Share.Errors;
using Infotecs2026.Share.Models;
using System.Globalization;
using System.Text;

namespace Infotecs2026.Services;

public class ValueService
{
    private readonly ILogger<ValueService> _logger;
    private readonly IValueRepository _valueRepository;

    public ValueService(ILogger<ValueService> logger, IValueRepository valueRepository)
    {
        _logger = logger;
        _valueRepository = valueRepository;
    }

    public async Task<StatusCode> ParseCsvFileAsync(string fileName, Stream stream)
    {
        if (!Path.GetExtension(fileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError(MsgPattern.Error, StatusCode.InvalidFileFormat,
                StatusCodeExtensions.GetMessage(StatusCode.InvalidFileFormat, fileName));
            return StatusCode.InvalidFileFormat;
        }

        List<Value> file = await _valueRepository.GetByNameAsync(fileName);

        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Read();
        csv.ReadHeader();

        int rowCounter = 0;
        Result result = new Result();
        while (csv.Read())
        {
            rowCounter++;

            // количество строк не может быть меньше 1 и больше 10 000
            if (rowCounter > 10000)
            {
                _logger.LogError(MsgPattern.Error, StatusCode.FileTooLarge,
                    StatusCodeExtensions.GetMessage(StatusCode.FileTooLarge, fileName));
                return StatusCode.FileTooLarge;
            }

            // Валидируем данные в каждой строке
            if (!IsValidRow(csv, out DateTime? date, out int? executionTime, out double? numericValue))
            {
                _logger.LogError(MsgPattern.Error, StatusCode.CsvParsingError,
                    StatusCodeExtensions.GetMessage(StatusCode.CsvParsingError));
                return StatusCode.InvalidFileFormat;
            }

            // Сохраняем или обновляем запись в базе данных
            if (file.Count == 0 || file.Count > rowCounter)
            {
                await _valueRepository.AddAsync(new()
                {
                    FileName = fileName,
                    Date = date.Value,
                    ExecutionTime = executionTime.Value,
                    NumericValue = numericValue.Value
                });
            }
            else
            {
                if (file.Count <= rowCounter)
                {
                    Value value = file[rowCounter - 1];

                    value.Date = date.Value;
                    value.ExecutionTime = executionTime.Value;
                    value.NumericValue = numericValue.Value;

                    _valueRepository.Update(value);
                }
                else
                    _valueRepository.RemoveRange(file.GetRange(rowCounter, file.Count - rowCounter));
            }
        }

        if (rowCounter < 1)
        {
            _logger.LogError(MsgPattern.Error, StatusCode.FileEmpty,
                StatusCodeExtensions.GetMessage(StatusCode.FileEmpty, fileName));
            return StatusCode.FileEmpty;
        }

        await _valueRepository.SaveAsync();

        _logger.LogInformation(MsgPattern.Info, StatusCode.Ok, StatusCodeExtensions.GetMessage(StatusCode.Ok));

        return StatusCode.Ok;
    }

    private bool IsValidRow(CsvReader csv, out DateTime? dateTime, out int? execution, out double? numeric)
    {
        // Валидируем данные в каждой строке
        DateTime? date = IsValid(csv, "Date", out dateTime, (DateTime dateTime) => dateTime >= new DateTime(2000, 1, 1) && dateTime <= DateTime.Now,
            StatusCode.InvalidDateFormat, StatusCode.InvalidDateRange);

        int? executionTime = IsValid(csv, "ExecutionTime", out execution, (int execution) => execution >= 0,
            StatusCode.InvalidNumberFormat, StatusCode.InvalidNumberValue);

        double? numericValue = IsValid(csv, "NumericValue", out numeric, (double numeric) => numeric >= 0,
            StatusCode.InvalidNumberFormat, StatusCode.InvalidNumberValue);

        // Если хоть одно из полей невалидно, возвращаем ошибку
        if (date is null || executionTime is null || numericValue is null)
        {
            _logger.LogError(MsgPattern.Error, StatusCode.CsvParsingError,
                StatusCodeExtensions.GetMessage(StatusCode.CsvParsingError));
            return false;
        }

        return true;
    }

    private T? IsValid<T>(CsvReader csv, string columnName, out T? dateValue, Func<T, bool> isValidValue,
            StatusCode formatStatusCode, StatusCode valueStatusCode) where T : struct
    {
        if (!csv.TryGetField(columnName, out dateValue) || dateValue == null)
        {
            _logger.LogError(MsgPattern.Error, formatStatusCode, StatusCodeExtensions.GetMessage(formatStatusCode, columnName));
            return null;
        }

        if (!isValidValue.Invoke(dateValue.Value))
        {
            _logger.LogError(MsgPattern.Error, valueStatusCode, StatusCodeExtensions.GetMessage(valueStatusCode));
            return null;
        }

        return dateValue;
    }

    private Result GetResult()
    {

    }
}
