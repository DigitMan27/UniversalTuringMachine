using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace UniversalTM
{

    public class FinalStatesDecl
    {
        private static string a, r;
        public static string accept_state { get { return a; } set { a = value; } }
        public static string reject_state { get { return r; } set { r = value; } }
    }

    public class SimpleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value.ToString() == FinalStatesDecl.accept_state)
            {
                return true;
            }else if(value.ToString() == FinalStatesDecl.reject_state)
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("SimpleConverter is a OneWay converter.");
        }
    }
    public partial class Transition_Table_Window : Window
    {
        DataTable transitions = new DataTable();

        public Transition_Table_Window(List<State> states)
        {
            
            InitializeComponent();
            this.CreateTransitionTable(states);
        }

        private void CreateTransitionTable(List<State> states)
        {

            string[] columns = { "Current State", "Input", "Write", "Move", "Next State" };
            
            foreach (string col in columns)
            {
                transitions.Columns.Add(new DataColumn(col, typeof(string)));
            }
            for (int i = 0; i < states.Count; i++)
            {
                string[] keys = states[i].nextState.Keys.ToArray();
                //isAccept.Accept = "";
                foreach (string key in keys)
                {
                    DataRow row = transitions.NewRow();
                    row[0] += states[i].Name;
                    row[1] += key;
                    row[2] += (states[i].writeToTape.ContainsKey(key)) ? states[i].writeToTape[key] : "";
                    row[3] += states[i].moveToTape[key].ToString();
                    row[4] += states[i].nextState[key].Name;

                    if (states[i].nextState[key].accept)
                    {
                        FinalStatesDecl.accept_state = states[i].nextState[key].Name;
                    }
                    if (states[i].nextState[key].reject)
                    {
                        FinalStatesDecl.reject_state = states[i].nextState[key].Name;
                    }
                    transitions.Rows.Add(row);

                }

            }
            this.transGrid.ItemsSource = transitions.DefaultView;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
