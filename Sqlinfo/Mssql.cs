using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System;
using System.Collections;

public static class Mssql
{
    public static void QuickOpen(this SqlConnection conn, int timeout)
    {
        // We'll use a Stopwatch here for simplicity. A comparison to a stored DateTime.Now value could also be used
        Stopwatch sw = new Stopwatch();
        bool connectSuccess = false;

        // Try to open the connection, if anything goes wrong, make sure we set connectSuccess = false
        Thread t = new Thread(delegate ()
        {
            try
            {
                sw.Start();
                conn.Open();
                connectSuccess = true;
            }
            catch { }
        });

        // Make sure it's marked as a background thread so it'll get cleaned up automatically
        t.IsBackground = true;
        t.Start();

        // Keep trying to join the thread until we either succeed or the timeout value has been exceeded
        while (timeout > sw.ElapsedMilliseconds)
            if (t.Join(1))
                break;

        // If we didn't connect successfully, throw an exception
        if (!connectSuccess)
            throw new Exception("Timed out while trying to connect.");
    }

    public static void Print(String host, String username, String password)
    {
        Console.WriteLine($"\n[ MSSQL ] Host: {host} Username: {username} Password: {password}");
        ArrayList Datebase = MsSQL_DateBase(host, username, password);
        foreach (string date in Datebase)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  [*] DataBases: " + date + " ");
            ArrayList Tables = MsSQL_Table(host, username, password, date);
            foreach (string table in Tables)
            {
                ArrayList Columns = MsSQL_Column(host, username, password, date, table);
                int count = MsSQL_Count(host, username, password, date, table);
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

    private static ArrayList MsSQL_DateBase(string Server, string User, string Password)
    {
        //Ip+端口+数据库名+用户名+密码
        string connectionString = "Server = " + Server + ";" + "User ID = " + User + ";" + "Password = " + Password + ";Connect Timeout=1;";
        ArrayList datebase = new ArrayList();
        SqlConnection conn = new SqlConnection(connectionString); ;
        try
        {

            //conn.Open();//跟数据库建立连接，并打开连接
            Mssql.QuickOpen(conn, 2000); //连接超时1秒后关闭连接，下一个
            string sql = "SELECT NAME FROM MASTER.DBO.SYSDATABASES ORDER BY NAME";
            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataReader msqlReader = cmd.ExecuteReader();
            while (msqlReader.Read())
            {   //do something with each record
                //  Console.WriteLine(" Datebase: " + msqlReader[0]);
                if ((msqlReader[0].ToString() != "master") && (msqlReader[0].ToString() != "model") && (msqlReader[0].ToString() != "msdb") && (msqlReader[0].ToString() != "tempdb"))
                {
                    datebase.Add(msqlReader[0]);
                }
            }
            msqlReader.Close();     //要记得每次调用SqlDataReader读取数据后，都要Close();
        }
        catch
        {
            Console.WriteLine("  Connection timed out!");
        }
        finally
        {
            conn.Close();
        }
        return datebase;
    }

    private static ArrayList MsSQL_Table(string Server, string User, string Password, string DataBase)
    {
        //Ip+端口+数据库名+用户名+密码
        string connectionString = "Server = " + Server + ";" + "Database =" + DataBase + ";" + "User ID = " + User + ";" + "Password = " + Password + ";Connect Timeout=1;";
        ArrayList tables = new ArrayList();
        SqlConnection conn = new SqlConnection(connectionString);
        try
        {
            conn.Open();//跟数据库建立连接，并打开连接
            string sql = "SELECT NAME FROM SYSOBJECTS WHERE XTYPE='U' ORDER BY NAME";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader msqlReader = cmd.ExecuteReader();
            while (msqlReader.Read())
            {   //do something with each record
                tables.Add(msqlReader[0]);
            }
            msqlReader.Close();     //要记得每次调用SqlDataReader读取数据后，都要Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            conn.Close();
        }
        return tables;
    }

    private static ArrayList MsSQL_Column(string Server, string User, string Password, string DataBase, string table)
    {
        //Ip+端口+数据库名+用户名+密码
        string connectionString = "Server = " + Server + ";" + "Database =" + DataBase + ";" + "User ID = " + User + ";" + "Password = " + Password + ";Connect Timeout=1;";
        ArrayList columns = new ArrayList();
        SqlConnection conn = new SqlConnection(connectionString); ;
        try
        {
            conn.Open();//跟数据库建立连接，并打开连接
            string sql = $"SELECT NAME FROM SYSCOLUMNS WHERE ID=OBJECT_ID('{table}');";
            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataReader msqlReader = cmd.ExecuteReader();
            while (msqlReader.Read())
            {   //do something with each record
                columns.Add(msqlReader[0]);
            }
            msqlReader.Close();     //要记得每次调用SqlDataReader读取数据后，都要Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            conn.Close();
        }
        return columns;
    }

    private static int MsSQL_Count(string Server, string User, string Password, string DataBase, string table)
    {
        //Ip+端口+数据库名+用户名+密码
        string connectionString = "Server = " + Server + ";" + "Database =" + DataBase + ";" + "User ID = " + User + ";" + "Password = " + Password + ";Connect Timeout=1;";
        ArrayList columns = new ArrayList();
        SqlConnection conn = new SqlConnection(connectionString); ;
        try
        {
            conn.Open();//跟数据库建立连接，并打开连接
            string sql = $"select count(*) from [{table}]";
            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataReader msqlReader = cmd.ExecuteReader();
            while (msqlReader.Read())
            {   //do something with each record
                int count = int.Parse(msqlReader[0].ToString());
                return count;
            }
            msqlReader.Close();     //要记得每次调用SqlDataReader读取数据后，都要Close();
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