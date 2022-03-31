using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finalproject.Data.Migrations
{
    public partial class AddUserRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserCreatorId",
                table: "Tasks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreatorId",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreatorId",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreatorId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserCreatorId",
                table: "Tasks",
                column: "UserCreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserCreatorId",
                table: "Projects",
                column: "UserCreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserCreatorId",
                table: "Notifications",
                column: "UserCreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserCreatorId",
                table: "Comments",
                column: "UserCreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserCreatorId",
                table: "Comments",
                column: "UserCreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserCreatorId",
                table: "Notifications",
                column: "UserCreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_UserCreatorId",
                table: "Projects",
                column: "UserCreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_UserCreatorId",
                table: "Tasks",
                column: "UserCreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserCreatorId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserCreatorId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_UserCreatorId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_UserCreatorId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserCreatorId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UserCreatorId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserCreatorId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UserCreatorId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UserCreatorId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UserCreatorId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UserCreatorId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserCreatorId",
                table: "Comments");
        }
    }
}
