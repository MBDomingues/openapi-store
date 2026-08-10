using System.ComponentModel.DataAnnotations;

namespace OpenApiPractice.Api.Contracts.Products;

public sealed record CreateProductRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public required string Name { get; init; }

    [Range(0.01, 999_999.99)]
    public decimal Price { get; init; }
}
