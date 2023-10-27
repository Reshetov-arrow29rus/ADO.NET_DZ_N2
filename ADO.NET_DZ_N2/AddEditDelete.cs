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
using System.Collections;

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
        public void UpdateDataInTable(DataTable productTable, int productID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //Создание объекта SqlDataAdapter с SQL - запросом и параметром
                using (SqlDataAdapter adapter = new SqlDataAdapter(ConfigurationManager.AppSettings["QueryProductId"], connection))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@ProductId", productID);

                    // Создание объекта DataTable
                    DataTable dataTable = new DataTable();

                    // Заполнение DataTable данными из базы данных
                    adapter.Fill(dataTable);

                    // Получение значений TypeProductID и ProviderID из первой строки
                    typeProductID = (int)dataTable.Rows[0]["TypeProductID"];
                    providerID = (int)dataTable.Rows[0]["ProviderID"];
                }

                // Обновление значения названия типа в таблице TypeProduct
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    DataTable typeProductlTable = CreateVirtualTable("TypeProduct", typeProductID, connection);

                    // Изменение данных в виртуальной таблице
                    typeProductlTable.Rows[0]["Name"] = productTable.Rows[0]["TypeProduct"];
                    
                    adapter.UpdateCommand = new SqlCommand(ConfigurationManager.AppSettings["UpdateTypeProductQuery"], connection);
                    adapter.UpdateCommand.Parameters.Add("@TypeProductID", SqlDbType.Int, 0, "Id").Value = typeProductID;
                    adapter.UpdateCommand.Parameters.Add("@TypeName", SqlDbType.NVarChar, 50, "Name").Value = typeProductlTable.Rows[0]["Name"];

                    adapter.Update(typeProductlTable);
                }

                // Обновление значения названия типа в таблице Providers
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    DataTable providersTable = CreateVirtualTable("Providers", providerID, connection);

                    // Изменение данных в виртуальной таблице,
                    providersTable.Rows[0]["Name"] = productTable.Rows[0]["Provider"];
                    
                    adapter.UpdateCommand = new SqlCommand(ConfigurationManager.AppSettings["UpdateProvidersQuery"], connection);
                    adapter.UpdateCommand.Parameters.Add("@ProviderID", SqlDbType.Int, 0, "Id").Value = providerID;
                    adapter.UpdateCommand.Parameters.Add("@ProviderName", SqlDbType.NVarChar, 50, "Name").Value = providersTable.Rows[0]["Name"];

                    adapter.Update(providersTable);
                }

                // обновляем строку в таблице Products
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    DataTable virtualProductsTable = CreateVirtualTable("Products", productID, connection);

                    // Изменение данных в виртуальной таблице
                    virtualProductsTable.Rows[0]["Name"] = productTable.Rows[0]["Name"];
                    virtualProductsTable.Rows[0]["Price"] = productTable.Rows[0]["Price"];
                    virtualProductsTable.Rows[0]["DateOfDelivery"] = productTable.Rows[0]["DateOfDelivery"];
                    virtualProductsTable.Rows[0]["Count"] = productTable.Rows[0]["Count"];
                   
                    adapter.UpdateCommand = new SqlCommand(ConfigurationManager.AppSettings["UpdateProductsQuery"], connection);
                    adapter.UpdateCommand.Parameters.Add("@ProductId", SqlDbType.Int, 0, "Id").Value = productID;
                    adapter.UpdateCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 50, "Name").Value = virtualProductsTable.Rows[0]["Name"];
                    adapter.UpdateCommand.Parameters.Add("@Price", SqlDbType.Money, 0, "Price").Value = virtualProductsTable.Rows[0]["Price"];
                    adapter.UpdateCommand.Parameters.Add("@DateOfDelivery", SqlDbType.Date, 0, "DateOfDelivery").Value = virtualProductsTable.Rows[0]["DateOfDelivery"];
                    adapter.UpdateCommand.Parameters.Add("@Count", SqlDbType.Int, 0, "Count").Value = virtualProductsTable.Rows[0]["Count"];

                    adapter.Update(virtualProductsTable);
                }
            }
        }

        //Методы для удаления товара из таблиц базы данных



        //Создание виртуальных таблиц схожей сигнатурой
        public DataTable CreateVirtualTable(string tableName, int Id, SqlConnection connection)
        {
            DataTable virtualTable = new DataTable(tableName);

            using (SqlDataAdapter adapter = new SqlDataAdapter())
            {
                adapter.SelectCommand = new SqlCommand("SELECT * FROM " + tableName + " WHERE Id = " +Id, connection); // Получение строки которую изменяем
                adapter.Fill(virtualTable); // Заполнение виртуальной таблицы строки которую изменяем
            }
            return virtualTable;
        }
    }
}
