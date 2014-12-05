using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monghost
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
