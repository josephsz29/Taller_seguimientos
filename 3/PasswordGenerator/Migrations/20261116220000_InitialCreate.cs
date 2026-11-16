using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasswordGenerator.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Length = table.Column<int>(type: "int", nullable: false),
                    IncludeUppercase = table.Column<bool>(type: "bit", nullable: false),
                    IncludeLowercase = table.Column<bool>(type: "bit", nullable: false),
                    IncludeDigits = table.Column<bool>(type: "bit", nullable: false),
                    IncludeSymbols = table.Column<bool>(type: "bit", nullable: false),
                    ExcludeAmbiguous = table.Column<bool>(type: "bit", nullable: false),
                    Strength = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordRecords");
        }
    }
}
