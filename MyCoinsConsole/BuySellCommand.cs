using Bittrex.Net;
using Bittrex.Net.Objects;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyCoinsConsole
{
    public static class BuySellCommand
    {
        public static void AddBuyCommand(this CommandLineApplication app)
        {
            app.Command("buy", cmd =>
            {
                cmd.Description = "Create new order to buy.";
                cmd.HelpOption("-?|-h|--help");

                var buyingCoinArgument = cmd.Argument("[buy-coin]", "Symbol of the coin to buy. (ie.: ETH)");
                var amountToUseArgument = cmd.Argument("[amount-to-use]", "How much to use from BTC wallet. Specify 'all' to use total balance.");
                var bidArgument = cmd.Argument("[bid]", "Bid amount (value/rate of [destination] coin in [origin] coin). Specify 'last' to use last market value.");

                cmd.OnExecute(async () => await PlaceOrder(OrderType.Buy, buyingCoinArgument.Value, amountToUseArgument, bidArgument));
            });
        }

        public static void AddSellCommand(this CommandLineApplication app)
        {
            app.Command("sell", cmd =>
            {
                cmd.Description = "Create new order to sell.";
                cmd.HelpOption("-?|-h|--help");

                var sellingCoinArgument = cmd.Argument("[sell-coin]", "Symbol of the selling coin. (ie.: ETH)");
                var amountToUseArgument = cmd.Argument("[amount-to-use]", "Amount to use from [sell-coin]. Specify 'all' to use total balance.");
                var askArgument = cmd.Argument("[bid]", "BTC/[sell-coin] to ask. Specify 'last' to use last market value.");

                cmd.OnExecute(async () => await PlaceOrder(OrderType.Sell, sellingCoinArgument.Value, amountToUseArgument, askArgument));
            });
        }

        private static async Task<int> PlaceOrder(
            OrderType type,
            string workingCoin,
            CommandArgument amountToUseArgument,
            CommandArgument bidOrAskArgument)
        {
            BittrexDefaults.SetDefaultApiCredentials(Program.ApiKey.Value(), Program.ApiSecret.Value());
            var client = new BittrexClient();

            if (string.IsNullOrEmpty(workingCoin))
            {
                Console.WriteLine($"Speficy a valid Market.");
                return -1;
            }
            const string referenceCoin = "BTC";

            Console.WriteLine("Getting market info ...");
            var marketSummary = await client.GetMarketSummaryAsync($"{referenceCoin}-{workingCoin}");
            if (!marketSummary.Success)
            {
                Console.WriteLine($"{referenceCoin}-{workingCoin} is not a valid Market.");
                Console.WriteLine(marketSummary.Error.ErrorMessage);
                return -1;
            }

            var originCoin = type == OrderType.Buy ? referenceCoin : workingCoin;
            Console.WriteLine($"Getting your wallet balance for {originCoin}...");
            var originBalance = await client.GetBalanceAsync(originCoin);

            if (!originBalance.Success)
            {
                Console.WriteLine($"Something happened trying to get your balance for {originCoin}.");
                Console.WriteLine(originBalance.Error.ErrorMessage);
                return -1;
            }

            if (0.000009m >= originBalance.Result.Available.GetValueOrDefault(0))
            {
                Console.WriteLine($"The available balance ({originBalance.Result.Available}) is too low.");
                return -1;
            }
            Console.WriteLine($"Current available balance {originCoin} {originBalance.Result.Available}.");


            Console.WriteLine("General cheking of arguments ...");

            var amountToUse = originBalance.Result.Available.GetValueOrDefault();
            var useAll = amountToUseArgument.Value.ToLowerInvariant() == "all";
            if (!useAll && (!decimal.TryParse(amountToUseArgument.Value, out amountToUse) || amountToUse <= 0.0m))
            {
                Console.WriteLine($"{workingCoin} {amountToUseArgument.Value} is not a valid quantity.");
                return -1;
            }
            if (!useAll && amountToUse > originBalance.Result.Available)
            {
                Console.WriteLine(
                    $"The amount to use in the origin wallet ({amountToUse:N9}) " +
                    $"is greater than the available ({originBalance.Result.Available}).");
                return -1;
            }

            Console.WriteLine("Verifying amounts ...");
            var useLast = bidOrAskArgument.Value.ToLowerInvariant() == "last";
            if ((!decimal.TryParse(bidOrAskArgument.Value, out var bidAmount) || bidAmount <= 0.0m)
                && !useLast)
            {
                Console.WriteLine($"{bidOrAskArgument.Value} is not a valid bid amount.");
                return -1;
            }

            if (useLast)
            {
                bidAmount = marketSummary.Result.Last.GetValueOrDefault(0);
            }

            var buyQuantity = amountToUse / bidAmount;
            buyQuantity /= 100;
            buyQuantity *= 25;
            var quantity = type == OrderType.Sell 
                ? amountToUse 
                : Math.Round(amountToUse / bidAmount - buyQuantity, 5, MidpointRounding.ToEven);
            if (quantity <= 0.000000m)
            {
                Console.WriteLine($"The quantity {workingCoin} {amountToUse} is too low to place an order.");
                return -1;
            }

            Console.WriteLine($"Will {(type == OrderType.Buy ? "buy" : "sell")} {(type == OrderType.Buy ? workingCoin : referenceCoin)} {quantity} " +
                $"using {originCoin} {amountToUse}. " +
                $"Rate: {originCoin} {bidAmount}/{workingCoin} 1.");
            
            var market = type == OrderType.Sell ? marketSummary.Result.MarketName : $"{workingCoin}-{referenceCoin}";
            var newOrderStatus = await client.PlaceOrderAsync(type, marketSummary.Result.MarketName, quantity, bidAmount);
            if (!newOrderStatus.Success)
            {
                Console.WriteLine($"Order can't be placed.");
                Console.WriteLine(newOrderStatus.Error.ErrorMessage);
                return -1;
            }
            Console.WriteLine($"Order placed. UUID = {newOrderStatus.Result.Uuid}");
            return 0;
        }
    }
}
