using Bittrex.Net.Objects;
using ConsoleTables;
using System;
using System.Linq;

namespace MyCoinsConsole
{
    public static class BittrexOpenOrdersOrderExtensions
    {
        public static void PrintOrders(this BittrexOpenOrdersOrder[] orders)
        {
            var currentOrders = orders.ToList();
            Console.WriteLine($"Got {currentOrders.Count} orders.");
            var table = new ConsoleTable("Open At", "Market", "Quantity", "Price", "ID");

            foreach (var order in currentOrders)
            {
                table.AddRow(
                    order.Opened.ToString("dd/MM/yyyy HH:mm"),
                    order.Exchange,
                    order.QuantityRemaining,
                    order.OrderType == OrderTypeExtended.LimitBuy ? order.Price : order.Limit,
                    order.OrderUuid);
            }
            table.Write(Format.Alternative);
        }
    }
}
