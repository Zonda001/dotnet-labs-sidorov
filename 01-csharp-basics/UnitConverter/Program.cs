using System.Globalization;
using System.Text;

namespace UnitConverter;

// ПР1, варіант 3: конвертер величин
class Program
{
    // коефіцієнти перерахунку (одна константа на пару одиниць)
    const double KmPerMile = 1.609344;
    const double KgPerPound = 0.45359237;

    // абсолютний нуль
    const double AbsoluteZeroCelsius = -273.15;
    const double AbsoluteZeroFahrenheit = -459.67;

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // щоб числа виводились з крапкою незалежно від мови системи
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        bool isRunning = true;

        while (isRunning)
        {
            PrintMenu();

            int choice = ReadInt("Ваш вибір: ", 0, 6);

            isRunning = choice switch
            {
                >= 1 and <= 6 => HandleConversion(choice),
                0 => false,
                _ => true
            };
        }

        Console.WriteLine("Роботу завершено.");
    }

    static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("========= Конвертер величин =========");
        Console.WriteLine(" Відстань:     1. км -> милі     2. милі -> км");
        Console.WriteLine(" Маса:         3. кг -> фунти    4. фунти -> кг");
        Console.WriteLine(" Температура:  5. °C -> °F       6. °F -> °C");
        Console.WriteLine(" 0. Вихід");
        Console.WriteLine("=====================================");
    }

    // пункти 1-6: ввід значення, перевірка, конвертація і вивід
    static bool HandleConversion(int direction)
    {
        string fromUnit = GetSourceUnit(direction);
        string toUnit = GetTargetUnit(direction);

        Console.WriteLine($"\n--- {GetDirectionTitle(direction)} ---");
        double value = ReadDouble($"Введіть значення ({fromUnit}): ");

        if (!IsPhysicallyValid(direction, value, out string errorMessage))
        {
            Console.WriteLine($"Попередження: {errorMessage} Конвертацію не виконано.");
            return true;
        }

        double result = ConvertValue(direction, value);

        // на дуже великих числах може вийти нескінченність
        if (!double.IsFinite(result))
        {
            Console.WriteLine("Попередження: результат виходить за межі допустимого діапазону чисел.");
            return true;
        }

        Console.WriteLine($"Результат: {value} {fromUnit} = {FormatResult(result)} {toUnit}");
        return true;
    }

    // великі числа виводимо в експоненційному вигляді, інакше буде сотня цифр
    static string FormatResult(double result) =>
        Math.Abs(result) < 1e12 ? $"{result:F2}" : $"{result:E3}";

    static string GetDirectionTitle(int direction) => direction switch
    {
        1 => "Кілометри в милі",
        2 => "Милі в кілометри",
        3 => "Кілограми у фунти",
        4 => "Фунти в кілограми",
        5 => "Градуси Цельсія в градуси Фаренгейта",
        6 => "Градуси Фаренгейта в градуси Цельсія",
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };

    static string GetSourceUnit(int direction) => direction switch
    {
        1 => "км",
        2 => "милі",
        3 => "кг",
        4 => "фунти",
        5 => "°C",
        6 => "°F",
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };

    static string GetTargetUnit(int direction) => direction switch
    {
        1 => "милі",
        2 => "км",
        3 => "фунти",
        4 => "кг",
        5 => "°F",
        6 => "°C",
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };

    // відстань і маса не можуть бути від'ємними, температура не нижче абсолютного нуля
    static bool IsPhysicallyValid(int direction, double value, out string errorMessage)
    {
        errorMessage = string.Empty;

        switch (direction)
        {
            case 1:
            case 2:
                if (value < 0)
                {
                    errorMessage = "відстань не може бути від'ємною.";
                    return false;
                }
                return true;

            case 3:
            case 4:
                if (value < 0)
                {
                    errorMessage = "маса не може бути від'ємною.";
                    return false;
                }
                return true;

            case 5:
                if (value < AbsoluteZeroCelsius)
                {
                    errorMessage = $"температура не може бути нижчою за абсолютний нуль ({AbsoluteZeroCelsius} °C).";
                    return false;
                }
                return true;

            case 6:
                if (value < AbsoluteZeroFahrenheit)
                {
                    errorMessage = $"температура не може бути нижчою за абсолютний нуль ({AbsoluteZeroFahrenheit} °F).";
                    return false;
                }
                return true;

            default:
                errorMessage = "невідомий напрямок конвертації.";
                return false;
        }
    }

    static double ConvertValue(int direction, double value) => direction switch
    {
        1 => KilometersToMiles(value),
        2 => MilesToKilometers(value),
        3 => KilogramsToPounds(value),
        4 => PoundsToKilograms(value),
        5 => CelsiusToFahrenheit(value),
        6 => FahrenheitToCelsius(value),
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };

    // методи конвертації
    static double KilometersToMiles(double kilometers) => kilometers / KmPerMile;

    static double MilesToKilometers(double miles) => miles * KmPerMile;

    static double KilogramsToPounds(double kilograms) => kilograms / KgPerPound;

    static double PoundsToKilograms(double pounds) => pounds * KgPerPound;

    static double CelsiusToFahrenheit(double celsius) => celsius * 9 / 5 + 32;

    static double FahrenheitToCelsius(double fahrenheit) => (fahrenheit - 32) * 5 / 9;

    // ввід цілого числа в заданому діапазоні, питає поки не введуть нормально
    static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            // ввід закрили (Ctrl+Z), просто виходимо
            if (input is null)
            {
                Environment.Exit(0);
            }

            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Некоректне значення. Введіть ціле число від {min} до {max}.");
        }
    }

    // ввід дробового числа, підходить і крапка, і кома
    static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (input is null)
            {
                Environment.Exit(0);
            }

            string normalized = input.Trim().Replace(',', '.');

            // TryParse пропускає NaN та Infinity, тому перевіряємо ще IsFinite
            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                && double.IsFinite(value))
            {
                return value;
            }

            Console.WriteLine("Некоректне значення. Введіть число (наприклад, 12.5 або 12,5).");
        }
    }
}
