using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using EventAggregator;
using EventAggregatorInterfaces;
using EventAggregatorSample.Data;
using EventAggregatorSample.Interfaces;
using EventAggregatorSample.ViewModels;
using EventAggregatorSample.Views;

namespace EventAggregatorSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ContainerBuilder();
            builder.RegisterType<MainWindow>();
            builder.RegisterType<SaveDataViewModel>().As<ISaveDataViewModel>();
            builder.RegisterType<WhatsUpViewModel>().As<IWhatsUpViewModel>();
            builder.RegisterType<MessageClient>().SingleInstance().PropertiesAutowired().As<IMessageClient>();
            builder.RegisterType<MessageDataAccess>().PropertiesAutowired().As<IMessageDataAccess>();
            builder.RegisterType<EventHub>().SingleInstance().As<IEventHub>();
            var container = builder.Build();

            this.MainWindow = container.Resolve<MainWindow>();
            this.MainWindow?.Show();
        }
    }
}
