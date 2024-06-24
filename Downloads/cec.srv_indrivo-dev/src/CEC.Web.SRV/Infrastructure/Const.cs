using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Infrastructure
{
    public static class Const
    {
        public const string CloseWindowContent = "$$CLOSE_WINDOW$$";
        public const string PasswordValidationExpression = @"^(?=.*[0-9])(?=.*[a-z]){2,}(?=.*[A-Z]){2,}.{8,}$";
        public const string EmailValidationExpression = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        public const string TelephoneValidationExpression = @"^0{1}[0-9]{8}$";

        public const string TelephonePrompt = "0xxxxxxxx";
        public const string OnlyNumbers = @"^[0-9]+$";
		public const string OnlyFiveNumbers = @"^[0-9]{1,5}$";
        public const string CharacterValidationExpression = @"^((?![А-я]).)*$";
        public const string NumberPollingStationValidationExpression = @"^([0-9]{1,3})(\/([0-9]{1,3}))*$";
    }
}