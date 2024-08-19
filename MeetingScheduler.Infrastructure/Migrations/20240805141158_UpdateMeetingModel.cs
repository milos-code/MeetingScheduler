using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMeetingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DelayOrCancelationNote",
                table: "Meetings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DelayOrCancelationNote",
                table: "Meetings");
        }
    }
}
