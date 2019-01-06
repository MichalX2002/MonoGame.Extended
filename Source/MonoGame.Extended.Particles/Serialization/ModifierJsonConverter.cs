﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Serialization;

namespace MonoGame.Extended.Particles.Serialization
{
    public class ModifierJsonConverter : BaseTypeJsonConverter<ParticleModifier>
    {
        public ModifierJsonConverter()
            : base(GetSupportedTypes(), "Modifier")
        {
        }

        private static IEnumerable<TypeInfo> GetSupportedTypes()
        {
            return typeof(ParticleModifier)
                .GetTypeInfo()
                .Assembly
                .DefinedTypes
                .Where(type => typeof(ParticleModifier).GetTypeInfo().IsAssignableFrom(type) && !type.IsAbstract);
        }
    }
}