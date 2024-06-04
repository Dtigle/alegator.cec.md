using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Globalization;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString TitleCaseNet4(SqlString s)
    {
        if (s.IsNull)
        {
            return s;
        }

        var ti = CultureInfo.CurrentCulture.TextInfo;
        var result = ti.ToTitleCase(ti.ToLower(s.Value));
        return new SqlString (result);
    }
}
