using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreEngine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSafetyIncidents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SafetyIncidents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IncidentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IncidentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Low"),
                    IncidentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ImmediateActions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ReportedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InjuredPersonName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InjuredPersonRole = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InjuryType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BodyPartAffected = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LostTimeDays = table.Column<int>(type: "int", nullable: true),
                    IsReportable = table.Column<bool>(type: "bit", nullable: false),
                    RegulatoryReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    WitnessNames = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ContributingFactors = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CorrectiveActions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CorrectiveActionDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CorrectiveActionCompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Open"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyIncidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SafetyIncidents_MineAreas_MineAreaId",
                        column: x => x.MineAreaId,
                        principalTable: "MineAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SafetyIncidents_MineSites_MineSiteId",
                        column: x => x.MineSiteId,
                        principalTable: "MineSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncidentInvestigations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SafetyIncidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestigatorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InvestigationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Methodology = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    RootCauseAnalysis = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Recommendations = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PreventiveMeasures = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EvidenceReferences = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "InProgress"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentInvestigations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentInvestigations_SafetyIncidents_SafetyIncidentId",
                        column: x => x.SafetyIncidentId,
                        principalTable: "SafetyIncidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentInvestigations_SafetyIncidentId",
                table: "IncidentInvestigations",
                column: "SafetyIncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_SafetyIncidents_MineAreaId",
                table: "SafetyIncidents",
                column: "MineAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_SafetyIncidents_MineSiteId",
                table: "SafetyIncidents",
                column: "MineSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_SafetyIncidents_TenantId_IncidentNumber",
                table: "SafetyIncidents",
                columns: new[] { "TenantId", "IncidentNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentInvestigations");

            migrationBuilder.DropTable(
                name: "SafetyIncidents");
        }
    }
}
