using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoreApp
{
    /// <summary>
    /// Логика взаимодействия для SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        private Entities context = new Entities();

        public SearchPage()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            SearchCategory.ItemsSource = context.Categories.ToList();
        }

        private void SearchProducts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var query = context.Products.Include(p => p.Category1).AsQueryable();

                // Поиск по названию (игнорируем placeholder текст)
                if (!string.IsNullOrWhiteSpace(SearchText.Text) && SearchText.Text != "Введите название...")
                {
                    query = query.Where(p => p.Name.Contains(SearchText.Text));
                }

                // Поиск по категории
                if (SearchCategory.SelectedItem != null)
                {
                    var selectedCategory = SearchCategory.SelectedItem as Category;
                    query = query.Where(p => p.Category1.CategoryID == selectedCategory.CategoryID);
                }


                SearchResultsGrid.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}");
            }
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchText.Text = "Введите название...";
            SearchCategory.SelectedIndex = -1;
            SearchResultsGrid.ItemsSource = null;
        }

        // Методы для placeholder эффекта в SearchText
        private void SearchText_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchText.Text == "Введите название...")
            {
                SearchText.Text = "";
            }
        }

        private void SearchText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchText.Text))
            {
                SearchText.Text = "Введите название...";
            }
        }
    }
}