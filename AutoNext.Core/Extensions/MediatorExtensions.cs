using AutoNext.Core.Exceptions;
using AutoNext.Core.Models;

namespace AutoNext.Core.Extensions
{
    public static class MediatorExtensions
    {
        public static async Task<Result<TResponse>> SendResultAsync<TResponse>(this Abstractions.IMediator mediator, Abstractions.IRequest<Result<TResponse>> request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(mediator);
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                return await mediator.Send(request, cancellationToken);
            }
            catch (ValidationException ex)
            {
                return Result.Failure<TResponse>(Error.Validation("ValidationFailed", ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Failure<TResponse>(Error.Unauthorized("Unauthorized", "User not authorized"));
            }
            catch (NotFoundException)
            {
                return Result.Failure<TResponse>(Error.NotFound("NotFound", "Resource not found"));
            }
            catch (Exception)
            {
                return Result.Failure<TResponse>(Error.Failure("InternalServerError", "An unexpected error occurred"));
            }
        }

        public static async Task<TResponse> SendSafeAsync<TResponse>(this Abstractions.IMediator mediator, Abstractions.IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(mediator);
            ArgumentNullException.ThrowIfNull(request);

            return await mediator.Send(request, cancellationToken);
        }

        public static async Task<Result> SendResultAsync(this Abstractions.IMediator mediator, Abstractions.IRequest<Result> request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(mediator);
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                return await mediator.Send(request, cancellationToken);
            }
            catch (ValidationException ex)
            {
                return Result.Failure(Error.Validation("ValidationFailed", ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Failure(Error.Unauthorized("Unauthorized", "User not authorized"));
            }
            catch (NotFoundException)
            {
                return Result.Failure(Error.NotFound("NotFound", "Resource not found"));
            }
            catch (Exception)
            {
                return Result.Failure(Error.Failure("InternalServerError", "An unexpected error occurred"));
            }
        }

        public static async Task<Abstractions.Unit> SendUnitAsync(this Abstractions.IMediator mediator, Abstractions.IRequest<Abstractions.Unit> request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(mediator);
            ArgumentNullException.ThrowIfNull(request);

            return await mediator.Send(request, cancellationToken);
        }
    }
}