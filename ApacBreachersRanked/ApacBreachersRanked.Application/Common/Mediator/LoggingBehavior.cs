using MediatR;
using Microsoft.Extensions.Logging;

namespace ApacBreachersRanked.Application.Common.Mediator
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public LoggingBehavior(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = request.GetType().Name;
            var requestGuid = Guid.NewGuid().ToString();

            var requestNameWithGuid = $"{requestName} [{requestGuid}]";

            using (var logScope = _logger.BeginScope(requestNameWithGuid))
            {
                _logger.LogInformation($"[START]");
                TResponse response;

                try
                {
                    response = await next();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred when trying to execute {@Request}", request);
                    throw;
                }
                finally
                {
                    _logger.LogInformation(
                        $"[END] {requestNameWithGuid}");
                }
                return response;
            }
        }
    }
}
