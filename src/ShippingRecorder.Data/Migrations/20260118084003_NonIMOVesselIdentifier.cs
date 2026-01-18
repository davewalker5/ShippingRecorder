using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRecorder.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class NonIMOVesselIdentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;");

            migrationBuilder.CreateTable(
                name: "COUNTRY",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    code = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COUNTRY", x => x.id);
                    table.CheckConstraint("CK_Code_Characters", "code GLOB '[A-Z0-9]*'");
                    table.CheckConstraint("CK_Code_Length", "length(code) = 2");
                });

            migrationBuilder.CreateTable(
                name: "FlagStatistics",
                columns: table => new
                {
                    Code = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Sightings = table.Column<int>(type: "INTEGER", nullable: true),
                    Locations = table.Column<int>(type: "INTEGER", nullable: true),
                    Vessels = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "JOB_STATUS",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    parameters = table.Column<string>(type: "TEXT", nullable: true),
                    start = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    end = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    error = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOB_STATUS", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "LOCATION",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOCATION", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "LocationStatistics",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Sightings = table.Column<int>(type: "INTEGER", nullable: true),
                    Vessels = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "MyVoyages",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    Identifier = table.Column<string>(type: "TEXT", nullable: true),
                    Vessel = table.Column<string>(type: "TEXT", nullable: true),
                    VesselId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "OPERATOR",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OPERATOR", x => x.id);
                });

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

            migrationBuilder.CreateTable(
                name: "SightingsByMonth",
                columns: table => new
                {
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Month = table.Column<int>(type: "INTEGER", nullable: false),
                    Sightings = table.Column<int>(type: "INTEGER", nullable: true),
                    Locations = table.Column<int>(type: "INTEGER", nullable: true),
                    Vessels = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "USER",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "VESSEL",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    identifier = table.Column<string>(type: "TEXT", nullable: false),
                    is_imo = table.Column<bool>(type: "INTEGER", nullable: false),
                    built = table.Column<int>(type: "INTEGER", nullable: true),
                    draught = table.Column<decimal>(type: "TEXT", nullable: true),
                    length = table.Column<int>(type: "INTEGER", nullable: true),
                    beam = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VESSEL", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "VESSEL_TYPE",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VESSEL_TYPE", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "VesselTypeStatistics",
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

            migrationBuilder.CreateTable(
                name: "PORT",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    country_id = table.Column<long>(type: "INTEGER", nullable: false),
                    code = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PORT", x => x.id);
                    table.CheckConstraint("CK_Code_AlphaNumeric", "code GLOB '[A-Za-z0-9]*'");
                    table.CheckConstraint("CK_Code_Length", "length(code) = 5");
                    table.ForeignKey(
                        name: "FK_PORT_COUNTRY_country_id",
                        column: x => x.country_id,
                        principalTable: "COUNTRY",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VOYAGE",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    operator_id = table.Column<long>(type: "INTEGER", nullable: false),
                    vessel_id = table.Column<long>(type: "INTEGER", nullable: false),
                    number = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VOYAGE", x => x.id);
                    table.ForeignKey(
                        name: "FK_VOYAGE_OPERATOR_operator_id",
                        column: x => x.operator_id,
                        principalTable: "OPERATOR",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VOYAGE_VESSEL_vessel_id",
                        column: x => x.vessel_id,
                        principalTable: "VESSEL",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REGISTRATION_HISTORY",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    vessel_id = table.Column<long>(type: "INTEGER", nullable: false),
                    vessel_type_id = table.Column<long>(type: "INTEGER", nullable: false),
                    flag_id = table.Column<long>(type: "INTEGER", nullable: false),
                    operator_id = table.Column<long>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    callsign = table.Column<string>(type: "TEXT", nullable: false),
                    mmsi = table.Column<string>(type: "TEXT", maxLength: 9, nullable: false),
                    tonnage = table.Column<int>(type: "INTEGER", nullable: true),
                    passengers = table.Column<int>(type: "INTEGER", nullable: true),
                    crew = table.Column<int>(type: "INTEGER", nullable: true),
                    decks = table.Column<int>(type: "INTEGER", nullable: true),
                    cabins = table.Column<int>(type: "INTEGER", nullable: true),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REGISTRATION_HISTORY", x => x.id);
                    table.CheckConstraint("CK_Callsign_Characters", "callsign GLOB '[A-Z0-9]*'");
                    table.CheckConstraint("CK_MMSI_Length", "length(mmsi) = 9");
                    table.CheckConstraint("CK_MMSI_Numeric", "mmsi GLOB '[0-9]*'");
                    table.ForeignKey(
                        name: "FK_REGISTRATION_HISTORY_COUNTRY_flag_id",
                        column: x => x.flag_id,
                        principalTable: "COUNTRY",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_REGISTRATION_HISTORY_OPERATOR_operator_id",
                        column: x => x.operator_id,
                        principalTable: "OPERATOR",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_REGISTRATION_HISTORY_VESSEL_TYPE_vessel_type_id",
                        column: x => x.vessel_type_id,
                        principalTable: "VESSEL_TYPE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_REGISTRATION_HISTORY_VESSEL_vessel_id",
                        column: x => x.vessel_id,
                        principalTable: "VESSEL",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIGHTING",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    location_id = table.Column<long>(type: "INTEGER", nullable: false),
                    voyage_id = table.Column<long>(type: "INTEGER", nullable: true),
                    vessel_id = table.Column<long>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    is_my_voyage = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIGHTING", x => x.id);
                    table.ForeignKey(
                        name: "FK_SIGHTING_LOCATION_location_id",
                        column: x => x.location_id,
                        principalTable: "LOCATION",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIGHTING_VESSEL_vessel_id",
                        column: x => x.vessel_id,
                        principalTable: "VESSEL",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIGHTING_VOYAGE_voyage_id",
                        column: x => x.voyage_id,
                        principalTable: "VOYAGE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VOYAGE_EVENT",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    voyage_id = table.Column<long>(type: "INTEGER", nullable: false),
                    event_type = table.Column<int>(type: "INTEGER", nullable: false),
                    port_id = table.Column<long>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VOYAGE_EVENT", x => x.id);
                    table.ForeignKey(
                        name: "FK_VOYAGE_EVENT_PORT_port_id",
                        column: x => x.port_id,
                        principalTable: "PORT",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VOYAGE_EVENT_VOYAGE_voyage_id",
                        column: x => x.voyage_id,
                        principalTable: "VOYAGE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_COUNTRY_code",
                table: "COUNTRY",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_COUNTRY_name",
                table: "COUNTRY",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LOCATION_name",
                table: "LOCATION",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OPERATOR_name",
                table: "OPERATOR",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PORT_code",
                table: "PORT",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PORT_country_id",
                table: "PORT",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_REGISTRATION_HISTORY_flag_id",
                table: "REGISTRATION_HISTORY",
                column: "flag_id");

            migrationBuilder.CreateIndex(
                name: "IX_REGISTRATION_HISTORY_operator_id",
                table: "REGISTRATION_HISTORY",
                column: "operator_id");

            migrationBuilder.CreateIndex(
                name: "IX_REGISTRATION_HISTORY_vessel_id",
                table: "REGISTRATION_HISTORY",
                column: "vessel_id");

            migrationBuilder.CreateIndex(
                name: "IX_REGISTRATION_HISTORY_vessel_type_id",
                table: "REGISTRATION_HISTORY",
                column: "vessel_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_SIGHTING_location_id",
                table: "SIGHTING",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_SIGHTING_vessel_id",
                table: "SIGHTING",
                column: "vessel_id");

            migrationBuilder.CreateIndex(
                name: "IX_SIGHTING_voyage_id",
                table: "SIGHTING",
                column: "voyage_id");

            migrationBuilder.CreateIndex(
                name: "IX_USER_UserName",
                table: "USER",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VESSEL_identifier",
                table: "VESSEL",
                column: "identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VESSEL_TYPE_name",
                table: "VESSEL_TYPE",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VOYAGE_operator_id",
                table: "VOYAGE",
                column: "operator_id");

            migrationBuilder.CreateIndex(
                name: "IX_VOYAGE_vessel_id",
                table: "VOYAGE",
                column: "vessel_id");

            migrationBuilder.CreateIndex(
                name: "IX_VOYAGE_EVENT_port_id",
                table: "VOYAGE_EVENT",
                column: "port_id");

            migrationBuilder.CreateIndex(
                name: "IX_VOYAGE_EVENT_voyage_id",
                table: "VOYAGE_EVENT",
                column: "voyage_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlagStatistics");

            migrationBuilder.DropTable(
                name: "JOB_STATUS");

            migrationBuilder.DropTable(
                name: "LocationStatistics");

            migrationBuilder.DropTable(
                name: "MyVoyages");

            migrationBuilder.DropTable(
                name: "OperatorStatistics");

            migrationBuilder.DropTable(
                name: "REGISTRATION_HISTORY");

            migrationBuilder.DropTable(
                name: "SIGHTING");

            migrationBuilder.DropTable(
                name: "SightingsByMonth");

            migrationBuilder.DropTable(
                name: "USER");

            migrationBuilder.DropTable(
                name: "VesselTypeStatistics");

            migrationBuilder.DropTable(
                name: "VOYAGE_EVENT");

            migrationBuilder.DropTable(
                name: "VESSEL_TYPE");

            migrationBuilder.DropTable(
                name: "LOCATION");

            migrationBuilder.DropTable(
                name: "PORT");

            migrationBuilder.DropTable(
                name: "VOYAGE");

            migrationBuilder.DropTable(
                name: "COUNTRY");

            migrationBuilder.DropTable(
                name: "OPERATOR");

            migrationBuilder.DropTable(
                name: "VESSEL");
        }
    }
}
