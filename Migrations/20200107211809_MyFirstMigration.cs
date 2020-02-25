using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ToqueToqueApi.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Allergen",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergen", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "BookingState",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingState", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Conversation",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Difficulty",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Difficulty", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    text = table.Column<string>(nullable: true),
                    user_id = table.Column<int>(nullable: false),
                    sending_time = table.Column<DateTime>(nullable: false),
                    conversation_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Particularity",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Particularity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    title = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    price = table.Column<decimal>(nullable: false),
                    available_tickets = table.Column<int>(nullable: false),
                    creator = table.Column<int>(nullable: false),
                    event_starting = table.Column<DateTime>(nullable: false),
                    registering_stop = table.Column<DateTime>(nullable: false),
                    created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    email = table.Column<string>(nullable: true),
                    first_name = table.Column<string>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    phone = table.Column<string>(nullable: true),
                    birth_date = table.Column<DateTime>(nullable: false),
                    profile_picture = table.Column<string>(nullable: true),
                    password_hash = table.Column<byte[]>(nullable: true),
                    password_salt = table.Column<byte[]>(nullable: true),
                    is_enable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Meal",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    title = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    difficulty_id = table.Column<int>(nullable: true),
                    particularity_id = table.Column<int>(nullable: true),
                    pictures = table.Column<List<string>>(nullable: true),
                    realization_time = table.Column<TimeSpan>(nullable: false),
                    link_to_full_meal = table.Column<string>(nullable: true),
                    owner_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meal", x => x.id);
                    table.ForeignKey(
                        name: "FK_Meal_Difficulty_difficulty_id",
                        column: x => x.difficulty_id,
                        principalTable: "Difficulty",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Meal_Particularity_particularity_id",
                        column: x => x.particularity_id,
                        principalTable: "Particularity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Geolocation",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    session_id = table.Column<int>(nullable: false),
                    latitude = table.Column<double>(nullable: false),
                    longitude = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geolocation", x => x.id);
                    table.ForeignKey(
                        name: "FK_Geolocation_Session_session_id",
                        column: x => x.session_id,
                        principalTable: "Session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "allergen_user_db",
                columns: table => new
                {
                    allergen_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_allergen_user_db", x => new { x.allergen_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_allergen_user_db_Allergen_allergen_id",
                        column: x => x.allergen_id,
                        principalTable: "Allergen",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_allergen_user_db_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "booking_state_session_user_db",
                columns: table => new
                {
                    session_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false),
                    booking_state_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_state_session_user_db", x => new { x.session_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_booking_state_session_user_db_BookingState_booking_state_id",
                        column: x => x.booking_state_id,
                        principalTable: "BookingState",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_booking_state_session_user_db_Session_session_id",
                        column: x => x.session_id,
                        principalTable: "Session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_booking_state_session_user_db_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "conversation_user_db",
                columns: table => new
                {
                    conversation_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversation_user_db", x => new { x.conversation_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_conversation_user_db_Conversation_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "Conversation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_conversation_user_db_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "allergen_meal_db",
                columns: table => new
                {
                    allergen_id = table.Column<int>(nullable: false),
                    meal_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_allergen_meal_db", x => new { x.allergen_id, x.meal_id });
                    table.ForeignKey(
                        name: "FK_allergen_meal_db_Allergen_allergen_id",
                        column: x => x.allergen_id,
                        principalTable: "Allergen",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_allergen_meal_db_Meal_meal_id",
                        column: x => x.meal_id,
                        principalTable: "Meal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session_meal_db",
                columns: table => new
                {
                    session_id = table.Column<int>(nullable: false),
                    meal_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session_meal_db", x => new { x.session_id, x.meal_id });
                    table.ForeignKey(
                        name: "FK_session_meal_db_Meal_meal_id",
                        column: x => x.meal_id,
                        principalTable: "Meal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_session_meal_db_Session_session_id",
                        column: x => x.session_id,
                        principalTable: "Session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_allergen_meal_db_meal_id",
                table: "allergen_meal_db",
                column: "meal_id");

            migrationBuilder.CreateIndex(
                name: "IX_allergen_user_db_user_id",
                table: "allergen_user_db",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_state_session_user_db_booking_state_id",
                table: "booking_state_session_user_db",
                column: "booking_state_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_state_session_user_db_user_id",
                table: "booking_state_session_user_db",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_conversation_user_db_user_id",
                table: "conversation_user_db",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Geolocation_session_id",
                table: "Geolocation",
                column: "session_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meal_difficulty_id",
                table: "Meal",
                column: "difficulty_id");

            migrationBuilder.CreateIndex(
                name: "IX_Meal_particularity_id",
                table: "Meal",
                column: "particularity_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_meal_db_meal_id",
                table: "session_meal_db",
                column: "meal_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "allergen_meal_db");

            migrationBuilder.DropTable(
                name: "allergen_user_db");

            migrationBuilder.DropTable(
                name: "booking_state_session_user_db");

            migrationBuilder.DropTable(
                name: "conversation_user_db");

            migrationBuilder.DropTable(
                name: "Geolocation");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "session_meal_db");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Allergen");

            migrationBuilder.DropTable(
                name: "BookingState");

            migrationBuilder.DropTable(
                name: "Conversation");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Meal");

            migrationBuilder.DropTable(
                name: "Session");

            migrationBuilder.DropTable(
                name: "Difficulty");

            migrationBuilder.DropTable(
                name: "Particularity");
        }
    }
}
