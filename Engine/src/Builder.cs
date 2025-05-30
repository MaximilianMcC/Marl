using System.Diagnostics;

class Builder
{
	public static string GamePath;

	public static string Status = "erhm";

	public static void BuildAndRun()
	{
		// Start to compile the code in a new thread
		Task.Run(() => {
			
			// Chuck it all in an assembly
			Status = "building rn";
			Compile();
			Status = "ok its done";

			// Run the game
			Status = "running game rn";
			Run();
		});
	}

	public static void HotReload()
	{
		// Start to compile the code in a new thread
		Task.Run(() => {
			
			// Chuck it all in an assembly
			Status = "hot reloading rn";
			Compile();
			Status = "ok its done";

			//? The game will detect the new 
			//? files and load them and stuff
		});
	}

	private static void Run()
	{
		Process game = Process.Start(GamePath, Project.JsonPath);
		game.EnableRaisingEvents = true;

		// If we close the engine then also close the game
		AppDomain.CurrentDomain.ProcessExit += (s, e) => game.Kill();

		// If we close the game then update the status
		game.Exited += (s, e) => Status = "js finished running game";
	}

	private static void Compile(bool release = false)
	{
		// Get the assembly output path
		// and the csproj path
		string outputPath = Path.Combine(Project.RootPath, "bin", "assemblies");
		string csprojPath = Path.Combine(Project.RootPath, Project.Namespace + ".csproj");

		// TODO: Maybe like delete the assemblies folder every like 10 runs to clean it

		// Check for if we're like fully building it (finished game)
		string configuration = release ? "Release" : "Debug";

		// Make the compile command
		ProcessStartInfo command = new ProcessStartInfo()
		{
			// TODO: Make so you can toggle between debug and not
			FileName = "dotnet",
			Arguments = $"build \"{csprojPath}\" --no-dependencies -c {configuration} -o \"{outputPath}\"",

			CreateNoWindow = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true
		};

		// Run the command to compile everything
		Process process = new Process();
		process.StartInfo = command;
		process.Start();

		Console.WriteLine("building/built to " + outputPath);
	}
}