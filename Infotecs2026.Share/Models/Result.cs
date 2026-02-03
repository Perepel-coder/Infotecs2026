using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infotecs2026.Share.Models;

public class Result
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("filename")]
    [Required]
    public string FileName { get; set; } = string.Empty;

    // Дельта времени Date в секундах
    [Column("total_duration_seconds")]
    public int TotalDurationSeconds { get; set; }

    // Минимальное дата и время
    [Column("start_date_time")]
    public DateTime StartDateTime { get; set; }

    // Среднее время выполнения
    [Column("average_execution_time")]
    public double AverageExecutionTime { get; set; }

    // Среднее значение
    [Column("average_value")]
    public double AverageValue { get; set; }

    // Медиана
    [Column("median_value")]
    public double MedianValue { get; set; }

    // Максимальное значение
    [Column("max_value")]
    public double MaxValue { get; set; }

    // Минимальное значение
    [Column("min_value")]
    public double MinValue { get; set; }
}