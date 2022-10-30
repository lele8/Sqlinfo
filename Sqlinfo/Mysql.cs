using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqlinfo
{
    internal class Mysql
    {
        public static void Print(string host, string username, string password, string port = null)
        {
            if (port == null) { port = "3306"; }
            Console.WriteLine($"\n[ MYSQL ] Host: {host} Username: {username} Password: {password}");
            ArrayList Datebase = MySQL_DateBase(host, username, password, port);
            foreach (string date in Datebase)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  [*] DataBases: " + date + " ");
                ArrayList Tables = MySQL_Table(host, username, password, date,port);
                foreach (string table in Tables)
                {
                    ArrayList Columns = MySQL_Column(host, username, password, date, table, port);
                    int count = MySQL_Count(host, username, password, date, table, port);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\t[+] Tables: " + String.Format("{0,-12}", table));
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"\n\t\tCount: {count}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("\t\t[-] Columns: [");
                    foreach (string column in Columns)
                    {
                        Console.Write(column + " ");
                    }
                    Console.WriteLine("]\n");
                }
            }
        }
        private static ArrayList MySQL_DateBase(string server, string username, string password, string port)
        {
            //Ip+端口+数据库名+用户名+密码
            string connectStr = "server=" + server + ";port=" + port + ";database=information_schema" + ";user=" + username + ";password=" + password + ";Connect Timeout=2";
            ArrayList datebase = new ArrayList();
            MySqlConnection conn = new MySqlConnection(connectStr); ;
            try
            {
                conn.Open();//跟数据库建立连接，并打开连接
                string sql = "select schema_name from  information_schema.schemata";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySql.Data.MySqlClient.MySqlDataReader msqlReader = cmd.ExecuteReader();
                while (msqlReader.Read())
                {   //do something with each record
                    //  Console.WriteLine(" Datebase: " + msqlReader[0]);
                    if ((msqlReader[0].ToString() != "information_schema") && (msqlReader[0].ToString() != "mysql") && (msqlReader[0].ToString() != "performance_schema") && (msqlReader[0].ToString() != "sys"))
                    {
                        datebase.Add(msqlReader[0]);
                    }
                }
            }
            catch
            {
                Console.WriteLine("  Connection timed out!");
            }
            finally
            {
                conn.Clone();
            }
            return datebase;
        }


        private static ArrayList MySQL_Table(string server, string username, string password, string database, string port)
        {
            //Ip+端口+数据库名+用户名+密码
            string connectStr = "server=" + server + ";port=" + port + ";database=information_schema" + ";user=" + username + ";password=" + password + ";";
            ArrayList tables = new ArrayList();
            MySqlConnection conn = new MySqlConnection(connectStr); ;
            try
            {
                conn.Open();//跟数据库建立连接，并打开连接
                string sql = "select table_name from information_schema.tables where table_schema='" + database + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySql.Data.MySqlClient.MySqlDataReader msqlReader = cmd.ExecuteReader();
                while (msqlReader.Read())
                {   //do something with each record
                    tables.Add(msqlReader[0]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Clone();
            }
            return tables;
        }



        private static ArrayList MySQL_Column(string server, string username, string password, string database, string table, string port)
        {
            //Ip+端口+数据库名+用户名+密码
            string connectStr = "server=" + server + ";port=" + port + ";database=information_schema" + ";user=" + username + ";password=" + password + ";";
            ArrayList columns = new ArrayList();
            MySqlConnection conn = new MySqlConnection(connectStr); ;
            try
            {
                conn.Open();//跟数据库建立连接，并打开连接
                string sql = "select column_name from information_schema.columns where table_schema='" + database + "' and table_name='" + table + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySql.Data.MySqlClient.MySqlDataReader msqlReader = cmd.ExecuteReader();
                while (msqlReader.Read())
                {   //do something with each record
                    columns.Add(msqlReader[0]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Clone();
            }
            return columns;
        }



        private static int MySQL_Count(string server, string username, string password, string database, string table, string port)
        {
            string connectStr = "server=" + server + ";port=" + port + ";database=" + database + ";user=" + username + ";password=" + password + ";";
            // server=127.0.0.1/localhost 代表本机，端口号port默认是3306可以不写
            MySqlConnection conn = new MySqlConnection(connectStr);
            try
            {
                conn.Open();//打开通道，建立连接，可能出现异常,使用try catch语句
                string sql = "select count(*) from " + table;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                Object result = cmd.ExecuteScalar();//执行查询，并返回查询结果集中第一行的第一列。所有其他的列和行将被忽略。select语句无记录返回时，ExecuteScalar()返回NULL值
                if (result != null)
                {
                    int count = int.Parse(result.ToString());
                    return count;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return 0;
        }
    }
}
