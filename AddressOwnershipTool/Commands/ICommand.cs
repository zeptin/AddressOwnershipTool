using MediatR;

namespace AddressOwnershipTool.Commands;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
