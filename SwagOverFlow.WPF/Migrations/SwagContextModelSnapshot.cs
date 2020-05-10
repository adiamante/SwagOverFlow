﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SwagOverFlow.WPF.Data;

namespace SwagOverFlow.WPF.Migrations
{
    [DbContext(typeof(SwagContext))]
    partial class SwagContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2");

            modelBuilder.Entity("SwagOverFlow.ViewModels.SwagItemBase", b =>
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

                    b.Property<bool>("IsSelected")
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

            modelBuilder.Entity("SwagOverFlow.ViewModels.SwagData", b =>
                {
                    b.HasBaseType("SwagOverFlow.ViewModels.SwagItemBase");

                    b.Property<int?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasIndex("ParentId");

                    b.HasDiscriminator().HasValue("SwagData");
                });

            modelBuilder.Entity("SwagOverFlow.ViewModels.SwagSetting", b =>
                {
                    b.HasBaseType("SwagOverFlow.ViewModels.SwagItemBase");

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

            modelBuilder.Entity("SwagOverFlow.WPF.ViewModels.SwagTabItem", b =>
                {
                    b.HasBaseType("SwagOverFlow.ViewModels.SwagItemBase");

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

            modelBuilder.Entity("SwagOverFlow.ViewModels.SwagDataColumn", b =>
                {
                    b.HasBaseType("SwagOverFlow.ViewModels.SwagData");

                    b.Property<string>("AppliedFilter")
                        .HasColumnType("TEXT");

                    b.Property<int>("ColSeq")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ColumnName")
                        .HasColumnType("TEXT");

                    b.Property<string>("DataTypeString")
                        .HasColumnType("TEXT");

                    b.Property<string>("Expression")
                        .HasColumnType("TEXT");

                    b.Property<string>("Header")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsReadOnly")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ReadOnly")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("SwagDataColumn");
                });

            modelBuilder.Entity("SwagOverFlow.ViewModels.SwagDataGroup", b =>
                {
                    b.HasBaseType("SwagOverFlow.ViewModels.SwagData");

                    b.HasDiscriminator().HasValue("SwagDataGroup");
                });

            modelBuilder.Entity("SwagOverFlow.ViewModels.SwagDataRow", b =>
                {
                    b.HasBaseType("SwagOverFlow.ViewModels.SwagData");

                    b.HasDiscriminator().HasValue("SwagDataRow");
                });

            modelBuilder.Entity("SwagOverFlow.ViewModels.SwagSettingBoolean", b =>
                {
                    b.HasBaseType("SwagOverFlow.ViewModels.SwagSetting");

                    b.HasDiscriminator().HasValue("SwagSettingBoolean");
                });

            modelBuilder.Entity("SwagOverFlow.ViewModels.SwagSettingGroup", b =>
                {
                    b.HasBaseType("SwagOverFlow.ViewModels.SwagSetting");

                    b.Property<string>("Name")
                        .HasColumnName("SwagSettingGroup_Name")
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("SwagSettingGroup");
                });

            modelBuilder.Entity("SwagOverFlow.ViewModels.SwagSettingString", b =>
                {
                    b.HasBaseType("SwagOverFlow.ViewModels.SwagSetting");

                    b.HasDiscriminator().HasValue("SwagSettingString");
                });

            modelBuilder.Entity("SwagOverFlow.WPF.ViewModels.SwagTabCollection", b =>
                {
                    b.HasBaseType("SwagOverFlow.WPF.ViewModels.SwagTabItem");

                    b.Property<bool>("IsInitialized")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SelectedIndex")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowChildText")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("SwagTabCollection");
                });

            modelBuilder.Entity("SwagOverFlow.ViewModels.SwagDataTable", b =>
                {
                    b.HasBaseType("SwagOverFlow.ViewModels.SwagDataGroup");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("SwagDataTable");
                });

            modelBuilder.Entity("SwagOverFlow.ViewModels.SwagData", b =>
                {
                    b.HasOne("SwagOverFlow.ViewModels.SwagDataGroup", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("SwagOverFlow.ViewModels.SwagSetting", b =>
                {
                    b.HasOne("SwagOverFlow.ViewModels.SwagSettingGroup", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("SwagOverFlow.WPF.ViewModels.SwagTabItem", b =>
                {
                    b.HasOne("SwagOverFlow.WPF.ViewModels.SwagTabCollection", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });
#pragma warning restore 612, 618
        }
    }
}
