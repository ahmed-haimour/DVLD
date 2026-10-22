using DrivingLicenseManagement.Application.Common.Models;
using DrivingLicenseManagement.Application.Features.Tests.Tests.Dtos;
using MediatR;

namespace DrivingLicenseManagement.Application.Features.Tests.Tests.Queries.GetTestDetails;

public sealed record GetTestDetailsQuery(
    Guid TestId) : IRequest<Result<TestDetailsDto>>;
