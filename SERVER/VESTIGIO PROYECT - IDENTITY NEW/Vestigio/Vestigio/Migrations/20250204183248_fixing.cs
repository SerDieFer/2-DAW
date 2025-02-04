using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vestigio.Migrations
{
    /// <inheritdoc />
    public partial class fixing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Verificar si la clave primaria existe antes de eliminarla
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.key_constraints WHERE [name] = 'PK_ProductSize')
                ALTER TABLE [ProductSize] DROP CONSTRAINT [PK_ProductSize];
            ");

            // Eliminar la columna 'Id' para poder recrearla con IDENTITY
            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductSize"
            );

            // Volver a crear la columna 'Id' con IDENTITY
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductSize",
                type: "int",
                nullable: false
            ).Annotation("SqlServer:Identity", "1, 1");

            // Agregar la clave primaria basada en 'Id'
            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSize",
                table: "ProductSize",
                column: "Id"
            );

            // Agregar la columna ProductSizeId en OrderDetails
            migrationBuilder.AddColumn<int>(
                name: "ProductSizeId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            // Crear índice único en ProductId y Size
            migrationBuilder.CreateIndex(
                name: "UQ_ProductId_Size",
                table: "ProductSize",
                columns: new[] { "ProductId", "Size" },
                unique: true
            );

            // Crear índice para ProductSizeId en OrderDetails
            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductSizeId",
                table: "OrderDetails",
                column: "ProductSizeId"
            );

            // Agregar la clave foránea de OrderDetails a ProductSize
            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_ProductSize_ProductSizeId",
                table: "OrderDetails",
                column: "ProductSizeId",
                principalTable: "ProductSize",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar clave foránea si existe
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE [name] = 'FK_OrderDetails_ProductSize_ProductSizeId')
                ALTER TABLE [OrderDetails] DROP CONSTRAINT [FK_OrderDetails_ProductSize_ProductSizeId];
            ");

            // Eliminar índice único en ProductSize
            migrationBuilder.DropIndex(
                name: "UQ_ProductId_Size",
                table: "ProductSize"
            );

            // Eliminar índice en OrderDetails
            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductSizeId",
                table: "OrderDetails"
            );

            // Eliminar la columna ProductSizeId en OrderDetails
            migrationBuilder.DropColumn(
                name: "ProductSizeId",
                table: "OrderDetails"
            );

            // Eliminar la clave primaria en ProductSize
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.key_constraints WHERE [name] = 'PK_ProductSize')
                ALTER TABLE [ProductSize] DROP CONSTRAINT [PK_ProductSize];
            ");

            // Eliminar la columna 'Id' con IDENTITY
            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductSize"
            );

            // Volver a agregar 'Id' sin IDENTITY
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductSize",
                type: "int",
                nullable: false
            );

            // Restaurar clave primaria original en ProductSize
            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSize",
                table: "ProductSize",
                columns: new[] { "ProductId", "Size" }
            );
        }
    }
}
