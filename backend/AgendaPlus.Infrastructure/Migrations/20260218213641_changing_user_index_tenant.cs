using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changing_user_index_tenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_users_tenant_email",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "idx_users_tenant_email",
                table: "users",
                columns: new[] { "tenant_id", "email" },
                unique: true,
                filter: "tenant_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_users_tenant_email",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "idx_users_tenant_email",
                table: "users",
                columns: new[] { "tenant_id", "email" },
                unique: true);
        }
    }
}
