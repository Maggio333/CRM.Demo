using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Demo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeysForTaskAndNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Contacts_ContactId",
                table: "Notes",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Customers_CustomerId",
                table: "Notes",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Tasks_TaskId",
                table: "Notes",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Contacts_ContactId",
                table: "Tasks",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Customers_CustomerId",
                table: "Tasks",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Contacts_ContactId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Customers_CustomerId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Tasks_TaskId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Contacts_ContactId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Customers_CustomerId",
                table: "Tasks");
        }
    }
}
