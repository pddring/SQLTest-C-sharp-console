using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // setup database
            using (SqliteConnection connection = new SqliteConnection("Data Source=data.db"))
            {
                connection.Open();

                bool running = true;
                Console.WriteLine("SQL Database test. Type exit to quit");
                while (running)
                {
                    string input = Console.ReadLine();
                    if (input.ToLower() == "exit")
                    {
                        running = false;
                    }
                    else
                    {
                        SqliteCommand cmd = connection.CreateCommand();
                        cmd.CommandText = input;
                        try
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                int fieldCount = reader.FieldCount;
                                List<Type> fieldTypes = new List<Type>();

                                // display field names
                                for (int i = 0; i < fieldCount; i++)
                                {
                                    fieldTypes.Add(reader.GetFieldType(i));
                                    Console.Write(String.Format("{0,20}", reader.GetName(i) + "(" + reader.GetFieldType(i).Name + ") |"));
                                }
                                Console.WriteLine();

                                // display rows
                                while (reader.Read())
                                {
                                    object[] fields = new object[fieldCount];
                                    reader.GetValues(fields);

                                    for (int i = 0; i < fieldCount; i++)
                                    {
                                        Console.Write(String.Format("{0,20}", fields[i] + " |"));
                                    }
                                    Console.WriteLine();
                                }
                            }
                        } catch(Exception e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                        }
                    }
                }
            }
        }
    }
}
