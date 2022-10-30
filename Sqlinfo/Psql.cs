using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqlinfo
{
    internal class Psql
    {
        public static void Print(string host, string username, string password, string port = null)
        {
            if (port == null) { port = "5432"; }
            Console.WriteLine($"\n[ PostgreSQL ] Host: {host} Username: {username} Password: {password}");
            ArrayList Datebase = Database(host, username, password, port);
            foreach (string date in Datebase)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  [*] DataBases: " + date + " ");
                ArrayList Tables = Table(host, username, password, date,port);
                foreach (string table in Tables)
                {
                    ArrayList Columns = Column(host, username, password, date, table, port);
                    int count = Count(host, username, password, date, table, port);
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

        private static ArrayList Database(string ip, string username, string password, string port)
        {
            ArrayList datebase = new ArrayList();
            string ConStr = $"PORT={port};HOST={ip};PASSWORD={password};USER ID={username};Timeout=2;";
            NpgsqlConnection SqlConn = new NpgsqlConnection(ConStr);
            try
            {

                SqlConn.Open();
                var sql = "select datname from pg_database";
                var cmd = new NpgsqlCommand(sql, SqlConn);
                var PsqlReader = cmd.ExecuteReader();
                while (PsqlReader.Read())
                {
                    if ((PsqlReader.GetString(0) != "postgres") && (PsqlReader.GetString(0) != "template1") && (PsqlReader.GetString(0) != "template0"))
                    {
                        datebase.Add(PsqlReader.GetString(0));
                    }
                }
                PsqlReader.Close();
            }
            catch
            {
                Console.WriteLine("  Connection timed out!");
            }
            finally
            {
                SqlConn.Close();
            }
            return datebase;
        }

        private static ArrayList Table(string ip, string username, string password, string database, string port)
        {
            ArrayList table = new ArrayList();
            string ConStr = $"PORT={port};HOST={ip};PASSWORD={password};USER ID={username};Database={database};";
            NpgsqlConnection SqlConn = new NpgsqlConnection(ConStr);
            try
            {

                SqlConn.Open();
                var sql = "SELECT tablename FROM pg_tables WHERE tablename NOT LIKE'pg%' AND tablename NOT LIKE'sql_%'";
                var cmd = new NpgsqlCommand(sql, SqlConn);
                var PsqlReader = cmd.ExecuteReader();
                while (PsqlReader.Read())
                {
                    table.Add(PsqlReader.GetString(0));
                }
                PsqlReader.Close();
            }
            finally
            {
                SqlConn.Close();
            }
            return table;
        }

        private static ArrayList Column(string ip, string username, string password, string database,string table, string port)
        {
            ArrayList column = new ArrayList();
            string ConStr = $"PORT={port};HOST={ip};PASSWORD={password};USER ID={username};Database={database};";
            NpgsqlConnection SqlConn = new NpgsqlConnection(ConStr);
            try
            {

                SqlConn.Open();
                var sql = $"SELECT A.attname FROM pg_class AS C,pg_attribute AS A WHERE C.relname = '{table}' AND A.attrelid = C.oid AND A.attnum > 0";
                var cmd = new NpgsqlCommand(sql, SqlConn);
                var PsqlReader = cmd.ExecuteReader();
                while (PsqlReader.Read())
                {
                    column.Add(PsqlReader.GetString(0));
                }
                PsqlReader.Close();
            }
            finally
            {
                SqlConn.Close();
            }
            return column;
        }

        private static int Count(string ip, string username, string password, string database,string table, string port)
        {
            string ConStr = $"PORT={port};HOST={ip};PASSWORD={password};USER ID={username};Database={database};";
            NpgsqlConnection SqlConn = new NpgsqlConnection(ConStr);
            try
            {
                SqlConn.Open();//打开通道，建立连接，可能出现异常,使用try catch语句
                string sql = "select count(*) from " + table;
                var cmd = new NpgsqlCommand(sql, SqlConn);
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
                SqlConn.Close();
            }
            return 0;
        }
    }
}
