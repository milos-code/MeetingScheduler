using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MettingNotesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MeetingNote",
                table: "MeetingNotes",
                newName: "Note");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingNotes_MeetingId",
                table: "MeetingNotes",
                column: "MeetingId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingNotes_Meetings_MeetingId",
                table: "MeetingNotes",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeetingNotes_Meetings_MeetingId",
                table: "MeetingNotes");

            migrationBuilder.DropIndex(
                name: "IX_MeetingNotes_MeetingId",
                table: "MeetingNotes");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "MeetingNotes",
                newName: "MeetingNote");
        }
    }
}
