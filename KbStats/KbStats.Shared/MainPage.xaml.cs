using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace KbStats
{
    using Core;
    using Views;
    using ViewModels;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : BasePage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainPageViewModel();

            // temporarily reacting to keyDowns is fine,
            // later we need a server sending us the pressed keys at OS wide level
            // this only works for UWP apps
            //Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            // this is just for WinUI 3
            //var window = (Application.Current as App)?.Window;
            //var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            //winUiWindow.KeyDown += OnKeyDown;//CoreWindow_KeyDown;
        }

        public void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            CommunicationLayer.AddKeyPressed(e.Key);
        }

        public void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            CommunicationLayer.AddKeyPressed(e.VirtualKey);
        }
    }
}