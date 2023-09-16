using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VManager.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vtuber",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TelegramId = table.Column<long>(type: "INTEGER", nullable: false),
                    TwitchId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Kicked = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastSubs = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vtuber", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dates",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date_Description = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    VtuberId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Important = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dates_Vtuber_VtuberId",
                        column: x => x.VtuberId,
                        principalTable: "Vtuber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Vtuber",
                columns: new[] { "Id", "Color", "Image", "Kicked", "LastSubs", "Name", "TelegramId", "TwitchId" },
                values: new object[,]
                {
                    { 1ul, "c8738b", new byte[0], false, 0ul, "KraNf", 493373972L, 808276081ul },
                    { 2ul, "cb2b72", new byte[0], false, 0ul, "Tsunya", 545498845L, 118662043ul },
                    { 3ul, "ad77ca", new byte[0], false, 0ul, "Pewa", 532671775L, 827228470ul },
                    { 4ul, "b4323d", new byte[0], false, 0ul, "Aya", 930000855L, 547036080ul },
                    { 5ul, "e8bd5e", new byte[0], false, 0ul, "xXxpososu", 818767715L, 820065100ul }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dates_VtuberId",
                table: "Dates",
                column: "VtuberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dates");

            migrationBuilder.DropTable(
                name: "Vtuber");
        }
    }
}
