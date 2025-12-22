using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlateformeRHCollaborative.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixEmployeeLeaveBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Employees SET SoldeConges = 25");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
