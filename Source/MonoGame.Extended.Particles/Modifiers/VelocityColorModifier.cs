using System;

namespace MonoGame.Extended.Particles.Modifiers
{
    public class VelocityColorModifier : Modifier
    {
        public HslColor StationaryColor { get; set; }
        public HslColor VelocityColor { get; set; }
        public float VelocityThreshold { get; set; }

        public override unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            float velocityThreshold2 = VelocityThreshold * VelocityThreshold;

            while (iterator.HasNext)
            {
                Particle* particle = iterator.Next();
                float velocity2 = particle->Velocity.X * particle->Velocity.X +
                                particle->Velocity.Y * particle->Velocity.Y;

                if (velocity2 >= velocityThreshold2)
                    VelocityColor.CopyTo(out particle->Color);
                else
                {
                    HslColor deltaColor = VelocityColor - StationaryColor;
                    float t = (float)Math.Sqrt(velocity2) / VelocityThreshold;

                    particle->Color = new HslColor(
                        deltaColor.H * t + StationaryColor.H,
                        deltaColor.S * t + StationaryColor.S,
                        deltaColor.L * t + StationaryColor.L);
                }
            }
        }
    }
}