using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using AOT;
using UnityEngine;

// https://blog.csdn.net/u014661152/article/details/113737625

/// <summary>
///     强制设置Unity游戏窗口的长宽比。你可以调整窗口的大小，他会强制保持一定比例
///     通过拦截窗口大小调整事件(WindowProc回调)并相应地修改它们来实现的
///     也可以用像素为窗口设置最小/最大宽度和高度
///     长宽比和最小/最大分辨率都与窗口区域有关，标题栏和边框不包括在内
///     该脚本还将在应用程序处于全屏状态时强制设置长宽比。当你切换到全屏，
///     应用程序将自动设置为当前显示器上可能的最大分辨率，而仍然保持固定比。如果显示器没有相同的宽高比，则会在左/右或上/下添加黑条
///     确保你在PlayerSetting中设置了“Resizable Window”，否则无法调整大小
///     如果取消不支持的长宽比在PlayerSetting中设置“Supported Aspect Rations”
///     注意:因为使用了WinAPI，所以只能在Windows上工作。在Windows 10上测试过
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "InvalidXmlDocComment")]
public static class AspectRatioController
{
    private const int taskBarHeight = 72;

    // 长宽比的宽度和高度
    private static float aspectRatioWidth = 9;
    private static float aspectRatioHeight = 16;

    // 当前锁定长宽比。
    private static float aspect;

    /// <summary>
    ///     每当窗口分辨率改变或用户切换全屏时，都会触发此事件
    ///     参数是新的宽度、高度和全屏状态(true表示全屏)
    /// </summary>
    private static Action<int, int, bool> resolutionChangedEvent;

    // 最小值和最大值的窗口宽度/高度像素
    private static readonly int minWidthPixel = 512;
    private static readonly int minHeightPixel = 512;
    private static readonly int maxWidthPixel = 2048;
    private static readonly int maxHeightPixel = 2048;

    // 窗口的宽度和高度。不包括边框和窗口标题栏
    // 当调整窗口大小时，就会设置这些值
    private static int setWidth = -1;
    private static int setHeight = -1;
    private static bool wasFullscreenLastFrame;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void OnBeforeSplashScreenRuntimeMethod()
    {
        DisableMaximizeButton();
        resolutionChangedEvent += (_, _, _) => DisableMaximizeButton();
        setHeight = GetCurrentResolution().height - taskBarHeight;
        setWidth = Mathf.RoundToInt(setHeight * aspect);
        Screen.SetResolution(setWidth, setHeight, Screen.fullScreen);
        //注册回调，然后应用程序想要退出
        Application.wantsToQuit += ApplicationWantsToQuit;

        // 找到主Unity窗口的窗口句柄
        EnumThreadWindows(GetCurrentThreadId(), LpEnumFunc, IntPtr.Zero);

        // 将长宽比应用于当前分辨率
        SetAspectRatio(aspectRatioWidth, aspectRatioHeight, true);

        // 保存当前的全屏状态
        wasFullscreenLastFrame = Screen.fullScreen;

        // Register (replace) WindowProc callback。每当一个窗口事件被触发时，这个函数都会被调用
        //例如调整大小或移动窗口
        //保存旧的WindowProc回调函数，因为必须从新回调函数中调用它
        wndProcDelegate = WndProc;
        _newWndProcPtr = Marshal.GetFunctionPointerForDelegate(wndProcDelegate);
        _oldWndProcPtr = SetWindowLong(_unityHWnd, GWLP_WNDPROC, _newWndProcPtr);
        GetWindowRect(_unityHWnd, out var windowRect);

        setWidth = windowRect.Right - windowRect.Left;
        setHeight = windowRect.Bottom - windowRect.Top;
    }

    [MonoPInvokeCallback(typeof(EnumWindowsProc))]
    private static bool LpEnumFunc(IntPtr hWnd, IntPtr lparam)
    {
        var classText = new StringBuilder(UnityWndClassname.Length + 1);
        GetClassName(hWnd, classText, classText.Capacity);

        if (classText.ToString() == UnityWndClassname)
        {
            _unityHWnd = hWnd;
            return false;
        }

        return true;
    }

    private static void OnChangeFullScene()
    {
        var (pixelWidthOfCurrentScreen, pixelHeightOfCurrentScreen) = GetCurrentResolution();
        //切换到全屏检测,设置为最大屏幕分辨率，同时保持长宽比
        int height;
        int width;

        //根据当前长宽比和显示器的比例进行比较，上下或左右添加黑边
        var blackBarsLeftRight = aspect < (float)pixelWidthOfCurrentScreen / pixelHeightOfCurrentScreen;
        if (blackBarsLeftRight)
        {
            height = pixelHeightOfCurrentScreen;
            width = Mathf.RoundToInt(pixelHeightOfCurrentScreen * aspect);
        }
        else
        {
            width = pixelWidthOfCurrentScreen;
            height = Mathf.RoundToInt(pixelWidthOfCurrentScreen / aspect);
        }

        Screen.SetResolution(width, height, Screen.fullScreen);
        resolutionChangedEvent?.Invoke(width, height, Screen.fullScreen);
    }

    private static void OnChangeWindows()
    {
        setHeight = setHeight != -1 ? Math.Min(setHeight, GetCurrentResolution().height - taskBarHeight) : Screen.height;
        setWidth = Mathf.RoundToInt(setHeight * aspect);
        Screen.SetResolution(setWidth, setHeight, Screen.fullScreen);
        resolutionChangedEvent?.Invoke(setWidth, setHeight, Screen.fullScreen);
    }

    private static void DisableMaximizeButton()
    {
        var handle = GetActiveWindow();
        var windowLong = GetWindowLong(handle, GWL_STYLE);
        windowLong &= ~WS_MAXIMIZEBOX;
        SetWindowLong(handle, GWL_STYLE, new IntPtr(windowLong));
    }

    /// <summary>
    ///     将目标长宽比设置为给定的长宽比。
    /// </summary>
    /// <param name="newAspectWidth">宽高比的新宽度</param>
    /// <param name="newAspectHeight">纵横比的新高度</param>
    /// <param name="apply">true，当前窗口分辨率将立即调整以匹配新的纵横比 false，则只在下次手动调整窗口大小时执行此操作</param>
    private static void SetAspectRatio(float newAspectWidth, float newAspectHeight, bool apply)
    {
        //计算新的纵横比
        aspectRatioWidth = newAspectWidth;
        aspectRatioHeight = newAspectHeight;
        aspect = aspectRatioWidth / aspectRatioHeight;

        // 调整分辨率以匹配长宽比(触发WindowProc回调)
        if (apply)
        {
            Screen.SetResolution(Screen.width, Mathf.RoundToInt(Screen.width / aspect), Screen.fullScreen);
        }
    }

    /// <summary>
    ///     WindowProc回调。应用程序定义的函数，用来处理发送到窗口的消息
    /// </summary>
    /// <param name="msg">用于标识事件的消息</param>
    /// <param name="wParam">额外的信息信息。该参数的内容取决于uMsg参数的值 </param>
    /// <param name="lParam">其他消息的信息。该参数的内容取决于uMsg参数的值 </param>
    /// <returns></returns>
    [MonoPInvokeCallback(typeof(WndProcDelegate))]
    private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        // 检查消息类型
        // resize事件
        if (msg == WM_SIZING)
        {
            // 获取窗口大小结构体
            var rc = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));

            GetWindowRect(_unityHWnd, out var windowRect);

            GetClientRect(_unityHWnd, out var clientRect);

            var borderWidth = windowRect.Right - windowRect.Left - (clientRect.Right - clientRect.Left);
            var borderHeight = windowRect.Bottom - windowRect.Top - (clientRect.Bottom - clientRect.Top);

            // 在应用宽高比之前删除边框(包括窗口标题栏)
            rc.Right -= borderWidth;
            rc.Bottom -= borderHeight;

            // 限制窗口大小
            var newWidth = Mathf.Clamp(rc.Right - rc.Left, minWidthPixel, maxWidthPixel);
            var newHeight = Mathf.Clamp(rc.Bottom - rc.Top, minHeightPixel, maxHeightPixel);

            // 根据纵横比和方向调整大小
            switch (wParam.ToInt32())
            {
                case WMSZ_LEFT:
                    rc.Left = rc.Right - newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_RIGHT:
                    rc.Right = rc.Left + newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_TOP:
                    rc.Top = rc.Bottom - newHeight;
                    rc.Right = rc.Left + Mathf.RoundToInt(newHeight * aspect);
                    break;
                case WMSZ_BOTTOM:
                    rc.Bottom = rc.Top + newHeight;
                    rc.Right = rc.Left + Mathf.RoundToInt(newHeight * aspect);
                    break;
                case WMSZ_RIGHT + WMSZ_BOTTOM:
                    rc.Right = rc.Left + newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_RIGHT + WMSZ_TOP:
                    rc.Right = rc.Left + newWidth;
                    rc.Top = rc.Bottom - Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_LEFT + WMSZ_BOTTOM:
                    rc.Left = rc.Right - newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_LEFT + WMSZ_TOP:
                    rc.Left = rc.Right - newWidth;
                    rc.Top = rc.Bottom - Mathf.RoundToInt(newWidth / aspect);
                    break;
            }

            // 保存实际分辨率,不包括边界
            setWidth = rc.Right - rc.Left;
            setHeight = rc.Bottom - rc.Top;

            // 添加边界
            rc.Right += borderWidth;
            rc.Bottom += borderHeight;

            // 触发分辨率更改事件
            resolutionChangedEvent?.Invoke(setWidth, setHeight, Screen.fullScreen);

            // 回写更改的窗口参数
            Marshal.StructureToPtr(rc, lParam, true);
        }

        if (msg == WM_WINDOWPOSCHANGED)
        {
            if (Screen.fullScreen && !wasFullscreenLastFrame)
            {
                OnChangeFullScene();
            }
            else if (!Screen.fullScreen && wasFullscreenLastFrame)
            {
                OnChangeWindows();
            }

            resolutionChangedEvent?.Invoke(setWidth, setHeight, Screen.fullScreen);
            wasFullscreenLastFrame = Screen.fullScreen;
        }

        return CallWindowProc(_oldWndProcPtr, hWnd, msg, wParam, lParam);
    }

    private static (int width, int height) GetCurrentResolution()
    {
        var foregroundWindow = GetForegroundWindow();
        var monitor = MonitorFromWindow(foregroundWindow, MonitorOptions.MONITOR_DEFAULTTONEAREST);

        var monitorInfo = new MONITORINFOEX
        {
            cbSize = MONITORINFOEX_SIZE
        };
        GetMonitorInfo(monitor, ref monitorInfo);

        var screenWidth = monitorInfo.rcMonitor.Right - monitorInfo.rcMonitor.Left;
        var screenHeight = monitorInfo.rcMonitor.Bottom - monitorInfo.rcMonitor.Top;
        return (screenWidth, screenHeight);
    }

    /// <summary>
    ///     调用SetWindowLong32或SetWindowLongPtr64，取决于可执行文件是32位还是64位。
    ///     这样，我们就可以同时构建32位和64位的可执行文件而不会遇到问题。
    /// </summary>
    /// <param name="hWnd">The window handle.</param>
    /// <param name="nIndex">要设置的值的从零开始的偏移量</param>
    /// <param name="dwNewLong">The replacement value.</param>
    /// <returns>返回值是指定偏移量的前一个值。否则零.</returns>
    private static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong) => IntPtr.Size == 4 ? SetWindowLong32(hWnd, nIndex, dwNewLong) : SetWindowLongPtr64(hWnd, nIndex, dwNewLong);

    /// <summary>
    ///     退出时调用。 返回false将中止并使应用程序保持活动。True会让它退出。
    /// </summary>
    /// <returns></returns>
    private static bool ApplicationWantsToQuit()
    {
        SetWindowLong(_unityHWnd, GWLP_WNDPROC, _oldWndProcPtr);
        return true;
    }

    // WinAPI相关定义

    #region WINAPI

    private const string UnityWndClassname = "UnityWndClass";

    // Unity窗口的窗口句柄
    private static IntPtr _unityHWnd;

    // 指向旧WindowProc回调函数的指针
    private static IntPtr _oldWndProcPtr;

    // 指向我们自己的窗口回调函数的指针
    private static IntPtr _newWndProcPtr;
    private const int WS_MAXIMIZEBOX = 0x00010000;
    private const int GWL_STYLE = -16;
    private const int MONITORINFOEX_SIZE = 40;

    // 当窗口调整时,WM_SIZING消息通过WindowProc回调发送到窗口
    private const int WM_SIZING = 0x214;

    // WM大小调整消息的参数
    private const int WMSZ_LEFT = 1;
    private const int WMSZ_RIGHT = 2;
    private const int WMSZ_TOP = 3;
    private const int WMSZ_BOTTOM = 6;

    // 获取指向WindowProc函数的指针
    private const int GWLP_WNDPROC = -4;
    private const int WM_WINDOWPOSCHANGED = 0x0047;

    // 委托设置为新的WindowProc回调函数

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    private static WndProcDelegate wndProcDelegate;

    // 检索调用线程的线程标识符

    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();

    // 检索指定窗口所属类的名称

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    // 通过将句柄传递给每个窗口，依次传递给应用程序定义的回调函数，枚举与线程关联的所有非子窗口

    [DllImport("user32.dll")] private static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);


    // 将消息信息传递给指定的窗口过程

    //用于查找窗口句柄的Unity窗口类的名称

    [DllImport("user32.dll")] private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    // 检索指定窗口的边框的尺寸

    // 尺寸是在屏幕坐标中给出的，它是相对于屏幕左上角的

    [DllImport("user32.dll", SetLastError = true)] private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

    //检索窗口客户区域的坐标。客户端坐标指定左上角

    //以及客户区的右下角。因为客户机坐标是相对于左上角的

    //在窗口的客户区域的角落，左上角的坐标是(0,0)

    [DllImport("user32.dll")] private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    // 更改指定窗口的属性。该函数还将指定偏移量的32位(长)值设置到额外的窗口内存中

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)] private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    //更改指定窗口的属性。该函数还在额外的窗口内存中指定的偏移量处设置一个值

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)] private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")] private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")] private static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorOptions dwFlags);

    [DllImport("user32.dll")] private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

    [Flags]
    private enum MonitorOptions : uint
    {
        MONITOR_DEFAULTTONULL = 0x00000000,
        MONITOR_DEFAULTTOPRIMARY = 0x00000001,
        MONITOR_DEFAULTTONEAREST = 0x00000002
    }

    /// <summary>
    ///     WinAPI矩形定义。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MONITORINFOEX
    {
        public int cbSize;
        public readonly RECT rcMonitor;
        private readonly RECT rcWork;
        private readonly uint dwFlags;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] private readonly char[] szDevice;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    private struct WindowPlacement
    {
        public int length;
        public int flags;
        public int showCmd;
        public Point ptMaxPosition;
        public Point ptMinPosition;
        public Rectangle rcNormalPosition;
    }

    #endregion
}