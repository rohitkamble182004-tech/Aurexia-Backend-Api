using System;
using System.Collections.Generic;

namespace Fashion.Api.Core.Entities
{
    public class Product
    {
        internal object Images;

        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public ICollection<Drop> Drops { get; set; }
            = new List<Drop>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ProductVariant> Variants { get; set; }
            = new List<ProductVariant>();
    }
}
