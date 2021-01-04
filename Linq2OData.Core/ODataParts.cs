using System;
using System.Linq;
using Linq2OData.Core.Enums;

namespace Linq2OData.Core
{
    public class ODataParts
    {
        public bool Count { get; set; }
        public string Filter { get; set; }
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

        public const string CountPrefix = "$count=";
        public const string FilterPrefix = "$filter=";
        public const string OrderByPrefix = "$orderby=";
        public const string TopPrefix = "$top=";
        public const string SkipPrefix = "$skip=";

        public ODataParts(string odata)
        {
            odata = UnEscapeOdata(odata);

            var parts = GetParts(odata);

            Count = GetPartValue(CountPrefix, parts) == "true";
            Filter = GetPartValue(FilterPrefix, parts);
            OrderBy = GetPartValue(OrderByPrefix, parts);
            Top = GetPartValue(TopPrefix, parts);
            Skip = GetPartValue(SkipPrefix, parts);
        }

        public ODataParts(string odata, Type vm)
        {
            odata = UnEscapeOdata(odata);
            // odata = ConvertOdataFields(odata, vm);

            var parts = GetParts(odata);

            Count = GetPartValue(CountPrefix, parts) == "true";
            Filter = GetPartValue(FilterPrefix, parts);
            OrderBy = GetPartValue(OrderByPrefix, parts);
            Top = GetPartValue(TopPrefix, parts);
            Skip = GetPartValue(SkipPrefix, parts);
        }

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

        // public string ConvertOdataFields(string odata, Type type)
        // {
        //     var properties = type.GetProperties();
        //     foreach (var property in properties)
        //     {
        //         var odataFields = ((OdataFieldAttribute[])property
        //             .GetCustomAttributes(typeof(OdataFieldAttribute), true))
        //             .FirstOrDefault();

        //         if (odataFields != null)
        //             odata = odata.Replace(property.Name, odataFields.FieldName);
        //     }
        //     return odata;
        // }
        
        public ODataParts AddFilter(string filter, LogicalOperators logicalOperator = LogicalOperators.And)
        {
            var logicalOperatorName = Enum.GetName(typeof(LogicalOperators), logicalOperator).ToLower();

            var prefix = !string.IsNullOrWhiteSpace(Filter) ?
                $" {logicalOperatorName} " :
                $"";

            Filter += prefix + $"({filter})";

            return this;
        }
    }
}
