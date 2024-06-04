﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto
{
    public class BallotPaperDataDto
    {
        public BallotPaperDataDto()
        {
            CompetitorResults = new List<CompetitorResultDto>();
        }

        public long BallotPaperId { get; set; }

        public long RegisteredVoters { get; set; }

        public long Supplementary { get; set; }

        public long BallotsIssued { get; set; }

        public long BallotsCasted { get; set; }

        public long DifferenceIssuedCasted { get; set; }

        public long BallotsValidVotes { get; set; }

        public long BallotsReceived { get; set; }

        public long BallotsUnusedSpoiled { get; set; }

        public long BallotsSpoiled { get; set; }

        public long BallotsUnused { get; set; }

        public long OpeningVotersCount { get; set; }

        public List<CompetitorResultDto> CompetitorResults { get; set; }
    }
}
