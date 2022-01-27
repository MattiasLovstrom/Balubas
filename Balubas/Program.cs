using System;

namespace Balubas
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var application = new Application();
                if (args.Length == 2 && args[0] == "createwallet") application.CreateWallet(friendlyName: args[1]);
                else if (args.Length == 3 && args[0] == "send") application.Send(walletFriendlyName: null, toPublicKey: args[1], amountString: args[2]);
                else if (args.Length == 4 && args[0] == "send") application.Send(walletFriendlyName: args[1], toPublicKey: args[2], amountString: args[3]);
                else if (args.Length == 1 && args[0] == "balance") application.WalletBalance();
                else if (args.Length == 2 && args[0] == "balance") application.WalletBalance(walletFriendlyName: args[1]);
                else if (args.Length == 1 && args[0] == "server") application.StartServer();
                else if (args.Length == 1 && args[0] == "creategenesis") application.CreateGenesis();
                else
                {
                    Console.Out.WriteLine("Usage:");
                    Console.Out.WriteLine("   balance [wallet friendly name] - show wallet balance");
                    Console.Out.WriteLine("   send [wallet friendly name] publicKey amount   - creates a new wallet");
                    Console.Out.WriteLine("   createwallet <friendly name> - creates a new wallet");
                    Console.Out.WriteLine("   server  - start application as WebAPI");
                    Console.Out.WriteLine("Wallets:");
                    application.ListWallets();
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
