using MediatR;

namespace DrivingLicenseManagement.Application.Common.Interfaces;

public interface ICachedQuery
{
    string CacheKey { get; }
    string[] Tags { get; }
}

public interface ICachedQuery<TResponse> : IRequest<TResponse>, ICachedQuery;
