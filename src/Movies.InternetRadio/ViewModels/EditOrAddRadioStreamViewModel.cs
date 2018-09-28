using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Movies.InternetRadio.EventHandle;
using Movies.InternetRadio.StreamManager;
using Movies.InternetRadio.Views;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Movies.InternetRadio.ViewModels
{
    public class EditOrAddRadioStream : NotificationObject
    {
        private EditOrAddRadioStreamView editOrAddRadioStreamView;

        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand AddLogoCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        public event EventHandler<NewStationAgrs> OnFinished;

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(() => this.Title); }
        }

        private RadioModel currentradiostation;

        public RadioModel CurrentRadioStation
        {
            get { return currentradiostation; }
            set { currentradiostation = value; RaisePropertyChanged(() => this.CurrentRadioStation); }
        }

        public EditOrAddRadioStream(RadioModel radioStation = null)
        {
            this.AddLogoCommand = new DelegateCommand(AddlogoHandler);
            this.SaveCommand = new DelegateCommand(SaveRadioStatioHandler);
            this.CloseCommand = new DelegateCommand(CloseHandler);

            //this.CurrentRadioStation = radioStation == null ? new RadioModel() :
            //    radioStation.Clone() as RadioModel;

            Title = radioStation == null ? "Create new RadioStation" : "Edit RadioStation";
            editOrAddRadioStreamView = new EditOrAddRadioStreamView() { DataContext = this };
        }

        private void CloseHandler()
        {
            if (OnFinished != null)
                OnFinished.Invoke(this, new NewStationAgrs());
            ModulePage.Docker.Content = null;
        }

        private void SaveRadioStatioHandler()
        {
            if (OnFinished != null)
                OnFinished.Invoke(this, new NewStationAgrs { RadioModel = CurrentRadioStation});
            ModulePage.Docker.Content = null;
        }

        private void AddlogoHandler()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "*.png|*.jpg"
            };
            if(openFileDialog.ShowDialog() == true)
            {
                string ImagePath = openFileDialog.FileName;
                System.Drawing.Image image = System.Drawing.Image.FromFile(ImagePath);
                //CurrentRadioStation.StationUidLogo = ImageToByteArray(image);
            }
        }
        
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public void ShowDialog()
        {
            if (ModulePage.Docker.Content == null)
                ModulePage.Docker.Content = editOrAddRadioStreamView;
        }

        IRadioService IRadioService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRadioService>();
            }
        }

        IMainPage ModulePage
        {
            get { return (IRadioService as RadioService).RadioHomepage; }
        }
    }
}
