using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Sqlinfo
{
    internal class redis
    {
        public static void GetAllkeys(string host, string password, string port = null)
        {
            try
            {
                if (port == null) { port = "6379"; }
                List<string> listKeys = new List<string>();
                Console.WriteLine($"\n[ Redis ] Host: {host} Password: {password}");
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect($"{host}:{port},password={password},connectTimeout=2000"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("  [*] All Keys:");
                    for (int i = 0; i <= 15; i++)
                    {
                        IServer keys = redis.GetServer(host, int.Parse(port));
                        Console.ForegroundColor = ConsoleColor.Green;
                        foreach (var key in keys.Keys(i))
                        {
                            Console.Write($"\t{key}");
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    redis.Close();
                }
            }
            catch { Console.WriteLine("  Connection timed out!");}
        }
    }
}
