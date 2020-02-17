using System.Web;
using System.Web.Mvc;

namespace AMiONGraphShift
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (filters == null)
            {
                throw new System.ArgumentNullException(nameof(filters));
            }

            filters.Add(new HandleErrorAttribute());
        }
    }
}
