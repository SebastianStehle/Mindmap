using System.Globalization;
using Windows.ApplicationModel.Resources;

namespace Hercules.Rendering
{
    internal static class ResourceManager
    {
        public static string GetString(string key)
        {
            ResourceLoader resourceLoader = new ResourceLoader();
            
            return resourceLoader.GetString(key) ?? key;
        }

        public static string FormatString(string key, params object[] args)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            return string.Format(CultureInfo.CurrentCulture, resourceLoader.GetString(key), args) ?? key;
        }
    }
}
