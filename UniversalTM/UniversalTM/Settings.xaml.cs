using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
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
            List<int> font_indexes = new List<int>();
            XmlDocument doc = new XmlDocument();
            doc.Load("Config.xml");
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {

                string text = node.Attributes["Value"].InnerText;
                int ind = int.Parse(node.Attributes["Index"].InnerText);
                settings.Add(text);
                if(ind!=-1)
                    font_indexes.Add(ind);
            }
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri("Themes\\" + settings[0] + ".xaml", UriKind.Relative);
            App.Current.Resources.MergedDictionaries.Add(dict);
            this.tape_len.Text = settings[6];
            this.blank_symb.Text = settings[7];
            this.Num_of_lines.Text = settings[1];

            this.theme_box.SelectedIndex = font_indexes[0];
            this.TM_font_changer.SelectedIndex = font_indexes[3];
            this.Tape_font_changer.SelectedIndex = font_indexes[2];
            this.font_changer.SelectedIndex = font_indexes[1];
            this.header_color.SelectedIndex = font_indexes[4];




        }

        private void ExitSettings(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

        private void ApplySettings(object sender, RoutedEventArgs e)
        {
            string inputDisplayLim = this.Num_of_lines.Text;
            string tapeLen = this.tape_len.Text;
            string blankStr = this.blank_symb.Text;
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
                
            }
            if(string.IsNullOrEmpty(tapeLen) || !Regex.IsMatch(inputDisplayLim, "\\d+"))
            {
                inputDisplayLim = "500";
            }

            if (string.IsNullOrEmpty(tapeLen) || !Regex.IsMatch(tapeLen,"\\d+"))
            {
                tapeLen = "256";
            }
            if (string.IsNullOrEmpty(blankStr))
            {
                blankStr = "ε";
            }
            
            UpdateLayout();

            // with these i pass the values for the header_color and InputShowValue so the user don't have to restart the app .
            
            window.InputShowValue = int.Parse(inputDisplayLim);
            
            window.tapeLen = int.Parse(tapeLen);
            

            window.header_color = header_color;
            window.blank_symbol = blankStr;

            window.tapeData.Clear();
            DataTable settingstb = new DataTable();
            DataRow row = settingstb.NewRow();
            for (int i = 0; i < int.Parse(tapeLen); i++)
            {
                //if (i >= window.tapeData.Columns.Count)
                settingstb.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
                row[i] = window.blank_symbol;

            }
            settingstb.Rows.Add(row);
            window.tapeData = settingstb;
            window.tape_data.ItemsSource = window.tapeData.DefaultView;
            

            List<Tuple<string, string, string,string,int>> settings = new List<Tuple<string, string, string,string,int>>{
                new Tuple<string, string, string,string,int>("MainTheme", "Value", str_theme,"Index",this.theme_box.SelectedIndex),
                new Tuple<string, string, string,string,int>("InputLimit", "Value", inputDisplayLim,"Index",-1),
                new Tuple<string, string, string,string,int>("LogFont","Value", execFont.FontFamily.ToString(),"Index",this.font_changer.SelectedIndex),
                new Tuple<string, string, string,string,int>("TapeFont","Value",tapeFont.FontFamily.ToString(),"Index",this.Tape_font_changer.SelectedIndex),
                new Tuple<string, string, string,string,int>("TMFont","Value",simTMFont.FontFamily.ToString(),"Index",this.TM_font_changer.SelectedIndex),
                new Tuple<string, string, string,string,int>("HeaderColor", "Value", header_color,"Index",this.header_color.SelectedIndex),
                new Tuple<string, string, string,string,int>("TapeLength", "Value", tapeLen,"Index",-1),
                new Tuple<string, string, string,string,int>("BlankSymbol", "Value", blankStr,"Index",-1)
            };

            XmlDocument config = new XmlDocument();
            XmlNode decl = config.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement setting_tag = config.CreateElement("Settings");
            config.AppendChild(decl);
            
            foreach (Tuple<string,string,string,string,int> element in settings)
            {
                XmlElement xmlEl = config.CreateElement(element.Item1);
                xmlEl.SetAttribute(element.Item2, element.Item3);
                xmlEl.SetAttribute(element.Item4, element.Item5.ToString());
                setting_tag.AppendChild(xmlEl);
            }
            config.AppendChild(setting_tag);
            
            config.Save("Config.xml");

        }
    }
}
