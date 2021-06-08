using Microsoft.EntityFrameworkCore.Migrations;

namespace cinemaApp.Api.Migrations
{
    public partial class addActorsMovie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActorName",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "ActorPicture",
                table: "Actors");

            migrationBuilder.AddColumn<int>(
                name: "MovieActorsId",
                table: "Actors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MovieActors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ActorPicture = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieActors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actors_MovieActorsId",
                table: "Actors",
                column: "MovieActorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actors_MovieActors_MovieActorsId",
                table: "Actors",
                column: "MovieActorsId",
                principalTable: "MovieActors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actors_MovieActors_MovieActorsId",
                table: "Actors");

            migrationBuilder.DropTable(
                name: "MovieActors");

            migrationBuilder.DropIndex(
                name: "IX_Actors_MovieActorsId",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "MovieActorsId",
                table: "Actors");

            migrationBuilder.AddColumn<string>(
                name: "ActorName",
                table: "Actors",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ActorPicture",
                table: "Actors",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
