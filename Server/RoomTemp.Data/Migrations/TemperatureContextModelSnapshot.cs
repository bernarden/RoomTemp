﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RoomTemp.Data;

namespace RoomTemp.Data.Migrations
{
    [DbContext(typeof(TemperatureContext))]
    partial class TemperatureContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity("RoomTemp.Data.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("Key");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Device");
                });

            modelBuilder.Entity("RoomTemp.Data.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("RoomTemp.Data.Sensor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Sensor");
                });

            modelBuilder.Entity("RoomTemp.Data.TempReading", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DeviceId");

                    b.Property<int>("LocationId");

                    b.Property<int>("SensorId");

                    b.Property<DateTime>("TakenAt");

                    b.Property<decimal>("Temperature");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("LocationId");

                    b.HasIndex("SensorId");

                    b.ToTable("TempReading");
                });

            modelBuilder.Entity("RoomTemp.Data.TempReading", b =>
                {
                    b.HasOne("RoomTemp.Data.Device", "Device")
                        .WithMany("TempReadings")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RoomTemp.Data.Location", "Location")
                        .WithMany("TempReadings")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RoomTemp.Data.Sensor", "Sensor")
                        .WithMany("TempReadings")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
