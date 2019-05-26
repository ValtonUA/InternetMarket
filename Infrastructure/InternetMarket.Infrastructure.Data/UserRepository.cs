using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternetMarket.Domain.Core;
using InternetMarket.Domain.Interfaces;

namespace InternetMarket.Infrastructure.Data
{
    public class UserRepository : IRepository<User>
    {
        private UserContext db;

        public UserRepository()
        {
            db = new UserContext();
        }

        public void Create(User item)
        {
            db.Users.Add(item);
        }

        public void CreateRole(Role item)
        {
            db.Roles.Add(item);
        }

        public void Delete(string login)
        {
            User user = db.Users.Find(login);
            if (user != null)
                db.Users.Remove(user);
        }

        public void DeleteRole(string roleName)
        {
            User user = db.Users.Find(roleName);
            if (user != null)
                db.Users.Remove(user);
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

        public IEnumerable<User> Get()
        {
            return db.Users;
        }

        public IEnumerable<Role> GetRoles()
        {
            return db.Roles;
        }

        public User Get(string login)
        {
            return db.Users.Find(login);
        }

        public Role GetRole(string roleName)
        {
            return db.Roles.Find(roleName);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public void Update(User item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void UpdateRole(Role item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
    }
}
