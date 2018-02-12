using System;
using Microsoft.Extensions.CommandLineUtils;

namespace MyCoinsConsole
{
    class Program
    {
        public static CommandOption ApiKey { get; private set; }
        public static CommandOption ApiSecret { get; private set; }

        static void Main(string[] args)
        {
            var app = new CommandLineApplication { Name = "MyCoinsConsole" };
            app.HelpOption("-?|-h|--help");
            
            ApiKey = app.Option("--apiKey <apiKey>", "Your API Key.", CommandOptionType.SingleValue);
            ApiSecret = app.Option("--apiSecret <apiSecret>", "Your API Secret.", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                app.ShowVersion();
                app.ShowHelp();

                return 0;
            });

            app.AddCheckWalletsCommand();
            app.AddCheckOrdersCommand();
            app.AddCancelOrderCommand();
            app.AddBuyCommand();
            app.AddSellCommand();
            app.Execute(args);
        }
    }
}
