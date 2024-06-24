using System;
using Amdaris;

namespace CEC.QuartzServer.Core
{
    /// <summary>
    /// Factory class to create Quartz server implementations from.
    /// </summary>
    public class QuartzServerFactory
    {
        /// <summary>
        /// Creates a new instance of an Quartz.NET server core.
        /// </summary>
        /// <returns></returns>
        public static IQuartzServer CreateServer()
        {
            var logger = DependencyResolver.Current.Resolve<ILogger>();
            string typeName = Configuration.ServerImplementationType;

            Type t = Type.GetType(typeName, true);

            logger.Debug("Creating new instance of server type '" + typeName + "'");
            var retValue = (IQuartzServer)Activator.CreateInstance(t);
            logger.Debug("Instance successfully created");
            return retValue;
        }
    }
}
