﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Request.Infrastructure.Data;

#nullable disable

namespace Request.Infrastructure.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220810021108_InitUpdate")]
    partial class InitUpdate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Request.Domain.Entities.Requests.LeaveRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CompensationDayEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CompensationDayStart")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DayOffEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DayOffStart")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<Guid>("RequestorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StatusId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RequestorId");

                    b.HasIndex("StatusId");

                    b.ToTable("LeaveRequests");
                });

            modelBuilder.Entity("Request.Domain.Entities.Requests.Stage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<Guid?>("LeaveRequestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("LeaveRequestId");

                    b.HasIndex("UserId");

                    b.ToTable("Stages");
                });

            modelBuilder.Entity("Request.Domain.Entities.Requests.Status", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Statuses");
                });

            modelBuilder.Entity("Request.Domain.Entities.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Request.Domain.Entities.Requests.LeaveRequest", b =>
                {
                    b.HasOne("Request.Domain.Entities.Users.User", "Requestor")
                        .WithMany("LeaveRequests")
                        .HasForeignKey("RequestorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Request.Domain.Entities.Requests.Status", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Requestor");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("Request.Domain.Entities.Requests.Stage", b =>
                {
                    b.HasOne("Request.Domain.Entities.Requests.LeaveRequest", "LeaveRequest")
                        .WithMany("States")
                        .HasForeignKey("LeaveRequestId");

                    b.HasOne("Request.Domain.Entities.Users.User", "User")
                        .WithMany("States")
                        .HasForeignKey("UserId");

                    b.Navigation("LeaveRequest");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Request.Domain.Entities.Requests.LeaveRequest", b =>
                {
                    b.Navigation("States");
                });

            modelBuilder.Entity("Request.Domain.Entities.Users.User", b =>
                {
                    b.Navigation("LeaveRequests");

                    b.Navigation("States");
                });
#pragma warning restore 612, 618
        }
    }
}