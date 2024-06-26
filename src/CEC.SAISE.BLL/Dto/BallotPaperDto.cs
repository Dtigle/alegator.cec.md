﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL.Dto
{
    public class BallotPaperDto : BallotPaperDataDto
    {
        public DateTime EditDate { get; set; }

        public string EditUser { get; set; }

        public bool IsResultsConfirmed { get; set; }

        public long? ConfirmationUserId { get; set; }

        public DateTime? ConfirmationDate { get; set; }

        public BallotPaperStatus Status { get; set; }

        public long ElectionType { get; set; }

        public bool AlreadySent
        {
            get { return Status > BallotPaperStatus.New || IsResultsConfirmed; }
        }

        public bool AllowSubmitResults { get; set; }

        public bool AllowSubmitConfirmation { get; set; }

    }
}
