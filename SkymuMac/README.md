# Skymu:Mac

yes - zech felms

## Special compile fix, for Rider 2019?

Note: I am on OSX 10.9. This is likely an issue with this and probably few version later.

1. Look for XCODE/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSXVERSION.sdk/usr/lib/libcompression.dylib
* `XCODE` is the path to Xcode.app. Should be `/Applications/Xcode.app` for most.
2. If it does not exist, create a dummy like this (CHANGE THE PLATFORM VERSION)
* `cp XCODE/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX10.10.sdk/usr/lib/libsandbox.dylib XCODE/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX10.10.sdk/usr/lib/libcompression.dylib`
