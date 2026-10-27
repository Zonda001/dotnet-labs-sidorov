namespace EmployeeManagement.Exceptions;

// виняток для некоректної зарплати (нуль або від'ємне значення)
public class InvalidSalaryException : Exception
{
    public InvalidSalaryException()
    {
    }

    public InvalidSalaryException(string message) : base(message)
    {
    }

    public InvalidSalaryException(string message, Exception inner) : base(message, inner)
    {
    }
}
