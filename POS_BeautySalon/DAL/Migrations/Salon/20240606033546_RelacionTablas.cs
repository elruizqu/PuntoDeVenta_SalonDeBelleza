using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations.Salon
{
    /// <inheritdoc />
    public partial class RelacionTablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Facturas_FacturaId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Facturas_FacturaId",
                table: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Productos_FacturaId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FacturaId",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "FacturaId",
                table: "Servicios",
                newName: "ProveedorId");

            migrationBuilder.RenameIndex(
                name: "IX_Servicios_FacturaId",
                table: "Servicios",
                newName: "IX_Servicios_ProveedorId");

            migrationBuilder.AlterColumn<int>(
                name: "ServicioId",
                table: "Facturas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                table: "Facturas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Carritos",
                columns: table => new
                {
                    CarritoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carritos", x => x.CarritoId);
                    table.ForeignKey(
                        name: "FK_Carritos_AspNetUsers_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Citas",
                columns: table => new
                {
                    CitaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServicioId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.CitaId);
                    table.ForeignKey(
                        name: "FK_Citas_AspNetUsers_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citas_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "ServicioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Promociones",
                columns: table => new
                {
                    PromocionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenPromocion = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ServicioId = table.Column<int>(type: "int", nullable: true),
                    ProductoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promociones", x => x.PromocionId);
                    table.ForeignKey(
                        name: "FK_Promociones_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId");
                    table.ForeignKey(
                        name: "FK_Promociones_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "ServicioId");
                });

            migrationBuilder.CreateTable(
                name: "CarritoProductos",
                columns: table => new
                {
                    CarritoProductoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarritoId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarritoProductos", x => x.CarritoProductoId);
                    table.ForeignKey(
                        name: "FK_CarritoProductos_Carritos_CarritoId",
                        column: x => x.CarritoId,
                        principalTable: "Carritos",
                        principalColumn: "CarritoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarritoProductos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_ProductoId",
                table: "Facturas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_ServicioId",
                table: "Facturas",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoProductos_CarritoId",
                table: "CarritoProductos",
                column: "CarritoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoProductos_ProductoId",
                table: "CarritoProductos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Carritos_ClienteId",
                table: "Carritos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_ClienteId",
                table: "Citas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_ServicioId",
                table: "Citas",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_ProductoId",
                table: "Promociones",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_ServicioId",
                table: "Promociones",
                column: "ServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_Productos_ProductoId",
                table: "Facturas",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_Servicios_ServicioId",
                table: "Facturas",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "ServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Proveedores_ProveedorId",
                table: "Servicios",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "ProveedorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_Productos_ProductoId",
                table: "Facturas");

            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_Servicios_ServicioId",
                table: "Facturas");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Proveedores_ProveedorId",
                table: "Servicios");

            migrationBuilder.DropTable(
                name: "CarritoProductos");

            migrationBuilder.DropTable(
                name: "Citas");

            migrationBuilder.DropTable(
                name: "Promociones");

            migrationBuilder.DropTable(
                name: "Carritos");

            migrationBuilder.DropIndex(
                name: "IX_Facturas_ProductoId",
                table: "Facturas");

            migrationBuilder.DropIndex(
                name: "IX_Facturas_ServicioId",
                table: "Facturas");

            migrationBuilder.RenameColumn(
                name: "ProveedorId",
                table: "Servicios",
                newName: "FacturaId");

            migrationBuilder.RenameIndex(
                name: "IX_Servicios_ProveedorId",
                table: "Servicios",
                newName: "IX_Servicios_FacturaId");

            migrationBuilder.AddColumn<int>(
                name: "FacturaId",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ServicioId",
                table: "Facturas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                table: "Facturas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_FacturaId",
                table: "Productos",
                column: "FacturaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Facturas_FacturaId",
                table: "Productos",
                column: "FacturaId",
                principalTable: "Facturas",
                principalColumn: "FacturaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Facturas_FacturaId",
                table: "Servicios",
                column: "FacturaId",
                principalTable: "Facturas",
                principalColumn: "FacturaId");
        }
    }
}
