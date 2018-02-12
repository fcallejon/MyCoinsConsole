using Bittrex.Net;
using ConsoleTables;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Linq;

namespace MyCoinsConsole
{
    public static class CheckWalletsCommand
    {
        public static void AddCheckWalletsCommand(this CommandLineApplication app)
        {
            app.Command("check-wallets", cmd =>
            {
                cmd.Description = "Get wallet data for your account.";
                cmd.HelpOption("-?|-h|--help");
                
                cmd.OnExecute(async () =>
                {
                    Bittrex.Net.BittrexDefaults.SetDefaultApiCredentials(Program.ApiKey.Value(), Program.ApiSecret.Value());
                    Console.WriteLine("Getting balances ...");
                    var client = new BittrexClient();
                    var balances = await client.GetBalancesAsync();

                    if (!balances.Success)
                    {
                        Console.WriteLine("Something happend while trying to get the balances ...");
                        Console.WriteLine(balances.Error.ErrorMessage);
                        return -1;
                    }

                    var balancesWithSomething = balances.Result
                        .Where(b => b.Balance.GetValueOrDefault() > 0.0m)
                        .ToList();
                    Console.WriteLine($"Got {balancesWithSomething.Count} balances (with something).");
                    var table = new ConsoleTable("Coin", "Total", "In BTC");

                    foreach (var balance in balancesWithSomething)
                    {
                        var inBtc = client.GetTicker($"BTC-{balance.Currency}");
                        table.AddRow(balance.Currency, balance.Balance.GetValueOrDefault(),
                            balance.Currency == "BTC" ? balance.Balance.GetValueOrDefault() : inBtc.Success ? inBtc.Result.Last : 0);
                    }

                    table.Write(Format.Alternative);

                    return 0;
                });

            });
        }
    }
}
