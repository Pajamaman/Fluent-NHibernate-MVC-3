using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NHibernate;
using NHibernate.Linq;

namespace FluentNHibernateMVC3.Models.Data
{
    /// <summary>
    /// http://www.bengtbe.com/blog/2009/10/08/nerddinner-with-fluent-nhibernate-part-3-the-infrastructure
    /// </summary>
    public class Repository<T> : IRepository<T>
    {
        public ISession Session
        {
            get { return NHibernateSessionPerRequest.GetCurrentSession(); }
        }

        public IQueryable<T> GetAll()
        {
            return Session.Query<T>();
        }

        public IQueryable<T> Get( Expression<Func<T, bool>> predicate )
        {
            return GetAll().Where( predicate );
        }

        public IEnumerable<T> SaveOrUpdateAll( params T[] entities )
        {
            foreach ( var entity in entities )
            {
                Session.SaveOrUpdate( entity );
            }

            return entities;
        }

        public T SaveOrUpdate( T entity )
        {
            Session.SaveOrUpdate( entity );

            return entity;
        }
    }
}