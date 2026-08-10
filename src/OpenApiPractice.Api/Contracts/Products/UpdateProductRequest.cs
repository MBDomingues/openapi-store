namespace OpenApiPractice.Api.Contracts.Products;
public sealed record UpdateProductRequest
{
    public required string Name { get; init; }
    public required decimal Price { get; init; }
}