using System;

namespace Homework.Utils
{
    public class DateTimeUtils
    {
        private static Random rnd = new Random();
        private static DateTime RandomDate(DateTime startDate, DateTime endDate)
        {
            int range = (endDate - startDate).Days;
            int randomDays = rnd.Next(range);
            return startDate.AddDays(randomDays);
        }

        private static int AgeCalculate(DateTime randomDate)
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
    }
}
