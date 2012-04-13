using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FluentNHibernateMvc3.Models.Data
{
    /// <summary>
    /// http://www.bengtbe.com/blog/2009/10/08/nerddinner-with-fluent-nhibernate-part-3-the-infrastructure
    /// </summary>
    public interface IRepository<T>
    {
        IQueryable<T> GetAll();
        IQueryable<T> Get( Expression<Func<T, bool>> predicate );
        IEnumerable<T> SaveOrUpdateAll( params T[] entities );
        T SaveOrUpdate( T entity );
    }
}