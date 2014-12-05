using System;
using System.Collections.Generic;

namespace Tests
{
    public class TestSingleContext : ITestEnvironment
    {
        public IList<WindowDescription> Windows {
            get {
                return new List<WindowDescription>()
                {
                    new WindowDescription("Basic Window", 1024, 768)
                };
            }
        }

        public String Identifier {
            get { return "T0001"; }
        }

        public String Summary {
            get { return  "Single AntTweakBar context"; }
        }

        public static bool DoNothing(WindowCollection windows)
        {
            Console.WriteLine("[?] Does this show a black window with a help icon in the bottom left?");
            if (Console.ReadLine() == "N") {
                return false;
            }

            Console.WriteLine("[?] If you click the icon, does it expand into a help bar?");
            if (Console.ReadLine() == "N") {
                return false;
            }

            Console.WriteLine("[?] Can you drag the help bar around the window?");
            if (Console.ReadLine() == "N") {
                return false;
            }

            windows[0].ATBCtx.ShowHelpBar(false);

            Console.WriteLine("[?] Did the help bar or icon disappear?");
            if (Console.ReadLine() == "N") {
                return false;
            }

            windows[0].ATBCtx.ShowHelpBar(true);

            Console.WriteLine("[?] Did the help bar or icon reappear?");
            if (Console.ReadLine() == "N") {
                return false;
            }

            return true;
        }
    }
}

