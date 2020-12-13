﻿namespace SignalRChatClient.Utilites
{
    using Ninject;

    using SignalRChatClient.Interfaces;
    using SignalRChatClient.Services;

    public static class NinjectKernel
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        static NinjectKernel()
        {
            Instance = new StandardKernel();
            Bind();
        }

        /// <summary>
        /// Экземпляр ядра.
        /// </summary>
        public static StandardKernel Instance { get; }

        /// <summary>
        /// Создать привязки.
        /// </summary>
        private static void Bind()
        {
            Instance.Bind<IPersonService>().To<PersonService>().InSingletonScope();
        }
    }
}