using System.Collections.Generic;

namespace SoftTehnica.MvcReportViewer
{
    public class ReportParameter
    {
        public ReportParameter(string _key, string[] _values, Dictionary<string, object> _configs = null)
        {
            Key = _key;
            Values = _values;
            Configs = _configs;
        }

        public ReportParameter(string _key, string _value, Dictionary<string, object> _configs = null)
        {
            Key = _key;
            Values = new string[] { _value };
            Configs = _configs;
        }


        public string Key { get; set; }

        public string[] Values { get; set; }

        public Dictionary<string, object> Configs { get; set; }
    }
}
