# MSUI - Mac cSharp UI

SwiftUI-like extension to Xamarin.Mac and .NET macOS workload.

## Examples

```cs
new SpacedStack(NSLayoutAttribute.Top,
                View,
                (38, new Image(new NSImage("hello"))
                    .NoTAMIC()
                    .AddTo(View)
                    .CenterX(View)
                    .Height(32)
                    .Hold(GHolder.H)
                ),
                (20, new Label("Sign in")
                    {
                        Alignment = NSTextAlignment.Center,
                        Font = NSFont.FromFontName("SegoeUI", 32),
                        TextColor = NSColor.White,
                    }
                    .NoTAMIC()
                    .AddTo(View)
                    .CenterX(View)
                    .Width(15, NSLayoutRelation.GreaterThanOrEqual)
                )
            );
```

## License

MIT.
