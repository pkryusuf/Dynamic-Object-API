using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRUD.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsForCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts");

            migrationBuilder.RenameTable(
                name: "OrderProducts",
                newName: "OrderProduct");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProduct",
                table: "OrderProduct",
                column: "OrderProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_OrderId",
                table: "OrderProduct",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProduct_Order_OrderId",
                table: "OrderProduct",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProduct_Order_OrderId",
                table: "OrderProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProduct",
                table: "OrderProduct");

            migrationBuilder.DropIndex(
                name: "IX_OrderProduct_OrderId",
                table: "OrderProduct");

            migrationBuilder.RenameTable(
                name: "OrderProduct",
                newName: "OrderProducts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts",
                column: "OrderProductId");
        }
    }
}
