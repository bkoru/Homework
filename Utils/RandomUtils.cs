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

        public static int AgeCalculate(DateTime randomDate)
        {
            int todayYear = DateTime.Now.Year;
            int todayMonth = DateTime.Now.Month;
            int todayDay = DateTime.Now.Day;
            int birthYear = randomDate.Year;
            int birthMonth = randomDate.Month;
            int birthDay = randomDate.Day;
            int age = todayYear - birthYear;
            if (todayMonth < birthMonth || (todayMonth == birthMonth && todayDay < birthDay))
                age--;

            return age;
        }

        public static int RandomId()
        {
            return rnd.Next(1, 999999999);
        }

        public static decimal RandomSalary(int min, int max)
        {
            return Convert.ToDecimal((double)rnd.Next(min, max) + rnd.NextDouble());
        }
    }
}
