using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Movies.Themes.Model;
using Movies.Themes.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Movies.Themes.ViewModel
{
    public class ThemeLoaderViewModel : NotificationObject
    {
        MovieThemeManager movieThemeManager;
        private ObservableCollection<ThemeModel> thememodels;
        private ThemeModel currentskin;
        private DelegateCommand<ThemeModel> themecommand;

        public DelegateCommand<ThemeModel> ThemeCommand
        {
            get
            {
                if (themecommand == null)
                    themecommand = new DelegateCommand<ThemeModel>((sender) =>
                    {
                        CurrentSkin = sender;
                    });
                return themecommand;
            }
        }

        public ObservableCollection<ThemeModel> ThemeModels
        {
            get { return thememodels; }
            set { thememodels = value; RaisePropertyChanged(() => this.ThemeModels); }
        }

        public ThemeModel CurrentSkin
        {
            get { return currentskin; }
            set
            {
                currentskin = value;
                if (value != null)
                {
                    movieThemeManager.SetNewCurrent(value);
                }
                RaisePropertyChanged(() => this.CurrentSkin);
            }
        }

        public ThemeLoaderViewModel()
        {
            movieThemeManager = new MovieThemeManager();
            ThemeModels = new ObservableCollection<ThemeModel>()
            {
                new ThemeModel
                {
                    ThemeName = "Black Skin",
                    ThemePath = "/Movies.Themes;component/Themes/BlackSkin.xaml",
                    IsActive = false
                },
                new ThemeModel
                {
                    ThemeName = "White Skin",
                    ThemePath = "/Movies.Themes;component/Themes/WhiteSkin.xaml",
                    IsActive = false
                },
                new ThemeModel
                {
                    ThemeName = "Hybrid Skin",
                    ThemePath = "/Movies.Themes;component/Themes/Hybrid.xaml",
                    IsActive = false
                }
            };
            CurrentSkin = ThemeModels[0];
        }
    }
}
