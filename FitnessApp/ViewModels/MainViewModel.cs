using FitnessApp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace FitnessApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Service> _services;
        private ObservableCollection<Service> _filteredServices;
        private string _selectedCategory = "Все категории";
        private string _searchText;

        public ObservableCollection<Service> Services
        {
            get => _services;
            set
            {
                _services = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Service> FilteredServices
        {
            get => _filteredServices;
            set
            {
                _filteredServices = value;
                OnPropertyChanged();
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                FilterServices();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterServices();
            }
        }

        public List<string> Categories { get; } = new List<string>
        {
            "Все категории",
            "Йога",
            "Кардио",
            "Силовые",
            "Бассейн"
        };

        public MainViewModel()
        {
            try
            {
                LoadServices();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке услуг: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadServices()
        {
            using (var context = new Data.FitnessContext())
            {
                var servicesList = context.Services
                    .AsNoTracking()
                    .OrderBy(s => s.Category)
                    .ThenBy(s => s.Name)
                    .ToList();

                Services = new ObservableCollection<Service>(servicesList);
                FilteredServices = new ObservableCollection<Service>(Services);
            }
        }

        private void FilterServices()
        {
            if (Services == null) return;

            var filtered = Services.AsEnumerable();

            // Фильтрация по категории
            if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "Все категории")
            {
                filtered = filtered.Where(s => s.Category == SelectedCategory);
            }

            // Фильтрация по поисковому запросу
            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filtered = filtered.Where(s =>
                    s.Name.ToLower().Contains(searchLower) ||
                    s.Description.ToLower().Contains(searchLower) ||
                    s.Category.ToLower().Contains(searchLower));
            }

            FilteredServices = new ObservableCollection<Service>(filtered);
        }

        public void RefreshServices()
        {
            LoadServices();
            FilterServices();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}