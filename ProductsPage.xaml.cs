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
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private Entities context = new Entities();
        private Product currentProduct;

        public ProductsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                CategoryCombo.SelectedValuePath = "CategoryID";
                CategoryCombo.ItemsSource = context.Categories.ToList();

                CategoryCombo.ItemsSource = context.Categories.ToList();
                StatusText.Text = "Данные загружены";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                var product = new Product
                {
                    Name = ProductName.Text,
                    Category = (CategoryCombo.SelectedItem as Category).CategoryID,
                    Price = decimal.Parse(ProductPrice.Text),
                    Quantity = int.Parse(ProductQuantity.Text)
                };

                context.Products.Add(product);
                context.SaveChanges();
                UpdateStatistics();
                LoadData();
                ClearForm();
                StatusText.Text = "Товар добавлен";
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            currentProduct = (sender as Button).DataContext as Product;
            if (currentProduct != null)
            {
                ProductName.Text = currentProduct.Name;
                ProductPrice.Text = currentProduct.Price.ToString();
                ProductQuantity.Text = currentProduct.Quantity.ToString();
                CategoryCombo.SelectedValue = currentProduct.Category;
                StatusText.Text = "Редактирование товара";
            }
        }

        private void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            if (currentProduct != null && ValidateInput())
            {
                currentProduct.Name = ProductName.Text;
                currentProduct.Price = decimal.Parse(ProductPrice.Text);
                currentProduct.Quantity = int.Parse(ProductQuantity.Text);
                currentProduct.Category = (CategoryCombo.SelectedItem as Category).CategoryID;

                context.SaveChanges();
                UpdateStatistics();
                LoadData();
                ClearForm();
                StatusText.Text = "Товар обновлен";
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).DataContext as Product;
            if (product != null && MessageBox.Show("Удалить товар?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                context.Products.Remove(product);
                context.SaveChanges();
                UpdateStatistics();
                LoadData();
                StatusText.Text = "Товар удален";
            }
        }

        private bool ValidateInput()
        {
            // Проверяем, что текст не является placeholder'ом
            if (ProductName.Text == "Название товара" ||
                string.IsNullOrWhiteSpace(ProductName.Text) ||
                CategoryCombo.SelectedItem == null ||
                ProductPrice.Text == "Цена" ||
                !decimal.TryParse(ProductPrice.Text, out decimal price) ||
                ProductQuantity.Text == "Количество" ||
                !int.TryParse(ProductQuantity.Text, out int quantity))
            {
                MessageBox.Show("Проверьте правильность введенных данных!");
                return false;
            }
            return true;
        }

        private void ClearForm()
        {
            ProductName.Text = "Название товара";
            ProductPrice.Text = "Цена";
            ProductQuantity.Text = "Количество";
            CategoryCombo.SelectedIndex = -1;
            currentProduct = null;
        }

        private void UpdateStatistics()
        {
            var stats = context.Statistics.FirstOrDefault();
            if (stats != null)
            {
                var products = context.Products.ToList();
                stats.TotalProducts = products.Count;
                stats.AveragePrice = products.Average(p => p.Price);
                stats.TotalValue = products.Sum(p => p.Price * p.Quantity);
                context.SaveChanges();
            }
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            StatusText.Text = "Редактирование отменено";
        }

        // Методы для placeholder эффекта
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Name == "ProductName" && textBox.Text == "Название товара")
                    textBox.Text = "";
                else if (textBox.Name == "ProductPrice" && textBox.Text == "Цена")
                    textBox.Text = "";
                else if (textBox.Name == "ProductQuantity" && textBox.Text == "Количество")
                    textBox.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Name == "ProductName" && string.IsNullOrWhiteSpace(textBox.Text))
                    textBox.Text = "Название товара";
                else if (textBox.Name == "ProductPrice" && string.IsNullOrWhiteSpace(textBox.Text))
                    textBox.Text = "Цена";
                else if (textBox.Name == "ProductQuantity" && string.IsNullOrWhiteSpace(textBox.Text))
                    textBox.Text = "Количество";
            }
        }

        private void ProductsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Логика при выборе строки в DataGrid
            if (ProductsGrid.SelectedItem is Product selectedProduct)
            {
                // Автоматически заполняем форму при выборе товара в таблице
                ProductName.Text = selectedProduct.Name;
                ProductPrice.Text = selectedProduct.Price.ToString();
                ProductQuantity.Text = selectedProduct.Quantity.ToString();

                // Устанавливаем выбранную категорию в комбобоксе
                if (selectedProduct.Category != null)
                {
                    CategoryCombo.SelectedValue = selectedProduct.Category;
                }

                currentProduct = selectedProduct;
                StatusText.Text = $"Выбран товар: {selectedProduct.Name}";
            }
        }
    }
}