using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finalproject.Data.Migrations
{
    public partial class customFieldSalary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "DailySalary",
                table: "AspNetUsers",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailySalary",
                table: "AspNetUsers");
        }
    }
}
