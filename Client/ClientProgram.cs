using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
   class ClientProgram
   {
        // 1. Allocate a buffer to store incoming data
        private readonly byte[] bytes = new byte[1024];

        static void Main(string[] args)
        {
            var app = new ClientProgram(); //Define the program

            // 1. Allocate a buffer to store incoming data
            byte[] bytes = new byte[1024];
            
            try
            {
                // 2. Establish a remote endpoint for the socket
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
                
                // 3. Create the socket
                var sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // 4. Connect the socket to the remote endpoint
                    sender.Connect(remoteEP);
                    Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

                    //Task = more abstract thread -> more scalable
                    Task receiveResponse = Task.Run         //Always listen for a response - task.run creates task and runs it in one operation - used when scheduling isn't needed (only using 2 here)
                    (                                       
                        () => app.ReceiveResponse(sender)   //Takes a Lambda - the function we're attaching to the task that we're using to run the worker thread 
                    ); 
                    
                    string userInput = "";
                    do  //Continues while there is no e - doesn't actually infinitely create new tasks - is blocked by sendRequest.Result
                    {
                        Task<string> sendRequest = new Task<string>
                        (
                            () => app.SendRequest(sender)  
                        );
                        sendRequest.Start();
                        userInput = sendRequest.Result; //this is how you get what is contained in the task (aka how we get the return from the method) - blocks and waits for the result
                    } while (userInput != "E");         //Don't end tasks, let the programmer put some mechanism in the called function to handle how to exit the thread
                    // 9. Close the socket
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (Exception e)
                {
                   Console.WriteLine("Unexpected Exception: {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
               Console.WriteLine("Exception {0}", e.ToString());
            }
        } // Main

        /// <summary>
        /// Loop as long as we're running to get any possible responses
        /// Notes: type is void, since it isn't returning anything - we can load it into task
        /// </summary>
        /// <param name="sender"></param>
        private void ReceiveResponse(Socket sender)
        {
             string response;
             do  //loop until we get a response
             {
                 // 7. Listen for the response (blocking call)
                 int bytesRec = sender.Receive(bytes);        //two lines - responsible for recieving the response - factor out into method - bytes is a socket, so we need to pass sender as a parameter and make bytes sharable
                                                              // 8. Process the response
                 response = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                 Console.WriteLine($"\n{response}");
             } while (response != "Exit");
        
        }

        /// <summary>
        /// Send the request - loops until exit - needs its own thread
        /// Notes: need to add the return type to task
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private string SendRequest(Socket sender)
        {
            string message = "";
            string userInput;

            do
            {
                userInput = GetUserInput();
                switch (userInput)
                {
                    case "1":
                        message = "View<EOF>";
                        break;
                    case "E":
                        message = "Exit<EOF>";
                        break;
                }

                // 5. Encode the data to be sent
                byte[] msg = Encoding.ASCII.GetBytes(message);

                // 6. Send the data through the socket
                sender.Send(msg);         //Still blocked, this is where we're sending
            } while (userInput != "E");

            return userInput;
        }

        private string GetUserInput()
        {
           string userInput;
           do
           {
              Console.WriteLine("======================");
              Console.WriteLine("1. View Map           ");
              Console.WriteLine("E. Exit               ");
              Console.WriteLine("======================");
              Console.Write("Make a choice:");
              userInput = Console.ReadLine();
           } while (userInput != "1" && userInput != "E");
           return userInput;
        }

        /*
         Other Notes:
        
            task is a thread at a higher lever - more scalable

            has a lot of nice functions included 
            we have to do some exception handling
            
            we can do custom scheduling among other things
            
            how do we create and run tasks?
            
            set up client program to be more or less asynchronous - we're gonna have the client continuously listening for the server
            we want to be listening on one thread and sending requests on another thread
        */
    }
}
