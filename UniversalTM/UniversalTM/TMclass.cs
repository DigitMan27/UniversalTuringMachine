using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UniversalTM
{
    public enum Flags
    {
        NO_STATES = 1,
        NO_DESCRIPTION = 2,
        SUCCESS=0
    }
    class TMclass
    {
        private string filename;
        public TMclass(string filename)
        {
            this.filename = filename;
        }

        public Tuple<List<State>,Flags,int> TM()
        {
            string[] lines = System.IO.File.ReadAllLines(this.filename);
            List<State> states = new List<State>();
            int line_count = -1;
            try 
            {
                string s = lines[0];
                string pattern = "(:)";
                string[] ss = Regex.Split(s, pattern);
                string[] ss1 = Regex.Split(ss[2], ",");
                foreach (string s1 in ss1)
                {
                    State state = new State(s1);
                    state.Name = s1;
                    states.Add(state);
                }

            }
            catch
            {
                return Tuple.Create(new List<State>() { }, Flags.NO_STATES, 1);
            }
            int ind = 1;
            if (states.Count != 0)
            {
                foreach (string line in lines)
                {
                    line_count += 1;
                    
                    if (ind == 1 || line == null)
                    {
                        ind = 0;
                        continue;
                    }
                    else if (!string.IsNullOrEmpty(line.Trim()))
                    {
                        List<string> cu = new List<string>();
                        System.Console.WriteLine("tokens");
                        try
                        {
                            string[] s_line = Regex.Split(line, "->");
                            for (int i = 0; i < s_line.Length; i++)
                            {
                                string[] tok = Regex.Split(s_line[i], ",");
                                foreach (string tok_s in tok)
                                {
                                    cu.Add(tok_s);
                                }
                            }
                        }catch
                        {
                            return Tuple.Create(new List<State>() { }, Flags.NO_DESCRIPTION, line_count);
                        }
                        State state = states.Find(o => o.Name.Equals(cu[0]));
                        System.Console.WriteLine("State: ", state.getName());
                        string input = cu[1];
                        if (input == string.Empty)
                        {
                            return Tuple.Create(new List<State>() { }, Flags.NO_DESCRIPTION, line_count);
                        }
                
                        if (cu.Count == 4)
                        {
                            string move;
                            State Nextstate;
                            move = cu[2];
                            Nextstate = states.Find(o => o.Name == cu[3]);
                            state.setMove2Tape(input, move);
                            state.setNextState(input, Nextstate);

                        }
                        else if (cu.Count == 5)
                        {
                            string output, move;
                            State Nextstate;
                            output = cu[2];
                            move = cu[3];
                            Nextstate = states.Find(o => o.Name == cu[4]);
                            state.setWrite2Tape(input, output);
                            state.setMove2Tape(input, move);
                            state.setNextState(input, Nextstate);
                        }
                    }
                    
                }
                
            }

            return Tuple.Create(states, Flags.SUCCESS, 0);
        }
    }
}
