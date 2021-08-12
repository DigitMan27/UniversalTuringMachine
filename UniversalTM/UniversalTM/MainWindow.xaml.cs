using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace UniversalTM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread loadTM_t;

        private Tuple<List<State>, Flags,int> states;
        private TMclass turingMachine;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void LoadTM(object sender, RoutedEventArgs e)
        {
            string filepath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "uTM files (*.uTM)|*.uTM";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                //Get the path of specified file
                filepath = openFileDialog.FileName;
                //MessageBox.Show("file path", filePath);
            }
            if (!filepath.Equals(""))
            {
                var reader = new StreamReader(filepath);
                int line_n = 0;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    this.TM_code.Inlines.Add(new Run(line_n + ".  " + line + "\n"));
                    line_n += 1;
                }
                turingMachine = new TMclass(filepath);
                states = turingMachine.TM();
                if (states.Item2 == Flags.SUCCESS)
                {
                    this.log.Text = "[+] Loading Turing Machine : Success .\n";
                    this.clearTM.IsEnabled = true;
                    this.loadTM.IsEnabled = false;
                    this.insertToTape.IsEnabled = true;
                    this.input_box.IsEnabled = true;
                }else if(states.Item2 == Flags.NO_STATES)
                {
                    this.TM_code.Inlines.ElementAt(0).Foreground = Brushes.Red;
                    this.log.Text = "[+] Loading Turing Machine : Failure(States Error) .\n";
                }
                else if(states.Item2 == Flags.NO_DESCRIPTION)
                {
                    this.TM_code.Inlines.ElementAt(states.Item3).Foreground = Brushes.Red;
                    this.log.Text = "[+] Loading Turing Machine : Failure(Machine Description Error line: "+states.Item3+"  ) .\n";
                }

            }
        }

        private void Write2Tape(object sender, RoutedEventArgs e)
        {
            char[] input = this.input_box.Text.ToCharArray();
            if(input.Length==0)
            {
                this.log.Text = "[+] Input is Empty\n";
            }
            else
            {
                DataTable table = new DataTable();
                
                DataRow row = table.NewRow();
                for (int i = 0; i < input.Length; i++)
                {
                    /*var ind_col = new DataGridTextColumn
                    {
                        Header = i.ToString(),
                        Binding = new Binding(input[i].ToString())
                    };*/
                    
                    table.Columns.Add(new DataColumn(i.ToString(),typeof(string)));
                    
                    row[i] = input[i].ToString();
                    
                }
                
                table.Rows.Add(row);
                this.tape_data.ItemsSource = table.DefaultView;
                /*this.tape_data.ItemsSource = table.AsDataView();*/
            }
        }
    }
}
