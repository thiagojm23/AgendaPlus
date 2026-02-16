using System;
using System.Text.Json;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    occurred_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    processed_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true),
                    retry_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", nullable: false),
                    settings = table.Column<TenantSettings>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    time_zone = table.Column<string>(type: "varchar(50)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "availability_exceptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reason = table.Column<string>(type: "varchar(500)", nullable: true),
                    strategy = table.Column<short>(type: "smallint", nullable: false),
                    start_block_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_block_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    new_start_time = table.Column<TimeOnly>(type: "time", nullable: true),
                    new_end_time = table.Column<TimeOnly>(type: "time", nullable: true),
                    new_price_per_hour = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_availability_exceptions", x => x.id);
                    table.CheckConstraint("chk_strategy_logic", "(strategy = 0) OR (strategy = 1 AND new_start_time IS NOT NULL AND new_end_time IS NOT NULL) OR (strategy = 2 AND new_price_per_hour IS NOT NULL) OR (strategy = 3 AND new_start_time IS NOT NULL AND new_end_time IS NOT NULL AND new_price_per_hour IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_availability_exceptions_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resources",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "varchar(50)", nullable: false),
                    description = table.Column<string>(type: "varchar(500)", nullable: true),
                    resource_type = table.Column<int>(type: "integer", nullable: false),
                    open_days = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resources", x => x.id);
                    table.CheckConstraint("chk_openDays_positive", "open_days >= 0");
                    table.ForeignKey(
                        name: "fk_resources_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenants_adress",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    cep = table.Column<string>(type: "varchar(15)", nullable: false),
                    street = table.Column<string>(type: "varchar(200)", nullable: false),
                    number = table.Column<string>(type: "varchar(20)", nullable: true),
                    country_code_alpha2 = table.Column<string>(type: "char(2)", nullable: false, defaultValue: "BR"),
                    state_code = table.Column<string>(type: "varchar(5)", nullable: false),
                    city_name = table.Column<string>(type: "varchar(50)", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(10,8)", nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(11,8)", nullable: true),
                    google_place_id = table.Column<string>(type: "varchar(500)", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenants_adress", x => x.id);
                    table.ForeignKey(
                        name: "fk_tenants_adress_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    email = table.Column<string>(type: "varchar(100)", nullable: false),
                    password_hash = table.Column<string>(type: "varchar(255)", nullable: false),
                    full_name = table.Column<string>(type: "varchar(100)", nullable: false),
                    role = table.Column<short>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.CheckConstraint("chk_users_role", "role IN (0, 1, 2)");
                    table.ForeignKey(
                        name: "fk_users_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "availability_patterns",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    price_per_hour = table.Column<decimal>(type: "numeric(10,2)", nullable: false, defaultValue: 0m),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_availability_patterns", x => x.id);
                    table.CheckConstraint("chk_endTime_greater_startTime", "end_time > start_time");
                    table.ForeignKey(
                        name: "fk_availability_patterns_resources_resource_id",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_availability_patterns_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_booking_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_booking_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bookings", x => x.id);
                    table.CheckConstraint("chk_booking_time_logic", "end_booking_date_time > start_booking_date_time");
                    table.ForeignKey(
                        name: "fk_bookings_resources_resource_id",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_bookings_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "auth_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    refresh_token = table.Column<string>(type: "text", nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    failed_attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    password_reset_token = table.Column<string>(type: "text", nullable: true),
                    password_reset_token_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auth_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_auth_tokens_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_auth_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tenants",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_tenants", x => new { x.user_id, x.tenant_id });
                    table.ForeignKey(
                        name: "fk_user_tenants_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_tenants_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_auth_tokens_tenant_id",
                table: "auth_tokens",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_auth_tokens_user_id",
                table: "auth_tokens",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_availability_exceptions_tenant_id",
                table: "availability_exceptions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_availability_patterns_tenant",
                table: "availability_patterns",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_availability_patterns_resource_id",
                table: "availability_patterns",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "idx_bookings_tenant",
                table: "bookings",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_resource_id",
                table: "bookings",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "idx_resources_tenant",
                table: "resources",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_adress_cep",
                table: "tenants_adress",
                column: "cep",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenants_adress_google_place_id",
                table: "tenants_adress",
                column: "google_place_id");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_adress_latitude_longitude",
                table: "tenants_adress",
                columns: new[] { "latitude", "longitude" });

            migrationBuilder.CreateIndex(
                name: "ix_tenants_adress_state_code_city_name",
                table: "tenants_adress",
                columns: new[] { "state_code", "city_name" });

            migrationBuilder.CreateIndex(
                name: "ix_tenants_adress_tenant_id",
                table: "tenants_adress",
                column: "tenant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_tenants_tenant_id",
                table: "user_tenants",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_users_tenant_email",
                table: "users",
                columns: new[] { "tenant_id", "email" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auth_tokens");

            migrationBuilder.DropTable(
                name: "availability_exceptions");

            migrationBuilder.DropTable(
                name: "availability_patterns");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "tenants_adress");

            migrationBuilder.DropTable(
                name: "user_tenants");

            migrationBuilder.DropTable(
                name: "resources");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "tenants");
        }
    }
}
