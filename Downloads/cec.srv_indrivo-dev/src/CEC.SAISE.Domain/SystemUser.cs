using Amdaris.Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CEC.SAISE.Domain
{
    public class SystemUser : SaiseBaseEntity, IUser<long>, IApplicationUser
    {
        private readonly IList<AssignedRole> _assignedRoles;

        public SystemUser()
        {
            _assignedRoles = new List<AssignedRole>();
        }

        public virtual string UserName { get; set; }

        public virtual string Password { get; set; }

        public virtual string Email { get; set; }

        public virtual int Level { get; set; }

        public virtual string Comments { get; set; }

        public virtual long Idnp { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string SureName { get; set; }

        public virtual string MiddleName { get; set; }

        public virtual DateTime DateOfBirth { get; set; }

        public virtual GenderType Gender { get; set; }

        public virtual string PasswordQuestion { get; set; }

        public virtual string PasswordAnswer{ get; set; }

        public virtual bool IsApproved { get; set; }

        public virtual bool IsOnLine { get; set; }

        public virtual bool IsLockedOut { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual DateTime LastActivityDate { get; set; }

        public virtual DateTime LastPasswordChangedDate { get; set; }

        public virtual DateTime LastLockoutDate { get; set; }

        public virtual DateTime FailedAttemptStart { get; set; }

        public virtual DateTime FailedAnswerStart { get; set; }

        public virtual int FailedAttemptCount { get; set; }

        public virtual int FailedAnswerCount { get; set; }

        public virtual DateTime LastLoginDate { get; set; }

        public virtual DateTime LastUpdateDate { get; set; }

        public virtual string Language { get; set; }

        public virtual string MobileNumber { get; set; }

        public virtual string ContactName { get; set; }

        public virtual string ContactMobileNumber { get; set; }

        public virtual string StreetAddress { get; set; }

        public virtual long? ElectionId { get; set; }

        public virtual long? RegionId { get; set; }

        public virtual long? PollingStationId { get; set; }

        public virtual long? CircumscriptionId { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual IReadOnlyCollection<AssignedRole> AssignedRoles
        {
            get { return new ReadOnlyCollection<AssignedRole>(_assignedRoles); }
        }

        public virtual IEnumerable<Role> GetRoles()
        {
            return _assignedRoles.Select(x => x.Role);
        }

        public virtual void AddRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            if (_assignedRoles.All(x => x.Role.Id != role.Id))
            {
                _assignedRoles.Add(new AssignedRole {SystemUser = this, Role = role});
            }
        }

        public virtual void RemoveRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            var assignedRole = _assignedRoles.FirstOrDefault(x => x.Role.Id == role.Id);
            if (assignedRole != null)
            {
                _assignedRoles.Remove(assignedRole);
            }
        }

        public virtual bool IsInRole(string roleName)
        {
            return _assignedRoles.Any(x => x.Role.Name == roleName);
        }

        public virtual bool HasPermission(string permissionName)
        {
            return _assignedRoles.SelectMany(x => x.Role.GetPermissions()).Any(x => x.Name == permissionName);
        }

        public virtual long GetElectionId()
        {
            return ElectionId.GetValueOrDefault();
        }

        public virtual long GetRegionId()
        {
            return RegionId.GetValueOrDefault();
        }

        public virtual long GetPollingStationId()
        {
            return PollingStationId.GetValueOrDefault();
        }

        public virtual long GetCircumsctiptionId()
        {
            return CircumscriptionId.GetValueOrDefault();
        }


        public virtual void SuccessLogin()
	    {
		    LastLoginDate = DateTime.Now;
	    }
    }
}
