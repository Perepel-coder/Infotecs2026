using Infotecs2026.Share.Models;
using MoreLinq;

namespace Infotecs2026.Services;

public class CalcStatistic
{
    private MedianFinder<double> _MedianFinder;
    public CalcStatistic(MedianFinder<double> MedianFinder) => _MedianFinder = MedianFinder;

    public void Calc(Value value)
    {
        _MedianFinder.Add(value.NumericValue);

        MinDate = value.Date < MinDate || RowCounter == 0 ? value.Date : MinDate;
        MaxDate = value.Date > MaxDate || RowCounter == 0 ? value.Date : MaxDate;
        MinValue = value.NumericValue < MinValue || RowCounter == 0 ? value.NumericValue : MinValue;
        MaxValue = value.NumericValue > MaxValue || RowCounter == 0 ? value.NumericValue : MaxValue;

        TotalDurationSeconds = (MaxDate - MinDate).Seconds;

        MergeDigitSums(value.ExecutionTime, ref _digitsExecutionTime);
        MergeDigitSums(value.NumericValue, ref _integersNumericValue, ref _fractionsNumericValue);

        RowCounter++;
    }

    private List<int> _digitsExecutionTime = new();

    private List<int> _integersNumericValue = new();
    private List<int> _fractionsNumericValue = new();

    private void MergeDigitSums(double number, ref List<int> integers, ref List<int> fractions)
    {
        int integer = (int)Math.Truncate(number);
        MergeDigitSums(integer, ref integers);

        double fraction = number - integer;

        List<int> digits = new();
        while (fraction > 0.0001)
        {
            int num = (int)Math.Truncate(fraction * 10);
            digits.Add(num);
            fraction -= num;
        }

        fractions = fractions.ZipLongest(digits, (x, y) => x + y).ToList();
    }

    private void MergeDigitSums(int number, ref List<int> source)
    {
        List<int> digits = new();
        while (number > 0)
        {
            digits.Add(number % 10);
            number /= 10;
        }

        source = source.ZipLongest(digits, (x, y) => x + y).ToList();
    }

    private double CalcAvrg(List<int> integers)
    {
        double avrg = 0;
        for (int i = 0; i < integers.Count; i++)
            avrg += integers[i] * (int)Math.Pow(10, i) / RowCounter;

        return avrg;
    }

    private double CalcAvrg(List<int> integers, List<int> fractions)
    {
        double avrg = CalcAvrg(integers);
        for (int i = 0; i < fractions.Count; i++)
            avrg += fractions[i] * (int)Math.Pow(10, -(i + 1)) / RowCounter;
        return avrg;
    }


    public int RowCounter { get; private set; } = 0;

    // минимальное Date
    public DateTime MinDate { get; private set; }

    // максимальное Date
    public DateTime MaxDate { get; private set; }

    // дельта времени Date в секундах 
    public int TotalDurationSeconds { get; private set; }

    // среднее время выполнения (ExecutionTime)
    public double AverageExecutionTime => CalcAvrg(_digitsExecutionTime);

    // среднее значение по показателям (Value)
    public double AverageValue => CalcAvrg(_integersNumericValue, _fractionsNumericValue);

    // медина по показателям (Value)
    public double MedianValue => _MedianFinder.FindMedian();

    // максимальное значение показателя (Value)
    public double MaxValue { get; private set; }

    // минимальное значение показателя (Value)
    public double MinValue { get; private set; }
}
