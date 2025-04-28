namespace Metrics_Project.Models;

public record PersonCreateModel(
    string FirstName,
    string LastName,
    string? MiddleName,
    DateTime BirthdayDate
);