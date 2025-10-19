using ServerApp.Models;
using Microsoft.Extensions.Caching.Memory;

public class ProductService
{
    private const string CacheKey_AllProducts = "AllProducts";
    private static List<Product> _products = new List<Product>
    {
        new Product { Id = 1, Name = "Laptop", Price = 999.99M , Stock = 10, Description = "A high-performance laptop.", Categories = new List<Category> { new Category { Id = 1, Name = "Electronics" } } },
        new Product { Id = 2, Name = "Phone", Price = 499.99M , Description = "A smartphone with a great camera.", Stock = 25, Categories = new List<Category> { new Category { Id = 1, Name = "Electronics" }, new Category { Id = 2, Name = "Mobile Devices" } } },
        new Product { Id = 3, Name = "Headphones", Price = 199.99M , Stock = 15, Description = "Noise-cancelling over-ear headphones.", Categories = new List<Category> { new Category { Id = 1, Name = "Electronics" }, new Category { Id = 3, Name = "Audio" } } }
    };

    private readonly IMemoryCache _cache;

    public ProductService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        // Try get from cache first
        if (_cache.TryGetValue(CacheKey_AllProducts, out IEnumerable<Product>? cached) && cached != null)
        {
            return Task.FromResult((IEnumerable<Product>)cached);
        }

        // Not in cache - add with sliding expiration and absolute expiration
        var products = _products.AsEnumerable();
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

        _cache.Set(CacheKey_AllProducts, products, cacheEntryOptions);

        return Task.FromResult(products);
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task<Product> CreateAsync(Product product)
    {
        product.Id = _products.Max(p => p.Id) + 1;
        _products.Add(product);

        // Invalidate cache when data changes
        _cache.Remove(CacheKey_AllProducts);

        return Task.FromResult(product);
    }

    public Task<bool> UpdateAsync(int id, Product updatedProduct)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return Task.FromResult(false);
        }
        product.Name = updatedProduct.Name;
        product.Price = updatedProduct.Price;
        product.Description = updatedProduct.Description;
        product.Stock = updatedProduct.Stock;

        // Invalidate cache when data changes
        _cache.Remove(CacheKey_AllProducts);

        return Task.FromResult(true);
    }
}