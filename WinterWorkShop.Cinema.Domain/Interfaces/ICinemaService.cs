using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ICinemaService
    {
        Task<List<CinemaDomainModel>> GetAllAsync();
        ActionResult<bool> Create(CinemaDomainModel cinemaModel);
        ActionResult<bool> Delete(int CinemaId);
    }
}
