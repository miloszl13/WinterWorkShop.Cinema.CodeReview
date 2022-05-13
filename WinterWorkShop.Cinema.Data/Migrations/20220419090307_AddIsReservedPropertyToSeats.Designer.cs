﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WinterWorkShop.Cinema.Data;

#nullable disable

namespace WinterWorkShop.Cinema.Data.Migrations
{
    [DbContext(typeof(CinemaContext))]
    [Migration("20220419090307_AddIsReservedPropertyToSeats")]
    partial class AddIsReservedPropertyToSeats
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Auditorium", b =>
                {
                    b.Property<int>("Auditorium_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Auditorium_Id"));

                    b.Property<string>("AuditName")
                        .HasColumnType("text");

                    b.Property<int>("Cinema_Id")
                        .HasColumnType("integer");

                    b.HasKey("Auditorium_Id");

                    b.HasIndex("Cinema_Id");

                    b.ToTable("Auditoriums");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Cinema", b =>
                {
                    b.Property<int>("Cinema_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Cinema_Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Cinema_Id");

                    b.ToTable("Cinemas");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Entities.Reservation", b =>
                {
                    b.Property<int>("Reservation_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Reservation_Id"));

                    b.Property<Guid>("User_Id")
                        .HasColumnType("uuid");

                    b.HasKey("Reservation_Id");

                    b.HasIndex("User_Id");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Movie", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Current")
                        .HasColumnType("boolean");

                    b.Property<double?>("Rating")
                        .HasColumnType("double precision");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Projection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Auditorium_Id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Movie_Id")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Auditorium_Id");

                    b.HasIndex("Movie_Id");

                    b.ToTable("Projections");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Seat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Auditorium_Id")
                        .HasColumnType("integer");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<int>("Reservation_Id")
                        .HasColumnType("integer");

                    b.Property<int>("Row")
                        .HasColumnType("integer");

                    b.Property<bool>("isReserver")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("Auditorium_Id");

                    b.HasIndex("Reservation_Id");

                    b.ToTable("Seats");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.User", b =>
                {
                    b.Property<Guid>("User_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("User_Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Auditorium", b =>
                {
                    b.HasOne("WinterWorkShop.Cinema.Data.Cinema", "Cinema")
                        .WithMany("Auditoriums")
                        .HasForeignKey("Cinema_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cinema");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Entities.Reservation", b =>
                {
                    b.HasOne("WinterWorkShop.Cinema.Data.User", "User")
                        .WithMany("Reservations")
                        .HasForeignKey("User_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Projection", b =>
                {
                    b.HasOne("WinterWorkShop.Cinema.Data.Auditorium", "Auditorium")
                        .WithMany("Projections")
                        .HasForeignKey("Auditorium_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WinterWorkShop.Cinema.Data.Movie", "Movie")
                        .WithMany("Projections")
                        .HasForeignKey("Movie_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Auditorium");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Seat", b =>
                {
                    b.HasOne("WinterWorkShop.Cinema.Data.Auditorium", "Auditorium")
                        .WithMany("Seats")
                        .HasForeignKey("Auditorium_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WinterWorkShop.Cinema.Data.Entities.Reservation", "Reservation")
                        .WithMany("Seats")
                        .HasForeignKey("Reservation_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Auditorium");

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Auditorium", b =>
                {
                    b.Navigation("Projections");

                    b.Navigation("Seats");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Cinema", b =>
                {
                    b.Navigation("Auditoriums");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Entities.Reservation", b =>
                {
                    b.Navigation("Seats");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.Movie", b =>
                {
                    b.Navigation("Projections");
                });

            modelBuilder.Entity("WinterWorkShop.Cinema.Data.User", b =>
                {
                    b.Navigation("Reservations");
                });
#pragma warning restore 612, 618
        }
    }
}
