using System;
using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace ItWebSite.Core.DAL
{
    public class FluentNHibernateDal : IFluentNHibernate
    {
        private static FluentNHibernateDal _instance;

        private static readonly object SyncObj = new object();

        public static FluentNHibernateDal Instance
        {
            get
            {
                lock (SyncObj)
                {
                    if (_instance == null)
                        _instance = new FluentNHibernateDal();
                }
                return _instance;
            }
        }

        public FluentNHibernateDal()
        {

        }

        private AutoPersistenceModel CreateAutomappings()
        {
            // This is the actual automapping - use AutoMap to start automapping,
            // then pick one of the static methods to specify what to map (in this case
            // all the classes in the assembly that contains Employee), and then either
            // use the Setup and Where methods to restrict that behaviour, or (preferably)
            // supply a configuration instance of your definition to control the automapper.
            return AutoMap.Assembly(Assembly.GetExecutingAssembly(), new AutomappingConfiguration())
                .Conventions.Add<CascadeConvention>();
        }

        private static ISessionFactory _sessionFactory;
        private static readonly object SyncFactory = new object();

        /// <summary>
        /// Configure NHibernate. This method returns an ISessionFactory instance that is
        /// populated with mappings created by Fluent NHibernate.
        /// 
        /// Line 1:   Begin configuration
        ///      2+3: Configure the database being used (SQLite file db)
        ///      4+5: Specify what mappings are going to be used (Automappings from the CreateAutomappings method)
        ///      6:   Expose the underlying configuration instance to the BuildSchema method,
        ///           this creates the database.
        ///      7:   Finally, build the session factory.
        /// </summary>
        /// <returns></returns>
        private ISessionFactory CreateSessionFactory()
        {
            lock (SyncFactory)
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = FluentConfig.BuildSessionFactory();
                }
            }
            return _sessionFactory;
        }

        public ISession GetSession()
        {
            try
            {
                return CreateSessionFactory().OpenSession();
            }
            catch (Exception ex)
            {
                //LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
                return CreateSessionFactory().GetCurrentSession();
            }
        }

        private FluentConfiguration FluentConfig
        {
            get
            {
                if (_fluentConfig == null)
                {
                    _fluentConfig = Fluently.Configure()
                        .Database((MySQLConfiguration.Standard.ConnectionString(c => c.FromConnectionStringWithKey("DefaultConnection"))))
                        //.Database(SQLiteConfiguration.Standard.UsingFile(UtilHelper.SqliteFilePath))
                                            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NewsCsdn>());
                    //m.AutoMappings.Add(CreateAutomappings));
                    BuildSchema(_fluentConfig.BuildConfiguration());
                }
                return _fluentConfig;
            }
        }

        private FluentConfiguration _fluentConfig;

        private void BuildSchema(Configuration config)
        {
            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaUpdate(config).Execute(false, true);
        }

    }

    class CascadeConvention : IReferenceConvention, IHasManyConvention, IHasManyToManyConvention, IIdConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.Cascade.All();
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Cascade.All();
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Cascade.All();
        }

        public void Apply(IIdentityInstance instance)
        {
            instance.Column("Id");
            instance.GeneratedBy.Native();
        }
    }

    class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.Namespace == "ItWebSite.Core.DbModel";
        }

        public override bool IsComponent(Type type)
        {
            return false;
        }

    }
}
