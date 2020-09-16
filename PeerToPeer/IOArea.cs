using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeerToPeer
{
   public class IOArea
   {
      private readonly ConsoleWrapper _console;

      public IOArea(ConsoleWrapper console)
      {
         this._console = console;
      }

      public char BorderCharacter { get; set; } = '*';
      public void BorderShow(int row, string message, string prompt = "MESSAGE")
      {
         var border = new StringBuilder();
         int y = row;
         for(var x = 0; x < Console.WindowWidth; x++)
         {
            border.Append(BorderCharacter);
         }
         _console.Output(0, y, border.ToString());
         y++;
         _console.Output(1, y, $"{prompt}: {message}");
         y++;
         _console.Output(0, y, border.ToString());
      }

      public void Show(int col, int row, string message, string prompt="MESSAGE")
      {
         _console.Output(col, row, $"{prompt}: {message}");
      }

      public string Input(int x, int y, string prompt)
      {
         return _console.Input(x, y, prompt);
      }
   }
}
