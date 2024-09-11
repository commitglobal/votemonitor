using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class NotificationReadFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObserverNotification_MonitoringObservers_Targeted~",
                table: "MonitoringObserverNotification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MonitoringObserverNotification",
                table: "MonitoringObserverNotification");

            migrationBuilder.DropIndex(
                name: "IX_MonitoringObserverNotification_TargetedObserversId",
                table: "MonitoringObserverNotification");

            migrationBuilder.RenameColumn(
                name: "TargetedObserversId",
                table: "MonitoringObserverNotification",
                newName: "MonitoringObserverId");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "MonitoringObserverNotification",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonitoringObserverNotification",
                table: "MonitoringObserverNotification",
                columns: new[] { "MonitoringObserverId", "NotificationId" });

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringObserverNotification_NotificationId",
                table: "MonitoringObserverNotification",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObserverNotification_MonitoringObservers_Monitori~",
                table: "MonitoringObserverNotification",
                column: "MonitoringObserverId",
                principalTable: "MonitoringObservers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringObserverNotification_MonitoringObservers_Monitori~",
                table: "MonitoringObserverNotification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MonitoringObserverNotification",
                table: "MonitoringObserverNotification");

            migrationBuilder.DropIndex(
                name: "IX_MonitoringObserverNotification_NotificationId",
                table: "MonitoringObserverNotification");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "MonitoringObserverNotification");

            migrationBuilder.RenameColumn(
                name: "MonitoringObserverId",
                table: "MonitoringObserverNotification",
                newName: "TargetedObserversId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonitoringObserverNotification",
                table: "MonitoringObserverNotification",
                columns: new[] { "NotificationId", "TargetedObserversId" });

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringObserverNotification_TargetedObserversId",
                table: "MonitoringObserverNotification",
                column: "TargetedObserversId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringObserverNotification_MonitoringObservers_Targeted~",
                table: "MonitoringObserverNotification",
                column: "TargetedObserversId",
                principalTable: "MonitoringObservers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
