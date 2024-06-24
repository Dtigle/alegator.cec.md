using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CEC.SAISE.Domain;

namespace CEC.SAISE.EDayModule.Models.ElectionResults
{
    public class BallotPaperModel : BallotPaperDataModel
    {
        public DateTime EditDate { get; set; }

        public string ServerEditDate {
            get { return EditDate.ToString("dd.MM.yyyy HH:mm:ss"); }
        }

        public string EditUser { get; set; }

        public bool IsResultsConfirmed { get; set; }

        public long? ConfirmationUserId { get; set; }

        public DateTime? ConfirmationDate { get; set; }

        public string ServerConfirmationDate
        {
            get { return ConfirmationDate.HasValue ? ConfirmationDate.Value.ToString("dd.MM.yyyy HH:mm:ss") : string.Empty; }
        }

        public BallotPaperStatus Status { get; set; }

        public long ElectionType { get; set; }

        public bool AlreadySent { get; set; }

        public bool AllowSubmitResults { get; set; }

        public bool AllowSubmitConfirmation { get; set; }
    }
}