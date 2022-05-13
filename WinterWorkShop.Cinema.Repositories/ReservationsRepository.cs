using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IReservationRepository : IRepository<Reservation> { }
    public class ReservationsRepository : IReservationRepository
    {
        private CinemaContext _cinemaContext;
        public ReservationsRepository(CinemaContext context)
        {
            _cinemaContext=context;
        }
        public Reservation Delete(object id)
        {
            Reservation existing = _cinemaContext.Reservations.Find(id);
            var result = _cinemaContext.Reservations.Remove(existing).Entity;

            return result;
        }

        public async Task<List<Reservation>> GetAll()
        {
            var data = await _cinemaContext.Reservations.ToListAsync();

            return data;
        }

        public async Task<Reservation> GetByIdAsync(object id)
        {
            return await _cinemaContext.Reservations.FindAsync(id);
        }

        public Reservation Insert(Reservation obj)
        {
            var data = _cinemaContext.Reservations.Add(obj);
            _cinemaContext.SaveChanges();
            return data.Entity;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Reservation Update(Reservation obj)
        {
            var updatedEntry = _cinemaContext.Reservations.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }
    }
}
