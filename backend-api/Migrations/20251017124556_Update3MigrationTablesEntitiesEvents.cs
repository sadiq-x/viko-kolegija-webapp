using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_api.Migrations
{
    /// <inheritdoc />
    public partial class Update3MigrationTablesEntitiesEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Entities_CreateById",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Auth",
                table: "Entities");

            migrationBuilder.AddColumn<int>(
                name: "TopicsId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Birthday",
                table: "Entities",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "Entities",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_TopicsId",
                table: "Events",
                column: "TopicsId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Type",
                table: "Topics",
                column: "Type",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Entities_CreateById",
                table: "Events",
                column: "CreateById",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Topics_TopicsId",
                table: "Events",
                column: "TopicsId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Entities_CreateById",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Topics_TopicsId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Events_TopicsId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TopicsId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Birthday",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "Entities");

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Auth",
                table: "Entities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Entities_CreateById",
                table: "Events",
                column: "CreateById",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
