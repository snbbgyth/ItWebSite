using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using ItWebSite.Crawler;

namespace ItWebSiteCrawlerService
{
    partial class CrawlerServerService : ServiceBase
    {
        public CrawlerServerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
             ExecuteHandler.Execute();
        }

        protected override void OnStop()
        {
            // TODO:  在此处添加代码以执行停止服务所需的关闭操作。
        }
    }
}
