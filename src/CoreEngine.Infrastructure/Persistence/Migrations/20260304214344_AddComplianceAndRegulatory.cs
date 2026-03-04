using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreEngine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddComplianceAndRegulatory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComplianceRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Jurisdiction = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    RegulatoryBody = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReferenceDocument = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "AsRequired"),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastCompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponsibleRole = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Medium"),
                    PenaltyForNonCompliance = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplianceRequirements_MineSites_MineSiteId",
                        column: x => x.MineSiteId,
                        principalTable: "MineSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComplianceAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComplianceRequirementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuditNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AuditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AuditType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ComplianceStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Compliant"),
                    CorrectiveActions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ActionDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionCompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvidenceReferences = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Open"),
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
                    table.PrimaryKey("PK_ComplianceAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplianceAudits_ComplianceRequirements_ComplianceRequirementId",
                        column: x => x.ComplianceRequirementId,
                        principalTable: "ComplianceRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceAudits_ComplianceRequirementId",
                table: "ComplianceAudits",
                column: "ComplianceRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceAudits_TenantId_AuditNumber",
                table: "ComplianceAudits",
                columns: new[] { "TenantId", "AuditNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRequirements_MineSiteId",
                table: "ComplianceRequirements",
                column: "MineSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRequirements_TenantId_Code",
                table: "ComplianceRequirements",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplianceAudits");

            migrationBuilder.DropTable(
                name: "ComplianceRequirements");
        }
    }
}
