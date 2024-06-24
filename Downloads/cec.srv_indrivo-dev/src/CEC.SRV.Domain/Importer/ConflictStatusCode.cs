using System;

namespace CEC.SRV.Domain.Importer
{
    [Flags]
    public enum ConflictStatusCode
    {
        None = 0,
        StatusConflict = 1,
        AddressConflict = 2,
        PollingStationConflict = 4,
        RegionConflict = 8,
        StreetConflict = 16,
        LocalityConflict = 32,
        StreetZeroConflict = 64,
        AddressFatalConflict = 128,
        
        //
        // GreenSoft: Conflicte rezolvare manual prin rulare de scripturi, vezi Database -> ManualRun
        //
        ManualSolved = 255
    }
}