namespace ChatService.Utils;
using Polly;
using Polly.CircuitBreaker;

public class PollyCircuitBreaker
{
    public static AsyncCircuitBreakerPolicy CreateCircuitBreakerPolicy()
    {
        return Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 3, // After 3 exceptions, the circuit "opens"
                durationOfBreak: TimeSpan.FromSeconds(30), // Circuit remains open for 30 seconds
                onBreak: (exception, timespan) =>
                {
                    Console.WriteLine($"Circuit broken! Breaking for {timespan.TotalSeconds} seconds due to: {exception.Message}");
                },
                onReset: () =>
                {
                    Console.WriteLine("Circuit reset. Operations will be attempted again.");
                },
                onHalfOpen: () =>
                {
                    Console.WriteLine("Circuit is half-open. Testing the service...");
                });
    }
}