using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace MakePNG
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }


        PNG myPng = new PNG();
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = KnownFolders.SavedPictures;
            var desFloder = await storageFolder.CreateFolderAsync("图片压缩", CreationCollisionOption.OpenIfExists);
            var desFileName = "cache"  + ".png";
            var file = await desFloder.CreateFileAsync(desFileName, CreationCollisionOption.ReplaceExisting);
            if (file != null)   //文件不为空则进行下一步  
            {
                if (await myPng.getJPG(file, root))
                {
                    try
                    {
                        var ima = await myPng.getPNG(file);
                        Result.Source = ima;
                    }
                    catch (Exception ex)
                    {
                        await new MessageDialog(ex.Message).ShowAsync();
                    }
                }
            }
        }
    }
}
