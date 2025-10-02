using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddResetPasswordLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResetPasswordLogs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdminID = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    ResetAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetPasswordLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ResetPasswordLogs_Users_AdminID",
                        column: x => x.AdminID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResetPasswordLogs_Users_TargetUserID",
                        column: x => x.TargetUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResetPasswordLogs_AdminID",
                table: "ResetPasswordLogs",
                column: "AdminID");

            migrationBuilder.CreateIndex(
                name: "IX_ResetPasswordLogs_TargetUserID",
                table: "ResetPasswordLogs",
                column: "TargetUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResetPasswordLogs");
        }
    }
}
