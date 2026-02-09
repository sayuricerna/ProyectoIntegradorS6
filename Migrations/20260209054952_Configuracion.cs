using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoIntegradorS6G7.Migrations
{
    /// <inheritdoc />
    public partial class Configuracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombreCompleto = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cedula = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefono = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    fechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    creadoPor = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    puedeCrearCreditos = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    puedeGestionarCobranzas = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    puedeVerReportes = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    puedeConfigurar = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Configuracion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    interesMinimo = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    interesMaximo = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    interesRiesgoBajo = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    interesRiesgoMedio = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    interesRiesgoAlto = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    diasParaMorosidad = table.Column<int>(type: "int", nullable: false),
                    diasParaRiesgoAlto = table.Column<int>(type: "int", nullable: false),
                    umbralRiesgoBajo = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    umbralRiesgoMedio = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    umbralRiesgoAlto = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    cuotasMinimasPorDefecto = table.Column<int>(type: "int", nullable: false),
                    cuotasMaximasPorDefecto = table.Column<int>(type: "int", nullable: false),
                    fechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    actualizadoPor = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuracion", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Configuracion");
        }
    }
}
