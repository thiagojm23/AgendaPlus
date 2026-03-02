using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class big_changes_implementing_all_endpoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_bookings_resource_id",
                table: "bookings");

            migrationBuilder.AddColumn<string>(
                name: "business_name",
                table: "users",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "document",
                table: "users",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "users",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "status",
                table: "bookings",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "reservation_code",
                table: "bookings",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_bookings_reservation_code",
                table: "bookings",
                column: "reservation_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_bookings_resource_datetime",
                table: "bookings",
                columns: new[] { "resource_id", "start_booking_date_time", "end_booking_date_time" });

            migrationBuilder.CreateIndex(
                name: "ix_bookings_user_id",
                table: "bookings",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_users_user_id",
                table: "bookings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bookings_users_user_id",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "idx_bookings_reservation_code",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "idx_bookings_resource_datetime",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "ix_bookings_user_id",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "business_name",
                table: "users");

            migrationBuilder.DropColumn(
                name: "document",
                table: "users");

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "users");

            migrationBuilder.DropColumn(
                name: "reservation_code",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "bookings");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "bookings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_resource_id",
                table: "bookings",
                column: "resource_id");
        }
    }
}
