using Microsoft.EntityFrameworkCore.Migrations;

namespace OrionBlog.Data.Migrations
{
    public partial class _013 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CommentBody",
                table: "CommentPost",
                type: "character varying(750)",
                maxLength: 750,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CommentBody",
                table: "CommentPost",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(750)",
                oldMaxLength: 750);
        }
    }
}
