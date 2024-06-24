using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSP.CEC.WebClient
{
    public class RspSerivceKnownErrors
    {
        public static int Success = 0;
        public static int MissingPersonData = 46;
        public static int MinorPerson = 48;

        private static readonly int[] _validResults = { MissingPersonData, MinorPerson, Success };
        private static readonly int[] _errorsToRemove = { MissingPersonData, MinorPerson };

        public static int[] ValidResults { get { return _validResults; } }

        public static int[] ErrorsToRemove { get { return _errorsToRemove; } }
    }
}
