using System;
using System.Collections.Generic;
using System.Text;

namespace Client
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

      public string Input()
      {
         lock (_lockObject)
         {
            return Console.ReadLine();
         }
      }
   }
}
