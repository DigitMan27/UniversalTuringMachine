using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UniversalTM
{
    public partial class Transition_Table_Window : Window
    {
        DataTable transitions = new DataTable();
        public string accept_state, reject_state;

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
                        accept_state = states[i].nextState[key].Name;

                    }
                    if (states[i].nextState[key].reject)
                    {
                        reject_state = states[i].nextState[key].Name;
                    }
                    transitions.Rows.Add(row);

                }

            }
            this.transGrid.ItemsSource = transitions.DefaultView;
            /*if (transGrid.Items.Count > 0) MessageBox.Show(transGrid.Items[1].ToString(), "sss");
            DataGridRow r = transGrid.Items[1] as DataGridRow;
            r.Background = Brushes.AliceBlue;*/
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
