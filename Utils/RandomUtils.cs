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


        public static int RandomId()
        {
            int[] randomID;
            randomID = new int[100];
            return rnd.Next(randomID.Length);
        }

        public static decimal RandomSalary(int min, int max)
        {
            return Convert.ToDecimal((double)rnd.Next(min, max) + rnd.NextDouble());
        }
    }
}
