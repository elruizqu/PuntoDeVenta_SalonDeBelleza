using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations.Salon
{
    /// <inheritdoc />
    public partial class CitaFactura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FacturaId",
                table: "Citas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Citas_FacturaId",
                table: "Citas",
                column: "FacturaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Facturas_FacturaId",
                table: "Citas",
                column: "FacturaId",
                principalTable: "Facturas",
                principalColumn: "FacturaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Facturas_FacturaId",
                table: "Citas");

            migrationBuilder.DropIndex(
                name: "IX_Citas_FacturaId",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "FacturaId",
                table: "Citas");
        }
    }
}
