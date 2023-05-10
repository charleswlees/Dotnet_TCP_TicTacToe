
/*
 * Charlie Lees
 * CS5700 - HW4 TicTacToe
 * This program runs in two different modes, as a "server" listening for a connection 
 * and as a "client" which looks for a server to connect to.
 * Once connected the two sessions play tic tac toe with each other. 
 * 
 * Once the game ends, each client will be asked if they want to replay.
 * Either client can quit the game at any time.
 * 
 * 
 * Compiling the Code
 * To compile this code dotnet is required, 
 * I have provided all required materials to install dotnet on your instance of the Khoury Server
 * Please see my HW4 README for instructions
 * 
 * Once dotnet is installed
 * To run the server run the following:
 * dotnet run -S 
 * Then in another shell, replace the -S flag with -C to run the client
 * dotnet run -C
 * There are other flags to specify the port and domain, please see the README for more information
 */


using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
class TicTacToe
{

    

    /*
     * processArgs - Returns an array of strings indicating which flags were given and what values are tied to them. 
     * If flags are used improperly, then exceptions will be thrown. 
     * args - String[] - Command line arguents provided
     */
     public static String[] processArgs(String[] args){
        //Variables which store the flags used, and any values given
        String server = "0";
        String client = "0";
        String port = "0";
        String portVal = "";
        String domain = "0";
        String domainVal = "";
        String[] flags = new String[]{server, client, port, portVal, domain, domainVal};

        String current = "";//Helper variable for iteration


        for(int i = 0; i<args.Length; i++){
            current = args[i];//Current argument
            //Checks which flag it is using and updates variables as needed
            if(current == "-S"){
                //Verifies that the flag given has not already been given
                if(flags[0]=="1")throw new ArgumentException("Cannot give more than one of the same flag");
                flags[0]="1";
                }
            else if(current == "-C"){
                //Verifies that the flag given has not already been given
                if(flags[1]=="1")throw new ArgumentException("Cannot give more than one of the same flag");
                flags[1]="1";
                }
            else if(current == "-P"){
                //Verifies that port is specified after using port flag
                if((i+1<args.Length)){
                    //Verifies that this flag has not already been given
                    if(flags[2]=="1")throw new ArgumentException("Cannot give more than one of the same flag");
                    //Verifies that the next index is indeed a port and not another flag
                    if(args[i+1] == "-S"|| args[i+1] == "-C"|| args[i+1] == "-P"|| args[i+1] == "-D"){
                        throw new ArgumentException("Port flag was specified, but no port was given");
                    }
                    flags[2] = "1";
                    flags[3] = args[++i];
                }
                else{
                    throw new ArgumentException("Port flag was specified, but no port was given");
                }
            }
            else if(current == "-D"){
                //Verifies that domain is specified after using domain flag
                if(i+1<args.Length){
                    //Verifies that this flag has not already been given
                    if(flags[4]=="1")throw new ArgumentException("Cannot give more than one of the same flag");
                    //Verifies that the next index is indeed a domain and not another flag
                    if(args[i+1] == "-S"|| args[i+1] == "-C"|| args[i+1] == "-P"|| args[i+1] == "-D"){
                        throw new ArgumentException("Domain flag was specified, but no domain was given");
                    }
                    flags[4] = "1";
                    flags[5] = args[++i];
                }
                else{
                    throw new ArgumentException("Domain flag was specified, but no domain was given");
                }
            }
            
        }



        return flags;
     }


    /*
    initializeGameBoard - initializes every spot on the game board
    gameBoard - int[,] - 2D array representing the game board
    This allows us to see which spots haven't been taken yet
    */
    public static void initializeGameBoard(int[,]gameBoard){
        //Iterates through the entire board
        for(int i = 0; i<3; i++){
            for(int k = 0; k<3; k++){
                gameBoard[i, k] = 99;//Sets each spot on the board to 99
            }
        }

    }



    /*
    processMove - Takes in a move as a string, parses it's coordinates, and then alters the state
    move - String - The move in question, two valid coordinates (between 0 and 2) separated by a comma
    player - int - 0 for X, and 1 for O.
    gameboard - int[,] - a 2 dimensional array representing the gameboard
    Returns an integer for whether the move was process successfully or not
    0 for a success or 1 for failure
    */
    public static int processMove(String move, int[,]gameBoard, int player){
        //Step 1: Parse the move from the string
            //If the string does not contain 2 valid numbers... throw an error
        String[] split = move.Split(",");
        int x = 99;
        int y = 99;
        try{
        if(split.Length != 2){
            throw new ArgumentException("Coordinates can only be x,y where x and y are integers");
        }
           if(!Int32.TryParse(split[0], out x))throw new ArgumentException("Coordinates must be integers");
           if(!Int32.TryParse(split[1], out y))throw new ArgumentException("Coordinates must be integers");

           if(x>2)throw new ArgumentException("Coordinates must be between 0 and 2");
           if(y>2)throw new ArgumentException("Coordinates must be between 0 and 2");


        //Step 2: Make sure that the given coordinates have not already been selected
            //If they have... throw an error
        if(gameBoard[x,y]!=99)throw new ArgumentException("Given index is already taken");

        //Step 3: Alter game-board with the respective icon
        else gameBoard[x,y]=player;
        }
        catch(ArgumentException e){
            Console.WriteLine(e.Message);
            return 1;
        }
        catch(Exception e){
            Console.WriteLine(e);
            return 1;
        }

            
        return 0;
    }
    

    /*
    formatValues - Takes the 0/1 player value and 99 placeholder value, and returns a nice string
    value - int - converts the representation of each player into the String representation
    Returns the String representation of either player or if the space has not been taken
    */
    public static String formatValues(int value){
        switch(value){
            case 0: return "X";
            case 1: return "O";
            case 99: return "-";
        }
        return "ERROR";

    }

    /*
     * gameState - returns the current state of the game, whether it is still ongoing or if it has ended.
     * gameBoard - int[,] - the 2D array representing the game board
     * returns the winning player(0 or 1), 2 if the board is full, and -1 if the game hasn't ended
     */
    public static int gameState(int[,]gameBoard){
        int openSpots = 0;//Stores how many open spots on the board
        int winner = -1;//Return value

        //Look through all y axis spots for win conditions 
        //Also check if the board is full
        for(int x = 0; x<3; x++){
            for(int y = 0; y<3; y++){
                if(gameBoard[x,y]==99)openSpots++;
                //Check for X axis victory
                if(x==0){
                    //Check for winning condition
                    if(gameBoard[x,y]==gameBoard[x+1,y]&&gameBoard[x,y]==gameBoard[x+2,y]){
                        //return winner
                        if(gameBoard[x,y]!=99)return gameBoard[x,y];
                    }
                }
                //Check for Y axis victory
                if(y==0){
                    //Check for winning condition
                    if(gameBoard[x,y]==gameBoard[x,y+1]&&gameBoard[x,y]==gameBoard[x,y+2]){
                        //return winner
                        if(gameBoard[x,y]!=99)return gameBoard[x,y];
                    }
                }
            }
        }

        //Check for Diagonal Victories
        if(gameBoard[0,0]==gameBoard[1,1]&&gameBoard[0,0]==gameBoard[2,2]&&gameBoard[0,0]!=99)return gameBoard[0,0];
        if(gameBoard[0,2]==gameBoard[1,1]&&gameBoard[0,2]==gameBoard[2,0]&&gameBoard[0,2]!=99)return gameBoard[0,2];


        //Check if the board is full
        if(openSpots==0)return 2;
        //Default return
        return winner;
    }


    /*
     * printBoard - prints the current game board to the console
     * gameBoard - int[,] - the 2D array that represents the game board
     */
    public static void printBoard(int[,]gameBoard){

        Console.WriteLine("     |     |    ");
        Console.WriteLine($"  {formatValues(gameBoard[0,2])}  |  {formatValues(gameBoard[1,2])}  |  {formatValues(gameBoard[2,2])} ");
        Console.WriteLine("_____|_____|_____");
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {formatValues(gameBoard[0,1])}  |  {formatValues(gameBoard[1,1])}  |  {formatValues(gameBoard[2,1])} ");
        Console.WriteLine("_____|_____|_____");
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {formatValues(gameBoard[0,0])}  |  {formatValues(gameBoard[1,0])}  |  {formatValues(gameBoard[2,0])} ");
        Console.WriteLine("     |     |     ");
    }


/*
     * boardToString - Returns a string representation of the board
     * gameBoard[] - int[,] - Representation of the game board
     * Return - A string representation of the game state
     * Format for gameboard:
     *
             |     |     
          6  |  7  |  8
        _____|_____|_____
             |     |     
          3  |  4  |  5  
        _____|_____|_____
             |     |     
          0  |  1  |  2
             |     |     
     * The message will be a comma delimited list of 0, 1, and 99
     * 0 is X, 1 is O, and 99 is empty 
     */
    public static String boardToString(int[,]gameBoard){

        String gameState = "";
        //Iterate through the board and add it's state to a string comma delimited
        for(int y = 0; y<3; y++){
            for(int x = 0; x<3; x++){
                //Don't include a comma after the final value
                if(y==2&&x==2)gameState+=$"{gameBoard[x,y]}";
                else{gameState+=$"{gameBoard[x,y]},";}
            }
        }

        return gameState;
    }

    /*
     * stateMatch - takes in a gameState from the opponent and verifies that they match
     * opponentGameState - String - game state of the opponent's board
     * gameBoard - int[,] - 2D array representing the current game board
     * Returns a 1 if they match, returns a -1 if they don't
     */
    public static int stateMatch(String opponentGameState, int[,]gameBoard){
        //Get the String representation of our current game board
        String localGameState = boardToString(gameBoard);
        //Parse the individual values out of both states
        String[] values = localGameState.Split(',',9);
        String[] opponentValues = opponentGameState.Split(',', 9);
        //If invalid game states were passed in, throw exception
        if(values.Length!=9 || opponentValues.Length!=9){throw new ArgumentException("Invalid move given");}
        int differences = 0;//Number of differences between the game states
        //Iterate through both states and count the differences
        for(int x = 0; x<values.Length; x++){
            if(values[x] != opponentValues[x])differences++;
        }
        //If there is only 1 difference
        if(differences==1)return 1;
        //If they are the same
        else if(differences == 0)return 0;
        return -1;

    }
    /*
     * parseMove - takes in a valid game state and parses out the move which was done
     * opponentGameState - String - the game state given from the opponent
     * gameBoard - int[,] - the 2D array representing the game board
     * Return - a string containing the x,y coordinates of the move
     */
    public static String parseMove(String opponentGameState, int[,]gameBoard){
        //String representation of our local games state
        String localGameState = boardToString(gameBoard);
        //Splitting our game states into arrays
        String[] values = localGameState.Split(',',9);
        String[] opponentValues = opponentGameState.Split(',', 9);
        int yVal = 0;//y coordinate
        int xVal = 0;//x coordinate
        for(int x = 0; x<values.Length; x++){
            //As this is just a 1D array, these adjustments allow us to keep track of our axis
            if(x == 3||x==6){
                yVal++;
                xVal = 0;
            }
            //If the move is found, return the coordinates
            if(values[x] != opponentValues[x])return $"{xVal},{yVal}";
            xVal++;

        }
        //Default return
        return "No move found";
    }


    /*
     * Main function of the program, processes the game and handles the TCP connection
     */
    static async Task Main(string[] args)
    {
        //TicTacToe Functionality
        //Store the game state in a 3x3 2D array
        //Each message sent and received should be coordinates:
        /*
                     |     |     
                 0,2 | 1,2 | 2,2 
                _____|_____|_____
                     |     |     
                 0,1 | 1,1 | 2,1  
                _____|_____|_____
                     |     |     
                 0,0 | 1,0 | 2,0  
                     |     |     
        */
        int[,] gameBoard= new int[3,3];//Stores the game state
        initializeGameBoard(gameBoard);
        int moveResult=99;//stores whether the move was a success
        int stateOfGame = -1; //Stores the state of the game, output of gameState
        Boolean done = false;//stores whether or not the player wants to play again 
        int player = -99;//Stores whether we're using X's or O's. 1 for O and 0 for X
        int opponent = -99;//Stores if your opponent is using X or O
        String[] flags = new String[6];//Stores processed Command Line Arguments
        IPAddress ipAddress;//Stores the user's IP information
        Boolean turn = false; //Stores whether the current turn is done
        try{
            //Processing arguments and making sure they are used properly
            //An exception will be thrown if a port or domain is not provided after using those flags
            flags = processArgs(args);
 

        //Checks if Server/Client flags were both used, or not used at all
        if(flags[0] == flags[1])throw new ArgumentException($"Either -S or -C must be used. However, you cannot use them both. ");
        
        
        Console.WriteLine("Welcome to Tic Tac Toe!");
        Console.WriteLine("Once a connection is established, please enter moves as comma delimited coordinates");
        Console.WriteLine($"Where 0,0 is the bottom left and 2,2 is the top right\n");
        

        /* Command Line Argument
         * For command line arguments we have a few flags that can be specified by the user.
         * -D : Domain
         * -P : Port
         * -S : Run as server
         * -C : Run as client
         * NOTE: if using -D or -P there should be a value following it. If given on their own we will throw an error
         */
        if(args.Length>= 1){
            //Port
            int port = 5131; //Default Port
            if(flags[2]=="1"){//Use custom port if given
                if((Int32.TryParse(flags[3], out port))== false){
                    throw(new ArgumentException("Inavlid Port Number given"));
                }
            }
            //Domain
            if(flags[4]=="1"){
	         //Use given domain and hostname
                IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(flags[5]);
                ipAddress = ipHostInfo.AddressList[0];
            }
            else{
                //IP information of the current machine
                string hostName = Dns.GetHostName(); 
                //string localIP = Dns.GetHostEntry(hostName).AddressList[0].ToString(); 
                //Only look for IPV4 addresses
                string localIP = Dns.GetHostEntry(hostName).AddressList.Where(a=> a.AddressFamily == AddressFamily.InterNetwork).ToArray()[0].ToString();
	            //Use local information to get IP information
                IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(localIP);
                ipAddress = ipHostInfo.AddressList[0];
            }


            if(flags[0]=="1"){ 
            //Server Mode
                Console.WriteLine("Launching as Server...");
                //Listening on all IP address available to this device for a connection
                var ipEndPoint = new IPEndPoint(IPAddress.Any, port);
                TcpListener listener = new(ipEndPoint);

                try
                {   //Starts listening for a connection 
                    listener.Start();
                    using TcpClient handler = await listener.AcceptTcpClientAsync();
                    await using NetworkStream stream = handler.GetStream();


                    Console.WriteLine($"Connection Established!");
                    do{     //Once we have a connection

                        Console.WriteLine($"\n-----Beginning New Game-----\n");
                        var message = "";//Variable to store message sent

                        //Prompt user for selection either "X" or "O"
                        Console.WriteLine("Do you want to use X or O: ");
                        message = Console.ReadLine(); 

                        if(message == null){Environment.Exit(1);}//Checks for NULL input
                        //Catches bad input
                        while(message!="X"&&message!="O"&&message!="x"&&message!="o"){
                            Console.WriteLine("Please enter either X or O");
                            message = Console.ReadLine();
                        }
                        //Processes good input
                        if(message=="x"||message=="X"){
                            message = "0"; 
                            player = 0;
                            opponent = 1;
                        }
                        else if(message =="o"||message=="O"){
                            message = "1";
                            player = 1; 
                            opponent = 0;
                        }

                        //Send choice to client
                        var XObytes = Encoding.UTF8.GetBytes(message);
                        await stream.WriteAsync(XObytes);

                        //Initialize variables for a new game
                        initializeGameBoard(gameBoard);
                        message = "";
                        do{
                            turn = false;//resetting for next turn
                            stateOfGame = gameState(gameBoard);//Determines the current state of the game
                            
                            if(stateOfGame!=-1){//If game is over, determines quit message
                                if(stateOfGame==player)message = "Server Wins!";
                                if(stateOfGame==opponent)message = "Client Wins!";
                                if(stateOfGame==2)message = "Board Full, game over";
                            }
                            else{//Takes in user input for move
                                Console.WriteLine("Enter Move: (or q to quit)");
                                message = Console.ReadLine();
                            }
                            if(message == null){Environment.Exit(1);}//Checks for NULL input
                            //Processing desired move if not quit value, or switching turns
                            if(message != "q" && message != "Server Wins!"&& message != "Client Wins!"&&message!="Board Full, game over"){
                                moveResult = processMove(message, gameBoard, player);
                                printBoard(gameBoard);
                                message = boardToString(gameBoard);//If valid move, send state
                            }
                            //Sending state 
                            var bytes = Encoding.UTF8.GetBytes(message);
                            await stream.WriteAsync(bytes);
                            if(message == "Server Wins!"||message == "Client Wins!"||message == "Board Full, game over"){
                                //If win value, print reason
                                Console.WriteLine($"Game Over: {message}");
                                message = "q";
                            }

                            if(moveResult == 0){//If move is processed succesfully, switch to listening
                                    turn = true;
                                    moveResult = 99;
                                }
                            if(turn){
                                turn = false;//resetting for next turn
                                do{
                                    Console.WriteLine("Waiting for message from opponent ...");
                                    //Listens for move from opponent and stores it in message
                                    var buffer= new byte[1_024];
                                    var received = await stream.ReadAsync(buffer);
                                    message = Encoding.UTF8.GetString(buffer, 0, received);
                                    //If win value, print reason
                                    if(message == "Server Wins!"||message == "Client Wins!"||message == "Board Full, game over"){
                                        Console.WriteLine($"Game Over: {message}");
                                        message = "q";
                                    }
                                    //If invalid move was used, state will not change
                                    if(message!="q"&& stateMatch(message,gameBoard)==0){
                                        Console.WriteLine("No valid move received, waiting for opponent to resend move");
                                    }
                                    //if not quit value process state
                                    else if(message != "q" && stateMatch(message, gameBoard)!=0){
                                        //Process state and parse out the specific move 
                                        if(stateMatch(message, gameBoard)==-1)throw new Exception("state doesn't match, exiting program");
                                        message = parseMove(message, gameBoard);

                                        Console.WriteLine($"Move received: \"{message}\"");
                                    
                                        //Processing Receiving a move
                                        if(message != "q"){
                                            moveResult = processMove(message, gameBoard, opponent);
                                            printBoard(gameBoard);
                                        }
                                        if(moveResult == 0){//if moved is received successfully, switch to sending
                                            turn=true;
                                            moveResult = 99;
                                        }
                                    }
                                } while(!turn && message!="q");
                            }
                        }while(message!="q");
                        //Prompts user if they want to play the game again
                        Console.WriteLine("Do you want to play again y/n?: ");
                        message = Console.ReadLine(); 
                        //Catches bad input
                        while(message!="y"&&message!="n"){
                            Console.WriteLine("Please enter either y or n");
                            message = Console.ReadLine();
                        }
                        //Send choice to client
                        if(message == null){Environment.Exit(1);}//Checks for NULL input
                        var bytes2 = Encoding.UTF8.GetBytes(message);
                        await stream.WriteAsync(bytes2);

                        //Receive Response from Client
                        String response = "";//Stores the user response
                        var buffer2= new byte[1_024];
                        int received2 = await stream.ReadAsync(buffer2);
                        response = Encoding.UTF8.GetString(buffer2, 0, received2);

                        //If either player is done, end the game
                        if(message == "n"||response == "n"){
                            Console.WriteLine("At least one party does not want to replay, ending game.");
                            done = true;
                        }



                    } while(!done);
                } 
                catch (Exception e){
                    Console.WriteLine(e);

                }
                finally
                {
                    listener.Stop();
                }
            }
        



            if(flags[1]=="1"){
            //Client Mode
                Console.WriteLine("Launching as Client...");
                var ipEndPoint = new IPEndPoint(ipAddress, port);
                //Connect to the given IP and Port
                using TcpClient client = new();
                await client.ConnectAsync(ipEndPoint);
                await using NetworkStream stream = client.GetStream();
                
                Console.WriteLine($"Connection Established!");
                do{
                    Console.WriteLine($"\n-----Beginning New Game-----\n");
                    Console.WriteLine("Waiting for message from opponent ...");
                    var message = "";
                    //Receive the server's choice of X or O, Use other option
                        var XObuffer= new byte[1_024];
                        int XOreceived = await stream.ReadAsync(XObuffer);
                        message = Encoding.UTF8.GetString(XObuffer, 0, XOreceived);
                    if(!Int32.TryParse(message, out opponent)){
                        throw(new ArgumentException("Invalid response from Server. Expected either 0 or 1"));
                    }
                    //Process received message
                    if(opponent == 1)player = 0;
                    else player = 1;

                    Console.WriteLine($"You will be playing as {formatValues(player)}\n");
                    //Initiallize variables for new game
                    initializeGameBoard(gameBoard);
                    message = "";
                    do {
                        turn = false;//resetting for new turn
                        Console.WriteLine("Waiting for message from opponent ...");
                        //Receive move from the server
                        var buffer= new byte[1_024];
                        int received = await stream.ReadAsync(buffer);
                        message = Encoding.UTF8.GetString(buffer, 0, received);
                        //If game over, print reason
                        if(message == "Server Wins!"||message == "Client Wins!"||message == "Board Full, game over"){
                            Console.WriteLine($"Game Over: {message}");
                            message = "q";
                        }
                        //If invalid move was given, state will not be changed
                        if(message!="q"&& stateMatch(message,gameBoard)==0){
                            Console.WriteLine("No valid move received, waiting for opponent to resend move");
                        }
                        //Process move
                        else if(message != "q" && stateMatch(message, gameBoard)!=0){
                            
                            //Process state and parse move
                            if(stateMatch(message, gameBoard)!=1)throw new Exception("state doesn't match, exiting program");
                            message = parseMove(message, gameBoard);

                            Console.WriteLine($"Move received: \"{message}\"");

                        
                            //Process normal moves received
                            if(message != "q"){
                                moveResult = processMove(message, gameBoard, opponent);
                                printBoard(gameBoard);
                            }
                            if(moveResult == 0){//If move is processed successfully, prepare to send a move
                                turn = true;
                                moveResult = 99;
                            }
                        }
                        if(turn){
                            turn = false;//resetting for new turn
                            do{
                                //Determines the state of the game
                                stateOfGame = gameState(gameBoard);
                                //If game is over, communicate that to the opponent
                                if(stateOfGame!=-1){
                                    if(stateOfGame==opponent)message = "Server Wins!";
                                    if(stateOfGame==player)message = "Client Wins!";
                                    if(stateOfGame==2)message = "Board Full, game over";
                                }
                                else{//Take in a normal move
                                    Console.WriteLine("Enter Move: (or q to quit)");
                                    message = Console.ReadLine();
                                }
                                if(message == null){Environment.Exit(1);}//Checks for NULL input
                                //Send move to the opponent
                        
                                //Processes move sent, if it isn't a game over, quit, or turn change 
                                if(message != "q" && message != "Server Wins!"&& message != "Client Wins!"&&message!="Board Full, game over"){
                                    moveResult = processMove(message, gameBoard, player);
                                    printBoard(gameBoard);
                                    message = boardToString(gameBoard);//If move is processed successfully, send state
                                }
                                //sending state
                                var bytes = Encoding.UTF8.GetBytes(message);
                                await stream.WriteAsync(bytes);
                                //If game over, print result
                                if(message == "Server Wins!"||message == "Client Wins!"||message == "Board Full, game over"){
                                    Console.WriteLine($"Game Over: {message}");
                                    message = "q";
                                }

                                if(moveResult == 0){//if move is sent successfully, switch back to listening
                                    turn = true;
                                    moveResult = 99;
                                }

                            }while(!turn&&message!="q");
                        }



                    }while(message!="q");
                    //Prompts both users if they want to replay or not 
                    Console.WriteLine("Waiting for message from opponent ...");
                    //Receive choice from server
                        var buffer2= new byte[1_024];
                        int received2 = await stream.ReadAsync(buffer2);
                        message = Encoding.UTF8.GetString(buffer2, 0, received2);
                        String response = message;  
                    //Prompt user for replay
                        Console.WriteLine("Do you want to play again y/n?: ");
                        message = Console.ReadLine(); 
                        while(message!="y"&&message!="n"){
                            Console.WriteLine("Please enter either y or n");
                            message = Console.ReadLine();
                        }
                    //Send choice to server
                        if(message == null){Environment.Exit(1);}//Checks for NULL input
                        var bytes2 = Encoding.UTF8.GetBytes(message);
                        await stream.WriteAsync(bytes2);

                    //If either play doesn't want to continue, end game 
                    if(message == "n"||response == "n"){
                        Console.WriteLine("At least one party does not want to replay, ending game.");
                        done = true;
                    }

                }while(!done);
                
            }


        }
        //If there are no command line arguments given
        else{
            Console.WriteLine($"Please provide a flag when compiling the program. \nEnter -help to see all available flags");
        }






    }
    catch(Exception e){
        Console.WriteLine(e);
    }
    }
    
}
