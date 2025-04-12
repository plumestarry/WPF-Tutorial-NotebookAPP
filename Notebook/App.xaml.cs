using System.Configuration;
using System.Data;
using System.Windows;
using Notebook.Common;
using Notebook.Service;
using Notebook.ViewModels;
using Notebook.ViewModels.Dialogs;
using Notebook.Views;
using Notebook.Views.Dialogs;

namespace Notebook;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainView>();
    }

    public static void LoginOut(IContainerProvider containerProvider)
    {
        Current.MainWindow.Hide();

        var dialog = containerProvider.Resolve<IDialogService>();

        dialog.ShowDialog("LoginView", callback =>
        {
            if (callback.Result != ButtonResult.OK)
            {
                Application.Current.Shutdown();
                //Environment.Exit(0);
                return;
            }

            Current.MainWindow.Show();
        });
    }

    protected override void OnInitialized()
    {
        var dialog = Container.Resolve<IDialogService>();

        dialog.ShowDialog("LoginView", callback =>
        {
            if (callback.Result != ButtonResult.OK)
            {
                Application.Current.Shutdown();
                //Environment.Exit(0);
                return;
            }

            var service = App.Current.MainWindow.DataContext as IConfigureService;
            if (service != null)
                service.Configure();
            base.OnInitialized();
        });
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {

        containerRegistry.GetContainer().Register<HttpRestClient>(made:Parameters.Of.Type<string>(serviceKey: "webUrl"));
        containerRegistry.GetContainer().RegisterInstance(@"http://localhost:5140/", serviceKey: "webUrl");

        containerRegistry.Register<ILoginService, LoginService>();
        containerRegistry.Register<INotebookService, NotebookService>();
        containerRegistry.Register<IMemoService, MemoService>();
        containerRegistry.Register<IDialogHostService, DialogHostService>();

        containerRegistry.RegisterDialog<LoginView, LoginViewModel>();

        containerRegistry.RegisterForNavigation<AddNotebookView, AddNotebookViewModel>();
        containerRegistry.RegisterForNavigation<AddMemoView, AddMemoViewModel>();
        containerRegistry.RegisterForNavigation<MsgView, MsgViewModel>();

        containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();
        containerRegistry.RegisterForNavigation<MemoView, MemoViewModel>();
        containerRegistry.RegisterForNavigation<NotebookView, NotebookViewModel>();
        containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
        containerRegistry.RegisterForNavigation<SkinView, SkinViewModel>();
        containerRegistry.RegisterForNavigation<AboutView>();
    }
}

