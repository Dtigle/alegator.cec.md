namespace CEC.SRV.BLL.Dto
{
    public class LookupDto : BaseDto<long>
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }
    }
}