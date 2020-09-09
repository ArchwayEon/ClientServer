using System;   //allows access to System functions and objects.

namespace Client
{
    /// <summary>
    /// Encapsulates functions related to writing messages for the client.
    /// </summary>
	class ClientMessageWriter
    {
        /// <summary>
        /// Writes a given message at given location.
        /// </summary>
        /// <param name="Col">Starting column to print from</param>
        /// <param name="Row">Starting row to print on</param>
        /// <param name="BorderChar">Character used to generate borders.</param>
        /// <param name="Prompt">Prompt preceeding message.</param>
        /// <param name="Message">Given message to display.</param>
        public void WriteMessage(string Message, byte Col = 0, byte Row = 0, char BorderChar = '*', string Prompt = "MESSAGE")
        {
            var BorderLine = String.Concat(Enumerable.Repeat(BorderChar, Console.WindowWidth)); //Generate the line of border characters
            var MessageLine = Prompt+ ":\t" + Message;                                          //Construct the message
            
            Console.SetCursorPosition(Col, Row);    //set the display position

            // Print out the constructed message with borders.
            Write(BorderLine, MessageLine);
        }//WriteMessage(byte, byte, char, string, string)

    }//ClientMessageWriter class

    /// <summary>
    /// Helper function to encapsulate printing to console.
    /// </summary>
    /// <param name="Border">Border string</param>
    /// <param name="Message">Message string</param>
    private void Write(string Border, string Message)
    {
        Console.WriteLine($"{Border}\n{Message}\n{Border}");
    }//Write(string, string)
}//client namespace