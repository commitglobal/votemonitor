using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditTrailAndConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "ImportValidationErrors",
                newName: "CreatedOn");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "PollingStations",
                type: "character varying(2024)",
                maxLength: 2024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PollingStations",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "PollingStations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "PollingStations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "PollingStations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "PollingStations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PlatformAdmins",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Observers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AlterColumn<string>(
                name: "NativeName",
                table: "Language",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Language",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Language",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Language",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                table: "ImportValidationErrors",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ImportValidationErrors",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ImportValidationErrors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "ImportValidationErrors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "ImportValidationErrors",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ElectionRounds",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ElectionRounds",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ElectionRounds",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ElectionRounds",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "ElectionRounds",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "ElectionRounds",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CSOs",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CSOs",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "CSOs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "CSOs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "CSOs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "CSOs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CSOAdmins",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Countries",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Countries",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Countries",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Countries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AuditTrails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    TableName = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    AffectedColumns = table.Column<string>(type: "text", nullable: true),
                    PrimaryKey = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrails", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("008c3138-73d8-dbbc-f1dd-521e4c68bcf1"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("015a9f83-6e57-bc1e-8227-24a4e5248582"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("057884bc-3c2e-dea9-6522-b003c9297f7a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("067c9448-9ad0-2c21-a1dc-fbdf5a63d18d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("06f8ad57-7133-9a5e-5a83-53052012b014"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0797a7d5-bbc0-2e52-0de8-14a42fc80baa"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0868cdd3-7f50-5a25-88d6-98c45f9157e3"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("08a999e4-e420-b864-2864-bef78c138448"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0932ed88-c79f-591a-d684-9a77735f947e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("096a8586-9702-6fec-5f6a-6eb3b7b7837f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0a25f96f-5173-2fff-a2f8-c6872393edf6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0ab731f0-5326-44be-af3a-20aa33ad0f35"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0aebadaa-91b2-8794-c153-4f903a2a1004"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0b3b04b4-9782-79e3-bc55-9ab33b6ae9c7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0c0fef20-0e8d-98ea-7724-12cea9b3b926"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0d4fe6e6-ea1e-d1ce-5134-6c0c1a696a00"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0e0fefd5-9a05-fde5-bee9-ef56db7748a1"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0e2a1681-d852-67ae-7387-0d04be9e7fd3"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0f1ba59e-ade5-23e5-6fce-e2fd3282e114"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("10b58d9b-42ef-edb8-54a3-712636fda55a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("11765ad0-30f2-bab8-b616-20f88b28b21e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("11dbce82-a154-7aee-7b5e-d5981f220572"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1258ec90-c47e-ff72-b7e3-f90c3ee320f8"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("13c69e56-375d-8a7e-c326-be2be2fd4cd8"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("141e589a-7046-a265-d2f6-b2f85e6eeadd"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("14f190c6-97c9-3e12-2eba-db17c59d6a04"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("15639386-e4fc-120c-6916-c0c980e24be1"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("17ed5f0f-e091-94ff-0512-ad291bde94d7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1934954c-66c2-6226-c5b6-491065a3e4c0"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("19ea3a6a-1a76-23c8-8e4e-1d298f15207f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1b634ca2-2b90-7e54-715a-74cee7e4d294"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1d2aa3ab-e1c3-8c76-9be6-7a3b3eca35da"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1d974338-decf-08e5-3e62-89e1bbdbb003"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1e5c0dcc-83e9-f275-c81d-3bc49f88e70c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1f8be615-5746-277e-d82b-47596b5bb922"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2167da32-4f80-d31d-226c-0551970304eb"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("220e980a-7363-0150-c250-89e83b967fb4"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("29201cbb-ca65-1924-75a9-0c4d4db43001"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("294978f0-2702-d35d-cfc4-e676148aea2e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2a039b16-2adf-0fb8-3bdf-fbdf14358d9d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2a1ca5b6-fba0-cfa8-9928-d7a2382bc4d7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2a848549-9777-cf48-a0f2-b32c6f942096"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2b68fb11-a0e0-3d23-5fb8-99721ecfc182"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2bebebe4-edaa-9160-5a0c-4d99048bd8d5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2dc643bd-cc6c-eb0c-7314-44123576f0ee"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2e1bd9d8-df06-d773-0eb9-98e274b63b43"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2f00fe86-a06b-dc95-0ea7-4520d1dec784"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2f49855b-ff93-c399-d72a-121f2bf28bc9"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2f4cc994-53f1-1763-8220-5d89e063804f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("316c68fc-9144-f6e1-8bf1-899fc54b2327"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3175ac19-c801-0b87-8e66-7480a40dcf1e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3252e51a-5bc1-f065-7101-5b34ba493dc4"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("32da0208-9048-1339-a8ee-6955cfff4c12"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3345e205-3e72-43ed-de1b-ac6e050543e5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("357369e3-85a8-86f7-91c7-349772ae7744"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("357c121b-e28d-1765-e699-cc4ec5ff86fc"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("360e3c61-aaac-fa2f-d731-fc0824c05107"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("37a79267-d38a-aaef-577a-aa68a96880ae"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("37c89068-a8e9-87e8-d651-f86fac63673a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("39be5e86-aea5-f64f-fd7e-1017fe24e543"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3bcd2aad-fb69-09f4-1ad7-2c7f5fa23f9f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3c5828e0-16a8-79ba-4e5c-9b45065df113"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3ce3d958-7341-bd79-f294-f2e6907c186c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3e2cccbe-1615-c707-a97b-421a799b2559"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3eea06f4-c085-f619-6d52-b76a5f6fd2b6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3ffe68ca-7350-175b-4e95-0c34f54dc1f4"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("414a34ce-2781-8f96-2bd0-7ada86c8cf38"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("42697d56-52cf-b411-321e-c51929f02f90"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("44caa0f4-1e78-d2fb-96be-d01b3224bdc1"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("46576b73-c05b-7498-5b07-9bbf59b7645d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("46e88019-c521-57b2-d1c0-c0e2478d3b05"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("46ef1468-86f6-0c99-f4e9-46f966167b05"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4736c1ad-54bd-c8e8-d9ee-492a88268de8"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("47804b6a-e705-b925-f4fd-4adf6500180b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("478786f7-1842-8c1e-921c-12e7ed5329c5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4826bc0f-235e-572f-2b1a-21f1c9e05f83"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("49c82f1b-968d-b5e7-8559-e39567d46787"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4b0729b6-f698-5730-767c-88e2d36691bb"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4d8bcda4-5598-16cd-b379-97eb7a5e1c29"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4ee6400d-5534-7c67-1521-870d6b732366"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4fc1a9dc-cc74-f6ce-5743-c5cee8d709ef"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("500bb0de-61f5-dc9b-0488-1c507456ea4d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("50e5954d-7cb4-2201-b96c-f2a846ab3ae3"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("51aa4900-30a6-91b7-2728-071542a064ff"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("52538361-bbdf-fafb-e434-5655fc7451e5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5283afbb-2744-e930-2c16-c5ea6b0ff7cc"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("52d9992c-19bd-82b4-9188-11dabcac6171"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("538114de-7db0-9242-35e6-324fa7eff44d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5476986b-11a4-8463-9bd7-0f7354ec7a20"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("550ca5df-3995-617c-c39d-437beb400a42"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("57765d87-2424-2c86-ad9c-1af58ef3127a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("58337ef3-3d24-43e9-a440-832306e7fc07"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("592b4658-a210-ab0a-5660-3dcc673dc581"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5a5d9168-081b-1e02-1fbb-cdfa910e526c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5aa0aeb7-4dc8-6a29-fc2f-35daec1541dd"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5b0ee3be-596d-bdc1-f101-00ef33170655"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5be18efe-6db8-a727-7f2a-62bd71bc6593"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5c0e654b-8547-5d02-ee7b-d65e3c5c5273"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5cab34ca-8c74-0766-c7ca-4a826b44c5bd"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5e7a08f2-7d59-bcdb-7ddd-876b87181420"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("61ba1844-4d33-84b4-dbac-70718aa91d59"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("65d871be-4a1d-a632-9cdb-62e3ff04928d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6699efd5-0939-7812-315e-21f37b279ee9"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("687320c8-e841-c911-6d30-b14eb998feb6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("688af4c8-9d64-ae1c-147f-b8afd54801e3"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("695c85b3-a6c6-c217-9be8-3baebc7719ce"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6984f722-6963-d067-d4d4-9fd3ef2edbf6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6a76d068-49e1-da80-ddb4-9ef3d11191e6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6aac6f0e-d13a-a629-4c2b-9d6eaf6680e4"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6ac64a20-5688-ccd0-4eca-88d8a2560079"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6af4d03e-edd0-d98a-bc7e-abc7df87d3dd"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6c366974-3672-3a2c-2345-0fda33942304"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6c8be2e6-8c2e-cd80-68a6-d18c80d0eedc"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6d0c77a7-a4aa-c2bd-2db6-0e2ad2d61f8a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("704254eb-6959-8ddc-a5df-ac8f9658dc68"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("70673250-4cc3-3ba1-a42c-6b62ea8ab1d5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("72d8d1fe-d5f6-f440-1185-82ec69427027"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("7453c201-ecf1-d3dd-0409-e94d0733173b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("74da982f-cf20-e1b4-517b-a040511af23c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("75634729-8e4a-4cfd-739d-9f679bfca3ab"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("75e4464b-a784-63b8-1ecc-69ee1f09f43f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("766c1ebb-78c1-bada-37fb-c45d1bd4baff"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("77f6f69b-ec41-8818-9395-8d39bf09e653"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("7bbf15f4-a907-c0b2-7029-144aafb3c59d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("7bf4a786-3733-c670-e85f-03ee3caa6ef9"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("7bf934fa-bcf4-80b5-fd7d-ab4cca45c67b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("7ffa909b-8a6a-3028-9589-fcc3dfa530a8"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("802c05db-3866-545d-dc1a-a02c83ea6cf6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("809c3424-8654-b82c-cbd4-d857d096943e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("824392e8-a6cc-0cd4-af13-3067dad3258e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8250c49f-9438-7c2e-f403-54d962db0c18"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("84d58b3d-d131-1506-0792-1b3228b6f71f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("86db2170-be87-fd1d-bf57-05ff61ae83a7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("875060ca-73f6-af3b-d844-1b1416ce4583"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("881b4bb8-b6da-c73e-55c0-c9f31c02aaef"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("899c2a9f-f35d-5a49-a6cd-f92531bb2266"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8a4fcb23-f3e6-fb5b-8cda-975872f600d5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8b5a477a-070a-a84f-bd3b-f54dc2a172de"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8c4441fd-8cd4-ff1e-928e-e46f9ca12552"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8d32a12d-3230-1431-8fbb-72c789184345"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8e0de349-f9ab-2bca-3910-efd48bf1170a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8e787470-aae6-575a-fe0b-d65fc78b648a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8ed6a34e-8135-27fa-f86a-caa247b29768"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("903bee63-bcf0-0264-6eaf-a8cde95c5f41"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("914618fd-86f9-827a-91b8-826f0db9e02d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("914d7923-3ac5-75e8-c8e2-47d72561e35d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("915805f0-9ff0-48ff-39b3-44a4af5e0482"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("9205dbfc-60cd-91d9-b0b8-8a18a3755286"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("943d2419-2ca6-95f8-9c3b-ed445aea0371"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("95467997-f989-f456-34b7-0b578302dcba"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("96a22cee-9af7-8f03-b483-b3e774a36d3b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("971c7e66-c6e3-71f4-580a-5caf2852f9f4"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("976e496f-ca38-d113-1697-8af2d9a3b159"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("97cd39d5-1aca-8f10-9f5e-3f611d7606d8"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("980176e8-7d9d-9729-b3e9-ebc455fb8fc4"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("9ae7ad80-9ce7-6657-75cf-28b4c0254238"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("9d4ec95b-974a-f5bb-bb4b-ba6747440631"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("9d6e6446-185e-235e-8771-9eb2d19f22e7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("9dacf00b-7d0a-d744-cc60-e5fa66371e9d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("9e7dbdc3-2c8b-e8ae-082b-e02695f8268e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a0098040-b7a0-59a1-e64b-0a9778b7f74c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a16263a5-810c-bf6a-206d-72cb914e2d5c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a1b83be0-6a9b-c8a9-2cce-531705a29664"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a2da72dc-5866-ba2f-6283-6575af00ade5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a32a9fc2-677f-43e0-97aa-9e83943d785c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a40b91b3-cc13-2470-65f0-a0fdc946f2a2"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a5d0c9af-2022-2b43-9332-eb6a2ce4305d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a7716d29-6ef6-b775-51c5-97094536329d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a7afb7b1-b26d-4571-1a1f-3fff738ff21e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a7c4c9db-8fe4-7d43-e830-1a70954970c3"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a8f30b36-4a25-3fb9-c69e-84ce6640d785"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a96fe9bb-4ef4-fca0-f38b-0ec729822f37"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a9940e91-93ef-19f7-79c0-00d31c6a9f87"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a9949ac7-8d2d-32b5-3f4f-e2a3ef291a67"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a9a5f440-a9bd-487d-e7f4-914df0d52fa6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("aa0f69b2-93aa-ec51-b43b-60145db79e38"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ab0b7e83-bf02-16e6-e5ae-46c4bd4c093b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ac6cde6e-f645-d04e-8afc-0391ecf38a70"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ad4f938a-bf7b-684b-2c9e-e824d3fa3863"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("af79558d-51fb-b08d-185b-afeb983ab99b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b0f4bdfa-17dd-9714-4fe8-3c3b1f010ffa"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b2261c50-1a57-7f1f-d72d-f8c21593874f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b2c4d2d7-7ada-7864-426f-10a28d9f9eba"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b32fe2b5-a06e-0d76-ffd2-f186c3e64b15"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b3460bab-2a35-57bc-17e2-4e117748bbb1"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b4e0625c-7597-c185-b8ae-cfb35a731f2f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b6f70436-9515-7ef8-af57-aad196503499"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b723594d-7800-0f37-db86-0f6b85bb6cf9"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b86375dc-edbb-922c-9ed4-2f724094a5a2"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b8b09512-ea4c-4a61-9331-304f55324ef7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("bd4bbfc7-d8bc-9d8d-7f7c-7b299c94e9e5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("bf210ee6-6c75-cf08-052e-5c3e608aed15"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c03d71a5-b215-8672-ec0c-dd8fe5c20e05"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c0b7e39e-223a-ebb0-b899-5404573bbdb7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c1a923f6-b9ec-78f7-cc1c-7025e3d69d7d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c4754c00-cfa5-aa6f-a9c8-a200457de7a8"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c522b3d3-74cc-846f-0394-737dff4d2b1a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c64288fc-d941-0615-47f9-28e6c294ce26"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c89e02a0-9506-90df-5545-b98a2453cd63"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c926f091-fe96-35b3-56b5-d418d17e0159"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c93bccaf-1835-3c02-e2ee-c113ced19e43"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c9702851-1f67-f2a6-89d4-37b3fbb12044"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c98174ef-8198-54ba-2ff1-b93f3c646db8"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ca2a5560-d4c4-3c87-3090-6f5436310b55"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("cb2e209b-d4c6-6d5c-8901-d989a9188783"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("cc7fabfc-4c2b-d9ff-bb45-003bfc2e468a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("cd0e8275-3def-1de4-8858-61aab36851c4"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("cd2c97c3-5473-0719-3803-fcacedfe2ea2"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("cfff3443-1378-9c7d-9d58-66146d7f29a6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d0e11a85-6623-69f5-bd95-3779dfeec297"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d13935c1-8956-1399-7c4e-0354795cd37b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d24b46ba-8e9d-2a09-7995-e35e8ae54f6b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d292ea2d-fbb6-7c1e-cb7d-23d552673776"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d525de3a-aecc-07de-0426-68f32af2968e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d6d31cdd-280a-56bc-24a4-a414028d2b67"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d7236157-d5a7-6b7a-3bc1-69802313fa30"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d8101f9d-8313-4054-c5f3-42c7a1c72862"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d97b5460-11ab-45c5-9a6f-ffa441ed70d6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("daf6bc7a-92c4-ef47-3111-e13199b86b90"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("db6ce903-ab43-3793-960c-659529bae6df"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("dcf19e1d-74a6-7b8b-a5ed-76b94a8ac2a7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("de503629-2607-b948-e279-0509d8109d0f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("df20d0d7-9fbe-e725-d966-4fdf9f5c9dfb"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e087f51c-feba-19b6-5595-fcbdce170411"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e0d562ca-f573-3c2f-eb83-f72d4d70d4fc"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e186a953-7ab3-c009-501c-a754267b770b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e1947bdc-ff2c-d2c1-3c55-f1f9bf778578"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e3bacefb-d79b-1569-a91c-43d7e4f6f230"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e6c7651f-182e-cf9c-1ef9-6293b95b500c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e75515a6-63cf-3612-a3a2-befa0d7048a7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e81c5db3-401a-e047-001e-045f39bef8ef"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ebf38b9a-6fbe-6e82-3977-2c4763bea072"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ed6278e0-436c-9fd9-0b9e-44fd424cbd1b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("edd4319b-86f3-24cb-248c-71da624c02f7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ee5dfc29-80f1-86ae-cde7-02484a18907a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ee926d09-799c-7c6a-2419-a6ff814b2c03"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f0219540-8b2c-bd29-4f76-b832de53a56f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f0965449-6b15-6c1a-f5cb-ebd2d575c02c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f33ced84-eb43-fb39-ef79-b266e4d4cd94"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f39cca22-449e-9866-3a65-465a5510483e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f3eef99a-661e-2c68-7a4c-3053e2f28007"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f5b15ea6-133d-c2c9-7ef9-b0916ea96edb"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f70ae426-f130-5637-0383-a5b63a06c500"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("fa633273-9866-840d-9739-c6c957901e46"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("fb9a713c-2de1-882a-64b7-0e8fef5d2f7e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("fbf4479d-d70d-c76e-b053-699362443a17"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("fc78fa89-b372-dcf7-7f1c-1e1bb14ecbe7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("fee6f04f-c4c1-e3e4-645d-bb6bb703aeb7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ff5b4d88-c179-ff0d-6285-cf46ba475d7d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("008c3138-73d8-dbbc-f1dd-521e4c68bcf1"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("06f8ad57-7133-9a5e-5a83-53052012b014"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("0797a7d5-bbc0-2e52-0de8-14a42fc80baa"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("081a5fdb-445a-015a-1e36-f2e5014265ae"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("0932ed88-c79f-591a-d684-9a77735f947e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("094b3769-68b1-6211-ba2d-6bba92d6a167"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("096a8586-9702-6fec-5f6a-6eb3b7b7837f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("0a25f96f-5173-2fff-a2f8-c6872393edf6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("0ab731f0-5326-44be-af3a-20aa33ad0f35"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("0b9b4368-7ceb-e519-153d-2c58c983852b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("0c0fef20-0e8d-98ea-7724-12cea9b3b926"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("0ce6f5e0-0789-fa0e-b4b5-23a5b1f5e257"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("0d4fe6e6-ea1e-d1ce-5134-6c0c1a696a00"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("0e2a1681-d852-67ae-7387-0d04be9e7fd3"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("11765ad0-30f2-bab8-b616-20f88b28b21e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("13016d0c-fbf0-9503-12f2-e0f8d27394ae"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("136610e1-8115-9cf1-d671-7950c6483495"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("17ed5f0f-e091-94ff-0512-ad291bde94d7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("1d974338-decf-08e5-3e62-89e1bbdbb003"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("1da84244-fa39-125e-06dc-3c0cb2342ce9"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("1e5c0dcc-83e9-f275-c81d-3bc49f88e70c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("1f8be615-5746-277e-d82b-47596b5bb922"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("2167da32-4f80-d31d-226c-0551970304eb"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("2299a74f-3ebc-f022-da1a-44ae59335b3b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("23785991-fef4-e625-4d3b-b6ac364d0fa0"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("285b9e82-38af-33ab-79fd-0b4f3fd4f2f1"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("29201cbb-ca65-1924-75a9-0c4d4db43001"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("294978f0-2702-d35d-cfc4-e676148aea2e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("2a039b16-2adf-0fb8-3bdf-fbdf14358d9d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("2b6d383a-9ab6-fcdf-bcfe-a4538faca407"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("2bebebe4-edaa-9160-5a0c-4d99048bd8d5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("2c7b808d-7786-2deb-5318-56f7c238520e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("2d013d34-b258-8fe9-ef52-dd34e82a4672"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("2dc643bd-cc6c-eb0c-7314-44123576f0ee"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("2e1bd9d8-df06-d773-0eb9-98e274b63b43"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("2e9cb133-68a7-2f3b-49d1-0921cf42dfae"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("2f00fe86-a06b-dc95-0ea7-4520d1dec784"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("3175ac19-c801-0b87-8e66-7480a40dcf1e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("3252e51a-5bc1-f065-7101-5b34ba493dc4"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("357369e3-85a8-86f7-91c7-349772ae7744"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("357c121b-e28d-1765-e699-cc4ec5ff86fc"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("37c89068-a8e9-87e8-d651-f86fac63673a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("3bf5a74a-6d12-e971-16bc-c75e487f2615"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("3c5828e0-16a8-79ba-4e5c-9b45065df113"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("3ce3d958-7341-bd79-f294-f2e6907c186c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("3e2cccbe-1615-c707-a97b-421a799b2559"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("3ffe68ca-7350-175b-4e95-0c34f54dc1f4"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("414a34ce-2781-8f96-2bd0-7ada86c8cf38"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("46576b73-c05b-7498-5b07-9bbf59b7645d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("46e88019-c521-57b2-d1c0-c0e2478d3b05"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("46ef1468-86f6-0c99-f4e9-46f966167b05"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("4826bc0f-235e-572f-2b1a-21f1c9e05f83"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("4a3aa5a4-473f-45cd-f054-fa0465c476a4"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("4d8bcda4-5598-16cd-b379-97eb7a5e1c29"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("4def223a-9524-596d-cc29-ab7830c590de"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("4ee6400d-5534-7c67-1521-870d6b732366"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("50e5954d-7cb4-2201-b96c-f2a846ab3ae3"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("51a86a09-0d0b-31c1-90f1-f237db8e29ad"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("51aa4900-30a6-91b7-2728-071542a064ff"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("52538361-bbdf-fafb-e434-5655fc7451e5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("5283afbb-2744-e930-2c16-c5ea6b0ff7cc"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("52d9992c-19bd-82b4-9188-11dabcac6171"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("538114de-7db0-9242-35e6-324fa7eff44d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("54686fcd-3f35-f468-7c9c-93217c5084bc"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("54726f17-03b8-8af3-0359-c42d8fe8459d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("57765d87-2424-2c86-ad9c-1af58ef3127a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("58337ef3-3d24-43e9-a440-832306e7fc07"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("596e8283-10ce-d81d-2e6f-400fa259d717"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("5a5d9168-081b-1e02-1fbb-cdfa910e526c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("5c0e654b-8547-5d02-ee7b-d65e3c5c5273"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("5e7a08f2-7d59-bcdb-7ddd-876b87181420"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("5f002f07-f2c3-9fa4-2e29-225d116c10a3"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("61ba1844-4d33-84b4-dbac-70718aa91d59"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("6200b376-9eae-d01b-de52-8674aaf5b013"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("629b68d8-1d71-d3ce-f13e-45048ffff017"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("67729f87-ef47-dd3f-65f7-b0f6df0d6384"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("6857242c-f772-38b5-b5a2-c8e8b9db551f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("688af4c8-9d64-ae1c-147f-b8afd54801e3"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("6aac6f0e-d13a-a629-4c2b-9d6eaf6680e4"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("6c366974-3672-3a2c-2345-0fda33942304"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("70673250-4cc3-3ba1-a42c-6b62ea8ab1d5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("720b4e12-b001-8d38-7c07-f43194b9645d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("7451108d-ad49-940a-d479-4d868b62a7c6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("74da982f-cf20-e1b4-517b-a040511af23c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("74f19a84-b1c5-fa2d-8818-2220b80a3056"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("75e4464b-a784-63b8-1ecc-69ee1f09f43f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("766c1ebb-78c1-bada-37fb-c45d1bd4baff"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("78b7020d-8b82-3fae-2049-30e490ae1faf"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("78c6e8af-fcb4-c783-987c-7e1aca3aed64"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("7a0725cf-311a-4f59-cff8-ad8b43dd226e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("7bbf15f4-a907-c0b2-7029-144aafb3c59d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("7bf4a786-3733-c670-e85f-03ee3caa6ef9"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("7bf934fa-bcf4-80b5-fd7d-ab4cca45c67b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("7f065da7-4ba4-81ca-5126-dbf606a73907"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("802c05db-3866-545d-dc1a-a02c83ea6cf6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("80b770b8-4797-3d62-ef66-1ded7b0da0e5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("80ecea2c-8969-1929-0d4a-39ed2324abc6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("849b5e66-dc68-a1ed-6ed3-e315fbd0a0e5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("84d58b3d-d131-1506-0792-1b3228b6f71f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("875060ca-73f6-af3b-d844-1b1416ce4583"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("87813ec7-4830-e4dc-5ab1-bd599057ede0"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("899392d7-d54f-a1c6-407a-1bada9b85fdd"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("8bc44f03-84a5-2afc-8b0b-40c727e4ce36"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("914618fd-86f9-827a-91b8-826f0db9e02d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("914d7923-3ac5-75e8-c8e2-47d72561e35d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("9204928b-c569-ef6a-446e-4853aee439b0"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("9205dbfc-60cd-91d9-b0b8-8a18a3755286"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("93fb8ace-4156-12d5-218e-64b7d35129b1"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("943d2419-2ca6-95f8-9c3b-ed445aea0371"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("95467997-f989-f456-34b7-0b578302dcba"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("976e496f-ca38-d113-1697-8af2d9a3b159"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("97cd39d5-1aca-8f10-9f5e-3f611d7606d8"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("9c11bb58-5135-453a-1d24-dc20ef0e9031"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("9d6e6446-185e-235e-8771-9eb2d19f22e7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("9dacf00b-7d0a-d744-cc60-e5fa66371e9d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("9e7dbdc3-2c8b-e8ae-082b-e02695f8268e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("9ec46cb5-6c2b-0e22-07c5-eb2fe1b8d2ff"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("a7716d29-6ef6-b775-51c5-97094536329d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("a7afb7b1-b26d-4571-1a1f-3fff738ff21e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("a8f30b36-4a25-3fb9-c69e-84ce6640d785"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("aa0f69b2-93aa-ec51-b43b-60145db79e38"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("b0f4bdfa-17dd-9714-4fe8-3c3b1f010ffa"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("b2261c50-1a57-7f1f-d72d-f8c21593874f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("b2a87091-32fb-ba34-a721-bf8b3de5935d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("b356a541-1383-3c0a-9afd-6aebae3753cb"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("b4292ad3-3ca8-eea5-f3e0-d1983db8f61e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("b6b2351f-4f1e-c92f-0e9a-a915f4cc5fa6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("b6f70436-9515-7ef8-af57-aad196503499"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("b8b09512-ea4c-4a61-9331-304f55324ef7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("b9da7f73-60cd-404c-18fb-1bc5bbfffb38"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("bd4f1638-6017-733d-f696-b8b4d72664d7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("c03d71a5-b215-8672-ec0c-dd8fe5c20e05"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("c2254fd9-159e-4064-0fbf-a7969cba06ec"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("c4754c00-cfa5-aa6f-a9c8-a200457de7a8"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("c522b3d3-74cc-846f-0394-737dff4d2b1a"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("c64288fc-d941-0615-47f9-28e6c294ce26"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("ca2a5560-d4c4-3c87-3090-6f5436310b55"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("ca44a869-d3b6-052d-1e1a-ad4e3682a2ed"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("ca6bfadf-4e87-0692-a6b3-20ea6a51555d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("caddae27-283a-82b2-9365-76a3d6c49eee"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("cd5689d6-7a06-73c7-650e-f6f94387fd88"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("cfff3443-1378-9c7d-9d58-66146d7f29a6"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("d13935c1-8956-1399-7c4e-0354795cd37b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("d292ea2d-fbb6-7c1e-cb7d-23d552673776"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("d4d5c45a-d3c2-891e-6d7d-75569c3386ac"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("d55a9eb2-48fc-2719-47bf-99e902c28e80"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("d5bffdfb-6a8e-6d9f-2e59-4ada912acdba"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("d685aa26-aee7-716b-9433-1b3411209f4b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("d6d31cdd-280a-56bc-24a4-a414028d2b67"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("d832c50a-112e-4591-9432-4ada24bc85b2"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("d8d4f63d-fa65-63dd-a788-de2eec3d24ec"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("d8ef067c-1087-4ff5-8e1f-2291df7ac958"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("dcf19e1d-74a6-7b8b-a5ed-76b94a8ac2a7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("de29d5e7-2ecf-a4ff-5e40-5e83edd0d9b4"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("de503629-2607-b948-e279-0509d8109d0f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("df20d0d7-9fbe-e725-d966-4fdf9f5c9dfb"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("df41c815-40f8-197a-7a8b-e456d43283d9"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("e1947bdc-ff2c-d2c1-3c55-f1f9bf778578"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("e3bacefb-d79b-1569-a91c-43d7e4f6f230"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("e43a2010-14fc-63a9-f9d3-0ab2a1d0e52f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("e7532b00-3b1b-ff2c-b7c0-26bd7e91af55"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("e75515a6-63cf-3612-a3a2-befa0d7048a7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("e9ad0bec-7dee-bd01-9528-1fc74d1d78dd"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("e9da8997-dee8-0c2d-79d3-05fafc45092e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("eace47f6-5499-f4f0-8f97-ed165b681d84"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("ebf38b9a-6fbe-6e82-3977-2c4763bea072"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("ed6278e0-436c-9fd9-0b9e-44fd424cbd1b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("edd4319b-86f3-24cb-248c-71da624c02f7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("ee1ace14-e945-4767-85ec-3d74be8b516b"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("ef584e3c-03f2-42b0-7139-69d15d21e5a8"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("f0219540-8b2c-bd29-4f76-b832de53a56f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("f0965449-6b15-6c1a-f5cb-ebd2d575c02c"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("f1f09549-a9bb-da4a-9b98-8655a01235aa"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("f21f562e-5c35-4806-4efc-416619b5b7f7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("f33ced84-eb43-fb39-ef79-b266e4d4cd94"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("f39cca22-449e-9866-3a65-465a5510483e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("f5b15ea6-133d-c2c9-7ef9-b0916ea96edb"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("fa633273-9866-840d-9739-c6c957901e46"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("fb1cce84-4a6c-1834-0ff2-6df002e3d56f"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("fb429393-f994-0a16-37f9-edc0510fced5"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("fb9a713c-2de1-882a-64b7-0e8fef5d2f7e"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("fee6f04f-c4c1-e3e4-645d-bb6bb703aeb7"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("ff5b4d88-c179-ff0d-6285-cf46ba475d7d"),
                column: "CreatedOn",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Language_Id",
                table: "Language",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Language_Iso1",
                table: "Language",
                column: "Iso1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportValidationErrors_Id",
                table: "ImportValidationErrors",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso2",
                table: "Countries",
                column: "Iso2",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso3",
                table: "Countries",
                column: "Iso3",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_NumericCode",
                table: "Countries",
                column: "NumericCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrails");

            migrationBuilder.DropIndex(
                name: "IX_Users_Login",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Language_Id",
                table: "Language");

            migrationBuilder.DropIndex(
                name: "IX_Language_Iso1",
                table: "Language");

            migrationBuilder.DropIndex(
                name: "IX_ImportValidationErrors_Id",
                table: "ImportValidationErrors");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Iso2",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Iso3",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_NumericCode",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "PollingStations");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Language");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ImportValidationErrors");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ImportValidationErrors");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "ImportValidationErrors");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ElectionRounds");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ElectionRounds");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ElectionRounds");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "ElectionRounds");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CSOs");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "CSOs");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "CSOs");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "CSOs");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Countries");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "ImportValidationErrors",
                newName: "Date");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "PollingStations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2024)",
                oldMaxLength: 2024);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PollingStations",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PlatformAdmins",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Observers",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "NativeName",
                table: "Language",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Language",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Language",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                table: "ImportValidationErrors",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ImportValidationErrors",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ElectionRounds",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ElectionRounds",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CSOs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CSOs",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CSOAdmins",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Countries",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Countries",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Countries",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
