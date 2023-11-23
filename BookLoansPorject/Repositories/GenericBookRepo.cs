using BookLoansPorject.Models;
using BookLoansPorject.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BookLoansPorject.Repositories
{
    public class GenericBookRepo<T> : IGenericBookRepo<T> where T : EntityBase
    {

        DbContext db;
        DbSet<T> books;
        public GenericBookRepo(DbContext db)
        {
            this.db = db;
            this.books = db.Set<T>();
        }

        public T Get(int id, string include = "")
        {
            if (include == "")
                return books.FirstOrDefault(x => x.Id == id);
            else
                return books.Include(include).FirstOrDefault(x => x.Id == id);
        }
        public IEnumerable<T> GetAll(string include = "")
        {
            if (include == "")
                return books.ToList();
            else
                return books.Include(include).ToList();
        }
        public void Insert(T item)
        {
            books.Add(item);
            db.SaveChanges();
        }

        public void Update(T item)
        {
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void Delete(int id)
        {
            var item = books.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                books.Remove(item);
                db.SaveChanges();
            }
        }
        public void ExecuteCommand(string sql)
        {
            db.Database.ExecuteSqlCommand(sql);
        }

        public IEnumerable<K> ExecuteSqlCollection<K>(string sql) where K : EntityBase
        {
            return db.Database.SqlQuery<K>(sql).ToList();
        }

        public K ExecuteSqlSingle<K>(string sql) where K : EntityBase
        {
            return db.Database.SqlQuery<K>(sql).FirstOrDefault();

        }


        public void DeleteRange(IEnumerable<T> items)
        {
            books.RemoveRange(items);
            db.SaveChanges();
        }
    }
}