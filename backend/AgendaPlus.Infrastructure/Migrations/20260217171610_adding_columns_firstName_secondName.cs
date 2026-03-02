using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adding_columns_firstName_secondName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "full_name",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "users",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "second_name",
                table: "users",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "first_name",
                table: "users");

            migrationBuilder.DropColumn(
                name: "second_name",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "full_name",
                table: "users",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");
        }
    }
}
