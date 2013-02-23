﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public class ModuleWeaver
{
    static List<IRemover> removers;
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public List<string> DefineConstants { get; set; }

    static ModuleWeaver()
    {
        var removerType = typeof (IRemover);
        removers = removerType.Assembly.GetTypes()
            .Where(x => x.IsClass && removerType.IsAssignableFrom(x))
            .Select(x=>(IRemover)Activator.CreateInstance(x))
            .ToList();
    }

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
    }

    public void Execute()
    {
        if (DefineConstants == null || DefineConstants.All(x => x != "Scalpel"))
        {
            return;
        }
        var typeDefinitions = ModuleDefinition.Types.ToList();
        foreach (var type in typeDefinitions)
        {
            if (removers.Any(x => x.ShouldRmoveType(type)))
            {
                ModuleDefinition.Types.Remove(type);
            }
        }
        var assemblyNameReferences = ModuleDefinition.AssemblyReferences.ToList();
        foreach (var reference in assemblyNameReferences)
        {
            if (removers.Any(x => x.ReferenceName == reference.Name))
            {
                ModuleDefinition.AssemblyReferences.Remove(reference);
            }
        }
    }
}