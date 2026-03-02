using AgendaPlus.Application.Commands;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers;

public class CreateUserCommandHandler(
    ILogger<CreateUserCommandHandler> logger,
    UserQueryBuilder userQueryBuilder,
    IApplicationDbContext applicationDbContext) : IRequestHandler<CreateUserCommand, Result>
{
    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to create user with email: {Email}", request.Email);

        var existingUser = await userQueryBuilder.GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser != null)
        {
            logger.LogWarning("User creation failed - email already exists: {Email}", request.Email);
            return Result.Failure("Email already in use");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var newUser = new User(userTenants: new List<UserTenant>(), email: request.Email, firstName: request.FirstName,
            lastName: request.LastName, passwordHash: hashedPassword, role: UserRole.Customer);

        applicationDbContext.Users.Add(newUser);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User created successfully with email: {Email} and last name: {LastName}", request.Email,
            request.LastName);

        return Result.Success();
    }
}