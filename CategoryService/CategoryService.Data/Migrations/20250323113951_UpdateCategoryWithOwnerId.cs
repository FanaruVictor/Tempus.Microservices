using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CategoryService.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryWithOwnerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "category_type",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Categories",
                newName: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Categories",
                newName: "UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "category_type",
                table: "Categories",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");
        }
    }
}
