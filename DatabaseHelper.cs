using System;
using System.Data.SqlClient;

namespace TeamDinnerOD
{
    public class DatabaseHelper
    {
        //測試資料庫

        private static string DebugCon = @"Data Source=sqlName;Initial Catalog=RESERVE;User ID=sa;Password=";

        //正式用

        private static string ReleaseCon = "Data Source=sqlName;Initial Catalog=RESERVE;User ID=sa;Password=";




        // 根據編譯條件選擇使用的連線字串
        public static string GetConnectionString()
        {
#if DEBUG
            return DebugCon;
#else
        return ReleaseCon;
#endif
        }

        // 取得 SQL 連線物件
        public static SqlConnection GetConnection()
        {
            string connString = GetConnectionString();
            return new SqlConnection(connString);
        }

    }

}
