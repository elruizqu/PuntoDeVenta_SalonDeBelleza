using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations.Salon
{
    /// <inheritdoc />
    public partial class ModificacionListaDeseos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_ListaDeseos_ListaDeseoListaID",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ListaDeseoListaID",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ListaDeseoListaID",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "ListaDeseos");

            migrationBuilder.CreateTable(
                name: "ListaDeseoProductos",
                columns: table => new
                {
                    ListaDeseoProductoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListaID = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaDeseoProductos", x => x.ListaDeseoProductoId);
                    table.ForeignKey(
                        name: "FK_ListaDeseoProductos_ListaDeseos_ListaID",
                        column: x => x.ListaID,
                        principalTable: "ListaDeseos",
                        principalColumn: "ListaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListaDeseoProductos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListaDeseoProductos_ListaID",
                table: "ListaDeseoProductos",
                column: "ListaID");

            migrationBuilder.CreateIndex(
                name: "IX_ListaDeseoProductos_ProductoId",
                table: "ListaDeseoProductos",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListaDeseoProductos");

            migrationBuilder.AddColumn<int>(
                name: "ListaDeseoListaID",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "ListaDeseos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ListaDeseoListaID",
                table: "Productos",
                column: "ListaDeseoListaID");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_ListaDeseos_ListaDeseoListaID",
                table: "Productos",
                column: "ListaDeseoListaID",
                principalTable: "ListaDeseos",
                principalColumn: "ListaID");
        }
    }
}
