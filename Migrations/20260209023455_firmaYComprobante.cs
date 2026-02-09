using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoIntegradorS6G7.Migrations
{
    /// <inheritdoc />
    public partial class firmaYComprobante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "comprobanteRuta",
                table: "Pagos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "firmaBase64",
                table: "Pagos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "registradoPor",
                table: "Pagos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comprobanteRuta",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "firmaBase64",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "registradoPor",
                table: "Pagos");
        }
    }
}
