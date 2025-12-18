using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Routes.Commands.UpdateRoute
{
    public class UpdateRouteCommandValidator : AbstractValidator<UpdateRouteCommand>
    {
        private readonly IRouteRepository _routeRepository;

        public UpdateRouteCommandValidator(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Route ID is required")
                .MustAsync(RouteExists).WithMessage("Route not found");

            RuleFor(x => x.DistanceKm)
                .GreaterThan(0).When(x => x.DistanceKm.HasValue)
                .WithMessage("Distance must be greater than 0");
        }

        private async Task<bool> RouteExists(int id, CancellationToken cancellationToken)
        {
            return await _routeRepository.GetTableNoTracking().AnyAsync(r => r.Id == id, cancellationToken);
        }
    }
}

