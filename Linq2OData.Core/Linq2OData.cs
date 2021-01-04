using System.Linq;

namespace Linq2OData.Core
{
    public static class Linq2OData
    {
        public static string ToOData(this IQueryable queryable)
        {
            return queryable.ToODataParts().ToString();
        }

        public static string ToODataParts(this IQueryable queryable)
        {
            
            return "";
        }
    }
    
}