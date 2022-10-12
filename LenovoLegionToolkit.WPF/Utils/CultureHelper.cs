using System.Globalization;
using System.Threading;

namespace LenovoLegionToolkit.WPF.Utils
{
    public static class CultureHelper
    {
        public static void Set(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");

            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            Resources.Resource.Culture = cultureInfo;
            Lib.Resources.Resource.Culture = cultureInfo;
            Lib.Automation.Resources.Resource.Culture = cultureInfo;
        }
    }
}
