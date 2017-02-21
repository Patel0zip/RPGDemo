// Copyright 1998-2016 Epic Games, Inc. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnrealBuildTool;
using System.IO;

public class unreal : ModuleRules
{
	/// <summary>
    /// Schema files are processed and output to this folder. It should be within
    /// the current module so the types are accessible to the game.
    /// </summary>
    protected string GeneratedCodeDir
    {
        get { return Path.GetFullPath(Path.Combine(ModuleDirectory, "Improbable", "Generated")); }
    }

	private string StandardLibraryDir
	{
		get { return Path.GetFullPath(Path.Combine(GeneratedCodeDir, "Std")); }
	}

	private string UserSchemaDir
	{
		get { return Path.GetFullPath(Path.Combine(GeneratedCodeDir, "Usr")); }
	}

	public unreal(TargetInfo Target)
	{
        var SpatialOS = new SpatialOSModule(this, Target);
		SpatialOS.SetupSpatialOS();
		if (UEBuildConfiguration.bCleanProject)
		{
			SpatialOS.RunSpatial("process_schema clean --language=cpp_unreal " + SpatialOS.QuoteString(GeneratedCodeDir));
		}
		else
		{
			var std = "process_schema generate --cachePath=.spatialos/schema_codegen_cache_std" +
				" --output=" + SpatialOS.QuoteString(StandardLibraryDir) +
				" --language=cpp_unreal" +
				" --input=../../build/dependencies/schema/WorkerSdkSchema";

			SpatialOS.RunSpatial(std);

			var user = "process_schema generate --cachePath=.spatialos/schema_codegen_cache_usr" +
				" --output=" + SpatialOS.QuoteString(UserSchemaDir) +
				" --language=cpp_unreal" +
				" --input=../../schema";

			SpatialOS.RunSpatial(user);
		}

		PublicDependencyModuleNames.AddRange(new string[] { "Core", "CoreUObject", "Engine", "InputCore" });

		PublicIncludePaths.AddRange(new[]
		{
			Path.GetFullPath(StandardLibraryDir),
				   Path.GetFullPath(UserSchemaDir)
		});
	}
}
