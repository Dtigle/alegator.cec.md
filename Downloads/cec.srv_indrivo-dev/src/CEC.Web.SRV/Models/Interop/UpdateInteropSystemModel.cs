using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.SRV.Domain.Interop;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Lookup;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Interop
{
    public class UpdateInteropSystemModel : UpdateLookupModel
    {

        [Display(Name = "InteropSystem_TransactionProcessingMode", ResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        public TransactionProcessingTypes TransactionProcessingType { get; set; }


        [Display(Name = "InteropSystem_PersonStatusConsignment", ResourceType = typeof(MUI))]
        [UIHint("Checkbox")]
        public bool PersonStatusConsignment { get; set; }
        

        [Display(Name = "InteropSystem_PersonStatusType", ResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        public long? StatusId { get; set; }


        [Display(Name = "InteropSystem_TemporaryAddressConsignment", ResourceType = typeof(MUI))]
        [UIHint("Checkbox")]
        public bool TemporaryAddressConsignment { get; set; }

    }
}