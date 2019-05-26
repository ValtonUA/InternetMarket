using System;
using System.Collections.Generic;
using System.Data.Entity;
using InternetMarket.Domain.Core;
using InternetMarket.Domain.Interfaces;

namespace InternetMarket.Infrastructure.Data
{
    public class OrderRepository : IRepository<Order>
    {
        public OrderContext db;

        public OrderRepository()
        {
            db = new OrderContext();
        }

        public void Create(Order item)
        {
            db.Orders.Add(item);
        }

        public void CreateStatus(Status item)
        {
            db.Statuses.Add(item);
        }

        public void Delete(string key)
        {
            int id;
            if (int.TryParse(key, out id))
            {
                Order order = db.Orders.Find(id);
                if (order != null)
                    db.Orders.Remove(order);
            }
        }

        public void DeleteStatus(string key)
        {
            Status status = db.Statuses.Find(key);
            if (status != null)
                db.Statuses.Remove(status);
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

        public IEnumerable<Order> Get()
        {
            return db.Orders;
        }

        public IEnumerable<Status> GetStatuses()
        {
            return db.Statuses;
        }

        public Order Get(string key)
        {
            int id;
            if (int.TryParse(key, out id))
            {
                return db.Orders.Find(id);
            }

            return null;
        }

        public Status GetStatus(string key)
        {
            return db.Statuses.Find(key);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public void Update(Order item) 
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void UpdateStatus(Status item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
    }
}
