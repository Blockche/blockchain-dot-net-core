# blockchain-dot-net-core
Sample blockchain written on C# and using .Net Core 2.1

-Using Bouncy Castle for Cryptography
-Swagger for Api display (Can be opened on {url}/swagger )


#To run the a node:
- Go to web project folder and, open Power Shell or Cmd and type : dotnet run (this will run the oplication on the default 5005, which is set up in hosting.json file)
- When want to run another nodes do the same but with specifing urls like that : dotnet run --urls http://localhost:5001
-swagger opens at :  {host}:{port}/swagger


#To run multiple instances on different ports:
- Generate exe: 
	-dotnet publish -c Release -r win10-x64 (in the package manager console)

- from Tools project run :
	-NodeInstanceRunner.StartMultipleInstances(SampleUrls) or 
	1.Go the Console App Folder
	2.Open Cmd
	3.type in: dotnet run
	
	