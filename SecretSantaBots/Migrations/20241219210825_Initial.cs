using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecretSantaBots.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chatId = table.Column<long>(type: "bigint", nullable: false),
                    currency = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "participant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    gameId = table.Column<Guid>(type: "uuid", nullable: false),
                    telegramId = table.Column<long>(type: "bigint", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    assignedToId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_participant", x => x.id);
                    table.ForeignKey(
                        name: "FK_participant_game_gameId",
                        column: x => x.gameId,
                        principalTable: "game",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_participant_participant_assignedToId",
                        column: x => x.assignedToId,
                        principalTable: "participant",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_participant_assignedToId",
                table: "participant",
                column: "assignedToId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_participant_gameId",
                table: "participant",
                column: "gameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "participant");

            migrationBuilder.DropTable(
                name: "game");
        }
    }
}
