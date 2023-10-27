using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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

namespace ADO.NET_DZ_N2
{
    /// <summary>
    /// Логика взаимодействия для Edit_a_position.xaml
    /// </summary>
    public partial class Edit_a_position : Window
    {
        int productID;
        public Edit_a_position(SqlConnection connection, DataRowView updateRow)
        {
            InitializeComponent();

            // Заполнение элементов управления информацией из выделенной строки
            titleTextBox.Text = updateRow["Name"].ToString().ToString();
            typeTextBox.Text = updateRow["TypeProductName"].ToString();
            priceTextBox.Text = updateRow["Price"].ToString();
            dateTimePicker.SelectedDate = (DateTime)updateRow["DateOfDelivery"];
            countTextBox.Text = updateRow["Count"].ToString();
            providerTextBox.Text = updateRow["ProviderName"].ToString();

            //Назначаем productID редактируемого товара
            productID = Convert.ToInt32(updateRow["Id"]);
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            // Создание таблицы с нужными столбцами
            DataTable updateTable = new DataTable();
            updateTable.Columns.Add("Name", typeof(string));
            updateTable.Columns.Add("TypeProduct", typeof(string));
            updateTable.Columns.Add("Price", typeof(decimal));
            updateTable.Columns.Add("DateOfDelivery", typeof(DateTime));
            updateTable.Columns.Add("Count", typeof(int));
            updateTable.Columns.Add("Provider", typeof(string));

           // try
           // {
                DataRow newRow = updateTable.NewRow();
                newRow["Name"] = titleTextBox.Text;
                newRow["TypeProduct"] = typeTextBox.Text; // Сохранение названия типа
                newRow["Price"] = Convert.ToDecimal(priceTextBox.Text);
                newRow["DateOfDelivery"] = dateTimePicker.Text;
                newRow["Count"] = Convert.ToInt32(countTextBox.Text);
                newRow["Provider"] = providerTextBox.Text; // Сохранение названия поставщика

                updateTable.Rows.Add(newRow);

                AddEditDelete addEditDelete = new AddEditDelete(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString);
                addEditDelete.UpdateDataInTable(updateTable, productID);

                MessageBox.Show("Объект в таблице успешно обновлен!");

                this.Close();
           // }
           // catch (Exception ex)
           // {
            //    MessageBox.Show("Произошла ошибка, объект не может быть обновлен: " + ex.Message);
           // }
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
