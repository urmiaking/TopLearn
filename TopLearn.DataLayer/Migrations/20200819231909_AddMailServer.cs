﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace TopLearn.DataLayer.Migrations
{
    public partial class AddMailServer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailServers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerAddress = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Port = table.Column<int>(nullable: false),
                    Host = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailServers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MailServers",
                columns: new[] { "Id", "Host", "Password", "Port", "ServerAddress" },
                values: new object[] { 1, "smtp.gmail.com", "MASOUD7559", 587, "masoud.xpress@gmail.com" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailServers");
        }
    }
}
