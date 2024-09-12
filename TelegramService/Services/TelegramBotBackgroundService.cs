using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
            ReceiverOptions receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message } // ќбрабатывает только текстовые сообщени€
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _botClient.ReceiveAsync(
                        updateHandler: HandlerUpdateAsync,
                        pollingErrorHandler: HandlerPollingErrorAsync,
                        receiverOptions: receiverOptions,
                        cancellationToken: stoppingToken);
                }
                catch (Exception ex) { _logger.LogError(ex, "Error occurred while receiving updates."); }
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
            try
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "я не могу ответить на это сообщение:\n" + messageText,
                    replyToMessageId: message.MessageId,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Failed to send message to chat {chatId}"); }
        }

        internal Task HandlerPollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInformation(ErrorMessage);
            return Task.CompletedTask;
        }

        internal async Task SendMessageToAllUsersAsync(string message, CancellationToken stoppingToken)
        {
            foreach (var chatId in _usersChatId)
            {
                try
                {
                    await _botClient.SendTextMessageAsync(chatId, message, cancellationToken: stoppingToken);
                }
                catch (Telegram.Bot.Exceptions.ApiRequestException ex) when (ex.ErrorCode == 403)
                {
                    // ѕользователь заблокировал бота или удалил чат
                    _logger.LogWarning($"Cannot send message to chat {chatId}. User has blocked the bot or deleted the chat.");
                }
                catch (Telegram.Bot.Exceptions.ApiRequestException ex) when (ex.ErrorCode == 400)
                {
                    // Ќеверный chatId или другие ошибки
                    _logger.LogWarning($"Failed to send message to chat {chatId}. Bad request: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send message to chat {chatId}");
                }
            }
        }
    }
}
