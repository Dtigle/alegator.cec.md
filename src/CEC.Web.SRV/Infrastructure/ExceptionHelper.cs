using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using CEC.SRV.BLL.Exceptions;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Infrastructure
{
    public static class ExceptionHelper
    {
        private static readonly IDictionary<Type, Func<Exception, string>> Resolver = new Dictionary<Type, Func<Exception, string>>
        {
            {typeof(SrvException), ResolveSrvExceptionMessage},
            {typeof(HttpException), ResolveHttpExceptionMessage}
        };

        private static string ResolveSrvExceptionMessage(Exception exception)
        {
            var error = exception as SrvException;
            if (error == null) throw new ArgumentNullException("error");
	        return string.IsNullOrEmpty(error.LocalizationKey) ? error.Message : MUI.ResourceManager.GetString(error.LocalizationKey);
        }

        private static string ResolveHttpExceptionMessage(Exception exception)
        {
            var error = exception as HttpException;
            if (error == null) throw new ArgumentNullException("error");

            switch (error.GetHttpCode())
            {
                case 404:
                    return string.Format(MUI.ResourceManager.GetString("Error_404"), error.Source);
                default:
                    return GetDefaultErrorMessage();
            }
        }

        private static string GetDefaultErrorMessage()
        {
            return MUI.ResourceManager.GetString("Error_Default");
        }

        public static string GetLocalizedMessage(this Exception exception)
        {
            var exceptionKey = exception.GetType();

            return !Resolver.ContainsKey(exceptionKey) 
                ? GetDefaultErrorMessage() 
                : Resolver[exceptionKey] (exception);
        }
    }
}