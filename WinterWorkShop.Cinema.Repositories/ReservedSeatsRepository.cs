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
    public interface IReservedSeatsRepository
    {
        Task<List<ReservedSeats>> GetAll();
        ReservedSeats Insert(ReservedSeats obj);
    }
    public class ReservedSeatsRepository : IReservedSeatsRepository
    {
        private CinemaContext _cinemaContext;
        public ReservedSeatsRepository(CinemaContext context)
        {
            _cinemaContext=context;
        }    

        public async Task<List<ReservedSeats>> GetAll()
        {
            var data = await _cinemaContext.ReservedSeats.ToListAsync();

            return data;
        }     
        public ReservedSeats Insert(ReservedSeats obj)
        {
            var data = _cinemaContext.ReservedSeats.Add(obj).Entity;
            _cinemaContext.SaveChanges();
            return data;
        }       
    }
}
