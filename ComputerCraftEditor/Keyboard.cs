namespace WindowsFormsApplication1
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class Keyboard
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);
        public const int INPUT_KEYBOARD = 1;

        [DllImport("user32.dll")]
        public static extern IntPtr GetMessageExtraInfo();


        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern short VkKeyScan(char ch);


        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);


        const int KEYEVENTF_KEYUP = 0x2;
        const int KEYEVENTF_KEYDOWN = 0x0;


        public static Keys ConvertCharToVirtualKey(char ch)
        {
            short vkey = VkKeyScan(ch);
            Keys retval = (Keys)(vkey & 0xff);
            int modifiers = vkey >> 8;
            if ((modifiers & 1) != 0) retval |= Keys.Shift;
            if ((modifiers & 2) != 0) retval |= Keys.Control;
            if ((modifiers & 4) != 0) retval |= Keys.Alt;
            return retval;
        }


        /// <summary>
        /// Simulates a keyboard key down. Note that the key will remain pressed until you call Release method.
        /// </summary>
        /// <param name="button">the specific key which you want to simulate.</param>
        public static void Hold(System.Windows.Forms.Keys button)
        {
            keybd_event((byte)button, 0, KEYEVENTF_KEYDOWN, 0);
        }
        /// <summary>
        /// Simulates a keyboard key down. Note that the key will remain pressed until you call Release method.
        /// </summary>
        /// <param name="button">the byte value of specific key which you want to simulate.</param>
        public static void Hold(byte button)
        {
            keybd_event(button, 0, KEYEVENTF_KEYDOWN, 0);
        }


        /// <summary>
        /// Simulates a keyboard key up. Note that you need to press the key using Hold method, otherwise, this method has no effect.
        /// </summary>
        /// <param name="button">the key you have simulated in Hold method.</param>
        public static void Release(System.Windows.Forms.Keys button)
        {
            keybd_event((byte)button, 0, KEYEVENTF_KEYUP, 0);
        }
        /// <summary>
        /// Simulates a keyboard key up. Note that you need to press the key using Hold method, otherwise, this method has no effect.
        /// </summary>
        /// <param name="button">the byte value of the key you have simulated in Hold method.</param>
        public static void Release(byte button)
        {
            keybd_event(button, 0, KEYEVENTF_KEYUP, 0);
        }


        /// <summary>
        /// Simulates a keyboard key press. Note that you do not need to release the key after calling this method because it will automatically performs this.
        /// </summary>
        /// <param name="button">the key you want to simulate.</param>
        public static void Simulate(System.Windows.Forms.Keys button)
        {
            if ((button & Keys.Shift) != Keys.None) keybd_event(KEYBDEVENTF_SHIFTVIRTUAL, KEYBDEVENTF_SHIFTSCANCODE, KEYBDEVENTF_KEYDOWN, 0);
            Hold(button);
            Release(button);
            if ((button & Keys.Shift) != Keys.None) keybd_event(KEYBDEVENTF_SHIFTVIRTUAL, KEYBDEVENTF_SHIFTSCANCODE, KEYBDEVENTF_KEYUP, 0);
        }
        /// <summary>
        /// Simulates a keyboard key press. Note that you do not need to release the key after calling this method because it will automatically performs this.
        /// </summary>
        /// <param name="button">the byte value of the key you want to simulate.</param>
        public static void Simulate(byte button)
        {
            Hold(button);
            Release(button);
        }
        public const int MOUSEEVENTF_LEFTDOWN = 0x2;
        public const int MOUSEEVENTF_LEFTUP = 0x4;
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        public const int MOUSEEVENTF_MIDDLEUP = 0x40;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const byte KEYBDEVENTF_SHIFTVIRTUAL = 0x10;
        public const byte KEYBDEVENTF_SHIFTSCANCODE = 0x2A;
        public const int KEYBDEVENTF_KEYDOWN = 0;
        public const int KEYBDEVENTF_KEYUP = 2;
        public static void SimulateChar(char ch)
        {
            var keys = ConvertCharToVirtualKey(ch);
            Simulate(keys);
        }


    }
}