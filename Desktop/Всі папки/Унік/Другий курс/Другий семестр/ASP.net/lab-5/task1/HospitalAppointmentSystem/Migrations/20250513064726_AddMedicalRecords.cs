using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalAppointmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicalRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDrafts_Doctors_DoctorId",
                table: "AppointmentDrafts");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDrafts_Patients_PatientId",
                table: "AppointmentDrafts");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentDrafts_SessionId",
                table: "AppointmentDrafts");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "AppointmentDrafts",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "AppointmentDrafts",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppointmentDateTime",
                table: "AppointmentDrafts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    MedicalRecordId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Diagnosis = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Treatment = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    RecordDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PatientId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.MedicalRecordId);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_PatientId",
                table: "MedicalRecords",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDrafts_Doctors_DoctorId",
                table: "AppointmentDrafts",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDrafts_Patients_PatientId",
                table: "AppointmentDrafts",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDrafts_Doctors_DoctorId",
                table: "AppointmentDrafts");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDrafts_Patients_PatientId",
                table: "AppointmentDrafts");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "AppointmentDrafts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "AppointmentDrafts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppointmentDateTime",
                table: "AppointmentDrafts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AbsoluteExpiration = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiresAtTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SlidingExpirationInSeconds = table.Column<long>(type: "INTEGER", nullable: true),
                    Value = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDrafts_SessionId",
                table: "AppointmentDrafts",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDrafts_Doctors_DoctorId",
                table: "AppointmentDrafts",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDrafts_Patients_PatientId",
                table: "AppointmentDrafts",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
