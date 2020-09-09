using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class MenuState
    {
        // Bad habit, but it works
        public string _optionName; // Long name of the menu option
        public string _interactionCode; // What the user types in the menu to select
        public Action effect; // Function to enact

        public MenuState(string name, string interactionCode, Action effect)
        {
            this._optionName = name;
            this._interactionCode = interactionCode;
            this.effect = effect;
        }

        public string getOption()
        {
            return _interactionCode + ". " + _optionName;
        }

        public void DoTheThing() { effect?.Invoke(); }
    }
}
