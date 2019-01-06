namespace MonoGame.Extended.Particles.Modifiers
{
    public sealed class OpacityFadeModifier : ParticleModifier
    {
        public override unsafe void Update(float elapsedSeconds, ParticleBuffer.Iterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Opacity = 1.0f - particle->Age;
            }
        }
    }
}