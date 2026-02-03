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
    private readonly IResultRepository _resultRepository;

    private const int CsvFileRowCount = 10000;

    public ValueService(ILogger<ValueService> logger, IValueRepository valueRepository, IResultRepository resultRepository)
    {
        _logger = logger;
        _valueRepository = valueRepository;
        _resultRepository = resultRepository;
    }

    public async Task<StatusCode> ParseCsvFileAsync(string fileName, Stream stream)
    {
        if (!Path.GetExtension(fileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError(MsgPattern.Error, StatusCode.InvalidFileFormat,
                StatusCodeExtensions.GetMessage(StatusCode.InvalidFileFormat, fileName));
            return StatusCode.InvalidFileFormat;
        }

        List<Value> curFile = await _valueRepository.GetByNameAsync(fileName);

        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = ["yyyy-MM-dd HH:mm:ss"];

        await csv.ReadAsync();
        csv.ReadHeader();

        CalcStatistic calcStatistic = new(new());
        while (await csv.ReadAsync())
        {
            // количество строк не может быть меньше 1 и больше 10 000
            if (calcStatistic.RowCounter > CsvFileRowCount)
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

            Value value = await SetToTable(fileName, date!.Value, executionTime!.Value, numericValue!.Value, 
                calcStatistic.RowCounter, curFile,  curFile.Count == 0 || calcStatistic.RowCounter >= curFile.Count);

            calcStatistic.Calc(value);
        }

        if (calcStatistic.RowCounter < 1)
        {
            _logger.LogError(MsgPattern.Error, StatusCode.FileEmpty,
                StatusCodeExtensions.GetMessage(StatusCode.FileEmpty, fileName));
            return StatusCode.FileEmpty;
        }

        // Если размер нового файла меньше старого, удаляем лишние строки
        if (calcStatistic.RowCounter < curFile.Count)
            _valueRepository.RemoveRange(
                curFile.GetRange(calcStatistic.RowCounter,
                curFile.Count - calcStatistic.RowCounter));

        SetStatistic(fileName, calcStatistic, curFile.Count == 0);

        await _valueRepository.SaveAsync();

        _logger.LogInformation(MsgPattern.Info, StatusCode.Ok, StatusCodeExtensions.GetMessage(StatusCode.Ok));

        return StatusCode.Ok;
    }

    private async void SetStatistic(string fileName, CalcStatistic calcStatistic, bool needUpdateResult)
    {
        Result result = new()
        {
            FileName = fileName,
            TotalDurationSeconds = calcStatistic.TotalDurationSeconds,
            StartDateTime = calcStatistic.MinDate,
            MinValue = calcStatistic.MinValue,
            MaxValue = calcStatistic.MaxValue,
            AverageExecutionTime = calcStatistic.AverageExecutionTime,
            AverageValue = calcStatistic.AverageValue,
            MedianValue = calcStatistic.MedianValue
        };

        if (needUpdateResult)
            await _resultRepository.AddAndSaveAsync(result);
        else
        {
            _resultRepository.Update(result);
            await _resultRepository.SaveAsync();
        }
    }

    private async Task<Value> SetToTable(string fileName, DateTime date, int executionTime,
        double numericValue, int rowCount, List<Value> curFileRows, bool needUpdatevalue)
    {
        Value value = new();

        if (needUpdatevalue)
        {
            value = new()
            {
                FileName = fileName,
                Date = date,
                ExecutionTime = executionTime,
                NumericValue = numericValue
            };

            await _valueRepository.AddAsync(value);
        }
        else
        {
            value = curFileRows[rowCount];

            value.Date = date;
            value.ExecutionTime = executionTime;
            value.NumericValue = numericValue;

            _valueRepository.Update(value);
        }

        return value;
    }

    private bool IsValidRow(CsvReader csv, out DateTime? dateTime, out int? execution, out double? numeric)
    {
        // Валидируем данные в каждой строке
        DateTime? date = IsValid(csv, "Date", out dateTime,
            (DateTime dateTime) => dateTime >= new DateTime(2000, 1, 1) && dateTime <= DateTime.Now,
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
}
