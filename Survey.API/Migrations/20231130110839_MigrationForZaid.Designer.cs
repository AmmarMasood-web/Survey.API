﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Survey.API.Database;

#nullable disable

namespace Survey.API.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20231130110839_MigrationForZaid")]
    partial class MigrationForZaid
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Survey.API.Models.SurveyModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("surveyId");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedOn");

                    b.Property<string>("jsonData")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("jsonData");

                    b.Property<string>("message")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("message");

                    b.Property<string>("respCode")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("code");

                    b.Property<string>("surveyDescription")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("description");

                    b.Property<string>("surveyTitle")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("title");

                    b.HasKey("Id");

                    b.ToTable("Surveys");
                });
#pragma warning restore 612, 618
        }
    }
}
