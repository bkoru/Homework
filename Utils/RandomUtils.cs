using System;

namespace Homework.Utils
{
    public class RandomUtils
    {
        private static Random rnd = new Random();

        public static int RandomIndex(int count)
        {
            return rnd.Next(count);
        }
        public static DateTime RandomDate(DateTime startDate, DateTime endDate)
        {
            int range = (endDate - startDate).Days;
            int randomDays = rnd.Next(range);
            return startDate.AddDays(randomDays);
        }


        public static int[] RandomEmployeeId()
        {
            int[] randomNum = new int[3];
            for (int i = 0; i < 3; i++)
            {
                randomNum[i] = rnd.Next(1, 3);
            }
            return randomNum;
        }

        public static int RandomCompanyId()
        {
            int[] RandomCompanyId;
            RandomCompanyId = new int[100];
            return rnd.Next(RandomCompanyId.Length);
        }

        public static decimal RandomSalary(int min, int max)
        {
            return Convert.ToDecimal((double)rnd.Next(min, max) + rnd.NextDouble());
        }
    }
}
