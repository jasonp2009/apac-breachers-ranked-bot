using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

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

            var timer = new Stopwatch();
            timer.Start();

            using (var logScope = _logger.BeginScope(requestNameWithGuid))
            {
                _logger.LogInformation("[START] {RequestNameWithGuid}",
                    requestNameWithGuid);
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
                    timer.Stop();
                    _logger.LogInformation(
                        "[END] {RequestNameWithGuid} RequestTime: {RequestTime}",
                        requestNameWithGuid,
                        timer.ElapsedMilliseconds);
                }
                return response;
            }
        }
    }
}
