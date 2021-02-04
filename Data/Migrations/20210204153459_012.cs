using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrionBlog.Data.Migrations
{
    public partial class _012 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "CategoryPost",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "CategoryPost",
                type: "bytea",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "CategoryPost");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "CategoryPost");
        }
    }
}
