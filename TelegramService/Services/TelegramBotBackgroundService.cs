using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using TelegramService.Options;

namespace TelegramService.Services
{
    public class TelegramBotBackgroundService : BackgroundService
    {
        private readonly ILogger<TelegramBotBackgroundService> _logger;
        private readonly TelegramOptions _options;
        private TelegramBotClient _botClient;
        private readonly List<long> _usersChatId;

        public TelegramBotBackgroundService(
            ILogger<TelegramBotBackgroundService> logger,
            IOptionsMonitor<TelegramOptions> telegramOptions,
            List<long> usersChatId,
            EventAggregator eventAggregator)
        {
            _logger = logger;
            _options = telegramOptions.CurrentValue;
            _botClient = CreateBotClient();

            _usersChatId = usersChatId ?? new List<long>();

            eventAggregator.Subscribe(async (message, token) =>
            {
                await SendMessageToAllUsersAsync(message, token);
            });
        }

        private TelegramBotClient CreateBotClient()
        {
            return new TelegramBotClient(_options.Token);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = []
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                await _botClient.ReceiveAsync(
                    updateHandler: HandlerUpdateAsync,
                    pollingErrorHandler: HandlerPollingErrorAsync,
                    receiverOptions: receiverOptions,
                    cancellationToken: stoppingToken);
            }
        }

        internal async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            var sendMessageRequest = new SendMessageRequest(chatId, messageText);

            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Я не могу ответить на это сообщение:\n" + messageText,
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);

        }

        internal Task HandlerPollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }


        internal async Task SendMessageToAllUsersAsync(string message, CancellationToken stoppingToken)
        {
            foreach (var chatId in _usersChatId)
            {
                await _botClient.SendTextMessageAsync(chatId, message, cancellationToken: stoppingToken);
            }
        }
    }
}
