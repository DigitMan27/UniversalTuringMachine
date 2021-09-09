using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UniversalTM
{
    public enum Flags
    {
        NO_STATES = 1, // Occurs if 1st line is not of the form States:q1,q2,q3 ... or if the second line of stating the accept state is missing .
        NO_DESCRIPTION = 2, // Occurs if there are not or are wrong typed the transition rules of the turing machine .
        NO_ACCEPT = 3, // no accept state defined
        NO_REJECT = 4, // no reject state defined
        SAME_STATE = 5, // accept state = reject state
        NO_INPUT=6, // Alphabet not declared or contains blank symbol
        UNKNOWN_STATE=7, // The machine uses an undefined state
        WRONG_TRANSITION = 8, // triggers when accept or reject state have transitions to other states which is forbidden by the definition of the Turing Machine
        SUCCESS=0 // success .
    }
    class TMclass
    {
        private string filename;
        private string blankSymbol;
        public TMclass(string filename,string blank)
        {
            this.filename = filename;
            this.blankSymbol = blank;
        }

        public Tuple<List<State>,Flags,int, List<string>> TM()
        {
            string[] lines = System.IO.File.ReadAllLines(this.filename);
            List<string> declaredInput = new List<string>();
            List<State> states = new List<State>();
            int line_count = -1;
            bool statesDefined = false;
            bool acceptStateDefined = false;
            bool rejectStateDefined = false;
            bool inputDefined = false;
            
            foreach (string line in lines)
            {
                line_count += 1;

                if (string.IsNullOrEmpty(line) || Regex.IsMatch(line, "(\\s)*[\\\\]([\\s*\\w*\\s* | \\s*])*")) // single line comments
                {
                    continue;
                }
                else if (Regex.IsMatch(line, "^\\s*States:\\s*\\w+(\\s*,\\s*\\w+)*\\s*$") && statesDefined == false) // states
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
                        return Tuple.Create(new List<State>() { }, Flags.NO_STATES, line_count, declaredInput);
                    }
                }
                else if (!Regex.IsMatch(line, "^\\s*States:\\s*\\w+(\\s*,\\s*\\w+)*\\s*$") && statesDefined == false)
                {
                    return Tuple.Create(new List<State>() { }, Flags.NO_STATES, line_count, declaredInput);
                }
                else if (Regex.IsMatch(line, "^\\s*Accept:\\s*\\w+\\s*$") && statesDefined == true && acceptStateDefined == false) //Accept
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
                        return Tuple.Create(new List<State>() { }, Flags.NO_ACCEPT, line_count, declaredInput);
                    }
                }
                else if (!Regex.IsMatch(line, "^\\s*Accept:\\s*\\w+\\s*$") && acceptStateDefined == false)
                {
                    return Tuple.Create(new List<State>() { }, Flags.NO_ACCEPT, line_count, declaredInput);
                }
                else if (Regex.IsMatch(line, "^\\s*Reject:\\s*\\w+\\s*$") && statesDefined == true && acceptStateDefined == true && rejectStateDefined == false) //Reject
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
                                if (v1 == true && v2 == true)
                                {
                                    throw new Exception();
                                }
                            }
                            catch (Exception)
                            {
                                return Tuple.Create(new List<State>() { }, Flags.SAME_STATE, line_count, declaredInput);
                            }

                        }
                        rejectStateDefined = true;

                    }
                    catch (Exception)
                    {
                        return Tuple.Create(new List<State>() { }, Flags.NO_REJECT, line_count, declaredInput);
                    }
                }
                else if (!Regex.IsMatch(line, "^\\s*Reject:\\s*\\w+\\s*$") && rejectStateDefined == false)
                {
                    return Tuple.Create(new List<State>() { }, Flags.NO_REJECT, line_count, declaredInput);
                }
                else if (Regex.IsMatch(line, pattern: "^\\s*Input:\\s*(\\w||[#%^$@!&*])(\\s*,\\s*(\\w||[#%^$@!&*]))*\\s*$") && statesDefined && acceptStateDefined && rejectStateDefined && inputDefined == false)
                {
                    try
                    {
                        string input_state_line = line;
                        string pattern = "[\\s*Input:\\s*]";
                        string[] input_state_split = Regex.Split(input_state_line.Trim(), pattern);
                        string[] input_split = Regex.Split(input_state_split[input_state_split.Length - 1], "\\s*,\\s*");
                        foreach (string input in input_split)
                        {
                            if (input.Equals(blankSymbol))
                                throw new Exception();
                            declaredInput.Add(input);
                        }
                        inputDefined = true;
                    }
                    catch
                    {
                        return Tuple.Create(new List<State>() { }, Flags.NO_INPUT, line_count, declaredInput);
                    }
                }
                else if (!Regex.IsMatch(line, pattern: "^\\s*Input:\\s*(\\w||[#%^$@!&*])(\\s*,\\s*(\\w||[#%^$@!&*]))*\\s*$") && inputDefined == false)
                {
                    return Tuple.Create(new List<State>() { }, Flags.NO_INPUT, line_count, declaredInput);
                }
                else if (Regex.IsMatch(line, "^\\s*\\w+,[\\w|\\W]->\\s*([\\w|\\W],)?[R|L|N],\\w+\\s*$") && statesDefined && acceptStateDefined && rejectStateDefined && inputDefined && states.Count > 1) // look these more 
                {
                    try
                    {
                        List<string> cu = new List<string>();
                        
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
                            return Tuple.Create(new List<State>() { }, Flags.NO_DESCRIPTION, line_count, declaredInput);
                        }

                        try
                        {
                            State state = states.Find(o => o.Name.Equals(cu[0]));
                            if (state == null)
                                throw new Exception();
                            try
                            {
                                if (state.accept || state.reject) // final states must not lead to other states (cause there are final states)
                                    throw new Exception();
                            }
                            catch
                            {
                                return Tuple.Create(new List<State>() { }, Flags.WRONG_TRANSITION, line_count, declaredInput);
                            }
                            

                        
                        

                            string input = cu[1];
                            if (input == string.Empty)
                            {
                                return Tuple.Create(new List<State>() { }, Flags.NO_DESCRIPTION, line_count, declaredInput);
                            }

                            if (cu.Count == 4)
                            {
                                string move;
                                State Nextstate;
                                move = cu[2];
                                try
                                {
                                    Nextstate = states.Find(o => o.Name.Equals(cu[3], StringComparison.Ordinal));
                                    if (Nextstate == null)
                                        throw new Exception();
                                }
                                catch
                                {
                                    return Tuple.Create(new List<State>() { }, Flags.UNKNOWN_STATE, line_count, declaredInput);
                                }
                                state.setMove2Tape(input, move);
                                state.setNextState(input, Nextstate);

                            }
                            else if (cu.Count == 5)
                            {
                                string output, move;
                                State Nextstate;
                                output = cu[2];
                                move = cu[3];
                                try
                                {
                                    Nextstate = states.Find(o => o.Name.Equals(cu[4], StringComparison.Ordinal));
                                    if (Nextstate == null)
                                        throw new Exception();
                                }
                                catch {
                                    return Tuple.Create(new List<State>() { }, Flags.UNKNOWN_STATE, line_count, declaredInput);
                                }
                            
                                state.setWrite2Tape(input, output);
                                state.setMove2Tape(input, move);
                                state.setNextState(input, Nextstate);
                            }
                        }
                        catch
                        {
                            return Tuple.Create(new List<State>() { }, Flags.UNKNOWN_STATE, line_count, declaredInput);
                        }
                    }
                    catch (Exception)
                    {
                        return Tuple.Create(new List<State>() { }, Flags.NO_DESCRIPTION, line_count, declaredInput);
                    }
                }
                else if (!Regex.IsMatch(line, "^\\s*\\w+,[\\w|\\W]->\\s*([\\w|\\W],)?[R|L|N],\\w+\\s*$") && statesDefined && acceptStateDefined && rejectStateDefined && states.Count > 1)
                {
                    return Tuple.Create(new List<State>() { }, Flags.NO_DESCRIPTION, line_count, declaredInput);
                }
                    
            }
                
            if (states.Count > 0 && acceptStateDefined && rejectStateDefined)
            {
                return Tuple.Create(states, Flags.SUCCESS, 0, declaredInput);
            }
            else
            {
                return Tuple.Create(new List<State>() { }, Flags.NO_STATES, line_count, declaredInput);
            }
            
        }
    }
}
