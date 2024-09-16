using FluentValidation.Results;

namespace BridgeInfrastructure.Behaviors;

public class ValidatorBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators, ILogger logger)
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Accepts a request and validates it using the provided validators.
    /// If the request is invalid, logs the validation failures and throws a ValidationException.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public async Task ValidateAsync(TRequest request)
    {
        ValidationContext<TRequest> context = new(request);

        ValidationResult[] validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context)));

        var failures = validationResults
            .Where(r => r != null && !r.IsValid) // Combined filtering
            .SelectMany(r => r.Errors)           // Flatten the errors
            .ToList();                           // Convert to a list


        if (failures.Count != 0) // If there are any validation failures
        {
            _logger.Error($"Validation failures for {typeof(TRequest).Name}: " +
                $"{string.Join(", ", failures.Select(f => f.ErrorMessage))}");

            throw new ValidationException(failures); // Throw the exception with the collected failures
        }
    }
}
