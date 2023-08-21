using BookStore.Domain.Entities;
using MediatR;

namespace BookStore.Application.Services;

public abstract class AppService
{
    private readonly IMediator _mediator;

    protected AppService(IMediator mediator)
    {
        _mediator = mediator;
    }

    protected async Task RaiseDomainEvents(Entity entity)
    {
        if (entity.DomainEvents is null) return;

        foreach (var domainEvent in entity.DomainEvents) await _mediator.Publish(domainEvent);

        entity.ClearDomainEvents();
    }
}