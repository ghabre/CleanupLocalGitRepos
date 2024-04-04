using System.Diagnostics;

namespace CleanupRepos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Path");
            var path = Console.ReadLine();
            var gits = Directory.GetDirectories(path, ".git", SearchOption.AllDirectories)
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .Distinct().ToList();

            Console.WriteLine("Found these:");
            foreach (var g in gits)
            {
                Console.WriteLine($"   {g}");
            }
            Console.WriteLine("Proceed? (y/n)");

            char proceedC;
            do
            {
                proceedC = Console.ReadKey().KeyChar;
            }
            while (!((IEnumerable<char>)['y', 'n', 'Y', 'N']).Contains(proceedC));

            bool proceed = proceedC == 'y' || proceedC == 'Y';

            if (proceed)
            {
                foreach (var g in gits)
                {
                    var dir = Directory.GetParent(g).FullName;
                    Directory.SetCurrentDirectory(dir);
                    var clean = $"clean -fdx";
                    var checkout = $"checkout .";

                    var p1 = StartProcess("git", clean);
                    var p2 = StartProcess("git", checkout);

                    Console.WriteLine($"Processed: {dir}");
                }

                Console.WriteLine("Done!!! Press any 3 keys to exit..");
                Console.ReadKey();
                Console.ReadKey();
                Console.ReadKey();
            }
        }

        static Process StartProcess(string fileName, string arguments)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process
            {
                StartInfo = processStartInfo
            };

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            return process;
        }
    }
}
