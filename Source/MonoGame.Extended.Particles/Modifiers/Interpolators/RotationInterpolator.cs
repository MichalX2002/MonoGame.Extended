namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    public class RotationInterpolator : ParticleInterpolator<float>
    {
        public override unsafe void Update(float amount, Particle* particle)
        {
            particle->Rotation = (EndValue - StartValue) * amount + StartValue;
        }
    }
}