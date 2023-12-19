using AddressOwnershipTool.Common;

namespace AddressOwnershipTool.Commands.Scan;

public class ScanCommandHandler : ICommandHandler<ScanCommand, Result>
{
    private readonly ISwapExtractionServiceFactory _swapExtractionServiceFactory;

    public ScanCommandHandler(ISwapExtractionServiceFactory swapExtractionServiceFactory)
    {
        _swapExtractionServiceFactory = swapExtractionServiceFactory;
    }

    public async Task<Result> Handle(ScanCommand request, CancellationToken cancellationToken)
    {
        var service = _swapExtractionServiceFactory.CreateSwapExtractionServiceFactory(request.Testnet, request.UseCirrus);

        await service.RunAsync(request.StartBlock);

        return Result.Ok();
    }
}
