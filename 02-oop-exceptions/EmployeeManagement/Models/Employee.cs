using EmployeeManagement.Exceptions;

namespace EmployeeManagement.Models;

// базовий клас співробітника
public class Employee : IComparable<Employee>
{
    public const double DefaultBaseSalary = 20000;

    private string _fullName = string.Empty;
    private double _baseSalary;

    public string FullName
    {
        get => _fullName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("ПІБ співробітника не може бути порожнім.", nameof(FullName));
            }

            _fullName = value.Trim();
        }
    }

    // перевірка тут, щоб некоректна ставка не потрапила в об'єкт ні через конструктор, ні пізніше
    public double BaseSalary
    {
        get => _baseSalary;
        set
        {
            if (value <= 0)
            {
                throw new InvalidSalaryException(
                    $"Базова ставка має бути додатною, отримано: {value:F2} грн.");
            }

            _baseSalary = value;
        }
    }

    public virtual string Position => "Співробітник";

    public Employee(string fullName, double baseSalary)
    {
        FullName = fullName;
        BaseSalary = baseSalary;
    }

    // якщо ставку не вказали, береться стандартна
    public Employee(string fullName) : this(fullName, DefaultBaseSalary)
    {
    }

    // звичайний співробітник отримує тільки ставку
    public virtual double CalculateSalary() => BaseSalary;

    public virtual string GetDetails() => "без надбавок";

    // порівнюємо по фактичній зарплаті, а не по ставці
    public int CompareTo(Employee? other)
    {
        if (other is null)
        {
            return 1;
        }

        return CalculateSalary().CompareTo(other.CalculateSalary());
    }

    public override string ToString() =>
        $"{Position}: {FullName}, ставка {BaseSalary:F2} грн, зарплата {CalculateSalary():F2} грн";
}
