using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TopLearn.DataLayer.Migrations
{
    public partial class AddPermissionData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ParentId", "Title" },
                values: new object[] { 1, null, "پنل مدیریت" });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1,
                column: "TransactionDate",
                value: new DateTime(2020, 10, 15, 2, 6, 40, 286, DateTimeKind.Local).AddTicks(2443));

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 2,
                column: "TransactionDate",
                value: new DateTime(2020, 10, 15, 2, 6, 40, 286, DateTimeKind.Local).AddTicks(4322));

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 3,
                column: "TransactionDate",
                value: new DateTime(2020, 10, 15, 2, 6, 40, 286, DateTimeKind.Local).AddTicks(4364));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 10, 15, 2, 6, 40, 281, DateTimeKind.Local).AddTicks(9182));

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ParentId", "Title" },
                values: new object[] { 2, 1, "مدیریت کاربران" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ParentId", "Title" },
                values: new object[] { 3, 2, "افزودن کاربر" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ParentId", "Title" },
                values: new object[] { 4, 2, "ویرایش کاربر" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1,
                column: "TransactionDate",
                value: new DateTime(2020, 10, 15, 1, 57, 24, 601, DateTimeKind.Local).AddTicks(6861));

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 2,
                column: "TransactionDate",
                value: new DateTime(2020, 10, 15, 1, 57, 24, 601, DateTimeKind.Local).AddTicks(8730));

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 3,
                column: "TransactionDate",
                value: new DateTime(2020, 10, 15, 1, 57, 24, 601, DateTimeKind.Local).AddTicks(8772));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegisterDate",
                value: new DateTime(2020, 10, 15, 1, 57, 24, 597, DateTimeKind.Local).AddTicks(6891));
        }
    }
}
