using FluentNHibernate.Mapping;
using NHibernate;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class PersonDocumentMap : ComponentMap<PersonDocument>
    {
        public PersonDocumentMap()
        {
            Map(x => x.Seria);
            Map(x => x.Number);
            Map(x => x.IssuedDate).CustomSqlType(NHibernateUtil.DateTime2.Name).CustomType(NHibernateUtil.DateTime2.Name); ;
            Map(x => x.IssuedBy).Length(50);
            Map(x => x.ValidBy);
            References(x => x.Type).Not.Nullable();

            Map(x => x.DocumentNumber).Formula("doc_seria + doc_number").Access.CamelCaseField(Prefix.Underscore).Not.Update().Not.Insert().ReadOnly();
        }
    }
}