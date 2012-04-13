using System;
using System.Web;

using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;

namespace FluentNHibernateMvc3.Models.Data
{
    /// <summary>
    /// http://www.bengtbe.com/blog/2009/10/08/nerddinner-with-fluent-nhibernate-part-3-the-infrastructure
    /// </summary>
    public class NHibernateSessionPerRequest : IHttpModule
    {
        private static readonly ISessionFactory _sessionFactory;

        static NHibernateSessionPerRequest()
        {
            _sessionFactory = CreateSessionFactory();
        }

        public void Init( HttpApplication context )
        {
            context.BeginRequest += BeginRequest;
            context.EndRequest += EndRequest;
        }

        public static ISession GetCurrentSession()
        {
            return _sessionFactory.GetCurrentSession();
        }

        public void Dispose() { }

        private static void BeginRequest( object sender, EventArgs e )
        {
            ISession session = _sessionFactory.OpenSession();

            session.BeginTransaction();

            CurrentSessionContext.Bind( session );
        }

        private static void EndRequest( object sender, EventArgs e )
        {
            ISession session = CurrentSessionContext.Unbind( _sessionFactory );

            if ( session == null ) return;

            try
            {
                session.Transaction.Commit();
            }
            catch ( Exception )
            {
                session.Transaction.Rollback();
            }
            finally
            {
                session.Close();
                session.Dispose();
            }
        }

        private static ISessionFactory CreateSessionFactory()
        {
            var mappings = CreateMappings();

            return Fluently
                .Configure()
                .Database( MsSqlConfiguration.MsSql2008
                    .ConnectionString( c => c
                        .FromConnectionStringWithKey( "Dave3" ) ) )
                .Mappings( m => m
                    .AutoMappings.Add( mappings ) )
                .ExposeConfiguration( c =>
                    {
                        BuildSchema( c );
                        c.Properties[ NHibernate.Cfg.Environment.CurrentSessionContextClass ] = "web";
                    } )
                .BuildSessionFactory();
        }

        private static AutoPersistenceModel CreateMappings()
        {
            return AutoMap
                .Assembly( System.Reflection.Assembly.GetCallingAssembly() )
                .Where( t => t.Namespace == "FluentNHibernateMvc3.Models" )
                .Conventions.Setup( c =>
                    {
                        c.Add( DefaultCascade.SaveUpdate() );
                    } );
        }

         // Drops and creates the database
         // private static void BuildSchema( Configuration cfg )
         // {
         //     new SchemaExport( cfg )
         //         .Create( false, true );
         // }

         // Updates the database if there are any changes to the model
         private static void BuildSchema( Configuration cfg )
         {
             new SchemaUpdate( cfg );
         }
    }
}