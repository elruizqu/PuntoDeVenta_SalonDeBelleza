using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations.Salon
{
    /// <inheritdoc />
    public partial class precioProv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrecioProveedor",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioProveedor",
                table: "Productos");
        }
    }
}
