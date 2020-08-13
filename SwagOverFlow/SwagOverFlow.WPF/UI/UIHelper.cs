using SwagOverFlow.WPF.Controls;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SwagOverFlow.WPF.UI
{
    public static class UIHelper
    {
        static PackIconKindOrCustomToDataTemplateConverter _iconCnvtr = new PackIconKindOrCustomToDataTemplateConverter();

        public static ImageSource GetImageSource(Enum iconEnum, double scale = 1.0, double width = 0.0, double height = 0.0, double margin = 0.0, System.Windows.Media.Brush brush = null)
        {
            brush = brush ?? System.Windows.Media.Brushes.White;
            
            ImageSource imageSource = _iconCnvtr.CreateImageSource(iconEnum, brush);
            imageSource = GetBitMapSource(imageSource, scale, width, height, margin);
            return imageSource;
        }

        public static ICollectionView GetCollectionView(IEnumerable col)
        {
            return CollectionViewSource.GetDefaultView(col);
        }

        public static string StringInputDialog(string message = "Please enter input:", String strDefault = "")
        {
            #region Window Setup

            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            FrameworkElement mainChild = mainWindow.Content as FrameworkElement;

            SwagWindow window = new SwagWindow();
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;

            #endregion Window Setup

            #region Content Setup

            DockPanel dockPanel = new DockPanel();

            TextBlock tbMessage = new TextBlock();
            tbMessage.Margin = new Thickness(5, 5, 5, 5);
            tbMessage.Text = message;
            DockPanel.SetDock(tbMessage, Dock.Top);

            TextBox txtInput = new TextBox();
            txtInput.Text = strDefault;
            txtInput.SelectAll();
            txtInput.Margin = new Thickness(5, 0, 5, 5);

            Grid gridButtons = new Grid();
            DockPanel.SetDock(gridButtons, Dock.Bottom);
            gridButtons.ColumnDefinitions.Add(new ColumnDefinition());
            gridButtons.ColumnDefinitions.Add(new ColumnDefinition());
            Button btnOK = new Button() { Content = "OK" };
            btnOK.Margin = new Thickness(5, 0, 5, 5);
            btnOK.Click += (s, e) =>
            {
                window.Close();
            };
            Grid.SetColumn(btnOK, 0);
            gridButtons.Children.Add(btnOK);
            Button btnCancel = new Button() { Content = "Cancel" };
            btnCancel.Margin = new Thickness(5, 0, 5, 5);
            btnCancel.Click += (s, e) =>
            {
                txtInput.Text = "";
                window.Close();
            };
            Grid.SetColumn(btnCancel, 1);
            gridButtons.Children.Add(btnCancel);

            txtInput.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Return)
                {
                    btnOK.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
                else if (e.Key == Key.Escape)
                {
                    btnCancel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            };

            dockPanel.Children.Add(tbMessage);
            dockPanel.Children.Add(gridButtons);
            dockPanel.Children.Add(txtInput);

            window.Content = dockPanel;

            #endregion Content Setup

            #region Show Dialog

            window.Left = mainWindow.Left + (mainWindow.Width - window.ActualWidth) / 2;
            window.Top = mainWindow.Top + (mainWindow.Height - window.ActualHeight) / 2;
            txtInput.Focus();
            window.ShowDialog();

            #endregion Show Dialog

            return txtInput.Text;
        }

        public static void CreateIcon(Enum iconEnum, double scale = 1.0, double width = 0.0, double height = 0.0, double margin = 0.0, System.Windows.Media.Brush brush = null)
        {
            brush = brush ?? System.Windows.Media.Brushes.White;
            String fileName = $"{iconEnum.GetType().Name}.{iconEnum}";
            CreateIcon(GetImageSource(iconEnum, scale, width, height, margin, brush), fileName);
        }

        public static void CreateIcon(ImageSource source, string fileName)
        {
            BitmapFrame frame = BitmapFrame.Create(GetBitMapSource(source, 1, 48, 48));
            Bitmap bitmap = GetBitmap(frame);
            bitmap.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

            #region Create png file
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(frame);
            FileStream fsPng = new FileStream($"{fileName}.png", FileMode.OpenOrCreate);
            encoder.Save(fsPng);
            #endregion Create png file

            #region Convert png byte array to ico byte array
            MemoryStream msPng = new MemoryStream();
            fsPng.Position = 0;
            fsPng.CopyTo(msPng);
            Byte[] arrIcon = ConvertPngToIco(msPng.ToArray());
            MemoryStream msIcon = new MemoryStream(arrIcon);
            msPng.Close();
            fsPng.Close();
            #endregion Convert png byte array to ico byte array

            #region Create ico file
            System.Drawing.Icon icon = new Icon(msIcon);
            FileStream fs = new FileStream($"{fileName}.ico", FileMode.OpenOrCreate);
            icon.Save(fs);
            msIcon.Close();
            fs.Close();
            #endregion Create ico file
        }

        //https://stackoverflow.com/questions/15779564/resize-image-in-wpf
        public static BitmapSource GetBitMapSource(ImageSource source, Double scale = 1.0, Double width = 0.0, Double height = 0.0, Double margin = 0.0)
        {
            width = width == 0.0 ? source.Width * scale  : width;
            height = height == 0.0 ? source.Height * scale  : height;
            Rect rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);
            DrawingGroup group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            group.Children.Add(new ImageDrawing(source, rect));
            
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing(group);

            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                (int)width, (int)height,         // Resized dimensions
                96, 96,                // Default DPI values
                PixelFormats.Pbgra32); // Default pixel format
            bitmap.Render(drawingVisual);
            
            return BitmapFrame.Create(bitmap);
        }

        //https://stackoverflow.com/questions/2284353/is-there-a-good-way-to-convert-between-bitmapsource-and-bitmap
        public static Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(source.PixelWidth, source.PixelHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        //https://stackoverflow.com/questions/9714743/is-there-any-iconbitmapencoder-in-wpf
        public static byte[] ConvertPngToIco(byte[] data)
        {
            System.Drawing.Image source;
            using (var inStream = new MemoryStream(data))
            {
                source = System.Drawing.Image.FromStream(inStream);
            }
            byte[] output;
            using (var outStream = new MemoryStream())
            {
                // Header
                {
                    // Reserved
                    outStream.WriteByte(0);
                    outStream.WriteByte(0);
                    // File format (ico)
                    outStream.WriteByte(1);
                    outStream.WriteByte(0);
                    // Image count (1)
                    outStream.WriteByte(1);
                    outStream.WriteByte(0);
                }

                // Image entry
                {
                    // Width
                    outStream.WriteByte((byte)source.Width);
                    // Height
                    outStream.WriteByte((byte)source.Height);
                    // Number of colors (0 = No palette)
                    outStream.WriteByte(0);
                    // Reserved
                    outStream.WriteByte(0);
                    // Color plane (1)
                    outStream.WriteByte(1);
                    outStream.WriteByte(0);
                    // Bits per pixel
                    var bppAsLittle = IntToLittle2(System.Drawing.Image.GetPixelFormatSize(source.PixelFormat));
                    outStream.Write(bppAsLittle, 0, 2);
                    // Size of data in bytes
                    var byteCountAsLittle = IntToLittle4(data.Length);
                    outStream.Write(byteCountAsLittle, 0, 4);
                    // Offset of data from beginning of file (data begins right here = 22)
                    outStream.WriteByte(22);
                    outStream.WriteByte(0);
                    outStream.WriteByte(0);
                    outStream.WriteByte(0);
                    // Data
                    outStream.Write(data, 0, data.Length);
                }
                output = outStream.ToArray();
            }
            return output;
        }

        private static byte[] IntToLittle2(int input)
        {
            byte[] b = new byte[2];
            b[0] = (byte)input;
            b[1] = (byte)(((uint)input >> 8) & 0xFF);
            return b;
        }
        private static byte[] IntToLittle4(int input)
        {
            byte[] b = new byte[4];
            b[0] = (byte)input;
            b[1] = (byte)(((uint)input >> 8) & 0xFF);
            b[2] = (byte)(((uint)input >> 16) & 0xFF);
            b[3] = (byte)(((uint)input >> 24) & 0xFF);
            return b;
        }
    }
}
