using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_api.Migrations
{
    /// <inheritdoc />
    public partial class Update9MigrationTablesStatusEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StatusEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_StatusId",
                table: "Events",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusEvents_Type",
                table: "StatusEvents",
                column: "Type",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_StatusEvents_StatusId",
                table: "Events",
                column: "StatusId",
                principalTable: "StatusEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_StatusEvents_StatusId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "StatusEvents");

            migrationBuilder.DropIndex(
                name: "IX_Events_StatusId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Events");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
