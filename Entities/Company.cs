using System;
using System.Linq;

namespace Homework.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxNo{ get; set; }

        public Company()
        {
        }

        public Company(int id, string name, string taxNo)
        {
            Id = id;
            Name = name;
            TaxNo = taxNo;
        }

        public static string RandomTaxNo()
        {
            Random rnd = new Random();
            string taxNo = null;

            for (int i = 1; i <= 10; i++)
            {
                taxNo += rnd.Next(0, 9);
            }

            return taxNo;
        }

        public static void CreateCompany(DataSlot dataSlot, int total)
        {
            var biggerId = 0;
            if (dataSlot.Companies.Count > 0)
            {
                biggerId = dataSlot.Companies.Max(e => e.Id);
            }
            for (int i = 0; i < total; i++)
            {
                var taxNo = RandomTaxNo();

                while (dataSlot.Companies.Where(e => e.TaxNo == taxNo).FirstOrDefault() != null)
                {
                    taxNo = RandomTaxNo();
                }

                var company = new Company(++biggerId, "Company" + biggerId, taxNo);
                dataSlot.Companies.Add(company);
            }
        }
    }
}
