using AgendaPlus.Application.Commands.Users;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Users;

public class CreateConsumerUserCommandHandler(
    ILogger<CreateConsumerUserCommandHandler> logger,
    UserQueryBuilder userQueryBuilder,
    IApplicationDbContext context) : IRequestHandler<CreateConsumerUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateConsumerUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating consumer user with email: {Email}", request.Email);

        var existingUser = await userQueryBuilder
            .AsNoTracking()
            .GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser != null)
        {
            logger.LogWarning("Email already in use: {Email}", request.Email);
            return Result.Failure<Guid>("Email is already in use");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User(
            request.FirstName,
            request.LastName,
            request.Email,
            hashedPassword,
            UserRole.Customer,
            new List<UserTenant>());

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Consumer user created with ID: {UserId}", user.Id);

        return Result.Success(user.Id);
    }
}