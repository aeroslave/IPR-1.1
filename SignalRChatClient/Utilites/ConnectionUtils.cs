﻿namespace SignalRChatClient.Utilites
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;

    using Microsoft.AspNetCore.SignalR.Client;

    using Newtonsoft.Json;

    using SignalRChatClient.Models;

    /// <summary>
    /// Инструмент для работы с соединением.
    /// </summary>
    public static class ConnectionUtils
    {
        /// <summary>
        /// Получить адрес подключения.
        /// </summary>
        /// <returns>Адрес подключения.</returns>
        public static AddressConfig GetAddressConnection()
        {
            var path = Environment.CurrentDirectory;
            var configTextFromFile = File.ReadAllText(path + "\\address_config.json");

            return JsonConvert.DeserializeObject<AddressConfig>(configTextFromFile);
        }

        /// <summary>
        /// Инициализация соединения с хабом.
        /// </summary>
        public static void InitHubConnection(MainWindowVM mainWindowVM)
        {
            var address = GetAddressConnection();

            //TODO надо изменить, назначать HubConnection не здесь.
            mainWindowVM.HubConnection = new HubConnectionBuilder().WithUrl(address.Address + "/chatHub").Build();

            RegisterHandler(mainWindowVM);

            mainWindowVM.HubConnection.Closed += error =>
            {
                Application.Current.Dispatcher?.Invoke(() =>
                {
                    mainWindowVM.MessageList.Add("Соединение потеряно");
                    mainWindowVM.NeedGetConnection = true;
                    mainWindowVM.IsEnabled = false;
                });

                return Task.CompletedTask;
            };

            Task.Run(async () => await TryGetConnectionAsync(mainWindowVM));
        }

        /// <summary>
        /// Зарегистрировать обработчики методов хаба.
        /// </summary>
        /// <param name="mainWindowVM">Вью-модель главного окна.</param>
        private static void RegisterHandler(MainWindowVM mainWindowVM)
        {
            mainWindowVM.HubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Application.Current.Dispatcher?.Invoke(() =>
                {
                    var newMessage = $"{user} отправил сообщение: {message}";
                    mainWindowVM.MessageList.Add(newMessage);
                });
            });

            mainWindowVM.HubConnection.On<string, bool>("UpdateUsers", (user, isActive) =>
            {
                Application.Current.Dispatcher?.Invoke(() =>
                {
                    if (isActive)
                        mainWindowVM.ActiveUsers.Add(user);
                    else
                        mainWindowVM.ActiveUsers.Remove(user);
                });
            });
        }

        /// <summary>
        /// Попытаться соединиться с хабом.
        /// </summary>
        /// <param name="mainWindowVM">Вью-модель главного окна.</param>
        private static async Task TryGetConnectionAsync(MainWindowVM mainWindowVM)
        {
            try
            {
                Application.Current.Dispatcher?.Invoke(() => mainWindowVM.NeedGetConnection = false);

                await mainWindowVM.HubConnection.StartAsync();
                Application.Current.Dispatcher?.Invoke(() =>
                {
                    mainWindowVM.IsEnabled = true;
                    mainWindowVM.NeedGetConnection = false;
                    mainWindowVM.MessageList.Add("Соединение установлено");
                });
            }
            catch (Exception exception)
            {
                Application.Current.Dispatcher?.Invoke(() =>
                {
                    mainWindowVM.MessageList.Add($"Не удалось соединиться с сервером: {exception.Message}");
                    mainWindowVM.NeedGetConnection = true;
                });
                await mainWindowVM.HubConnection.DisposeAsync();
            }
        }
    }
}