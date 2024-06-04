using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Amdaris.NHibernateProvider;
using CEC.SAISE.Domain;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Linq;
using CEC.SAISE.EDayModule.Properties;

namespace CEC.SAISE.EDayModule.Infrastructure.Security
{
    public class SaiseUserStore : Repository, IUserStore<SystemUser, long>, IUserRoleStore<SystemUser, long>, 
        IUserPasswordStore<SystemUser, long>, IUserClaimStore<SystemUser, long>, IUserLockoutStore<SystemUser, long>
    {
        private bool _isDisposed;

	    private readonly TimeSpan _defaultAccountLockoutTimeSpan;

        public SaiseUserStore(ISessionFactory sessionFactory) : base(sessionFactory)
        {
            _defaultAccountLockoutTimeSpan = Settings.Default.DefaultAccountLockoutTimeSpan;
        }

	    #region User

	    public Task CreateAsync(SystemUser user)
	    {
		    ThrowIfDisposed();
		    if (user == null) throw new ArgumentNullException("user");

		    var session = Session;
		    return ExecuteAsync(() => session.Save(user));
	    }

	    public Task UpdateAsync(SystemUser user)
	    {
		    ThrowIfDisposed();
		    if (user == null) throw new ArgumentNullException("user");

		    var session = Session;
		    return ExecuteAsync(() => session.Update(user));
	    }

	    public Task DeleteAsync(SystemUser user)
	    {
		    ThrowIfDisposed();
		    if (user == null) throw new ArgumentNullException("user");

		    var session = Session;
		    return ExecuteAsync(() => session.Delete(user));
	    }

	    public Task<SystemUser> FindByIdAsync(long userId)
	    {
		    ThrowIfDisposed();

		    return GetAsync<SystemUser, long>(userId);
	    }

	    public Task<SystemUser> FindByNameAsync(string userName)
	    {
		    ThrowIfDisposed();
		    if (userName == null) throw new ArgumentNullException("userName");

		    var session = Session;
		    return ExecuteAsync(() => session.Query<SystemUser>().SingleOrDefault(x => x.UserName == userName));
	    }

	    #endregion

	    #region Dispose

	    public void Dispose()
	    {
		    _isDisposed = true;
	    }

	    private void ThrowIfDisposed()
	    {
		    if (_isDisposed)
			    throw new ObjectDisposedException(GetType().Name);
	    }

	    #endregion
		
		#region Role
		public Task AddToRoleAsync(SystemUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (roleName == null) throw new ArgumentNullException("role");

            var session = Session;
            return ExecuteAsync(() =>
            {
                var identityRole = session.QueryOver<Role>()
                    .Where(x => x.Name == roleName)
                    .SingleOrDefault();

                user.AddRole(identityRole);
                session.Update(user);
            });
        }

        public Task RemoveFromRoleAsync(SystemUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (roleName == null) throw new ArgumentNullException("role");

            var session = Session;
            return ExecuteAsync(() =>
            {
                var roleToRemove = user.GetRoles().Single(x => x.Name == roleName);
                user.RemoveRole(roleToRemove);
                session.Update(user);
            });
        }

        public Task<IList<string>> GetRolesAsync(SystemUser user)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");

            return ExecuteAsync(() => (IList<string>) user.GetRoles().Select(x => x.Name).ToList());
        }

        public Task<bool> IsInRoleAsync(SystemUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException("user");
            if (roleName == null) throw new ArgumentNullException("role");

            return ExecuteAsync(() => user.IsInRole(roleName));
        }

		#endregion

	    #region Password

	    public Task SetPasswordHashAsync(SystemUser user, string passwordHash)
	    {
		    throw new NotImplementedException();
	    }

	    public Task<string> GetPasswordHashAsync(SystemUser user)
	    {
		    ThrowIfDisposed();
		    if (user == null) throw new ArgumentNullException("user");

		    return Task.FromResult(user.Password);
	    }

	    public Task<bool> HasPasswordAsync(SystemUser user)
	    {
		    ThrowIfDisposed();
		    if (user == null) throw new ArgumentNullException("user");

		    return Task.FromResult(!string.IsNullOrEmpty(user.Password));
	    }

	    #endregion
		
	    #region Claims

	    public Task<IList<Claim>> GetClaimsAsync(SystemUser user)
	    {
		    ThrowIfDisposed();
		    if (user == null) throw new ArgumentNullException("user");

		    return ExecuteAsync(() =>
			    (IList<Claim>)
				    user.GetRoles()
					    .SelectMany(x => x.GetPermissions())
					    .Select(x => new Claim("permission", x.Name))
					    .ToList());
	    }

	    public Task AddClaimAsync(SystemUser user, Claim claim)
	    {
		    throw new NotImplementedException();
	    }

	    public Task RemoveClaimAsync(SystemUser user, Claim claim)
	    {
		    throw new NotImplementedException();
	    }

	    #endregion

	    #region Lockout

	    public Task<DateTimeOffset> GetLockoutEndDateAsync(SystemUser user)
	    {
			ThrowIfDisposed();

            DateTimeOffset lockoutEndDate;
	        if (_defaultAccountLockoutTimeSpan == TimeSpan.MaxValue)
	        {
	            lockoutEndDate = DateTimeOffset.MaxValue;
	        }
	        else
	        {
	            lockoutEndDate = user.LastLockoutDate.Add(_defaultAccountLockoutTimeSpan);
	        }
	        return Task.FromResult(lockoutEndDate);
	    }

	    public Task SetLockoutEndDateAsync(SystemUser user, DateTimeOffset lockoutEnd)
	    {
			ThrowIfDisposed();
			if (user == null) throw new ArgumentNullException("user");

			var session = Session;
		    user.LastLockoutDate = DateTime.Now;

			return ExecuteAsync(() => session.Update(user));
	    }

	    public Task<int> IncrementAccessFailedCountAsync(SystemUser user)
	    {
			ThrowIfDisposed();
			if (user == null) throw new ArgumentNullException("user");

		    var count = user.FailedAttemptCount;
		    Interlocked.Increment(ref count);
		    user.FailedAttemptCount = count;
	        if (count == Settings.Default.MaxFailedAccessAttemptsBeforeLockout)
	        {
	            user.IsLockedOut = true;
                user.LastLockoutDate = DateTime.Now;
	        }

	        var session = Session;

			ExecuteAsync(() => session.Update(user));

		    return Task.FromResult(user.FailedAttemptCount);
	    }

	    public Task ResetAccessFailedCountAsync(SystemUser user)
	    {
			ThrowIfDisposed();
			if (user == null) throw new ArgumentNullException("user");

			user.FailedAttemptCount = 0;

			var session = Session;

			return ExecuteAsync(() => session.Update(user));
	    }

	    public Task<int> GetAccessFailedCountAsync(SystemUser user)
	    {
			ThrowIfDisposed();
			if (user == null) throw new ArgumentNullException("user");

			return Task.FromResult(user.FailedAttemptCount);
	    }

	    public Task<bool> GetLockoutEnabledAsync(SystemUser user)
	    {
			ThrowIfDisposed();
			if (user == null) throw new ArgumentNullException("user");

            return Task.FromResult(user.IsLockedOut);
	    }

	    public Task SetLockoutEnabledAsync(SystemUser user, bool enabled)
	    {
			ThrowIfDisposed();
			if (user == null) throw new ArgumentNullException("user");

            var session = Session;

	        user.IsLockedOut = enabled;
            return ExecuteAsync(() => session.Update(user));
	    }

	    #endregion

    }
}