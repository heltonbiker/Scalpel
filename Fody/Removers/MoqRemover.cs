﻿using Mono.Cecil;

class MoqRemover : IRemover
{
    public string[] ReferenceNames
    {
        get { return new[]{ "moq"}; }
    }

    public bool ShouldRemoveType(TypeDefinition typeDefinition)
    {
        return false;
    }

}