using System;
using System.Collections.Generic;
using Fashion.Api.Core.Enums;


namespace Fashion.Api.Core.Entities
{
    public class Outfit
    {
        public Guid Id { get; set; }
        public Aesthetic Aesthetic { get; set; }
        public Weather Weather { get; set; }
        public Mood Mood { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}