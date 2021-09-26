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
                    try
                    {
                        Console.WriteLine("Main Menu:");
                        Console.WriteLine(
                            "1) Enter SQL\n" +
                            "2) Load SQL from a file\n" +
                            "3) Export database to SQL file\n" +
                            "4) Example: Search for user (vulnerable to SQL injection)\n" +
                            "5) Example: Search for user (safe)\n" +
                            "Q) Quit");
                        // prompt for SQL or command
                        string input = Console.ReadLine();
                        switch (input)
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

                            // query the sample user database (vulnerable to SQL injection)
                            case "4":
                                QueryUserUnsafe(connection);
                                break;

                            // query the sample user database but filter out any SQL injection
                            case "5":
                                QueryUserSafe(connection);
                                break;

                            // quit
                            case "q":
                            case "Q":
                                running = false;
                                break;
                        }
                    } catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }
            }

        }

        private static void QueryUserSafe(SqliteConnection connection)
        {
            // ask for which user to search for
            Console.Write("Search for a user: ");
            string name = Console.ReadLine();

            string sql = "SELECT * FROM People WHERE firstname LIKE @name OR lastname LIKE @name";
            
            // create a list of SQL parameters that can be safely filtered before being added into the SQL statement
            List<SqliteParameter> sqlParams = new List<SqliteParameter>();
            sqlParams.Add(new SqliteParameter("@name", $"%{name}%"));

            // Display SQL statement and query parameters
            Console.WriteLine($"Executing query: {sql}");
            foreach(SqliteParameter p in sqlParams)
            {
                Console.WriteLine($"Param: {p.ParameterName}: {p.Value}");
            }

            DisplaySQLQueryResults(connection, sql, sqlParams);
            SqliteParameter[] parameters = new SqliteParameter[1];
            
        }

        private static void QueryUserUnsafe(SqliteConnection connection)
        {
            // ask for which user to search for
            Console.Write("Search for a user: ");
            string name = Console.ReadLine();

            // construct SQL statement
            string sql = $"SELECT * FROM People WHERE firstname LIKE '%{name}%' OR lastname LIKE '%{name}%'";
            Console.WriteLine($"Executing query: {sql}");
            DisplaySQLQueryResults(connection, sql);
        }

        private static void ExportToSQLFile(SqliteConnection connection)
        {
            throw new NotImplementedException();
        }

        public static void LoadFromSQLFile(SqliteConnection connection) {
            Console.Write("Enter filename [default: db.sql]: ");
            string filename = Console.ReadLine();
            if(filename == "")
            {
                filename = "db.sql";
            }
            Console.WriteLine($"Loading SQL from {filename}");
            string sql = System.IO.File.ReadAllText(filename);
            SqliteCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Executed:\n {sql}");
        }

        public static void DisplaySQLQueryResults(SqliteConnection connection, string sql, List<SqliteParameter> sqlParams = null)
        {
            // execute query
            SqliteCommand cmd = connection.CreateCommand();

            cmd.CommandText = sql;
            
            // add SQL parameters if they exist
            if(sqlParams != null)
            {
                cmd.Parameters.AddRange(sqlParams);
            }

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

            DisplaySQLQueryResults(connection, sql);
        }
    }
}
