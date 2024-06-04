namespace CEC.SRV.BLL.Responses
{
    public enum StatusCode
    {
        /// <summary>
        /// Request executed with success
        /// </summary>
        OK = 1,

        /// <summary>
        /// Generic error
        /// </summary>
        Error = 2,

        /// <summary>
        /// Database entity not found
        /// </summary>
        EntityNotFound = 3,
    }
}