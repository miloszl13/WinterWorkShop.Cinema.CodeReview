namespace WinterWorkShop.Cinema.Domain.Common
{
    public static class Messages
    {
        #region Users

        #endregion

        #region Payments
        public const string PAYMENT_CREATION_ERROR = "Connection error, occured while creating new payment, please try again";
        #endregion

        #region Auditoriums
        public const string AUDITORIUM_GET_ALL_AUDITORIUMS_ERROR = "Error occured while getting all auditoriums, please try again.";
        public const string AUDITORIUM_PROPERTIE_NAME_NOT_VALID = "The auditorium Name cannot be longer than 50 characters.";
        public const string AUDITORIUM_PROPERTIE_SEATROWSNUMBER_NOT_VALID = "The auditorium number of seats rows must be between 1-20.";
        public const string AUDITORIUM_PROPERTIE_SEATNUMBER_NOT_VALID = "The auditorium number of seats number must be between 1-20.";
        public const string AUDITORIUM_CREATION_ERROR = "Error occured while creating new auditorium, please try again.";
        public const string AUDITORIUM_SEATS_CREATION_ERROR = "Error occured while creating seats for auditorium, please try again.";
        public const string AUDITORIUM_SAME_NAME = "Cannot create new auditorium, auditorium with same name alredy exist.";
        public const string AUDITORIUM_UNVALID_CINEMAID = "Cannot create new auditorium, cinema with given cinemaId does not exist.";
        public const string AUDITORIUM_EMPTY_TABLE = "There are currentyl no auditoriums..";
        #endregion

        #region Cinemas
        public const string CINEMA_GET_ALL_CINEMAS_ERROR = "Error occured while getting all cinemas, please try again";
        public const string CINEMA_CREATE_ERROR = "Error occured while creating new cinema, please try again";
        public const string CINEMA_NOT_EXIST = "The cinema you are trying to delete does not exist";
        #endregion

        #region Movies        
        public const string MOVIE_DOES_NOT_EXIST = "Movie does not exist.";
        public const string MOVIE_PROPERTIE_TITLE_NOT_VALID = "The movie title cannot be longer than 50 characters.";
        public const string MOVIE_PROPERTIE_YEAR_NOT_VALID = "The movie year must be between 1895-2100.";
        public const string MOVIE_PROPERTIE_RATING_NOT_VALID = "The movie rating must be between 1-10.";
        public const string MOVIE_CREATION_ERROR = "Error occured while creating new movie, please try again.";
        public const string MOVIE_GET_ALL_CURRENT_MOVIES_ERROR = "Error occured while getting current movies, please try again.";
        public const string MOVIE_GET_BY_ID = "Error occured while getting movie by Id, please try again.";
        public const string MOVIE_GET_ALL_MOVIES_ERROR = "Error occured while getting all movies, please try again.";
        public const string MOVIE_CANNOT_DEACTIVATE_MOVIE = "You cant deactivate this movie because it has projections in future!";
        public const string MOVIE_CANNOT_ACTIVATE_MOVIE = "You cant activate this movie because it does not have any projections!";
        #endregion

        #region Projections
        public const string PROJECTION_GET_ALL_PROJECTIONS_ERROR = "Error occured while getting all projections, please try again.";
        public const string PROJECTION_DOES_NOT_EXIST = "Projection with that id does not exist";
        public const string PROJECTION_CREATION_ERROR = "Error occured while creating new projection, please try again.";
        public const string PROJECTIONS_AT_SAME_TIME = "Cannot create new projection, there are projections at same time alredy.";
        public const string PROJECTION_IN_PAST = "Projection time cannot be in past.";
        public const string PROJECTION_IN_CINEMA_NOT_EXIST = "There are no projections in that cinema.";
        public const string PROJECTION_IN_AUDITORIUM_NOT_EXIST = "There are no projections in that auditorium.";
        public const string PROJECTION_FOR_THAT_MOVIE_NOT_EXIST = "There are no projections for that movie.";
        public const string PROJECTION_FOR_THAT_TIME_NOT_EXIST = "There are no projections that start at that time.";
        #endregion

        #region Seats
        public const string SEAT_GET_ALL_SEATS_ERROR = "Error occured while getting all seats, please try again.";
        public const string SEAT_ALREADY_RESERVED = "You cant reserve seat because it is already reserved!";
        #endregion

        #region User
        public const string USER_NOT_FOUND = "User does not exist.";
        public const string USER_CREATION_ERROR = "Error occured while creating new user, please try again.";
        public const string USER_USERNAME_TOO_LONG = "Username is too short";
        public const string USER_PASSWORD_ERROR = "Password must be between 5-15 characters";

        #endregion
        #region Reservation
        public const string SEATS_ARE_NOT_IN_THE_SAME_ROW = "You can only reserve seats in the same row!If u want you can create multiple reservations and separate them or choose another projection.";
        public const string SEATS_ARE_NOT_ONE_TO_ANOTHER = "You can only reserve seats that are one next to another !If u want you can create multiple reservations and separate them or choose another projection.";
        public const string RESERVATION_ALREADY_EXIST = "Error occured while creating reservation...";
        #endregion

        #region ReservedSeats
        public const string RESERVED_SEAT_DOES_NOT_EXIST = "There are currently no seats reserved";
        #endregion


    }
}
