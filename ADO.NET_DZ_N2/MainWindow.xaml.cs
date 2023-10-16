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

        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
