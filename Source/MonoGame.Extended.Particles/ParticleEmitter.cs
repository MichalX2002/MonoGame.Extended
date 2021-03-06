﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Particles
{
    public unsafe class ParticleEmitter : IDisposable
	{
		// Creates a not-so-random number.
		private readonly FastRandom _random = new FastRandom();
		private float _totalSeconds;
        internal ParticleBuffer Buffer;

        public string Name { get; set; }
        public int ActiveParticles => Buffer.Count;
        public Vector2 Offset { get; set; }
        public List<ParticleModifier> Modifiers { get; }
        public Profile Profile { get; set; }
        public ParticleReleaseParameters Parameters { get; set; }
        public TextureRegion2D TextureRegion { get; set; }
        public ParticleModifierExecutionStrategy ModifierExecutionStrategy { get; set; }
        
        [JsonConstructor]
		public ParticleEmitter(string name, TextureRegion2D textureRegion, int capacity, TimeSpan lifeSpan, Profile profile)
		{
            _lifeSpanSeconds = (float)lifeSpan.TotalSeconds;

			Name = name;
			TextureRegion = textureRegion;
			Buffer = new ParticleBuffer(capacity);
			Offset = Vector2.Zero;
			Profile = profile ?? throw new ArgumentNullException(nameof(profile));
			Modifiers = new List<ParticleModifier>();
			ModifierExecutionStrategy = ParticleModifierExecutionStrategy.Serial;
			Parameters = new ParticleReleaseParameters();
		}

		public ParticleEmitter(TextureRegion2D textureRegion, int capacity, TimeSpan lifeSpan, Profile profile)
			: this(null, textureRegion, capacity, lifeSpan, profile)
		{
		}

		/// <summary>
		/// Getter returns the capacity of the size <see cref="ParticleBuffer" />.
		/// Setter resizes the internal <see cref="Buffer"/>.
		/// </summary>
		public int Capacity
        {
            get => Buffer.Size;
            set
            {
                if (Buffer.Size != value)
                {
                    Buffer.Dispose();
                    Buffer = new ParticleBuffer(value);
                }
            }
        }

        private float _lifeSpanSeconds;
		public TimeSpan LifeSpan
        {
            get => TimeSpan.FromSeconds(_lifeSpanSeconds);
            set => _lifeSpanSeconds = (float)value.TotalSeconds;
        }

        private float _nextAutoTrigger;

		private bool _autoTrigger = true;
		public bool AutoTrigger
        {
            get => _autoTrigger;
            set
            {
                _autoTrigger = value;
                _nextAutoTrigger = 0;
            }
        }

        private float _autoTriggerFrequency;
		public float AutoTriggerFrequency
        {
            get => _autoTriggerFrequency;
            set
            {
                _autoTriggerFrequency = value;
                _nextAutoTrigger = 0;
            }
        }

        private void ReclaimExpiredParticles()
		{
			var iterator = Buffer.GetIterator();
			int expired = 0;

			while (iterator.HasNext)
			{
				var particle = iterator.Next();
				if (_totalSeconds - particle->Inception < _lifeSpanSeconds)
					break;

				expired++;
			}

			if (expired != 0)
				Buffer.Reclaim(expired);
		}

		public void Clear()
		{
            Buffer.Clear();
        }

		public bool Update(float elapsedSeconds, Vector2 position = default)
		{
			_totalSeconds += elapsedSeconds;

			if (_autoTrigger)
			{
				_nextAutoTrigger -= elapsedSeconds;

                const int maxTriggers = 5;
                int triggers = 0;
				while (_nextAutoTrigger <= 0)
				{
					Trigger(position);

                    // allow multiple triggers if delta was higher than trigger frequency
                    _nextAutoTrigger = MathHelper.Clamp(_nextAutoTrigger + _autoTriggerFrequency, -0.25f, _autoTriggerFrequency);

                    triggers++;
                    if (triggers >= maxTriggers)
                        break;
				}
			}

			if (Buffer.Count == 0)
				return false;

			ReclaimExpiredParticles();

			var iterator = Buffer.GetIterator();
			while (iterator.HasNext)
			{
				var particle = iterator.Next();
				particle->Age = (_totalSeconds - particle->Inception) / _lifeSpanSeconds;
				particle->Position = particle->Position + particle->Velocity * elapsedSeconds;
			}

			ModifierExecutionStrategy.ExecuteModifiers(Modifiers, elapsedSeconds, iterator);
			return true;
		}

		public void Trigger(Vector2 position, float layerDepth = 0)
		{
			var numToRelease = _random.Next(Parameters.Quantity);
			Release(position + Offset, numToRelease, layerDepth);
		}

		public void Trigger(LineSegment line)
		{
			var numToRelease = _random.Next(Parameters.Quantity);
			var lineVector = line.ToVector();

			for (var i = 0; i < numToRelease; i++)
			{
				var offset = lineVector * _random.NextSingle();
				Release(line.Origin + offset, 1);
			}
		}

		private void Release(Vector2 position, int numToRelease, float layerDepth = 0)
		{
			var iterator = Buffer.Release(numToRelease);
			while (iterator.HasNext)
			{
				var particle = iterator.Next();
				Profile.GetOffsetAndHeading(out particle->Position, out Vector2 heading);

				particle->Age = 0f;
				particle->Inception = _totalSeconds;
				particle->Position += position;
				particle->TriggerPos = position;

				var speed = _random.NextSingle(Parameters.Speed);
				particle->Velocity = heading * speed;

				_random.NextColor(out particle->Color, Parameters.Color);

                particle->Opacity = _random.NextSingle(Parameters.Opacity);
                
                if(Parameters.MaintainAspectRatioOnScale)
                {
                    var scale = _random.NextSingle(Parameters.Scale);
                    particle->Scale = new Vector2(scale, scale);
                }
                else
                {
                    particle->Scale = new Vector2(_random.NextSingle(Parameters.ScaleX), _random.NextSingle(Parameters.ScaleY));
                }
                
                particle->Rotation = _random.NextSingle(Parameters.Rotation);
                particle->Mass = _random.NextSingle(Parameters.Mass);
                particle->LayerDepth = layerDepth;
            }
        }

		public override string ToString()
		{
			return Name;
		}

        public void Dispose()
        {
            Buffer.Dispose();
        }
	}
}
