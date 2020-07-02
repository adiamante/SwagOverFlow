﻿// <auto-generated />
using Dreamporter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dreamporter.Migrations
{
    [DbContext(typeof(DreamporterContext))]
    [Migration("20200701043907_Third")]
    partial class Third
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Dreamporter.Core.Integration", b =>
                {
                    b.Property<int>("IntegrationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CoreBuild")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DataContexts")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DefaultOptions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InitialSchemas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InstructionTemplates")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OptionsSet")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfileBuild")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SelectedOptions")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IntegrationId");

                    b.ToTable("Integrations");
                });
#pragma warning restore 612, 618
        }
    }
}
