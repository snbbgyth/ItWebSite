using Autofac;
using ItWebSite.Core.DAL;
using ItWebSite.Core.IDAL;
using NHibernate;

namespace ItWebSite.Core.BLL
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<FluentNHibernateDal>().As<IFluentNHibernate>().SingleInstance();
            builder.Register(c => c.Resolve<IFluentNHibernate>().GetSession()).As<ISession>();
            builder.RegisterType<OtherLogInfoDal>().As<IOtherLogInfo>();
            builder.RegisterType<NewsCsdnDal>().As<INewsCsdnDal>();
            builder.RegisterType<NewsTypeDal>().As<INewsTypeDal>();
            builder.RegisterType<CommentDal>().As<ICommentDal>();
            builder.RegisterType<OrderDal>().As<IOrderDal>();
            builder.RegisterType<OrderItemDal>().As<IOrderItemDal>();
            builder.RegisterType<ProductDal>().As<IProductDal>();
            builder.RegisterType<ProductTypeDal>().As<IProductTypeDal>();
            builder.RegisterType<ShopCartItemDal>().As<IShopCartItemDal>();
            builder.RegisterType<PictureDal>().As<IPictureDal>();
            builder.RegisterType<ProductPictureDal>().As<IProductPictureDal>();
            builder.RegisterType<WebContentDal>().As<IWebContentDal>();
            builder.RegisterType<WebContentTypeDal>().As<IWebContentTypeDal>();
            builder.RegisterType<BlogContentDal>().As<IBlogContentDal>();
            builder.RegisterType<BlogContentTypeDal>().As<IBlogContentTypeDal>();
            builder.RegisterType<News51CtoDal>().As<INews51CtoDal>();
        }
    }
}
