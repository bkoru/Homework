﻿using CsvHelper.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Homework.CsvHelper
{
    class EmployeeMap : ClassMap<EmployeeCsvHelperDto>
    {
        public EmployeeMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Name).Validate(args => new Regex(@"^[a-zA-Z]{2,16}$").IsMatch(args.Field));
            Map(m => m.Salary).Validate(args => new Regex(@"^[0-9]{4,5}$").IsMatch(args.Field));
            Map(m => m.TrId).Validate(args => new Regex(@"^[1-9]{1}[0-9]{10}$").IsMatch(args.Field));
        }
    }
}
