using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRecorder.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class VoyageVessel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.AddColumn<long>(
                name: "vessel_id",
                table: "VOYAGE",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_VOYAGE_vessel_id",
                table: "VOYAGE",
                column: "vessel_id");

            migrationBuilder.AddForeignKey(
                name: "FK_VOYAGE_VESSEL_vessel_id",
                table: "VOYAGE",
                column: "vessel_id",
                principalTable: "VESSEL",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VOYAGE_VESSEL_vessel_id",
                table: "VOYAGE");

            migrationBuilder.DropIndex(
                name: "IX_VOYAGE_vessel_id",
                table: "VOYAGE");

            migrationBuilder.DropColumn(
                name: "vessel_id",
                table: "VOYAGE");
        }
    }
}
