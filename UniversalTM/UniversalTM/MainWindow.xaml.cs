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
using System.Windows.Controls.Primitives;
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

        private Tuple<List<State>, Flags, int> states = null;
        
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
                    this.log.Inlines.Add(new Run("[+] Loading Turing Machine : Success .\n"));
                    this.clearTM.IsEnabled = true;
                    this.loadTM.IsEnabled = false;
                    if (tape_data.Columns.Count > 0)
                    {
                        this.exec.IsEnabled = true;
                    }
                    /*this.insertToTape.IsEnabled = true;
                    this.input_box.IsEnabled = true;*/
                }else if(states.Item2 == Flags.NO_STATES)
                {
                    this.TM_code.Inlines.ElementAt(0).Foreground = Brushes.Red;
                    this.log.Inlines.Add(new Run("[+] Loading Turing Machine : Failure(States Error) .\n"));
                }
                else if(states.Item2 == Flags.NO_DESCRIPTION)
                {
                    this.TM_code.Inlines.ElementAt(states.Item3).Foreground = Brushes.Red;
                    this.log.Inlines.Add(new Run("[+] Loading Turing Machine : Failure(Machine Description Error line: " + states.Item3 + "  ) .\n"));
                }

            }
        }

        private void Write2Tape(object sender, RoutedEventArgs e)
        {
            char[] input = this.input_box.Text.ToCharArray();
            if(input.Length==0)
            {
                
                this.log.Inlines.Add(new Run("[+] Input is Empty\n"));
                this.exec.IsEnabled = false;
            }
            else
            {
                DataTable table = new DataTable();
                
                DataRow row = table.NewRow();
                
                for (int i = 0; i < input.Length; i++)
                {
                    
                    table.Columns.Add(new DataColumn(i.ToString(),typeof(string)));
                    row[i] = input[i].ToString();
                    
                }
                
                table.Rows.Add(row);
                this.tape_data.ItemsSource = table.DefaultView;
                this.log.Inlines.Add(new Run("[+] Input has written in the tape .\n"));
                if (states!=null)
                {
                    if(states.Item1.Count>0)
                        this.exec.IsEnabled = true;
                }
                this.clearTape.IsEnabled = true;
            }
        }

        private void ClearTM(object sender, RoutedEventArgs e)
        {
            if (this.states.Item1.Count > 0)
            {

                this.states = null;
                /*this.log.Inlines.Add(new Run(this.states.Item1[0].Name+" \n"));*/
                this.exec.IsEnabled = false;
                this.clearTM.IsEnabled = false;
                this.loadTM.IsEnabled = true;
                this.TM_code.Inlines.Clear();
                this.log.Inlines.Add(new Run("[+] Turing machine removed .\n"));
                //this.log.Inlines.Clear();
            }
        }

        private void Execute(object sender, RoutedEventArgs e)
        {
            int header = 0;
            State state = states.Item1[0];
            //this.log.Inlines.Add(new Run("[+] IN RUN .\n"));

            
            while (true)
            {
                try
                {


                    if (header == this.tape_data.Columns.Count)
                    {
                        if (state == states.Item1[(states.Item1.Count) - 1])
                        {
                            this.log.Inlines.Add(new Run("[+] Input was successfuly accepted by the Turing machine .\n"));
                            break;
                        }
                        else
                        {
                            this.log.Inlines.Add(new Run("[+] Input was not successfuly accepted by the Turing machine .\n"));
                            break;
                        }
                    }
                    DataTable tapeData = ((DataView)tape_data.ItemsSource).ToTable();
                    string i = tapeData.Rows[0][header].ToString();
                    
                    this.log.Inlines.Add(new Run("[+] Input " + i + " .\n"));

                    if (state.writeToTape.ContainsKey(i))
                    {
                        tapeData.Rows[0][header] = state.writeToTape[i];
                        this.tape_data.ItemsSource = tapeData.DefaultView;
                    }

                    int move = (int)state.moveToTape[i];
                    if ((header + move) >= 0)
                    {
                        header += move;
                    }
                    state = state.nextState[i];
                 }
                catch {
                    this.log.Inlines.Add(new Run("[+] This input string is not supported by the TM.\n"));
                    break; 
                }

            }
        }

        private void ClearTape(object sender, RoutedEventArgs e)
        {
            if (this.tape_data.Columns.Count > 0)
            {

                this.exec.IsEnabled = false;
                this.clearTape.IsEnabled = false;
                this.tape_data.Columns.Clear();
                this.tape_data.ItemsSource = null;
                this.log.Inlines.Add(new Run("[+] Input removed from the tape .\n"));
            }
        }
    }
}
