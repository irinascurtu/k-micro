﻿using Microsoft.EntityFrameworkCore;
using ProductsApi.Data.Entities;
using ProductsApi.Data.Entities.Extensions;

namespace ProductsApi.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _context;

        public ProductRepository(ProductContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            // _context.Entry(product).State = EntityState.Modified;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(e => e.Id == id);
        }

        public async Task<IQueryable<Product>> GetProductsAsync(int? categoryId)
        {
            return  _context.Products.Where(p => p.CategoryId == categoryId);
        }


        public async Task<IEnumerable<ProductStock>> GetProductStocksAsync(List<int> productIds)
        {
            var stocks = await _context.Products.Where(p => productIds.Contains(p.Id)).Select(x => new ProductStock
            {
                Stock = x.Stock,
                ProductId = x.Id
            }).ToListAsync();
            return stocks;
        }
    }
}
