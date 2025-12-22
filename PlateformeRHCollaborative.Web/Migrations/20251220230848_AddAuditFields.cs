using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlateformeRHCollaborative.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Teleworks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedById",
                table: "Teleworks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Teleworks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledById",
                table: "Teleworks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Leaves",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedById",
                table: "Leaves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Leaves",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledById",
                table: "Leaves",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Teleworks");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "Teleworks");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Teleworks");

            migrationBuilder.DropColumn(
                name: "CancelledById",
                table: "Teleworks");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "CancelledById",
                table: "Leaves");


        }
    }
}
