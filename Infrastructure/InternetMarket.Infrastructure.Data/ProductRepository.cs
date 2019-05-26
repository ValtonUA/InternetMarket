using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternetMarket.Domain.Core;
using InternetMarket.Domain.Interfaces;
using System.Data.Entity;

namespace InternetMarket.Infrastructure.Data
{
    public class ProductRepository : IRepository<Product>
    {
        private ProductContext db;

        public ProductRepository()
        {
            db = new ProductContext();
        }

        public void Create(Product item)
        {
            db.Products.Add(item);
        }

        public void CreateCategory(Category item)
        {
            db.Categories.Add(item);
        }

        public void Delete(string key)
        {
            Product product = db.Products.Find(key);
            if (product != null)
                db.Products.Remove(product);
        }

        public void DeleteCategory(string key)
        {
            Category category = db.Categories.Find(key);
            if (category != null)
                db.Categories.Remove(category);
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<Product> Get()
        {
            return db.Products;
        }

        public IEnumerable<Category> GetCategories()
        {
            return db.Categories;
        }

        public Product Get(string key)
        {
            return db.Products.Find(key);
        }

        public Category GetCategory(string key)
        {
            return db.Categories.Find(key);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public void Update(Product item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void UpdateCategory(Category item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
    }
}
