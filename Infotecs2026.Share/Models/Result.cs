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
    public double TotalDurationSeconds { get; set; }

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

    [Column("processed_at")]
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    [Column("rows_count")]
    public int RowsCount { get; set; }

    [Column("file_hash")]
    public string? FileHash { get; set; }
}