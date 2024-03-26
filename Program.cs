using Polly.Timeout;
using Polly;
using PollySample;

var pipeline = new ResiliencePipelineBuilder<bool>()
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(1),
            })
            .AddRetry(new()
            {
                Delay = TimeSpan.FromMilliseconds(1),
                MaxRetryAttempts = int.MaxValue,
                BackoffType = DelayBackoffType.Constant,
                ShouldHandle = args => ValueTask.FromResult(args.Outcome.Result),
            })
            .Build();

var longTask = new HandleTask(TimeSpan.FromSeconds(2));
CancellationToken cancellationToken = new CancellationToken();

// cancellation token used to cancel the task, TimeoutRejectedException is thrown
try
{
    await pipeline.ExecuteAsync<bool>(ct => new ValueTask<bool>(longTask.ExecuteAsync(ct)), cancellationToken);

    Console.WriteLine("Attempt 1: Task completed successfully");
}
catch (TimeoutRejectedException e)
{
    Console.WriteLine("Attempt 1 failed", e);
}

// cancellation token is not used to cancel the task, no exception is thrown, pipeline finishes even though our condition has not been met
try
{
    await pipeline.ExecuteAsync<bool>(ct => new ValueTask<bool>(longTask.Execute()), cancellationToken);

    Console.WriteLine("Attempt 2: Task completed successfully");
}
catch (TimeoutRejectedException e)
{
    Console.WriteLine("Attempt 2 failed", e);
}

// cancellation token is not used to cancel the task, no exception is thrown, pipeline finishes even though our condition has not been met
try
{
    pipeline.Execute<bool>(ct => longTask.Execute(), cancellationToken);

    Console.WriteLine("Attempt 3: Task completed successfully");
}
catch (TimeoutRejectedException e)
{
    Console.WriteLine("Attempt 3 failed", e);
}
