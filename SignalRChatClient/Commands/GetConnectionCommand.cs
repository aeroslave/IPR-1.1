﻿namespace SignalRChatClient.Commands
{
    using SignalRChatClient.Utilites;
    using SignalRChatClient.VMs;

    /// <summary>
    /// Команда получения соединения.
    /// </summary>
    public class GetConnectionCommand : TypedBaseCommand<MainWindowVM>
    {
        /// <inheritdoc />
        public override void Execute(MainWindowVM mainWindowVM)
        {
            ConnectionUtils.InitHubConnection(mainWindowVM);
        }
    }
}