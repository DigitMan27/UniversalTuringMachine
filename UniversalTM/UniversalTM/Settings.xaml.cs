using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace UniversalTM
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// Functionality of the Settings window
    /// </summary>
    public partial class Settings : Window
    {
        private string inputValue, colorValue; // with these i pass the values for the header_color and InputShowValue so the user don't have to restart the app .
        private MainWindow window;
        public Settings(MainWindow parent)
        {
            this.window = parent;
            InitializeComponent();
            if (File.Exists("Config.xml"))
            {
                LoadSettings();
            }
        }

        private void LoadSettings()
        {
            List<string> settings = new List<string>();
            XmlDocument doc = new XmlDocument();
            doc.Load("Config.xml");
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {

                string text = node.Attributes["Value"].InnerText;
                settings.Add(text);
            }
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri("Themes\\" + settings[0] + ".xaml", UriKind.Relative);
            App.Current.Resources.MergedDictionaries.Add(dict);


        }

        private void ExitSettings(object sender, RoutedEventArgs e)
        {
            window.InputShowValue = int.Parse(inputValue);
            window.header_color = colorValue;
            this.Close();
        }

        private void ApplySettings(object sender, RoutedEventArgs e)
        {
            string inputDisplayLim = this.Num_of_lines.Text;
            string header_color="";
            ComboBoxItem theme = (ComboBoxItem)this.theme_box.SelectedItem;
            string str_theme = theme.Content.ToString();
            if (!string.IsNullOrEmpty(str_theme))
            {
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri("Themes\\" + str_theme + ".xaml", UriKind.Relative);
                App.Current.Resources.MergedDictionaries.Add(dict);
            }

            ComboBoxItem simTMFont = (ComboBoxItem)this.TM_font_changer.SelectedItem;
            ComboBoxItem execFont = (ComboBoxItem)this.font_changer.SelectedItem;
            ComboBoxItem tapeFont = (ComboBoxItem)this.Tape_font_changer.SelectedItem;
            ComboBoxItem ColortapeHeader = (ComboBoxItem)this.header_color.SelectedItem;
            if (!string.IsNullOrEmpty(simTMFont.FontFamily.ToString()))
            {
                App.Current.Resources["simTMRes"] = new FontFamily(simTMFont.FontFamily.ToString());
            }
            if (!string.IsNullOrEmpty(execFont.FontFamily.ToString()))
            {
                App.Current.Resources["ExecFontRes"] = new FontFamily(execFont.FontFamily.ToString());
            }
            if (!string.IsNullOrEmpty(tapeFont.FontFamily.ToString()))
            {
                App.Current.Resources["TapeFontRes"] = new FontFamily(tapeFont.FontFamily.ToString());
            }
            if (!string.IsNullOrEmpty(ColortapeHeader.Background.ToString()))
            {
                header_color = ColortapeHeader.Background.ToString();
                colorValue = header_color;
            }
            if (!string.IsNullOrEmpty(inputDisplayLim) && int.Parse(inputDisplayLim)!=500 && int.Parse(inputDisplayLim)>0) // default value if negative,empty or same as the default value
            {
                    //pass
            }
            else
            {
                inputDisplayLim = "500";
            }
            inputValue = inputDisplayLim;
            UpdateLayout();

            List<Tuple<string, string, string>> settings = new List<Tuple<string, string, string>>{
                new Tuple<string, string, string>("MainTheme", "Value", str_theme),
                new Tuple<string, string, string>("InputLimit", "Value", inputDisplayLim),
                new Tuple<string, string, string>("LogFont","Value", execFont.FontFamily.ToString()),
                new Tuple<string, string, string>("TapeFont","Value",tapeFont.FontFamily.ToString()),
                new Tuple<string, string, string>("TMFont","Value",simTMFont.FontFamily.ToString()),
                new Tuple<string, string, string>("HeaderColor", "Value", header_color)
            };

            XmlDocument config = new XmlDocument();
            XmlNode decl = config.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement setting_tag = config.CreateElement("Settings");
            config.AppendChild(decl);
            
            foreach (Tuple<string,string,string> element in settings)
            {
                XmlElement xmlEl = config.CreateElement(element.Item1);
                xmlEl.SetAttribute(element.Item2, element.Item3);
                setting_tag.AppendChild(xmlEl);
            }
            config.AppendChild(setting_tag);
            
            config.Save("Config.xml");

        }
    }
}
