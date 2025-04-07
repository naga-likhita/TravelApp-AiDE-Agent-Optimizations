using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Diagnostics;

namespace TravelApp;

using Microsoft.Extensions.Configuration;

public class QueryMetricsInterceptor : DbCommandInterceptor
{
    private readonly ILogger<QueryMetricsInterceptor> _logger;
    private readonly long _longQueryThresholdMs;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public QueryMetricsInterceptor(
        ILogger<QueryMetricsInterceptor> logger,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _longQueryThresholdMs = config.GetValue<long>("QueryMetrics:LongQueryThresholdMs", 500);
        _httpContextAccessor = httpContextAccessor;
    }

    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        LogQueryMetrics(command, eventData);
        return base.ReaderExecuted(command, eventData, result);
    }

    private void LogQueryMetrics(DbCommand command, CommandExecutedEventData eventData)
    {
        var durationMs = eventData.Duration.TotalMilliseconds;
        var correlationId = _httpContextAccessor.HttpContext?.TraceIdentifier;

        var logData = new Dictionary<string, object>
        {
            ["CommandText"] = command.CommandText,
            ["Parameters"] = command.Parameters,
            ["DurationMs"] = durationMs,
            ["CorrelationId"] = correlationId ?? "N/A"
        };

        var logLevel = durationMs > _longQueryThresholdMs ? LogLevel.Warning : LogLevel.Debug;

        _logger.Log(logLevel, "EF Query executed in {DurationMs}ms{Correlation} - {CommandText}",
            durationMs,
            correlationId != null ? $" [CID: {correlationId}]" : "",
            command.CommandText);

        _logger.Log(logLevel, "Query Metrics: {@Metrics}", logData);
    }
}
