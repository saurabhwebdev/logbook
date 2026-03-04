using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreEngine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEnvironmentalMonitoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnvironmentalIncidents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IncidentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ImpactAssessment = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ContainmentActions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RemediationPlan = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReportedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NotifiedAuthority = table.Column<bool>(type: "bit", nullable: false),
                    AuthorityReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Open"),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosureNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentalIncidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnvironmentalIncidents_MineSites_MineSiteId",
                        column: x => x.MineSiteId,
                        principalTable: "MineSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnvironmentalReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReadingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReadingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Parameter = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThresholdMin = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ThresholdMax = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    IsExceedance = table.Column<bool>(type: "bit", nullable: false),
                    ReadingDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MonitoringStation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InstrumentUsed = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CalibratedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecordedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WeatherConditions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Normal"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentalReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnvironmentalReadings_MineAreas_MineAreaId",
                        column: x => x.MineAreaId,
                        principalTable: "MineAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EnvironmentalReadings_MineSites_MineSiteId",
                        column: x => x.MineSiteId,
                        principalTable: "MineSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalIncidents_MineSiteId",
                table: "EnvironmentalIncidents",
                column: "MineSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalIncidents_TenantId_IncidentNumber",
                table: "EnvironmentalIncidents",
                columns: new[] { "TenantId", "IncidentNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalReadings_MineAreaId",
                table: "EnvironmentalReadings",
                column: "MineAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalReadings_MineSiteId",
                table: "EnvironmentalReadings",
                column: "MineSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalReadings_TenantId_ReadingNumber",
                table: "EnvironmentalReadings",
                columns: new[] { "TenantId", "ReadingNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnvironmentalIncidents");

            migrationBuilder.DropTable(
                name: "EnvironmentalReadings");
        }
    }
}
