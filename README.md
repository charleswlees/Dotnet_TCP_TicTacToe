# Charlie Lees CS5700 HW4

This is my submission for HW4 for CS5700: Tic Tac Toe

## Microsoft .Net Installation
To compile the code you will need to have .Net installed on your instance of the Khoury server. \
I have provided the Microsoft official installation script in the root of my github titled 'dotnet-install.sh'

Script Source: (https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#scripted-install)

#### Installation:

### Step 1: Copy the Microsoft script to your HOME directory
```bash
#From the root folder of my Github CS5700_charleswlees
cp dotnet-install.sh ~/
```

### Step 2: Make the script executable with the chmod command
```bash
#Navigate to your HOME directory
cd ~/
#Make the script executable
chmod +x dotnet-install.sh
```


### Step 3: Run the Script
```bash
#In your HOME directory
./dotnet-install.sh
```

### Step 4: Add .Net values to PATH
The Khoury server uses .profile to configure the shell\
Create/Modify this file in your HOME directory
```bash
#Navigate to your HOME directory
cd ~/
#Create or Modify your .profile file
vim .profile

```
Once in the .profile file, add the following lines to the bottom
```bash
#Within the .profile file 
DOTNET_ROOT=$HOME/.dotnet
PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools
```
### Step 5: Reconnect the shell
Disconnect and reconnect to the Khoury server and now the 'dotnet' command should work
```bash
dotnet
```
This should provide the following result
```bash
Usage: dotnet [options]
Usage: dotnet [path-to-application]

Options:
  -h|--help         Display help.
  --info            Display .NET information.
  --list-sdks       Display the installed SDKs.
  --list-runtimes   Display the installed runtimes.

path-to-application:
  The path to an application .dll file to execute.
```

## Compiling the Project

Navigate to the project folder for my submission
```bash
cd CS5700_charleswlees/HW4/ttt/
```
The project can then be run using the 'dotnet run' command with some of the below flags.

Example:
```bash
dotnet run -S
```
The project uses various flags for different command line arguments.

- -S : used to run program as the server
- -C : used to run program as the client
- -D : used to provide a domain. The text after this flag will be taken as the domain
   - To get a valid domain to use, use the 'hostname' command in Linux
   - For example, on the Khoury server 'hostname' returns 'login-students.khoury.northeastern.edu'
   - This flag is optional
- -P : used to provide a port. The text after this flag will be taken as the port 
   - This flag is optional 

Example: 
```bash
dotnet run -S -D login-students.khoury.northeastern.edu -P 8000
```
To connect with this server once it's launched we would then run
```bash
dotnet run -C -D login-students.khoury.northeastern.edu -P 8000
```

It is required that either -S or -C is used. \
The other flags are optional.

**NOTE: All flags use uppercase letters**
\
In C# the lowercase -c flag is reserved so they have to be uppercase for this program\
See below for the error generated from using a lowercase c.

**Things to keep in mind**

- -S or -C must be selected for the program to run, but they cannot both be used.
- -P and -D require that a port/domain is provided after the flag. If one is not provided the program will terminate.
- There is no specified order for the flags, they can be added in any order.
- More than one of the same flag cannot be used.

## Common Errors

#### Transient Server Resource Errors

The following errors have appeared intermittently while testing my code on the Khoury Servers. They do not appear often but they have shown up more than once so I wanted to include them in case anyone ran into them. 

All of these errors are transient and are tied to server resources. After waiting a moment these errors will pass.

After speaking with Scott, this is to be expected with how much is going on in the Khoury Server.



```bash
Unhandled exception. System.ComponentModel.Win32Exception (11): Resource temporarily unavailable
   at System.ConsolePal.EnsureInitializedCore()
   at System.ConsolePal.Write(SafeFileHandle fd, ReadOnlySpan`1 buffer, Boolean mayChangeCursorPosition)
   at System.ConsolePal.UnixConsoleStream.Write(ReadOnlySpan`1 buffer)
   at System.IO.StreamWriter.Flush(Boolean flushStream, Boolean flushEncoder)
   at System.IO.StreamWriter.WriteLine(String value)
   at System.IO.TextWriter.WriteLine(Object value)
   at System.IO.TextWriter.SyncTextWriter.WriteLine(Object value)
   at System.Console.WriteLine(Object value)
   at TicTacToe.Main(String[] args) in /home/charleswlees/cs5700/CS5700_charleswlees/HW4/ttt/Program.cs:line 467
   at TicTacToe.<Main>(String[] args)
```



```bash
System.TypeInitializationException: The type initializer for 'System.Net.Sockets.SocketAsyncEngine' threw an exception.
 ---> System.OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown.
   at System.Threading.Thread.StartInternal(ThreadHandle t, Int32 stackSize, Int32 priority, Char* pThreadName)
   at System.Threading.Thread.StartCore()
   at System.Threading.Thread.Start(Object parameter, Boolean captureContext)
   at System.Threading.Thread.UnsafeStart(Object parameter)
   at System.Net.Sockets.SocketAsyncEngine..ctor()
   at System.Net.Sockets.SocketAsyncEngine.CreateEngines()
   at System.Net.Sockets.SocketAsyncEngine..cctor()
   --- End of inner exception stack trace ---
   at System.Net.Sockets.SocketPal.CreateSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, SafeSocketHandle& socket)
   at System.Net.Sockets.Socket..ctor(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
   at System.Net.Sockets.TcpListener..ctor(IPEndPoint localEP)
   at TicTacToe.Main(String[] args) in /home/charleswlees/cs5700/CS5700_charleswlees/HW4/ttt/Program.cs:line 219
```
```bash
#This error is equivalent to the previous error, simply communicating that the environment is out of memory in that moment
Failed to create CoreCLR, HRESULT: 0x8007000E
```
#### Lowercase Server Client Flags

The lowercase -c flag is reserved for .Net. If you try to use it you will get an error:
```bash
#For this example I entered the following 
dotnet run -c
```
```bash
#Resulting Error
System.InvalidOperationException: Required argument missing for option: -c
   at System.CommandLine.Binding.ArgumentConverter.GetValueOrDefault[T](ArgumentConversionResult result)
   at System.CommandLine.Parsing.OptionResult.GetValueOrDefault[T]()
   at System.CommandLine.Parsing.SymbolResult.GetValueForOption(Option option)
   at System.CommandLine.ParseResult.GetValueForOption(Option option)
   at Microsoft.DotNet.Cli.Telemetry.TopLevelCommandNameAndOptionToLog.AllowList(ParseResult parseResult, Dictionary`2 measurements)
   at Microsoft.DotNet.Cli.Telemetry.TelemetryFilter.Filter(Object objectToFilter)
   at Microsoft.DotNet.Cli.Utils.TelemetryEventEntry.SendFiltered(Object o)
   at Microsoft.DotNet.Cli.Program.ProcessArgs(String[] args, TimeSpan startupTime, ITelemetry telemetryClient)
   at Microsoft.DotNet.Cli.Program.Main(String[] args)
```

To resolve this, use uppercase letters when running the programs
```bash
dotnet run -C
```



