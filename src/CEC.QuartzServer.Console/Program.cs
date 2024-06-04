using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.QuartzServer.Core;

namespace CEC.QuartzServer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            IQuartzServer server;

            try
            {
                //Bootstrapper.Initialize();
                //  HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

                //                BuildProgramaticJobs();
                //                System.Console.WriteLine("Done");
                //                return;
                UnityConfig.RegisterComponents();

                server = QuartzServerFactory.CreateServer();
                server.Initialize();

                server.Start();
            }
            catch (Exception e)
            {
                System.Console.Write("Error starting server: " + e.Message);
                System.Console.WriteLine(e.ToString());
                System.Console.WriteLine("Hit any key to close");
                System.Console.ReadKey();
                return;
            }

            System.Console.WriteLine();
            System.Console.WriteLine("The scheduler will now run until you type \"exit\"");
            System.Console.WriteLine("   If it was configured to export itself via remoting,");
            System.Console.WriteLine("   then other process may now use it.");
            System.Console.WriteLine();

            while (true)
            {
                System.Console.WriteLine("Type 'exit' to shutdown the server: ");
                if ("exit".Equals(System.Console.ReadLine()))
                {
                    break;
                }
            }

            System.Console.WriteLine(Environment.NewLine + "...Shutting down server...");

            server.Stop();
            server.Dispose();
        }
    }
}
