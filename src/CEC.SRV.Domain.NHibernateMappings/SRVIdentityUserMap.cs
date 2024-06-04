using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class SRVIdentityUserMap : SubclassMap<SRVIdentityUser>
    {
        public SRVIdentityUserMap()
        {
            Schema(Schemas.RSA);
            Table("SRVIdentityUsers");
            Map(x => x.Comments);
            Map(x => x.LastLogin).Access.CamelCaseField(Prefix.Underscore);
            Map(x => x.LastLogout);
            Map(x => x.IsBuiltIn);
            Map(x => x.Created);
            Map(x => x.BlockedDate);
            Map(x => x.IsBlocked).Access.CamelCaseField(Prefix.Underscore).Default("0");
            Map(x => x.LoginAttempts).Access.CamelCaseField(Prefix.Underscore).Default("0");

            References(x => x.AdditionalInfo).Cascade.All();
            References(x => x.CreatedBy);

            HasManyToMany(x => x.Regions)
                .Access.CamelCaseField(Prefix.Underscore)
                .ExtraLazyLoad()
                .ParentKeyColumn("identityUserId")
                .ChildKeyColumn("regionId")
                .Table("SRVIdentityUsersRegions")
                .Schema(Schemas.RSA)
                .ForeignKeyConstraintNames("FK_SRVIdentityUsersRegions_SRVIdentityUsers", "FK_SRVIdentityUsersRegions_Regions");

            Map(x => x.IsOnline).Access.CamelCaseField(Prefix.Underscore).Formula("case when isnull(LastLogin, '1900-01-01 00:00:00') > isnull(LastLogout, '1900-01-01 00:00:00') then 1 else 0 end");
        }
    }
}