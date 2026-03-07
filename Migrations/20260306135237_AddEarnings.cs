using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pharmacareAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEarnings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Earnings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodType = table.Column<int>(type: "int", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Investment = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Earnings = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Profit = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Earnings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Earnings_PeriodType_PeriodStart_PeriodEnd",
                table: "Earnings",
                columns: new[] { "PeriodType", "PeriodStart", "PeriodEnd" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Earnings");
        }
    }
}
