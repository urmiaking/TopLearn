using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TopLearn.DataLayer.Migrations
{
    public partial class SeedWalletData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ActivationCode", "Avatar", "Email", "IsActive", "Name", "Password", "RegisterDate" },
                values: new object[] { 1, "", "default-avatar.png", "masoud.brilliant@hotmail.com", true, "مسعود خدادادی", "db901737c41e490dec8bded913f112e5e7c720c3847558f0e5c65128bdb1b34c", new DateTime(2020, 8, 23, 1, 16, 33, 977, DateTimeKind.Local).AddTicks(5230) });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "Description", "IsPaid", "TransactionDate", "TransactionType", "UserId" },
                values: new object[] { 1, 25000, "شارژ حساب", true, new DateTime(2020, 8, 23, 1, 16, 33, 981, DateTimeKind.Local).AddTicks(3585), 1, 1 });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "Description", "IsPaid", "TransactionDate", "TransactionType", "UserId" },
                values: new object[] { 2, 6000, "شارژ حساب", true, new DateTime(2020, 8, 23, 1, 16, 33, 981, DateTimeKind.Local).AddTicks(5756), 1, 1 });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "Description", "IsPaid", "TransactionDate", "TransactionType", "UserId" },
                values: new object[] { 3, 8000, "خرید آموزش", true, new DateTime(2020, 8, 23, 1, 16, 33, 981, DateTimeKind.Local).AddTicks(5804), 0, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
