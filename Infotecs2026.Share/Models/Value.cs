using CmdScale.EntityFrameworkCore.TimescaleDB.Configuration.Hypertable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infotecs2026.Share.Models;

public class Value
{
    [Column("id")]
    public int Id { get; set; }

    [Column("file_name")]
    [Required]
    public string FileName { get; set; } = string.Empty;

    [Column("date")]
    public DateTime Date { get; set; }

    [Column("execution_time")]
    public int ExecutionTime { get; set; }

    [Column("value")]
    public double NumericValue { get; set; }
}
