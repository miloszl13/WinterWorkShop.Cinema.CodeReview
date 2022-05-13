using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Authent;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _usersRepository;

        public UserService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<UserDomainModel> AddUser(UserDomainModel newUser)
        {
            User userToCreate = new User()
            {
                FirstName = newUser.FirstName,
                LastName= newUser.LastName,
                UserName=newUser.UserName,
                Password=newUser.Password,
                Role=newUser.Role,
                //IsAdmin=newUser.IsAdmin,
                BonusPoints=newUser.BonusPoints ?? 0
            };

            var data = _usersRepository.Insert(userToCreate);
            if (data == null)
            {
                return null;
            }

            _usersRepository.Save();

            UserDomainModel domainModel = new UserDomainModel()
            {
                Id = data.User_Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                Role=data.Role,
                //IsAdmin = data.IsAdmin,
                BonusPoints=data.BonusPoints
            };

            return domainModel;
        }

        public async Task<List<UserDomainModel>> GetAllAsync()
        {
            var data = await _usersRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<UserDomainModel> result = new List<UserDomainModel>();
            UserDomainModel model;
            foreach (var item in data)
            {
                model = new UserDomainModel
                {
                    Id = item.User_Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    UserName = item.UserName,
                    Role=item.Role,
                    //IsAdmin = item.IsAdmin,
                    BonusPoints = item.BonusPoints
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<UserDomainModel> GetUserByIdAsync(Guid id)
        {
            var data = await _usersRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            UserDomainModel domainModel = new UserDomainModel
            {
                Id = data.User_Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                Role=data.Role,
                //IsAdmin = data.IsAdmin,
                BonusPoints=data.BonusPoints
            };

            return domainModel;
        }

        public async Task<UserDomainModel> GetUserByUserName(string username)
        {
            var data = _usersRepository.GetByUserName(username);

            if (data == null)
            {
                return null;
            }

            UserDomainModel domainModel = new UserDomainModel
            {
                Id = data.User_Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                //IsAdmin = data.IsAdmin,
                Role= data.Role,
                BonusPoints=data.BonusPoints
            };

            return domainModel;
        }
        public ActionResult<bool> AddBonusPoint(Guid userId)
        {
            var user=_usersRepository.GetByIdAsync(userId).Result;
            if (user == null)
            {
                var responseModel = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.USER_NOT_FOUND,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return new NotFoundObjectResult(responseModel);
            }
            user.BonusPoints += 1;
            if (user.BonusPoints == 10)
            {
                user.Role = "super-user";
                _usersRepository.Update(user);
                return true;
            }
            _usersRepository.Update(user);
            return true;
        }

        public UserModel Authenticate(UserLogin userlogin)
        {
            var currentUser = _usersRepository.GetByUsernameAndPass(userlogin.Username, userlogin.Password);
            if (currentUser != null)
            {
                var resultUser = new UserModel()
                {
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Password = currentUser.Password,
                    UserName = currentUser.UserName,
                    Role = currentUser.Role
                };
                return resultUser;
            }
            return null;
        }

    }
}
