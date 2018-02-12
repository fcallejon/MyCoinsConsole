using Bittrex.Net;
using Microsoft.Extensions.CommandLineUtils;
using System;

namespace MyCoinsConsole
{
    public static class CheckOrdersCommand
    {
        public static void AddCheckOrdersCommand(this CommandLineApplication app)
        {
            app.Command("check-orders", cmd =>
            {
                cmd.Description = "Get current open orders for your account.";
                cmd.HelpOption("-?|-h|--help");
                
                cmd.OnExecute(async () =>
                {
                    Console.WriteLine("Getting open orders ...");
                    Bittrex.Net.BittrexDefaults.SetDefaultApiCredentials(Program.ApiKey.Value(), Program.ApiSecret.Value());
                    var client = new BittrexClient();
                    var orders = await client.GetOpenOrdersAsync();

                    if (!orders.Success)
                    {
                        Console.WriteLine("Something happend while trying to get the orders ...");
                        Console.WriteLine(orders.Error.ErrorMessage);
                        return -1;
                    }

                    orders.Result.PrintOrders();

                    return 0;
                });

            });
        }
    }
}
