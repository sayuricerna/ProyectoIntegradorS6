using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoIntegradorS6G7.Migrations
{
    /// <inheritdoc />
    public partial class UnificarUsuariosYAdministradores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "cedula",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "creadoPor",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "fechaCreacion",
                table: "Usuarios",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "nombreCompleto",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "puedeConfigurar",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "puedeCrearCreditos",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "puedeGestionarCobranzas",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "puedeVerReportes",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "telefono",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "activo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "cedula",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "creadoPor",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "fechaCreacion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "nombreCompleto",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "puedeConfigurar",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "puedeCrearCreditos",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "puedeGestionarCobranzas",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "puedeVerReportes",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "telefono",
                table: "Usuarios");

            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cedula = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    creadoPor = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    nombreCompleto = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    puedeConfigurar = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    puedeCrearCreditos = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    puedeGestionarCobranzas = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    puedeVerReportes = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    telefono = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
