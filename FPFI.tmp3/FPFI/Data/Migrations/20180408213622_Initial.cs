using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FPFI.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<bool>(
                name: "IsActivated",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Last",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MemberSince",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleAssigner",
                table: "AspNetUserRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUserRoles",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationRoleId",
                table: "AspNetUserClaims",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "AspNetRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetRoles",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Entry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AgeEnd = table.Column<int>(nullable: false),
                    AgeStart = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Distribution = table.Column<int>(nullable: false),
                    DistributionThinning = table.Column<int>(nullable: false),
                    IP = table.Column<string>(nullable: true),
                    Model = table.Column<int>(nullable: false),
                    Output = table.Column<string>(nullable: true),
                    ProcessStart = table.Column<DateTime>(nullable: true),
                    ProcessTime = table.Column<double>(nullable: true),
                    Stage = table.Column<int>(nullable: false),
                    VolumeFormula = table.Column<int>(nullable: false),
                    Way = table.Column<int>(nullable: false),
                    Deviation = table.Column<decimal>(nullable: true),
                    HeightFunction = table.Column<int>(nullable: true),
                    Species = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entry_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inputs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Age = table.Column<double>(nullable: false),
                    BA = table.Column<double>(nullable: false),
                    D100 = table.Column<double>(nullable: false),
                    Dg = table.Column<double>(nullable: false),
                    Entry2Id = table.Column<int>(nullable: true),
                    EntryId = table.Column<int>(nullable: false),
                    Hd = table.Column<double>(nullable: false),
                    Hm = table.Column<int>(nullable: false),
                    Hp = table.Column<int>(nullable: false),
                    Id_ = table.Column<int>(nullable: false),
                    Macrostand = table.Column<string>(nullable: true),
                    N = table.Column<double>(nullable: false),
                    NAfterThins = table.Column<string>(nullable: true),
                    Pyear = table.Column<int>(nullable: false),
                    ThinCoefs = table.Column<string>(nullable: true),
                    ThinTypes = table.Column<string>(nullable: true),
                    ThinningAges = table.Column<string>(nullable: true),
                    Vt = table.Column<int>(nullable: false),
                    Years = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inputs_Entry_Entry2Id",
                        column: x => x.Entry2Id,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inputs_Entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Alpha1 = table.Column<double>(nullable: false),
                    Alpha2 = table.Column<double>(nullable: false),
                    Beta1 = table.Column<double>(nullable: false),
                    Beta2 = table.Column<double>(nullable: false),
                    Beta3 = table.Column<double>(nullable: false),
                    Beta4 = table.Column<double>(nullable: false),
                    EntryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameters_Entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Diameter = table.Column<int>(nullable: false),
                    Entry2Id = table.Column<int>(nullable: true),
                    EntryId = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    X_1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products2_Entry_Entry2Id",
                        column: x => x.Entry2Id,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products2_Entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Simulations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Age = table.Column<double>(nullable: false),
                    BA = table.Column<double>(nullable: false),
                    CAI_Dg = table.Column<double>(nullable: true),
                    CAI_Vt = table.Column<double>(nullable: true),
                    Dg = table.Column<double>(nullable: false),
                    Distr = table.Column<string>(nullable: true),
                    EntryId = table.Column<int>(nullable: false),
                    Hd = table.Column<double>(nullable: false),
                    Id_ = table.Column<int>(nullable: false),
                    Idg = table.Column<int>(nullable: false),
                    MAI_Dg = table.Column<double>(nullable: false),
                    MAI_Vt = table.Column<double>(nullable: false),
                    Macrostand = table.Column<string>(nullable: true),
                    N = table.Column<double>(nullable: false),
                    Sd = table.Column<double>(nullable: false),
                    ThinCoefs = table.Column<string>(nullable: true),
                    ThinTypes = table.Column<string>(nullable: true),
                    Thin_trees = table.Column<double>(nullable: false),
                    Thinaction = table.Column<bool>(nullable: false),
                    Vt = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Simulations_Entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tapers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Dbh = table.Column<double>(nullable: false),
                    Entry2Id = table.Column<int>(nullable: true),
                    EntryId = table.Column<int>(nullable: false),
                    Freq = table.Column<int>(nullable: false),
                    Hm = table.Column<int>(nullable: false),
                    Hp = table.Column<int>(nullable: false),
                    Ht = table.Column<double>(nullable: false),
                    Id_ = table.Column<int>(nullable: false),
                    Idg = table.Column<int>(nullable: false),
                    Idgu = table.Column<int>(nullable: false),
                    MerchVol = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tapers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tapers_Entry_Entry2Id",
                        column: x => x.Entry2Id,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tapers_Entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Diams",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    EntryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TaperId = table.Column<long>(nullable: false),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diams_Tapers_TaperId",
                        column: x => x.TaperId,
                        principalTable: "Tapers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_ApplicationRoleId",
                table: "AspNetUserClaims",
                column: "ApplicationRoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Diams_TaperId",
                table: "Diams",
                column: "TaperId");

            migrationBuilder.CreateIndex(
                name: "IX_Entry_ApplicationUserId",
                table: "Entry",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Inputs_Entry2Id",
                table: "Inputs",
                column: "Entry2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Inputs_EntryId",
                table: "Inputs",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_EntryId",
                table: "Parameters",
                column: "EntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products2_Entry2Id",
                table: "Products2",
                column: "Entry2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Products2_EntryId",
                table: "Products2",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulations_EntryId",
                table: "Simulations",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tapers_Entry2Id",
                table: "Tapers",
                column: "Entry2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tapers_EntryId",
                table: "Tapers",
                column: "EntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetRoles_ApplicationRoleId",
                table: "AspNetUserClaims",
                column: "ApplicationRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetRoles_ApplicationRoleId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Diams");

            migrationBuilder.DropTable(
                name: "Inputs");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "Products2");

            migrationBuilder.DropTable(
                name: "Simulations");

            migrationBuilder.DropTable(
                name: "Tapers");

            migrationBuilder.DropTable(
                name: "Entry");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserClaims_ApplicationRoleId",
                table: "AspNetUserClaims");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsActivated",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Last",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MemberSince",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RoleAssigner",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "ApplicationRoleId",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetRoles");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}
