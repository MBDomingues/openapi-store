using System.Collections.Concurrent;
using OpenApiPractice.Api.Models;

namespace OpenApiPractice.Api.Services;

public sealed class ProductService
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();

    public ProductService()
    {
        var example = new Product(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Mouse sem fio",
            129.90m,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);

        _products[example.Id] = example;
    }

    public Product? Update(Guid id, string newName, decimal price)
    {
        var product = this.GetById(id);

        if(product == null)
            return null;

        var newProduct = new Product(
            id,
            newName.Trim(),
            price,
            product.CreatedAt,
            DateTimeOffset.UtcNow);
        
        _products[id] = newProduct;

        return newProduct;
    }

    public IReadOnlyCollection<Product> GetAll() =>
        _products.Values
            .OrderBy(product => product.Name)
            .ToArray();

    public Product? GetById(Guid id) =>
        _products.GetValueOrDefault(id);

    public Product Create(string name, decimal price)
    {
        var product = new Product(
            Guid.NewGuid(),
            name.Trim(),
            price,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);

        _products[product.Id] = product;
        return product;
    }
}
