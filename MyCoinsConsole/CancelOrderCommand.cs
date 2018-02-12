using Bittrex.Net;
using Bittrex.Net.Objects;
using ConsoleTables;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCoinsConsole
{
    public static class CancelOrderCommand
    {
        public static void AddCancelOrderCommand(this CommandLineApplication app)
        {
            app.Command("cancel-order", cmd =>
            {
                cmd.Description = "Get current open orders for your account.";
                cmd.HelpOption("-?|-h|--help");

                var orderArgument = cmd.Argument("[order]", "Order Id");
                
                cmd.OnExecute(async () =>
                {
                    Bittrex.Net.BittrexDefaults.SetDefaultApiCredentials(Program.ApiKey.Value(), Program.ApiSecret.Value());
                    var inputOrderUuid = orderArgument.Value ?? "INVALID";
                    if (!Guid.TryParse(inputOrderUuid, out var orderUuid))
                    {
                        Console.WriteLine($"{inputOrderUuid} can't be parsed as Guid.");
                        return -1;
                    }

                    Console.WriteLine($"Getting order {orderUuid}");
                    var client = new BittrexClient();
                    var order = await client.GetOrderAsync(orderUuid);

                    if (!order.Success)
                    {
                        Console.WriteLine("Something happend while trying to get the order ...");
                        Console.WriteLine(order.Error.ErrorMessage);
                        return -1;
                    }

                    var currentOrder = order.Result;
                    Console.WriteLine($"Canceling order {orderUuid}...");
                    var cancelOrder = await client.CancelOrderAsync(orderUuid);

                    if (!cancelOrder.Success)
                    {
                        Console.WriteLine("Something happend while trying to cancel the order ...");
                        Console.WriteLine(cancelOrder.Error.ErrorMessage);
                        return -1;
                    }

                    Console.WriteLine("Order cancelled ...");

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
