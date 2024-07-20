using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations.Salon
{
    /// <inheritdoc />
    public partial class AñadirAlertaAProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alerta",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alerta",
                table: "Productos");
        }
    }
}
