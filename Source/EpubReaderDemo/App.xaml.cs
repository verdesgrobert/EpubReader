using EpubReaderDemo.ViewModels;
using EpubReaderDemo.WpfEnvironment;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EpubReaderDemo
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            TranslationViewModel startViewModel = new TranslationViewModel();
            IWindowContext startViewModelContext = WindowManager.Instance.CreateWindow(startViewModel);
            startViewModelContext.Closed += (sender, args) => Shutdown();
            startViewModelContext.Show();
        }
    }
}
