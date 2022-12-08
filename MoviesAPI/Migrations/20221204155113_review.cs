using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesAPI.Migrations
{
    /// <inheritdoc />
    public partial class review : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rate = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "07cf57c9-4ace-4454-80d5-fa17a61ce614",
                column: "ConcurrencyStamp",
                value: "1276bd13-806d-45b7-b141-cb62b8b012dd");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f2de2c4e-87fb-40ef-bd50-7caae593fb43",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4c2ea7d4-6a98-4944-8a07-361d259f31e2", "AQAAAAEAACcQAAAAEMUwC8orw4fySgFZtR+V6AkAMUGMlEekNa0+NvcyOW6XxbA9GFmTdZ9nf1nwZnhwIw==", "15222fa5-be8a-4ffd-89f5-f882e90641af" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MovieId",
                table: "Reviews",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "07cf57c9-4ace-4454-80d5-fa17a61ce614",
                column: "ConcurrencyStamp",
                value: "88e81bf5-425b-4ab8-a046-04aff071a854");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f2de2c4e-87fb-40ef-bd50-7caae593fb43",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6b173b0a-1a76-4719-b81d-9a9a0d3b57e7", "AQAAAAEAACcQAAAAEIoRV9MjMVvtUwM68+qinW8O4HrzDhD8OOCP+cPGFiA0+ixEZkw4LW40eXPB/50EAw==", "b6f85047-5e5f-4335-9c93-9ca5b12463d5" });
        }
    }
}
