using CEC.SAISE.EDayModule.Models.Voting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.Document
{
    public class DocumentModel
    {
        public int TemplateNameId { get; set; }
        public UserDataModel UserData { get; set; }
        public DocumentDataModel DocumentData { get; set; }

    }
}