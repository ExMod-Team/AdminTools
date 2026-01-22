using Exiled.API.Enums;
using System.Collections.Generic;
using Exiled.API.Features.Items;
using PlayerRoles;
using RelativePositioning;
using Exiled.API.Features;

namespace AdminTools
{
    public class Jailed
    {
        public string Name;
        public List<Item> Items;
        public List<Effect> Effects;
        public RoleTypeId Role;
        public RelativePosition RelativePosition;
        public float Health;
        public Dictionary<AmmoType, ushort> Ammo;
        public bool CurrentRound;
    }
}