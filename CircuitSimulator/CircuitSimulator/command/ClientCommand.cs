using CircuitSimulator.worker;
using System;

namespace CircuitSimulator.command
{
    public class ClientCommand : ICommand
    {
        public ClientCommand() { }

        public void Execute()
        {
            Console.WriteLine("ホストアドレスを入力してください");
            var address =  Console.ReadLine();
            Console.WriteLine("ポートを入力してください");
            var port = int.Parse(Console.ReadLine());

            try
            {
                var client = new WorkerClient(address, port);
                client.ConnectAndStartJob().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }
        }

        public string GetCommandType()
        {
            return "c";
        }
    }
}
