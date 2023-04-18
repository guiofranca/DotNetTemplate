using Template.MigrationRunner.Migrator;

namespace Template.MigrationRunner.ConsoleMenu;

//https://stackoverflow.com/questions/60767909/c-sharp-console-app-how-do-i-make-an-interactive-menu
public class MenuRunner
{
    private readonly List<Option> _options;
    private readonly Runner _runner;
    public MenuRunner(Runner runner)
    {
        _runner = runner;
        _options = new List<Option>
        {
            new Option("List Migrations", () => GetUserInputForSelectedOption(MigratorSelect.ListMigrations)),
            new Option("MigrationUp", () =>  GetUserInputForSelectedOption(MigratorSelect.MigrateUp)),
            new Option("MigrationDown", () =>  GetUserInputForSelectedOption(MigratorSelect.MigrateDown)),
            new Option("Rollback", () =>  GetUserInputForSelectedOption(MigratorSelect.Rollback)),
            new Option("Exit", () => Environment.Exit(0)),
        };
    }

    public void Run(string[] args)
    {
        HandleArgs(args);

        RunMenu();
    }

    private void HandleArgs(string[] args)
    {
        if (args.Count() == 0) return;
        if (args.Count() > 2) InvalidOperation();
        switch (args.First().ToLower())
        {
            case "--help":
                ShowHelp();
                break;
            case "--listmigrations" or "--list":
                _runner.ExecuteOption(MigratorSelect.ListMigrations);
                break;
            case "--migrateup" or "--up":
                _runner.ExecuteOption(MigratorSelect.MigrateUp);
                break;
            case "--migratedown" or "--down":
                if (args.Count() != 2) InvalidOperation();
                var versionRead = args[1];
                long version = long.MaxValue;
                Int64.TryParse(versionRead, out version);
                _runner.ExecuteOption(MigratorSelect.MigrateDown, version: version);
                break;
            case "--rollback":
                if (args.Count() != 2) InvalidOperation();
                var rollbackRead = args[1];
                int rollback = 0;
                Int32.TryParse(rollbackRead, out rollback);
                _runner.ExecuteOption(MigratorSelect.Rollback, rollback: rollback);
                break;
        }

        Environment.Exit(0);
    }

    private static void InvalidOperation()
    {
        System.Console.WriteLine("Invalid operation. Try --help.");
        Environment.Exit(1);
    }

    private void ShowHelp()
    {
        Console.WriteLine("This is the console migration tool to easily manage database migrations using FluentMigrator");
        Console.WriteLine("Set the database's Connection String under appSettings.ConnectionStrings.Default");
        System.Console.WriteLine("The following options are available:");
        System.Console.WriteLine("--help:");
        System.Console.WriteLine("\tShow this help.");
        System.Console.WriteLine("--listmigrations | --list");
        System.Console.WriteLine("\tShow the migration status");
        System.Console.WriteLine("--migrateup | --up");
        System.Console.WriteLine("\tRuns all unapplied migrations");
        System.Console.WriteLine("--migratedown {version} | --down {version}");
        System.Console.WriteLine("\tDown migrations to a specified version. Ex.: --down 20230101101010");
        System.Console.WriteLine("--rollback {steps}");
        System.Console.WriteLine("\tRollback the number of steps specified. Ex.: --rollback 1");
        Environment.Exit(0);
    }

    private void RunMenu()
    {
        // Set the default index of the selected item to be the first
        int index = 0;

        // Write the menu out
        WriteMenu(_options, _options[index]);

        // Store key info in here
        ConsoleKeyInfo keyinfo;
        do
        {
            keyinfo = Console.ReadKey();

            // Handle each key input (down arrow will write the menu again with a different selected item)
            if (keyinfo.Key == ConsoleKey.DownArrow)
            {
                if (index + 1 < _options.Count)
                {
                    index++;
                    WriteMenu(_options, _options[index]);
                }
            }
            if (keyinfo.Key == ConsoleKey.UpArrow)
            {
                if (index - 1 >= 0)
                {
                    index--;
                    WriteMenu(_options, _options[index]);
                }
            }
            // Handle different action for the option
            if (keyinfo.Key == ConsoleKey.Enter)
            {
                _options[index].Selected.Invoke();
                WaitForMenu();
                index = 0;
            }
        }
        while (keyinfo.Key != ConsoleKey.X);

        Console.ReadKey();
    }

    private void GetUserInputForSelectedOption(MigratorSelect migratorSelect)
    {
        try
        {
            switch (migratorSelect)
            {
                case MigratorSelect.ListMigrations:
                    _runner.ExecuteOption(migratorSelect);
                    break;
                case MigratorSelect.MigrateUp:
                    _runner.ExecuteOption(migratorSelect);
                    break;
                case MigratorSelect.MigrateDown:
                    Console.Write("Specify the version to migrate down to (leave empty to go back): ");
                    var versionRead = Console.ReadLine();
                    if (string.IsNullOrEmpty(versionRead)) break;
                    long version = long.MaxValue;
                    Int64.TryParse(versionRead, out version);
                    _runner.ExecuteOption(migratorSelect, version: version);
                    break;
                case MigratorSelect.Rollback:
                    Console.Write("Specify how many rollbacks: ");
                    var rollbackRead = Console.ReadLine();
                    int rollback = 0;
                    Int32.TryParse(rollbackRead, out rollback);
                    _runner.ExecuteOption(migratorSelect, rollback: rollback);
                    break;
            }
        }
        catch { }
    }

    private void WaitForMenu()
    {
        Console.WriteLine("Press return to continue...");
        Console.ReadLine();
        Console.Clear();
        WriteMenu(_options, _options.First());
    }

    private static void WriteMenu(List<Option> options, Option selectedOption)
    {
        Console.Clear();

        foreach (Option option in options)
        {
            if (option == selectedOption)
            {
                Console.Write("> ");
            }
            else
            {
                Console.Write(" ");
            }

            Console.WriteLine(option.Name);
        }
    }
}