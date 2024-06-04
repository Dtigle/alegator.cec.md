using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto
{
   public enum AuditEventTypeDto
    {
        [Description("AUTHORISATION")]
        Login,

        [Description("AUTHORISATION")]
        Logout,

        [Description("CREARE-CERTIFICAT")]
        CreateCertificat,

        [Description("DESCHIDERE-SV")]
        OpenElction,

        [Description("CAUTARE-IDNP")]
        SearchIdnp,

        [Description("PROCES-VERBAL")]
        BallotPaper,

        [Description("GENERARE-PROCES-VERBAL")]
        GenerateBallotPaper,

        [Description("PROCES-VERBAL-APOBAT")]
        BallotPaperAproved,

        

        [Description("GESTIONARE-FUNTIONALA")]
        FunctionalManagement,

        [Description("ACCESARE-STATISTICI")]
        Statistic,
            
        [Description("ACCESARE-TRANSFER")]
        Transfer,

        [Description("AJUSTARE-CANDIDATI")]
        AdjustCandidate,

        [Description("AJUSTARE-VOTERS")]
        AdjustVoters

    }
}
