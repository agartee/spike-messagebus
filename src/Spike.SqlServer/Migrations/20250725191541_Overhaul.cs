using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spike.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Overhaul : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSending",
                schema: "Spike",
                table: "MessageOutbox");

            migrationBuilder.RenameColumn(
                name: "LastSendAttempt",
                schema: "Spike",
                table: "MessageOutbox",
                newName: "LastDequeuedAt");

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

            migrationBuilder.AddColumn<Guid>(
                name: "CorrelationId",
                schema: "Spike",
                table: "MessageOutbox",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Spike",
                table: "MessageOutbox",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrelationId",
                schema: "Spike",
                table: "MessageOutbox");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Spike",
                table: "MessageOutbox");

            migrationBuilder.RenameColumn(
                name: "LastDequeuedAt",
                schema: "Spike",
                table: "MessageOutbox",
                newName: "LastSendAttempt");

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

            migrationBuilder.AddColumn<bool>(
                name: "IsSending",
                schema: "Spike",
                table: "MessageOutbox",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
