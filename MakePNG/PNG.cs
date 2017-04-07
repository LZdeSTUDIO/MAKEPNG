using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace MakePNG
{
    class PNG
    {
        public async Task<bool> getJPG(StorageFile sFile,UIElement Source)
        {
            try
            {
                CachedFileManager.DeferUpdates(sFile);
                //把控件变成图像  
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                //传入参数Image控件  
                await renderTargetBitmap.RenderAsync(Source);
                var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
                using (var fileStream = await sFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                    encoder.SetPixelData(
                   BitmapPixelFormat.Bgra8,
                   BitmapAlphaMode.Ignore,
                   (uint)renderTargetBitmap.PixelWidth,
                   (uint)renderTargetBitmap.PixelHeight,
                   DisplayInformation.GetForCurrentView().LogicalDpi,
                   DisplayInformation.GetForCurrentView().LogicalDpi,
                   pixelBuffer.ToArray());
                    //刷新图像  
                    await encoder.FlushAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<WriteableBitmap> getPNG(StorageFile sFile)
        {
            try
            {
                IRandomAccessStream stream = await sFile.OpenReadAsync();
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.PngDecoderId, stream);
                // 获取第一帧
                BitmapFrame frame = await decoder.GetFrameAsync(0);
                // 获取像素数据
                PixelDataProvider pixprd = await frame.GetPixelDataAsync(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, new BitmapTransform(), ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);
                byte[] rgbaBuff = pixprd.DetachPixelData();
                byte[] res = new byte[rgbaBuff.Length];
                for (int i = 0; i < rgbaBuff.Length; i += 4)
                {
                    byte r = rgbaBuff[i];
                    byte g = rgbaBuff[i + 1];
                    byte b = rgbaBuff[i + 2];
                    byte a = rgbaBuff[i + 3];
                    if (r == 0 && g == 0 && b == 0)
                    {
                        a = 0;
                    }
                    // 反色就是用255分别减去R，G，B的值
                    res[i] = r;
                    res[i + 1] =g;
                    res[i + 2] = b;
                    res[i + 3] = a;
                }
                // 创建新的位图对象
                WriteableBitmap wb = new WriteableBitmap((int)frame.PixelWidth, (int)frame.PixelHeight);
                // 复制数据
                res.CopyTo(wb.PixelBuffer);
                return wb;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async void savePNG(StorageFile outputFile, WriteableBitmap ima)
        {
            using (var outputStream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                outputStream.Size = 0;
            }
        }



    }
}
