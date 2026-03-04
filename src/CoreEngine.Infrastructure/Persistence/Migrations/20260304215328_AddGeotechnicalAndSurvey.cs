using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreEngine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGeotechnicalAndSurvey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeotechnicalAssessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssessmentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AssessmentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssessorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RockMassRating = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    SlopeAngle = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    WaterTableDepth = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    GroundCondition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StabilityStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Stable"),
                    RecommendedActions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MonitoringRequired = table.Column<bool>(type: "bit", nullable: false),
                    NextAssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Draft"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeotechnicalAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeotechnicalAssessments_MineAreas_MineAreaId",
                        column: x => x.MineAreaId,
                        principalTable: "MineAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeotechnicalAssessments_MineSites_MineSiteId",
                        column: x => x.MineSiteId,
                        principalTable: "MineSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurveyRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SurveyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SurveyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SurveyorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SurveyorLicense = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Easting = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Northing = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Elevation = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Datum = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CoordinateSystem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EquipmentUsed = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Accuracy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VolumeCalculated = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    AreaCalculated = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Findings = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Draft"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyRecords_MineAreas_MineAreaId",
                        column: x => x.MineAreaId,
                        principalTable: "MineAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyRecords_MineSites_MineSiteId",
                        column: x => x.MineSiteId,
                        principalTable: "MineSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeotechnicalAssessments_MineAreaId",
                table: "GeotechnicalAssessments",
                column: "MineAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_GeotechnicalAssessments_MineSiteId",
                table: "GeotechnicalAssessments",
                column: "MineSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_GeotechnicalAssessments_TenantId_AssessmentNumber",
                table: "GeotechnicalAssessments",
                columns: new[] { "TenantId", "AssessmentNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyRecords_MineAreaId",
                table: "SurveyRecords",
                column: "MineAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyRecords_MineSiteId",
                table: "SurveyRecords",
                column: "MineSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyRecords_TenantId_SurveyNumber",
                table: "SurveyRecords",
                columns: new[] { "TenantId", "SurveyNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeotechnicalAssessments");

            migrationBuilder.DropTable(
                name: "SurveyRecords");
        }
    }
}
