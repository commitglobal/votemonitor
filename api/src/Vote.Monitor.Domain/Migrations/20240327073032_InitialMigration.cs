using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

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

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Iso2 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Iso3 = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    NumericCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FormTemplateType = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Languages = table.Column<string>(type: "jsonb", nullable: false),
                    Questions = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportValidationErrors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImportType = table.Column<string>(type: "text", nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportValidationErrors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NativeName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Iso1 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ngos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ngos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Login = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Password = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElectionRounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EnglishTitle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionRounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectionRounds_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NgoAdmins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NgoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NgoAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NgoAdmins_Ngos_NgoId",
                        column: x => x.NgoId,
                        principalTable: "Ngos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NgoAdmins_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Observers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Observers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlatformAdmins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformAdmins_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.ApplicationUserId);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringNgos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    NgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringNgos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonitoringNgos_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonitoringNgos_Ngos_NgoId",
                        column: x => x.NgoId,
                        principalTable: "Ngos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollingStationInformationForms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    Languages = table.Column<string>(type: "jsonb", nullable: false),
                    Questions = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationInformationForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationInformationForms_ElectionRounds_ElectionRound~",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollingStations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "character varying(2024)", maxLength: 2024, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Tags = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStations_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Body = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_NgoAdmins_SenderId",
                        column: x => x.SenderId,
                        principalTable: "NgoAdmins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationTokens_Observers_ObserverId",
                        column: x => x.ObserverId,
                        principalTable: "Observers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringObservers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    InviterNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringObservers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonitoringObservers_MonitoringNgos_InviterNgoId",
                        column: x => x.InviterNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonitoringObservers_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MonitoringObservers_Observers_ObserverId",
                        column: x => x.ObserverId,
                        principalTable: "Observers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObserversGuides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UploadedFileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObserversGuides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObserversGuides_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringObserverNotification",
                columns: table => new
                {
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetedObserversId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringObserverNotification", x => new { x.NotificationId, x.TargetedObserversId });
                    table.ForeignKey(
                        name: "FK_MonitoringObserverNotification_MonitoringObservers_Targeted~",
                        column: x => x.TargetedObserversId,
                        principalTable: "MonitoringObservers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonitoringObserverNotification_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollingStationAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UploadedFileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationAttachments_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationAttachments_MonitoringObservers_MonitoringObs~",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObservers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationAttachments_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollingStationInformations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationInformationFormId = table.Column<Guid>(type: "uuid", nullable: false),
                    Answers = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationInformations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationInformations_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationInformations_MonitoringObservers_MonitoringOb~",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObservers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationInformations_PollingStationInformationForms_P~",
                        column: x => x.PollingStationInformationFormId,
                        principalTable: "PollingStationInformationForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationInformations_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollingStationNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    PollingStationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringObserverId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollingStationNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollingStationNotes_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationNotes_MonitoringObservers_MonitoringObserverId",
                        column: x => x.MonitoringObserverId,
                        principalTable: "MonitoringObservers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollingStationNotes_PollingStations_PollingStationId",
                        column: x => x.PollingStationId,
                        principalTable: "PollingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CreatedOn", "FullName", "Iso2", "Iso3", "Name", "NumericCode" },
                values: new object[,]
                {
                    { new Guid("008c3138-73d8-dbbc-f1dd-521e4c68bcf1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Azerbaijan", "AZ", "AZE", "Azerbaijan", "031" },
                    { new Guid("015a9f83-6e57-bc1e-8227-24a4e5248582"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of the Union of Myanmar", "MM", "MMR", "Myanmar", "104" },
                    { new Guid("057884bc-3c2e-dea9-6522-b003c9297f7a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Palau", "PW", "PLW", "Palau", "585" },
                    { new Guid("067c9448-9ad0-2c21-a1dc-fbdf5a63d18d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "State of Qatar", "QA", "QAT", "Qatar", "634" },
                    { new Guid("06f8ad57-7133-9a5e-5a83-53052012b014"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tunisian Republic", "TN", "TUN", "Tunisia", "788" },
                    { new Guid("0797a7d5-bbc0-2e52-0de8-14a42fc80baa"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Belgium", "BE", "BEL", "Belgium", "056" },
                    { new Guid("0868cdd3-7f50-5a25-88d6-98c45f9157e3"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United States Minor Outlying Islands", "UM", "UMI", "United States Minor Outlying Islands", "581" },
                    { new Guid("08a999e4-e420-b864-2864-bef78c138448"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mayotte", "YT", "MYT", "Mayotte", "175" },
                    { new Guid("0932ed88-c79f-591a-d684-9a77735f947e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kyrgyz Republic", "KG", "KGZ", "Kyrgyz Republic", "417" },
                    { new Guid("096a8586-9702-6fec-5f6a-6eb3b7b7837f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Guam", "GU", "GUM", "Guam", "316" },
                    { new Guid("0a25f96f-5173-2fff-a2f8-c6872393edf6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of San Marino", "SM", "SMR", "San Marino", "674" },
                    { new Guid("0ab731f0-5326-44be-af3a-20aa33ad0f35"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Sweden", "SE", "SWE", "Sweden", "752" },
                    { new Guid("0aebadaa-91b2-8794-c153-4f903a2a1004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Honduras", "HN", "HND", "Honduras", "340" },
                    { new Guid("0b3b04b4-9782-79e3-bc55-9ab33b6ae9c7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United Kingdom of Great Britain & Northern Ireland", "GB", "GBR", "United Kingdom of Great Britain and Northern Ireland", "826" },
                    { new Guid("0c0fef20-0e8d-98ea-7724-12cea9b3b926"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Namibia", "NA", "NAM", "Namibia", "516" },
                    { new Guid("0d4fe6e6-ea1e-d1ce-5134-6c0c1a696a00"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Faroe Islands", "FO", "FRO", "Faroe Islands", "234" },
                    { new Guid("0e0fefd5-9a05-fde5-bee9-ef56db7748a1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Turks and Caicos Islands", "TC", "TCA", "Turks and Caicos Islands", "796" },
                    { new Guid("0e2a1681-d852-67ae-7387-0d04be9e7fd3"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Fiji", "FJ", "FJI", "Fiji", "242" },
                    { new Guid("0f1ba59e-ade5-23e5-6fce-e2fd3282e114"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Christmas Island", "CX", "CXR", "Christmas Island", "162" },
                    { new Guid("10b58d9b-42ef-edb8-54a3-712636fda55a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Mozambique", "MZ", "MOZ", "Mozambique", "508" },
                    { new Guid("11765ad0-30f2-bab8-b616-20f88b28b21e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tokelau", "TK", "TKL", "Tokelau", "772" },
                    { new Guid("11dbce82-a154-7aee-7b5e-d5981f220572"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "French Polynesia", "PF", "PYF", "French Polynesia", "258" },
                    { new Guid("1258ec90-c47e-ff72-b7e3-f90c3ee320f8"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Democratic Republic of the Congo", "CD", "COD", "Congo", "180" },
                    { new Guid("13c69e56-375d-8a7e-c326-be2be2fd4cd8"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Japan", "JP", "JPN", "Japan", "392" },
                    { new Guid("141e589a-7046-a265-d2f6-b2f85e6eeadd"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sint Maarten (Dutch part)", "SX", "SXM", "Sint Maarten (Dutch part)", "534" },
                    { new Guid("14f190c6-97c9-3e12-2eba-db17c59d6a04"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Botswana", "BW", "BWA", "Botswana", "072" },
                    { new Guid("15639386-e4fc-120c-6916-c0c980e24be1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Commonwealth of Australia", "AU", "AUS", "Australia", "036" },
                    { new Guid("17ed5f0f-e091-94ff-0512-ad291bde94d7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Cabo Verde", "CV", "CPV", "Cabo Verde", "132" },
                    { new Guid("1934954c-66c2-6226-c5b6-491065a3e4c0"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of the Congo", "CG", "COG", "Congo", "178" },
                    { new Guid("19ea3a6a-1a76-23c8-8e4e-1d298f15207f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Commonwealth of Dominica", "DM", "DMA", "Dominica", "212" },
                    { new Guid("1b634ca2-2b90-7e54-715a-74cee7e4d294"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Mauritius", "MU", "MUS", "Mauritius", "480" },
                    { new Guid("1d2aa3ab-e1c3-8c76-9be6-7a3b3eca35da"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Maldives", "MV", "MDV", "Maldives", "462" },
                    { new Guid("1d974338-decf-08e5-3e62-89e1bbdbb003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Indonesia", "ID", "IDN", "Indonesia", "360" },
                    { new Guid("1e5c0dcc-83e9-f275-c81d-3bc49f88e70c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lebanese Republic", "LB", "LBN", "Lebanon", "422" },
                    { new Guid("1f8be615-5746-277e-d82b-47596b5bb922"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Croatia", "HR", "HRV", "Croatia", "191" },
                    { new Guid("2167da32-4f80-d31d-226c-0551970304eb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Seychelles", "SC", "SYC", "Seychelles", "690" },
                    { new Guid("220e980a-7363-0150-c250-89e83b967fb4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Saint Lucia", "LC", "LCA", "Saint Lucia", "662" },
                    { new Guid("29201cbb-ca65-1924-75a9-0c4d4db43001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United Arab Emirates", "AE", "ARE", "United Arab Emirates", "784" },
                    { new Guid("294978f0-2702-d35d-cfc4-e676148aea2e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ireland", "IE", "IRL", "Ireland", "372" },
                    { new Guid("2a039b16-2adf-0fb8-3bdf-fbdf14358d9d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Portuguese Republic", "PT", "PRT", "Portugal", "620" },
                    { new Guid("2a1ca5b6-fba0-cfa8-9928-d7a2382bc4d7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Chad", "TD", "TCD", "Chad", "148" },
                    { new Guid("2a848549-9777-cf48-a0f2-b32c6f942096"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Tajikistan", "TJ", "TJK", "Tajikistan", "762" },
                    { new Guid("2b68fb11-a0e0-3d23-5fb8-99721ecfc182"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Anguilla", "AI", "AIA", "Anguilla", "660" },
                    { new Guid("2bebebe4-edaa-9160-5a0c-4d99048bd8d5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Haiti", "HT", "HTI", "Haiti", "332" },
                    { new Guid("2dc643bd-cc6c-eb0c-7314-44123576f0ee"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Estonia", "EE", "EST", "Estonia", "233" },
                    { new Guid("2e1bd9d8-df06-d773-0eb9-98e274b63b43"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Federal Republic of Nigeria", "NG", "NGA", "Nigeria", "566" },
                    { new Guid("2f00fe86-a06b-dc95-0ea7-4520d1dec784"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Greenland", "GL", "GRL", "Greenland", "304" },
                    { new Guid("2f49855b-ff93-c399-d72a-121f2bf28bc9"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Saint Vincent and the Grenadines", "VC", "VCT", "Saint Vincent and the Grenadines", "670" },
                    { new Guid("2f4cc994-53f1-1763-8220-5d89e063804f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Western Sahara", "EH", "ESH", "Western Sahara", "732" },
                    { new Guid("316c68fc-9144-f6e1-8bf1-899fc54b2327"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Barbados", "BB", "BRB", "Barbados", "052" },
                    { new Guid("3175ac19-c801-0b87-8e66-7480a40dcf1e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Senegal", "SN", "SEN", "Senegal", "686" },
                    { new Guid("3252e51a-5bc1-f065-7101-5b34ba493dc4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Slovakia (Slovak Republic)", "SK", "SVK", "Slovakia (Slovak Republic)", "703" },
                    { new Guid("32da0208-9048-1339-a8ee-6955cfff4c12"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bouvet Island (Bouvetøya)", "BV", "BVT", "Bouvet Island (Bouvetøya)", "074" },
                    { new Guid("3345e205-3e72-43ed-de1b-ac6e050543e5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Curaçao", "CW", "CUW", "Curaçao", "531" },
                    { new Guid("357369e3-85a8-86f7-91c7-349772ae7744"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Uzbekistan", "UZ", "UZB", "Uzbekistan", "860" },
                    { new Guid("357c121b-e28d-1765-e699-cc4ec5ff86fc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Slovenia", "SI", "SVN", "Slovenia", "705" },
                    { new Guid("360e3c61-aaac-fa2f-d731-fc0824c05107"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "New Zealand", "NZ", "NZL", "New Zealand", "554" },
                    { new Guid("37a79267-d38a-aaef-577a-aa68a96880ae"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Djibouti", "DJ", "DJI", "Djibouti", "262" },
                    { new Guid("37c89068-a8e9-87e8-d651-f86fac63673a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Swiss Confederation", "CH", "CHE", "Switzerland", "756" },
                    { new Guid("39be5e86-aea5-f64f-fd7e-1017fe24e543"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "British Virgin Islands", "VG", "VGB", "British Virgin Islands", "092" },
                    { new Guid("3bcd2aad-fb69-09f4-1ad7-2c7f5fa23f9f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Guadeloupe", "GP", "GLP", "Guadeloupe", "312" },
                    { new Guid("3c5828e0-16a8-79ba-4e5c-9b45065df113"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cayman Islands", "KY", "CYM", "Cayman Islands", "136" },
                    { new Guid("3ce3d958-7341-bd79-f294-f2e6907c186c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Singapore", "SG", "SGP", "Singapore", "702" },
                    { new Guid("3e2cccbe-1615-c707-a97b-421a799b2559"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Uganda", "UG", "UGA", "Uganda", "800" },
                    { new Guid("3eea06f4-c085-f619-6d52-b76a5f6fd2b6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Niue", "NU", "NIU", "Niue", "570" },
                    { new Guid("3ffe68ca-7350-175b-4e95-0c34f54dc1f4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Guinea", "GN", "GIN", "Guinea", "324" },
                    { new Guid("414a34ce-2781-8f96-2bd0-7ada86c8cf38"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Spain", "ES", "ESP", "Spain", "724" },
                    { new Guid("42697d56-52cf-b411-321e-c51929f02f90"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Burkina Faso", "BF", "BFA", "Burkina Faso", "854" },
                    { new Guid("44caa0f4-1e78-d2fb-96be-d01b3224bdc1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Bahrain", "BH", "BHR", "Bahrain", "048" },
                    { new Guid("46576b73-c05b-7498-5b07-9bbf59b7645d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Bulgaria", "BG", "BGR", "Bulgaria", "100" },
                    { new Guid("46e88019-c521-57b2-d1c0-c0e2478d3b05"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Commonwealth of the Bahamas", "BS", "BHS", "Bahamas", "044" },
                    { new Guid("46ef1468-86f6-0c99-f4e9-46f966167b05"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Federal Republic of Germany", "DE", "DEU", "Germany", "276" },
                    { new Guid("4736c1ad-54bd-c8e8-d9ee-492a88268de8"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United Republic of Tanzania", "TZ", "TZA", "Tanzania", "834" },
                    { new Guid("47804b6a-e705-b925-f4fd-4adf6500180b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Norfolk Island", "NF", "NFK", "Norfolk Island", "574" },
                    { new Guid("478786f7-1842-8c1e-921c-12e7ed5329c5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Angola", "AO", "AGO", "Angola", "024" },
                    { new Guid("4826bc0f-235e-572f-2b1a-21f1c9e05f83"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gabonese Republic", "GA", "GAB", "Gabon", "266" },
                    { new Guid("49c82f1b-968d-b5e7-8559-e39567d46787"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Ecuador", "EC", "ECU", "Ecuador", "218" },
                    { new Guid("4b0729b6-f698-5730-767c-88e2d36691bb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "New Caledonia", "NC", "NCL", "New Caledonia", "540" },
                    { new Guid("4d8bcda4-5598-16cd-b379-97eb7a5e1c29"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of El Salvador", "SV", "SLV", "El Salvador", "222" },
                    { new Guid("4ee6400d-5534-7c67-1521-870d6b732366"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Iceland", "IS", "ISL", "Iceland", "352" },
                    { new Guid("4fc1a9dc-cc74-f6ce-5743-c5cee8d709ef"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hellenic Republic of Greece", "GR", "GRC", "Greece", "300" },
                    { new Guid("500bb0de-61f5-dc9b-0488-1c507456ea4d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hong Kong Special Administrative Region of China", "HK", "HKG", "Hong Kong", "344" },
                    { new Guid("50e5954d-7cb4-2201-b96c-f2a846ab3ae3"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Montserrat", "MS", "MSR", "Montserrat", "500" },
                    { new Guid("51aa4900-30a6-91b7-2728-071542a064ff"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Romania", "RO", "ROU", "Romania", "642" },
                    { new Guid("52538361-bbdf-fafb-e434-5655fc7451e5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Lithuania", "LT", "LTU", "Lithuania", "440" },
                    { new Guid("5283afbb-2744-e930-2c16-c5ea6b0ff7cc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Federative Republic of Brazil", "BR", "BRA", "Brazil", "076" },
                    { new Guid("52d9992c-19bd-82b4-9188-11dabcac6171"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bolivarian Republic of Venezuela", "VE", "VEN", "Venezuela", "862" },
                    { new Guid("538114de-7db0-9242-35e6-324fa7eff44d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "American Samoa", "AS", "ASM", "American Samoa", "016" },
                    { new Guid("5476986b-11a4-8463-9bd7-0f7354ec7a20"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Saint Pierre and Miquelon", "PM", "SPM", "Saint Pierre and Miquelon", "666" },
                    { new Guid("550ca5df-3995-617c-c39d-437beb400a42"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Turkmenistan", "TM", "TKM", "Turkmenistan", "795" },
                    { new Guid("57765d87-2424-2c86-ad9c-1af58ef3127a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Cuba", "CU", "CUB", "Cuba", "192" },
                    { new Guid("58337ef3-3d24-43e9-a440-832306e7fc07"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Russian Federation", "RU", "RUS", "Russian Federation", "643" },
                    { new Guid("592b4658-a210-ab0a-5660-3dcc673dc581"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Heard Island and McDonald Islands", "HM", "HMD", "Heard Island and McDonald Islands", "334" },
                    { new Guid("5a5d9168-081b-1e02-1fbb-cdfa910e526c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Finland", "FI", "FIN", "Finland", "246" },
                    { new Guid("5aa0aeb7-4dc8-6a29-fc2f-35daec1541dd"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Albania", "AL", "ALB", "Albania", "008" },
                    { new Guid("5b0ee3be-596d-bdc1-f101-00ef33170655"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bailiwick of Guernsey", "GG", "GGY", "Guernsey", "831" },
                    { new Guid("5be18efe-6db8-a727-7f2a-62bd71bc6593"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Cote d'Ivoire", "CI", "CIV", "Cote d'Ivoire", "384" },
                    { new Guid("5c0e654b-8547-5d02-ee7b-d65e3c5c5273"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Canada", "CA", "CAN", "Canada", "124" },
                    { new Guid("5cab34ca-8c74-0766-c7ca-4a826b44c5bd"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Principality of Monaco", "MC", "MCO", "Monaco", "492" },
                    { new Guid("5e7a08f2-7d59-bcdb-7ddd-876b87181420"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Union of the Comoros", "KM", "COM", "Comoros", "174" },
                    { new Guid("61ba1844-4d33-84b4-dbac-70718aa91d59"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Suriname", "SR", "SUR", "Suriname", "740" },
                    { new Guid("65d871be-4a1d-a632-9cdb-62e3ff04928d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bailiwick of Jersey", "JE", "JEY", "Jersey", "832" },
                    { new Guid("6699efd5-0939-7812-315e-21f37b279ee9"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Jamaica", "JM", "JAM", "Jamaica", "388" },
                    { new Guid("687320c8-e841-c911-6d30-b14eb998feb6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Democratic Socialist Republic of Sri Lanka", "LK", "LKA", "Sri Lanka", "144" },
                    { new Guid("688af4c8-9d64-ae1c-147f-b8afd54801e3"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Armenia", "AM", "ARM", "Armenia", "051" },
                    { new Guid("695c85b3-a6c6-c217-9be8-3baebc7719ce"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "State of Libya", "LY", "LBY", "Libya", "434" },
                    { new Guid("6984f722-6963-d067-d4d4-9fd3ef2edbf6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Zimbabwe", "ZW", "ZWE", "Zimbabwe", "716" },
                    { new Guid("6a76d068-49e1-da80-ddb4-9ef3d11191e6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Saint Helena, Ascension and Tristan da Cunha", "SH", "SHN", "Saint Helena, Ascension and Tristan da Cunha", "654" },
                    { new Guid("6aac6f0e-d13a-a629-4c2b-9d6eaf6680e4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of South Sudan", "SS", "SSD", "South Sudan", "728" },
                    { new Guid("6ac64a20-5688-ccd0-4eca-88d8a2560079"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Commonwealth of the Northern Mariana Islands", "MP", "MNP", "Northern Mariana Islands", "580" },
                    { new Guid("6af4d03e-edd0-d98a-bc7e-abc7df87d3dd"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "South Georgia and the South Sandwich Islands", "GS", "SGS", "South Georgia and the South Sandwich Islands", "239" },
                    { new Guid("6c366974-3672-3a2c-2345-0fda33942304"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sultanate of Oman", "OM", "OMN", "Oman", "512" },
                    { new Guid("6c8be2e6-8c2e-cd80-68a6-d18c80d0eedc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Iraq", "IQ", "IRQ", "Iraq", "368" },
                    { new Guid("6d0c77a7-a4aa-c2bd-2db6-0e2ad2d61f8a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Ghana", "GH", "GHA", "Ghana", "288" },
                    { new Guid("704254eb-6959-8ddc-a5df-ac8f9658dc68"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Austria", "AT", "AUT", "Austria", "040" },
                    { new Guid("70673250-4cc3-3ba1-a42c-6b62ea8ab1d5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Grand Duchy of Luxembourg", "LU", "LUX", "Luxembourg", "442" },
                    { new Guid("72d8d1fe-d5f6-f440-1185-82ec69427027"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of India", "IN", "IND", "India", "356" },
                    { new Guid("7453c201-ecf1-d3dd-0409-e94d0733173b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Solomon Islands", "SB", "SLB", "Solomon Islands", "090" },
                    { new Guid("74da982f-cf20-e1b4-517b-a040511af23c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Islamic Republic of Mauritania", "MR", "MRT", "Mauritania", "478" },
                    { new Guid("75634729-8e4a-4cfd-739d-9f679bfca3ab"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Peru", "PE", "PER", "Peru", "604" },
                    { new Guid("75e4464b-a784-63b8-1ecc-69ee1f09f43f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Burundi", "BI", "BDI", "Burundi", "108" },
                    { new Guid("766c1ebb-78c1-bada-37fb-c45d1bd4baff"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Democratic Republic of Sao Tome and Principe", "ST", "STP", "Sao Tome and Principe", "678" },
                    { new Guid("77f6f69b-ec41-8818-9395-8d39bf09e653"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Saint Barthélemy", "BL", "BLM", "Saint Barthélemy", "652" },
                    { new Guid("7bbf15f4-a907-c0b2-7029-144aafb3c59d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Italy", "IT", "ITA", "Italy", "380" },
                    { new Guid("7bf4a786-3733-c670-e85f-03ee3caa6ef9"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Panama", "PA", "PAN", "Panama", "591" },
                    { new Guid("7bf934fa-bcf4-80b5-fd7d-ab4cca45c67b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Korea", "KR", "KOR", "Korea", "410" },
                    { new Guid("7ffa909b-8a6a-3028-9589-fcc3dfa530a8"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "State of Israel", "IL", "ISR", "Israel", "376" },
                    { new Guid("802c05db-3866-545d-dc1a-a02c83ea6cf6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Federal Republic of Somalia", "SO", "SOM", "Somalia", "706" },
                    { new Guid("809c3424-8654-b82c-cbd4-d857d096943e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "People's Republic of Bangladesh", "BD", "BGD", "Bangladesh", "050" },
                    { new Guid("824392e8-a6cc-0cd4-af13-3067dad3258e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Equatorial Guinea", "GQ", "GNQ", "Equatorial Guinea", "226" },
                    { new Guid("8250c49f-9438-7c2e-f403-54d962db0c18"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "People's Republic of China", "CN", "CHN", "China", "156" },
                    { new Guid("84d58b3d-d131-1506-0792-1b3228b6f71f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Thailand", "TH", "THA", "Thailand", "764" },
                    { new Guid("86db2170-be87-fd1d-bf57-05ff61ae83a7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Montenegro", "ME", "MNE", "Montenegro", "499" },
                    { new Guid("875060ca-73f6-af3b-d844-1b1416ce4583"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Taiwan, Province of China", "TW", "TWN", "Taiwan", "158" },
                    { new Guid("881b4bb8-b6da-c73e-55c0-c9f31c02aaef"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Réunion", "RE", "REU", "Réunion", "638" },
                    { new Guid("899c2a9f-f35d-5a49-a6cd-f92531bb2266"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Saint Martin (French part)", "MF", "MAF", "Saint Martin", "663" },
                    { new Guid("8a4fcb23-f3e6-fb5b-8cda-975872f600d5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Denmark", "DK", "DNK", "Denmark", "208" },
                    { new Guid("8b5a477a-070a-a84f-bd3b-f54dc2a172de"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "State of Eritrea", "ER", "ERI", "Eritrea", "232" },
                    { new Guid("8c4441fd-8cd4-ff1e-928e-e46f9ca12552"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Yemen", "YE", "YEM", "Yemen", "887" },
                    { new Guid("8d32a12d-3230-1431-8fbb-72c789184345"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Macao Special Administrative Region of China", "MO", "MAC", "Macao", "446" },
                    { new Guid("8e0de349-f9ab-2bca-3910-efd48bf1170a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gibraltar", "GI", "GIB", "Gibraltar", "292" },
                    { new Guid("8e787470-aae6-575a-fe0b-d65fc78b648a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Eastern Republic of Uruguay", "UY", "URY", "Uruguay", "858" },
                    { new Guid("8ed6a34e-8135-27fa-f86a-caa247b29768"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Bhutan", "BT", "BTN", "Bhutan", "064" },
                    { new Guid("903bee63-bcf0-0264-6eaf-a8cde95c5f41"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "French Southern Territories", "TF", "ATF", "French Southern Territories", "260" },
                    { new Guid("914618fd-86f9-827a-91b8-826f0db9e02d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Kiribati", "KI", "KIR", "Kiribati", "296" },
                    { new Guid("914d7923-3ac5-75e8-c8e2-47d72561e35d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Norway", "NO", "NOR", "Norway", "578" },
                    { new Guid("915805f0-9ff0-48ff-39b3-44a4af5e0482"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Morocco", "MA", "MAR", "Morocco", "504" },
                    { new Guid("9205dbfc-60cd-91d9-b0b8-8a18a3755286"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Latvia", "LV", "LVA", "Latvia", "428" },
                    { new Guid("943d2419-2ca6-95f8-9c3b-ed445aea0371"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of the Marshall Islands", "MH", "MHL", "Marshall Islands", "584" },
                    { new Guid("95467997-f989-f456-34b7-0b578302dcba"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Trinidad and Tobago", "TT", "TTO", "Trinidad and Tobago", "780" },
                    { new Guid("96a22cee-9af7-8f03-b483-b3e774a36d3b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Benin", "BJ", "BEN", "Benin", "204" },
                    { new Guid("971c7e66-c6e3-71f4-580a-5caf2852f9f4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Serbia", "RS", "SRB", "Serbia", "688" },
                    { new Guid("976e496f-ca38-d113-1697-8af2d9a3b159"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Madagascar", "MG", "MDG", "Madagascar", "450" },
                    { new Guid("97cd39d5-1aca-8f10-9f5e-3f611d7606d8"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Niger", "NE", "NER", "Niger", "562" },
                    { new Guid("980176e8-7d9d-9729-b3e9-ebc455fb8fc4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Georgia", "GE", "GEO", "Georgia", "268" },
                    { new Guid("9ae7ad80-9ce7-6657-75cf-28b4c0254238"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hashemite Kingdom of Jordan", "JO", "JOR", "Jordan", "400" },
                    { new Guid("9d4ec95b-974a-f5bb-bb4b-ba6747440631"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Czech Republic", "CZ", "CZE", "Czechia", "203" },
                    { new Guid("9d6e6446-185e-235e-8771-9eb2d19f22e7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Principality of Liechtenstein", "LI", "LIE", "Liechtenstein", "438" },
                    { new Guid("9dacf00b-7d0a-d744-cc60-e5fa66371e9d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Togolese Republic", "TG", "TGO", "Togo", "768" },
                    { new Guid("9e7dbdc3-2c8b-e8ae-082b-e02695f8268e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Tonga", "TO", "TON", "Tonga", "776" },
                    { new Guid("a0098040-b7a0-59a1-e64b-0a9778b7f74c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Antarctica (the territory South of 60 deg S)", "AQ", "ATA", "Antarctica", "010" },
                    { new Guid("a16263a5-810c-bf6a-206d-72cb914e2d5c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cocos (Keeling) Islands", "CC", "CCK", "Cocos (Keeling) Islands", "166" },
                    { new Guid("a1b83be0-6a9b-c8a9-2cce-531705a29664"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isle of Man", "IM", "IMN", "Isle of Man", "833" },
                    { new Guid("a2da72dc-5866-ba2f-6283-6575af00ade5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Federated States of Micronesia", "FM", "FSM", "Micronesia", "583" },
                    { new Guid("a32a9fc2-677f-43e0-97aa-9e83943d785c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Eswatini", "SZ", "SWZ", "Eswatini", "748" },
                    { new Guid("a40b91b3-cc13-2470-65f0-a0fdc946f2a2"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of the Gambia", "GM", "GMB", "Gambia", "270" },
                    { new Guid("a5d0c9af-2022-2b43-9332-eb6a2ce4305d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pitcairn Islands", "PN", "PCN", "Pitcairn Islands", "612" },
                    { new Guid("a7716d29-6ef6-b775-51c5-97094536329d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bosnia and Herzegovina", "BA", "BIH", "Bosnia and Herzegovina", "070" },
                    { new Guid("a7afb7b1-b26d-4571-1a1f-3fff738ff21e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Argentine Republic", "AR", "ARG", "Argentina", "032" },
                    { new Guid("a7c4c9db-8fe4-7d43-e830-1a70954970c3"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Independent State of Samoa", "WS", "WSM", "Samoa", "882" },
                    { new Guid("a8f30b36-4a25-3fb9-c69e-84ce6640d785"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Saudi Arabia", "SA", "SAU", "Saudi Arabia", "682" },
                    { new Guid("a96fe9bb-4ef4-fca0-f38b-0ec729822f37"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Åland Islands", "AX", "ALA", "Åland Islands", "248" },
                    { new Guid("a9940e91-93ef-19f7-79c0-00d31c6a9f87"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United Mexican States", "MX", "MEX", "Mexico", "484" },
                    { new Guid("a9949ac7-8d2d-32b5-3f4f-e2a3ef291a67"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Co-operative Republic of Guyana", "GY", "GUY", "Guyana", "328" },
                    { new Guid("a9a5f440-a9bd-487d-e7f4-914df0d52fa6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Guinea-Bissau", "GW", "GNB", "Guinea-Bissau", "624" },
                    { new Guid("aa0f69b2-93aa-ec51-b43b-60145db79e38"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of North Macedonia", "MK", "MKD", "North Macedonia", "807" },
                    { new Guid("ab0b7e83-bf02-16e6-e5ae-46c4bd4c093b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Zambia", "ZM", "ZMB", "Zambia", "894" },
                    { new Guid("ac6cde6e-f645-d04e-8afc-0391ecf38a70"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "French Guiana", "GF", "GUF", "French Guiana", "254" },
                    { new Guid("ad4f938a-bf7b-684b-2c9e-e824d3fa3863"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Chile", "CL", "CHL", "Chile", "152" },
                    { new Guid("af79558d-51fb-b08d-185b-afeb983ab99b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cook Islands", "CK", "COK", "Cook Islands", "184" },
                    { new Guid("b0f4bdfa-17dd-9714-4fe8-3c3b1f010ffa"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Sierra Leone", "SL", "SLE", "Sierra Leone", "694" },
                    { new Guid("b2261c50-1a57-7f1f-d72d-f8c21593874f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "French Republic", "FR", "FRA", "France", "250" },
                    { new Guid("b2c4d2d7-7ada-7864-426f-10a28d9f9eba"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dominican Republic", "DO", "DOM", "Dominican Republic", "214" },
                    { new Guid("b32fe2b5-a06e-0d76-ffd2-f186c3e64b15"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Kenya", "KE", "KEN", "Kenya", "404" },
                    { new Guid("b3460bab-2a35-57bc-17e2-4e117748bbb1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Islamic Republic of Iran", "IR", "IRN", "Iran", "364" },
                    { new Guid("b4e0625c-7597-c185-b8ae-cfb35a731f2f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Central African Republic", "CF", "CAF", "Central African Republic", "140" },
                    { new Guid("b6f70436-9515-7ef8-af57-aad196503499"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "State of Kuwait", "KW", "KWT", "Kuwait", "414" },
                    { new Guid("b723594d-7800-0f37-db86-0f6b85bb6cf9"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Kazakhstan", "KZ", "KAZ", "Kazakhstan", "398" },
                    { new Guid("b86375dc-edbb-922c-9ed4-2f724094a5a2"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Falkland Islands (Malvinas)", "FK", "FLK", "Falkland Islands (Malvinas)", "238" },
                    { new Guid("b8b09512-ea4c-4a61-9331-304f55324ef7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "British Indian Ocean Territory (Chagos Archipelago)", "IO", "IOT", "British Indian Ocean Territory (Chagos Archipelago)", "086" },
                    { new Guid("bd4bbfc7-d8bc-9d8d-7f7c-7b299c94e9e5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Principality of Andorra", "AD", "AND", "Andorra", "020" },
                    { new Guid("bf210ee6-6c75-cf08-052e-5c3e608aed15"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Lesotho", "LS", "LSO", "Lesotho", "426" },
                    { new Guid("c03d71a5-b215-8672-ec0c-dd8fe5c20e05"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Mali", "ML", "MLI", "Mali", "466" },
                    { new Guid("c0b7e39e-223a-ebb0-b899-5404573bbdb7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Cameroon", "CM", "CMR", "Cameroon", "120" },
                    { new Guid("c1a923f6-b9ec-78f7-cc1c-7025e3d69d7d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Syrian Arab Republic", "SY", "SYR", "Syrian Arab Republic", "760" },
                    { new Guid("c4754c00-cfa5-aa6f-a9c8-a200457de7a8"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lao People's Democratic Republic", "LA", "LAO", "Lao People's Democratic Republic", "418" },
                    { new Guid("c522b3d3-74cc-846f-0394-737dff4d2b1a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mongolia", "MN", "MNG", "Mongolia", "496" },
                    { new Guid("c64288fc-d941-0615-47f9-28e6c294ce26"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Colombia", "CO", "COL", "Colombia", "170" },
                    { new Guid("c89e02a0-9506-90df-5545-b98a2453cd63"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Belize", "BZ", "BLZ", "Belize", "084" },
                    { new Guid("c926f091-fe96-35b3-56b5-d418d17e0159"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Independent State of Papua New Guinea", "PG", "PNG", "Papua New Guinea", "598" },
                    { new Guid("c93bccaf-1835-3c02-e2ee-c113ced19e43"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of the Philippines", "PH", "PHL", "Philippines", "608" },
                    { new Guid("c9702851-1f67-f2a6-89d4-37b3fbb12044"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of Cambodia", "KH", "KHM", "Cambodia", "116" },
                    { new Guid("c98174ef-8198-54ba-2ff1-b93f3c646db8"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Vanuatu", "VU", "VUT", "Vanuatu", "548" },
                    { new Guid("ca2a5560-d4c4-3c87-3090-6f5436310b55"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bermuda", "BM", "BMU", "Bermuda", "060" },
                    { new Guid("cb2e209b-d4c6-6d5c-8901-d989a9188783"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United States of America", "US", "USA", "United States of America", "840" },
                    { new Guid("cc7fabfc-4c2b-d9ff-bb45-003bfc2e468a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Islamic Republic of Pakistan", "PK", "PAK", "Pakistan", "586" },
                    { new Guid("cd0e8275-3def-1de4-8858-61aab36851c4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Nicaragua", "NI", "NIC", "Nicaragua", "558" },
                    { new Guid("cd2c97c3-5473-0719-3803-fcacedfe2ea2"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Commonwealth of Puerto Rico", "PR", "PRI", "Puerto Rico", "630" },
                    { new Guid("cfff3443-1378-9c7d-9d58-66146d7f29a6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kingdom of the Netherlands", "NL", "NLD", "Netherlands", "528" },
                    { new Guid("d0e11a85-6623-69f5-bd95-3779dfeec297"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Holy See (Vatican City State)", "VA", "VAT", "Holy See (Vatican City State)", "336" },
                    { new Guid("d13935c1-8956-1399-7c4e-0354795cd37b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Costa Rica", "CR", "CRI", "Costa Rica", "188" },
                    { new Guid("d24b46ba-8e9d-2a09-7995-e35e8ae54f6b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Guatemala", "GT", "GTM", "Guatemala", "320" },
                    { new Guid("d292ea2d-fbb6-7c1e-cb7d-23d552673776"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Malaysia", "MY", "MYS", "Malaysia", "458" },
                    { new Guid("d525de3a-aecc-07de-0426-68f32af2968e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Svalbard & Jan Mayen Islands", "SJ", "SJM", "Svalbard & Jan Mayen Islands", "744" },
                    { new Guid("d6d31cdd-280a-56bc-24a4-a414028d2b67"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "State of Palestine", "PS", "PSE", "Palestine", "275" },
                    { new Guid("d7236157-d5a7-6b7a-3bc1-69802313fa30"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Socialist Republic of Vietnam", "VN", "VNM", "Vietnam", "704" },
                    { new Guid("d8101f9d-8313-4054-c5f3-42c7a1c72862"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bonaire, Sint Eustatius and Saba", "BQ", "BES", "Bonaire, Sint Eustatius and Saba", "535" },
                    { new Guid("d97b5460-11ab-45c5-9a6f-ffa441ed70d6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Belarus", "BY", "BLR", "Belarus", "112" },
                    { new Guid("daf6bc7a-92c4-ef47-3111-e13199b86b90"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Moldova", "MD", "MDA", "Moldova", "498" },
                    { new Guid("db6ce903-ab43-3793-960c-659529bae6df"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Paraguay", "PY", "PRY", "Paraguay", "600" },
                    { new Guid("dcf19e1d-74a6-7b8b-a5ed-76b94a8ac2a7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hungary", "HU", "HUN", "Hungary", "348" },
                    { new Guid("de503629-2607-b948-e279-0509d8109d0f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Poland", "PL", "POL", "Poland", "616" },
                    { new Guid("df20d0d7-9fbe-e725-d966-4fdf9f5c9dfb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Cyprus", "CY", "CYP", "Cyprus", "196" },
                    { new Guid("e087f51c-feba-19b6-5595-fcbdce170411"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ukraine", "UA", "UKR", "Ukraine", "804" },
                    { new Guid("e0d562ca-f573-3c2f-eb83-f72d4d70d4fc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tuvalu", "TV", "TUV", "Tuvalu", "798" },
                    { new Guid("e186a953-7ab3-c009-501c-a754267b770b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Wallis and Futuna", "WF", "WLF", "Wallis and Futuna", "876" },
                    { new Guid("e1947bdc-ff2c-d2c1-3c55-f1f9bf778578"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "United States Virgin Islands", "VI", "VIR", "United States Virgin Islands", "850" },
                    { new Guid("e3bacefb-d79b-1569-a91c-43d7e4f6f230"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Nauru", "NR", "NRU", "Nauru", "520" },
                    { new Guid("e6c7651f-182e-cf9c-1ef9-6293b95b500c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Aruba", "AW", "ABW", "Aruba", "533" },
                    { new Guid("e75515a6-63cf-3612-a3a2-befa0d7048a7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Federal Democratic Republic of Ethiopia", "ET", "ETH", "Ethiopia", "231" },
                    { new Guid("e81c5db3-401a-e047-001e-045f39bef8ef"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nepal", "NP", "NPL", "Nepal", "524" },
                    { new Guid("ebf38b9a-6fbe-6e82-3977-2c4763bea072"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of South Africa", "ZA", "ZAF", "South Africa", "710" },
                    { new Guid("ed6278e0-436c-9fd9-0b9e-44fd424cbd1b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Brunei Darussalam", "BN", "BRN", "Brunei Darussalam", "096" },
                    { new Guid("edd4319b-86f3-24cb-248c-71da624c02f7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Islamic Republic of Afghanistan", "AF", "AFG", "Afghanistan", "004" },
                    { new Guid("ee5dfc29-80f1-86ae-cde7-02484a18907a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Arab Republic of Egypt", "EG", "EGY", "Egypt", "818" },
                    { new Guid("ee926d09-799c-7c6a-2419-a6ff814b2c03"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Liberia", "LR", "LBR", "Liberia", "430" },
                    { new Guid("f0219540-8b2c-bd29-4f76-b832de53a56f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Malta", "MT", "MLT", "Malta", "470" },
                    { new Guid("f0965449-6b15-6c1a-f5cb-ebd2d575c02c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Sudan", "SD", "SDN", "Sudan", "729" },
                    { new Guid("f33ced84-eb43-fb39-ef79-b266e4d4cd94"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Plurinational State of Bolivia", "BO", "BOL", "Bolivia", "068" },
                    { new Guid("f39cca22-449e-9866-3a65-465a5510483e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Türkiye", "TR", "TUR", "Türkiye", "792" },
                    { new Guid("f3eef99a-661e-2c68-7a4c-3053e2f28007"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Antigua and Barbuda", "AG", "ATG", "Antigua and Barbuda", "028" },
                    { new Guid("f5b15ea6-133d-c2c9-7ef9-b0916ea96edb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Rwanda", "RW", "RWA", "Rwanda", "646" },
                    { new Guid("f70ae426-f130-5637-0383-a5b63a06c500"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Democratic People's Republic of Korea", "KP", "PRK", "Korea", "408" },
                    { new Guid("fa633273-9866-840d-9739-c6c957901e46"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Federation of Saint Kitts and Nevis", "KN", "KNA", "Saint Kitts and Nevis", "659" },
                    { new Guid("fb9a713c-2de1-882a-64b7-0e8fef5d2f7e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Democratic Republic of Timor-Leste", "TL", "TLS", "Timor-Leste", "626" },
                    { new Guid("fbf4479d-d70d-c76e-b053-699362443a17"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Republic of Malawi", "MW", "MWI", "Malawi", "454" },
                    { new Guid("fc78fa89-b372-dcf7-7f1c-1e1bb14ecbe7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Martinique", "MQ", "MTQ", "Martinique", "474" },
                    { new Guid("fee6f04f-c4c1-e3e4-645d-bb6bb703aeb7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "People's Democratic Republic of Algeria", "DZ", "DZA", "Algeria", "012" },
                    { new Guid("ff5b4d88-c179-ff0d-6285-cf46ba475d7d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Grenada", "GD", "GRD", "Grenada", "308" }
                });

            migrationBuilder.InsertData(
                table: "Language",
                columns: new[] { "Id", "CreatedOn", "Iso1", "Name", "NativeName" },
                values: new object[,]
                {
                    { new Guid("008c3138-73d8-dbbc-f1dd-521e4c68bcf1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AZ", "Azerbaijani", "azərbaycan dili" },
                    { new Guid("06f8ad57-7133-9a5e-5a83-53052012b014"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TN", "Tswana", "Setswana" },
                    { new Guid("0797a7d5-bbc0-2e52-0de8-14a42fc80baa"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BE", "Belarusian", "беларуская мова" },
                    { new Guid("081a5fdb-445a-015a-1e36-f2e5014265ae"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KL", "Kalaallisut", "kalaallisut" },
                    { new Guid("0932ed88-c79f-591a-d684-9a77735f947e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KG", "Kongo", "Kikongo" },
                    { new Guid("094b3769-68b1-6211-ba2d-6bba92d6a167"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EN", "English", "English" },
                    { new Guid("096a8586-9702-6fec-5f6a-6eb3b7b7837f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "GU", "Gujarati", "ગુજરાતી" },
                    { new Guid("0a25f96f-5173-2fff-a2f8-c6872393edf6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SM", "Samoan", "gagana fa'a Samoa" },
                    { new Guid("0ab731f0-5326-44be-af3a-20aa33ad0f35"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SE", "Northern Sami", "Davvisámegiella" },
                    { new Guid("0b9b4368-7ceb-e519-153d-2c58c983852b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "XH", "Xhosa", "isiXhosa" },
                    { new Guid("0c0fef20-0e8d-98ea-7724-12cea9b3b926"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NA", "Nauru", "Dorerin Naoero" },
                    { new Guid("0ce6f5e0-0789-fa0e-b4b5-23a5b1f5e257"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ZH", "Chinese", "中文" },
                    { new Guid("0d4fe6e6-ea1e-d1ce-5134-6c0c1a696a00"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "FO", "Faroese", "føroyskt" },
                    { new Guid("0e2a1681-d852-67ae-7387-0d04be9e7fd3"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "FJ", "Fijian", "vosa Vakaviti" },
                    { new Guid("11765ad0-30f2-bab8-b616-20f88b28b21e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TK", "Turkmen", "Türkmençe" },
                    { new Guid("13016d0c-fbf0-9503-12f2-e0f8d27394ae"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "YI", "Yiddish", "ייִדיש" },
                    { new Guid("136610e1-8115-9cf1-d671-7950c6483495"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "RM", "Romansh", "rumantsch grischun" },
                    { new Guid("17ed5f0f-e091-94ff-0512-ad291bde94d7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CV", "Chuvash", "чӑваш чӗлхи" },
                    { new Guid("1d974338-decf-08e5-3e62-89e1bbdbb003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ID", "Indonesian", "Bahasa Indonesia" },
                    { new Guid("1da84244-fa39-125e-06dc-3c0cb2342ce9"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EO", "Esperanto", "Esperanto" },
                    { new Guid("1e5c0dcc-83e9-f275-c81d-3bc49f88e70c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "LB", "Luxembourgish", "Lëtzebuergesch" },
                    { new Guid("1f8be615-5746-277e-d82b-47596b5bb922"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HR", "Croatian", "Hrvatski" },
                    { new Guid("2167da32-4f80-d31d-226c-0551970304eb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SC", "Sardinian", "sardu" },
                    { new Guid("2299a74f-3ebc-f022-da1a-44ae59335b3b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TY", "Tahitian", "Reo Tahiti" },
                    { new Guid("23785991-fef4-e625-4d3b-b6ac364d0fa0"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IK", "Inupiaq", "Iñupiaq" },
                    { new Guid("285b9e82-38af-33ab-79fd-0b4f3fd4f2f1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OR", "Oriya", "ଓଡ଼ିଆ" },
                    { new Guid("29201cbb-ca65-1924-75a9-0c4d4db43001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AE", "Avestan", "avesta" },
                    { new Guid("294978f0-2702-d35d-cfc4-e676148aea2e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IE", "Interlingue", "Interlingue" },
                    { new Guid("2a039b16-2adf-0fb8-3bdf-fbdf14358d9d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PT", "Portuguese", "Português" },
                    { new Guid("2b6d383a-9ab6-fcdf-bcfe-a4538faca407"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OC", "Occitan", "occitan" },
                    { new Guid("2bebebe4-edaa-9160-5a0c-4d99048bd8d5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HT", "Haitian", "Kreyòl ayisyen" },
                    { new Guid("2c7b808d-7786-2deb-5318-56f7c238520e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ZU", "Zulu", "isiZulu" },
                    { new Guid("2d013d34-b258-8fe9-ef52-dd34e82a4672"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OS", "Ossetian", "ирон æвзаг" },
                    { new Guid("2dc643bd-cc6c-eb0c-7314-44123576f0ee"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EE", "Ewe", "Eʋegbe" },
                    { new Guid("2e1bd9d8-df06-d773-0eb9-98e274b63b43"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NG", "Ndonga", "Owambo" },
                    { new Guid("2e9cb133-68a7-2f3b-49d1-0921cf42dfae"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HA", "Hausa", "هَوُسَ" },
                    { new Guid("2f00fe86-a06b-dc95-0ea7-4520d1dec784"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "GL", "Galician", "galego" },
                    { new Guid("3175ac19-c801-0b87-8e66-7480a40dcf1e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SN", "Shona", "chiShona" },
                    { new Guid("3252e51a-5bc1-f065-7101-5b34ba493dc4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SK", "Slovak", "slovenčina" },
                    { new Guid("357369e3-85a8-86f7-91c7-349772ae7744"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "UZ", "Uzbek", "Ўзбек" },
                    { new Guid("357c121b-e28d-1765-e699-cc4ec5ff86fc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SI", "Sinhala", "සිංහල" },
                    { new Guid("37c89068-a8e9-87e8-d651-f86fac63673a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CH", "Chamorro", "Chamoru" },
                    { new Guid("3bf5a74a-6d12-e971-16bc-c75e487f2615"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TE", "Telugu", "తెలుగు" },
                    { new Guid("3c5828e0-16a8-79ba-4e5c-9b45065df113"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KY", "Kyrgyz", "Кыргызча" },
                    { new Guid("3ce3d958-7341-bd79-f294-f2e6907c186c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SG", "Sango", "yângâ tî sängö" },
                    { new Guid("3e2cccbe-1615-c707-a97b-421a799b2559"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "UG", "Uyghur", "ئۇيغۇرچە‎" },
                    { new Guid("3ffe68ca-7350-175b-4e95-0c34f54dc1f4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "GN", "Guaraní", "Avañe'ẽ" },
                    { new Guid("414a34ce-2781-8f96-2bd0-7ada86c8cf38"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ES", "Spanish", "Español" },
                    { new Guid("46576b73-c05b-7498-5b07-9bbf59b7645d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BG", "Bulgarian", "български език" },
                    { new Guid("46e88019-c521-57b2-d1c0-c0e2478d3b05"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BS", "Bosnian", "bosanski jezik" },
                    { new Guid("46ef1468-86f6-0c99-f4e9-46f966167b05"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "DE", "German", "Deutsch" },
                    { new Guid("4826bc0f-235e-572f-2b1a-21f1c9e05f83"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "GA", "Irish", "Gaeilge" },
                    { new Guid("4a3aa5a4-473f-45cd-f054-fa0465c476a4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NB", "Norwegian Bokmål", "Norsk bokmål" },
                    { new Guid("4d8bcda4-5598-16cd-b379-97eb7a5e1c29"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SV", "Swedish", "Svenska" },
                    { new Guid("4def223a-9524-596d-cc29-ab7830c590de"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CS", "Czech", "čeština" },
                    { new Guid("4ee6400d-5534-7c67-1521-870d6b732366"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IS", "Icelandic", "Íslenska" },
                    { new Guid("50e5954d-7cb4-2201-b96c-f2a846ab3ae3"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MS", "Malay", "Bahasa Melayu" },
                    { new Guid("51a86a09-0d0b-31c1-90f1-f237db8e29ad"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "FF", "Fula", "Fulfulde" },
                    { new Guid("51aa4900-30a6-91b7-2728-071542a064ff"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "RO", "Romanian", "Română" },
                    { new Guid("52538361-bbdf-fafb-e434-5655fc7451e5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "LT", "Lithuanian", "lietuvių kalba" },
                    { new Guid("5283afbb-2744-e930-2c16-c5ea6b0ff7cc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BR", "Breton", "brezhoneg" },
                    { new Guid("52d9992c-19bd-82b4-9188-11dabcac6171"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "VE", "Venda", "Tshivenḓa" },
                    { new Guid("538114de-7db0-9242-35e6-324fa7eff44d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AS", "Assamese", "অসমীয়া" },
                    { new Guid("54686fcd-3f35-f468-7c9c-93217c5084bc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HI", "Hindi", "हिन्दी" },
                    { new Guid("54726f17-03b8-8af3-0359-c42d8fe8459d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MI", "Māori", "te reo Māori" },
                    { new Guid("57765d87-2424-2c86-ad9c-1af58ef3127a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CU", "Old Church Slavonic", "ѩзыкъ словѣньскъ" },
                    { new Guid("58337ef3-3d24-43e9-a440-832306e7fc07"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "RU", "Russian", "Русский" },
                    { new Guid("596e8283-10ce-d81d-2e6f-400fa259d717"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TI", "Tigrinya", "ትግርኛ" },
                    { new Guid("5a5d9168-081b-1e02-1fbb-cdfa910e526c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "FI", "Finnish", "suomi" },
                    { new Guid("5c0e654b-8547-5d02-ee7b-d65e3c5c5273"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CA", "Catalan", "Català" },
                    { new Guid("5e7a08f2-7d59-bcdb-7ddd-876b87181420"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KM", "Khmer", "ខេមរភាសា" },
                    { new Guid("5f002f07-f2c3-9fa4-2e29-225d116c10a3"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SW", "Swahili", "Kiswahili" },
                    { new Guid("61ba1844-4d33-84b4-dbac-70718aa91d59"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SR", "Serbian", "српски језик" },
                    { new Guid("6200b376-9eae-d01b-de52-8674aaf5b013"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TS", "Tsonga", "Xitsonga" },
                    { new Guid("629b68d8-1d71-d3ce-f13e-45048ffff017"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "WA", "Walloon", "walon" },
                    { new Guid("67729f87-ef47-dd3f-65f7-b0f6df0d6384"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NV", "Navajo", "Diné bizaad" },
                    { new Guid("6857242c-f772-38b5-b5a2-c8e8b9db551f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "JA", "Japanese", "日本語" },
                    { new Guid("688af4c8-9d64-ae1c-147f-b8afd54801e3"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AM", "Amharic", "አማርኛ" },
                    { new Guid("6aac6f0e-d13a-a629-4c2b-9d6eaf6680e4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SS", "Swati", "SiSwati" },
                    { new Guid("6c366974-3672-3a2c-2345-0fda33942304"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OM", "Oromo", "Afaan Oromoo" },
                    { new Guid("70673250-4cc3-3ba1-a42c-6b62ea8ab1d5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "LU", "Luba-Katanga", "Kiluba" },
                    { new Guid("720b4e12-b001-8d38-7c07-f43194b9645d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NY", "Chichewa", "chiCheŵa" },
                    { new Guid("7451108d-ad49-940a-d479-4d868b62a7c6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KU", "Kurdish", "Kurdî" },
                    { new Guid("74da982f-cf20-e1b4-517b-a040511af23c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MR", "Marathi", "मराठी" },
                    { new Guid("74f19a84-b1c5-fa2d-8818-2220b80a3056"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KO", "Korean", "한국어" },
                    { new Guid("75e4464b-a784-63b8-1ecc-69ee1f09f43f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BI", "Bislama", "Bislama" },
                    { new Guid("766c1ebb-78c1-bada-37fb-c45d1bd4baff"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ST", "Southern Sotho", "Sesotho" },
                    { new Guid("78b7020d-8b82-3fae-2049-30e490ae1faf"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KV", "Komi", "коми кыв" },
                    { new Guid("78c6e8af-fcb4-c783-987c-7e1aca3aed64"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AY", "Aymara", "aymar aru" },
                    { new Guid("7a0725cf-311a-4f59-cff8-ad8b43dd226e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "RN", "Kirundi", "Ikirundi" },
                    { new Guid("7bbf15f4-a907-c0b2-7029-144aafb3c59d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IT", "Italian", "Italiano" },
                    { new Guid("7bf4a786-3733-c670-e85f-03ee3caa6ef9"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PA", "Panjabi", "ਪੰਜਾਬੀ" },
                    { new Guid("7bf934fa-bcf4-80b5-fd7d-ab4cca45c67b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KR", "Kanuri", "Kanuri" },
                    { new Guid("7f065da7-4ba4-81ca-5126-dbf606a73907"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IA", "Interlingua", "Interlingua" },
                    { new Guid("802c05db-3866-545d-dc1a-a02c83ea6cf6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SO", "Somali", "Soomaaliga" },
                    { new Guid("80b770b8-4797-3d62-ef66-1ded7b0da0e5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "LG", "Ganda", "Luganda" },
                    { new Guid("80ecea2c-8969-1929-0d4a-39ed2324abc6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KJ", "Kwanyama", "Kuanyama" },
                    { new Guid("849b5e66-dc68-a1ed-6ed3-e315fbd0a0e5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "GV", "Manx", "Gaelg" },
                    { new Guid("84d58b3d-d131-1506-0792-1b3228b6f71f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TH", "Thai", "ไทย" },
                    { new Guid("875060ca-73f6-af3b-d844-1b1416ce4583"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TW", "Twi", "Twi" },
                    { new Guid("87813ec7-4830-e4dc-5ab1-bd599057ede0"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HO", "Hiri Motu", "Hiri Motu" },
                    { new Guid("899392d7-d54f-a1c6-407a-1bada9b85fdd"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IU", "Inuktitut", "ᐃᓄᒃᑎᑐᑦ" },
                    { new Guid("8bc44f03-84a5-2afc-8b0b-40c727e4ce36"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TA", "Tamil", "தமிழ்" },
                    { new Guid("914618fd-86f9-827a-91b8-826f0db9e02d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KI", "Kikuyu", "Gĩkũyũ" },
                    { new Guid("914d7923-3ac5-75e8-c8e2-47d72561e35d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NO", "Norwegian", "Norsk" },
                    { new Guid("9204928b-c569-ef6a-446e-4853aee439b0"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KA", "Georgian", "ქართული" },
                    { new Guid("9205dbfc-60cd-91d9-b0b8-8a18a3755286"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "LV", "Latvian", "latviešu valoda" },
                    { new Guid("93fb8ace-4156-12d5-218e-64b7d35129b1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "QU", "Quechua", "Runa Simi" },
                    { new Guid("943d2419-2ca6-95f8-9c3b-ed445aea0371"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MH", "Marshallese", "Kajin M̧ajeļ" },
                    { new Guid("95467997-f989-f456-34b7-0b578302dcba"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TT", "Tatar", "татар теле" },
                    { new Guid("976e496f-ca38-d113-1697-8af2d9a3b159"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MG", "Malagasy", "fiteny malagasy" },
                    { new Guid("97cd39d5-1aca-8f10-9f5e-3f611d7606d8"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NE", "Nepali", "नेपाली" },
                    { new Guid("9c11bb58-5135-453a-1d24-dc20ef0e9031"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AA", "Afar", "Afaraf" },
                    { new Guid("9d6e6446-185e-235e-8771-9eb2d19f22e7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "LI", "Limburgish", "Limburgs" },
                    { new Guid("9dacf00b-7d0a-d744-cc60-e5fa66371e9d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TG", "Tajik", "тоҷикӣ" },
                    { new Guid("9e7dbdc3-2c8b-e8ae-082b-e02695f8268e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TO", "Tonga", "faka Tonga" },
                    { new Guid("9ec46cb5-6c2b-0e22-07c5-eb2fe1b8d2ff"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OJ", "Ojibwe", "ᐊᓂᔑᓈᐯᒧᐎᓐ" },
                    { new Guid("a7716d29-6ef6-b775-51c5-97094536329d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BA", "Bashkir", "башҡорт теле" },
                    { new Guid("a7afb7b1-b26d-4571-1a1f-3fff738ff21e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AR", "Arabic", "اَلْعَرَبِيَّةُ" },
                    { new Guid("a8f30b36-4a25-3fb9-c69e-84ce6640d785"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SA", "Sanskrit", "संस्कृतम्" },
                    { new Guid("aa0f69b2-93aa-ec51-b43b-60145db79e38"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MK", "Macedonian", "македонски јазик" },
                    { new Guid("b0f4bdfa-17dd-9714-4fe8-3c3b1f010ffa"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SL", "Slovenian", "slovenščina" },
                    { new Guid("b2261c50-1a57-7f1f-d72d-f8c21593874f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "FR", "French", "Français" },
                    { new Guid("b2a87091-32fb-ba34-a721-bf8b3de5935d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EU", "Basque", "euskara" },
                    { new Guid("b356a541-1383-3c0a-9afd-6aebae3753cb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "DA", "Danish", "dansk" },
                    { new Guid("b4292ad3-3ca8-eea5-f3e0-d1983db8f61e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ND", "Northern Ndebele", "isiNdebele" },
                    { new Guid("b6b2351f-4f1e-c92f-0e9a-a915f4cc5fa6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KK", "Kazakh", "қазақ тілі" },
                    { new Guid("b6f70436-9515-7ef8-af57-aad196503499"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KW", "Cornish", "Kernewek" },
                    { new Guid("b8b09512-ea4c-4a61-9331-304f55324ef7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IO", "Ido", "Ido" },
                    { new Guid("b9da7f73-60cd-404c-18fb-1bc5bbfffb38"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EL", "Greek", "Ελληνικά" },
                    { new Guid("bd4f1638-6017-733d-f696-b8b4d72664d7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AB", "Abkhaz", "аҧсуа бызшәа" },
                    { new Guid("c03d71a5-b215-8672-ec0c-dd8fe5c20e05"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ML", "Malayalam", "മലയാളം" },
                    { new Guid("c2254fd9-159e-4064-0fbf-a7969cba06ec"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "VO", "Volapük", "Volapük" },
                    { new Guid("c4754c00-cfa5-aa6f-a9c8-a200457de7a8"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "LA", "Latin", "latine" },
                    { new Guid("c522b3d3-74cc-846f-0394-737dff4d2b1a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MN", "Mongolian", "Монгол хэл" },
                    { new Guid("c64288fc-d941-0615-47f9-28e6c294ce26"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CO", "Corsican", "corsu" },
                    { new Guid("ca2a5560-d4c4-3c87-3090-6f5436310b55"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BM", "Bambara", "bamanankan" },
                    { new Guid("ca44a869-d3b6-052d-1e1a-ad4e3682a2ed"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "LN", "Lingala", "Lingála" },
                    { new Guid("ca6bfadf-4e87-0692-a6b3-20ea6a51555d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "WO", "Wolof", "Wollof" },
                    { new Guid("caddae27-283a-82b2-9365-76a3d6c49eee"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IG", "Igbo", "Asụsụ Igbo" },
                    { new Guid("cd5689d6-7a06-73c7-650e-f6f94387fd88"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CE", "Chechen", "нохчийн мотт" },
                    { new Guid("cfff3443-1378-9c7d-9d58-66146d7f29a6"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NL", "Dutch", "Nederlands" },
                    { new Guid("d13935c1-8956-1399-7c4e-0354795cd37b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CR", "Cree", "ᓀᐦᐃᔭᐍᐏᐣ" },
                    { new Guid("d292ea2d-fbb6-7c1e-cb7d-23d552673776"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MY", "Burmese", "ဗမာစာ" },
                    { new Guid("d4d5c45a-d3c2-891e-6d7d-75569c3386ac"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AN", "Aragonese", "aragonés" },
                    { new Guid("d55a9eb2-48fc-2719-47bf-99e902c28e80"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "YO", "Yoruba", "Yorùbá" },
                    { new Guid("d5bffdfb-6a8e-6d9f-2e59-4ada912acdba"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HZ", "Herero", "Otjiherero" },
                    { new Guid("d685aa26-aee7-716b-9433-1b3411209f4b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HE", "Hebrew", "עברית" },
                    { new Guid("d6d31cdd-280a-56bc-24a4-a414028d2b67"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PS", "Pashto", "پښتو" },
                    { new Guid("d832c50a-112e-4591-9432-4ada24bc85b2"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HY", "Armenian", "Հայերեն" },
                    { new Guid("d8d4f63d-fa65-63dd-a788-de2eec3d24ec"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "DV", "Divehi", "ދިވެހި" },
                    { new Guid("d8ef067c-1087-4ff5-8e1f-2291df7ac958"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PI", "Pāli", "पाऴि" },
                    { new Guid("dcf19e1d-74a6-7b8b-a5ed-76b94a8ac2a7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HU", "Hungarian", "magyar" },
                    { new Guid("de29d5e7-2ecf-a4ff-5e40-5e83edd0d9b4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "UK", "Ukrainian", "Українська" },
                    { new Guid("de503629-2607-b948-e279-0509d8109d0f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PL", "Polish", "Polski" },
                    { new Guid("df20d0d7-9fbe-e725-d966-4fdf9f5c9dfb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CY", "Welsh", "Cymraeg" },
                    { new Guid("df41c815-40f8-197a-7a8b-e456d43283d9"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NN", "Norwegian Nynorsk", "Norsk nynorsk" },
                    { new Guid("e1947bdc-ff2c-d2c1-3c55-f1f9bf778578"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "VI", "Vietnamese", "Tiếng Việt" },
                    { new Guid("e3bacefb-d79b-1569-a91c-43d7e4f6f230"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NR", "Southern Ndebele", "isiNdebele" },
                    { new Guid("e43a2010-14fc-63a9-f9d3-0ab2a1d0e52f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AV", "Avaric", "авар мацӀ" },
                    { new Guid("e7532b00-3b1b-ff2c-b7c0-26bd7e91af55"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "JV", "Javanese", "basa Jawa" },
                    { new Guid("e75515a6-63cf-3612-a3a2-befa0d7048a7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ET", "Estonian", "eesti" },
                    { new Guid("e9ad0bec-7dee-bd01-9528-1fc74d1d78dd"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "LO", "Lao", "ພາສາລາວ" },
                    { new Guid("e9da8997-dee8-0c2d-79d3-05fafc45092e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "FA", "Persian", "فارسی" },
                    { new Guid("eace47f6-5499-f4f0-8f97-ed165b681d84"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KS", "Kashmiri", "कश्मीरी" },
                    { new Guid("ebf38b9a-6fbe-6e82-3977-2c4763bea072"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ZA", "Zhuang", "Saɯ cueŋƅ" },
                    { new Guid("ed6278e0-436c-9fd9-0b9e-44fd424cbd1b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BN", "Bengali", "বাংলা" },
                    { new Guid("edd4319b-86f3-24cb-248c-71da624c02f7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AF", "Afrikaans", "Afrikaans" },
                    { new Guid("ee1ace14-e945-4767-85ec-3d74be8b516b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SU", "Sundanese", "Basa Sunda" },
                    { new Guid("ef584e3c-03f2-42b0-7139-69d15d21e5a8"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AK", "Akan", "Akan" },
                    { new Guid("f0219540-8b2c-bd29-4f76-b832de53a56f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MT", "Maltese", "Malti" },
                    { new Guid("f0965449-6b15-6c1a-f5cb-ebd2d575c02c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SD", "Sindhi", "सिन्धी" },
                    { new Guid("f1f09549-a9bb-da4a-9b98-8655a01235aa"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "UR", "Urdu", "اردو" },
                    { new Guid("f21f562e-5c35-4806-4efc-416619b5b7f7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "II", "Nuosu", "ꆈꌠ꒿ Nuosuhxop" },
                    { new Guid("f33ced84-eb43-fb39-ef79-b266e4d4cd94"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BO", "Tibetan", "བོད་ཡིག" },
                    { new Guid("f39cca22-449e-9866-3a65-465a5510483e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TR", "Turkish", "Türkçe" },
                    { new Guid("f5b15ea6-133d-c2c9-7ef9-b0916ea96edb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "RW", "Kinyarwanda", "Ikinyarwanda" },
                    { new Guid("fa633273-9866-840d-9739-c6c957901e46"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "KN", "Kannada", "ಕನ್ನಡ" },
                    { new Guid("fb1cce84-4a6c-1834-0ff2-6df002e3d56f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SQ", "Albanian", "Shqip" },
                    { new Guid("fb429393-f994-0a16-37f9-edc0510fced5"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "FY", "Western Frisian", "Frysk" },
                    { new Guid("fb9a713c-2de1-882a-64b7-0e8fef5d2f7e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TL", "Tagalog", "Wikang Tagalog" },
                    { new Guid("fee6f04f-c4c1-e3e4-645d-bb6bb703aeb7"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "DZ", "Dzongkha", "རྫོང་ཁ" },
                    { new Guid("ff5b4d88-c179-ff0d-6285-cf46ba475d7d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "GD", "Scottish Gaelic", "Gàidhlig" }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ElectionRounds_CountryId",
                table: "ElectionRounds",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportValidationErrors_Id",
                table: "ImportValidationErrors",
                column: "Id");

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
                name: "IX_MonitoringNgos_ElectionRoundId",
                table: "MonitoringNgos",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringNgos_NgoId",
                table: "MonitoringNgos",
                column: "NgoId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringObserverNotification_TargetedObserversId",
                table: "MonitoringObserverNotification",
                column: "TargetedObserversId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringObservers_InviterNgoId",
                table: "MonitoringObservers",
                column: "InviterNgoId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringObservers_MonitoringNgoId",
                table: "MonitoringObservers",
                column: "MonitoringNgoId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringObservers_ObserverId",
                table: "MonitoringObservers",
                column: "ObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_NgoAdmins_NgoId",
                table: "NgoAdmins",
                column: "NgoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ElectionRoundId",
                table: "Notifications",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SenderId",
                table: "Notifications",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTokens_ObserverId",
                table: "NotificationTokens",
                column: "ObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_ObserversGuides_MonitoringNgoId",
                table: "ObserversGuides",
                column: "MonitoringNgoId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationAttachments_ElectionRoundId",
                table: "PollingStationAttachments",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationAttachments_MonitoringObserverId",
                table: "PollingStationAttachments",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationAttachments_PollingStationId",
                table: "PollingStationAttachments",
                column: "PollingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformationForms_ElectionRoundId",
                table: "PollingStationInformationForms",
                column: "ElectionRoundId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformations_ElectionRoundId",
                table: "PollingStationInformations",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformations_MonitoringObserverId",
                table: "PollingStationInformations",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformations_PollingStationId",
                table: "PollingStationInformations",
                column: "PollingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationInformations_PollingStationInformationFormId",
                table: "PollingStationInformations",
                column: "PollingStationInformationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationNotes_ElectionRoundId",
                table: "PollingStationNotes",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationNotes_MonitoringObserverId",
                table: "PollingStationNotes",
                column: "MonitoringObserverId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStationNotes_PollingStationId",
                table: "PollingStationNotes",
                column: "PollingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_PollingStations_ElectionRoundId",
                table: "PollingStations",
                column: "ElectionRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrails");

            migrationBuilder.DropTable(
                name: "FormTemplates");

            migrationBuilder.DropTable(
                name: "ImportValidationErrors");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "MonitoringObserverNotification");

            migrationBuilder.DropTable(
                name: "NotificationTokens");

            migrationBuilder.DropTable(
                name: "ObserversGuides");

            migrationBuilder.DropTable(
                name: "PlatformAdmins");

            migrationBuilder.DropTable(
                name: "PollingStationAttachments");

            migrationBuilder.DropTable(
                name: "PollingStationInformations");

            migrationBuilder.DropTable(
                name: "PollingStationNotes");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PollingStationInformationForms");

            migrationBuilder.DropTable(
                name: "MonitoringObservers");

            migrationBuilder.DropTable(
                name: "PollingStations");

            migrationBuilder.DropTable(
                name: "NgoAdmins");

            migrationBuilder.DropTable(
                name: "MonitoringNgos");

            migrationBuilder.DropTable(
                name: "Observers");

            migrationBuilder.DropTable(
                name: "ElectionRounds");

            migrationBuilder.DropTable(
                name: "Ngos");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
