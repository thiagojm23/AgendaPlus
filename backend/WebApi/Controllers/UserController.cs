using AgendaPlus.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPlus.WebApi.Controllers;

public class UserController(ICurrentUserService currentUser) : ControllerBase
{
    private readonly ICurrentUserService _currentUserService = currentUser;
}