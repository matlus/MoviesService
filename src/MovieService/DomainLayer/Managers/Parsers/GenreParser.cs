using MovieService.DomainLayer.Exceptions;
using MovieService.DomainLayer.Managers.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MovieService.DomainLayer.Managers.Parsers
{
    internal static class GenreParser
    {
        private static FieldInfo[] s_genreFieldInfos = typeof(Genre).GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static);
        private static Dictionary<string, Genre> stringToGenreMappings = new Dictionary<string, Genre>();
        private static Dictionary<Genre, string> genreToStringMappings = new Dictionary<Genre, string>();

        static GenreParser()
        {
            foreach (var fieldInfo in s_genreFieldInfos)
            {
                var genre = (Genre)fieldInfo.GetValue(null);

                var enumDescriptionAttributes = (EnumDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute));

                if (!enumDescriptionAttributes.Any())
                {                    
                    stringToGenreMappings.Add(fieldInfo.Name.ToUpper(), genre);
                    genreToStringMappings.Add(genre, fieldInfo.Name);
                }
                else
                {
                    foreach (var enumDescAttribute in enumDescriptionAttributes)
                    {
                        stringToGenreMappings.Add(enumDescAttribute.Description.ToUpper(), genre);

                        if (!genreToStringMappings.ContainsKey(genre))
                        {
                            genreToStringMappings.Add(genre, enumDescAttribute.Description);
                        }
                    }
                }
            }
        }

        public static Genre Parse(string genreAsString)
        {
            if (genreAsString == null || genreAsString.Length == 0)
            {
                throw new InvalidGenreException($"A null or Empty genre is not valid. Valid values are: {string.Join(", ", GetGenreValues())}");
            }

            var genre = genreAsString.ToUpper();
            if (stringToGenreMappings.ContainsKey(genre))
            {
                return stringToGenreMappings[genre];
            }
            else
            {
                throw new InvalidGenreException($"The genre: {genreAsString}, is not a valid Genre. Valid values are: {string.Join(", ", GetGenreValues())}");
            }
        }

        public static IEnumerable<string> GetGenreValues()
        {
            foreach (var kvp in genreToStringMappings)
            {
                yield return kvp.Value;
            }
        }

        public static string ToString(Genre genre)
        {
            return genreToStringMappings[genre];
        }
    }
}
