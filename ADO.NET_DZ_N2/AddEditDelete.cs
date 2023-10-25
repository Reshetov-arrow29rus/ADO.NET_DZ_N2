using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;
using System.Configuration.Provider;

namespace ADO.NET_DZ_N2
{
    internal class AddEditDelete
    {
        private string connectionString;
        int typeProductID = -1;
        int providerID = -1;

        public AddEditDelete(string connectionString)
        {
            this.connectionString = connectionString;
        }

        //Методы для добавления нового товара в таблицу базы данных
        public void InsertDataFromTable(DataTable productTable)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Записываем данные в таблицу "TypeProduct"
                using (SqlCommand typeProductCommand = new SqlCommand("INSERT INTO TypeProduct (Name) VALUES (@Name); SELECT SCOPE_IDENTITY()", connection))
                {
                    typeProductCommand.Parameters.AddWithValue("@Name", productTable.Rows[0]["TypeProduct"].ToString());
                    typeProductID = Convert.ToInt32(typeProductCommand.ExecuteScalar());
                }

                //string providerName = productTable.Rows[0]["Provider"].ToString();
                using (SqlCommand providerCommand = new SqlCommand("INSERT INTO Providers (Name) VALUES (@Name); SELECT SCOPE_IDENTITY()", connection))
                {
                    providerCommand.Parameters.AddWithValue("@Name", productTable.Rows[0]["Provider"].ToString());
                    providerID = Convert.ToInt32(providerCommand.ExecuteScalar());
                }

                // Записываем данные в таблицу "Products"
                using (SqlDataAdapter productAdapter = new SqlDataAdapter("SELECT * FROM Products", connection))
                {
                    productAdapter.InsertCommand = new SqlCommand("INSERT INTO Products (Name, TypeProductID, Price, DateOfDelivery, Count, ProviderID) " +
                            "VALUES (@Name, @TypeProductID, @Price, @DateOfDelivery, @Count, @ProviderID)", connection);
                    productAdapter.InsertCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 50, "Name").Value = productTable.Rows[0]["Name"];
                    productAdapter.InsertCommand.Parameters.Add("@TypeProductID", SqlDbType.Int).Value = typeProductID;// Добавляем значение для параметра TypeProductID
                    productAdapter.InsertCommand.Parameters.Add("@Price", SqlDbType.Money, 0, "Price").Value = productTable.Rows[0]["Price"];
                    productAdapter.InsertCommand.Parameters.Add("@DateOfDelivery", SqlDbType.Date, 0, "DateOfDelivery").Value = productTable.Rows[0]["DateOfDelivery"];
                    productAdapter.InsertCommand.Parameters.Add("@Count", SqlDbType.Int, 0, "Count").Value = productTable.Rows[0]["Count"];
                    productAdapter.InsertCommand.Parameters.Add("@ProviderID", SqlDbType.Int).Value = providerID; // Добавляем значение для параметра ProviderID

                    // Записываем измененные данные в таблицу "Products"
                    productAdapter.Update(productTable);
                }
            }
        }

        //Методы для редактирования товара в таблице базы данных
        public void UpdateDataInTable(DataTable productTable)
        {

        }

    }
}
