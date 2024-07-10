using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations.Salon
{
    /// <inheritdoc />
    public partial class ActualizacionConTextCierre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cierres",
                columns: table => new
                {
                    CierreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalProductos = table.Column<int>(type: "int", nullable: false),
                    TotalServicios = table.Column<int>(type: "int", nullable: false),
                    TotalCierre = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cierres", x => x.CierreId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cierres");
        }
    }
}
