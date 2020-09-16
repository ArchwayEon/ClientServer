using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
   public class ConsoleMenu
   {
      private readonly List<string> _options = new List<string>();
      private string Prompt { get; set; } = "Make a choice: ";

      private int _x, _y;
      private readonly ConsoleWrapper _console;

      public ConsoleMenu(ConsoleWrapper console)
      {
         this._console = console;
      }

      public void Show(int x, int y)
      {
         _console.Output(x, y, "======================");
         foreach(var option in _options)
         {
            y++;
            _console.Output(x, y, option);
         }
         y++;
         _console.Output(x, y, "======================");
         y++;
         _console.Output(x, y, Prompt);
         _x = x + Prompt.Length;
         _y = y;
      }

      public void AddOption(string option)
      {
         _options.Add(option);
      }

      public void PositionCursor()
      {
         Console.SetCursorPosition(_x, _y);
      }
   }
}
