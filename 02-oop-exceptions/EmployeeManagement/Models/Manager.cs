namespace EmployeeManagement.Models;

public class Manager : Employee
{
    // надбавка за кожного підлеглого
    public const double BonusPerSubordinate = 2000;

    private int _subordinatesCount;

    public int SubordinatesCount
    {
        get => _subordinatesCount;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(SubordinatesCount), "Кількість підлеглих не може бути від'ємною.");
            }

            _subordinatesCount = value;
        }
    }

    public override string Position => "Менеджер";

    public Manager(string fullName, double baseSalary, int subordinatesCount)
        : base(fullName, baseSalary)
    {
        SubordinatesCount = subordinatesCount;
    }

    // менеджер без підлеглих
    public Manager(string fullName, double baseSalary) : this(fullName, baseSalary, 0)
    {
    }

    public override double CalculateSalary() =>
        BaseSalary + SubordinatesCount * BonusPerSubordinate;

    public override string GetDetails() =>
        $"підлеглих: {SubordinatesCount} (+{SubordinatesCount * BonusPerSubordinate:F2} грн)";
}
