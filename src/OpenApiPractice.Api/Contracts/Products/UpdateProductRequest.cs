using System.ComponentModel.DataAnnotations;

namespace OpenApiPractice.Api.Contracts.Products;
public sealed record UpdateProductRequest
{
    private readonly string _name = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public required string Name
    {
        get => _name;
        init => _name = value?.Trim() ?? string.Empty;
    }

    [Range(0.01, 999_999.99)]
    public required decimal Price { get; init; }
}
