using Mono.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace rTimeSheet_StressCaller {
    class Program {

        static void Main(string[] args) {

            bool help;
            string strPort = null, strRequests = null;
            int port = 80;
            string host = null;
            int requests = 1;

            OptionSet option_set = new OptionSet()
                .Add("?|help|h", "Prints out options", op => help = op != null)
                .Add("host=", "hostname / ip where to make the requests", op => host = op)
                .Add("port=", "destination port nº", op => strPort = op)
                .Add("n=", "Number of Requests (default: 1)", op => strRequests = op);


            try {
                option_set.Parse(args);
            } catch (OptionException ex) {
                ShowHelp(string.Format("Error {0}", ex.Message), option_set);
                return; //redundant 
            }

            if (strPort != null) {
                if (!int.TryParse(strPort, out port)) ShowHelp("Invalid port number", option_set);
                if (port <= 20) ShowHelp("Invalid port number", option_set);
            }

            if (strRequests != null) {
                if (!int.TryParse(strRequests, out requests)) ShowHelp("Invalid number of requests", option_set);
                if (requests <= 0) ShowHelp("Invalid number of requests", option_set);
            }

            if (host == null) ShowHelp("Host is required", option_set);

            DoWork(host, port, requests);

        }

        public static void DoWork(string host, int port, int requests) {

            Console.WriteLine("Sending {0} requests with 16 timesheets each", requests);

            var start = DateTime.Now;
            try {
                for (int i = 0; i < requests; i++) {
                    RunAsync(host, port).Wait();
                }
            } finally {
                Console.WriteLine("Done in: {0}", DateTime.Now.Subtract(start));
            }
        }

        //public static WorkShell(object threadContext) {

        //}


        public static void ShowHelp(string message, OptionSet option_set) {
            Console.Error.WriteLine(message);
            option_set.WriteOptionDescriptions(Console.Error);
            Environment.Exit(-1);
        }

        static async Task RunAsync(string host, int port) {
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(string.Format("http://{0}:{1}/", host, port));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                ExecutionParameters obj = GetExecutionPostData();
                var response = await client.PostAsJsonAsync("api/Engine/Calculate/", obj);

                if (response.IsSuccessStatusCode) {
                    Console.WriteLine("OK: {0}", response.ReasonPhrase);
                } else
                    Console.WriteLine("Error: {0}", response.ReasonPhrase);

            }
        }

        private static object _locker = new object();
        private static ExecutionParameters _data = null;

        public static ExecutionParameters GetExecutionPostData() {
            if (_data == null) {
                lock (_locker) {
                    if (_data == null) {

                        var resourceName = "rTimeSheet_StressCaller.Data2.json";
                        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

                        string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                        string jsonData = "{ }";

                        try {
                            StreamReader reader = new StreamReader(stream);
                            jsonData = reader.ReadToEnd();
                        } catch {

                        }

                        _data = JsonConvert.DeserializeObject<ExecutionParameters>(jsonData);
                    }
                }
            }
            return _data;
        }
    }
}
