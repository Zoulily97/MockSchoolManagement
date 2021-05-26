using Microsoft.EntityFrameworkCore.Migrations;

namespace MockSchoolManagement.Migrations
{
    public partial class seedstudentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Email", "Major", "Name" },
                values: new object[] { 2, "1178.com", 1, "zouli" });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Email", "Major", "Name" },
                values: new object[] { 3, "1178lisi.com", 2, "lisi" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
