using CsvHelper.Configuration;
using Homework.Entities;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Homework.Utils
{
    class CompanyMap : ClassMap<Company>
    {
        public CompanyMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Id).Ignore();
            Map(m => m.Name).Validate(args => new Regex(@"^[a-zA-Z0-9]{2,16}$").IsMatch(args.Field));
            Map(m => m.TaxNo).Validate(args => new Regex(@"^[0-9]{10}$").IsMatch(args.Field));
        }
    }
}
