using System.ComponentModel;
using System.ServiceProcess;

namespace ItWebSiteCrawlerService
{
    [RunInstaller(true)]
    public partial class CrawlerServiceInstaller : System.Configuration.Install.Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;
        public CrawlerServiceInstaller()
        {
            InitializeComponent();
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "ItWebSiteCrawlerNewsService";
            serviceInstaller.Description = "CSDN爬虫服务";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);

        }
    }
}
