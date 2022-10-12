using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Utils
{
    public static class ResourceHelper
    {
        public static IEnumerable<CultureInfo> GetLanguages()
        {
            var rm = new ResourceManager(typeof(Resource));
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                ResourceSet? rs = null;

                try { rs = rm.GetResourceSet(culture, true, false); }
                catch { }

                if (rs is null)
                    continue;

                if (culture.Equals(CultureInfo.InvariantCulture))
                    continue;

                yield return culture;
            }
        }
    }
}
