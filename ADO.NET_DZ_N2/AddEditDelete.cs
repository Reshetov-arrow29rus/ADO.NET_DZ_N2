using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;

namespace ADO.NET_DZ_N2
{
    internal class AddEditDelete
    {
        //SqlConnection connection;
        //SqlCommand command;
        //SqlDataAdapter adapter;

        private string connectionString;
        int typeProductID;
        int providerID;

        public AddEditDelete(string connectionString)
        {
            this.connectionString = connectionString;
        }

        //Методы для добавления нового товара в таблицу базы данных
        public void InsertDataFromTable(DataTable dataTable)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();

            // Записываем данные в таблицу "TypeProduct"
            using (SqlDataAdapter typeProductAdapter = new SqlDataAdapter("SELECT * FROM TypeProduct", connection))
            {
                //SCOPE_IDENTITY() - это функция Transact-SQL в Microsoft SQL Server, которая возвращает последний автоматически
                //сгенерированный идентификатор (ID) в текущей области видимости.
                typeProductAdapter.InsertCommand = new SqlCommand("INSERT INTO TypeProduct (Name) VALUES (@Name);  SELECT SCOPE_IDENTITY()", connection);
                typeProductAdapter.InsertCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 50, "Name");

                // Записываем измененные данные в таблицу "TypeProduct"
                typeProductAdapter.Update(dataTable);
                // Получаем ID вставленной записи из таблицы "TypeProduct"
                typeProductID = Convert.ToInt32(typeProductAdapter.InsertCommand.ExecuteScalar());
            }

                // Записываем данные в таблицу "Providers"
            using (SqlDataAdapter providerAdapter = new SqlDataAdapter("SELECT * FROM Providers", connection))
            {
                //SCOPE_IDENTITY() - это функция Transact-SQL в Microsoft SQL Server, которая возвращает последний автоматически
                //сгенерированный идентификатор (ID) в текущей области видимости.
                providerAdapter.InsertCommand = new SqlCommand("INSERT INTO Providers (Name) VALUES (@Name); SELECT SCOPE_IDENTITY()", connection);
                providerAdapter.InsertCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 50, "Name");

                // Записываем измененные данные в таблицу "Providers"
                providerAdapter.Update(dataTable);

                // Получаем ID вставленной записи из таблицы "Providers"
                providerID = Convert.ToInt32(providerAdapter.InsertCommand.ExecuteScalar());

            }

            // Записываем данные в таблицу "Products"
            using (SqlDataAdapter productAdapter = new SqlDataAdapter("SELECT * FROM Products", connection))
            {
                productAdapter.InsertCommand = new SqlCommand("INSERT INTO Products (Name, TypeProductID, Price, DateOfDelivery, Count, ProviderID) " +
                        "VALUES (@Name, @TypeProductID, @Price, @DateOfDelivery, @Count, @ProviderID)", connection);
                productAdapter.InsertCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 50, "Name");
                productAdapter.InsertCommand.Parameters["@TypeProductID"].Value = typeProductID;
                //productAdapter.InsertCommand.Parameters.Add("@TypeProductID", SqlDbType.Int, 0, "TypeProductID");
                productAdapter.InsertCommand.Parameters.Add("@Price", SqlDbType.Money, 0, "Price");
                productAdapter.InsertCommand.Parameters.Add("@DateOfDelivery", SqlDbType.Date, 0, "DateOfDelivery");
                productAdapter.InsertCommand.Parameters.Add("@Count", SqlDbType.Int, 0, "Count");
                productAdapter.InsertCommand.Parameters["@ProviderID"].Value = providerID;
                //productAdapter.InsertCommand.Parameters.Add("@ProviderID", SqlDbType.Int, 0, "ProviderID");

                // Задаем значения параметров для вставки в таблицу "Products"
                //productAdapter.InsertCommand.Parameters["@TypeProductID"].Value = typeProductID;
                //productAdapter.InsertCommand.Parameters["@ProviderID"].Value = providerID;


                // Записываем измененные данные в таблицу "Products"
                productAdapter.Update(dataTable);
            }
            
        }
    }
}
