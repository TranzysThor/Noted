using System.ComponentModel;
using System.Windows;
using Capstone.Core;
using Capstone.MVVM.Model;
using Capstone.MVVM.View;

namespace Capstone.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand DiscoveryViewCommand { get; set; }
        public RelayCommand BrowseViewCommand { get; set; }
        private DocumentModel _document;
        public EditorViewModel Editor { get; set; }
        public FileViewModel File { get; set; } 
        public DiscoveryViewModel DiscoveryVM { get; set; }
        public BrowseViewModel BrowseVM { get; set; }
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel()
        {
            DiscoveryVM = new DiscoveryViewModel();
            BrowseVM = new BrowseViewModel();
            _document = new DocumentModel();
            Editor = new EditorViewModel(_document);
            File = new FileViewModel(_document);

            CurrentView = DiscoveryVM;

            DiscoveryViewCommand = new RelayCommand(o =>
            {
                CurrentView = DiscoveryVM;
            });
            BrowseViewCommand = new RelayCommand(o =>
            {
                CurrentView = BrowseVM;
            });
        }
    }
}