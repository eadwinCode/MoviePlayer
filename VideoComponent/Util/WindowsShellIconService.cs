using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoComponent.Util
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SHFileInfo
    {
        public IntPtr hIcon;

        public int iIcon;

        public uint dwAttributes;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    public static class FileManager
    {
        public static ImageSource GetImageSource(string filename)
        {
            try
            {
                return FileManager.GetImageSource(filename, new Size(32, 32));
            }
            catch
            {
                throw;
            }
        }

        public static ImageSource GetImageSource(string filename, Size size)
        {
            try
            {
                using (var icon = ShellObject.FromParsingName(filename).Thumbnail.LargeIcon)
                {
                    return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight((int)size.Width, (int)size.Height));
                }
            }
            catch
            {
                throw;
            }
        }
    }
    public enum ItemType : short
    {
        Folder,
        File
    }
    public enum IconSize : short
    {
        Small,
        Large
    }

    public enum ItemState : short
    {
        Undefined,
        Open,
        Close
    }

    public static partial class WindowsShellIconService
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFileInfo psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        public const uint SHGFI_ICON = 0x000000100;
        public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        public const uint SHGFI_OPENICON = 0x000000002;
        public const uint SHGFI_SMALLICON = 0x000000001;
        public const uint SHGFI_LARGEICON = 0x000000000;
        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        public const uint FILE_ATTRIBUTE_FILE = 0x00000100;
    }

}
