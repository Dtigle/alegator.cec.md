using Amdaris;
using CEC.QuartzServer.Core;
using System;
using System.ServiceProcess;

namespace CEC.QuartzServer.Service
{
    public class QuartzService : ServiceBase
    {
        private readonly ILogger _logger = Amdaris.DependencyResolver.Current.Resolve<ILogger>();
        private IQuartzServer _server;

        public QuartzService()
        {
            ServiceName = "CEC.QuartzService";
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            _logger.Error((Exception)unhandledExceptionEventArgs.ExceptionObject, "Unexpected exception");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _server != null)
            {
                _logger.Debug("Disposing service");
                _server.Dispose();
                _logger.Debug("Service disposed");
            }
            base.Dispose(disposing);
        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("Start QuartzService");

            //UnityConfig.RegisterComponents(); 
            _server = QuartzServerFactory.CreateServer();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            _logger.Debug("Obtaining instance of an IQuartzServer");

            _logger.Debug("Initializing server");
            try
            {
                _server.Initialize();
            }
            catch (Exception ex)
            {
                _logger.Debug(string.Format("Failed to initialize Quartz Server. Exception: {0}", ex));
                throw;
            }
            _logger.Debug("Server initialized");

            _logger.Debug("Starting service");
            _server.Start();
            _logger.Debug("Service started");
        }

        protected override void OnStop()
        {
            _logger.Debug("Stopping service");
            _server.Stop();
            _logger.Debug("Service stopped");
        }
    }
}
