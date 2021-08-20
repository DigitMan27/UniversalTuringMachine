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
        NO_STATES = 1, // Occurs if 1st line is not of the form States:q1,q2,q3 ... or if the second line of stating the accept state is missing .
        NO_DESCRIPTION = 2, // Occurs if there are not or are wrong typed the transition rules of the turing machine .
        SUCCESS=0 // success .
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
                string[] ss = Regex.Split(s.Trim(), "[States]"+pattern);
                string[] ss1 = Regex.Split(ss[2], ",");
                foreach (string s1 in ss1)
                {
                    State state = new State(s1);
                    state.Name = s1;
                    states.Add(state);
                }
                string accept_state_line = lines[1];
                string[] accept_state_split = Regex.Split(accept_state_line.Trim(), "[Accept]"+pattern);
                if (accept_state_split.Length != 3)
                {
                    line_count += 1;
                    throw new Exception();
                }else if(accept_state_split.Length == 3)
                {
                    states.Find(o => o.Name.Equals(accept_state_split[2])).finish = true;
                }
                

            }
            catch
            {
                return Tuple.Create(new List<State>() { }, Flags.NO_STATES, line_count+1);
            }
            
            if (states.Count != 0)
            {
                foreach (string line in lines)
                {
                    line_count += 1;
                    
                    if (line_count<2 || line == null)
                    {
                       
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
