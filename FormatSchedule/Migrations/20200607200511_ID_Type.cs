using Microsoft.EntityFrameworkCore.Migrations;

namespace FormatSchedule.Migrations
{
    public partial class ID_Type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "SportsEngineID",
                table: "Events",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "LeagueAthleticsID",
                table: "Events",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SportsEngineID",
                table: "Events",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "LeagueAthleticsID",
                table: "Events",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
