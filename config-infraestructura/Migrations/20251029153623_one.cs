using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace config_infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class one : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entornos",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entornos", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "Variables",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_sensitive = table.Column<bool>(type: "boolean", nullable: false),
                    name_entorno = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variables", x => x.name);
                    table.ForeignKey(
                        name: "FK_Variables_Entornos_name_entorno",
                        column: x => x.name_entorno,
                        principalTable: "Entornos",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Variables_name_entorno",
                table: "Variables",
                column: "name_entorno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Variables");

            migrationBuilder.DropTable(
                name: "Entornos");
        }
    }
}
