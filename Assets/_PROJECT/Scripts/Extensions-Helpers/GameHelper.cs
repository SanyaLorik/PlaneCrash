using System;

namespace _PROJECT.Scripts.Extensions_Helpers {
    public abstract class GameHelper {
        public static string ValuteFormatter(double value) {
            if (value < 1000)
                return Math.Floor(value).ToString();

            string[] suffixes = { "", "K", "M", "B", "T" };

            int tier = (int)Math.Floor(Math.Log10(value) / 3);
            tier = Math.Min(tier, suffixes.Length - 1);

            double scaled = value / Math.Pow(1000, tier);

            // Оставляем максимум 2 знака после запятой
            string formatted = scaled.ToString("0.##");

            return formatted + suffixes[tier];
        }
    }

}
