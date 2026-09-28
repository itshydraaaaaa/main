using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Industrie.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ecrans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ecrans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OSList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OSList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PcName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Session = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mdp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdresseIpExterne = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdresseIpInterne = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bukup = table.Column<bool>(type: "bit", nullable: false),
                    DateBukup = table.Column<DateOnly>(type: "date", nullable: false),
                    EcranId = table.Column<int>(type: "int", nullable: false),
                    OSId = table.Column<int>(type: "int", nullable: false),
                    SiteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Machines_Ecrans_EcranId",
                        column: x => x.EcranId,
                        principalTable: "Ecrans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Machines_OSList_OSId",
                        column: x => x.OSId,
                        principalTable: "OSList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Machines_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Machines_EcranId",
                table: "Machines",
                column: "EcranId");

            migrationBuilder.CreateIndex(
                name: "IX_Machines_OSId",
                table: "Machines",
                column: "OSId");

            migrationBuilder.CreateIndex(
                name: "IX_Machines_SiteId",
                table: "Machines",
                column: "SiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "Ecrans");

            migrationBuilder.DropTable(
                name: "OSList");

            migrationBuilder.DropTable(
                name: "Sites");
        }
    }
}
