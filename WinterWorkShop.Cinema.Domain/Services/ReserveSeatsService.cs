using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class ReserveSeatsService : IReserveSeatsService
    {
        private ISeatsRepository _seatRepository;
        private IReservationRepository _reservationRepository;
        private IProjectionsRepository _projectionsRepository;
        private IReservedSeatsRepository _reservedSeatsRepository;
        private ILevi9PaymentService _payment;
        private IUserService _userService;

        public ReserveSeatsService(ISeatsRepository seatRepository,
            IReservationRepository reservationRepository ,IProjectionsRepository projectionsRepository,
            IReservedSeatsRepository reservedSeatsRepository, ILevi9PaymentService payment,IUserService userService)
        {
            _seatRepository = seatRepository;
            _reservationRepository = reservationRepository;
            _projectionsRepository = projectionsRepository;
            _reservedSeatsRepository = reservedSeatsRepository;
            _payment=payment;
            _userService=userService;
        }

        public ActionResult<bool> ReserveSeat(int idReservation, List<Guid> seats, Guid user_id, Guid projection_id)
        {
            List<Seat> seatList = new List<Seat>();
            List<Seat> SeatsDb = _seatRepository.GetAll().Result.ToList();  //all seats from db
            if (SeatsDb.Count == 0)
            {
                var errorResponse = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.SEAT_GET_ALL_SEATS_ERROR,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return new NotFoundObjectResult(errorResponse);
            }
            //store seats that we need (by id) in seatList
            foreach (Guid g in seats)
            {
                //find seat by seat id
                var seat = SeatsDb.FirstOrDefault(x => x.Seat_Id == g);
                //add that seat to list
                seatList.Add(seat);
            }

            //sort list by seat number
            seatList = seatList.OrderBy(x => x.Number).ToList();
            //check if seats are in the same row and one next to another
            var areValidSeats = validateSeatsRowAndNumber(seatList);
            if (areValidSeats.Value != true)
            {
                return areValidSeats;
            }

            //find projection by id
            var projection = _projectionsRepository.GetByIdAsync(projection_id).Result;
            if (projection == null)
            {
                var errorResponse = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.PROJECTION_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return new NotFoundObjectResult(errorResponse);
            }

            //calculate reservation price
            var projectionPrice = projection.Price;
            var reservationPrice = projectionPrice * Convert.ToDouble(seatList.Count());

            //check if any of seats is already reserved in that time
            var areReserved = CheckSeatsReservations(seatList, idReservation, projection);
            if (areReserved.Value != true)
            {
                return areReserved;
            }
            var paymentResponse=_payment.MakePayment().Result;
            if (paymentResponse.IsSuccess == true)
            {
                try
                {


                    //kreiramo rezervaciju
                    var reservation = new Reservation()
                    {
                        Projection_Id = projection.Projection_Id,
                        User_Id = user_id,
                        Reservation_Id = idReservation,
                        Reservation_Price = reservationPrice
                    };
                    _reservationRepository.Insert(reservation);

                    //kreiramo rezervaciju stolice za svaku stolicu posebno
                    foreach (Seat seat in seatList)
                    {
                        var reserveSeat = new ReservedSeats()
                        {
                            Reservation_Id = idReservation,
                            Seat_Id = seat.Seat_Id,
                            DateTime = projection.DateTime
                        };
                        _reservedSeatsRepository.Insert(reserveSeat);
                    }
                    _userService.AddBonusPoint(user_id);
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorResponseModels errorResponse = new ErrorResponseModels
                    {
                        ErrorMessage = ex.InnerException.Message ?? ex.Message,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };

                    return new BadRequestObjectResult(errorResponse);
                }
            }
            else
            {
                var errorResponse = new ErrorResponseModels()
                {
                    ErrorMessage = paymentResponse.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return new BadRequestObjectResult(errorResponse);
            }
        }

        private ActionResult<bool> validateSeatsRowAndNumber(List<Seat> seats)
        {
            var row = seats[0].Row;

            for (int i = 1; i < seats.Count; i++) //check if all seats are in same row
            {
                //if they are not in the same in row return bad request object result
                if (seats[i].Row != row)
                {
                    var errorResponse = new ErrorResponseModels()
                    {
                        ErrorMessage = Messages.SEATS_ARE_NOT_IN_THE_SAME_ROW,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return new BadRequestObjectResult(errorResponse);
                }
                //because seats are one next to another ,every next seats number should be +1 from last
                //otherwise they are not one to another
                //else if (seatList[i].Number != seatNumber + 1)
                else if (seats[i].Number != seats[i - 1].Number + 1)
                {
                    var errorResponse = new ErrorResponseModels()
                    {
                        ErrorMessage = Messages.SEATS_ARE_NOT_ONE_TO_ANOTHER,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return new BadRequestObjectResult(errorResponse);
                }
            }
            return true;
        }
        private ActionResult<bool> CheckSeatsReservations(List<Seat> seats,int idReservation,Projection projection)
        {
            //check if any of seats is already reserved in that time
            foreach (Seat s in seats)
            {
                var reservedSeatDb = _reservedSeatsRepository.GetAll().Result
                .FirstOrDefault(rs => rs.Reservation_Id == idReservation && rs.Seat_Id == s.Seat_Id && rs.DateTime == projection.DateTime);
                if (reservedSeatDb != null)
                {
                    var errorResponse = new ErrorResponseModels()
                    {
                        ErrorMessage = Messages.SEAT_ALREADY_RESERVED,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return new BadRequestObjectResult(errorResponse);

                }
            }
            return true;
        }

        public ActionResult<List<ReserveSeatDomainModel>> GetAllReservedSeats()
        {
            var data =_reservedSeatsRepository.GetAll().Result;

            if (data.Count==0)
            {
                var errorResponse = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.RESERVED_SEAT_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return new NotFoundObjectResult(errorResponse);
            }

            List<ReserveSeatDomainModel> result = new List<ReserveSeatDomainModel>();
            ReserveSeatDomainModel model;
            foreach (var item in data)
            {
                model = new ReserveSeatDomainModel
                {
                   Reservation_Id= item.Reservation_Id,
                   Seat_Id= item.Seat_Id,
                   DateTime = item.DateTime
                };
                result.Add(model);
            }

            return new OkObjectResult(result);
        }
    }
}
