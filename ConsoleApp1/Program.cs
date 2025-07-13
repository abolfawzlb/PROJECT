// Base class for transactions
using System.Text.Json;

public abstract class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
}

// Derived class for income
public class Income : Transaction
{
    public string Source { get; set; }
}

// Derived class for expense
public class Expense : Transaction
{
    public string Category { get; set; }
}

// User class
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}

// Main program class
public class Program
{
    private static List<User> users = new List<User>();
    private static List<Transaction> transactions = new List<Transaction>();
    private static List<string> categories = new List<string> { "Food", "Transportation", "Bills", "Shopping", "Entertainment", "Other" };
    private static User currentUser;
    private static int nextUserId = 1;
    private static int nextTransactionId = 1;

    private static readonly string usersFile = "users.json";
    private static readonly string transactionsFile = "transactions.json";
    private static readonly string categoriesFile = "categories.json";

    static void Main(string[] args)
    {
        LoadData();
        while (true)
        {
            DisplayMainMenu();
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1": ManageUsers(); break;
                case "2": ManageTransactions(); break;
                case "3": ManageCategories(); break;
                case "4": GenerateReports(); break;
                case "5": SaveData(); Environment.Exit(0); break;
                default: Console.WriteLine("Invalid choice. Try again."); break;
            }
        }
    }

    private static void DisplayMainMenu()
    {
        Console.Clear();
        Console.WriteLine("Personal Finance Manager");
        Console.WriteLine($"Current User: {(currentUser != null ? currentUser.Username : "None")}");
        Console.WriteLine("1. Manage Users");
        Console.WriteLine("2. Manage Transactions");
        Console.WriteLine("3. Manage Categories");
        Console.WriteLine("4. Generate Reports");
        Console.WriteLine("5. Exit");
        Console.Write("Enter your choice: ");
    }

    // User Management
    private static void ManageUsers()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("User Management");
            Console.WriteLine("1. Add New User");
            Console.WriteLine("2. Change Active User");
            Console.WriteLine("3. View Users");
            Console.WriteLine("4. Back");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Enter username: ");
                    string username = Console.ReadLine();
                    Console.Write("Enter email: ");
                    string email = Console.ReadLine();
                    users.Add(new User { Id = nextUserId++, Username = username, Email = email });
                    SaveData();
                    Console.WriteLine("User added successfully!");
                    Console.ReadKey();
                    break;
                case "2":
                    Console.WriteLine("Select user ID:");
                    ViewUsers();
                    if (int.TryParse(Console.ReadLine(), out int userId))
                    {
                        currentUser = users.Find(u => u.Id == userId);
                        if (currentUser != null)
                            Console.WriteLine($"Active user changed to {currentUser.Username}");
                        else
                            Console.WriteLine("Invalid user ID");
                    }
                    Console.ReadKey();
                    break;
                case "3":
                    ViewUsers();
                    Console.ReadKey();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static void ViewUsers()
    {
        Console.WriteLine("\nUsers List:");
        foreach (var user in users)
        {
            Console.WriteLine($"ID: {user.Id}, Username: {user.Username}, Email: {user.Email}");
        }
    }

    // Transaction Management
    private static void ManageTransactions()
    {
        if (currentUser == null)
        {
            Console.WriteLine("Please select an active user first!");
            Console.ReadKey();
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Transaction Management");
            Console.WriteLine("1. Add Income");
            Console.WriteLine("2. Add Expense");
            Console.WriteLine("3. Edit/Delete Transaction");
            Console.WriteLine("4. View Transactions");
            Console.WriteLine("5. Back");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddIncome();
                    break;
                case "2":
                    AddExpense();
                    break;
                case "3":
                    EditDeleteTransaction();
                    break;
                case "4":
                    ViewTransactions();
                    Console.ReadKey();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static void AddIncome()
    {
        Console.Write("Enter amount: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.Write("Enter source: ");
            string source = Console.ReadLine();
            Console.Write("Enter description: ");
            string description = Console.ReadLine();
            transactions.Add(new Income
            {
                Id = nextTransactionId++,
                Amount = amount,
                Source = source,
                Date = DateTime.Now,
                Description = description
            });
            SaveData();
            Console.WriteLine("Income added successfully!");
        }
        else
        {
            Console.WriteLine("Invalid amount!");
        }
        Console.ReadKey();
    }

    private static void AddExpense()
    {
        Console.WriteLine("Categories:");
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {categories[i]}");
        }
        Console.Write("Select category number: ");
        if (int.TryParse(Console.ReadLine(), out int categoryIndex) && categoryIndex > 0 && categoryIndex <= categories.Count)
        {
            Console.Write("Enter amount: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                Console.Write("Enter description: ");
                string description = Console.ReadLine();
                transactions.Add(new Expense
                {
                    Id = nextTransactionId++,
                    Amount = amount,
                    Category = categories[categoryIndex - 1],
                    Date = DateTime.Now,
                    Description = description
                });
                SaveData();
                Console.WriteLine("Expense added successfully!");
            }
            else
            {
                Console.WriteLine("Invalid amount!");
            }
        }
        else
        {
            Console.WriteLine("Invalid category!");
        }
        Console.ReadKey();
    }

    private static void EditDeleteTransaction()
    {
        ViewTransactions();
        Console.Write("Enter transaction ID to edit/delete (0 to cancel): ");
        if (int.TryParse(Console.ReadLine(), out int id) && id != 0)
        {
            var transaction = transactions.Find(t => t.Id == id);
            if (transaction != null)
            {
                Console.WriteLine("1. Edit");
                Console.WriteLine("2. Delete");
                Console.Write("Choose action: ");
                string action = Console.ReadLine();
                if (action == "1")
                {
                    Console.Write("Enter new amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal newAmount))
                    {
                        transaction.Amount = newAmount;
                        Console.Write("Enter new description: ");
                        transaction.Description = Console.ReadLine();
                        if (transaction is Expense expense)
                        {
                            Console.WriteLine("Categories:");
                            for (int i = 0; i < categories.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {categories[i]}");
                            }
                            Console.Write("Select new category number: ");
                            if (int.TryParse(Console.ReadLine(), out int categoryIndex) && categoryIndex > 0 && categoryIndex <= categories.Count)
                            {
                                expense.Category = categories[categoryIndex - 1];
                            }
                        }
                        else if (transaction is Income income)
                        {
                            Console.Write("Enter new source: ");
                            income.Source = Console.ReadLine();
                        }
                        SaveData();
                        Console.WriteLine("Transaction updated successfully!");
                    }
                }
                else if (action == "2")
                {
                    transactions.Remove(transaction);
                    SaveData();
                    Console.WriteLine("Transaction deleted successfully!");
                }
            }
            else
            {
                Console.WriteLine("Transaction not found!");
            }
        }
        Console.ReadKey();
    }

    private static void ViewTransactions()
    {
        Console.WriteLine("\nTransactions List:");
        foreach (var transaction in transactions)
        {
            if (transaction is Income income)
            {
                Console.WriteLine($"ID: {income.Id}, Type: Income, Amount: {income.Amount}, Source: {income.Source}, Date: {income.Date}, Desc: {income.Description}");
            }
            else if (transaction is Expense expense)
            {
                Console.WriteLine($"ID: {expense.Id}, Type: Expense, Amount: {expense.Amount}, Category: {expense.Category}, Date: {expense.Date}, Desc: {expense.Description}");
            }
        }
    }

    // Category Management
    private static void ManageCategories()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Category Management");
            Console.WriteLine("1. View Categories");
            Console.WriteLine("2. Add Custom Category");
            Console.WriteLine("3. Back");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.WriteLine("\nCategories:");
                    for (int i = 0; i < categories.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {categories[i]}");
                    }
                    Console.ReadKey();
                    break;
                case "2":
                    Console.Write("Enter new category name: ");
                    string newCategory = Console.ReadLine();
                    if (!categories.Contains(newCategory))
                    {
                        categories.Add(newCategory);
                        SaveData();
                        Console.WriteLine("Category added successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Category already exists!");
                    }
                    Console.ReadKey();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    // Report Generation
    private static void GenerateReports()
    {
        if (currentUser == null)
        {
            Console.WriteLine("Please select an active user first!");
            Console.ReadKey();
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Reports");
            Console.WriteLine("1. Monthly Summary");
            Console.WriteLine("2. Category Report");
            Console.WriteLine("3. Daily Balance");
            Console.WriteLine("4. Back");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    MonthlySummary();
                    Console.ReadKey();
                    break;
                case "2":
                    CategoryReport();
                    Console.ReadKey();
                    break;
                case "3":
                    DailyBalance();
                    Console.ReadKey();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static void MonthlySummary()
    {
        Console.Write("Enter year (YYYY): ");
        if (int.TryParse(Console.ReadLine(), out int year))
        {
            Console.Write("Enter month (MM): ");
            if (int.TryParse(Console.ReadLine(), out int month))
            {
                var monthlyTransactions = transactions.Where(t => t.Date.Year == year && t.Date.Month == month);
                decimal totalIncome = monthlyTransactions.OfType<Income>().Sum(i => i.Amount);
                decimal totalExpense = monthlyTransactions.OfType<Expense>().Sum(e => e.Amount);

                Console.WriteLine($"\nMonthly Summary for {month}/{year}");
                Console.WriteLine($"Total Income: {totalIncome}");
                Console.WriteLine($"Total Expenses: {totalExpense}");
                Console.WriteLine($"Balance: {totalIncome - totalExpense}");

                // Simple bar chart
                Console.WriteLine("\nIncome vs Expense Bar Chart:");
                int incomeBars = (int)(totalIncome / 100);
                int expenseBars = (int)(totalExpense / 100);
                Console.WriteLine($"Income:   |{new string('#', incomeBars > 50 ? 50 : incomeBars)}");
                Console.WriteLine($"Expenses: |{new string('#', expenseBars > 50 ? 50 : expenseBars)}");
                Console.WriteLine("(Each # represents approximately 100 units)");
            }
        }
    }

    private static void CategoryReport()
    {
        var expenseByCategory = transactions.OfType<Expense>()
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) })
            .OrderByDescending(g => g.Total);

        Console.WriteLine("\nCategory Report:");
        foreach (var group in expenseByCategory)
        {
            Console.WriteLine($"{group.Category}: {group.Total}");
            int bars = (int)(group.Total / 100);
            Console.WriteLine($"|{new string('#', bars > 50 ? 50 : bars)}");
        }
        Console.WriteLine("(Each # represents approximately 100 units)");
    }

    private static void DailyBalance()
    {
        Console.Write("Enter date (YYYY-MM-DD): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            decimal balance = 0;
            var dailyTransactions = transactions.Where(t => t.Date.Date <= date.Date);
            foreach (var transaction in dailyTransactions)
            {
                if (transaction is Income income)
                    balance += income.Amount;
                else if (transaction is Expense expense)
                    balance -= expense.Amount;
            }
            Console.WriteLine($"\nBalance as of {date.ToShortDateString()}: {balance}");
        }
        else
        {
            Console.WriteLine("Invalid date format!");
        }
    }

    // Data Persistence
    private static void LoadData()
    {
        if (File.Exists(usersFile))
            users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(usersFile));
        if (File.Exists(transactionsFile))
            transactions = JsonSerializer.Deserialize<List<Transaction>>(File.ReadAllText(transactionsFile));
        if (File.Exists(categoriesFile))
            categories = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(categoriesFile));

        if (users.Any()) nextUserId = users.Max(u => u.Id) + 1;
        if (transactions.Any()) nextTransactionId = transactions.Max(t => t.Id) + 1;
    }

    private static void SaveData()
    {
        File.WriteAllText(usersFile, JsonSerializer.Serialize(users));
        File.WriteAllText(transactionsFile, JsonSerializer.Serialize(transactions));
        File.WriteAllText(categoriesFile, JsonSerializer.Serialize(categories));
    }
}


