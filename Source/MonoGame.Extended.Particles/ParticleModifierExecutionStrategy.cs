using System;
using System.Collections.Generic;
using MonoGame.Extended.Particles.Modifiers;

namespace MonoGame.Extended.Particles
{
    using TPL = System.Threading.Tasks;

    public abstract class ParticleModifierExecutionStrategy
    {
        public static readonly ParticleModifierExecutionStrategy Serial = new SerialModifierExecutionStrategy();
        public static readonly ParticleModifierExecutionStrategy Parallel = new ParallelModifierExecutionStrategy();

        internal abstract void ExecuteModifiers(List<ParticleModifier> modifiers, float elapsedSeconds, ParticleBuffer.Iterator iterator);

        internal class SerialModifierExecutionStrategy : ParticleModifierExecutionStrategy
        {
            internal override void ExecuteModifiers(List<ParticleModifier> modifiers, float elapsedSeconds, ParticleBuffer.Iterator iterator)
            {
                for (var i = 0; i < modifiers.Count; i++)
                {
                    iterator.Reset();
                    modifiers[i].Update(elapsedSeconds, iterator);
                }
            }

            public override string ToString()
            {
                return nameof(Serial);
            }
        }

        internal class ParallelModifierExecutionStrategy : ParticleModifierExecutionStrategy
        {
            internal override void ExecuteModifiers(List<ParticleModifier> modifiers, float elapsedSeconds, ParticleBuffer.Iterator iterator)
            {
                TPL.Parallel.ForEach(modifiers, modifier =>
                {
                    iterator.Reset();
                    modifier.Update(elapsedSeconds, iterator);
                });
            }

            public override string ToString()
            {
                return nameof(Parallel);
            }
        }

        public static ParticleModifierExecutionStrategy Parse(string value)
        {
            if (string.Equals(nameof(Parallel), value, StringComparison.OrdinalIgnoreCase))
                return Parallel;

            if (string.Equals(nameof(Serial), value, StringComparison.OrdinalIgnoreCase))
                return Serial;

            throw new InvalidOperationException($"Unknown particle modifier execution strategy '{value}'");
        }
    }
}