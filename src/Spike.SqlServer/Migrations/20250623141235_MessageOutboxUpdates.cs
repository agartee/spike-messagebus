using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spike.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class MessageOutboxUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Json",
                schema: "Spike",
                table: "MessageOutbox",
                newName: "Body");

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                schema: "Spike",
                table: "MessageOutbox",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Body",
                schema: "Spike",
                table: "MessageOutbox",
                newName: "Json");

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                schema: "Spike",
                table: "MessageOutbox",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
