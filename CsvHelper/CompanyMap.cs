using CsvHelper.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Homework.CsvHelper
{
    class CompanyMap : ClassMap<CompanyCsvHelperDto>
    {
        public CompanyMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Name).Validate(args => new Regex(@"^[a-zA-Z]{2,16}$").IsMatch(args.Field));
            Map(m => m.TaxNo).Validate(args => new Regex(@"^[0-9]{10}$").IsMatch(args.Field));
        }
    }
}
