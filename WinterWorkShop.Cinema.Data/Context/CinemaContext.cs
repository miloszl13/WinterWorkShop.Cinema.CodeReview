using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Data
{
    public class CinemaContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Projection> Projections { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Auditorium> Auditoriums { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservedSeats> ReservedSeats { get; set; }
        public DbSet<MovieTags> MovieTags { get; set; }
        public CinemaContext(DbContextOptions<CinemaContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //composite key
            modelBuilder.Entity<ReservedSeats>().HasKey(rs => new { rs.Reservation_Id,rs.Seat_Id });
            modelBuilder.Entity<MovieTags>().HasKey(mt => new { mt.Tag_Id,mt.Movie_Id });

        }

    }
}
