using CEC.QuartzServer.Core;
using System;
using System.ServiceProcess;

namespace CEC.QuartzServer.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            UnityConfig.RegisterComponents(); 
            var servicesToRun = new ServiceBase[] 
            { 
                new QuartzService() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
