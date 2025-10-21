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
    /// Логика взаимодействия для StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {
        private Entities context = new Entities();

        public StatisticsPage()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                // Общая статистика
                var stats = context.Statistics.FirstOrDefault();
                if (stats != null)
                {
                    TotalStats.Text = $"Всего товаров: {stats.TotalProducts}\n" +
                                    $"Средняя цена: {stats.AveragePrice:C2}\n" +
                                    $"Общая стоимость: {stats.TotalValue:C2}";
                }

                // Статистика по категориям
                var categoryStats = context.Categories
                    .Select(c => new
                    {
                        CategoryName = c.CategoryName,
                        ProductCount = c.Products.Count,
                        AveragePrice = c.Products.Any() ? c.Products.Average(p => p.Price) : 0,
                        TotalValue = c.Products.Sum(p => p.Price * p.Quantity)
                    }).ToList();

                CategoryStatsGrid.ItemsSource = categoryStats;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статистики: {ex.Message}");
            }
        }

        private void RefreshStats_Click(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
        }
    }
}