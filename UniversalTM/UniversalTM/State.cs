using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalTM
{
    class State
    {
        public enum Move
        {
            LEFT = -1,
            RIGHT = 1,
            NOP = 0
        }
        private string name;
        public bool accept { get; set; } = false; // true if state is accept state
        public bool reject { get; set; } = false;
        public Dictionary<string, string> writeToTape = new Dictionary<string, string>();
        public Dictionary<string, Move> moveToTape = new Dictionary<string, Move>();
        public Dictionary<string, State> nextState = new Dictionary<string, State>();
        public string Name { get { return name; } set { name = value; } }
        public State(string name)
        {
            this.name = name;
        }

        public void setWrite2Tape(string input_symbol, string output_symbol)
        {
            if (!this.writeToTape.ContainsKey(input_symbol))
            {
                this.writeToTape[input_symbol] = output_symbol;
            }
        }
        public void setMove2Tape(string input_symbol, string move)
        {
            if (!this.moveToTape.ContainsKey(input_symbol))
            {
                if (move == "L")
                {
                    this.moveToTape[input_symbol] = Move.LEFT;
                }
                else if (move == "R")
                {
                    this.moveToTape[input_symbol] = Move.RIGHT;
                }
                else if (move == "N")
                {
                    this.moveToTape[input_symbol] = Move.NOP;
                }

            }
        }

        public void setNextState(string input_symbol, State s)
        {
            if (!this.nextState.ContainsKey(input_symbol))
            {
                this.nextState[input_symbol] = s;
            }
        }

        public string getName()
        {
            return this.name;
        }

        public static implicit operator State(bool v)
        {
            throw new NotImplementedException();
        }
    }
}
