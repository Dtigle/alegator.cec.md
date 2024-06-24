using System;

namespace CEC.SRV.BLL.Responses
{
    public class Responser<TModel> where TModel : class
    {
        /// <summary>
        /// Gets the request Status <see cref="ResponseStatus"/>
        /// </summary>
        public ResponseStatus Status { get; private set; }


        /// <summary>
        /// ctor with defined <see cref="ResponseStatus"/>
        /// </summary>
        /// <param name="status"></param>
        public Responser(ResponseStatus status)
        {
            if (status == null) throw new ArgumentNullException("status");
            Status = status;
        }

        /// <summary>
        /// default ctor
        /// </summary>
        public Responser()
        {
            Status = new ResponseStatus();
        }

        /// <summary>
        /// Model of the response
        /// </summary>
        public TModel Model { get; set; }
    }
}