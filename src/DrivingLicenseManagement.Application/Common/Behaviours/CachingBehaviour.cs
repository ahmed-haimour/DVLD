using DrivingLicenseManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Common.Behaviours;

public sealed class CachingBehavior<TRequest, TResponse>(HybridCache cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : ICachedQuery<TResponse>
{
    private readonly HybridCache _cache = cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _logger.LogInformation("Checking cache for {RequestName}", typeof(TRequest).Name);

        bool shouldReturnUncached = false;
        TResponse? uncachedResponse = default;

        var response = await _cache.GetOrCreateAsync(
            request.CacheKey,
            async ct =>
            {
                var result = await next(ct);

                if (result is IResult r && r.IsSuccess)
                {
                    _logger.LogInformation(
                        "Caching response for {RequestName}",
                        typeof(TRequest).Name);

                    return result;
                }

                _logger.LogInformation(
                    "Skipping cache for failed {RequestName}",
                    typeof(TRequest).Name);

                shouldReturnUncached = true;
                uncachedResponse = result;

                return default!;
            },
            tags: request.Tags,
            cancellationToken: ct);

        return shouldReturnUncached
            ? uncachedResponse!
            : response;
    }

}