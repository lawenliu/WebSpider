using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSpider.WebCrawl;

namespace WebSpider
{
    public partial class Spider : Form
    {
        List<string> urlList = new List<string>();
        static int urlIndex = 0;
        private string projectPath = @".\Cassandra";

        public Spider()
        {
            InitializeComponent();
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlDocument document = webBrowser.Document;
            HtmlElement navElem = document.GetElementById("toc_nav");
            HtmlElementCollection liList = navElem.GetElementsByTagName("li");
            GetPages(liList);
            RefreshUrl();
        }

        private void RefreshUrl()
        {
            Random rd = new Random(DateTime.Now.Second);
            int n = rd.Next(5000);
            Thread.Sleep(n);
            if (urlList.Count > urlIndex)
            {
                webBrowser1.Url = new Uri(urlList[urlIndex++]);
            }
        }

        private void GetPages(HtmlElementCollection liList)
        {
            if (liList == null)
            {
                return;
            }
            foreach (HtmlElement elem in liList)
            {
                GetPages(elem.Children);
                if (elem.TagName.Equals("a", StringComparison.OrdinalIgnoreCase))
                {
                    string url = elem.GetAttribute("href");
                    urlList.Add(url);
                }
            }
        }

        private void InitWebBrowser()
        {
            int BrowserVer, RegVal;

            // get the installed IE version
            using (WebBrowser Wb = new WebBrowser())
                BrowserVer = Wb.Version.Major;

            // set the appropriate IE version
            if (BrowserVer >= 11)
                RegVal = 11001;
            else if (BrowserVer == 10)
                RegVal = 10001;
            else if (BrowserVer == 9)
                RegVal = 9999;
            else if (BrowserVer == 8)
                RegVal = 8888;
            else
                RegVal = 7000;

            // set the actual key
            RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            Key.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe", RegVal, RegistryValueKind.DWord);
            Key.Close();
        }

        private void Spider_Load(object sender, EventArgs e)
        {
            InitWebBrowser();
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser1.ScriptErrorsSuppressed = true;            
            webBrowser.Url = new Uri("https://docs.datastax.com/en/cassandra/2.0/share/glossary/gloss_bootstrap.html");
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            HtmlDocument doc = webBrowser1.Document;
            HtmlElement element1 = doc.GetElementById("center");
            HtmlElementCollection collection1 = element1.GetElementsByTagName("article");
            if(collection1.Count > 0)
            {
                ParseContent(collection1[0]);
            }
            
            RefreshUrl();
        }

        private void ParseContent(HtmlElement elem)
        {
            if (elem.Children == null)
            {
                return;
            }            
                        
            foreach (HtmlElement tmpElem in elem.Children)
            {
                if (tmpElem.TagName.Equals("noscript", StringComparison.OrdinalIgnoreCase)
                    || (tmpElem.Id != null && tmpElem.Id.Equals("footer", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                else
                {
                    string innerText = tmpElem.InnerText;
                    FileOperator.WriteFile(projectPath, urlIndex.ToString(), innerText);
                }
            }
        }
    }
}
