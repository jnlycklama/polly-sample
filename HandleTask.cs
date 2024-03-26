namespace PollySample
{
    internal class HandleTask
    {
        private readonly TimeSpan _duration;

        public HandleTask(TimeSpan duration)
        {
            _duration = duration;
        }

        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(_duration, cancellationToken);
            return true;
        }

        public bool Execute()
        {
            Task.Delay(_duration).Wait();
            return true;
        }
    }
}
