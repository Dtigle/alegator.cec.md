namespace CEC.SAISE.BLL
{
    public enum Status
    {
        Success,
        Error
    }

    public class BllResult<T>
    {
        public Status ExecStatus { get; set; }

        public string ExecMessage { get; set; }

        public T EntityId { get; set; }
    }

    public class BllResult : BllResult<long> { }
}
