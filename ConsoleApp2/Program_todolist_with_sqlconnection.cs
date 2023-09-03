using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ToDoListApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=DESKTOP-Q8H73VQ\SQLEXPRESS;Initial Catalog=ToDoListDB;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                bool isRunning = true;

                while (isRunning)
                {
                    Console.WriteLine("To-Do List Application\n");
                    Console.WriteLine("1. Add Task");
                    Console.WriteLine("2. View Active Tasks");
                    Console.WriteLine("3. View Completed Tasks");
                    Console.WriteLine("4. Mark Task as Completed");
                    Console.WriteLine("5. Quit");

                    Console.Write("\nEnter your choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            AddTask(connection);
                            break;
                        case "2":
                            ViewTasks(connection, "Active Tasks:");
                            break;
                        case "3":
                            ViewTasks(connection, "Completed Tasks:");
                            break;
                        case "4":
                            MarkTaskAsCompleted(connection);
                            break;
                        case "5":
                            isRunning = false;
                            Console.WriteLine("Goodbye!");
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please choose again.");
                            break;
                    }

                    Console.WriteLine();
                }
            }
        }

        static void AddTask(SqlConnection connection)
        {
            Console.Write("Enter the task: ");
            string task = Console.ReadLine();

            string insertQuery = $"INSERT INTO Tasks (TaskName) VALUES ('{task}')";

            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Task added!");
            }
        }

        static void ViewTasks(SqlConnection connection, string title)
        {
            int isCompleted = (title == "Active Tasks:") ? 0 : 1;

            string selectQuery = $"SELECT TaskName FROM Tasks WHERE IsCompleted = {isCompleted}";
            
            List<string> taskList = new List<string>();

            using (SqlCommand command = new SqlCommand(selectQuery, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    taskList.Add(reader.GetString(0));
                }
            }

            if (taskList.Count == 0)
            {
                Console.WriteLine($"No {title.ToLower()}.");
            }
            else
            {
                Console.WriteLine(title);
                for (int i = 0; i < taskList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {taskList[i]}");
                }
            }
        }

        static void MarkTaskAsCompleted(SqlConnection connection)
        {
            ViewTasks(connection, "Active Tasks:");

            string selectQuery = "SELECT TaskName FROM Tasks where IsCompleted = 0";
            List<string> tasks = new List<string>();

            using (SqlCommand command = new SqlCommand(selectQuery, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tasks.Add(reader.GetString(0));
                }
            }
            if (tasks.Count > 0)
            {
                Console.Write("Enter the task number to mark as completed: ");
                if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
                {
                    string taskName = tasks[taskNumber - 1];
                    string updateQuery = $"UPDATE Tasks SET IsCompleted = 1 WHERE TaskName = '{taskName}'";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Task marked as completed!");
                        }
                        else
                        {
                            Console.WriteLine("Error marking task as completed.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid task number.");
                }
            }
        }
    }
}

this is very good really appreciate you