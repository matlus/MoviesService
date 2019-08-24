using MovieService.DomainLayer.Managers.Models;
using System.Collections.Generic;

namespace AcceptanceTests.EqualityComparers
{
    public sealed class MovieEqualityComparer : IEqualityComparer<Movie>
    {
        public bool Equals(Movie x, Movie y)
        {
            return x.Name == y.Name && x.Genre == y.Genre && x.Year == y.Year && x.ImageUrl == y.ImageUrl;
        }

        public int GetHashCode(Movie obj)
        {
            unchecked
            {
                int hash = 17;                
                hash = hash * 23 + obj.Name.GetHashCode();
                hash = hash * 23 + obj.Genre.GetHashCode();
                hash = hash * 23 + obj.Year.GetHashCode();
                hash = hash * 23 + obj.ImageUrl.GetHashCode();
                return hash;
            }
        }
    }
}
