using System.Data.Entity.Migrations;
using ItWebSite.Web.Areas.Admin.Models;

namespace ItWebSite.Web.DAL.MySql
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            // register mysql code generator
            SetSqlGenerator("MySql.Data.MySqlClient",   new global::MySql.Data.Entity.MySqlMigrationSqlGenerator());
            SetHistoryContextFactory("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));
        }

        protected override void Seed(ApplicationDbContext context)
        {

        }
    }
}