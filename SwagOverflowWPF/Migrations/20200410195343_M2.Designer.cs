﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SwagOverflowWPF.Data;

namespace SwagOverflowWPF.Migrations
{
    [DbContext(typeof(SwagContext))]
    [Migration("20200410195343_M2")]
    partial class M2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2");

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagItemBase", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AlternateId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("CanUndo")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Display")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsExpanded")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.Property<string>("ObjValue")
                        .HasColumnType("TEXT");

                    b.Property<int>("Sequence")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ValueTypeString")
                        .HasColumnType("TEXT");

                    b.HasKey("ItemId");

                    b.HasIndex("AlternateId")
                        .IsUnique();

                    b.ToTable("SwagItems");

                    b.HasDiscriminator<string>("Discriminator").HasValue("SwagItemBase");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagDataRow", b =>
                {
                    b.HasBaseType("SwagOverflowWPF.ViewModels.SwagItemBase");

                    b.Property<int?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasIndex("ParentId");

                    b.HasDiscriminator().HasValue("SwagDataRow");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagSetting", b =>
                {
                    b.HasBaseType("SwagOverflowWPF.ViewModels.SwagItemBase");

                    b.Property<string>("IconString")
                        .HasColumnType("TEXT");

                    b.Property<string>("IconTypeString")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemsSourceTypeString")
                        .HasColumnType("TEXT");

                    b.Property<string>("ObjItemsSource")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentId")
                        .HasColumnName("SwagSetting_ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SettingType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasIndex("ParentId");

                    b.HasDiscriminator().HasValue("SwagSetting");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagTabItem", b =>
                {
                    b.HasBaseType("SwagOverflowWPF.ViewModels.SwagItemBase");

                    b.Property<string>("IconString")
                        .HasColumnName("SwagTabItem_IconString")
                        .HasColumnType("TEXT");

                    b.Property<string>("IconTypeString")
                        .HasColumnName("SwagTabItem_IconTypeString")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentId")
                        .HasColumnName("SwagTabItem_ParentId")
                        .HasColumnType("INTEGER");

                    b.HasIndex("ParentId");

                    b.HasDiscriminator().HasValue("SwagTabItem");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagDataTable", b =>
                {
                    b.HasBaseType("SwagOverflowWPF.ViewModels.SwagDataRow");

                    b.Property<string>("Columns")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Listening")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Settings")
                        .HasColumnType("TEXT");

                    b.Property<string>("Tabs")
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("SwagDataTable");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagSettingBoolean", b =>
                {
                    b.HasBaseType("SwagOverflowWPF.ViewModels.SwagSetting");

                    b.HasDiscriminator().HasValue("SwagSettingBoolean");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagSettingGroup", b =>
                {
                    b.HasBaseType("SwagOverflowWPF.ViewModels.SwagSetting");

                    b.Property<string>("Name")
                        .HasColumnName("SwagSettingGroup_Name")
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("SwagSettingGroup");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagSettingString", b =>
                {
                    b.HasBaseType("SwagOverflowWPF.ViewModels.SwagSetting");

                    b.HasDiscriminator().HasValue("SwagSettingString");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagTabCollection", b =>
                {
                    b.HasBaseType("SwagOverflowWPF.ViewModels.SwagTabItem");

                    b.Property<bool>("IsInitialized")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SelectedIndex")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowChildText")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("SwagTabCollection");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagWindowSettingGroup", b =>
                {
                    b.HasBaseType("SwagOverflowWPF.ViewModels.SwagSettingGroup");

                    b.HasDiscriminator().HasValue("SwagWindowSettingGroup");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagDataRow", b =>
                {
                    b.HasOne("SwagOverflowWPF.ViewModels.SwagDataTable", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagSetting", b =>
                {
                    b.HasOne("SwagOverflowWPF.ViewModels.SwagSettingGroup", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("SwagOverflowWPF.ViewModels.SwagTabItem", b =>
                {
                    b.HasOne("SwagOverflowWPF.ViewModels.SwagTabCollection", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });
#pragma warning restore 612, 618
        }
    }
}