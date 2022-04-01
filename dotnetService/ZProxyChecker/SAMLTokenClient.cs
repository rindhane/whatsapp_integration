using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using System.IO.Compression;
using System.Xml;
/*
using System.Linq;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
*/

//credits & Reference: https://docs.microsoft.com/en-us/archive/blogs/rodneyviana/how-to-get-a-saml-protocol-response-from-adfs-using-c
namespace SamlTestForm
{
    //form to use the saml
    /*
    public partial class Form1 : Form
    {
    public Form1()
    {

         InitializeComponent();

    }
    private string getUrl = null;
    private string GetUrl
    {
        get
        {

            if (!String.IsNullOrEmpty(getUrl))

                return getUrl;

            StringBuilder domain = new StringBuilder();

            domain.Append(Environment.GetEnvironmentVariable("USERDNSDOMAIN"));

            if (domain.Length > 0)

            {

                domain.Clear();

                domain.Append(Environment.UserDomainName);

            }
            //return String.Format("https://{0}.{1}.lab/adfs/ls/MyIdpInitiatedSignOn.aspx?loginToRp=https://mysyte.com", Environment.MachineName.ToLower(), Environment.UserDomainName.ToLower());

            return String.Format("https://{0}.{1}.lab/adfs/ls/IdpInitiatedSignOn.aspx", Environment.MachineName.ToLower(), Environment.UserDomainName.ToLower());
        }

    }

    private void Form1_Load(object sender, EventArgs e)

    {

        textBox1.Text = GetUrl;

    }

    protected List<KeyValuePair<string, string>> forms;

    private HttpClient client = null;

    private HttpClientHandler handler;

    private HttpClient Client
    {

        get

        {

            if (client == null)

            {

                handler = new HttpClientHandler();

                handler.UseDefaultCredentials = true;

                handler.AllowAutoRedirect = false;

                handler.CookieContainer = new System.Net.CookieContainer();

                handler.UseCookies = true;

                client = new HttpClient(handler);

                client.MaxResponseContentBufferSize = 256000;

                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");

                client.DefaultRequestHeaders.ExpectContinue = false;

            }

            return client;

        }

    }

    private async void button1_Click(object sender, EventArgs e)

    {

        string url = String.Format("{0}?loginToRp={1}", textBox1.Text, HttpUtility.UrlEncode(comboBox1.SelectedItem.ToString()));

        // Limit the max buffer size for the response so we don't get overwhelmed

        HttpResponseMessage result;

        textBox3.Text="============== Start ===============";

        var nl = Environment.NewLine;

    

        string text;

        do

        {

            textBox3.AppendText(String.Format("{1}********** GET {0}{1}", url, Environment.NewLine));

            result = await Client.GetAsync(url);

            text = await result.Content.ReadAsStringAsync();

            IEnumerable<string> values;

            if(result.Headers.TryGetValues("location", out values))

            {

                foreach(string s in values)

                {

                    if (s.StartsWith("/"))

                    {

                        url = url.Substring(0, url.IndexOf("/adfs/ls")) + s;

                    }

                    else

                        url = s;

                }

            } else

            {

                url = "";

            }

            textBox3.AppendText(String.Format("{0}[Headers]{0}", Environment.NewLine));

            foreach(var pair in result.Headers)

            {

                string key = pair.Key;

                foreach(var val in pair.Value)

                    textBox3.AppendText(String.Format(" {0}={1}{2}", key, val, Environment.NewLine));

            }

            textBox3.AppendText(text);

        } while (!String.IsNullOrEmpty(url));

        Regex reg = new Regex("SAMLResponse\\W+value\\=\\\"([^\\\"]+)\\\"");

        MatchCollection matches = reg.Matches(text);

        string last = null;

        foreach (Match m in matches)

        {

            last = m.Groups[1].Value;

            textBox3.AppendText(String.Format(" {1}{1}{1}SAMLResponse={0}{1}", last, Environment.NewLine));

        }

        if(last != null)

        {

            byte[] decoded = Convert.FromBase64String(last);

            string deflated = Encoding.UTF8.GetString(decoded);

            XmlDocument doc = new XmlDocument();

            StringBuilder sb = new StringBuilder();

            doc.LoadXml(deflated);

            using(StringWriter sw = new StringWriter(sb))

            {

                using (XmlTextWriter tw = new XmlTextWriter(sw) { Formatting = Formatting.Indented })

                {

                    doc.WriteTo(tw);

                }

            }

            textBox3.AppendText(String.Format(" {1}{1}{1}XML Formated:{1}{0}{1}", sb.ToString(), Environment.NewLine));

            

        }

    }

    private void DysplayError(Exception ex)

    {

        MessageBox.Show(String.Format("Error: {0}{1}Stack:{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace));

    }

    private async void button2_Click(object sender, EventArgs e)
    {
        try
        {

            var response = await Client.GetAsync(textBox1.Text);

            response.EnsureSuccessStatusCode();

            comboBox1.Items.Clear();

            string text = await response.Content.ReadAsStringAsync();

            Regex reg = new Regex("option\\W+value\\=\\\"([^\\\"]+)\\\"");

            MatchCollection matches = reg.Matches(text);

            foreach(Match m in matches)

            {

                comboBox1.Items.Add(m.Groups[1].Value);

            }

            if (matches.Count == 0)

            {

                MessageBox.Show("No Reliant Party found");

                button1.Enabled = false;

            } else

            {

                button1.Enabled = true;

                comboBox1.SelectedIndex = 0;

            }

        } catch(Exception ex)

        {

            DysplayError(ex);

            return;

        }

    }

}
*/
}

namespace AdfsSaml //Microsoft.Samples.AdfsSaml
    {

        /// <summary>

        /// Class to get a SAML response from ADFS.

        /// it requires that the SAML endpoint with POST binding is configured in ADFS

        /// </summary>

        public class SamlResponse

        {

            /// <summary>

            /// If true, ADFS url will not be validated

            /// </summary>

            public bool EnableRawUrl

            {

                get;

                set;

            }

            private Uri serverAddress = null;

            private HttpClient client = null;

            private HttpClientHandler handler;

            private HttpClient Client

            {

                get

                {

                    if (client == null)

                    {

                        handler = new HttpClientHandler();

                        handler.UseDefaultCredentials = true;

                        handler.AllowAutoRedirect = false;

                        handler.CookieContainer = new System.Net.CookieContainer();

                        handler.UseCookies = true;

                        client = new HttpClient(handler);

                        client.MaxResponseContentBufferSize = 256000;

                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

                        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");

                        client.DefaultRequestHeaders.ExpectContinue = false;

                    }

                    return client;

                }

            }

            /// <summary>

            /// Url of ADFS server (e.g. https://adfs.contoso.com)

            /// </summary>

            public Uri ServerAddress

            {

                get

                {

                    return serverAddress;

                }

                set

                {

                    if(EnableRawUrl)

                    {

                        serverAddress = value;

                        return;

                    }

                    string host = value.Host;

                    string scheme = value.Scheme;

                    if(scheme != "https")

                    {

                        throw new ArgumentException("ADFS rquires scheme https. Set EnableRawUrl to true to override it", "ServerAddress");

                    }

                    serverAddress = new Uri(String.Format("https://{0}//adfs/ls/IdpInitiatedSignOn.aspx", host));

                }

            }

            /// <summary>

            /// Initialize the class

            /// </summary>

            public SamlResponse()

            {

            }

            /// <summary>

            /// Initialize the class

            /// </summary>

            /// <param name="Url">Urn of reliant party as defined in ADFS. Ise GetReliantPArtyCollection to get the list</param>

            /// <param name="IsRawUrl">If true, ADFS url will not be validated</param>

            public SamlResponse(Uri Url, bool IsRawUrl = false)

            {

                EnableRawUrl = IsRawUrl;

                ServerAddress = Url;

            }

            private async Task<string[]> GetReliantPartyCollectionInternal()

            {

                if (serverAddress == null)

                {

                    throw new NullReferenceException("ServerAddress was not set");

                }

                var response = await Client.GetAsync(serverAddress);

                response.EnsureSuccessStatusCode();

                string text = await response.Content.ReadAsStringAsync();
                Console.WriteLine("reliantParty"+text);

                Regex reg = new Regex("option\\W+value\\=\\\"([^\\\"]+)\\\"");

                MatchCollection matches = reg.Matches(text);

                if(matches.Count == 0)

                {

                    return null;

                }

                string[] rps = new string[matches.Count];

                uint i = 0;

                foreach (Match m in matches)

                {

                    rps[i++]=m.Groups[1].Value;

                }

                return rps;

            }

            /// <summary>

            /// Get the list of Reliant Parties with SAML endpoint with binding POST in ADFS

            /// </summary>

            public string[] GetReliantPartyCollection()

            {

                return GetReliantPartyCollectionInternal().Result;

            }

            /// <summary>

            /// Retrieve the SAML Response from ADFS for ReliantPartyUrn

            /// </summary>

            /// <param name="ReliantPartyUrn">Urn of reliant party as defined in ADFS. Ise GetReliantPArtyCollection to get the list</param>

            public string RequestSamlResponse(string ReliantPartyUrn)

            {

                if(serverAddress == null)

                {

                    throw new NullReferenceException("ServerAddress was not set");

                }

                if(String.IsNullOrEmpty(ReliantPartyUrn) && !EnableRawUrl)

                {

                    throw new ArgumentException("Reliant Party Urn cannot be empty if EnableRawUrl is not true");

                }

                return SamlResponseInternal(ReliantPartyUrn).Result;

                

            }

            private async Task<string> SamlResponseInternal(string ReliantPartyUrn)

            {

                StringBuilder url = new StringBuilder(String.Format("{0}?loginToRp={1}", serverAddress, HttpUtility.UrlEncode(ReliantPartyUrn)));

                HttpResponseMessage result;

                

                do

                {

                    result = await Client.GetAsync(url.ToString());

                    string text = await result.Content.ReadAsStringAsync();

                    IEnumerable<string> values;

                    if (result.Headers.TryGetValues("location", out values))

                    {

                        foreach (string s in values)

                        {

                            if (s.StartsWith("/"))

                            {

                                string newUrl = url.ToString().Substring(0, url.ToString().IndexOf("/adfs/ls"));

                                url.Clear();

                                url.Append(newUrl);

                                url.Append(s);

                            }

                            else

                            {

                                url.Clear();

                                url.Append(s);

                            }

                        }

                    }

                    else

                    {

                        url.Clear();

                    }

                    if (url.Length == 0)

                    {

                        Regex reg = new Regex("SAMLResponse\\W+value\\=\\\"([^\\\"]+)\\\"");

                        MatchCollection matches = reg.Matches(text);

                        foreach (Match m in matches)

                        {

                            return m.Groups[1].Value;

                        }

                    }

                } while (url.Length > 0);

                throw new InvalidOperationException("Unable to get a SAMLP response from ADFS");

            }

            public static string SamlToXmlString(string EncodedResponse)

            {

                byte[] decoded = Convert.FromBase64String(EncodedResponse);

                string deflated = Encoding.UTF8.GetString(decoded);

                XmlDocument doc = new XmlDocument();

                StringBuilder sb = new StringBuilder();

                doc.LoadXml(deflated);

                using (StringWriter sw = new StringWriter(sb))

                {

                    using (XmlTextWriter tw = new XmlTextWriter(sw) { Formatting = Formatting.Indented })

                    {

                        doc.WriteTo(tw);

                    }

                }

                return sb.ToString();

            }

        }

    }
