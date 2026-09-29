using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewPhanHoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NgayPhanHoi",
                table: "Reviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhanHoi",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgayPhanHoi",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "PhanHoi",
                table: "Reviews");
        }
    }
}
