using System;


namespace Fashion.Api.Core.Entities
{
    public class ProductTag
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}