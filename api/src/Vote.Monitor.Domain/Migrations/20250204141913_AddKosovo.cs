using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddKosovo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CreatedOn", "FullName", "Iso2", "Iso3", "Name", "NumericCode" },
                values: new object[] { new Guid("4b07d158-c1d0-8ab0-a28e-a56d64f910e1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Kosovo", "XK", "XKX", "Kosovo", "926" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4b07d158-c1d0-8ab0-a28e-a56d64f910e1"));
        }
    }
}
