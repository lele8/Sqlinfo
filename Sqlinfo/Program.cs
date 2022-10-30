using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Sqlinfo
{
    class Program
    {
        public static void banner()
        {
            Console.WriteLine(@"
  ____        _ _        __       
 / ___|  __ _| (_)_ __  / _| ___  
 \___ \ / _` | | | '_ \| |_ / _ \ 
  ___) | (_| | | | | | |  _| (_) |
 |____/ \__, |_|_|_| |_|_|  \___/ 
           |_|                    
");
        }
        static void Main(string[] args)
        {
            banner();
            if (args.Length >= 7 && (args[6] == "-mysql"))
            {
                Mysql.Print(args[1], args[3], args[5]);
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (args.Length >= 7 && (args[6] == "-mssql"))
            {
                Mssql.Print(args[1], args[3], args[5]);
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (args.Length >= 7 && (args[6] == "-psql"))
            {
                Psql.Print(args[1], args[3], args[5]);
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (args.Length >= 5 && (args[4] == "-redis"))
            {
                redis.GetAllkeys(args[1], args[3]);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (args.Length == 2 && args[0] == "-f" || args[0] == "-F")
            {
                redfile(args[1]);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (args.Length <= 1)
            {
                System.Console.WriteLine("Usage: Sqlinfo.exe -f result.txt");
                System.Console.WriteLine("       Sqlinfo.exe -h ip -u username -p password -mysql");
                System.Console.WriteLine("       Sqlinfo.exe -h ip -u username -p password -mssql");
                System.Console.WriteLine("       Sqlinfo.exe -h ip -u username -p password -psql");
                System.Console.WriteLine("       Sqlinfo.exe -h ip -p password -redis");
            }
            //oracle.test();
        }

        public static void redfile(string name)
        {
            try
            {
                List<string> list = new List<string>();
                // 创建一个 StreamReader 的实例来读取文件 
                // using 语句也能关闭 StreamReader
                using (StreamReader sr = new StreamReader(name))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        Regex mysqlreg = new Regex("\\[\\+] mysql:((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}:.*?:.*? (.*)");
                        Match mysql = mysqlreg.Match(line);

                        Regex mssqlreg = new Regex("\\[\\+] mssql:((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}:.*?:.*? (.*)");
                        Match mssql = mssqlreg.Match(line);

                        Regex psqlreg = new Regex("\\[\\+] Postgres:((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}:.*?:.*? (.*)");
                        Match psql = psqlreg.Match(line);

                        Regex redisreg = new Regex("\\[\\+] Redis:((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}:.*?:.*? (.*)");
                        Match redismatch = redisreg.Match(line);

                        if (mysql.Success)
                        {
                            try
                            {
                                foreach (Match m in mysql.Groups)
                                {
                                    if (!string.IsNullOrWhiteSpace(m.Value))
                                    {
                                        string v, ip, port, up, user, pass;
                                        v = m.Value.Replace("[+] mysql", "");
                                        ip = v.Split(':')[1];
                                        port = v.Split(':')[2];
                                        up = v.Split(':')[3];
                                        user = up.Split(' ')[0];
                                        pass = up.Split(' ')[1];
                                        Mysql.Print(ip, user, pass, port);
                                    }
                                }
                            }
                            catch { }
                        }
                        else if (mssql.Success)
                        {
                            try
                            {
                                foreach (Match m in mssql.Groups)
                                {
                                    if (!string.IsNullOrWhiteSpace(m.Value))
                                    {
                                        string v, ip, up, user, pass;
                                        v = m.Value.Replace("[+] mssql", "");
                                        ip = v.Split(':')[1];
                                        up = v.Split(':')[3];
                                        user = up.Split(' ')[0];
                                        pass = up.Split(' ')[1];
                                        Mssql.Print(ip, user, pass);

                                    }
                                }
                            }
                            catch { }
                        }
                        else if (psql.Success)
                        {
                            try
                            {
                                foreach (Match m in psql.Groups)
                                {
                                    if (!string.IsNullOrWhiteSpace(m.Value))
                                    {
                                        string v, ip, port, up, user, pass;
                                        v = m.Value.Replace("[+] Postgres", "");
                                        ip = v.Split(':')[1];
                                        port = v.Split(':')[2];
                                        up = v.Split(':')[3];
                                        user = up.Split(' ')[0];
                                        pass = up.Split(' ')[1];
                                        Psql.Print(ip, user, pass, port);

                                    }
                                }
                            }
                            catch { }
                        } 
                        else if (redismatch.Success)
                        {
                            try
                            {
                                foreach (Match m in redismatch.Groups)
                                {
                                    if (!string.IsNullOrWhiteSpace(m.Value))
                                    {
                                        string v, ip, up, port, pass;
                                        v = m.Value.Replace("[+] Redis","");
                                        ip = v.Split(':')[1];
                                        up = v.Split(':')[2];
                                        port = up.Split(' ')[0];
                                        pass = up.Split(' ')[1];
                                        if (pass.Equals("unauthorized"))
                                        {
                                            pass = "";
                                        }
                                        redis.GetAllkeys(ip, pass, port);
                                    }
                                    
                                }
                            }
                            catch { }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
