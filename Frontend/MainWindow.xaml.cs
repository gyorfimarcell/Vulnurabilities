using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using static System.Formats.Asn1.AsnWriter;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Vulnurability> vulnurabilities;
        HostInfo hostInfo;

        public MainWindow()
        {
            InitializeComponent();

            dgVulnurabilities.ItemsSource = vulnurabilities;
        }

        private void ReadPdf(string file)
        {
            using (PdfDocument document = PdfDocument.Open(file))
            {
                List<string> lines = [];

                foreach (var page in document.GetPages())
                {
                    var words = page.GetWords();
                    var blocks = DefaultPageSegmenter.Instance.GetBlocks(words);
                    blocks.ToList().ForEach(x => x.TextLines.Select(y => y.Text).ToList().ForEach(y => lines.Add(y)));
                }

                hostInfo = new();

                Regex dnsRegex = new(@"DNS Name: (.*)");
                string dnsLine = lines.Find(x => dnsRegex.IsMatch(x));
                hostInfo.Dns = dnsRegex.Match(dnsLine).Groups[1].Value;

                Regex ipRegex = new(@"IP: (.*)");
                string ipLine = lines.Find(x => ipRegex.IsMatch(x));
                hostInfo.Ip = ipRegex.Match(ipLine).Groups[1].Value;

                Regex macRegex = new(@"MAC Address: (.*)");
                string macLine = lines.Find(x => macRegex.IsMatch(x));
                hostInfo.Mac = macRegex.Match(macLine).Groups[1].Value;

                Regex osRegex = new(@"OS: (.*)");
                string osLine = lines.Find(x => osRegex.IsMatch(x));
                hostInfo.Os = osRegex.Match(osLine).Groups[1].Value;


                vulnurabilities = [];

                Regex nameRegex = new(@"[\d]+ - .+");

                int startIndex = lines.FindIndex(x => x == "Vulnurabilities");
                int i = startIndex + 1;

                string addTo = "";

                string[] HEADERS = [
                    "Synopsis",
                    "Description",
                    "See Also",
                    "Solution",
                    "Risk Factor",
                    "CVSS v3.0 Base Score",
                    "CVSS v3.0 Temporal Score",
                    "CVSS Base Score",
                    "CVSS Temporal Score",
                    "References",
                    "Plugin Information:",
                    "Plugin Output"
                ];

                while (i < lines.Count)
                {
                    if (nameRegex.IsMatch(lines[i]))
                    {
                        // Begin new vulnurability
                        vulnurabilities.Add(new() { Name = lines[i] });
                        addTo = "";
                    }
                    else if (lines[i] == "Synopsis")
                    {
                        vulnurabilities.Last().Synopsis = lines[++i];
                    }
                    else if (lines[i] == "Description")
                    {
                        vulnurabilities.Last().Description = "";
                        addTo = "desc";
                    }
                    else if (lines[i] == "See Also")
                    {
                        vulnurabilities.Last().SeeAlso = "";
                        addTo = "also";
                    }
                    else if (lines[i] == "Solution")
                    {
                        vulnurabilities.Last().Solution = "";
                        addTo = "sol";
                    }
                    else if (lines[i] == "Risk Factor")
                    {
                        vulnurabilities.Last().RiskFactor = lines[++i];
                    }
                    else if (lines[i] == "CVSS v3.0 Base Score")
                    {
                        vulnurabilities.Last().BaseScore = lines[++i];
                    }
                    else if (lines[i] == "CVSS v3.0 Temporal Score")
                    {
                        vulnurabilities.Last().TemporalScore = lines[++i];
                    }
                    else if (HEADERS.Contains(lines[i]))
                    {
                        addTo = "";
                    }
                    else if (addTo != "")
                    {
                        switch (addTo)
                        {
                            case "desc":
                                vulnurabilities.Last().Description += (vulnurabilities.Last().Description == "" ? "" : "\n") +  lines[i];
                                break;
                            case "sol":
                                vulnurabilities.Last().Solution += (vulnurabilities.Last().Solution == "" ? "" : "\n") + lines[i];
                                break;
                            case "also":
                                vulnurabilities.Last().SeeAlso += (vulnurabilities.Last().SeeAlso == "" ? "" : "\n") + lines[i];
                                break;
                            default:
                                break;
                        }
                    }
                    i++;
                }

                dgVulnurabilities.ItemsSource = vulnurabilities;
            }
        }

        private async void GetLabels()
        {
            await Task.WhenAll(vulnurabilities.Select(x => x.GetLabel(hostInfo)));
            dgVulnurabilities.Items.Refresh();
            cbLabels.ItemsSource = vulnurabilities.Select(x => x.Label).Distinct().Prepend("All");
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                ReadPdf(ofd.FileName);
                GetLabels();
            }
        }

        private void cbLabels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedLabel = cbLabels.SelectedItem.ToString();
            dgVulnurabilities.ItemsSource = vulnurabilities.Where(x => selectedLabel == "All" || x.Label == selectedLabel);
        }
    }
}