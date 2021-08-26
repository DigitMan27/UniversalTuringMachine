using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UniversalTM
{
    public enum Flags
    {
        NO_STATES = 1, // Occurs if 1st line is not of the form States:q1,q2,q3 ... or if the second line of stating the accept state is missing .
        NO_DESCRIPTION = 2, // Occurs if there are not or are wrong typed the transition rules of the turing machine .
        NO_ACCEPT = 3,
        NO_REJECT = 4,
        SAME_STATE = 5,
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
            bool statesDefined = false;
            bool acceptStateDefined = false;
            bool rejectStateDefined = false;
            
            foreach (string line in lines)
            {
                line_count += 1;

                if (line == null || Regex.IsMatch(line, "(\\s)*[\\\\]([\\s*\\w*\\s* | \\s*])*")) // single line comments
                {
                    continue;
                }
                else if (Regex.IsMatch(line, "(\\s)*[States:](\\s)*[\\w+](\\s*,\\s*\\w+)*\\s*") && statesDefined == false) // states
                {
                    try
                    {
                        string s = line;
                        string pattern = "(\\s)*[States:](\\s)*";
                        string[] ss = Regex.Split(s.Trim(), pattern);
                        string[] ss1 = Regex.Split(ss[ss.Length - 1], "\\s*,\\s*");
                        foreach (string s1 in ss1)
                        {
                            State state = new State(s1);
                            state.Name = s1;
                            states.Add(state);
                        }
                        if (states.Count == 1) throw new Exception();
                        statesDefined = true;
                    }
                    catch
                    {
                        return Tuple.Create(new List<State>() { }, Flags.NO_STATES, line_count);
                    }
                } else if (!Regex.IsMatch(line, "(\\s)*[States:](\\s)*[\\w+](\\s*,\\s*\\w+)*\\s*") && statesDefined == false) { 
                    return Tuple.Create(new List<State>() { }, Flags.NO_STATES, line_count); 
                } 
                else if (Regex.IsMatch(line, "(\\s)*[Accept:](\\s)*[\\w+]\\s*") && statesDefined == true && acceptStateDefined == false) // accept state
                {
                    try
                    {
                        string accept_state_line = line;
                        string pattern = "[\\s*Accept:\\s*]";
                        string[] accept_state_split = Regex.Split(accept_state_line.Trim(), pattern);
                        if (string.IsNullOrEmpty(accept_state_split[accept_state_split.Length - 1]))
                        {
                            throw new Exception();
                        }
                        else
                        {
                            states.Find(o => o.Name.Equals(accept_state_split[accept_state_split.Length - 1])).accept = true;
                        }
                        acceptStateDefined = true;

                    }
                    catch
                    {
                        return Tuple.Create(new List<State>() { }, Flags.NO_ACCEPT, line_count);
                    }
                }
                else if (!Regex.IsMatch(line, "(\\s)*[Accept:](\\s)*[\\w+]\\s*") && acceptStateDefined == false)
                {
                    return Tuple.Create(new List<State>() { }, Flags.NO_ACCEPT, line_count);
                }
                else if (Regex.IsMatch(line, "(\\s)*[Reject:](\\s)*[\\w+]\\s*") && statesDefined == true && acceptStateDefined == true && rejectStateDefined == false) // reject state
                {
                    try
                    {
                        string reject_state_line = line;
                        string pattern = "[\\s*Reject:\\s*]";
                        string[] reject_state_split = Regex.Split(reject_state_line.Trim(), pattern);
                        if (string.IsNullOrEmpty(reject_state_split[reject_state_split.Length - 1]))
                        {
                            throw new Exception();
                        }
                        else
                        {
                            bool v1 = states.Find(o => o.Name.Equals(reject_state_split[reject_state_split.Length - 1])).accept;
                            bool v2 = states.Find(o => o.Name.Equals(reject_state_split[reject_state_split.Length - 1])).reject = true;
                            try
                            {
                                if (v1==true && v2==true)
                                {
                                    throw new Exception();
                                }
                            }
                            catch(Exception)
                            {
                                return Tuple.Create(new List<State>() { }, Flags.SAME_STATE, line_count);
                            }

                        }
                        rejectStateDefined = true;

                    }
                    catch(Exception)
                    {
                        return Tuple.Create(new List<State>() { }, Flags.NO_REJECT, line_count);
                    }
                }
                else if (!Regex.IsMatch(line, "(\\s)*[Reject:](\\s)*[\\w+]\\s*") && rejectStateDefined == false)
                {
                    return Tuple.Create(new List<State>() { }, Flags.NO_REJECT, line_count);
                }
                else if (!string.IsNullOrEmpty(line.Trim()) && statesDefined && acceptStateDefined && rejectStateDefined && states.Count > 1)
                {
                    try
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
                        }
                        catch
                        {
                            return Tuple.Create(new List<State>() { }, Flags.NO_DESCRIPTION, line_count);
                        }
                        State state = states.Find(o => o.Name.Equals(cu[0]));

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
                    catch (Exception)
                    {
                        return Tuple.Create(new List<State>() { }, Flags.NO_DESCRIPTION, line_count);
                    }
                }
                    
            }
                
            if (states.Count > 0 && acceptStateDefined && rejectStateDefined)
            {
                return Tuple.Create(states, Flags.SUCCESS, 0);
            }
            else
            {
                return Tuple.Create(new List<State>() { }, Flags.NO_STATES, line_count);
            }
            
        }
    }
}
