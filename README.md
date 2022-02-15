# Win11ClockToggler

One of the many bummers that **Windows 11** brings to the table is **the inability to hide the clock** in the taskbar. This is really an annoyance if you create screen recordings or tutorials and don't want the clock to be shown in your final video.

After waiting for months for them to fix this issue, I decided to write this small and fast little command line tool: **Win11ClockToggler**.

![The app icon](Win11ClockToggler.png)

Win11ClockToggler searches and hides the full notification area, or just the clock, in the Windows 11 taskbar.

- Just **run it once** and the full notification area will **disappear** (this is the default option, which I find more useful)

- **Run it again to restore** everything to its normal state.

In my opinion it's better to hide the full notification area by default, since it will hide all the distracting icons, which is good if you're recording your screen for a tutorial.

Anyway, **if you just want to hide the clock** and not the full notification area,  you can use the `-c`, `/c` or **`--clock` switch**. 

> **IMPORTANT**: In this case it will hide both **the clock and the system icons** in Windows 11. This due to how the new Windows 11 taskbar works internally, exposing to the Win32 API the whole system icon are (as a `Windows.UI.Composition.DesktopWindowContentBridge`, with an underlying XAML UI for that). if you know how to overcome this, please, fork this repo and make a Pull Request that I'll gladly merge :blush:

The full list of arguments you can use are:

- **`-h`, `/h`, `--help`**: show help
- **`-n`, `/n`, `--notificationArea`**: hides the full notification area, not just the clock. This is the default option, so you don't need to really use it unless you want to be explicit in a script or something
- **`-c`, `/c`, `--clock`**: hides just the clock (and system icons)
- **`-b`, `/b`, `--batch`**: doesn't wait for a key to be pressed after running. Useful to include the tool in a script file.

> **IMPORTANT**: It works for **Windows 10** too, but it will always hide the full notification area.
> If you just want to hide the clock in Windows 10, you already have native support for it
> in the notifications' configuration of the Windows Settings app.

If you find any kind of problem using it, maybe is because ne updates to Windows 11 that break compatibility. Please, open an issue here to tell me about it. Thanks.
