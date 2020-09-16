using System;
using System.Collections.Generic;
using System.Text;

namespace PeerToPeer
{
   public class ConsoleWrapper
   {
      private readonly object _lockObject = new object();

      public void Output(int x, int y, string message)
      {
         lock (_lockObject)
         {
            Console.SetCursorPosition(x, y);
            Console.Write(message);
         }
      }

      public string Input(int x, int y, string prompt)
      {
         lock (_lockObject)
         {
            Console.SetCursorPosition(x, y);
            Console.Write(prompt);
            return Console.ReadLine();
         }
      }

      public void SetCursorPosition(int x, int y)
      {
         lock (_lockObject)
         {
            Console.SetCursorPosition(x, y);
         }
      }
   }
}
