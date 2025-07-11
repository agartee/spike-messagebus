﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spike.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class MessageBoxTweaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SendAttemptCount",
                schema: "Spike",
                table: "MessageOutbox",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SendAttemptCount",
                schema: "Spike",
                table: "MessageOutbox",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
