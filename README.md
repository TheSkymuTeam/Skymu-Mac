# Skymu:Mac

***To anybody looking for the original Avalonia Skymu:Mac, check out the `main` branch. This is `xamarin` branch.***

Note: The Avalonia Skymu:Mac just has a splash screen, and is not compatible with legacy OS versions.

Skymu:Mac is a OS X/macOS app attempting to replicate the looks and feel of Skype for macOS, while letting you connect to other serivces (Tox, Discord, Matrix, etc), unlike a Skype server replacement service.

It is compatible with OS X 10.9 or higher, all the way until macOS 26, and possibly also 27.

Currently, the Skype 7.14 theme is the only available theme.

## Compiling

### Skymu:Mac Core (Standard .NET with its AppKit binding)

WARN: This only supports newer macOS (currently 10.15).

CRIT: 

Make sure that [.NET](https://dotnet.microsoft.com/download) is installed.

For MacPorters, `dotnet-sdk-6` is the minimum.

It's as easy as:

```sh
dotnet workload restore SkymuMacCore.sln
dotnet build SkymuMacCore.sln
```

First command is responsible for installing the macOS workload.

Do NOT use the Makefile, besides `clear`. Other commands are for easing the development/testing with msbuild.

### Skymu:Mac Legacy (Xamarin.Mac)

Make sure that [Mono](https://mono-project.com/) and [Xamarin.Mac 6.2.0 or higher?](https://web.archive.org/web/20240720085849if_/https://download.visualstudio.microsoft.com/download/pr/54b422d1-7448-4c23-a8dd-f6db04641531/b83dc3119d4c49f3922ff286128b4874/xamarin.mac-6.2.0.42.pkg) is installed.

```sh
make all
```

Alternatively, use `make min` for Skymu:Mac only. Since there are no plugins, you are unable to actually use it.

`make core` can be used to build Skymu itself and Stub, giving bare minimum to play around with the UI.

`make restore` just runs NuGet restore. In case restore has to be forced, `make forcerestore` can be used.

If you want to quickly remove all the build files, use `make clean`.

`make PluginName`, like `make Tox` can be used to build a single plugin. This does not build Skymu:Mac itself.

You should run `make min` or `cp -r Plugins/bin SkymuMac/bin/CONFIG/Skymu.app/Contents/Plugins` after every plugin build, or set the plugin path in config to Plugins/bin if you do plugin development with Skymu:Mac.

#### Special compile fix, for Rider 2019?

Note: I am on OSX 10.9. This is likely an issue with this and probably few version later.

1. Look for XCODE/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSXVERSION.sdk/usr/lib/libcompression.dylib
* `XCODE` is the path to Xcode.app. Should be `/Applications/Xcode.app` for most.
2. If it does not exist, create a dummy like this (CHANGE THE SDK VERSION - MacOSX10.10 IS NOT PRESENT IN CERTAIN CASES)
* `cp XCODE/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX10.10.sdk/usr/lib/libsandbox.dylib XCODE/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX10.10.sdk/usr/lib/libcompression.dylib`

## Coding practices

All code must be compatible and compileable with OS X 10.9. Testing is not required - one of the devs can test it, although please try to spend some effort to make sure that the new code is 10.9 compatible (such as avoiding addArrangedSubview for NSStackView, using the Q helpers instead of accessing Xamarin.Mac APIs directly, especially for constraints, etc)

All files that depends on nint and/or nfloat must have the compatibility shim. We pin our project to C# 7.3 due to potential future legacy constraints, and C# 7.3 did not have nint nor nfloat outside of Xamarin.Mac.

If a dependency is added, that also must follow the above requirement.

Due to the above requirement, it is forbidden to push a XIB (Xcode Interface Builder) file. You must make all the user interface inside of the code. Prototyping with XIB tool, and translating it to code with a tool is allowed.

There are two coding style in the project. Please try to use the Skymu:Mac specific one (no `private` modifier, use `=>` whenever you can, etc). This is subject to change, as I'm not sure if Windows Skymu lead dev is happy with my choice or not.

We also try to mimick the original structure for Themes/VERSION/\*.cs files.
Like for example, we have a useless ConversationView that is just View \>
SplitView \> two views. There are some exclusions, like globally using
LoginWindow and MainWindow combo instead of WebLoginWindow and M5Window, and
potentially more.

### Misc dev info

You are likely unable to do any form of hot reload. Sorry!
