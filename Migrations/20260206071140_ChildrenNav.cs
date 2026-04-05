using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fashion.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChildrenNav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEditorial",
                table: "Drops",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Drops",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subtitle",
                table: "Drops",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drops_ParentId",
                table: "Drops",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drops_Drops_ParentId",
                table: "Drops",
                column: "ParentId",
                principalTable: "Drops",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drops_Drops_ParentId",
                table: "Drops");

            migrationBuilder.DropIndex(
                name: "IX_Drops_ParentId",
                table: "Drops");

            migrationBuilder.DropColumn(
                name: "IsEditorial",
                table: "Drops");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Drops");

            migrationBuilder.DropColumn(
                name: "Subtitle",
                table: "Drops");
        }
    }
}
