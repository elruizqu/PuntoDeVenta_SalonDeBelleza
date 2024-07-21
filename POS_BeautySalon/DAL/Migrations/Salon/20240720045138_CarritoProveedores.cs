using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations.Salon
{
    /// <inheritdoc />
    public partial class CarritoProveedores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarritoProveedores",
                columns: table => new
                {
                    CarritoProveedorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarritoProveedores", x => x.CarritoProveedorId);
                    table.ForeignKey(
                        name: "FK_CarritoProveedores_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacturaProveedores",
                columns: table => new
                {
                    FacturaProveedorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrecioTotal = table.Column<int>(type: "int", nullable: false),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaProveedores", x => x.FacturaProveedorId);
                    table.ForeignKey(
                        name: "FK_FacturaProveedores_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarritoProvProductos",
                columns: table => new
                {
                    CarritoProvProductoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarritoProveedorId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarritoProvProductos", x => x.CarritoProvProductoId);
                    table.ForeignKey(
                        name: "FK_CarritoProvProductos_CarritoProveedores_CarritoProveedorId",
                        column: x => x.CarritoProveedorId,
                        principalTable: "CarritoProveedores",
                        principalColumn: "CarritoProveedorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarritoProvProductos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetalleProveedorFacturas",
                columns: table => new
                {
                    DetalleProveedorFacturaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaProveedorId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<int>(type: "int", nullable: false),
                    FacturaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleProveedorFacturas", x => x.DetalleProveedorFacturaId);
                    table.ForeignKey(
                        name: "FK_DetalleProveedorFacturas_FacturaProveedores_FacturaProveedorId",
                        column: x => x.FacturaProveedorId,
                        principalTable: "FacturaProveedores",
                        principalColumn: "FacturaProveedorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleProveedorFacturas_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "FacturaId");
                    table.ForeignKey(
                        name: "FK_DetalleProveedorFacturas_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarritoProveedores_ProveedorId",
                table: "CarritoProveedores",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoProvProductos_CarritoProveedorId",
                table: "CarritoProvProductos",
                column: "CarritoProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoProvProductos_ProductoId",
                table: "CarritoProvProductos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleProveedorFacturas_FacturaId",
                table: "DetalleProveedorFacturas",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleProveedorFacturas_FacturaProveedorId",
                table: "DetalleProveedorFacturas",
                column: "FacturaProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleProveedorFacturas_ProductoId",
                table: "DetalleProveedorFacturas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaProveedores_ProveedorId",
                table: "FacturaProveedores",
                column: "ProveedorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarritoProvProductos");

            migrationBuilder.DropTable(
                name: "DetalleProveedorFacturas");

            migrationBuilder.DropTable(
                name: "CarritoProveedores");

            migrationBuilder.DropTable(
                name: "FacturaProveedores");
        }
    }
}
