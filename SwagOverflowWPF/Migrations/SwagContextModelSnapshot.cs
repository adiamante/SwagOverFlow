﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SwagOverflowWPF.Data;

namespace SwagOverflowWPF.Migrations
{
    [DbContext(typeof(SwagContext))]
    partial class SwagContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagGroupViewModel", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AlternateId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Display")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RootId")
                        .HasColumnType("int");

                    b.HasKey("GroupId");

                    b.HasIndex("AlternateId")
                        .IsUnique()
                        .HasFilter("[AlternateId] IS NOT NULL");

                    b.ToTable("SwagGroups");

                    b.HasDiscriminator<string>("Discriminator").HasValue("SwagGroupViewModel");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagItemViewModel", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AlternateId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Display")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GroupRootId")
                        .HasColumnType("int");

                    b.Property<bool>("IsExpanded")
                        .HasColumnType("bit");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<int>("Sequence")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ValueTypeString")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GroupId", "ItemId");

                    b.HasIndex("AlternateId")
                        .IsUnique()
                        .HasFilter("[AlternateId] IS NOT NULL");

                    b.HasIndex("GroupRootId")
                        .IsUnique()
                        .HasFilter("[GroupRootId] IS NOT NULL");

                    b.HasIndex("GroupId", "ParentId");

                    b.ToTable("SwagItems");

                    b.HasDiscriminator<string>("Discriminator").HasValue("SwagItemViewModel");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagSettingGroupViewModel", b =>
                {
                    b.HasBaseType("SwagOverflowWPF.ViewModels.SwagGroupViewModel");

                    b.HasDiscriminator().HasValue("SwagSettingGroupViewModel");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagSettingViewModel", b =>
                {
                    b.HasBaseType("SwagOverflowWPF.ViewModels.SwagItemViewModel");

                    b.Property<string>("ItemsSource")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ItemsSourceTypeString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SettingType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("SwagSettingViewModel");
                });

            modelBuilder.Entity("SwagOverflowWPF.Controls.SwagWindowSettingGroup", b =>
                {
                    b.HasBaseType("SwagOverflowWPF.ViewModels.SwagSettingGroupViewModel");

                    b.HasDiscriminator().HasValue("SwagWindowSettingGroup");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagItemViewModel", b =>
                {
                    b.HasOne("SwagOverflowWPF.ViewModels.SwagGroupViewModel", "Group")
                        .WithMany("Descendants")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SwagOverflowWPF.ViewModels.SwagGroupViewModel", "GroupRoot")
                        .WithOne("Root")
                        .HasForeignKey("SwagOverflowWPF.ViewModels.SwagItemViewModel", "GroupRootId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("SwagOverflowWPF.ViewModels.SwagItemViewModel", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("GroupId", "ParentId")
                        .OnDelete(DeleteBehavior.NoAction);
                });
#pragma warning restore 612, 618
        }
    }
}
