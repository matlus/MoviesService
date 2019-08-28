using MovieService.DomainLayer.Exceptions;
using MovieService.DomainLayer.Managers.Enums;
using System.Collections.Generic;
using System.Reflection;

namespace MovieService.DomainLayer.Managers.Parsers
{
    internal static class GenreParser
    {
        private static FieldInfo[] s_genreFieldInfos = typeof(Genre).GetFields(BindingFlags.Public | BindingFlags.Static);
        private static Dictionary<string, Genre> stringToGenreMappings = new Dictionary<string, Genre>();
        private static Dictionary<Genre, string> genreToStringMappings = new Dictionary<Genre, string>();

        static GenreParser()
        {
            foreach (var fieldInfo in s_genreFieldInfos)
            {
                var genre = (Genre)fieldInfo.GetValue(null);

                var enumDescriptionAttributes = (EnumDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute));

                if (enumDescriptionAttributes.Length == 0)
                {                    
                    stringToGenreMappings.Add(fieldInfo.Name.ToLower(), genre);
                    genreToStringMappings.Add(genre, fieldInfo.Name);
                }
                else
                {
                    genreToStringMappings.Add(genre, enumDescriptionAttributes[0].Description);

                    foreach (var enumDescAttribute in enumDescriptionAttributes)
                    {
                        stringToGenreMappings.Add(enumDescAttribute.Description.ToLower(), genre);
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

            var genre = genreAsString.ToLower();
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
