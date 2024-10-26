﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.Bussiness.Infrastructure
{
    public interface IBaseRepository<T> where T : class
    {
        void Add(T entity);
        void Add(IEnumerable<T> entities);
        void Update(T entity);
        void Update(List<T> entity);
        void Delete(T entity, bool isHardDelete = false);
        void Delete(int id);  // Changed Guid to int
        void Delete(Expression<Func<T, bool>> where, bool isHardDelete = false);

        void DeleteRange(IEnumerable<T> entities, bool isHardDelete = false);  // New method

        IQueryable<T> GetQuery();
        IQueryable<T> GetQuery(Expression<Func<T, bool>> where);
        T GetById(int id);  // Changed Guid to int
        Task<T?> GetByIdAsync(int id);  // Changed Guid to int
    }
}
