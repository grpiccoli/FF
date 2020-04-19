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
                name: "Diams",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Entry2Id = table.Column<int>(nullable: false),
                    Idg = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entry2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AgeEnd = table.Column<int>(nullable: false),
                    AgeStart = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    Deviation = table.Column<decimal>(nullable: false),
                    Distribution = table.Column<int>(nullable: false),
                    DistributionThinning = table.Column<int>(nullable: false),
                    HeightFunction = table.Column<int>(nullable: false),
                    IP = table.Column<string>(nullable: true),
                    Model = table.Column<int>(nullable: false),
                    ProcessStart = table.Column<DateTime>(nullable: true),
                    ProcessTime = table.Column<double>(nullable: true),
                    Species = table.Column<int>(nullable: false),
                    Stage = table.Column<int>(nullable: false),
                    VolumeFormula = table.Column<int>(nullable: false),
                    Way = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entry2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entry2_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Command = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Command = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Input2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Age = table.Column<double>(nullable: false),
                    BA = table.Column<double>(nullable: false),
                    D100 = table.Column<double>(nullable: false),
                    Dg = table.Column<double>(nullable: false),
                    Entry2Id = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_Input2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Input2_Entry2_Entry2Id",
                        column: x => x.Entry2Id,
                        principalTable: "Entry2",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parameter2",
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
                    Entry2Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameter2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameter2_Entry2_Entry2Id",
                        column: x => x.Entry2Id,
                        principalTable: "Entry2",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Diameter = table.Column<int>(nullable: false),
                    Entry2Id = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    X_1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product2_Entry2_Entry2Id",
                        column: x => x.Entry2Id,
                        principalTable: "Entry2",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Simulation2",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Age = table.Column<double>(nullable: false),
                    BA = table.Column<double>(nullable: false),
                    CAI_Dg = table.Column<double>(nullable: true),
                    CAI_Vt = table.Column<double>(nullable: true),
                    Dg = table.Column<double>(nullable: false),
                    Distr = table.Column<string>(nullable: true),
                    Entry2Id = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_Simulation2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Simulation2_Entry2_Entry2Id",
                        column: x => x.Entry2Id,
                        principalTable: "Entry2",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Taper2",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Dbh = table.Column<double>(nullable: false),
                    Entry2Id = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_Taper2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Taper2_Entry2_Entry2Id",
                        column: x => x.Entry2Id,
                        principalTable: "Entry2",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tree",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RegionId = table.Column<int>(nullable: false),
                    SpeciesId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tree", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tree_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tree_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entry3",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AgeEnd = table.Column<int>(nullable: false),
                    AgeStart = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    ByClass = table.Column<bool>(nullable: false),
                    Distribution = table.Column<int>(nullable: false),
                    DistributionThinning = table.Column<int>(nullable: false),
                    IP = table.Column<string>(nullable: true),
                    Include_Thinning = table.Column<bool>(nullable: false),
                    LengthDisc = table.Column<double>(nullable: false),
                    MgDisc = table.Column<double>(nullable: false),
                    Model = table.Column<int>(nullable: false),
                    ProcessStart = table.Column<DateTime>(nullable: true),
                    ProcessTime = table.Column<double>(nullable: true),
                    Stage = table.Column<int>(nullable: false),
                    Stump = table.Column<double>(nullable: false),
                    TreeId = table.Column<int>(nullable: false),
                    VolumeFormula = table.Column<int>(nullable: false),
                    Way = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entry3", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entry3_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entry3_Tree_TreeId",
                        column: x => x.TreeId,
                        principalTable: "Tree",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Input3",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Age = table.Column<double>(nullable: false),
                    BA = table.Column<double>(nullable: false),
                    D100 = table.Column<double>(nullable: false),
                    DBH_max = table.Column<double>(nullable: false),
                    DBH_sd = table.Column<double>(nullable: false),
                    Dg = table.Column<double>(nullable: false),
                    Entry3Id = table.Column<int>(nullable: false),
                    Hd = table.Column<double>(nullable: false),
                    Hm = table.Column<int>(nullable: false),
                    Hp = table.Column<int>(nullable: false),
                    Id_ = table.Column<int>(nullable: false),
                    Macrostand = table.Column<string>(nullable: true),
                    N = table.Column<double>(nullable: false),
                    NAfterThins = table.Column<string>(nullable: true),
                    Pyear = table.Column<int>(nullable: false),
                    Random_BA = table.Column<string>(nullable: true),
                    Random_SI = table.Column<string>(nullable: true),
                    ThinCoefs = table.Column<string>(nullable: true),
                    ThinTypes = table.Column<string>(nullable: true),
                    ThinningAges = table.Column<string>(nullable: true),
                    Vt = table.Column<int>(nullable: false),
                    Years = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Input3", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Input3_Entry3_Entry3Id",
                        column: x => x.Entry3Id,
                        principalTable: "Entry3",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parameter3",
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
                    Entry3Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameter3", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameter3_Entry3_Entry3Id",
                        column: x => x.Entry3Id,
                        principalTable: "Entry3",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product3",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Class = table.Column<int>(nullable: false),
                    Diameter = table.Column<int>(nullable: false),
                    Entry3Id = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    LogType = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false),
                    X_1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product3", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product3_Entry3_Entry3Id",
                        column: x => x.Entry3Id,
                        principalTable: "Entry3",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Simulation3",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Age = table.Column<double>(nullable: false),
                    BA = table.Column<double>(nullable: false),
                    CAI_Dg = table.Column<double>(nullable: true),
                    CAI_Vt = table.Column<double>(nullable: true),
                    Dg = table.Column<double>(nullable: false),
                    Distr = table.Column<string>(nullable: true),
                    Entry3Id = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_Simulation3", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Simulation3_Entry3_Entry3Id",
                        column: x => x.Entry3Id,
                        principalTable: "Entry3",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaperLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Class = table.Column<string>(nullable: true),
                    Dbh = table.Column<double>(nullable: false),
                    Diameter = table.Column<double>(nullable: false),
                    Entry3Id = table.Column<int>(nullable: false),
                    Freq = table.Column<double>(nullable: false),
                    Grade = table.Column<string>(nullable: true),
                    Hm = table.Column<int>(nullable: false),
                    Hp = table.Column<int>(nullable: false),
                    Ht = table.Column<double>(nullable: false),
                    Id_ = table.Column<int>(nullable: false),
                    Idg = table.Column<int>(nullable: false),
                    Idgu = table.Column<int>(nullable: false),
                    Idseq = table.Column<int>(nullable: false),
                    Log = table.Column<int>(nullable: false),
                    LogType = table.Column<string>(nullable: true),
                    Product = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false),
                    Volume = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaperLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaperLog_Entry3_Entry3Id",
                        column: x => x.Entry3Id,
                        principalTable: "Entry3",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaperStandHarvest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Age = table.Column<int>(nullable: false),
                    Entry3Id = table.Column<int>(nullable: false),
                    Id_ = table.Column<int>(nullable: false),
                    Idg = table.Column<int>(nullable: false),
                    Macrostand = table.Column<string>(nullable: true),
                    Pyear = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaperStandHarvest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaperStandHarvest_Entry3_Entry3Id",
                        column: x => x.Entry3Id,
                        principalTable: "Entry3",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaperStandThinning",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Age = table.Column<int>(nullable: false),
                    Entry3Id = table.Column<int>(nullable: false),
                    Id_ = table.Column<int>(nullable: false),
                    Idg = table.Column<int>(nullable: false),
                    Macrostand = table.Column<string>(nullable: true),
                    Pyear = table.Column<int>(nullable: false),
                    Thin_name = table.Column<string>(nullable: true),
                    Thin_year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaperStandThinning", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaperStandThinning_Entry3_Entry3Id",
                        column: x => x.Entry3Id,
                        principalTable: "Entry3",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Class = table.Column<string>(nullable: true),
                    Entry3Id = table.Column<int>(nullable: false),
                    Idg = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VP_Entry3_Entry3Id",
                        column: x => x.Entry3Id,
                        principalTable: "Entry3",
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
                name: "IX_Entry2_ApplicationUserId",
                table: "Entry2",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Entry3_ApplicationUserId",
                table: "Entry3",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Entry3_TreeId",
                table: "Entry3",
                column: "TreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Input2_Entry2Id",
                table: "Input2",
                column: "Entry2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Input3_Entry3Id",
                table: "Input3",
                column: "Entry3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Parameter2_Entry2Id",
                table: "Parameter2",
                column: "Entry2Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parameter3_Entry3Id",
                table: "Parameter3",
                column: "Entry3Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product2_Entry2Id",
                table: "Product2",
                column: "Entry2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Product3_Entry3Id",
                table: "Product3",
                column: "Entry3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Simulation2_Entry2Id",
                table: "Simulation2",
                column: "Entry2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Simulation3_Entry3Id",
                table: "Simulation3",
                column: "Entry3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Taper2_Entry2Id",
                table: "Taper2",
                column: "Entry2Id");

            migrationBuilder.CreateIndex(
                name: "IX_TaperLog_Entry3Id",
                table: "TaperLog",
                column: "Entry3Id");

            migrationBuilder.CreateIndex(
                name: "IX_TaperStandHarvest_Entry3Id",
                table: "TaperStandHarvest",
                column: "Entry3Id");

            migrationBuilder.CreateIndex(
                name: "IX_TaperStandThinning_Entry3Id",
                table: "TaperStandThinning",
                column: "Entry3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tree_RegionId",
                table: "Tree",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Tree_SpeciesId",
                table: "Tree",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_VP_Entry3Id",
                table: "VP",
                column: "Entry3Id");

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
                name: "Input2");

            migrationBuilder.DropTable(
                name: "Input3");

            migrationBuilder.DropTable(
                name: "Parameter2");

            migrationBuilder.DropTable(
                name: "Parameter3");

            migrationBuilder.DropTable(
                name: "Product2");

            migrationBuilder.DropTable(
                name: "Product3");

            migrationBuilder.DropTable(
                name: "Simulation2");

            migrationBuilder.DropTable(
                name: "Simulation3");

            migrationBuilder.DropTable(
                name: "Taper2");

            migrationBuilder.DropTable(
                name: "TaperLog");

            migrationBuilder.DropTable(
                name: "TaperStandHarvest");

            migrationBuilder.DropTable(
                name: "TaperStandThinning");

            migrationBuilder.DropTable(
                name: "VP");

            migrationBuilder.DropTable(
                name: "Entry2");

            migrationBuilder.DropTable(
                name: "Entry3");

            migrationBuilder.DropTable(
                name: "Tree");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "Species");

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
                name: "MemberSince",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
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
