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
                if (args.Length == 2 && args[0] == "createwallet") application.CreateWallet(args[1]);
                else if (args.Length == 3 && args[0] == "send") application.Send(null, args[1], args[2]);
                else if (args.Length == 4 && args[0] == "send") application.Send(args[1], args[2], args[3]);
                else if (args.Length == 1 && args[0] == "balance") application.WalletBalance();
                else if (args.Length == 2 && args[0] == "balance") application.WalletBalance(args[1]);
                else if (args.Length == 1 && args[0] == "creategenesis") application.CreateGenesis();
                else
                {
                    Console.Out.WriteLine("Usage:");
                    Console.Out.WriteLine("   balance [wallet friendly name] - show wallet balance");
                    Console.Out.WriteLine("   send [wallet friendly name] amount publicKey  - creates a new wallet");
                    Console.Out.WriteLine("   createwallet <friendly name> - creates a new wallet");
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }
    }
}
