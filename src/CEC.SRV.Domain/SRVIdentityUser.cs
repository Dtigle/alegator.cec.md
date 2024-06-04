using System.Collections.ObjectModel;
using Amdaris.Domain.Identity;
using System;
using System.Collections.Generic;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class SRVIdentityUser : IdentityUser
    {
        private readonly IList<Region> _regions;
        private bool _isBlocked;
        private int _loginAttempts;
        private DateTimeOffset? _lastLogin;
        private bool _isOnline;
        private const int AllowedFailedLogins = 5;

        public SRVIdentityUser()
        {
            _regions = new List<Region>();
        }

        public virtual bool IsBlocked
        {
            get { return _isBlocked; }
        }

        public virtual int LoginAttempts
        {
            get { return _loginAttempts; }
        }

        public virtual string Comments { get; set; }

        public virtual bool IsBuiltIn { get; set; }

        public virtual DateTimeOffset? BlockedDate { get; set; }

        public virtual DateTimeOffset? LastLogout { get; set;}

        public virtual bool IsOnline
        {
            get { return _isOnline; }
        }

        public virtual DateTimeOffset? LastLogin
        {
            get { return _lastLogin; }
        }

        public virtual AdditionalUserInfo AdditionalInfo { get; set; }

        public virtual DateTimeOffset Created { get; set; }

        public virtual SRVIdentityUser CreatedBy { get; set; }

        
        /// <summary>
        /// List of regions User is assigned
        /// </summary>
        public virtual IReadOnlyCollection<Region> Regions
        {
            get
            {
                return new ReadOnlyCollection<Region>(_regions);
            }
        }

        /// <summary>
        /// Assigns a <see cref="Region"/> to user if its not already assigned
        /// </summary>
        /// <param name="region">Region to be assigned</param>
        public virtual void AddRegion(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            if (!_regions.Contains(region))
            {
                _regions.Add(region);
            }
        }


        /// <summary>
        /// Removes a <see cref="Region"/> from user's regions
        /// </summary>
        /// <param name="region">Region to be assigned</param>
        public virtual void RemoveRegion(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            if (_regions.Contains(region))
            {
                _regions.Remove(region);
            }
        }

        /// <summary>
        /// User successfully loggined
        /// </summary>
        public virtual void SuccessLogin()
        {
            _loginAttempts = 0;
            _lastLogin = DateTimeOffset.Now;
        }

        /// <summary>
        /// User failed the login
        /// </summary>
        public virtual void FailLogin()
        {
            if (_loginAttempts >= AllowedFailedLogins - 1)
            {
                Block();
            }
            
            _loginAttempts++;    
        }

        public virtual void Block()
        {
            _isBlocked = true;
            _loginAttempts = 0;
            BlockedDate = DateTimeOffset.Now;
        }

        public virtual void UnBlock()
        {
            _isBlocked = false;
            _loginAttempts = 0;
            BlockedDate = null;

        }


    }
}