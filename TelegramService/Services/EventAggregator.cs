namespace TelegramService.Services
{
    public class EventAggregator
    {
        public event Func<string, CancellationToken, Task> OnMessageReceived = delegate { return Task.CompletedTask; };

        public async Task PublishMessage(string message, CancellationToken token)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (OnMessageReceived != null)
                {
                    var invocationList = OnMessageReceived.GetInvocationList();
                    var tasks = invocationList.Select(d => ((Func<string, CancellationToken, Task>)d)(message, token));
                    await Task.WhenAll(tasks);
                }
            }
        }

        public void Subscribe(Func<string, CancellationToken, Task> action)
        {
            OnMessageReceived += action;
        }

        public void Unsubscribe(Func<string, CancellationToken, Task> action)
        {
            OnMessageReceived -= action;
        }
    }
}