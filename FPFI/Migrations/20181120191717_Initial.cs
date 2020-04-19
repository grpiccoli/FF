using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FPFI.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    IPAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    MemberSince = table.Column<DateTime>(nullable: false),
                    IsActivated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

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
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Command = table.Column<string>(nullable: true)
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
                    Name = table.Column<string>(nullable: true),
                    Command = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    ApplicationRoleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetRoles_ApplicationRoleId",
                        column: x => x.ApplicationRoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    RoleAssigner = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId1",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entry2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    AgeStart = table.Column<int>(nullable: false),
                    AgeEnd = table.Column<int>(nullable: false),
                    Distribution = table.Column<int>(nullable: false),
                    DistributionThinning = table.Column<int>(nullable: false),
                    Model = table.Column<int>(nullable: false),
                    VolumeFormula = table.Column<int>(nullable: false),
                    Way = table.Column<int>(nullable: false),
                    IP = table.Column<string>(nullable: true),
                    ProcessStart = table.Column<DateTime>(nullable: true),
                    ProcessTime = table.Column<double>(nullable: true),
                    Stage = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Output = table.Column<string>(nullable: true),
                    Deviation = table.Column<double>(nullable: false),
                    Species = table.Column<int>(nullable: false),
                    HeightFunction = table.Column<int>(nullable: false)
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
                name: "Tree",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SpeciesId = table.Column<int>(nullable: false),
                    RegionId = table.Column<int>(nullable: false),
                    Default = table.Column<bool>(nullable: false)
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
                name: "Input2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Id_ = table.Column<int>(nullable: false),
                    Macrostand = table.Column<string>(nullable: true),
                    Pyear = table.Column<int>(nullable: false),
                    Age = table.Column<double>(nullable: false),
                    N = table.Column<double>(nullable: false),
                    BA = table.Column<double>(nullable: false),
                    Dg = table.Column<double>(nullable: false),
                    D100 = table.Column<double>(nullable: false),
                    Hd = table.Column<double>(nullable: false),
                    Vt = table.Column<int>(nullable: false),
                    Years = table.Column<int>(nullable: false),
                    ThinningAges = table.Column<string>(nullable: true),
                    NAfterThins = table.Column<string>(nullable: true),
                    ThinTypes = table.Column<string>(nullable: true),
                    ThinCoefs = table.Column<string>(nullable: true),
                    Hp = table.Column<int>(nullable: false),
                    Hm = table.Column<int>(nullable: false),
                    Entry2Id = table.Column<int>(nullable: false)
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
                    Beta1 = table.Column<double>(nullable: false),
                    Beta2 = table.Column<double>(nullable: false),
                    Beta3 = table.Column<double>(nullable: false),
                    Beta4 = table.Column<double>(nullable: false),
                    Alpha1 = table.Column<double>(nullable: false),
                    Alpha2 = table.Column<double>(nullable: false),
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
                    X_1 = table.Column<string>(nullable: true),
                    Diameter = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    Entry2Id = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false)
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
                    Id_ = table.Column<int>(nullable: false),
                    Macrostand = table.Column<string>(nullable: true),
                    Age = table.Column<double>(nullable: false),
                    N = table.Column<double>(nullable: false),
                    BA = table.Column<double>(nullable: false),
                    Dg = table.Column<double>(nullable: false),
                    Hd = table.Column<double>(nullable: false),
                    Vt = table.Column<double>(nullable: false),
                    Sd = table.Column<double>(nullable: false),
                    Thin_trees = table.Column<double>(nullable: false),
                    Thinaction = table.Column<bool>(nullable: false),
                    ThinTypes = table.Column<string>(nullable: true),
                    ThinCoefs = table.Column<string>(nullable: true),
                    Distr = table.Column<string>(nullable: true),
                    Idg = table.Column<int>(nullable: false),
                    CAI_Dg = table.Column<double>(nullable: true),
                    CAI_Vt = table.Column<double>(nullable: true),
                    MAI_Dg = table.Column<double>(nullable: false),
                    MAI_Vt = table.Column<double>(nullable: false),
                    Entry2Id = table.Column<int>(nullable: false)
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
                    Id_ = table.Column<int>(nullable: false),
                    Entry2Id = table.Column<int>(nullable: false),
                    Idg = table.Column<int>(nullable: false),
                    Dbh = table.Column<double>(nullable: false),
                    Ht = table.Column<double>(nullable: false),
                    Freq = table.Column<int>(nullable: false),
                    Idgu = table.Column<int>(nullable: false),
                    Hp = table.Column<int>(nullable: false),
                    Hm = table.Column<int>(nullable: false),
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
                name: "Entry3",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    AgeStart = table.Column<int>(nullable: false),
                    AgeEnd = table.Column<int>(nullable: false),
                    Distribution = table.Column<int>(nullable: false),
                    DistributionThinning = table.Column<int>(nullable: false),
                    Model = table.Column<int>(nullable: false),
                    VolumeFormula = table.Column<int>(nullable: false),
                    Way = table.Column<int>(nullable: false),
                    IP = table.Column<string>(nullable: true),
                    ProcessStart = table.Column<DateTime>(nullable: true),
                    ProcessTime = table.Column<double>(nullable: true),
                    Stage = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Output = table.Column<string>(nullable: true),
                    Include_Thinning = table.Column<bool>(nullable: false),
                    TreeId = table.Column<int>(nullable: false),
                    ByClass = table.Column<bool>(nullable: false),
                    Stump = table.Column<double>(nullable: false),
                    MgDisc = table.Column<double>(nullable: false),
                    LengthDisc = table.Column<double>(nullable: false)
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
                    Id_ = table.Column<int>(nullable: false),
                    Macrostand = table.Column<string>(nullable: true),
                    Pyear = table.Column<int>(nullable: false),
                    Age = table.Column<double>(nullable: false),
                    N = table.Column<double>(nullable: false),
                    BA = table.Column<double>(nullable: false),
                    Dg = table.Column<double>(nullable: false),
                    D100 = table.Column<double>(nullable: false),
                    Hd = table.Column<double>(nullable: false),
                    Vt = table.Column<int>(nullable: false),
                    Years = table.Column<int>(nullable: false),
                    ThinningAges = table.Column<string>(nullable: true),
                    NAfterThins = table.Column<string>(nullable: true),
                    ThinTypes = table.Column<string>(nullable: true),
                    ThinCoefs = table.Column<string>(nullable: true),
                    Hp = table.Column<int>(nullable: false),
                    Hm = table.Column<int>(nullable: false),
                    Entry3Id = table.Column<int>(nullable: false),
                    DBH_sd = table.Column<double>(nullable: false),
                    DBH_max = table.Column<double>(nullable: false),
                    Random_SI = table.Column<string>(nullable: true),
                    Random_BA = table.Column<string>(nullable: true)
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
                    Beta1 = table.Column<double>(nullable: false),
                    Beta2 = table.Column<double>(nullable: false),
                    Beta3 = table.Column<double>(nullable: false),
                    Beta4 = table.Column<double>(nullable: false),
                    Alpha1 = table.Column<double>(nullable: false),
                    Alpha2 = table.Column<double>(nullable: false),
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
                    X_1 = table.Column<string>(nullable: true),
                    Diameter = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    Entry3Id = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false),
                    LogType = table.Column<int>(nullable: false),
                    Class = table.Column<string>(nullable: true)
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
                    Id_ = table.Column<int>(nullable: false),
                    Macrostand = table.Column<string>(nullable: true),
                    Age = table.Column<double>(nullable: false),
                    N = table.Column<double>(nullable: false),
                    BA = table.Column<double>(nullable: false),
                    Dg = table.Column<double>(nullable: false),
                    Hd = table.Column<double>(nullable: false),
                    Vt = table.Column<double>(nullable: false),
                    Sd = table.Column<double>(nullable: false),
                    Thin_trees = table.Column<double>(nullable: false),
                    Thinaction = table.Column<bool>(nullable: false),
                    ThinTypes = table.Column<string>(nullable: true),
                    ThinCoefs = table.Column<string>(nullable: true),
                    Distr = table.Column<string>(nullable: true),
                    Idg = table.Column<int>(nullable: false),
                    CAI_Dg = table.Column<double>(nullable: true),
                    CAI_Vt = table.Column<double>(nullable: true),
                    MAI_Dg = table.Column<double>(nullable: false),
                    MAI_Vt = table.Column<double>(nullable: false),
                    Entry3Id = table.Column<int>(nullable: false)
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
                    Entry3Id = table.Column<int>(nullable: false),
                    Idseq = table.Column<int>(nullable: false),
                    Grade = table.Column<string>(nullable: true),
                    LogType = table.Column<string>(nullable: true),
                    Log = table.Column<int>(nullable: false),
                    Volume = table.Column<double>(nullable: false),
                    Product = table.Column<string>(nullable: true),
                    Value = table.Column<int>(nullable: false),
                    Class = table.Column<string>(nullable: true),
                    Id_ = table.Column<int>(nullable: false),
                    Idg = table.Column<int>(nullable: false),
                    Dbh = table.Column<double>(nullable: false),
                    Ht = table.Column<double>(nullable: false),
                    Freq = table.Column<double>(nullable: false),
                    Idgu = table.Column<int>(nullable: false),
                    Hp = table.Column<int>(nullable: false),
                    Hm = table.Column<int>(nullable: false),
                    LED_h = table.Column<double>(nullable: false),
                    LED = table.Column<double>(nullable: false),
                    MED_h = table.Column<double>(nullable: false),
                    MED = table.Column<double>(nullable: false),
                    SED_h = table.Column<double>(nullable: false),
                    SED = table.Column<double>(nullable: false),
                    Type = table.Column<int>(nullable: false)
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
                    Entry3Id = table.Column<int>(nullable: false),
                    NA = table.Column<double>(nullable: true),
                    Id_ = table.Column<int>(nullable: false),
                    Macrostand = table.Column<string>(nullable: true),
                    Pyear = table.Column<int>(nullable: false),
                    Idg = table.Column<int>(nullable: false),
                    Age = table.Column<int>(nullable: false)
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
                    Entry3Id = table.Column<int>(nullable: false),
                    NA = table.Column<double>(nullable: true),
                    Id_ = table.Column<int>(nullable: false),
                    Macrostand = table.Column<string>(nullable: true),
                    Pyear = table.Column<int>(nullable: false),
                    Idg = table.Column<int>(nullable: false),
                    Age = table.Column<int>(nullable: false),
                    Thin_year = table.Column<int>(nullable: false),
                    Thin_name = table.Column<string>(nullable: true)
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
                    Entry3Id = table.Column<int>(nullable: false),
                    Class = table.Column<string>(nullable: true),
                    Value = table.Column<double>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Idg = table.Column<int>(nullable: false)
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_ApplicationRoleId",
                table: "AspNetUserClaims",
                column: "ApplicationRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

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
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Entry2");

            migrationBuilder.DropTable(
                name: "Entry3");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Tree");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "Species");
        }
    }
}
