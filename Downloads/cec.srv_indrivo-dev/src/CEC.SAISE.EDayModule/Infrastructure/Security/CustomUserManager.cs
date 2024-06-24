using System;
using System.Configuration;
using System.Web;
using CEC.SAISE.Domain;
using Microsoft.AspNet.Identity;
using CEC.SAISE.EDayModule.Properties;

namespace CEC.SAISE.EDayModule.Infrastructure.Security
{
    public class CustomUserManager : UserManager<SystemUser, long>
    {
        public CustomUserManager(IUserStore<SystemUser, long> store) : base(store)
        {
            this.PasswordHasher = new SaisePasswordHasher();

            UserLockoutEnabledByDefault = Settings.Default.UserLockoutEnabledByDefault;
            DefaultAccountLockoutTimeSpan = Settings.Default.DefaultAccountLockoutTimeSpan;
            MaxFailedAccessAttemptsBeforeLockout = Settings.Default.MaxFailedAccessAttemptsBeforeLockout;
        }
    }
}