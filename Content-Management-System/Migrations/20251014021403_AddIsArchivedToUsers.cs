using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddIsArchivedToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Users");
        }
    }
}
