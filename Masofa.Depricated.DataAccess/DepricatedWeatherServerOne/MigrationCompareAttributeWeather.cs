using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne
{
    public class MigrationCompareAttributeWeather : Attribute
    {
        public Type CompareToType { get; set; }
        public Type CompareToDbContext { get; set; }
    }
}
