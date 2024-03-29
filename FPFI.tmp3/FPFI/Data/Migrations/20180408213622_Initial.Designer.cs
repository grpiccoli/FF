﻿// <auto-generated />
using FPFI.Data;
using FPFI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;

namespace FPFI.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20180408213622_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FPFI.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("IsActivated");

                    b.Property<string>("Last");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<DateTime>("MemberSince");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("ProfileImageUrl");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("FPFI.Models.Diam", b =>
                {
                    b.Property<long>("Id");

                    b.Property<int>("EntryId");

                    b.Property<string>("Name");

                    b.Property<long>("TaperId");

                    b.Property<double>("Value");

                    b.HasKey("Id");

                    b.HasIndex("TaperId");

                    b.ToTable("Diams");
                });

            modelBuilder.Entity("FPFI.Models.Entry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AgeEnd");

                    b.Property<int>("AgeStart");

                    b.Property<string>("ApplicationUserId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int>("Distribution");

                    b.Property<int>("DistributionThinning");

                    b.Property<string>("IP");

                    b.Property<int>("Model");

                    b.Property<string>("Output");

                    b.Property<DateTime?>("ProcessStart");

                    b.Property<double?>("ProcessTime");

                    b.Property<int>("Stage");

                    b.Property<int>("VolumeFormula");

                    b.Property<int>("Way");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Entry");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Entry");
                });

            modelBuilder.Entity("FPFI.Models.Input", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Age");

                    b.Property<double>("BA");

                    b.Property<double>("D100");

                    b.Property<double>("Dg");

                    b.Property<int?>("Entry2Id");

                    b.Property<int>("EntryId");

                    b.Property<double>("Hd");

                    b.Property<int>("Hm");

                    b.Property<int>("Hp");

                    b.Property<int>("Id_");

                    b.Property<string>("Macrostand");

                    b.Property<double>("N");

                    b.Property<string>("NAfterThins");

                    b.Property<int>("Pyear");

                    b.Property<string>("ThinCoefs");

                    b.Property<string>("ThinTypes");

                    b.Property<string>("ThinningAges");

                    b.Property<int>("Vt");

                    b.Property<int>("Years");

                    b.HasKey("Id");

                    b.HasIndex("Entry2Id");

                    b.HasIndex("EntryId");

                    b.ToTable("Inputs");
                });

            modelBuilder.Entity("FPFI.Models.Parameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Alpha1");

                    b.Property<double>("Alpha2");

                    b.Property<double>("Beta1");

                    b.Property<double>("Beta2");

                    b.Property<double>("Beta3");

                    b.Property<double>("Beta4");

                    b.Property<int>("EntryId");

                    b.HasKey("Id");

                    b.HasIndex("EntryId")
                        .IsUnique();

                    b.ToTable("Parameters");
                });

            modelBuilder.Entity("FPFI.Models.Product2", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Diameter");

                    b.Property<int?>("Entry2Id");

                    b.Property<int>("EntryId");

                    b.Property<int>("Length");

                    b.Property<int>("Priority");

                    b.Property<string>("X_1");

                    b.HasKey("Id");

                    b.HasIndex("Entry2Id");

                    b.HasIndex("EntryId");

                    b.ToTable("Products2");
                });

            modelBuilder.Entity("FPFI.Models.Simulation", b =>
                {
                    b.Property<long>("Id");

                    b.Property<double>("Age");

                    b.Property<double>("BA");

                    b.Property<double?>("CAI_Dg");

                    b.Property<double?>("CAI_Vt");

                    b.Property<double>("Dg");

                    b.Property<string>("Distr");

                    b.Property<int>("EntryId");

                    b.Property<double>("Hd");

                    b.Property<int>("Id_");

                    b.Property<int>("Idg");

                    b.Property<double>("MAI_Dg");

                    b.Property<double>("MAI_Vt");

                    b.Property<string>("Macrostand");

                    b.Property<double>("N");

                    b.Property<double>("Sd");

                    b.Property<string>("ThinCoefs");

                    b.Property<string>("ThinTypes");

                    b.Property<double>("Thin_trees");

                    b.Property<bool>("Thinaction");

                    b.Property<double>("Vt");

                    b.HasKey("Id");

                    b.HasIndex("EntryId");

                    b.ToTable("Simulations");
                });

            modelBuilder.Entity("FPFI.Models.Taper", b =>
                {
                    b.Property<long>("Id");

                    b.Property<double>("Dbh");

                    b.Property<int?>("Entry2Id");

                    b.Property<int>("EntryId");

                    b.Property<int>("Freq");

                    b.Property<int>("Hm");

                    b.Property<int>("Hp");

                    b.Property<double>("Ht");

                    b.Property<int>("Id_");

                    b.Property<int>("Idg");

                    b.Property<int>("Idgu");

                    b.Property<double>("MerchVol");

                    b.HasKey("Id");

                    b.HasIndex("Entry2Id");

                    b.HasIndex("EntryId");

                    b.ToTable("Tapers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityRole");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationRoleId");

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ApplicationRoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUserRole<string>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("FPFI.Models.Entry2", b =>
                {
                    b.HasBaseType("FPFI.Models.Entry");

                    b.Property<decimal>("Deviation");

                    b.Property<int>("HeightFunction");

                    b.Property<int>("Species");

                    b.ToTable("Entry2");

                    b.HasDiscriminator().HasValue("Entry2");
                });

            modelBuilder.Entity("FPFI.Models.ApplicationRole", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityRole");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description");

                    b.Property<string>("IPAddress");

                    b.ToTable("ApplicationRole");

                    b.HasDiscriminator().HasValue("ApplicationRole");
                });

            modelBuilder.Entity("FPFI.Models.ApplicationUserRole", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUserRole<string>");

                    b.Property<string>("RoleAssigner");

                    b.ToTable("ApplicationUserRole");

                    b.HasDiscriminator().HasValue("ApplicationUserRole");
                });

            modelBuilder.Entity("FPFI.Models.Diam", b =>
                {
                    b.HasOne("FPFI.Models.Taper", "Taper")
                        .WithMany("Diameters")
                        .HasForeignKey("TaperId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FPFI.Models.Entry", b =>
                {
                    b.HasOne("FPFI.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("Entries")
                        .HasForeignKey("ApplicationUserId");
                });

            modelBuilder.Entity("FPFI.Models.Input", b =>
                {
                    b.HasOne("FPFI.Models.Entry2")
                        .WithMany("Inputs")
                        .HasForeignKey("Entry2Id");

                    b.HasOne("FPFI.Models.Entry", "Entry")
                        .WithMany()
                        .HasForeignKey("EntryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FPFI.Models.Parameter", b =>
                {
                    b.HasOne("FPFI.Models.Entry", "Entry")
                        .WithOne("Parameter")
                        .HasForeignKey("FPFI.Models.Parameter", "EntryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FPFI.Models.Product2", b =>
                {
                    b.HasOne("FPFI.Models.Entry2")
                        .WithMany("Products")
                        .HasForeignKey("Entry2Id");

                    b.HasOne("FPFI.Models.Entry", "Entry")
                        .WithMany()
                        .HasForeignKey("EntryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FPFI.Models.Simulation", b =>
                {
                    b.HasOne("FPFI.Models.Entry", "Entry")
                        .WithMany("Simulations")
                        .HasForeignKey("EntryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FPFI.Models.Taper", b =>
                {
                    b.HasOne("FPFI.Models.Entry2")
                        .WithMany("Tapers")
                        .HasForeignKey("Entry2Id");

                    b.HasOne("FPFI.Models.Entry", "Entry")
                        .WithMany()
                        .HasForeignKey("EntryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("FPFI.Models.ApplicationRole")
                        .WithMany("Claims")
                        .HasForeignKey("ApplicationRoleId");

                    b.HasOne("FPFI.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("FPFI.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("FPFI.Models.ApplicationRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FPFI.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("FPFI.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
