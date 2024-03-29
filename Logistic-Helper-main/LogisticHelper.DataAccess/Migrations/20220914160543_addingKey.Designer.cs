﻿// <auto-generated />
using System;
using LogisticHelper.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LogisticHelper.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220914160543_addingKey")]
    partial class addingKey
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-preview.4.22229.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("LogisticHelper.Models.Terc", b =>
                {
                    b.Property<string>("GMI")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NAZWA")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NAZWA_DOD")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("POW")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RODZ")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("STAN_NA")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WOJ")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("Tercs");
                });

            modelBuilder.Entity("LogisticHelper.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<DateTime>("DATE_OF_BIRTH")
                        .HasColumnType("datetime2");

                    b.Property<string>("LAST_NAME")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MAIL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NAME")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PHONE")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
