
namespace MonoGame.Extended.Particles.Modifiers
{
    public abstract class ParticleModifier
    {
        protected ParticleModifier()
        {
            Name = GetType().Name;
        }

        public string Name { get; set; }
        public abstract void Update(float elapsedSeconds, ParticleBuffer.Iterator iterator);

        public override string ToString()
        {
            return $"{Name} [{GetType().Name}]";
        }
    }
}