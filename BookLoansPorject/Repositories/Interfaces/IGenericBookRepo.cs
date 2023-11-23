using BookLoansPorject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLoansPorject.Repositories.Interfaces
{
    public interface IGenericBookRepo<T> where T : EntityBase
    {
        IEnumerable<T> GetAll(string include = "");
        T Get(int id, string include = "");
        void Insert(T item);
        void Update(T item);
        void Delete(int id);
        void DeleteRange(IEnumerable<T> items);
        K ExecuteSqlSingle<K>(string sql) where K : EntityBase;
        IEnumerable<K> ExecuteSqlCollection<K>(string sql) where K : EntityBase;
        void ExecuteCommand(string sql);
    }
}
