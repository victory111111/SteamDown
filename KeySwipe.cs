using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public class TouchDetector
{
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    public const int VK_LBUTTON = 0x01;
    public const int VK_SHIFT = 0x10;
    public const int VK_TAB = 0x09;

    public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
    public const int KEYEVENTF_KEYUP = 0x0002;

    private static readonly int TopScreenTolerance = 10;
    private static readonly int DragThreshold = 100;
    private static bool isMouseDown = false;
    private static int startY = 0;

    public static void Main()
    {
        StartTouchDetection();
        Application.Run();
    }

    private static void StartTouchDetection()
    {
        Thread thread = new Thread(() =>
        {
            while (true)
            {
                if ((GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0)
                {
                    int y = Cursor.Position.Y;
                    if (y < TopScreenTolerance)
                    {
                        if (!isMouseDown)
                        {
                            isMouseDown = true;
                            startY = y;
                        }
                    }
                    else
                    {
                        if (isMouseDown && y - startY > DragThreshold) 
                        {
                            ExecuteKeyCombination();
                        }
                    }
                }
                else
                {
                    isMouseDown = false;
                }
            }
        });

        thread.IsBackground = true;
        thread.Start();
    }

    private static void ExecuteKeyCombination()
    {
        keybd_event(VK_SHIFT, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        keybd_event(VK_TAB, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        Thread.Sleep(100); 
        keybd_event(VK_TAB, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        keybd_event(VK_SHIFT, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
    }
}
