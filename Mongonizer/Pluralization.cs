using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace Mongonizer
{
    internal static class Pluralization
    {
        public static string Pluralize(string noun)
        {
            var pluralizationService = PluralizationService.CreateService(new CultureInfo("en-US"));
            return pluralizationService.Pluralize(noun);
        }
    }
}
