using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlateformeRHCollaborative.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsToLeavesAndTeleworks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Teleworks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedById",
                table: "Teleworks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Leaves",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedById",
                table: "Leaves",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Teleworks");

            migrationBuilder.DropColumn(
                name: "RejectedById",
                table: "Teleworks");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "RejectedById",
                table: "Leaves");
        }
    }
}
