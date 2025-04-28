using Metrics_Project.Entities;

namespace Metrics_Project.Models;

public record PersonViewModel(
    int Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    DateTime BirthdayDate)
{
    public static PersonViewModel FromEntity(Person person)
    {
        return new PersonViewModel(
            person.Id,
            person.FirstName,
            person.LastName,
            person.MiddleName,
            person.BirthdayDate
        );
    }
}