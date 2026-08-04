using DrivingLicenseManagement.Application.Common.Errors;
using DrivingLicenseManagement.Application.Common.Interfaces;
using DrivingLicenseManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace DrivingLicenseManagement.Application.Features.TestTypes.Commands.UpdateTestType;

public sealed class UpdateTestTypeCommandHandler(
    IAppDbContext context,
    ILogger<UpdateTestTypeCommandHandler> logger,
    HybridCache cache)
    : IRequestHandler<UpdateTestTypeCommand, Result>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdateTestTypeCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result> Handle(UpdateTestTypeCommand request, CancellationToken cancellationToken)
    {
        var testType = await _context.TestTypes
            .FirstOrDefaultAsync(testType => testType.Id == request.Id, cancellationToken);

        if (testType is null)
        {
            _logger.LogWarning("Test type with id {TestTypeId} was not found.", request.Id);

            return Result.Failure(ApplicationErrors.TestTypeNotFound);
        }

        testType.TestTypeTitle = request.TestTypeTitle.Trim();
        testType.TestTypeDescription = request.TestTypeDescription.Trim();
        testType.TestTypeFees = request.TestTypeFees;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Test type with id {TestTypeId} was updated successfully.", testType.Id);

        await _cache.RemoveByTagAsync("test-types", cancellationToken);

        return Result.Success();
    }
}
