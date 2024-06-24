namespace CEC.SRV.Domain.Importer
{
    public abstract class RspData : PersonRaw
    {
        protected RspData()
        {
            Source = SourceEnum.Rsv;
        }

    }
}