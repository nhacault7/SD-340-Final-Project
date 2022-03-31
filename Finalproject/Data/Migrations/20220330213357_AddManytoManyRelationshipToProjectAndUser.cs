using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finalproject.Data.Migrations
{
    public partial class AddManytoManyRelationshipToProjectAndUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_UserCreatorId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UserCreatorId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UserCreatorId",
                table: "Projects");

            migrationBuilder.CreateTable(
                name: "UserProject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProject_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProject_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProject_ProjectId",
                table: "UserProject",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProject_UserId",
                table: "UserProject",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProject");

            migrationBuilder.AddColumn<string>(
                name: "UserCreatorId",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserCreatorId",
                table: "Projects",
                column: "UserCreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_UserCreatorId",
                table: "Projects",
                column: "UserCreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
