using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IMovieTagsRepository
    {
        List<MovieTags> GetAllMovieTags();
    }
    public class MovieTagsRepository : IMovieTagsRepository
    {
        private readonly CinemaContext _cinemaContext;
        public MovieTagsRepository(CinemaContext cinemacontext)
        {
            _cinemaContext= cinemacontext;
        }
        public List<MovieTags> GetAllMovieTags()
        {
            return _cinemaContext.MovieTags.ToList();
        }
    }
}
