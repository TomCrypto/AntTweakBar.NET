/* AntTweakBar.NET
 * ===============
 * 
 * This is a sample program which doesn't actually do
 * anything, but shows how to use this wrapper. Since
 * it was designed to be engine-agnostic, each engine
 * specific section is replaced with a comment, which
 * explains what you need to do to integrate the code
 * into your own graphics application. I suggest that
 * you read through the entire source file, though to
 * be fair the wrapper is rather easy to use.
*/

using System;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;

using AntTweakBar;

namespace Tutorial
{
    static class Program
    {
        private const int EXIT_SUCCESS = 0;
        private const int EXIT_FAILURE = 1;

        public static int Main(String[] args)
        {
            try
            {
                /*========================== CONTEXTS ===========================*/

                /* First you need to create a context. A given context conceptually
                 * represents a window (following the AntTweakBar API), and as such
                 * every context has its own separate bars and variables. You can't
                 * share bars across contexts. For the first context, you must give
                 * the graphics API you are going to use (OpenGL or DirectX) - this
                 * is not necessary for subsequent contexts and is just ignored. */

                using (var context = new Context(TW.GraphicsAPI.OpenGL))
                {
                    /*=========================== BARS ==========================*/

                    /* Using this context you can now create (and remove) bars, via
                     * the bar constructor which takes the context to add it to. */

                    var bar = new Bar(context);
                    bar.Label = "My new bar!";

                    /* Bars have a variety of properties that you can manipulate as
                     * if it were a typical control. For example, its title...   */

                    bar.Label = "Another title";

                    /* ... or its color... */

                    bar.Color = Color.BlueViolet;

                    /* ... or whether it must stay within the window... */

                    bar.Contained = true;

                    /* To remove a bar, simply dispose of it, calling Dispose(). */

                    /*======================== VARIABLES ========================*/

                    /* With this bar we can now create variables. There are exactly
                     * 3 variable types in the AntTweakBar object model hierarchy:
                     * - typed variables (integers, colors, vectors, ...)
                     * - buttons
                     * - separators
                     * Starting with the simple stuff first, you can add separators
                     * into bars via their constructor - separators have no special
                     * properties beyond the generic variable properties and can be
                     * used to separate two sets of variables in your bar.       */

                    var separator = new Separator(bar);

                    /* You can remove any variable by simply disposing of them.  */

                    separator.Dispose();

                    /* You can add buttons similarly - buttons have a Clicked event
                     * that you can subscribe to, pretty intuitive and easy.     */

                    var button = new Button(bar);
                    button.Label = "Click this!";

                    button.Clicked += (sender, e) => Console.WriteLine("Clicked!");

                    /* Finally, typed variables are created as you would expect, by
                     * using the relevant variable type, like IntVariable - each of
                     * these types have a "Value" property which can be used to get
                     * or set the value of the variable, as well as a Changed event
                     * which is raised when a variable is changed by the user (note
                     * it isn't raised when the value is set programmatically).  */

                    var intVar = new IntVariable(bar, 10); // 10 = default value

                    intVar.Changed += (sender, e) => Console.WriteLine("Changed!");

                    intVar.Value = 42;

                    var strVar = new StringVariable(bar, "hello");

                    strVar.Value = "12345";

                    /* All of the Separator, Button and Variable types descend from
                     * the base Variable type which has a bunch of properties which
                     * can be used to customize the appearance of the variable.  */

                    button.Group = "New Group"; // button moved into a group
                    intVar.Group = "New Group"; // this is the same group!

                    button.Visible = false; // button is not drawn
                    intVar.ReadOnly = true; // user can't change variable

                    /* Furthermore, the variable types also have additional options
                     * which can be used to further specify their behavior - taking
                     * IntVariable as an example, it offers Min, Max, Step and also
                     * Hexadecimal properties, which are used to:
                     * - Min  => set the minimum value of the variable
                     * - Max  => set the maximum value of the variable
                     * - Step => set the increment by which the variable is to be
                     *           incremented when the user clicks the +/- buttons
                     * - Hexadecimal => set whether to use hexadecimal notation  */

                    intVar.Hexadecimal = true;
                    intVar.Min = -5;
                    intVar.Max = 100;

                    /* That's it. Many variable types are available, notably enums,
                     * which are natively supported, as illustrated below:       */

                    var enumVar = new EnumVariable<Foobar>(bar, Foobar.Cat);
                    enumVar.Changed += (sender, e) => Console.WriteLine("enum!");

                    /* See remarks on the FooBar enum at the end of the source, for
                     * additional information on how to use enums in your bars.  */

                    /*===================== ERROR HANDLING ======================*/

                    /* In principle every AntTweakBar error should be translated to
                     * an exception by AntTweakBar.NET, however, you may still wish
                     * to intercept errors to log them somewhere, this will let you
                     * do it: subscribe your favorite logger to the Error event. */

                    TW.Error += (sender, e) => Debug.WriteLine(e.Error, "GUI/ATB");

                    /*================= EVENT/WINDOW INTEGRATION ================*/

                    /* In order to interact with your newly created bar, you should
                     * pass window events to the AntTweakBar library - this is best
                     * done using the predefined event handlers, which can be found
                     * in the Context class, such as EventHandlerSFML, which simply
                     * take some opaque event structure and return whether you need
                     * to handle the event yourself. There are handlers for popular
                     * window managers, such as SFML, SDL, Winforms, etc... You may
                     * also handle them manually, by calling the Handle* methods as
                     * needed, but beware you will need to do some translation.  */

                    // [hook up context to event loop]

                    /* For AntTweakBar to be able to draw into your window, it must
                     * know its size. For most window managers, the event loop will
                     * automatically handle this, but for some others, you must set
                     * the initial window size manually, through this method - note
                     * you only have to call it once each time the size changes. */

                    context.HandleResize(new Size(1280, 800));

                    /* Finally, right before displaying your frame, call the method
                     * below to tell AntTweakBar to draw every bar in a context, on
                     * top of whatever has been rendered (if anything). Enjoy!   */

                    context.Draw();

                    // [display frame to screen]
                }

                return EXIT_SUCCESS;
            }
            catch (AntTweakBarException e)
            {
                Console.WriteLine("AntTweakBar error: " + e.Message + ".");
                Console.WriteLine("Exception details: " + e.Details + ".");
                Console.WriteLine("\n========== STACK TRACE ==========\n");
                Console.WriteLine(e.StackTrace); return EXIT_FAILURE;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message + ".");
                Console.WriteLine("\n========== STACK TRACE ==========\n");
                Console.WriteLine(e.StackTrace); return EXIT_FAILURE;
            }
        }

        /// <summary>
        /// You can use enums as variables, and you can even tag them
        /// with DescriptionAttributes in which case the wrapper will
        /// use more informative strings to display them graphically.
        /// </summary>
        public enum Foobar
        {
            [Description("A biped model")] // << will show this instead of "Human" in the bar
            Human = 2,
            [Description("An animal model")]
            Cat = 5,
            [Description("An inanimate model")]
            Table = 99
        }
    }
}
