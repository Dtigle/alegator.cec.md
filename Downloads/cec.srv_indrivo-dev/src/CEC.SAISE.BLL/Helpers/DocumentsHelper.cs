using System.Collections.Generic;

namespace CEC.SAISE.BLL.Helpers
{
    public static class DocumentsHelper
    {
        public static readonly Dictionary<long, string> RegionTypeNames = new Dictionary<long, string>
        {
            { 1, "naționale "},
            { 2, "raionale " },
            { 3, "" },
            { 4, "municipale " },
            { 5, "" },
            { 6, "orășenești " },
            { 7, "orășenești " },
            { 8, "comunale " },
            { 9, "sătești " },
            { 10, "municipale " }
        };

        public static readonly Dictionary<long, string> ElectionDetailsType2Area1 = new Dictionary<long, string>
        {
            { 1, "" },
            { 2, "Consiliului raional " },
            { 3, "" },
            { 4, "Consiliului municipal " },
            { 5, "" },
            { 6, "Consiliului orășenesc " },
            { 7, "Consiliului orășenesc " },
            { 8, "Consiliului comunal " },
            { 9, "Consiliului sătesc " },
            { 10, "Consiliului municipal " }
        };

        public static readonly Dictionary<long, string> ElectionDetailsTypeDescription = new Dictionary<long, string>
        {
            { 1, " " },
            { 2, "raionul " },
            { 3, "UTA " },
            { 4, "municipiul " },
            { 5, " " },
            { 6, " " },
            { 7, " " },
            { 8, " " },
            { 9, " " },
            { 10, "municipiul " }
        };
    }
}
