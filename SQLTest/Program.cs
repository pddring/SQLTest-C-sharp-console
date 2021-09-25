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
                Console.WriteLine("SQL Database test. ");
                while (running)
                {
                    Console.WriteLine("Main Menu:");
                    Console.WriteLine(
                        "1) Enter SQL\n" +
                        "2) Load SQL from a file\n" + 
                        "3) Export database to SQL file\n" + 
                        "4) Quit");
                    // prompt for SQL or command
                    string input = Console.ReadLine();
                    switch(input)
                    {
                        // enter and run a single SQL query manually into the console
                        case "1": 
                            EnterSingleSQLQuery(connection);
                            break;
                        
                        // load and execute queries from an SQL file
                        case "2":
                            LoadFromSQLFile(connection);
                            break;

                        // dump database to SQL file
                        case "3":
                            ExportToSQLFile(connection);
                            break;

                        // quit
                        case "4":
                            running = false;
                            break;
                    }
                }
            }

        }

        private static void ExportToSQLFile(SqliteConnection connection)
        {
            throw new NotImplementedException();
        }

        public static void LoadFromSQLFile(SqliteConnection connection) {
            throw new NotImplementedException();
        }
        
        public static void EnterSingleSQLQuery(SqliteConnection connection)
        {

            // allow the user to enter SQL over multiple lines
            Console.WriteLine("Type in some SQL and then enter an empty line to execute it");
            string sql = "";
            bool finished = false;
            while(!finished)
            {
                string line = Console.ReadLine();
                if(line == "")
                {
                    finished = true;
                }
                sql += line;
            }

            // execute query
            SqliteCommand cmd = connection.CreateCommand();

            cmd.CommandText = sql;
            try
            {
                // try to get results
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

            }
            catch (Exception e)
            {
                // display error message if something has gone wrong
                Console.WriteLine("Error: " + e.Message);
            }
        }
    }
}
