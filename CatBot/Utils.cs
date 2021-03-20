
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CatBot
{


    class Utils
    {
        public static string sConn;
        public static string catToken;

        internal static string GetCatPic()
        {
            var arr = JArray.Parse(HttpRequest("https://api.thecatapi.com/v1/images/search", new Dictionary<string, string>
            {
                { "x-api-key", catToken }
            }));
            return arr[0]["url"].ToString();
        }

        internal static string GetUniqueCatPic(long chat_id)
        {
            bool isNew = false;
            string link = "";
            int res = 0;
            while (!isNew)
            {
                link = GetCatPic();
                res = ExecuteScalar($"exec bot.isUniqueCat '{link}', {chat_id}");
                isNew = res != -1;

            }
            if (res > 1)
                return "-1";
            else
                return link;
        }

        private static string HttpRequest(string link, Dictionary<string, string> headers)
        {
            var client = new WebClient();
            string result = "";
            foreach(var item in headers) {
                client.Headers.Add(item.Key, item.Value);
            };
            
            using (Stream stream = client.OpenRead(link))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result += reader.ReadToEnd();
                }
            }
            return result;
        }

        internal static void Log(string error, string method)
        {
            try
            {
                using (var conn = new SqlConnection(sConn))
                {
                    string query = $@"exec bot.write_log '{error.Replace("'", "''")}', '{method}'";
                    //Debug.Write(query);
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            } catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }

        internal static DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(sConn))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt = new DataTable();
                        dt.Load(reader);
                    }
                    cmd.Connection.Close();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            return dt;
        }

        internal static void ExecuteNonQuery(string query)
        {
            using (var conn = new SqlConnection(sConn))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
        }

        internal static int ExecuteScalar(string query)
        {
            int res = 0;
            using (var conn = new SqlConnection(sConn))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Connection.Open();
                res = (int)cmd.ExecuteScalar();
                cmd.Connection.Close();
            }
            return res;
        }

        internal static void WriteUptime(string serviceName)
        {
            var dt = DateTime.Now;
            StreamWriter handle = File.AppendText(@"O:\monitor.axeit.ru\data.csv");
            string[] arr = { serviceName, dt.ToString("yyyy-MM-dd HH:mm:ss"), "1" };
            fputcsv(handle, arr, ";", "\"");
            handle.Close();
        }

        internal static bool fputcsv(StreamWriter handle, string[] arr, string delimiter, string fieldDelimiter)
        {
            string row = "";
            bool needFieldDelimiter = false;
            int i = 0;
            foreach (string item in arr)
            {
                needFieldDelimiter = item.Contains("\n") || item.Contains("\r") || item.Contains("\t") || item.Contains(" ");
                row += (needFieldDelimiter ? fieldDelimiter : "") + item + (needFieldDelimiter ? fieldDelimiter : "");
                if (i == arr.Length - 1) break;
                row += delimiter;
                i++;
            }
            handle.WriteLine(row);
            return true;
        }
    }
}
