using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlateformeRHCollaborative.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixSalaryBasePrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SalaryBase",
                table: "Employees",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryBase",
                table: "Employees");
        }
    }
}
