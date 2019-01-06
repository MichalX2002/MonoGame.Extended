using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Serialization;

namespace MonoGame.Extended.Particles.Serialization
{
    public class InterpolatorJsonConverter : BaseTypeJsonConverter<ParticleInterpolator>
    {
        public InterpolatorJsonConverter() 
            : base(GetSupportedTypes(), "Interpolator")
        {
        }

        private static IEnumerable<TypeInfo> GetSupportedTypes()
        {
            return typeof(ParticleInterpolator)
                .GetTypeInfo()
                .Assembly
                .DefinedTypes
                .Where(type => typeof(ParticleInterpolator).GetTypeInfo().IsAssignableFrom(type) && !type.IsAbstract);
        }
    }
}