using System;
using System.Collections.Generic;


namespace Fashion.Api.Core.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}