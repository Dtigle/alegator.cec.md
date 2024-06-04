namespace CEC.SRV.BLL.Dto
{
    public abstract class BaseDto<TId>
    {
        public virtual TId Id { get; set; }
    }
}