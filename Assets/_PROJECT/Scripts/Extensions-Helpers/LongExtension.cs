using System;

public static class LongExtension {
    // Безопасное умножение long на float
    public static long SafeMultiply(this long value, float multiplier) {
        try {
            checked {
                // Умножаем как double для сохранения дробной части, потом кастуем в long
                double result = value * multiplier;
                if (result > long.MaxValue) return long.MaxValue;
                if (result < long.MinValue) return long.MinValue;
                return (long)result;
            }
        }
        catch (OverflowException) {
            return multiplier > 0 ? long.MaxValue : long.MinValue;
        }
    }
    
    // Безопасное умножение long на int
    public static long SafeMultiply(this long value, int multiplier) {
        try {
            checked {
                return value * multiplier;
            }
        }
        catch (OverflowException) {
            return multiplier > 0 ? long.MaxValue : long.MinValue;
        }
    }
    
    // Безопасное сложение long + long
    public static long SafeAdd(this long value, long add) {
        try {
            checked {
                return value + add;
            }
        }
        catch (OverflowException) {
            return add > 0 ? long.MaxValue : long.MinValue;
        }
    }
    
    // Безопасное сложение long + float
    public static long SafeAdd(this long value, float add) {
        try {
            checked {
                double result = value + add;
                if (result > long.MaxValue) return long.MaxValue;
                if (result < long.MinValue) return long.MinValue;
                return (long)result;
            }
        }
        catch (OverflowException) {
            return add > 0 ? long.MaxValue : long.MinValue;
        }
    }
    
    
}