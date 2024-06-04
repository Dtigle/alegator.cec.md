namespace CEC.SRV.Domain
{
    public class DocTypeCode : Lookup.Classifier
    {
    }

    public class RegTypeCode : Lookup.Classifier
    {
    }

    public class StreetTypeCode : Lookup.Classifier
    {
        public virtual string Docprint { get; set; }

        public virtual long RspStreetTypeCodeId { get; set; }
    }

	public class SexCode : Lookup.Classifier
	{
	}
}