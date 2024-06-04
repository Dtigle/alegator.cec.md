using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Models.PollingStation
{
    public enum ElectionListNumSort
    {
        [Description("Nume, Prenume")]
        fisrst = 1,

        [Description("Nume, Prenume , Adresa")]
        second = 2,
    }
}