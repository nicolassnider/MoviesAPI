using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesAPI.Migrations
{
    /// <inheritdoc />
    public partial class allUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "07cf57c9-4ace-4454-80d5-fa17a61ce614", "88e81bf5-425b-4ab8-a046-04aff071a854", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f2de2c4e-87fb-40ef-bd50-7caae593fb43", 0, "6b173b0a-1a76-4719-b81d-9a9a0d3b57e7", "nicolas@snider.com", false, false, null, "nicolas@snider.com", "nicolas@snider.com", "AQAAAAEAACcQAAAAEIoRV9MjMVvtUwM68+qinW8O4HrzDhD8OOCP+cPGFiA0+ixEZkw4LW40eXPB/50EAw==", null, false, "b6f85047-5e5f-4335-9c93-9ca5b12463d5", false, "nicolas@snider.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[] { 1, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin", "f2de2c4e-87fb-40ef-bd50-7caae593fb43" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "07cf57c9-4ace-4454-80d5-fa17a61ce614");

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f2de2c4e-87fb-40ef-bd50-7caae593fb43");
        }
    }
}
