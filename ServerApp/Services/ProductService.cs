using ServerApp.Models;

public class ProductService
{
    private static List<Product> _products = new List<Product>
    {
        new Product { Id = 1, Name = "Laptop", Price = 999.99M , Stock = 10, Description = "A high-performance laptop.", Categories = new List<Category> { new Category { Id = 1, Name = "Electronics" } } },
        new Product { Id = 2, Name = "Phone", Price = 499.99M , Description = "A smartphone with a great camera.", Stock = 25, Categories = new List<Category> { new Category { Id = 1, Name = "Electronics" }, new Category { Id = 2, Name = "Mobile Devices" } } },
        new Product { Id = 3, Name = "Headphones", Price = 199.99M , Stock = 15, Description = "Noise-cancelling over-ear headphones.", Categories = new List<Category> { new Category { Id = 1, Name = "Electronics" }, new Category { Id = 3, Name = "Audio" } } }
    };

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult(_products.AsEnumerable());
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
        return Task.FromResult(true);
    }
}