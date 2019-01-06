﻿namespace MonoGame.Extended.Particles.Modifiers.Interpolators
{
    public class HueInterpolator : ParticleInterpolator<float>
    {
        public override unsafe void Update(float amount, Particle* particle)
        {
            particle->Color = new HslColor(
                (EndValue - StartValue) * amount + StartValue,
                particle->Color.S,
                particle->Color.L);
        }
    }
}