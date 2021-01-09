using System;
using System.Linq;
using Linq2OData.Core.Enums;

namespace Linq2OData.Core.Models
{
    public class OData<T>
    {
        #region Constructor
        public OData(string odata)
        {
            odata = UnEscapeOdata(odata);

            var parts = GetParts(odata);

            Count = GetPartValue(CountPrefix, parts) == "true";
            Filter = GetPartValue(FilterPrefix, parts);
            OrderBy = GetPartValue(OrderByPrefix, parts);
            Top = GetPartValue(TopPrefix, parts);
            Skip = GetPartValue(SkipPrefix, parts);
        }
        #endregion

        #region Properties
        public bool Count { get; set; }
        public ODataFilter<T> Filter { get; set; }
        public string OrderBy { get; set; }
        public string Top { get; set; }
        private string _skip;
        public string Skip
        {
            get
            {
                // Default value of skip must be always 0 when not provided
                return string.IsNullOrWhiteSpace(_skip) ? "0" : _skip;
            }
            set
            {
                _skip = value;
            }
        }        
        #endregion

        #region Operators
        public static implicit operator string(OData<T> d) {
            return d.ToString();
        }
        public static implicit operator OData<T>(string d) {
            return new OData<T>(d);
        }            
        #endregion
        
        #region Constants
        public const string CountPrefix = "$count=";

        public const string FilterPrefix = "$filter=";

        public const string OrderByPrefix = "$orderby=";

        public const string TopPrefix = "$top=";

        public const string SkipPrefix = "$skip=";
        #endregion        

        public string EscapeOdata(string odata) => odata.Replace(" ", "%20").Replace("'", "%27");

        public string UnEscapeOdata(string odata) => odata.Replace("%20", " ").Replace("%27", "'");

        public string GetPartValue(string prefix) => GetPart(prefix)?.Replace(prefix, "");

        public string GetPartValue(string prefix, string[] parts) => GetPart(prefix, parts)?.Replace(prefix, "");

        public string GetPartValue(string prefix, string odata) => GetPart(prefix, odata)?.Replace(prefix, "");

        public string GetPart(string prefix) => GetParts().FirstOrDefault(x => x.StartsWith(prefix));

        public string GetPart(string prefix, string[] parts) => parts.FirstOrDefault(x => x.StartsWith(prefix));

        public string GetPart(string prefix, string odata) => GetParts(odata).FirstOrDefault(x => x.StartsWith(prefix));

        public string[] GetParts(string odata) => odata.Split("&".ToCharArray());

        public string[] GetParts() =>
            new string[]{
                $"{CountPrefix}{(Count ? "true" : "false")}",
                $"{FilterPrefix}{Filter}",
                $"{OrderByPrefix}{OrderBy}",
                $"{TopPrefix}{Top}",
                $"{SkipPrefix}{Skip}",
            };

        public override string ToString() => EscapeOdata(string.Join("&", GetParts()));        
    }
}
