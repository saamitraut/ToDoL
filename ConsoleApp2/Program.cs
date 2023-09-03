using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ToDoListApp
{
    class Program
    {

        // Global variable to store the username
        private static string loggedInUsername = null;
        private static string SessionId = null;
        
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
                    Console.WriteLine("1. User Registration\t\t2. User Login\t3. Add Task\t4. View Active Tasks\t5. View Completed Tasks");
                    Console.WriteLine("6. Mark Task as Completed\t7. Logout\t8. Quit\t\t9. Assign Role\t\t10. List Roles");
                    
                    Console.Write("\nEnter your choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            RegisterUser(connection);
                            break;
                        case "2":
                            LoginUser(connection);
                            break;
                        case "3":
                            AddTask(connection);
                            break;
                        case "4":
                            ViewTasks(connection, "Active Tasks:");
                            break;
                        case "5":
                            ViewTasks(connection, "Completed Tasks:");
                            break;
                        case "6":
                            MarkTaskAsCompleted(connection);
                            break;
                        case "7":
                            LogoutUser(connection);
                            break;
                        case "8":                            
                            isRunning = false;
                            LogoutUser(connection);
                            Console.WriteLine("Goodbye!");
                            break;
                        case "9":
                            AssignUserRole(connection, loggedInUsername,"Admin");
                            break;
                        case "10":
                            ListRoles(connection);
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please choose again.");
                            break;
                    }

                    Console.WriteLine();
                }
            }
        }


        static void RegisterUser(SqlConnection connection)
        {
            // Check if the user is already logged in (based on an existing session)
            if (IsUserLoggedIn(connection, loggedInUsername))
            {
                Console.WriteLine("You are already logged in.");
                return;
            }

             Console.Write("Enter a username: ");
            string username = Console.ReadLine();

            Console.Write("Enter a password: ");
            string password = Console.ReadLine();

            Console.Write("Enter your email: ");
            string email = Console.ReadLine();

            // TODO: Implement password hashing and validation

            // SQL query to insert the user into the Users table
            string insertUserQuery = $"INSERT INTO Users (Username, PasswordHash, Email) VALUES ('{username}', '{password}', '{email}')";

            using (SqlCommand command = new SqlCommand(insertUserQuery, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Registration successful!");
                }
                else
                {
                    Console.WriteLine("Error registering user.");
                }
            }
        }

        // Method to list all roles
        static void ListRoles(SqlConnection connection)
        {
            Console.WriteLine("Roles:");

            string listRolesQuery = "SELECT RoleName FROM Roles";

            using (SqlCommand command = new SqlCommand(listRolesQuery, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string roleName = reader.GetString(0);
                    Console.WriteLine(roleName);
                }
            }
        }

        // Method to create a new role
        static void CreateRole(SqlConnection connection)
        {
            Console.Write("Enter the name of the new role: ");
            string roleName = Console.ReadLine();

            // Check if the role already exists
            string checkRoleQuery = $"SELECT COUNT(*) FROM Roles WHERE RoleName = '{roleName}'";

            using (SqlCommand checkRoleCommand = new SqlCommand(checkRoleQuery, connection))
            {
                int roleExists = (int)checkRoleCommand.ExecuteScalar();

                if (roleExists > 0)
                {
                    Console.WriteLine($"Role '{roleName}' already exists.");
                }
                else
                {
                    // Insert the new role into the Roles table
                    string insertRoleQuery = $"INSERT INTO Roles (RoleName) VALUES ('{roleName}')";

                    using (SqlCommand insertRoleCommand = new SqlCommand(insertRoleQuery, connection))
                    {
                        int rowsAffected = insertRoleCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Role '{roleName}' created successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Error creating the role.");
                        }
                    }
                }
            }
        }

        // Method to edit an existing role
        static void EditRole(SqlConnection connection)
        {
            Console.Write("Enter the name of the role to edit: ");
            string roleName = Console.ReadLine();

            Console.Write("Enter the new name for the role: ");
            string newRoleName = Console.ReadLine();

            // Check if the role exists
            string checkRoleQuery = $"SELECT COUNT(*) FROM Roles WHERE RoleName = '{roleName}'";

            using (SqlCommand checkRoleCommand = new SqlCommand(checkRoleQuery, connection))
            {
                int roleExists = (int)checkRoleCommand.ExecuteScalar();

                if (roleExists > 0)
                {
                    // Update the role name in the Roles table
                    string updateRoleQuery = $"UPDATE Roles SET RoleName = '{newRoleName}' WHERE RoleName = '{roleName}'";

                    using (SqlCommand updateRoleCommand = new SqlCommand(updateRoleQuery, connection))
                    {
                        int rowsAffected = updateRoleCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Role '{roleName}' updated to '{newRoleName}' successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Error updating the role.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Role '{roleName}' does not exist.");
                }
            }
        }

        static void LoginUser(SqlConnection connection)
        {
            // Check if the user is already logged in (based on an existing session)
            if (IsUserLoggedIn(connection, loggedInUsername))
            {
                Console.WriteLine("You are already logged in.");
                return;
            }

            Console.Write("Enter your username: ");
            string username = Console.ReadLine();

            Console.Write("Enter your password: ");
            string password = Console.ReadLine();

            // TODO: Implement password hashing and validation

            // SQL query to check if the user with the provided credentials exists
            string loginQuery = $"SELECT UserID FROM Users WHERE Username = '{username}' AND PasswordHash = '{password}'";

            using (SqlCommand command = new SqlCommand(loginQuery, connection))
            {
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    int userId = (int)result;

                    // Create a new session record in the Sessions table
                    string sessionId = Guid.NewGuid().ToString();
                    DateTime loginTime = DateTime.Now;

                    // SQL query to insert session information
                    string insertSessionQuery = $"INSERT INTO Sessions (SessionID, UserID, LoginTime) VALUES ('{sessionId}', {userId}, '{loginTime:yyyy-MM-dd HH:mm:ss}')";

                    using (SqlCommand sessionCommand = new SqlCommand(insertSessionQuery, connection))
                    {
                        int rowsAffected = sessionCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Login successful!");
                            Console.WriteLine("Session ID: " + sessionId); SessionId = sessionId;
                            // Set the logged-in username
                            loggedInUsername = username;
                        }
                        else
                        {
                            Console.WriteLine("Error creating session.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid username or password.");
                }
            }
        }

        static void AssignUserRole(SqlConnection connection, string username, string roleName)
        {
            // Check if the user exists
            string checkUserQuery = $"SELECT UserID FROM Users WHERE Username = '{username}'";

            using (SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection))
            {
                object userIdResult = checkUserCommand.ExecuteScalar();

                if (userIdResult != null)
                {
                    int userId = (int)userIdResult;

                    // Check if the role exists
                    string checkRoleQuery = $"SELECT RoleID FROM Roles WHERE RoleName = '{roleName}'";

                    using (SqlCommand checkRoleCommand = new SqlCommand(checkRoleQuery, connection))
                    {
                        object roleIdResult = checkRoleCommand.ExecuteScalar();

                        if (roleIdResult != null)
                        {
                            int roleId = (int)roleIdResult;

                            // Assign the role to the user in the UserRoles table
                            string assignRoleQuery = $"INSERT INTO UserRoles (UserID, RoleID) VALUES ({userId}, {roleId})";

                            using (SqlCommand assignRoleCommand = new SqlCommand(assignRoleQuery, connection))
                            {
                                int rowsAffected = assignRoleCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine($"Role '{roleName}' assigned to user '{username}' successfully.");
                                }
                                else
                                {
                                    Console.WriteLine("Error assigning the role.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Role '{roleName}' does not exist.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"User '{username}' does not exist.");
                }
            }
        }

        static void LogoutUser(SqlConnection connection)
        {
            if (string.IsNullOrEmpty(loggedInUsername))
            {
                Console.WriteLine("You are not currently logged in.");
            }
            else
            {
                Console.WriteLine("Logging out " + loggedInUsername);

                // Update the LogoutTime in the Sessions table for the user's session
                string updateSessionQuery = $"UPDATE Sessions SET LogoutTime = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}' WHERE UserID = (SELECT UserID FROM Users WHERE Username = '{loggedInUsername}') AND SessionId = '{SessionId}'";

                using (SqlCommand command = new SqlCommand(updateSessionQuery, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Logout successful!");
                    }
                    else
                    {
                        Console.WriteLine("Error logging out.");
                    }
                }

                loggedInUsername = null;
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
        static bool IsUserLoggedIn(SqlConnection connection, string username)
        {
            // SQL query to check if the user is already logged in (based on an active session)
            string checkSessionQuery = $"SELECT TOP 1 1 FROM Sessions WHERE UserID = (SELECT UserID FROM Users WHERE Username = '{username}') AND LogoutTime IS NULL";

            using (SqlCommand command = new SqlCommand(checkSessionQuery, connection))
            {
                object result = command.ExecuteScalar();
                return (result != null);
            }
        }

    }
}
