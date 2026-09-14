namespace AgeOfEnlightenment.Spellcasting
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

	public static class SpellTargetManager
	{
		public static Entity GetTarget(TargetingSettings settings, SpellActionContext context)
		{
			//Normally there would be a big switch statement thing here. But for now we go with BASIC
			return SphereCast(settings, context);
		}

		
		static Entity SphereCast(TargetingSettings settings, SpellActionContext context)
		{
			Transform origin = null;
			Vector3 direction = Vector3.zero;

			switch (settings.Source)
			{
				case TargetSource.Caster:
					Debug.Log("Whoops! This source isnt set up yet");
					break;
				case TargetSource.CastOrigin:
					origin = context.CastOrigin;
					break;
				case TargetSource.PlayerView:
					origin = Camera.main.transform;

					break;
			}

			switch (settings.Direction)
			{
				case SpellShootDirection.Left:
                    direction = -origin.right;
                    break;
				case SpellShootDirection.Right:
                    direction = origin.right;
                    break;
				case SpellShootDirection.Up:
                    direction = origin.up;
                    break;
				case SpellShootDirection.Down:
					direction = -origin.up;
					break;
				case SpellShootDirection.Forward:
                    direction = origin.forward;
                    break;
				case SpellShootDirection.Back:
					direction = -origin.forward;
					break;
				case SpellShootDirection.PlayerDirection:
					direction = Camera.main.transform.forward;
					break;
				case SpellShootDirection.PlayerDirectionWithCastVelocity:
                    Debug.Log("Whoops! This direction isnt set up yet");
                    break;
			}

			var hitEntities = ConvertHitsToEntity(Physics.SphereCastAll(origin.position, settings.CastSize, direction, settings.CastDistance, ~settings.IgnoredLayers));


			if (hitEntities.Count == 0)
				return null;

			if(settings.Selection == TargetSelection.Closest || settings.Selection == TargetSelection.Furthest)
			{
				//If we need to worry about the distance to the entities we sort them by distance to the source
				hitEntities.Sort((ent1, ent2) =>
				{
					float distTo1 = Vector3.Distance(origin.position, ent1.transform.position);
					float distTo2 = Vector3.Distance(origin.position, ent2.transform.position);

					if (distTo1 < distTo2)
						return -1;
					else if (distTo1 > distTo2)
						return 1;
					else
						return 0;
				});
			}

			if (settings.Selection == TargetSelection.Closest)
				return hitEntities[0];
			if (settings.Selection == TargetSelection.Furthest)
				return hitEntities.Last();
			else
				return null;
        }

		static List<Entity> ConvertHitsToEntity(RaycastHit[] hits)
		{
            List<Entity> hitEntities = new();
            HashSet<Entity> seenEntity = new();

            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent(out Entity entity))
                {
                    //This sort of method returns true if the item could be added to the HashSet. Meaning if it's already in the list then it would return false
                    if (seenEntity.Add(entity))
                    {
                        hitEntities.Add(entity);
                    }
                }
            }

			return hitEntities;
        }
	}
}