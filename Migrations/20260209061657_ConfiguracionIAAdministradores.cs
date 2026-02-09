using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoIntegradorS6G7.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguracionIAAdministradores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuracion");

            migrationBuilder.CreateTable(
                name: "ConfiguracionIA",
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
                    table.PrimaryKey("PK_ConfiguracionIA", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionIA");

            migrationBuilder.CreateTable(
                name: "Configuracion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    actualizadoPor = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cuotasMaximasPorDefecto = table.Column<int>(type: "int", nullable: false),
                    cuotasMinimasPorDefecto = table.Column<int>(type: "int", nullable: false),
                    diasParaMorosidad = table.Column<int>(type: "int", nullable: false),
                    diasParaRiesgoAlto = table.Column<int>(type: "int", nullable: false),
                    fechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    interesMaximo = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    interesMinimo = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    interesRiesgoAlto = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    interesRiesgoBajo = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    interesRiesgoMedio = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    umbralRiesgoAlto = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    umbralRiesgoBajo = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    umbralRiesgoMedio = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuracion", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
