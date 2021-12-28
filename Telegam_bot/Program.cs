using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Countries;

var botClient = new TelegramBotClient("2133841858:AAEkskJ2uCrfQ1GL4NIYfHduGjcRMlweMec");
var me = await botClient.GetMeAsync();
Random rng = new Random();
List<ValueTuple<long, Country>> countryList = new List<ValueTuple<long, Country>>();
List<ValueTuple<long, string>> langList = new List<ValueTuple<long, string>>();

using var cts = new CancellationTokenSource();

ReplyKeyboardMarkup? GetMenuButtonsRU = new(new[]
{
    new KeyboardButton[] { "🇷🇺", "🇬🇧" },
    new KeyboardButton[] { "Начать тест" },
})
{
    ResizeKeyboard = true
};

ReplyKeyboardMarkup? GetMenuButtonsEN = new(new[]
{
    new KeyboardButton[] { "🇷🇺", "🇬🇧" },
    new KeyboardButton[] { "Start test" },
})
{
    ResizeKeyboard = true
};

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }
};
botClient.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);



Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();


cts.Cancel();


async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Type != UpdateType.Message)
        return;

    if (update.Message!.Type != MessageType.Text)
        return;

    var chatId = update.Message.Chat.Id;
    var messageText = update.Message.Text;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId} at {DateTime.Now}.");
    if (countryList.Count != 0)
    {
        IEnumerable<ValueTuple<long, Country>> evens = countryList.Where(item => item.Item1 == chatId);
        if (evens.Count() == 0)
        {
            countryList.Add(new ValueTuple<long, Country>(chatId, new Country(countriesList.countries[rng.Next(0, countriesList.countries.Length)])));
        }
    }
    else
    {

        countryList.Add(new ValueTuple<long, Country>(chatId, new Country(countriesList.countries[rng.Next(0, countriesList.countries.Length)])));
    }

    if (langList.Count != 0)
    {
        IEnumerable<ValueTuple<long, string>> evens = langList.Where(item => item.Item1 == chatId);
        if (evens.Count() == 0)
        {
            langList.Add(new ValueTuple<long, string>(chatId, "ru"));
        }
    }
    else
    {

        langList.Add(new ValueTuple<long, string>(chatId, "ru"));
    }

    Country? selectedCountry = countryList.Where(item => item.Item1 == chatId).ElementAt(0).Item2;

    switch (messageText)
    {
        case "🇷🇺":
            langList[langList.FindIndex(item => item.Item1 == chatId)] = (chatId, "ru");
            break;
        case "🇬🇧":
            langList[langList.FindIndex(item => item.Item1 == chatId)] = (chatId, "en");
            break;
        default:
            break;
    }
    switch (langList[langList.FindIndex(item => item.Item1 == chatId)].Item2)
    {
        case "ru":
            switch (messageText)
            {
                case "❌Закончить тест❌":
                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Пока! Надеюсь поиграем еще!",
                        replyMarkup: GetMenuButtonsRU,
                        cancellationToken: cancellationToken);
                    break;
                case "Начать тест":
                    countryList[countryList.FindIndex(x => x.Item1 == chatId)] = (chatId, countriesList.countries[rng.Next(0, countriesList.countries.Length)]);
                    selectedCountry = countryList.Where(item => item.Item1 == chatId).ElementAt(0).Item2;
                    Message startTestingMessage = await botClient.SendTextMessageAsync(
               chatId: chatId,
               text: "Все очень просто.\nЯ буду присылать тебе флаг страны, а ты должен сказать, что это за страна.",
               cancellationToken: cancellationToken);
                    GameRU(chatId, cts.Token);
                    break;
                default:
                    var game = false;
                    for (int i = 0; i < Enum.GetNames(typeof(CountriesVariantListRU)).Length; i++)
                    {
                        if (messageText == ((CountriesVariantListRU)i).ToString())
                        {
                            if (GameAnswerRU(chatId, messageText, cts.Token).Result)
                            {
                                GameRU(chatId, cts.Token);
                                game = true;
                                break;
                            }
                            game = true;
                        }
                    }
                    if (game)
                    {
                        break;
                    }
                    Message helloMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Давай поиграем!",
                        replyMarkup: GetMenuButtonsRU,
                        cancellationToken: cancellationToken);
                    countryList[countryList.FindIndex(x=>x.Item1==chatId)] = (chatId, countriesList.countries[rng.Next(0, countriesList.countries.Length)]);
                    selectedCountry = countryList.Where(item => item.Item1 == chatId).ElementAt(0).Item2;
                    break;
            }
            break;
        case "en":
            switch (messageText)
            {
                case "❌End test❌":
                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Bye! Hopefully we'll play some more!",
                        replyMarkup: GetMenuButtonsEN,
                        cancellationToken: cancellationToken);
                    break;
                case "Start test":
                    countryList[countryList.FindIndex(x => x.Item1 == chatId)] = (chatId, countriesList.countries[rng.Next(0, countriesList.countries.Length)]);
                    selectedCountry = countryList.Where(item => item.Item1 == chatId).ElementAt(0).Item2;
                    Message startTestingMessage = await botClient.SendTextMessageAsync(
               chatId: chatId,
               text: "It's very simple.\nI will send you the flag of the country, and you have to say what kind of country it is.",
               cancellationToken: cancellationToken);
                    GameEN(chatId, cts.Token);
                    break;
                default:
                    var game = false;
                    for (int i = 0; i < Enum.GetNames(typeof(CountriesVariantListEN)).Length; i++)
                    {
                        if (messageText == ((CountriesVariantListEN)i).ToString())
                        {
                            if (GameAnswerEN(chatId, messageText, cts.Token).Result)
                            {
                                GameEN(chatId, cts.Token);
                                game = true;
                                break;
                            }
                            game = true;
                        }
                    }
                    if (game)
                    {
                        break;
                    }
                    Message helloMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Let's play!",
                        replyMarkup: GetMenuButtonsEN,
                        cancellationToken: cancellationToken);
                    countryList[countryList.FindIndex(x => x.Item1 == chatId)] = (chatId, countriesList.countries[rng.Next(0, countriesList.countries.Length)]);
                    selectedCountry = countryList.Where(item => item.Item1 == chatId).ElementAt(0).Item2;
                    break;
            }
            break;
        default:
            break;
    }
    async Task<bool> GameAnswerRU(long chatId, string? messageText, CancellationToken cancellationToken)
    {
        if (messageText == selectedCountry.correctAnswer.Item1)
        {
            Message gameMessage = await botClient.SendPhotoAsync(
            chatId: chatId,
            photo: selectedCountry.url,
            caption: $"✅<b>Молодец! Это {selectedCountry.correctAnswer}</b>.\n<i>Можешь почитать больше об этой стране тут</i>: <a href=\"{selectedCountry.description}\">👉Тык</a>",
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

            Message startTestingMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Давай продолжим!",
            cancellationToken: cancellationToken);
            return true;
        }
        else
        {
            Message startTestingMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Это неправильный ответ 😢, попробуй еще раз",
            replyMarkup: GetChooseMenuRU(selectedCountry),
            cancellationToken: cancellationToken);
            return false;
        }
    }

    async Task<bool> GameAnswerEN(long chatId, string? messageText, CancellationToken cancellationToken)
    {
        if (messageText == selectedCountry.correctAnswer.Item2)
        {
            Message gameMessage = await botClient.SendPhotoAsync(
            chatId: chatId,
            photo: selectedCountry.url,
            caption: $"✅<b>Well done! This is the {selectedCountry.correctAnswer}</b>.\n<i>You can read more about this country here</i>: <a href=\"{selectedCountry.description}\">👉push</a>",
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

            Message startTestingMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Let's continue!",
            cancellationToken: cancellationToken);
            return true;
        }
        else
        {
            Message startTestingMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "This is the wrong answer 😢, please try again",
            replyMarkup: GetChooseMenuEN(selectedCountry),
            cancellationToken: cancellationToken);
            return false;
        }
    }

    async void GameRU(long chatId, CancellationToken cancellationToken)
    {
        Task.Delay(100);
        countryList[countryList.FindIndex(x => x.Item1 == chatId)] = (chatId, countriesList.countries[rng.Next(0, countriesList.countries.Length)]);
        selectedCountry = countryList.Where(item => item.Item1 == chatId).ElementAt(0).Item2;
        Message gameMessage = await botClient.SendPhotoAsync(
    chatId: chatId,
    photo: selectedCountry.url,
    caption: "Что это за страна?",
    replyMarkup: GetChooseMenuRU(selectedCountry),
    cancellationToken: cancellationToken);
    }

    async void GameEN(long chatId, CancellationToken cancellationToken)
    {
        Task.Delay(100);
        countryList[countryList.FindIndex(x => x.Item1 == chatId)] = (chatId, countriesList.countries[rng.Next(0, countriesList.countries.Length)]);
        selectedCountry = countryList.Where(item => item.Item1 == chatId).ElementAt(0).Item2;
        Message gameMessage = await botClient.SendPhotoAsync(
        chatId: chatId,
        photo: selectedCountry.url,
        caption: "What country is it?",
        replyMarkup: GetChooseMenuEN(selectedCountry),
        cancellationToken: cancellationToken);
    }
}


Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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


ReplyKeyboardMarkup? GetChooseMenuRU(Country country)
{
    Random rng = new Random();
    int random = rng.Next(1, 5);
    string choose1 = "error";
    string choose2 = "error";
    string choose3 = "error";
    string choose4 = "error";
    switch (random)
    {
        case 1:
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            choose1 = country.correctAnswer.Item1;
            do
            {
                if (country.correctAnswer.Item1 != ((CountriesVariantListRU)random).ToString())
                {
                    choose2 = ((CountriesVariantListRU)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
                }
            }
            while (country.correctAnswer.Item1 == ((CountriesVariantListRU)random).ToString() || choose2 != ((CountriesVariantListRU)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            do
            {
                if (country.correctAnswer.Item1 != ((CountriesVariantListRU)random).ToString())
                {
                    choose3 = ((CountriesVariantListRU)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
                }
            }
            while (country.correctAnswer.Item1 == ((CountriesVariantListRU)random).ToString() || choose3 != ((CountriesVariantListRU)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            do
            {
                if (country.correctAnswer.Item1 != ((CountriesVariantListRU)random).ToString())
                {
                    choose4 = ((CountriesVariantListRU)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
                }
            }
            while (country.correctAnswer.Item1 == ((CountriesVariantListRU)random).ToString() || choose4 != ((CountriesVariantListRU)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            ReplyKeyboardMarkup? GetChooseMenu1 = new(new[]
            {
                new KeyboardButton[] { choose1, choose2 },
                new KeyboardButton[] { choose3, choose4 },
                new KeyboardButton[] { "❌Закончить тест❌" },
            })
            {
                ResizeKeyboard = true
            };
            return GetChooseMenu1;
        case 2:
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            choose2 = country.correctAnswer.Item1;
            do
            {
                if (country.correctAnswer.Item1 != ((CountriesVariantListRU)random).ToString())
                {
                    choose1 = ((CountriesVariantListRU)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
                }
            }
            while (country.correctAnswer.Item1 == ((CountriesVariantListRU)random).ToString() || choose1 != ((CountriesVariantListRU)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            do
            {
                if (country.correctAnswer.Item1 != ((CountriesVariantListRU)random).ToString())
                {
                    choose3 = ((CountriesVariantListRU)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
                }
            }
            while (country.correctAnswer.Item1 == ((CountriesVariantListRU)random).ToString() || choose3 != ((CountriesVariantListRU)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            do
            {
                if (country.correctAnswer.Item1 != ((CountriesVariantListRU)random).ToString())
                {
                    choose4 = ((CountriesVariantListRU)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
                }
            }
            while (country.correctAnswer.Item1 == ((CountriesVariantListRU)random).ToString() || choose4 != ((CountriesVariantListRU)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            ReplyKeyboardMarkup? GetChooseMenu2 = new(new[]
            {
                new KeyboardButton[] { choose1, choose2 },
                new KeyboardButton[] { choose3, choose4 },
                new KeyboardButton[] { "❌Закончить тест❌" },
            })
            {
                ResizeKeyboard = true
            };
            return GetChooseMenu2;
        case 3:
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            choose3 = country.correctAnswer.Item1;
            do
            {
                if (country.correctAnswer.Item1 != ((CountriesVariantListRU)random).ToString())
                {
                    choose1 = ((CountriesVariantListRU)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
                }
            }
            while (country.correctAnswer.Item1 == ((CountriesVariantListRU)random).ToString() || choose1 != ((CountriesVariantListRU)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            do
            {
                if (country.correctAnswer.Item1 != ((CountriesVariantListRU)random).ToString())
                {
                    choose2 = ((CountriesVariantListRU)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
                }
            }
            while (country.correctAnswer.Item1 == ((CountriesVariantListRU)random).ToString() || choose2 != ((CountriesVariantListRU)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            do
            {
                if (country.correctAnswer.Item1 != ((CountriesVariantListRU)random).ToString())
                {
                    choose4 = ((CountriesVariantListRU)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
                }
            }
            while (country.correctAnswer.Item1 == ((CountriesVariantListRU)random).ToString() || choose4 != ((CountriesVariantListRU)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            ReplyKeyboardMarkup? GetChooseMenu3 = new(new[]
            {
                new KeyboardButton[] { choose1, choose2 },
                new KeyboardButton[] { choose3, choose4 },
                new KeyboardButton[] { "❌Закончить тест❌" },
            })
            {
                ResizeKeyboard = true
            };
            return GetChooseMenu3;
        case 4:
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            choose4 = country.correctAnswer.Item1;
            do
            {
                if (country.correctAnswer.Item1 != ((CountriesVariantListRU)random).ToString())
                {
                    choose1 = ((CountriesVariantListRU)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
                }
            }
            while (country.correctAnswer.Item1 == ((CountriesVariantListRU)random).ToString() || choose1 != ((CountriesVariantListRU)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            do
            {
                if (country.correctAnswer.Item1 != ((CountriesVariantListRU)random).ToString())
                {
                    choose2 = ((CountriesVariantListRU)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
                }
            }
            while (country.correctAnswer.Item1 == ((CountriesVariantListRU)random).ToString() || choose2 != ((CountriesVariantListRU)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            do
            {
                if (country.correctAnswer.Item1 != ((CountriesVariantListRU)random).ToString())
                {
                    choose3 = ((CountriesVariantListRU)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
                }
            }
            while (country.correctAnswer.Item1 == ((CountriesVariantListRU)random).ToString() || choose3 != ((CountriesVariantListRU)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListRU)).Length);
            ReplyKeyboardMarkup? GetChooseMenu4 = new(new[]
            {
                new KeyboardButton[] { choose1, choose2 },
                new KeyboardButton[] { choose3, choose4 },
                new KeyboardButton[] { "❌Закончить тест❌" },
            })
            {
                ResizeKeyboard = true
            };
            return GetChooseMenu4;
        default:
            ReplyKeyboardMarkup? GetChooseMenu = new(new[]
            {
                new KeyboardButton[] { choose1, choose2 },
                new KeyboardButton[] { choose3, choose4 },
                new KeyboardButton[] { "❌Закончить тест❌" },
            })
            {
                ResizeKeyboard = true
            };
            return GetChooseMenu;
    }
}

ReplyKeyboardMarkup? GetChooseMenuEN(Country country)
{
    Random rng = new Random();
    int random = rng.Next(1, 5);
    string choose1 = "error";
    string choose2 = "error";
    string choose3 = "error";
    string choose4 = "error";
    switch (random)
    {
        case 1:
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            choose1 = country.correctAnswer.Item2;
            do
            {
                if (country.correctAnswer.Item2 != ((CountriesVariantListEN)random).ToString())
                {
                    choose2 = ((CountriesVariantListEN)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
                }
            }
            while (country.correctAnswer.Item2 == ((CountriesVariantListEN)random).ToString() || choose2 != ((CountriesVariantListEN)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            do
            {
                if (country.correctAnswer.Item2 != ((CountriesVariantListEN)random).ToString())
                {
                    choose3 = ((CountriesVariantListEN)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
                }
            }
            while (country.correctAnswer.Item2 == ((CountriesVariantListEN)random).ToString() || choose3 != ((CountriesVariantListEN)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            do
            {
                if (country.correctAnswer.Item2 != ((CountriesVariantListEN)random).ToString())
                {
                    choose4 = ((CountriesVariantListEN)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
                }
            }
            while (country.correctAnswer.Item2 == ((CountriesVariantListEN)random).ToString() || choose4 != ((CountriesVariantListEN)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            ReplyKeyboardMarkup? GetChooseMenu1 = new(new[]
            {
                new KeyboardButton[] { choose1, choose2 },
                new KeyboardButton[] { choose3, choose4 },
                new KeyboardButton[] { "❌End test❌" },
            })
            {
                ResizeKeyboard = true
            };
            return GetChooseMenu1;
        case 2:
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            choose2 = country.correctAnswer.Item2;
            do
            {
                if (country.correctAnswer.Item2 != ((CountriesVariantListEN)random).ToString())
                {
                    choose1 = ((CountriesVariantListEN)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
                }
            }
            while (country.correctAnswer.Item2 == ((CountriesVariantListEN)random).ToString() || choose1 != ((CountriesVariantListEN)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            do
            {
                if (country.correctAnswer.Item2 != ((CountriesVariantListEN)random).ToString())
                {
                    choose3 = ((CountriesVariantListEN)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
                }
            }
            while (country.correctAnswer.Item2 == ((CountriesVariantListEN)random).ToString() || choose3 != ((CountriesVariantListEN)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            do
            {
                if (country.correctAnswer.Item2 != ((CountriesVariantListEN)random).ToString())
                {
                    choose4 = ((CountriesVariantListEN)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
                }
            }
            while (country.correctAnswer.Item2 == ((CountriesVariantListEN)random).ToString() || choose4 != ((CountriesVariantListEN)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            ReplyKeyboardMarkup? GetChooseMenu2 = new(new[]
            {
                new KeyboardButton[] { choose1, choose2 },
                new KeyboardButton[] { choose3, choose4 },
                new KeyboardButton[] { "❌End test❌" },
            })
            {
                ResizeKeyboard = true
            };
            return GetChooseMenu2;
        case 3:
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            choose3 = country.correctAnswer.Item2;
            do
            {
                if (country.correctAnswer.Item2 != ((CountriesVariantListEN)random).ToString())
                {
                    choose1 = ((CountriesVariantListEN)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
                }
            }
            while (country.correctAnswer.Item2 == ((CountriesVariantListEN)random).ToString() || choose1 != ((CountriesVariantListEN)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            do
            {
                if (country.correctAnswer.Item2 != ((CountriesVariantListEN)random).ToString())
                {
                    choose2 = ((CountriesVariantListEN)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
                }
            }
            while (country.correctAnswer.Item2 == ((CountriesVariantListEN)random).ToString() || choose2 != ((CountriesVariantListEN)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            do
            {
                if (country.correctAnswer.Item2 != ((CountriesVariantListEN)random).ToString())
                {
                    choose4 = ((CountriesVariantListEN)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
                }
            }
            while (country.correctAnswer.Item2 == ((CountriesVariantListEN)random).ToString() || choose4 != ((CountriesVariantListEN)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            ReplyKeyboardMarkup? GetChooseMenu3 = new(new[]
            {
                new KeyboardButton[] { choose1, choose2 },
                new KeyboardButton[] { choose3, choose4 },
                new KeyboardButton[] { "❌End test❌" },
            })
            {
                ResizeKeyboard = true
            };
            return GetChooseMenu3;
        case 4:
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            choose4 = country.correctAnswer.Item2;
            do
            {
                if (country.correctAnswer.Item2 != ((CountriesVariantListEN)random).ToString())
                {
                    choose1 = ((CountriesVariantListEN)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
                }
            }
            while (country.correctAnswer.Item2 == ((CountriesVariantListEN)random).ToString() || choose1 != ((CountriesVariantListEN)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            do
            {
                if (country.correctAnswer.Item2 != ((CountriesVariantListEN)random).ToString())
                {
                    choose2 = ((CountriesVariantListEN)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
                }
            }
            while (country.correctAnswer.Item2 == ((CountriesVariantListEN)random).ToString() || choose2 != ((CountriesVariantListEN)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            do
            {
                if (country.correctAnswer.Item2 != ((CountriesVariantListEN)random).ToString())
                {
                    choose3 = ((CountriesVariantListEN)random).ToString();
                }
                else
                {
                    random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
                }
            }
            while (country.correctAnswer.Item2 == ((CountriesVariantListEN)random).ToString() || choose3 != ((CountriesVariantListEN)random).ToString());
            random = rng.Next(Enum.GetNames(typeof(CountriesVariantListEN)).Length);
            ReplyKeyboardMarkup? GetChooseMenu4 = new(new[]
            {
                new KeyboardButton[] { choose1, choose2 },
                new KeyboardButton[] { choose3, choose4 },
                new KeyboardButton[] { "❌End test❌" },
            })
            {
                ResizeKeyboard = true
            };
            return GetChooseMenu4;
        default:
            ReplyKeyboardMarkup? GetChooseMenu = new(new[]
            {
                new KeyboardButton[] { choose1, choose2 },
                new KeyboardButton[] { choose3, choose4 },
                new KeyboardButton[] { "End test" },
            })
            {
                ResizeKeyboard = true
            };
            return GetChooseMenu;
    }
}

enum CountriesVariantListRU
{
    Австралия,
    Австрия,
    Азербайджан,
    Албания,
    Алжир,
    Ангола,
    Андорра,
    Аргентина,
    Армения,
    Афганистан,
    Багамы,
    Бангладеш,
    Барбадос,
    Бахрейн,
    Белоруссия,
    Белиз,
    Бельгия,
    Бенин,
    Болгария,
    Боливия,
    Бразилия,
    Бруней,
    Бурунди,
    Бутан,
    Вануату,
    Великобритания,
    Венгрия,
    Венесуэла,
    Вьетнам,
    Габон,
    Гаити,
    Гайана,
    Гамбия,
    Гана,
    Гватемала,
    Гвинея,
    Германия,
    Гондурас,
    Гренада,
    Греция,
    Грузия,
    Дания,
    Джибути,
    Доминика,
    Египет,
    Замбия,
    Зимбабве,
    Израиль,
    Индия,
    Индонезия,
    Иордания,
    Ирак,
    Иран,
    Ирландия,
    Исландия,
    Испания,
    Италия,
    Йемен,
    Казахстан,
    Камбоджа,
    Камерун,
    Канада,
    Катар,
    Кения,
    Кипр,
    Киргизия,
    Кирибати,
    Китай,
    Колумбия,
    Коморы,
    Конго,
    Корея,
    Куба,
    Кувейт,
    Лаос,
    Латвия,
    Лесото,
    Либерия,
    Ливан,
    Ливия,
    Литва,
    Лихтенштейн,
    Люксембург,
    Маврикий,
    Мавритания,
    Мадагаскар,
    Малави,
    Малайзия,
    Мали,
    Мальдивы,
    Мальта,
    Марокко,
    Мексика,
    Микронезия,
    Мозамбик,
    Молдавия,
    Монако,
    Монголия,
    Мьянма,
    Намибия,
    Науру,
    Непал,
    Нигер,
    Нигерия,
    Нидерланды,
    Никарагуа,
    Норвегия,
    ОАЭ,
    Оман,
    Пакистан,
    Палау,
    Панама,
    Парагвай,
    Перу,
    Польша,
    Португалия,
    Россия,
    Руанда,
    Румыния,
    Сальвадор,
    Самоа,
    Сейшелы,
    Сенегал,
    Сербия,
    Сингапур,
    Сирия,
    Словакия,
    Словения,
    США,
    Сомали,
    Судан,
    Суринам,
    Таджикистан,
    Таиланд,
    Танзания,
    Того,
    Тонга,
    Тувалу,
    Тунис,
    Туркмения,
    Турция,
    Уганда,
    Узбекистан,
    Украина,
    Уругвай,
    Фиджи,
    Филиппины,
    Финляндия,
    Франция,
    Хорватия,
    ЦАР,
    Чад,
    Черногория,
    Чехия,
    Чили,
    Швейцария,
    Швеция,
    Эквадор,
    Эритрея,
    Эсватини,
    Эстония,
    Эфиопия,
    ЮАР,
    Ямайка,
    Япония
}
enum CountriesVariantListEN
{
    Australia,
    Austria,
    Azerbaijan,
    Albania,
    Algeria,
    Angola,
    Andorra,
    Argentina,
    Armenia,
    Afghanistan,
    Bahamas,
    Bangladesh,
    Barbados,
    Bahrain,
    Belarus,
    Belize,
    Belgium,
    Benin,
    Bulgaria,
    Bolivia,
    Brazil,
    Brunei,
    Burundi,
    Bhutan,
    Vanuatu,
    UK,
    Hungary,
    Venezuela,
    Vietnam,
    Gabon,
    Haiti,
    Guyana,
    Gambia,
    Ghana,
    Guatemala,
    Guinea,
    Germany,
    Honduras,
    Grenada,
    Greece,
    Georgia,
    Denmark,
    Djibouti,
    Dominica,
    Egypt,
    Zambia,
    Zimbabwe,
    Israel,
    India,
    Indonesia,
    Jordan,
    Iraq,
    Iran,
    Ireland,
    Iceland,
    Spain,
    Italy,
    Yemen,
    Kazakhstan,
    Cambodia,
    Cameroon,
    Canada,
    Qatar,
    Kenya,
    Cyprus,
    Kyrgyzstan,
    Kiribati,
    China,
    Colombia,
    Komors,
    Congo,
    Korea,
    Cuba,
    Kuwait,
    Laos,
    Latvian,
    Lesotho,
    Liberia,
    Lebanon,
    Libya,
    Lithuania,
    Liechtenstein,
    Luxembourg,
    Mauritius,
    Mauritania,
    Madagascar,
    Malawi,
    Malaysia,
    Mali,
    Maldives,
    Malta,
    Morocco,
    Mexico,
    Micronesia,
    Mozambique,
    Moldova,
    Monaco,
    Mongolia,
    Myanmar,
    Namibia,
    Nauru,
    Nepal,
    Niger,
    Nigeria,
    Netherlands,
    Nicaragua,
    Norway,
    UAE,
    Oman,
    Pakistan,
    Palau,
    Panama,
    Paraguay,
    Peru,
    Poland,
    Portugal,
    Russia,
    Rwanda,
    Romania,
    Salvador,
    Samoa,
    Seychelles,
    Senegal,
    Serbia,
    Singapore,
    Syria,
    Slovakia,
    Slovenia,
    USA,
    Somalia,
    Sudan,
    Suriname,
    Tajikistan,
    Thailand,
    Tanzania,
    Togo,
    Tonga,
    Tuvalu,
    Tunisia,
    Turkmenistan,
    Turkey,
    Uganda,
    Uzbekistan,
    Ukraine,
    Uruguay,
    Fiji,
    Philippines,
    Finland,
    France,
    Croatia,
    CAR,
    Chad,
    Montenegro,
    Czech,
    Chile,
    Switzerland,
    Sweden,
    Ecuador,
    Eritrea,
    Eswatini,
    Estonia,
    Ethiopia,
    Africa,
    Jamaica,
    Japan
}
