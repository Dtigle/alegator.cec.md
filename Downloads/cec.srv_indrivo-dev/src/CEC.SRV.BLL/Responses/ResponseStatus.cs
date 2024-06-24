using System;
using System.Collections.Generic;
using System.Linq;

namespace CEC.SRV.BLL.Responses
{
    public class ResponseStatus
    {
        private readonly IList<string> _errors;

        public ResponseStatus()
        {
            StatusCode = Responses.StatusCode.OK;
            _errors = new List<string>();
        }

        public ResponseStatus(params string[] errors)
        {
            StatusCode = Responses.StatusCode.Error;
            _errors = errors;
        }

        /// <summary>
        /// Gets or sets the <see cref="StatusCode"/> of the response
        /// </summary>
        public StatusCode? StatusCode { get; set; }

        /// <summary>
        /// Gets list of errors
        /// </summary>
        public IEnumerable<string> Errors   
        {
            get { return _errors; }
        }

        /// <summary>
        /// Adds an error to Status
        /// </summary>
        /// <param name="error">Error to be added. Its check if same error already exists</param>
        public void AddError(string error)
        {
            if (!_errors.Contains(error))
            {
                _errors.Add(error);
            }
        }

        /// <summary>
        /// Adds a list of errors to the Status
        /// </summary>
        /// <param name="errors">List of errors</param>
        public void AddErrors(params string[] errors)
        {
            foreach (var error in errors)
            {
                AddError(error);
            }
        }

        /// <summary>
        /// Has request succeded or not
        /// </summary>
        public bool Succeeded
        {
            get
            {
                if (StatusCode == Responses.StatusCode.OK && !_errors.Any())
                {
                    return true;
                }

                throw new Exception("Response Status is damaged");
            }
        }

    }
}