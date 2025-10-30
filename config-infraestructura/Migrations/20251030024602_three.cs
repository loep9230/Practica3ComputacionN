using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace config_infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class three : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Variables",
                table: "Variables");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Variables",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Variables",
                table: "Variables",
                columns: new[] { "name", "name_entorno" });

            migrationBuilder.CreateIndex(
                name: "IX_Variables_name_entorno",
                table: "Variables",
                column: "name_entorno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Variables",
                table: "Variables");

            migrationBuilder.DropIndex(
                name: "IX_Variables_name_entorno",
                table: "Variables");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Variables",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Variables",
                table: "Variables",
                column: "name_entorno");
        }
    }
}
