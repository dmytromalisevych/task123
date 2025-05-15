using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalAppointmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentDrafts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentDrafts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PatientId = table.Column<int>(type: "INTEGER", nullable: false),
                    DoctorId = table.Column<int>(type: "INTEGER", nullable: false),
                    AppointmentDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    SessionId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentDrafts_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentDrafts_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<byte[]>(type: "BLOB", nullable: false),
                    ExpiresAtTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SlidingExpirationInSeconds = table.Column<long>(type: "INTEGER", nullable: true),
                    AbsoluteExpiration = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDrafts_DoctorId",
                table: "AppointmentDrafts",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDrafts_PatientId",
                table: "AppointmentDrafts",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDrafts_SessionId",
                table: "AppointmentDrafts",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentDrafts");

            migrationBuilder.DropTable(
                name: "Sessions");
        }
    }
}
