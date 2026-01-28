using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infotecs2026.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "results",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    filename = table.Column<string>(type: "text", nullable: false),
                    total_duration_seconds = table.Column<double>(type: "double precision", nullable: false),
                    start_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    average_execution_time = table.Column<double>(type: "double precision", nullable: false),
                    average_value = table.Column<double>(type: "double precision", nullable: false),
                    median_value = table.Column<double>(type: "double precision", nullable: false),
                    max_value = table.Column<double>(type: "double precision", nullable: false),
                    min_value = table.Column<double>(type: "double precision", nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    rows_count = table.Column<int>(type: "integer", nullable: false),
                    file_hash = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_results", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "value",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    execution_time = table.Column<long>(type: "bigint", nullable: false),
                    value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_value", x => new { x.id, x.date });
                });

            migrationBuilder.CreateIndex(
                name: "IX_results_filename",
                table: "results",
                column: "filename",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_value_date",
                table: "value",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_value_file_name",
                table: "value",
                column: "file_name");

            migrationBuilder.Sql(@"
                SELECT create_hypertable('public.""value""', 'date', chunk_time_interval => INTERVAL '7 days');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "results");

            migrationBuilder.DropTable(
                name: "value");
        }
    }
}
