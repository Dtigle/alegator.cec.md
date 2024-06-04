namespace CEC.SRV.BLL.Responses
{
    public static class ResponseExtensions
    {
        /// <summary>
        /// Sets the <see cref="StatusCode"/> that should be set for final request
        /// </summary>
        /// <param name="responser"></param>
        /// <param name="statusCode"><see cref="StatusCode"/> which should be used</param>
        /// <returns>Updated Responser object</returns>
        public static Responser<TResult> WithStatusCode<TResult>(this Responser<TResult> responser, StatusCode statusCode) where TResult: class
        {
            responser.Status.StatusCode = statusCode;
            return responser;
        }

        /// <summary>
        /// Adds an error to the Responser object
        /// </summary>
        /// <param name="responser">Responser object</param>
        /// <param name="error"></param>
        /// <returns>Updated Responser object</returns>
        public static Responser<TResult> AddError<TResult>(this Responser<TResult> responser, string error) where TResult : class
        {
            responser.Status.AddError(error);
            return responser;
        }

        /// <summary>
        /// Adds more errors to the Status
        /// </summary>
        /// <param name="responser">Responser object</param>
        /// <param name="errors">params errors</param>
        /// <returns>Updated Responser object</returns>
        public static Responser<TResult> AddErrors<TResult>(this Responser<TResult> responser, params string[] errors) where TResult : class
        {
            responser.Status.AddErrors(errors);
            return responser;
        }

        /// <summary>
        /// Sets the model to be returned
        /// </summary>
        /// <param name="responser">Responser object</param>
        /// <param name="model">Response model</param>
        /// <returns>Updated Responser object</returns>
        public static Responser<TResult> WithModel<TResult>(this Responser<TResult> responser, dynamic model) where TResult : class
        {
            responser.Model = model;
            return responser;
        }
    }
}