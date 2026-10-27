using System.Globalization;
using System.Text;
using EmployeeManagement.Exceptions;
using EmployeeManagement.Models;

namespace EmployeeManagement;

// ПР2, варіант 3: співробітники компанії
class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        // список базового типу, а всередині лежать менеджери, розробники і т.д.
        List<Employee> employees = new()
        {
            new Manager("Коваленко Олена Петрівна", 35000, 6),
            new Developer("Шевчук Андрій Іванович", 30000, DeveloperLevel.Senior),
            new Developer("Мельник Ірина Сергіївна", 28000, DeveloperLevel.Middle),
            new Developer("Бондар Максим Олегович", 22000),
            new Employee("Ткаченко Наталія Василівна", 18000),
            new Manager("Кравченко Олег Миколайович", 32000, 3)
        };

        bool isRunning = true;

        while (isRunning)
        {
            PrintMenu();

            int choice = ReadInt("Ваш вибір: ", 0, 5);

            isRunning = choice switch
            {
                1 => HandleAddEmployee(employees),
                2 => HandleShowAll(employees),
                3 => HandleShowPayrollFund(employees),
                4 => HandleShowSortedBySalary(employees),
                5 => HandleChangeBaseSalary(employees),
                0 => false,
                _ => true
            };
        }

        Console.WriteLine("Роботу завершено.");
    }

    static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=========== Співробітники компанії ===========");
        Console.WriteLine("1. Додати співробітника");
        Console.WriteLine("2. Показати список співробітників із зарплатою");
        Console.WriteLine("3. Показати фонд заробітної плати");
        Console.WriteLine("4. Показати співробітників за зарплатою (від більшої до меншої)");
        Console.WriteLine("5. Змінити базову ставку співробітника");
        Console.WriteLine("0. Вихід");
        Console.WriteLine("==============================================");
    }

    // 1. додавання співробітника
    static bool HandleAddEmployee(List<Employee> employees)
    {
        Console.WriteLine("\n--- Новий співробітник ---");
        Console.WriteLine("Посада: 1. Співробітник (без надбавок)  2. Менеджер  3. Розробник");
        int position = ReadInt("Оберіть посаду: ", 1, 3);

        Console.Write("ПІБ: ");
        string fullName = Console.ReadLine() ?? string.Empty;

        double baseSalary = ReadDouble("Базова ставка, грн: ");

        // числа вже перевірені TryParse, а на допустимість значень перевіряють самі класи
        try
        {
            Employee employee = position switch
            {
                2 => new Manager(fullName, baseSalary, ReadInt("Кількість підлеглих: ")),
                3 => new Developer(fullName, baseSalary, ReadDeveloperLevel()),
                _ => new Employee(fullName, baseSalary)
            };

            employees.Add(employee);
            Console.WriteLine($"Додано. {employee}");
        }
        catch (InvalidSalaryException ex)
        {
            Console.WriteLine($"Помилка зарплати: {ex.Message} Співробітника не додано.");
        }
        // ArgumentOutOfRangeException наслідується від ArgumentException, тому має стояти вище
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Некоректне значення: {DescribeArgumentError(ex)} Співробітника не додано.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Некоректні дані: {DescribeArgumentError(ex)} Співробітника не додано.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Непередбачена помилка: {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"Співробітників у списку: {employees.Count}.");
        }

        return true;
    }

    // 2. список
    static bool HandleShowAll(List<Employee> employees)
    {
        Console.WriteLine("\n--- Список співробітників ---");
        PrintTable(employees);
        return true;
    }

    // 3. фонд зарплати
    static bool HandleShowPayrollFund(List<Employee> employees)
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return true;
        }

        double totalBase = 0;
        double totalSalary = 0;
        Employee topEarner = employees[0];

        foreach (Employee employee in employees)
        {
            totalBase += employee.BaseSalary;
            totalSalary += employee.CalculateSalary();

            if (employee.CompareTo(topEarner) > 0)
            {
                topEarner = employee;
            }
        }

        Console.WriteLine("\n--- Фонд заробітної плати ---");
        Console.WriteLine($"Співробітників:             {employees.Count}");
        Console.WriteLine($"Сума базових ставок:        {totalBase,12:F2} грн");
        Console.WriteLine($"Сума надбавок:              {totalSalary - totalBase,12:F2} грн");
        Console.WriteLine($"Фонд заробітної плати:      {totalSalary,12:F2} грн");
        Console.WriteLine($"Середня зарплата:           {totalSalary / employees.Count,12:F2} грн");
        Console.WriteLine($"Найбільша зарплата:         {topEarner.FullName} ({topEarner.CalculateSalary():F2} грн)");
        return true;
    }

    // 4. сортування від більшої зарплати до меншої
    static bool HandleShowSortedBySalary(List<Employee> employees)
    {
        // сортуємо копію, щоб у пункті 5 номери співробітників не змінювались
        List<Employee> sorted = new(employees);
        sorted.Sort((a, b) => b.CompareTo(a));

        Console.WriteLine("\n--- Співробітники за зарплатою (від більшої до меншої) ---");
        PrintTable(sorted);
        return true;
    }

    // 5. зміна ставки, тут перевіряється граничний випадок з від'ємною ставкою
    static bool HandleChangeBaseSalary(List<Employee> employees)
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return true;
        }

        Console.WriteLine("\n--- Зміна базової ставки ---");
        PrintTable(employees);

        int number = ReadInt($"Номер співробітника (1-{employees.Count}): ", 1, employees.Count);
        Employee employee = employees[number - 1];

        double newSalary = ReadDouble($"Нова базова ставка для {employee.FullName}, грн: ");

        try
        {
            double oldSalary = employee.BaseSalary;
            employee.BaseSalary = newSalary;
            Console.WriteLine($"Ставку змінено з {oldSalary:F2} на {employee.BaseSalary:F2} грн. " +
                              $"Нова зарплата: {employee.CalculateSalary():F2} грн.");
        }
        catch (InvalidSalaryException ex)
        {
            Console.WriteLine($"Помилка зарплати: {ex.Message}");
            Console.WriteLine($"Ставку не змінено, поточне значення: {employee.BaseSalary:F2} грн.");
        }

        return true;
    }

    static void PrintTable(List<Employee> employees)
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return;
        }

        Console.WriteLine($"{"№",3}  {"Посада",-12} {"ПІБ",-30} {"Ставка",11} {"Зарплата",11}  Деталі");
        Console.WriteLine(new string('-', 100));

        int index = 1;
        foreach (Employee employee in employees)
        {
            // Position, CalculateSalary і GetDetails викликаються для реального типу об'єкта
            Console.WriteLine(
                $"{index,3}. {employee.Position,-12} {employee.FullName,-30} " +
                $"{employee.BaseSalary,11:F2} {employee.CalculateSalary(),11:F2}  {employee.GetDetails()}");
            index++;
        }
    }

    // .NET дописує до повідомлення " (Parameter 'X')", прибираємо це
    static string DescribeArgumentError(ArgumentException ex) =>
        ex.ParamName is null
            ? ex.Message
            : ex.Message.Replace($" (Parameter '{ex.ParamName}')", string.Empty);

    static DeveloperLevel ReadDeveloperLevel()
    {
        Console.WriteLine("Рівень: 1. Junior  2. Middle  3. Senior");
        int level = ReadInt("Оберіть рівень: ", 1, 3);

        return level switch
        {
            1 => DeveloperLevel.Junior,
            2 => DeveloperLevel.Middle,
            _ => DeveloperLevel.Senior
        };
    }

    static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (input is null)
            {
                Environment.Exit(0);
            }

            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine(min == int.MinValue && max == int.MaxValue
                ? "Некоректне значення. Введіть ціле число."
                : $"Некоректне значення. Введіть ціле число від {min} до {max}.");
        }
    }

    // підходить і крапка, і кома
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

            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                && double.IsFinite(value))
            {
                return value;
            }

            Console.WriteLine("Некоректне значення. Введіть число (наприклад, 25000 або 25000,50).");
        }
    }
}
