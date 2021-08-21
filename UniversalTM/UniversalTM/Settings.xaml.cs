using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace UniversalTM
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public int inputshowvalue;
        public Settings()
        {
            InitializeComponent();
        }

        private void ExitSettings(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplySettings(object sender, RoutedEventArgs e)
        {

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
            string txt_value = this.Num_of_lines.Text;
            if (int.Parse(txt_value) > 0)
            {
                inputshowvalue = int.Parse(txt_value);
            }

            UpdateLayout();


            List<Tuple<string, string, string>> settings = new List<Tuple<string, string, string>>{ 
                new Tuple<string, string, string>("MainTheme", "Name", str_theme),
                new Tuple<string, string, string>("InputLimit", "Value", inputshowvalue.ToString()),
                new Tuple<string, string, string>("Log","Font", execFont.FontFamily.ToString()),
                new Tuple<string, string, string>("Tape","Font",tapeFont.FontFamily.ToString()),
                new Tuple<string, string, string>("TM","Font",simTMFont.FontFamily.ToString())
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
        public int getValue()
        {
            return this.inputshowvalue;
        }
        private void Reset(object sender, RoutedEventArgs e)
        {

        }
    }
}
