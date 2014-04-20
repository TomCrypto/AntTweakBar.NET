AntTweakBar.NET
===============

AntTweakBar.NET is a C# wrapper for the [AntTweakBar](http://anttweakbar.sourceforge.net) GUI library.

Getting Started
---------------

A solution file is provided which can be used to compile the managed library wrapper, `AntTweakBar.NET.dll`, just add it as a reference to your project and you're good to go! See the tutorial provided to find out how to use it (it's really short) and refer to the FAQ below if you encounter a problem or have a specific question. If the FAQ does not help, feel free to post an issue and I'll take a look at it.

Status
------

The wrapper is functional, but not every feature AntTweakBar provides is implemented. Each feature needs to be tested and integrated into the code, which is why I've tried to start with a minimal set of often-used features, leaving the less popular ones to be added over time. The wrapper is also not a direct mapping from the native C API to C#, as you will soon notice if you are familiar with the former.

**Todo**:

- add more predefined event handlers for all of the events supported by AntTweakBar (SDL and so on)
- more/better unit tests
- test multi-window support
- check it works on OSX
- ...

FAQ
---

**Q**. *I've added the library in my references but when I run my program I get a strange exception at startup.*

**A**. The AntTweakBar library (the native one, not the managed wrapper DLL) cannot be found. Are you sure it is somewhere your program can find it, and is it built for the right architecture (32-bit or 64-bit)? The exception is somewhat convoluted because the wrapper does some setup work in a static constructor (the inner exception is the relevant one).

**Q**. *I'm able to create bars and variables but as soon as I call the `Draw` method on my context I get a "Bad Size" exception.*

**A**. This means AntTweakBar doesn't know the size of your window, and can't draw itself. You need to set up event handling, and you might also have to call the `HandleResize` method depending on your window manager.

**Q**. *My bars are correctly drawn, but I can't interact with them or anything.*

**A**. You haven't set up event handling. Make sure you are sending your window events to AntTweakBar by using one of the predefined event handlers (e.g. `EventHandlerSFML` for SFML) or do it manually by hooking up your events to call `HandleKeyPress`, `HandleMouseMove`, and so on.

**Q**. *The AntTweakBar API lets you give identifiers to variables to refer to them later on. Is there an equivalent?*

**A**. Yes, the variable instance itself. If you need to keep it around, do so. In many cases, though, you just need to set the variable up once at startup and then bind it to some concrete variable in your program via its `Changed` event, and can forget about it afterwards.

**Q**. *The AntTweakBar API has a handy mechanism to create/configure a variable from a definition string (e.g. `label=foo visible=false`). Can I do this with AntTweakBar.NET?*

**A**. Partly, yes. You cannot implicitly create variables this way, but you can provide a definition string to the constructor or to the `SetDefinition` method in order to configure the variable. Remember that variables have no unique "identifier" in AntTweakBar.NET, it is more natural in C# to consider the variable instance itself as an identifier. I'm sure it will become clear once you use the wrapper for a bit.

**Q**. *So I can't create a color variable because it wants my color type to implement the ColorType interface. It doesn't, and I can't change that (also applies to vectors and quaternions).*

**A**. This is a problem with generics which I have given a lot of thought to, and ultimately decided that the best way to solve this problem was for you to write a transitional type which implements ColorType and internally holds an instance of your own color type (by composition). If you then write implicit conversion operators to and from your type and this transitional type, you can use your own type with the wrapper almost seamlessly. If your type uses doubles, just cast away as precision is irrelevant here.

**Q**. *Why is there only one integer variable type? AntTweakBar supports 8-, 16-, and 32-bit signed and unsigned variable types.*

**A**. The integer variable type that's there already supports minimum and maximum ranges, and its value can be easily converted to and from the other types. Each additional variable type is one that must be tested and maintained, so it's not worth the effort (except maybe unsigned 32-bit integers, which might be added eventually).

**Q**. *How expensive is it to read the `Value` property of a variable? Can I safely read it in tight loops or do I want to copy it to a local variable before working with it?*

**A**. Go ahead and read it directly if you need to. The value is actually stored on the C# side and is modified by the native AntTweakBar library through callbacks, so reading it is about as cheap as it gets. In short, there is no interop cost associated with reading back the value of a variable (this is **not** true of the other variable or bar properties, which all involve a native call to the AntTweakBar library, so bear that in mind).

Compatibility
-------------

AntTweakBar.NET runs on the Microsoft .NET framework and on the Mono runtime, both 32-bit and 64-bit (provided it can find the appropriate AntTweakBar DLL or shared library, of course). It has been tested on Windows and Linux, and is expected to work - but has not been tested - on Mac OSX and presumably BSD.

Contribute
----------

Any issues or pull requests are welcome, I especially need help with verifying and streamlining multi-window support, thread safety, Mac OSX testing, and adding more stuff to the FAQ, but any contribution is greatly appreciated.
