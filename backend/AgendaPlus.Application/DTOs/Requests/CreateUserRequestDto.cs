namespace AgendaPlus.Application.DTOs.Requests;

public record CreateUserRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword);