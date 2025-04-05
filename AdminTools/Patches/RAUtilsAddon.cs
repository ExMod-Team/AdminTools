using Exiled.API.Enums;
using Exiled.API.Features;
using NorthwoodLib.Pools;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utils;

namespace AdminTools.Patches
{
    public static class CustomRAUtilsAddon
    {
        public static bool Prefix(ref List<ReferenceHub> __result, ArraySegment<string> args, int startindex, out string[] newargs, bool keepEmptyEntries = false)
        {
            Log.Debug("BetterCommands::CustomRAUtilsAddon was called.");
            string text = RAUtils.FormatArguments(args, startindex);
            List<ReferenceHub> list = ListPool<ReferenceHub>.Shared.Rent();
            
            if (text.StartsWith("@", StringComparison.Ordinal))
            {
                foreach (object obj in new Regex("@\"(.*?)\".|@[^\\s.]+\\.").Matches(text))
                {
                    Match match = (Match)obj;
                    text = RAUtils.ReplaceFirst(text, match.Value, "");
                    string name = match.Value.Substring(1).Replace("\"", "").Replace(".", "");
                    List<ReferenceHub> list2 = (from ply in ReferenceHub.AllHubs
                                                where ply.nicknameSync.MyNick.Equals(name)
                                                select ply).ToList();
                    if (list2.Count == 1 && !list.Contains(list2[0]))
                    {
                        list.Add(list2[0]);
                    }
                }
                newargs = text.Split(new char[]
                {
                        ' '
                }, keepEmptyEntries ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);
                __result = list;
            }
            else
            {
                if (args.At(startindex).Length > 0)
                {
                    string[] array = args.At(startindex).Split('.');
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i].ToLower() is "all" or "*")
                        {
                            list.AddRange(Player.List.Select(x => x.ReferenceHub));
                            break;
                        }
                        if (int.TryParse(array[i], out int playerId))
                        {
                            if (playerId < 1)
                                continue;

                            if (ReferenceHub.TryGetHub(playerId, out ReferenceHub item))
                                list.Add(item);
                        }
                        else if (Enum.TryParse(array[i], true, out SimplifyTeam simplifyTeam))
                        {
                            switch (simplifyTeam)
                            {
                                case SimplifyTeam.Alive:
                                    list.AddRange(Player.List.Where(p => p.IsAlive).Select(x => x.ReferenceHub));
                                    break;
                                case SimplifyTeam.Human:
                                    list.AddRange(Player.List.Where(p => p.IsHuman).Select(x => x.ReferenceHub));
                                    break;
                                case SimplifyTeam.Civilian:
                                    list.AddRange(Player.List.Where(p => p.Role.Team is Team.Scientists or Team.ClassD).Select(x => x.ReferenceHub));
                                    break;
                                case SimplifyTeam.Military:
                                    list.AddRange(Player.List.Where(p => p.Role.Team is Team.FoundationForces or Team.ChaosInsurgency).Select(p => p.ReferenceHub));
                                    break;
                                case SimplifyTeam.Staff:
                                    list.AddRange(Player.List.Where(p => p.RemoteAdminAccess).Select(p => p.ReferenceHub));
                                    break;
                                case SimplifyTeam.NoStaff:
                                    list.AddRange(Player.List.Where(p => !p.RemoteAdminAccess).Select(p => p.ReferenceHub));
                                    break;
                                default:
                                    list.AddRange(Player.Get((Team)simplifyTeam).Select(x => x.ReferenceHub));
                                    break;
                            }
                        }
                        else if (Enum.TryParse(array[i], true, out RoleTypeId roleType))
                        {
                            list.AddRange(Player.Get(roleType).Select(x => x.ReferenceHub));
                        }
                        else if (Enum.TryParse(array[i], true, out Side side))
                        {
                            list.AddRange(Player.Get(side).Select(x => x.ReferenceHub));
                        }
                        else if (Enum.TryParse(array[i], true, out Team team))
                        {
                            list.AddRange(Player.Get(team).Select(x => x.ReferenceHub));
                        }
                    }
                }
                newargs = args.Count > 1 ? RAUtils.FormatArguments(args, startindex + 1).Split(new char[]
                {
                        ' '
                }, keepEmptyEntries ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries) : null;
                __result = list;
            }
            return false;
        }

        /// <summary>
        /// <see cref="Team"/> simplified.
        /// </summary>
        public enum SimplifyTeam
        {
            /// <summary>
            /// Refer to <see cref="Team.SCPs"/>.
            /// </summary>
            Scp = Team.SCPs,
            /// <summary>
            /// Refer to <see cref="Team.FoundationForces"/>.
            /// </summary>
            Mtf = Team.FoundationForces,
            /// <summary>
            /// Refer to <see cref="Team.FoundationForces"/>.
            /// </summary>
            Ntf = Team.FoundationForces,
            /// <summary>
            /// Refer to <see cref="Team.ChaosInsurgency"/>.
            /// </summary>
            Ci = Team.ChaosInsurgency,
            /// <summary>
            /// Refer to <see cref="Team.ChaosInsurgency"/>.
            /// </summary>
            Chaos = Team.ChaosInsurgency,
            /// <summary>
            /// Refer to <see cref="Team.Scientists"/>.
            /// </summary>
            Science = Team.Scientists,
            /// <summary>
            /// Refer to <see cref="Team.Scientists"/>.
            /// </summary>
            Scientist = Team.Scientists,
            /// <summary>
            /// Refer to <see cref="Team.ClassD"/>.
            /// </summary>
            Cld = Team.ClassD,
            /// <summary>
            /// Refer to <see cref="Team.Dead"/>.
            /// </summary>
            Rip = Team.Dead,
            /// <summary>
            /// Refer to <see cref="Team.OtherAlive"/>.
            /// </summary>
            Tut = Team.OtherAlive,
            /// <summary>
            /// Refer to <see cref="Team.OtherAlive"/>.
            /// </summary>
            Tutorial = Team.OtherAlive,
            /// <summary>
            /// All alive players.
            /// </summary>
            Alive,
            /// <summary>
            /// All alive human players.
            /// </summary>
            Human,
            /// <summary>
            /// All alive civilian players.
            /// </summary>
            Civilian,
            /// <summary>
            /// All alive military players.
            /// </summary>
            Military,
            /// <summary>
            /// All non staff players.
            /// </summary>
            NoStaff,
            /// <summary>
            /// All staff players.
            /// </summary>
            Staff,
        }
    }
}