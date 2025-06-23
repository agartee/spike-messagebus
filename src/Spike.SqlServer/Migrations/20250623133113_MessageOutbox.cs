using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spike.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class MessageOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageOutbox",
                schema: "Spike",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Json = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CommitSequence = table.Column<int>(type: "int", nullable: false),
                    IsSending = table.Column<bool>(type: "bit", nullable: false),
                    SendAttemptCount = table.Column<int>(type: "int", nullable: true),
                    LastSendAttempt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageOutbox", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageOutbox",
                schema: "Spike");
        }
    }
}
