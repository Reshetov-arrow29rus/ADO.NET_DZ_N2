using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace ADO.NET_DZ_N2
{
    /// <summary>
    /// Логика взаимодействия для Add_a_position.xaml
    /// </summary>
    public partial class Add_a_position : Window
    {
        public Add_a_position(SqlConnection connection)
        {
            InitializeComponent();
        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            // Создание таблицы с нужными столбцами
            DataTable productTable = new DataTable();
            productTable.Columns.Add("Name", typeof(string));
            productTable.Columns.Add("TypeProduct", typeof(string));
            productTable.Columns.Add("Price", typeof(decimal));
            productTable.Columns.Add("DateOfDelivery", typeof(DateTime));
            productTable.Columns.Add("Count", typeof(int));
            productTable.Columns.Add("Provider", typeof(string));

            try
            {
                DataRow newRow = productTable.NewRow();
                newRow["Name"] = titleTextBox.Text;
                newRow["TypeProduct"] = typeTextBox.Text; // Сохранение названия типа
                newRow["Price"] = Convert.ToDecimal(priceTextBox.Text);
                newRow["DateOfDelivery"] = dateTimePicker.Text;
                newRow["Count"] = Convert.ToInt32(countTextBox.Text);
                newRow["Provider"] = providerTextBox.Text; // Сохранение названия поставщика

                productTable.Rows.Add(newRow);

                AddEditDelete addEditDelete = new AddEditDelete(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString);
                addEditDelete.InsertDataFromTable(productTable);

                MessageBox.Show("Объект успешно добавлен в таблицу!");

                // Очистка полей ввода в Add_a_position
                titleTextBox.Text = string.Empty;
                typeTextBox.Text = string.Empty;
                priceTextBox.Text = string.Empty;
                dateTimePicker.Text = string.Empty;
                countTextBox.Text = string.Empty;
                providerTextBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка, объект не может быть добавлен: " + ex.Message);
            }

        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {

            // Проверяем пустые ли значения в TextBox
            if (string.IsNullOrEmpty(titleTextBox.Text) || string.IsNullOrEmpty(typeTextBox.Text) ||
                string.IsNullOrEmpty(priceTextBox.Text) || dateTimePicker.SelectedDate == null ||
                string.IsNullOrEmpty(countTextBox.Text) || string.IsNullOrEmpty(providerTextBox.Text))
            {
                // Если хотя бы одно значение пустое, делаем кнопку добавления некликабельной
                AddButton.IsEnabled = false;
            }
            else
            {
                // Проверяем корректность данных в TextBox, где это необходимо
                int value;
                decimal price;
                if (decimal.TryParse(priceTextBox.Text, out price) && int.TryParse(countTextBox.Text, out value))
                {
                    // Если все значения заполнены и корректны, делаем кнопку добавления кликабельной
                    AddButton.IsEnabled = true;
                }
                else
                {
                    // Если значение некорректное, делаем кнопку добавления некликабельной
                    AddButton.IsEnabled = false;
                }
            }
        }
    }
}
