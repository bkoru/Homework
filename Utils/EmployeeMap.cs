using CsvHelper.Configuration;
using Homework.Entities;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Homework.Utils
{
    class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Id).Ignore();
            Map(m => m.Company.TaxNo).Ignore();
            Map(m => m.Company.Id).Ignore();
            Map(m => m.Name).Validate(args => new Regex(@"^[-a-zA-Z0-9]{2,16}$").IsMatch(args.Field));
            Map(m => m.Salary).Validate(args => new Regex(@"^[0-9]{4,5}$").IsMatch(args.Field));
            Map(m => m.TrId).Validate(args => new Regex(@"^[1-9]{1}[0-9]{10}$").IsMatch(args.Field));
        }
    }
}
