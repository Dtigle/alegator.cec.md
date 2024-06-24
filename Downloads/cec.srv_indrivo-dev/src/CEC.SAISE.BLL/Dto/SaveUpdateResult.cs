using System;

namespace CEC.SAISE.BLL.Dto
{
    public class SaveUpdateResult
    {
        public bool Success { get; set; }

        public BallotPaperValidationStatus ValidationStatus { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string UserName { get; set; }
    }
}
