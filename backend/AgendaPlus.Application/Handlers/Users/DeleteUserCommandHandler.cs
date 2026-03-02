using AgendaPlus.Application.Commands.Users;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Users;

public class DeleteUserCommandHandler(
    ILogger<DeleteUserCommandHandler> logger,
    UserQueryBuilder userQueryBuilder,
    IApplicationDbContext context) : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deactivating user with ID: {Id}", request.Id);

        var user = await userQueryBuilder
            .GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User not found with ID: {Id}", request.Id);
            return Result.Failure("User not found");
        }

        user.Deactivate();
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User deactivated successfully with ID: {Id}", request.Id);
        return Result.Success();
    }
}