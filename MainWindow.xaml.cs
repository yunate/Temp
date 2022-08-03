using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DynamicLoadWV2Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadAssmbly();
        }

        private void LoadAssmbly()
        {
            string[] dlls = {
                @"..\..\..\Microsoft.Web.WebView2.Wpf.dll",
                @"..\..\..\Microsoft.Web.WebView2.Core.dll"
            };
            foreach (string dll in dlls)
            {
                Assembly.LoadFrom(dll);
            }
        }

        private Assembly GetAssmbly(String name)
        {
            AppDomain domain = AppDomain.CurrentDomain;
            foreach (Assembly asm in domain.GetAssemblies())
            {
                if (asm.FullName.Contains(name))
                {
                    return asm;
                }
            }
            throw new ApplicationException($"Can't find assembly {name}");
        }

        private async void CreateWV2(object sender, RoutedEventArgs e)
        {
            var DLL = GetAssmbly("Microsoft.Web.WebView2.Wpf");
            var type = DLL.GetType("Microsoft.Web.WebView2.Wpf.WebView2");
            dynamic newWebView = Activator.CreateInstance(type);

            var Source = type.GetProperty("Source");
            Source.SetValue(newWebView, new Uri("https://www.google.com"));

            // must show first
            Window window = new Window();
            window.Content = newWebView;
            window.Show();

            var EnsureCoreWebView2Async = type.GetMethod("EnsureCoreWebView2Async");
            EnsureCoreWebView2Async.Invoke(newWebView, new object[] { null, null });
        }
    }
}
