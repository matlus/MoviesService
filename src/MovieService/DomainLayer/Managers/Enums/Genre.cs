using System;

namespace MovieService.DomainLayer.Managers.Enums
{
    public enum Genre
    {
        None,
        Action,
        Animation,
        Comedy,
        Drama,
        Musical,
        [EnumDescription("Sci-Fi")]
        [EnumDescription("SciFi")]
        SciFi,
        Thriller
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnumDescriptionAttribute : Attribute
    {
        private readonly string _description;
        public string Description { get { return _description; } }
        public EnumDescriptionAttribute(string description)
        {
            _description = description;
        }
    }
}
