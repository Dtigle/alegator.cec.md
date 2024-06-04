using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Infrastructure.Logger
{
    public static class Events
    {
        public static class Authorisation
        {
            public const string Value = "AUTHORISATION";
            public const string Description = "Login/Logout";
            public static class Attributes
            {
                public const string Action = "ACTION";
            }
        }
        public static class DeleteNumberList
        {
            public const string Value = "DELETENUMBERLIST";
            public const string Description = "Sterge nr. lista alegatori";
            public static class Attributes
            {
                public const string Action = "ACTION";
            }
        }

        public static class Conflict
        {
            public const string ConflictStatus = "CONFLICT-STATUS";
            public const string ConflictStatusDesc = "Vizualizare conflict statut";

            public const string ConflictAddress = "CONFLICT-ADDRESS";
            public const string ConflictAddressDesc = "Vizualizare conflict adresă";

            public const string ConflictPollingStation = "CONFLICT-POLLINGSTATION";
            public const string ConflictPollingStationDesc = "Vizualizare conflict secția de votare";

            public const string ConflictStreet = "CONFLICT-STREET";
            public const string ConflictStreetDesc = "Vizualizare conflict lipsă stradă";

            public const string ConflictCuatm = "CONFLICT-CUATM";
            public const string ConflictCuatmDesc = "Vizualizare conflict lipsă CUATM cod";

            public static class Attributes
            {
                public const string Action = "ACTION";
            }
        }
        public static class VoterStatus
        {
            public const string Value = "VOTER-STATUS";
            public const string Description = "Modificare statut alegător";
            public static class Attributes
            {
                public const string Action = "ACTION";
                public const string Voter = "VOTER";
                public const string Status = "STATUS";
            }
        }
        public static class VoterAddress
        {
            public const string Value = "VOTER-ADDRESS";
            public const string Description = "Modificare adresa alegător";
            public static class Attributes
            {
                public const string Action = "ACTION";
                public const string Voter = "VOTER";
                public const string Address = "ADDRESS";
            }
        }

        public static class VoterExport
        {
            public const string Value = "VOTER-EXPORT";
            public const string Description = "Export liste alegători";
            public static class Attributes
            {
                public const string Region = "REGION";
                public const string Election = "ELECTION";
            }
        }

        public static class RspCheck
        {
            public const string Value = "RSP-CHECK";
            public const string Description = "Accesarea serviciului ”Verifică-te în RSA”";
            public static class Attributes
            {
                public const string Person = "PERSON";
                public const string Status = "STATUS";
            }
        }

        public static class Reports
        {
            public const string Value = "REPORTS";
            public const string Description = "Accesare RAPOARTE";
            public static class Attributes
            {
                public const string Report = "REPORT";
            }
        }
    }
}