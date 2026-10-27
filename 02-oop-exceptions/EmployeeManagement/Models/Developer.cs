namespace EmployeeManagement.Models;

public class Developer : Employee
{
    private DeveloperLevel _level;

    public DeveloperLevel Level
    {
        get => _level;
        set
        {
            // в enum можна засунути будь-яке число, тому перевіряємо
            if (!Enum.IsDefined(value))
            {
                throw new ArgumentOutOfRangeException(nameof(Level), "Невідомий рівень розробника.");
            }

            _level = value;
        }
    }

    // надбавка залежно від рівня
    public double LevelBonus => Level switch
    {
        DeveloperLevel.Junior => 0,
        DeveloperLevel.Middle => 5000,
        DeveloperLevel.Senior => 10000,
        _ => 0
    };

    public override string Position => "Розробник";

    public Developer(string fullName, double baseSalary, DeveloperLevel level)
        : base(fullName, baseSalary)
    {
        Level = level;
    }

    // якщо рівень не вказали, то Junior
    public Developer(string fullName, double baseSalary) : this(fullName, baseSalary, DeveloperLevel.Junior)
    {
    }

    public override double CalculateSalary() => BaseSalary + LevelBonus;

    public override string GetDetails() => $"рівень: {Level} (+{LevelBonus:F2} грн)";
}
