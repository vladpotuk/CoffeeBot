using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CoffeeBot
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("7250458837:AAGRg-ZQaOOeq6czZTqWqP3QetLcAlSsW3Y");

        static async Task Main(string[] args)
        {
            var me = await Bot.GetMeAsync();
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

            var cts = new System.Threading.CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new Telegram.Bot.Extensions.Polling.ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            Bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            Console.WriteLine($"Start listening for @{me.Username}");
            await Task.Delay(-1, cancellationToken); // Use Task.Delay to keep the application running asynchronously until cancelled
            cts.Cancel();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Text)
                {
                    await BotOnMessageReceived(botClient, update.Message);
                }
                else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
                {
                    await BotOnCallbackQueryReceived(botClient, update.CallbackQuery);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling update: {ex.Message}");
            }
        }

        private static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, System.Threading.CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text?.ToLower() == "/start")
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Ласкаво просимо до Кавового Бота! Оберіть свою улюблену каву:",
                    replyMarkup: GetCoffeeOptions()
                );
            }
            else if (message.Text?.ToLower() == "/help")
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Я можу допомогти тобі обрати каву. Використай /coffee, щоб обрати каву."
                );
            }
            else if (message.Text?.ToLower() == "/coffee")
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Ось декілька варіантів кави, яку ти можеш обрати:",
                    replyMarkup: GetCoffeeOptions()
                );
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Вибач, я не розумію цю команду. Спробуй /help для отримання допомоги."
                );
            }
        }

        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            switch (callbackQuery.Data)
            {
                case "Цукор":
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Скільки цукру бажаєте додати?"
                    );
                    break;
                case "Без цукру":
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Додано до замовлення: Без цукру. Чи бажаєте додати сіроп?"
                    );
                    break;
                case "Додати сіроп":
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Оберіть тип сиропу:",
                        replyMarkup: GetSyrupOptions()
                    );
                    break;
                case "Ні, дякую":
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Дякую за ваше замовлення!"
                    );
                    break;
                // Handle syrup options callback here if needed
                default:
                    // Handle coffee selection
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: $"Ви обрали: {callbackQuery.Data}"
                    );
                    break;
            }
        }

        private static InlineKeyboardMarkup GetCoffeeOptions()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Еспресо", "Еспресо"),
                    InlineKeyboardButton.WithCallbackData("Капучіно", "Капучіно")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Лате", "Лате"),
                    InlineKeyboardButton.WithCallbackData("Американо", "Американо")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Мокко", "Мокко"),
                    InlineKeyboardButton.WithCallbackData("Рістретто", "Рістретто")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Флет Вайт", "Флет Вайт"),
                    InlineKeyboardButton.WithCallbackData("Макіато", "Макіато")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Цукор", "Цукор"),
                    InlineKeyboardButton.WithCallbackData("Без цукру", "Без цукру")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Додати сіроп", "Додати сіроп"),
                    InlineKeyboardButton.WithCallbackData("Ні, дякую", "Ні, дякую")
                }
            });
        }

        private static InlineKeyboardMarkup GetSyrupOptions()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Ванільний", "Ванільний"),
                    InlineKeyboardButton.WithCallbackData("Карамельний", "Карамельний")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Шоколадний", "Шоколадний"),
                    InlineKeyboardButton.WithCallbackData("Фруктовий", "Фруктовий")
                }
                // Add more syrup options as needed
            });
        }
    }
}
