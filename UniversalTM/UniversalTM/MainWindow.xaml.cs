﻿using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;

namespace UniversalTM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread execTM_t;
        Settings window_setting;

        private delegate void delUpdateTextBlock(string txt);
        private delegate void delUpdateTape(DataTable tp);
        private delegate void delEnableBtns();
        private delegate void delUpdateHeader(int pos,int move);

        public DataTable tapeData = new DataTable();


        //settings default variable values
        public int InputShowValue = 500; // default value in case that the user dont use the settings .
        public string header_color = "#FFFF00"; // default value (Yellow) 
        public string blank_symbol = "ε";
        public int tapeLen = 256;
        //End 
        
        private bool termination = false;
        private bool _is_running = false;
        private char[] input; // user input
        private int output_len = 0; // increment (not used yet)

        private Tuple<List<State>, Flags, int> states = null;
        
        private TMclass turingMachine;

        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists("Config.xml"))
            {
                LoadSettings();
            }
            InitTape();
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
            InputShowValue = int.Parse(settings[1]);
            header_color = settings[5];
            tapeLen = int.Parse(settings[6]);
            blank_symbol = settings[7];
            App.Current.Resources["simTMRes"] = new FontFamily(settings[4]);
            App.Current.Resources["ExecFontRes"] = new FontFamily(settings[2]);
            App.Current.Resources["TapeFontRes"] = new FontFamily(settings[3]); 
            App.Current.Resources.MergedDictionaries.Add(dict);
            

        }

        public void InitTape()
        {

            DataRow row = tapeData.NewRow();
            for (int i = 0; i < tapeLen; i++)
            {

                tapeData.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
                row[i] = blank_symbol;

            }
            
            tapeData.Rows.Add(row);
            this.tape_data.ItemsSource = tapeData.DefaultView;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void LoadTM(object sender, RoutedEventArgs e)
        {
            if(this.TM_code.Inlines.Count>0)
                this.TM_code.Inlines.Clear();
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
                StreamReader reader = new StreamReader(filepath);
                int line_n = 0;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    this.TM_code.Inlines.Add(new Run(line_n + ". " + line + "\n"));
                    line_n += 1;
                }
                turingMachine = new TMclass(filepath);
                states = turingMachine.TM();
                if (states.Item2 == Flags.SUCCESS)
                {
                    this.log.Inlines.Add(new Run("[+] Loading Turing Machine : Success .\n"));
                    this.clearTM.IsEnabled = true;
                    this.loadTM.IsEnabled = false;
                    if (input!=null)
                    {
                        this.exec.IsEnabled = true;
                    }
                }else if(states.Item2 == Flags.NO_STATES)
                {
                    this.TM_code.Inlines.ElementAt(states.Item3).Foreground = Brushes.Red;
                    this.log.Inlines.Add(new Run("[+] Loading Turing Machine : Failure(States Error) .\n"));
                }
                else if(states.Item2 == Flags.NO_DESCRIPTION)
                {
                    this.TM_code.Inlines.ElementAt(states.Item3).Foreground = Brushes.Red;
                    this.log.Inlines.Add(new Run("[+] Loading Turing Machine : Failure(Machine Description Error line: " + states.Item3 + "  ) .\n"));
                }
                else if (states.Item2 == Flags.NO_ACCEPT)
                {
                    this.TM_code.Inlines.ElementAt(states.Item3).Foreground = Brushes.Red;
                    this.log.Inlines.Add(new Run("[+] Loading Turing Machine : Failure(Missing Accept State) .\n"));
                }
                else if (states.Item2 == Flags.NO_REJECT)
                {
                    this.TM_code.Inlines.ElementAt(states.Item3).Foreground = Brushes.Red;
                    this.log.Inlines.Add(new Run("[+] Loading Turing Machine : Failure(Missing Reject State) .\n"));
                }
                else if (states.Item2 == Flags.SAME_STATE)
                {
                    this.TM_code.Inlines.ElementAt(states.Item3).Foreground = Brushes.Red;
                    this.log.Inlines.Add(new Run("[+] Loading Turing Machine : Failure(Same Accept and Reject State) .\n"));
                }

            }
        }

        private void Write2Tape(object sender, RoutedEventArgs e)
        {
            if (this.input_box.Text.Contains(this.blank_symbol))
            {
                MessageBox.Show("The blank symbol( " + this.blank_symbol + " ) detected on your input\n","Error");
                return;
            }
            input = this.input_box.Text.ToCharArray();
            
            if(input.Length==0)
            {
                
                this.log.Inlines.Add(new Run("[+] Input is Empty\n"));
                this.exec.IsEnabled = false;
            }
            else
            {
                //DataTable table = new DataTable();


                DataRow row = tapeData.Rows[0];
                for (int i = 0; i < input.Length; i++)
                {
                    
                    //tapeData.Columns.Add(new DataColumn(i.ToString(),typeof(string)));
                    row[i] = input[i].ToString();
                    
                }

                //tapeData.Rows.Add(row);
                this.tape_data.ItemsSource = tapeData.DefaultView;
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
                this.stop.IsEnabled = false;
                this.TM_code.Inlines.Clear();
                this.log.Inlines.Add(new Run("[+] Turing machine removed .\n"));
                //this.log.Inlines.Clear();
            }
        }

        private void Execute(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < output_len+1; i++)
            {
                this.tape_data.Columns[i].HeaderStyle = null;
            }

            this.exec.IsEnabled = false;
            this.clearTM.IsEnabled = false;
            this.clearTape.IsEnabled = false;
            this.insertToTape.IsEnabled = false;
            this.stop.IsEnabled = true;
            this.termination = false;
            this.exec_speed.IsEnabled = false;
            this.menu.IsEnabled = false;
            this._is_running = true;
            double time;
            
            time = (string.IsNullOrEmpty(exec_speed.Text) || double.Parse(exec_speed.Text) < 0) ? time = 0 : time = double.Parse(exec_speed.Text); // default value if empty or negative value .

            tapeData = ((DataView)this.tape_data.ItemsSource).ToTable();

            execTM_t = new Thread(() => Execute_in_Thread(tapeData, time));
            execTM_t.IsBackground = true;
            execTM_t.Start();

        }

        private void UpdateTXT(string txt)
        {
            this.log.Inlines.Add(new Run(txt));
            
        }

        private void UpdateTable(DataTable tp)
        {
            this.tape_data.ItemsSource = tp.DefaultView;
        }

        private void EnableBtns()
        {
            this.exec.IsEnabled = true;
            this.clearTM.IsEnabled = true;
            this.clearTape.IsEnabled = true;
            this.insertToTape.IsEnabled = true;
            this.exec_speed.IsEnabled = true;
            this.stop.IsEnabled = false;
            this.menu.IsEnabled = true;
        }

        private void HeaderUpdate(int pos,int move)
        {
            System.Collections.ObjectModel.ObservableCollection<System.Windows.Controls.DataGridColumn> cols = this.tape_data.Columns;

            Style style = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
            style.Setters.Add(new Setter
            {
                Property = BackgroundProperty,
                Value = new BrushConverter().ConvertFromString(header_color) as SolidColorBrush
            }) ;
            style.Setters.Add(new Setter
            {
                Property = BorderBrushProperty,
                Value = Brushes.Black
            });

            cols[pos].HeaderStyle = style;
            if (move > 0) { cols[pos - 1].HeaderStyle = null; }
            else if(move < 0 && pos < tapeData.Columns.Count-1) { cols[pos + 1].HeaderStyle = null; }

        }

        private void Execute_in_Thread(DataTable tapeData,double time) // do something if table is out of space | detailed mesgs when running TM
        {
            delUpdateTextBlock DupdateTextBlock = new delUpdateTextBlock(UpdateTXT);
            delUpdateTape Dtape_update = new delUpdateTape(UpdateTable);
            delEnableBtns DenableBtns = new delEnableBtns(EnableBtns);
            delUpdateHeader DheaderUpdate = new delUpdateHeader(HeaderUpdate);

            int header = -1;
            int count = 0;
            State state = states.Item1[0];


            //this.Dispatcher.BeginInvoke(DheaderUpdate, header, 0);

            Stopwatch timer = new Stopwatch();

            while (termination == false)
            {
                if (header == -1)
                {
                    this.Dispatcher.BeginInvoke(DheaderUpdate, header + 1, 0);
                    header = 0;
                    timer.Start();

                    while (timer.Elapsed < TimeSpan.FromSeconds(time)) ;
                    timer.Stop();
                    timer.Reset();
                }
                else
                {
                    try
                    {
                        if (state.accept /*== states.Item1[(states.Item1.Count) - 1]*/)
                        {
                            this.Dispatcher.BeginInvoke(DupdateTextBlock, "[+] Input was successfuly accepted by the Turing machine .\n");
                            this.Dispatcher.BeginInvoke(DenableBtns);

                            break;
                        }
                        else if(state.reject)
                        {
                            this.Dispatcher.BeginInvoke(DupdateTextBlock, "[+] Input was not successfuly accepted by the Turing machine .\n");
                            this.Dispatcher.BeginInvoke(DenableBtns);

                            break;
                        }

                        string i = tapeData.Rows[0][header].ToString();
                        if (count < InputShowValue)
                        {
                            this.Dispatcher.BeginInvoke(DupdateTextBlock, "[+] Input " + i + " .\n");
                            count += 1;
                            if (count == InputShowValue)
                            {
                                this.Dispatcher.BeginInvoke(DupdateTextBlock, "[+] Input display limit reached (Limit(#Lines)) " + count + " .\n");
                            }
                        }


                        if (state.writeToTape.ContainsKey(i))
                        {
                            tapeData.Rows[0][header] = state.writeToTape[i];
                            this.Dispatcher.BeginInvoke(Dtape_update, tapeData);
                        }

                        int move = (int)state.moveToTape[i];
                        if ((header + move) >= 0)
                        {
                            header += move;
                            if (header > output_len)
                                output_len = header+1;
                            if (header <= tapeData.Columns.Count - 1)
                                this.Dispatcher.BeginInvoke(DheaderUpdate, header, move);
                        }

                        state = state.nextState[i];
                        timer.Start();

                        while (timer.Elapsed < TimeSpan.FromSeconds(time)) ;
                        timer.Stop();
                        timer.Reset();

                    }
                    catch
                    {

                        this.Dispatcher.BeginInvoke(DupdateTextBlock, "[+] There is not declared transition for this input at this state(Failure).\n");
                        this.Dispatcher.BeginInvoke(DenableBtns);
                        timer.Stop();
                        break;
                    }
                }
               

            }
            timer.Stop();
            var ct = new CancellationTokenSource();
            CancellationToken tok = ct.Token;
            Task.Factory.StartNew(() =>
            {
                if (tok.IsCancellationRequested)
                {
                    try
                    {
                        tok.ThrowIfCancellationRequested();
                    }
                    catch
                    {
                        //nothing
                    }
                }
            }, tok);
            try
            {
                ct.Cancel();
            }
            catch(OperationCanceledException) {
                return;
            }
            
        }
        

        private void ClearTape(object sender, RoutedEventArgs e) // NOTE: do not know what happens when we write past the input len
        {
            if (this.tape_data.Columns.Count > 0)
            {

                this.exec.IsEnabled = false;
                this.clearTape.IsEnabled = false;
                //this.tape_data.ColumnHeaderStyle = null;
                //this.tape_data.Columns.Clear();
                if (!_is_running) output_len = input.Length;
                DataRow row = tapeData.Rows[0];
                for (int i = 0; i < output_len; i++)
                {
                    this.tape_data.Columns[i].HeaderStyle = null;
                    row[i] = blank_symbol;
                }

                //tapeData.Rows.Add(row);
                this.tape_data.ItemsSource = tapeData.DefaultView;
                //InitTape();
                this.log.Inlines.Add(new Run("[+] Input removed from the tape .\n"));
                input = null;
            }
        }

        private void StopRun(object sender, RoutedEventArgs e)
        {
            this.stop.IsEnabled = false;
            this.termination = true;
            this.exec.IsEnabled = true;
            this.clearTM.IsEnabled = true;
            this.clearTape.IsEnabled = true;
            this.insertToTape.IsEnabled = true;
            this.exec_speed.IsEnabled = true;
            this.menu.IsEnabled = true;
            this._is_running = false;
            this.log.Inlines.Add(new Run("[+] Turing machine simulation stopped .\n"));

        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            this.window_setting = new Settings(this);
            this.window_setting.Show();
        }
    }
}
