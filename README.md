# Server

*Local Web Server inside a cross‑platform .NET MAUI app (Mac Catalyst + Android + iOS)*

> **Techs**: .NET 8 LTS • MAUI • EmbedIO 4 • AOT ✔ • Sandboxed networking ✔

---

## ✨ What it does

| Feature                 | Details                                                                     |
| ----------------------- | --------------------------------------------------------------------------- |
| **EmbedIO server**      | Starts automatically at **`http://127.0.0.1:9696/`** on every platform.     |
| **Static file module**  | Serves files from the user‑writable *Static* folder (created on first run). |
| **Multi‑TFM**           | Works on **Mac Catalyst**, **Android** and **iOS** from a single code base. |
| **AOT‑ready**           | Full LLVM AOT on iOS/Catalyst, optional on Android, JIT on Windows/macOS.   |
| **App‑Store compliant** | Correct entitlements/permissions so Apple & Google are happy.               |

---

## 📦 Prerequisites

| Tool            | Minimum version                        | Notes                                      |
| --------------- | -------------------------------------- | ------------------------------------------ |
| **macOS**       | 13 Ventura (ARM or x64)                | macOS host for all builds                  |
| **Xcode**       | 15.2                                   | iOS & Catalyst tool‑chain / codesign       |
| **Android SDK** | API 34 (via VS for Mac / command‑line) | Needed only to run the Android target      |
| **.NET SDK**    | **8.0.401** (pinned in `global.json`)  | Same feature‑band as every workload        |
| **Workloads**   | `maccatalyst`, `android`, `ios`        | Installed with `dotnet workload install …` |

---

## 🚀 Quick start

> First time only
>
> ```bash
> # Install workloads once
> dotnet workload install maccatalyst android ios
> ```
>
> The following commands assume you are in the repo root.

### Run on Mac Catalyst

```bash
# Debug (JIT, faster hot‑reload)
dotnet build -t:Run -f net8.0-maccatalyst

# Release (AOT, fully signed bundle)
dotnet publish -c Release -f net8.0-maccatalyst /p:PublishAot=true
```

### Run on Android (emulator or device)

```bash
# Make sure ANDROID_HOME is set or use VS for Mac emulator manager

dotnet build -t:Run -f net8.0-android
```

### Run on iOS (simulator or device)

```bash
# Simulator (ARM64 Mac or x64 depending on sim)
dotnet build -t:Run -f net8.0-ios

# Device (requires provisioning profile)
dotnet publish -c Release -f net8.0-ios /p:ArchiveOnBuild=true
```

A browser/tab should open on **[http://127.0.0.1:9696/](http://127.0.0.1:9696/)** and show *“EmbedIO”*.

---

## 🔐 Permissions & entitlements

| Platform         | File                                       | Must contain                                                                                                          |
| ---------------- | ------------------------------------------ | --------------------------------------------------------------------------------------------------------------------- |
| **Mac Catalyst** | `Platforms/MacCatalyst/Entitlements.plist` | `com.apple.security.network.server` + `com.apple.security.network.client`                                             |
|                  | `Platforms/MacCatalyst/Info.plist`         | `NSLocalNetworkUsageDescription` string                                                                               |
| **iOS**          | `Platforms/iOS/Info.plist`                 | Same `NSLocalNetworkUsageDescription`                                                                                 |
| **Android**      | `Platforms/Android/AndroidManifest.xml`    | `<uses-permission android:name="android.permission.INTERNET"/>` + `<application android:usesCleartextTraffic="true">` |

**Important:** keep the listener bound to `127.0.0.1` only. Publishing an externally reachable daemon will fail App Store review.

---

## 🛠 Configuration points

| File                  | Change this if…                                                        |
| --------------------- | ---------------------------------------------------------------------- |
| `MauiProgram.cs`      | Different port, switch to `HttpListenerMode.Kestrel`, etc.             |
| `Entitlements.plist`  | Need Bonjour / multicast → add `network.multicast`.                    |
| `Info.plist`          | Customise the local‑network privacy text.                              |
| `AndroidManifest.xml` | Want HTTPS → add a network security config that allows localhost cert. |

---

## 🌍 Target frameworks

```xml
<TargetFrameworks>
  net8.0-maccatalyst;net8.0-android;net8.0-ios
</TargetFrameworks>
```

* **Mac Catalyst** is the desktop build shipped to macOS App Store.
* **Android** runs on API 34 by default. Keep Debug without AOT for faster iterations; enable AOT only in Release if the startup time is critical.
* **iOS** is always AOT. Simulator builds skip code‑signature steps.

---

## 🐞 Known quirks

* iOS/Catalyst require the Local Network prompt on first packet; until the user accepts, the socket may not bind.
* IPv6 `::1` can break binding on iOS 17 / macOS 14. Use `127.0.0.1` everywhere.
* When trimming is enabled, add `[DynamicDependency]` attributes to controllers you load via reflection.

---

## 📚 Further reading

* [.NET workloads & feature‑bands](https://learn.microsoft.com/dotnet/core/tools/dotnet-workload)
* [.NET for Android & iOS docs](https://learn.microsoft.com/dotnet/maui/ios-mac/overview)
* [EmbedIO documentation](https://unosquare.github.io/embedio/)
* [Apple Local Network privacy](https://developer.apple.com/forums/thread/666611)

---

Happy multi‑platform hacking! 🚀
