using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    class MenuManager
    {
        List<MenuState> _states; // List of menu options

        public MenuManager()
        {
            _states = new List<MenuState>();
            // Slowly but surely
        }

        // Contract: x >= 0, and y >= 0
        public void DisplayMenu(int x, int y)
        {

            // Display top border
            Console.SetCursorPosition(x, y);
            Console.Write("======================");
            foreach (var state in _states)
            {
                Console.SetCursorPosition(x, ++y);
                Console.Write(state.getOption());
            }

            // Display bottom border
            Console.SetCursorPosition(x, ++y);
            Console.Write("======================");
            Console.SetCursorPosition(x, ++y);
            Console.Write("Make a choice: ");

            // Do what the user says to do
            string interactionCode = Console.ReadLine();
            foreach (var state in _states)
            {
                if (state._interactionCode == interactionCode)
                {
                    state.effect?.Invoke();
                }
            }
        }


        public void AddState(MenuState state)
        {
            _states.Add(state);
        }

        public void AddState(string name, string interactionCode)
        {
            _states.Add(new MenuState(name, interactionCode, null));
        }

        public void AddState(string name, string interactionCode, Action effect)
        {
            _states.Add(new MenuState(name, interactionCode, effect));
        }
    }
}

