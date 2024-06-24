using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEC.SAISE.Domain;

namespace CEC.SAISE.EDayModule.Infrastructure
{
	public class SearchableColumnsHelper
	{
		public static Dictionary<string, string> GetBooleanSelect()
		{
			return new Dictionary<string, string>
            {
                {"", "Selectați"},    
                {bool.TrueString, "Da"},
                {bool.FalseString, "Nu"}
            };
		}

	    public static Dictionary<string, string> GetBallotPaperStatusSelect()
	    {
	        var list = new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>("", "Selectați...")};

            list.AddRange(PoliticalPartyStatusExtension.GetValuesAsArray<BallotPaperStatus>()
	                .Select(x => new KeyValuePair<string, string>(x.Key.ToString(), x.Value)));
	            
	        var dict = list.ToDictionary(x => x.Key.ToString(), x => x.Value);
	        return dict;
	    }
	}
}