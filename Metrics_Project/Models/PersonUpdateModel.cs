namespace Metrics_Project.Models;

public record PersonUpdateModel(
    string FirstName,
    string LastName,
    string? MiddleName,
    DateTime BirthdayDate
);