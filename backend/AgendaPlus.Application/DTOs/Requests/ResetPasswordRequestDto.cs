namespace AgendaPlus.Application.DTOs.Requests;

public record ResetPasswordRequestDto(string Token, string NewPassword, string ConfirmPassword);