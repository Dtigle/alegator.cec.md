using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Amdaris;

namespace CEC.QuartzServer.Service
{
    [RunInstaller(true)]
    public class QuartzServiceInstaller : Installer
    {
        private ServiceProcessInstaller _serviceProcessInstaller;
        private ServiceInstaller _serviceInstaller;
       // private static readonly ILogger _logger = Amdaris.DependencyResolver.Current.Resolve<ILogger>();

        public QuartzServiceInstaller()
        {
            // This call is required by the Designer.
            InitializeComponent();

            _serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_serviceProcessInstaller != null)
                {
                    _serviceProcessInstaller.Dispose();
                }

                if (_serviceInstaller != null)
                {
                    _serviceInstaller.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            _serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            _serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller
            // 
            //_serviceProcessInstaller.Password = null;
            //_serviceProcessInstaller.Username = null;

            _serviceInstaller.Description = "SRV Quartz Server Service";
            _serviceInstaller.DisplayName = "CEC.QuartzServer.Service";
            _serviceInstaller.ServiceName = "CEC.QuartzService";

            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[]
                                         {
                                             _serviceProcessInstaller,
                                             _serviceInstaller
                                         });
        }

        #endregion
    }
}
