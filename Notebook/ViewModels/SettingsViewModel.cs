using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notebook.Extensions;

namespace Notebook.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel(IRegionManager regionManager)
        {
            MenuBars = new ObservableCollection<MenuBar>();
            CreateMenuBar();
            this.regionManager = regionManager;
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
        }

        private void Navigate(MenuBar obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.NameSpace))
                return;

            regionManager.Regions[PrismManager.SettingsViewRegionName].RequestNavigate(obj.NameSpace);
        }

        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }

        private ObservableCollection<MenuBar>? menuBars;
        private readonly IRegionManager regionManager;
        private IRegionNavigationJournal journal;

        public ObservableCollection<MenuBar>? MenuBars
        {
            get { return menuBars; }
            set { menuBars = value; RaisePropertyChanged(); }
        }

        void CreateMenuBar()
        {
            MenuBars.Add(new MenuBar() { Icon = "Palette", Title = "个性化", NameSpace = "SkinView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "系统设置", NameSpace = "" });
            MenuBars.Add(new MenuBar() { Icon = "Information", Title = "关于更多", NameSpace = "AboutView" });
        }
    }
}
