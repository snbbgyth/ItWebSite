using NHibernate;

namespace ItWebSite.Core.IDAL
{
    public interface IFluentNHibernate
    {
        ISession GetSession();
    }
}
