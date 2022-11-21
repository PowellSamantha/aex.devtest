using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aex.devtest.infrastucture.Migrations.ApplicationDb
{
    public partial class ApplicationDb_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "entity");

            migrationBuilder.CreateTable(
                name: "vehicles",
                schema: "entity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "varchar(50)", nullable: false),
                    Make = table.Column<string>(type: "varchar(255)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    WheelCount = table.Column<byte>(type: "tinyint", nullable: false),
                    FuelType = table.Column<string>(type: "varchar(50)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    AnnualTaxableLevy = table.Column<float>(type: "real", nullable: false),
                    RoadworthyTestInterval = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_Type_Make_Model",
                schema: "entity",
                table: "vehicles",
                columns: new[] { "Type", "Make", "Model" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vehicles",
                schema: "entity");
        }
    }
}
