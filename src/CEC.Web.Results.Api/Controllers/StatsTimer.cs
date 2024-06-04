using System.Collections.Generic;
using CEC.Web.Results.Api.Dtos;
using System;
using System.Timers;
using CEC.Web.Results.Api.Infrastructure;
using FluentNHibernate.Testing.Values;

namespace CEC.Web.Results.Api.Controllers
{

    internal class StatsTimer : Timer
    {
        public StatsInfo Info { get; set; }
        public TurnoutInfo TurnoutInfo { get; set; }

        /// <summary>
        /// Resfresh UI interval and Live results from DataBase in minutes 
        /// </summary>
        public int TimerIntervalConfig
        {
            get { return GetAppParamInt("timerRefreshIntervalInMinutes", 15); }
        }

        public int VoteStartHourConfig
        {
            get { return GetAppParamInt("voteStartHour", 9); }
        }

        public int VoteEndHourConfig
        {
            get { return GetAppParamInt("voteEndHour", 19); }
        }

        public static int ElectionsId
        {
            get { return GetAppParamInt("electionsId", 0); }
        }

        private static int GetAppParamInt(string paramName, int deflt)
        {
            string param = System.Configuration.ConfigurationManager.AppSettings[paramName];
         
            int tValue;
            if (Int32.TryParse(param, out tValue))
                return tValue;
            else
                return deflt;
        }
        
        public StatsTimer()
        {
            // Set interval to 1/2 minute (30 sec)
            Interval = 60 * 1000;

            Info = new StatsInfo();
            TurnoutInfo = new TurnoutInfo();
        }
    }
}