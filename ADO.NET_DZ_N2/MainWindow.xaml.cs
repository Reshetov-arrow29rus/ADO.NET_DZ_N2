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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ADO.NET_DZ_N2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con;
        SqlCommand com;
        public MainWindow()
        {
            InitializeComponent();
            con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
            com = new SqlCommand();
            com.Connection = con;
        }

        private void Show_Button_Click(object sender, RoutedEventArgs e)
        {
            com.CommandText = ConfigurationManager.AppSettings["ShowAllInfo"];

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(com);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGrid.ItemsSource = dataTable.DefaultView;

            }
            catch (Exception ex)
            {
                // Обработка исключения
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }

        private void Add_a_position_Button_Click(object sender, RoutedEventArgs e)
        {
            // Создаем второе окно и вызываем его.
            // передаем права доступа в класс Add_a_position
            Add_a_position add_A_Position = new Add_a_position(con);
            add_A_Position.ShowDialog();
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("В разработке!");
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            //Сохряняем выделенную строку
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;

            // Создаем второе окно и вызываем его.
            // передаем права доступа в класс Add_a_position
            Edit_a_position edit_a_Position = new Edit_a_position(con, selectedRow);
            edit_a_Position.ShowDialog();
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            //Сохряняем выделенную строку
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            // Получаем Id товара для удаления его из таблиц
            int productID = (int)selectedRow["Id"];

            try
            { 
                AddEditDelete addEditDelete = new AddEditDelete(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString);
                addEditDelete.DeleteRowTable(productID);

                MessageBox.Show("Данный товар и вся информация о нем успешно удалены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Удаление не возможно!" + ex.Message);
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получение выбранных строк
            var selectedItems = dataGrid.SelectedItems;

            // Проверка, что выбрана хотя бы одна строка
            if (selectedItems.Count == 1)
            {
                // Включение кнопки "EditButton" и "DeleteButton"
                EditButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            else
            {
                // Отключение кнопки "EditButton" и "DeleteButton"
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }

        }
    }
}
