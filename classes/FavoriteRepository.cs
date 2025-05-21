using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace mapApi.classes
{
    internal class FavoriteRepository
    {
        private readonly string _connectionString;

        public FavoriteRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
        }

        public void AddFavorite(MyLocale locale)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open(); // db 연결 

                    string sql = "INSERT INTO Favorite (Name, Lat, Lng) VALUES (@Name, @Lat, @Lng)";
                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Name", locale.Name);
                    cmd.Parameters.AddWithValue("@Lat", locale.Lat);
                    cmd.Parameters.AddWithValue("@Lng", locale.Lng);

                    int result = cmd.ExecuteNonQuery();
               
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("DB 저장 오류: " + ex.ToString());
            }
        }

        public List<MyLocale> GetAllFavorites()
        {
            var list = new List<MyLocale>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT Name, Lat, Lng FROM Favorite", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new MyLocale(
                            reader.GetString(0),
                            reader.GetDouble(1),
                            reader.GetDouble(2)
                        ));
                    }
                }
            }
            return list;
        }
    }
}