using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Themes;
using System.Windows.Media;
using MaterialDesignColors.ColorManipulation;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;


namespace Notebook.ViewModels
{

    public class SkinViewModel : BindableBase
    {
        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    ModifyTheme(value ? BaseTheme.Dark : BaseTheme.Light);
                }
            }
        }

        public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;

        public DelegateCommand<object> ChangeHueCommand { get; private set; }

        private readonly PaletteHelper paletteHelper = new PaletteHelper();

        public SkinViewModel()
        {
            ChangeHueCommand = new DelegateCommand<object>(ChangeHue);
        }

        private void ModifyTheme(BaseTheme themeBase)
        {
            var palette = paletteHelper.GetTheme();
            palette.SetBaseTheme(themeBase);
            paletteHelper.SetTheme(palette);
        }

        private void ChangeHue(object obj)
        {
            if (obj is Color color)
            {
                var palette = paletteHelper.GetTheme();
                palette.PrimaryLight = new ColorPair(color.Lighten());
                palette.PrimaryMid = new ColorPair(color);
                palette.PrimaryDark = new ColorPair(color.Darken());
                paletteHelper.SetTheme(palette);
            }
        }
    }

}
