using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineChronometer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chronometers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    AccumulatedMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    RunningSince = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chronometers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChronometerLaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChronometerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    TotalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    LapMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChronometerLaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChronometerLaps_Chronometers_ChronometerId",
                        column: x => x.ChronometerId,
                        principalTable: "Chronometers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChronometerLaps_ChronometerId_Number",
                table: "ChronometerLaps",
                columns: new[] { "ChronometerId", "Number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChronometerLaps");

            migrationBuilder.DropTable(
                name: "Chronometers");
        }
    }
}
