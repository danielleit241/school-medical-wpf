using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolMedicalWpf.Dal.Migrations
{
    /// <inheritdoc />
    public partial class removeNotificiation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("2a9411a6-bc8d-4d87-8e80-d0e124726b49"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("4efb2b1f-70b4-4587-9cc4-de8f29793951"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("7222fcfc-d5e3-42e3-bcae-43e819b8ed2f"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("a7a1d982-e5a0-418b-a98d-981c929eda56"));

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserID", "Address", "AvatarUrl", "DayOfBirth", "EmailAddress", "FullName", "PasswordHash", "PhoneNumber", "RoleID", "Status" },
                values: new object[,]
                {
                    { new Guid("06cf676f-fdb0-45ad-ae12-57c76190108b"), null, null, null, "manager@system.local", "System Manager", "AQAAAAIAAYagAAAAEIe2uljFDw2/xBSEeVyD+my7rw2JEZviP2Hv9pgNmOv1mExS9JEoUB6JhIUl456raw==", "0900000002", 2, null },
                    { new Guid("595c4d63-98b8-4fc6-b433-98e6dbda761e"), null, null, null, "nurse@system.local", "System Nurse", "AQAAAAIAAYagAAAAEJVDOccSevTPD0SVJ9t2nmT6u1+e3xQzAQPyiQpqEAWhB9VZ98SbOV08x4KrNvdrZA==", "0900000003", 3, null },
                    { new Guid("7a5efebe-ace2-41fb-acff-e5f855ca4377"), null, null, null, "admin@system.local", "System Admin", "AQAAAAIAAYagAAAAEDgvTNhovRzZ3j+Y2eDxhvjLfLhD1Ub4Q4WmgHa2B6wMLP2VL1Z6kdeebpSQQNvwWA==", "0900000001", 1, null },
                    { new Guid("b0499e5f-7670-4c96-a72c-a60c65fd46e8"), null, null, null, "parent@system.local", "System Parent", "AQAAAAIAAYagAAAAEH50Qo+5lnj0iF5eqZNxQCg9sEtmEp9HjPpZyM3Hgl7tbSWYOj4KzA0hlPCcd2mJcg==", "0900000004", 4, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("06cf676f-fdb0-45ad-ae12-57c76190108b"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("595c4d63-98b8-4fc6-b433-98e6dbda761e"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("7a5efebe-ace2-41fb-acff-e5f855ca4377"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("b0499e5f-7670-4c96-a72c-a60c65fd46e8"));

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    SenderID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__20CF2E32D1A83A45", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK__Notificat__UserI__619B8048",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserID", "Address", "AvatarUrl", "DayOfBirth", "EmailAddress", "FullName", "PasswordHash", "PhoneNumber", "RoleID", "Status" },
                values: new object[,]
                {
                    { new Guid("2a9411a6-bc8d-4d87-8e80-d0e124726b49"), null, null, null, "nurse@system.local", "System Nurse", "AQAAAAIAAYagAAAAEN8BDiWU4MpZs43eQWoGXr5Q+iybxXxmj44DkOY0QOOrT2tqcxU8k1bPoFAOJwMYHw==", "0900000003", 3, null },
                    { new Guid("4efb2b1f-70b4-4587-9cc4-de8f29793951"), null, null, null, "admin@system.local", "System Admin", "AQAAAAIAAYagAAAAEN1BYdqnVPb7zRaKOGjp7qkwg8eVt/R132qDqqi+K6C+beWGfpW8eQxlNhjRq8Qykg==", "0900000001", 1, null },
                    { new Guid("7222fcfc-d5e3-42e3-bcae-43e819b8ed2f"), null, null, null, "parent@system.local", "System Parent", "AQAAAAIAAYagAAAAECK7usNnHQBcnT/L1Mpz1MeUMyjweoh+EB/HubLCJM8xZ1C8Hbm3o7sC1xgewNP/qA==", "0900000004", 4, null },
                    { new Guid("a7a1d982-e5a0-418b-a98d-981c929eda56"), null, null, null, "manager@system.local", "System Manager", "AQAAAAIAAYagAAAAECHnprDihQeUBqRnWCMBtkWvOBZsLqKfNgmqe7GGuEaSPF0fwErcYoxqMgf1rz9j0Q==", "0900000002", 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserID",
                table: "Notification",
                column: "UserID");
        }
    }
}
