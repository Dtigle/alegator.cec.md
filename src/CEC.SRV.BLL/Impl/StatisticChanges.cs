using System;

namespace CEC.SRV.BLL.Impl
{
    [Flags]
    public enum StatisticChanges
    {
        None = 0,
        StatusUpdate = 1,
        ConflictAddress = 2,
        ConflictStatus = 4,
        ConflictPolling = 8,
        Total = 16,
        Update = 32,
        AddressUpdate = 64,
        New = 128,
        Error = 256,
        Conflict = ConflictAddress | ConflictStatus | ConflictPolling
    }
}