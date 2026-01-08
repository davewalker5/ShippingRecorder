using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRecorder.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class OperatorStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperatorStatistics",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Sightings = table.Column<int>(type: "INTEGER", nullable: true),
                    Locations = table.Column<int>(type: "INTEGER", nullable: true),
                    Vessels = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperatorStatistics");
        }
    }
}
