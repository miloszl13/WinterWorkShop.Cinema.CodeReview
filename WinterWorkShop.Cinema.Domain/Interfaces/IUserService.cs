using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Authent;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of Users</returns>
        Task<List<UserDomainModel>> GetAllAsync();

        /// <summary>
        /// Get a user by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User</returns>
        Task<UserDomainModel> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>User</returns>
        Task<UserDomainModel> GetUserByUserName(string username);
        Task<UserDomainModel> AddUser(UserDomainModel newUser);
        ActionResult<bool> AddBonusPoint(Guid userId);
        UserModel Authenticate(UserLogin userlogin);


    }
}
