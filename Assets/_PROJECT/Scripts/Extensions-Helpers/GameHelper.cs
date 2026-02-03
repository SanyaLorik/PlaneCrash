using System;

namespace _PROJECT.Scripts.Extensions_Helpers {
    public abstract class GameHelper {
        public static string ValuteFormatter(double value) {
            if (value < 1000)
                return Math.Ceiling(value).ToString();

            string[] suffixes = { "", "k", "kk", "kkk", "kkkk" };

            int tier = 0;

            while (value >= 1000 && tier < suffixes.Length - 1) {
                value /= 1000;
                tier++;
            }

            long rounded = (long)Math.Ceiling(value);

            return rounded + suffixes[tier];
        }
    }
}
