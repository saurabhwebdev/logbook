using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreEngine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVentilationAndGasMonitoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GasReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReadingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GasType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Concentration = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThresholdTWA = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ThresholdSTEL = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ThresholdCeiling = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    IsExceedance = table.Column<bool>(type: "bit", nullable: false),
                    LocationDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReadingDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InstrumentId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CalibrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionTaken = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Normal"),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GasReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GasReadings_MineAreas_MineAreaId",
                        column: x => x.MineAreaId,
                        principalTable: "MineAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GasReadings_MineSites_MineSiteId",
                        column: x => x.MineSiteId,
                        principalTable: "MineSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VentilationReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReadingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LocationDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AirflowVelocity = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    AirflowVolume = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Humidity = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    BarometricPressure = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ReadingDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InstrumentUsed = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DoorStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FanStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VentilationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Normal"),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentilationReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VentilationReadings_MineAreas_MineAreaId",
                        column: x => x.MineAreaId,
                        principalTable: "MineAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VentilationReadings_MineSites_MineSiteId",
                        column: x => x.MineSiteId,
                        principalTable: "MineSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GasReadings_MineAreaId",
                table: "GasReadings",
                column: "MineAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_GasReadings_MineSiteId",
                table: "GasReadings",
                column: "MineSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_GasReadings_TenantId_ReadingNumber",
                table: "GasReadings",
                columns: new[] { "TenantId", "ReadingNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_VentilationReadings_MineAreaId",
                table: "VentilationReadings",
                column: "MineAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_VentilationReadings_MineSiteId",
                table: "VentilationReadings",
                column: "MineSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_VentilationReadings_TenantId_ReadingNumber",
                table: "VentilationReadings",
                columns: new[] { "TenantId", "ReadingNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GasReadings");

            migrationBuilder.DropTable(
                name: "VentilationReadings");
        }
    }
}
