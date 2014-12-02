using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public static class Runner
    {
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

            public WindowCollection(params WindowDescription[] windows)
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

        private const int EXIT_SUCCESS = 0;
        private const int EXIT_FAILURE = 1;

        public static bool IntegrityTests()
        {
            using (var window = new TestWindow("Integrity Test", 800, 600)) {
                window.Run();
            }

            return true;
        }

        public static bool RunTest(Func<WindowCollection, bool> test, params WindowDescription[] windowDescr) {
            using (var windows = new WindowCollection(windowDescr)) {
                var testResult = new TaskFactory().StartNew<bool>(() => test(windows));

                foreach (var window in windows) {
                    window.Visible = true;
                }

                while (!testResult.IsCompleted) {
                    foreach (var window in windows) {
                        window.ProcessEvents();
                    }
                }

                return testResult.Result;
            }
        }

        public static int Main(String[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n==> Running integrity tests\n");

            if (!RunTest((windows) => { System.Threading.Thread.Sleep(2000); windows[0].Close(); return true; },
                         new WindowDescription("Window 1", 800, 600))) {
                Console.WriteLine("FAILED!");
                return EXIT_FAILURE;
            }

            return EXIT_SUCCESS;
        }
    }
}

