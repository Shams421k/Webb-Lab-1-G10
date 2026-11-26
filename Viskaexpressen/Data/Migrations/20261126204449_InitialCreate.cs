using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Viskaexpressen.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Linjer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Namn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tagtyp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Beskrivning = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RestidMinuter = table.Column<int>(type: "int", nullable: false),
                    Pris = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Linjer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tagsatter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nummer = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AntalPlatser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tagsatter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Avgangar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LinjeId = table.Column<int>(type: "int", nullable: false),
                    TagsattId = table.Column<int>(type: "int", nullable: false),
                    Avgangstid = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avgangar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Avgangar_Linjer_LinjeId",
                        column: x => x.LinjeId,
                        principalTable: "Linjer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Avgangar_Tagsatter_TagsattId",
                        column: x => x.TagsattId,
                        principalTable: "Tagsatter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bokningar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvgangId = table.Column<int>(type: "int", nullable: false),
                    PlatsNummer = table.Column<int>(type: "int", nullable: false),
                    Bokningsnummer = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Namn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Epost = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bokningar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bokningar_Avgangar_AvgangId",
                        column: x => x.AvgangId,
                        principalTable: "Avgangar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avgangar_LinjeId",
                table: "Avgangar",
                column: "LinjeId");

            migrationBuilder.CreateIndex(
                name: "IX_Avgangar_TagsattId",
                table: "Avgangar",
                column: "TagsattId");

            migrationBuilder.CreateIndex(
                name: "IX_Bokningar_AvgangId_PlatsNummer",
                table: "Bokningar",
                columns: new[] { "AvgangId", "PlatsNummer" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bokningar_Bokningsnummer",
                table: "Bokningar",
                column: "Bokningsnummer",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tagsatter_Nummer",
                table: "Tagsatter",
                column: "Nummer",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bokningar");

            migrationBuilder.DropTable(
                name: "Avgangar");

            migrationBuilder.DropTable(
                name: "Linjer");

            migrationBuilder.DropTable(
                name: "Tagsatter");
        }
    }
}
