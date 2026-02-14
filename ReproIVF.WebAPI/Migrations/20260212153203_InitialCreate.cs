using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReproIVF.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bulls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bulls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreservationMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreservationMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SemenTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemenTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Technicians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technicians", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Donors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProgramId = table.Column<int>(type: "int", nullable: true),
                    EmbryoCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Donors_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Implants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramId = table.Column<int>(type: "int", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: true),
                    OpuDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FreezingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StrawId = table.Column<int>(type: "int", nullable: true),
                    DonorId = table.Column<int>(type: "int", nullable: true),
                    BullId = table.Column<int>(type: "int", nullable: true),
                    SemenTypeId = table.Column<int>(type: "int", nullable: true),
                    PreservationMethodId = table.Column<int>(type: "int", nullable: true),
                    ImplantDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecipId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TechnicianId = table.Column<int>(type: "int", nullable: true),
                    InCalf = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Implants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Implants_Bulls_BullId",
                        column: x => x.BullId,
                        principalTable: "Bulls",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Implants_Clients_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Implants_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "Donors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Implants_PreservationMethods_PreservationMethodId",
                        column: x => x.PreservationMethodId,
                        principalTable: "PreservationMethods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Implants_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Implants_SemenTypes_SemenTypeId",
                        column: x => x.SemenTypeId,
                        principalTable: "SemenTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Implants_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "PreservationMethods",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Fresh" },
                    { 2, "DT" },
                    { 3, "Vit" }
                });

            migrationBuilder.InsertData(
                table: "SemenTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "conv" },
                    { 2, "sex" },
                    { 3, "Empty" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bulls_Name",
                table: "Bulls",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Name",
                table: "Clients",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donors_Code",
                table: "Donors",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donors_ProgramId",
                table: "Donors",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Implants_BullId",
                table: "Implants",
                column: "BullId");

            migrationBuilder.CreateIndex(
                name: "IX_Implants_DonorId",
                table: "Implants",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_Implants_OwnerId",
                table: "Implants",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Implants_PreservationMethodId",
                table: "Implants",
                column: "PreservationMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Implants_ProgramId",
                table: "Implants",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Implants_SemenTypeId",
                table: "Implants",
                column: "SemenTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Implants_TechnicianId",
                table: "Implants",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationMethods_Name",
                table: "PreservationMethods",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Programs_Name",
                table: "Programs",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SemenTypes_Name",
                table: "SemenTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Technicians_Name",
                table: "Technicians",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Implants");

            migrationBuilder.DropTable(
                name: "Bulls");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Donors");

            migrationBuilder.DropTable(
                name: "PreservationMethods");

            migrationBuilder.DropTable(
                name: "SemenTypes");

            migrationBuilder.DropTable(
                name: "Technicians");

            migrationBuilder.DropTable(
                name: "Programs");
        }
    }
}
