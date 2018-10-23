using Catel.ApiCop;
using Catel.ApiCop.Listeners;
using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Catel.Services;
using Pierniczek.Services;
using Pierniczek.Services.Interfaces;
using Pierniczek.ViewModels;
using Pierniczek.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Pierniczek
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            LogManager.AddDebugListener();
#endif

            Log.Info("Starting application");

            // Want to improve performance? Uncomment the lines below. Note though that this will disable
            // some features. 
            //
            // For more information, see http://docs.catelproject.com/vnext/faq/performance-considerations/

            // Log.Info("Improving performance");
            // Catel.Windows.Controls.UserControl.DefaultCreateWarningAndErrorValidatorForViewModelValue = false;
            // Catel.Windows.Controls.UserControl.DefaultSkipSearchingForInfoBarMessageControlValue = true;

            // TODO: Register custom types in the ServiceLocator
            //Log.Info("Registering custom types");
            var serviceLocator = ServiceLocator.Default;
            //serviceLocator.RegisterType<IRuleService, RuleService>();
            serviceLocator.RegisterType<IFileService, FileService>();
            serviceLocator.RegisterType<IClassService, ClassService>();
            serviceLocator.RegisterType<IScaleService, ScaleService>();


            // To auto-forward styles, check out Orchestra (see https://github.com/wildgums/orchestra)
            // StyleHelper.CreateStyleForwardersForDefaultStyles();

            var uiVisualizerService = serviceLocator.ResolveType<IUIVisualizerService>();
            uiVisualizerService.Register<MainWindowViewModel, MainWindow>();
            uiVisualizerService.Register<OpenFileWindowViewModel, OpenFileWindow>();
            uiVisualizerService.Register<NewColumnDataWindowViewModel, NewColumnDataWindow>();
            uiVisualizerService.Register<SelectColumnDataWindowViewModel, SelectColumnDataWindow>();
            uiVisualizerService.Register<NewRangeDataWindowViewModel, NewRangeDataWindow>();
            uiVisualizerService.Register<SetRangesDataWindowViewModel, SetRangesDataWindow>();
            //uiVisualizerService.Register<SourceConfigurationWindowViewModel, SourceConfigurationWindow>();
            //uiVisualizerService.Register<MessageDetailsViewModel, MessageDetailsView>();

            //uiVisualizerService.Register<RuleWindowViewModel, RuleWindow>();


            var viewModelLocator = serviceLocator.ResolveType<IViewModelLocator>();
            //viewModelLocator.Register<RuleWindow, RuleWindowViewModel>();

            Log.Info("Calling base.OnStartup");
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Get advisory report in console
            ApiCopManager.AddListener(new ConsoleApiCopListener());
            ApiCopManager.WriteResults();

            base.OnExit(e);
        }
    }
}
