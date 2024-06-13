using System;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Collections.Generic;

namespace Homework9
{
    class Program
    {
        static readonly TelegramBotClient bot = new TelegramBotClient("1906948226:AAFS5zRKFPCqdfRS_32bSa4e66Uw0XogGbQ");
        static List<Message> ms = new List<Message>();

        static void Main()
        {
            bot.StartReceiving();

            bot.OnMessage += BotOnMessageReceived;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;

            Console.ReadLine();

            bot.StopReceiving();
        }

        private static async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            if (ms.Count == 0)
                return;

            if (e.CallbackQuery.Data == "Показать загруженные файлы")
            {
                foreach (var el in ms)
                {
                    if (el.Type == MessageType.Document) await bot.SendDocumentAsync(el.From.Id, el.Document.FileId);
                    if (el.Type == MessageType.Audio) await bot.SendAudioAsync(el.From.Id, el.Audio.FileId);
                    if (el.Type == MessageType.Photo) await bot.SendPhotoAsync(el.From.Id, el.Photo[0].FileId);
                }
            }
            else
            {
                foreach (var el in ms)
                {
                    if (el.Type == MessageType.Document) DownLoad(el.Document.FileId, el.Document.FileName);
                    if (el.Type == MessageType.Audio) DownLoad(el.Audio.FileId, el.Audio.FileName);
                    if (el.Type == MessageType.Photo) DownLoad(el.Photo[1].FileId, el.Photo[0].FileUniqueId);
                }
            }
        }

        private static async void DownLoad(string file_id, string path)
        {
            var file = await bot.GetFileAsync(file_id);

            FileStream fs = new FileStream(path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);

            fs.Close();
        }

        private static async void BotOnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Type == MessageType.Text)
            {
                switch (message.Text)
                {
                    case "/start":
                        string text = "Список команд:\n" +
                                      "/callback - вызов команд";
                        await bot.SendTextMessageAsync(message.From.Id, text);
                        break;
                    case "/callback":
                        var inline_keyboard = new InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Показать загруженные файлы"),
                                InlineKeyboardButton.WithCallbackData("Сохранить все файлы")
                            }
                        });
                        await bot.SendTextMessageAsync(message.From.Id, "Выберите пункт меню", replyMarkup: inline_keyboard);
                        break;
                }
            }
            else if (message.Type == MessageType.Document ||
                     message.Type == MessageType.Audio ||
                     message.Type == MessageType.Photo)
            {
                ms.Add(message);
            }
            else
            {
                return;
            }
        }
    }
}
