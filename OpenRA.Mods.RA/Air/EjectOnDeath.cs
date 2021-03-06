#region Copyright & License Information
/*
 * Copyright 2007-2011 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using OpenRA.FileFormats;
using OpenRA.Mods.RA.Effects;
using OpenRA.Traits;

namespace OpenRA.Mods.RA
{
	public class EjectOnDeathInfo : TraitInfo<EjectOnDeath>
	{
		[ActorReference]
		public readonly string PilotActor = "E1";
		public readonly int SuccessRate = 50;
		public readonly string ChuteSound = "chute1.aud";
		public readonly WRange MinimumEjectHeight = new WRange(427);
	}

	public class EjectOnDeath : INotifyKilled
	{
		public void Killed(Actor self, AttackInfo e)
		{
			var info = self.Info.Traits.Get<EjectOnDeathInfo>();
			var pilot = self.World.CreateActor(false, info.PilotActor.ToLowerInvariant(),
				new TypeDictionary { new OwnerInit(self.Owner) });

			var r = self.World.SharedRandom.Next(1, 100);
			var cp = self.CenterPosition;

			if (IsSuitableCell(pilot, self.Location) && r > 100 - info.SuccessRate && cp.Z > info.MinimumEjectHeight.Range
				&& self.Owner.WinState != WinState.Lost)
			{
				self.World.AddFrameEndTask(w => w.Add(new Parachute(pilot, cp)));
				Sound.Play(info.ChuteSound, cp);
			}
			else
				pilot.Destroy();
		}

		bool IsSuitableCell(Actor actorToDrop, CPos p)
		{
			return actorToDrop.Trait<IPositionable>().CanEnterCell(p);
		}
	}
}