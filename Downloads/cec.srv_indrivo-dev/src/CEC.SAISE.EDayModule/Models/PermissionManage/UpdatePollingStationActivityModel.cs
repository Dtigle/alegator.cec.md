using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.PermissionManage
{
    public class UpdatePollingStationActivityModel
    {
        [Required]
        public List<long> Ids { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public string ElectionStartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public string ElectionEndTime { get; set; }

        [Required]
        [Range(-12, 12, ErrorMessage = "Diferența de ora Moldovei trebuie să fie între -12 și 12.")]
        public int TimeDifferenceMoldova { get; set; }

        [Required]
        [Range(0, 120, ErrorMessage = "Numărul de ore de prelungire trebuie să fie între 10 și 120 în incrementări de 10.")]
        public int ActivityTimeExtendedFirstDay { get; set; }

        [Required]
        [Range(0, 120, ErrorMessage = "Numărul de ore de prelungire trebuie să fie între 10 și 120 în incrementări de 10.")]
        public int ActivityTimeExtendedSecondDay { get; set; }

        [Required]
        public int ElectionDurationId { get; set; } = 1; // default to 1 day election duration
    }
}