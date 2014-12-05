using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public interface ITestEnvironment
    {
        IList<WindowDescription> Windows { get; }
        String Identifier { get; }
        String Summary { get; }
    }

    public struct WindowDescription
    {
        public int width;
        public int height;
        public String title;

        public WindowDescription(String title, int width, int height) : this()
        {
            this.title = title;
            this.width = width;
            this.height = height;
        }
    }

    public sealed class WindowCollection : IEnumerable<TestWindow>, IDisposable
    {
        private readonly IList<TestWindow> windowList = new List<TestWindow>();

        public TestWindow this[int index]
        {
            get
            {
                return windowList[index];
            }
        }

        public WindowCollection(IList<WindowDescription> windows)
        {
            foreach (var window in windows) {
                windowList.Add(new TestWindow(window.title,
                                              window.width,
                                              window.height));
            }
        }

        public IEnumerator<TestWindow> GetEnumerator()
        {
            return windowList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ~WindowCollection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing) {
                    foreach (var window in windowList) {
                        window.Dispose();
                    }
                }

                disposed = true;
            }
        }

        private bool disposed = false;
    }

    public static class TestRunner
    {
        private const int EXIT_SUCCESS = 0;
        private const int EXIT_FAILURE = 1;

        public static bool IntegrityTests()
        {
            using (var window = new TestWindow("Integrity Test", 800, 600)) {
                window.Run();
            }

            return true;
        }

        public static bool RunTest(Func<WindowCollection, bool> test, IList<WindowDescription> windowDescr) {
            using (var windows = new WindowCollection(windowDescr)) {
                foreach (var window in windows) {
                    window.ProcessEvents();
                }

                var testResult = new TaskFactory().StartNew<bool>(() => test(windows));

                while (!testResult.IsCompleted) {
                    foreach (var window in windows) {
                        window.ProcessEvents();
                    }
                }

                return testResult.Result;
            }
        }

        public static IDictionary<Func<WindowCollection, bool>, ITestEnvironment> Tests = new Dictionary<Func<WindowCollection, bool>, ITestEnvironment>()
        {
            { TestSingleContext.DoNothing, new TestSingleContext() }
        };

        public static int Main(String[] args)
        {
            foreach (var test in Tests) {
                if ((args.Length > 0) && (!args.Contains(test.Value.Identifier))) {
                    Console.WriteLine("[-] Skipping {0}", test.Value.Identifier);
                    continue;
                }

                Console.WriteLine("[+] Running {0} :: {1}", test.Value.Identifier, test.Value.Summary);
                Console.WriteLine();

                if (!RunTest(test.Key, test.Value.Windows)) {
                    Console.WriteLine("\n[!] Test failed!");
                    return EXIT_FAILURE;
                } else {
                    Console.WriteLine();
                }
            }

            Console.WriteLine("[+] All tests passed.");

            return EXIT_SUCCESS;
        }
    }
}

