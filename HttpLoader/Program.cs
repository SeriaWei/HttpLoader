using HttpLoader.HttpParser;
using HttpLoader.RequestClient;

namespace HttpLoader
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string file = "Request.http";
            int delay = 100;
            if (args.Length > 0)
            {
                file = args[0];
            }
            if (args.Length > 1)
            {
                delay = int.Parse(args[1]);
            }
            string requestDefinition = File.ReadAllText(file);
            var requests = HttpRequestContent.ParseMultiple(requestDefinition);
            List<HttpRequesetSender> senders = new List<HttpRequesetSender>();
            foreach (var request in requests)
            {
                senders.Add(new HttpRequesetSender(request));
            }
            Console.WriteLine("Request: {0}, Delay: {1}ms/request", requests.Count(), delay);
            Console.WriteLine("Will start after 3 seconds. Press Ctrl + C exit.");
            await Task.Delay(3000);
            var tasks = new Task[senders.Count];
            for (int i = 0; i < senders.Count; i++)
            {
                var sender = senders[i];
                tasks[i] = Task.Run(async () =>
                {
                    while (true)
                    {
                        Console.WriteLine(sender.Action);
                        var start = DateTime.Now;
                        var response = await sender.SendAsync();
                        Console.WriteLine(response.StatusCode + " " + (DateTime.Now - start).TotalMilliseconds + "ms");
                        await Task.Delay(delay);
                    }
                });
            }
            await Task.WhenAll(tasks);
        }
    }
}