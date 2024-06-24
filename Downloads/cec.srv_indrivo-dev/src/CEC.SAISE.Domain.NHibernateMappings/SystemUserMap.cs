using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class SystemUserMap : IAutoMappingOverride<SystemUser>
    {
        public void Override(AutoMapping<SystemUser> mapping)
        {
            mapping.Table("SystemUser");
            mapping.Schema("dbo");
            mapping.Id(x => x.Id).Column("SystemUserId");
            mapping.Map(x => x.UserName).Not.Nullable().Length(50);
            mapping.Map(x => x.Password).Not.Nullable();
            mapping.Map(x => x.Email).Not.Nullable().Length(50);
            mapping.Map(x => x.Level).Not.Nullable();
            mapping.Map(x => x.Comments).Nullable();
            mapping.Map(x => x.Idnp).Not.Nullable();
            mapping.Map(x => x.FirstName).Not.Nullable().Length(100);
            mapping.Map(x => x.SureName).Not.Nullable().Length(100).Column("Surname");
            mapping.Map(x => x.MiddleName).Nullable().Length(100);
            mapping.Map(x => x.DateOfBirth).Not.Nullable();
            mapping.Map(x => x.Gender).Not.Nullable().CustomType<EnumType<GenderType>>();
            mapping.Map(x => x.PasswordQuestion).Nullable().Length(100);
            mapping.Map(x => x.PasswordAnswer).Nullable().Length(100);
            mapping.Map(x => x.IsApproved).Not.Nullable();
            mapping.Map(x => x.IsOnLine).Not.Nullable();
            mapping.Map(x => x.IsLockedOut).Not.Nullable();
            mapping.Map(x => x.CreationDate).Not.Nullable();
            mapping.Map(x => x.LastActivityDate).Not.Nullable();
            mapping.Map(x => x.LastPasswordChangedDate).Not.Nullable();
            mapping.Map(x => x.LastLockoutDate).Not.Nullable();
            mapping.Map(x => x.FailedAttemptStart).Not.Nullable();
            mapping.Map(x => x.FailedAnswerStart).Not.Nullable();
            mapping.Map(x => x.FailedAttemptCount).Not.Nullable();
            mapping.Map(x => x.FailedAnswerCount).Not.Nullable();
            mapping.Map(x => x.LastLoginDate).Not.Nullable();
            mapping.Map(x => x.LastUpdateDate).Not.Nullable();
            mapping.Map(x => x.Language).Nullable().Length(100);
            mapping.Map(x => x.MobileNumber).Nullable().Length(20);
            mapping.Map(x => x.ContactName).Nullable().Length(100);
            mapping.Map(x => x.ContactMobileNumber).Nullable().Length(20);
            mapping.Map(x => x.StreetAddress).Nullable().Length(100);
            mapping.Map(x => x.IsDeleted).Not.Nullable();
            mapping.Map(x => x.ElectionId);
            mapping.Map(x => x.RegionId);
            mapping.Map(x => x.PollingStationId);
            mapping.Map(x => x.CircumscriptionId);
            mapping.HasMany(x => x.AssignedRoles)
                .ExtraLazyLoad()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}