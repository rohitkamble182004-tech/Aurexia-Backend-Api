using System;
using System.Collections.Generic;

namespace Fashion.Api.Core.Entities
{
    public class Drop
    {
        public Guid Id { get; set; }

        // ✅ NAV LABEL
        public string Name { get; set; } = string.Empty;

        // ✅ URL
        public string Slug { get; set; } = string.Empty;

        // ✅ VISIBILITY
        public bool IsActive { get; set; } = true;

        // ✅ NAV + HOME ORDER
        public int SortOrder { get; set; }

        // ✅ SHOW / HIDE FROM NAV
        public bool ShowInNav { get; set; } = true;

        // ✅ EDITORIAL FLAG
        public bool IsEditorial { get; set; } = false;

        // ✅ EDITORIAL COPY (OPTIONAL)
        public string? Subtitle { get; set; }

        // =============================
        // PARENT → CHILD RELATIONSHIP
        // =============================

        // Parent drop (nullable for top-level)
        public Guid? ParentId { get; set; }
        public Drop? Parent { get; set; }

        // Child drops (dropdown items)
        public ICollection<Drop> Children { get; set; }
            = new List<Drop>();

        // =============================
        // PRODUCTS (FOR DROP PAGE)
        // =============================
        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
