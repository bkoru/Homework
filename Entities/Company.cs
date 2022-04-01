using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Company()
        {
        }

        public Company(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static void CreateCompany(DataSlot dataSlot, int total, int id)
        {
            for (int i = 0; i < total; i++)
            {
                var company = new Company(id, "Company" + (i + 1).ToString());
                dataSlot.Companies.Add(company);
            }
        }
    }
}
