using AgendaPlus.Application.Commands.Users;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Users;

public class UpdateUserCommandHandler(
    ILogger<UpdateUserCommandHandler> logger,
    UserQueryBuilder userQueryBuilder,
    IApplicationDbContext context) : IRequestHandler<UpdateUserCommand, Result>
{
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating user with ID: {Id}", request.Id);

        var user = await userQueryBuilder
            .GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User not found with ID: {Id}", request.Id);
            return Result.Failure("User not found");
        }

        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Document,
            request.BusinessName);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User updated successfully with ID: {Id}", request.Id);
        return Result.Success();
    }
}