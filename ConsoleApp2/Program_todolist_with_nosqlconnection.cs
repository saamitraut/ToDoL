using System;
using System.Collections.Generic;

namespace ToDoListApp
{
    class Program
    {
        static List<string> tasks = new List<string>();
        static List<string> completedTasks = new List<string>();

        static void Main(string[] args)
        {
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
                        AddTask();
                        break;
                    case "2":
                        ViewTasks(tasks, "Active Tasks:");
                        break;
                    case "3":
                        ViewTasks(completedTasks, "Completed Tasks:");
                        break;
                    case "4":
                        MarkTaskAsCompleted();
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

        static void AddTask()
        {
            Console.Write("Enter the task: ");
            string task = Console.ReadLine();
            tasks.Add(task);
            Console.WriteLine("Task added!");
        }

        static void ViewTasks(List<string> taskList, string title)
        {
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

        static void MarkTaskAsCompleted()
        {
            
			ViewTasks(tasks, "Active Tasks");
            
			if (tasks.Count > 0)
            {
                Console.Write("Enter the task number to mark as completed: ");
                if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
                {
                    completedTasks.Add(tasks[taskNumber - 1]);
                    tasks.RemoveAt(taskNumber - 1);
                    Console.WriteLine("Task marked as completed!");
                }
                else
                {
                    Console.WriteLine("Invalid task number.");
                }
            }
        }
    }
}
 