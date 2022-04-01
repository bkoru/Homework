using System.Linq;

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

        public static void CreateCompany(DataSlot dataSlot, int total)
        {
            var biggerId = 0;
            if (dataSlot.Companies.Count > 0)
            {
                biggerId = dataSlot.Companies.Max(e => e.Id);
            }
            for (int i = 0; i < total; i++)
            {
                var company = new Company(++biggerId, "Company" + (i + 1).ToString());
                dataSlot.Companies.Add(company);
            }
        }
    }
}
