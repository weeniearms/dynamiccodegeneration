using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace ImageProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            var bmp = Image.FromFile(@"C:\Users\Public\Pictures\Sample Pictures\Chrysanthemum.bmp") as Bitmap;
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            var tc = TypeDescriptor.GetConverter(typeof (Bitmap));
            var src = (byte[]) tc.ConvertTo(bmp, typeof (byte[]));
            
            //var src = File.ReadAllBytes(@"C:\Users\Public\Pictures\Sample Pictures\Chrysanthemum.bmp");
            var dst1 = new byte[src.Length];
            var dst2 = new byte[src.Length];
            var dst3 = new byte[src.Length];

            var filter = new ImageFilter(3, new double[] {1, 1, 1, 1, 1, 1, 1, 1, 1});
            var fixedFilter = new FixedImageFilter(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var dynamicFilter = new DynamicImageFilter(3, new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 });

            var stopWatch = new Stopwatch();
            
            stopWatch.Reset();
            stopWatch.Start();
            filter.Filter(src, dst1, bmpData.Stride, 4);
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);
            File.WriteAllBytes("dst1.dat", dst1);

            stopWatch.Reset();
            stopWatch.Start();
            fixedFilter.Filter(src, dst2, bmpData.Stride, 4);
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);
            File.WriteAllBytes("dst2.dat", dst2);

            stopWatch.Reset();
            stopWatch.Start();
            dynamicFilter.Filter(src, dst3, bmpData.Stride, 4);
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);
            File.WriteAllBytes("dst3.dat", dst3);
        }
    }
}
