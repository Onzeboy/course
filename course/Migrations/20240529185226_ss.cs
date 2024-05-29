using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace course.Migrations
{
    /// <inheritdoc />
    public partial class ss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    orderdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    adresscity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    adressstreet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    adresshome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "producthistory",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producthistory", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "history",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    orderdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    adresscity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    adressstreet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    adresshome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_history", x => x.ID);
                    table.ForeignKey(
                        name: "FK_history_orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "orders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "cart",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductsHistoryID = table.Column<int>(type: "int", nullable: false),
                    ProductsID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart", x => x.ID);
                    table.ForeignKey(
                        name: "FK_cart_product_ProductsID",
                        column: x => x.ProductsID,
                        principalTable: "product",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_cart_producthistory_ProductsHistoryID",
                        column: x => x.ProductsHistoryID,
                        principalTable: "producthistory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "carthistory",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cartID = table.Column<int>(type: "int", nullable: false),
                    ProductsHistoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carthistory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_carthistory_producthistory_ProductsHistoryID",
                        column: x => x.ProductsHistoryID,
                        principalTable: "producthistory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cart_ProductsHistoryID",
                table: "cart",
                column: "ProductsHistoryID");

            migrationBuilder.CreateIndex(
                name: "IX_cart_ProductsID",
                table: "cart",
                column: "ProductsID");

            migrationBuilder.CreateIndex(
                name: "IX_carthistory_ProductsHistoryID",
                table: "carthistory",
                column: "ProductsHistoryID");

            migrationBuilder.CreateIndex(
                name: "IX_history_OrderID",
                table: "history",
                column: "OrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cart");

            migrationBuilder.DropTable(
                name: "carthistory");

            migrationBuilder.DropTable(
                name: "history");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "producthistory");

            migrationBuilder.DropTable(
                name: "orders");
        }
    }
}
