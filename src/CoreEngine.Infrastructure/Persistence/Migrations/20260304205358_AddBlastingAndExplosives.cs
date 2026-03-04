using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreEngine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBlastingAndExplosives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlastEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BlastNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BlastType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScheduledDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DrillingPattern = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NumberOfHoles = table.Column<int>(type: "int", nullable: true),
                    TotalExplosivesKg = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExplosiveType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DetonatorType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Planned"),
                    BlastDesignNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SafetyRadius = table.Column<double>(type: "float", nullable: true),
                    EvacuationConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SentryPostsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PreBlastWarningGiven = table.Column<bool>(type: "bit", nullable: false),
                    SupervisorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LicensedBlasterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VibrationReading = table.Column<double>(type: "float", nullable: true),
                    AirBlastReading = table.Column<double>(type: "float", nullable: true),
                    PostBlastInspection = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PostBlastNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    FragmentationQuality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MisfireCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlastEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlastEvents_MineAreas_MineAreaId",
                        column: x => x.MineAreaId,
                        principalTable: "MineAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlastEvents_MineSites_MineSiteId",
                        column: x => x.MineSiteId,
                        principalTable: "MineSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DispatchRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DispatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VehicleNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Material = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SourceLocation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DestinationLocation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    WeighbridgeTicketNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TareWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Tonnes"),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Loading"),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispatchRecords_MineSites_MineSiteId",
                        column: x => x.MineSiteId,
                        principalTable: "MineSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShiftInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LogNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Material = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceLocation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DestinationLocation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    QuantityTonnes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantityBCM = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EquipmentUsed = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OperatorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HaulingDistance = table.Column<double>(type: "float", nullable: true),
                    LoadCount = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Draft"),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionLogs_MineAreas_MineAreaId",
                        column: x => x.MineAreaId,
                        principalTable: "MineAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionLogs_MineSites_MineSiteId",
                        column: x => x.MineSiteId,
                        principalTable: "MineSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExplosiveUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlastEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExplosiveName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuantityIssued = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    QuantityUsed = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    QuantityReturned = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "kg"),
                    MagazineSource = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExplosiveUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExplosiveUsages_BlastEvents_BlastEventId",
                        column: x => x.BlastEventId,
                        principalTable: "BlastEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlastEvents_MineAreaId",
                table: "BlastEvents",
                column: "MineAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_BlastEvents_MineSiteId",
                table: "BlastEvents",
                column: "MineSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_BlastEvents_TenantId_BlastNumber",
                table: "BlastEvents",
                columns: new[] { "TenantId", "BlastNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchRecords_MineSiteId",
                table: "DispatchRecords",
                column: "MineSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchRecords_TenantId_DispatchNumber",
                table: "DispatchRecords",
                columns: new[] { "TenantId", "DispatchNumber" },
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExplosiveUsages_BlastEventId",
                table: "ExplosiveUsages",
                column: "BlastEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionLogs_MineAreaId",
                table: "ProductionLogs",
                column: "MineAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionLogs_MineSiteId",
                table: "ProductionLogs",
                column: "MineSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionLogs_TenantId_LogNumber",
                table: "ProductionLogs",
                columns: new[] { "TenantId", "LogNumber" },
                unique: true,
                filter: "IsDeleted = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DispatchRecords");

            migrationBuilder.DropTable(
                name: "ExplosiveUsages");

            migrationBuilder.DropTable(
                name: "ProductionLogs");

            migrationBuilder.DropTable(
                name: "BlastEvents");
        }
    }
}
