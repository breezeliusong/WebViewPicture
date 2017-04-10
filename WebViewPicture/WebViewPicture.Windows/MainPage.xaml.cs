using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WebViewPicture
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            // The 'Host' part of the URI for the ms-local-stream protocol needs to be a combination of the package name
            // and an application-defined key, which identifies the specific resolver, in this case 'MyTag'.

            Uri url = webview.BuildLocalStreamUri("WebViewPicture.Windows", "test.html");
            StreamUriWinRTResolver myResolver = new StreamUriWinRTResolver();

            // Pass the resolver object to the navigate call.
            webview.NavigateToLocalStreamUri(url, myResolver);
        }


        public sealed class StreamUriWinRTResolver : IUriToStreamResolver
        {
            public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
            {
                if (uri == null)
                {
                    throw new Exception();
                }
                string path = uri.AbsolutePath;

                // Because of the signature of the this method, it can't use await, so we 
                // call into a seperate helper method that can use the C# await pattern.
                return GetContent(path).AsAsyncOperation();
            }

            private async Task<IInputStream> GetContent(string path)
            {
                // We use a package folder as the source, but the same principle should apply
                // when supplying content from other locations
                try
                {
                    Uri localUri = new Uri("ms-appdata:///local" + path);
                    StorageFile f = await StorageFile.GetFileFromApplicationUriAsync(localUri);
                    IRandomAccessStream stream = await f.OpenAsync(FileAccessMode.Read);
                    return stream;
                }
                catch (Exception) { throw new Exception("Invalid path"); }
            }
        }
    }
}
