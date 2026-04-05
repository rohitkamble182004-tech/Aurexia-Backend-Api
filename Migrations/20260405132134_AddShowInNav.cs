using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fashion.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddShowInNav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<bool>(
        name: "ShowInNav",
        table: "Drops", // ⚠️ check your actual table name
        type: "boolean",
        nullable: false,
        defaultValue: false
    );
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(
        name: "ShowInNav",
        table: "Drops"
    );
}
    }
}
