﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Visualizer;
using SabberStoneCore.Model.Zones;

namespace SabberStoneGui.AsciiVisual
{
    public class Visualizer
    {

        public static string Visualize(Game currentGame)
        {
            var p1 = ManaString(currentGame.Player1);
            var p2 = ManaString(currentGame.Player2);

            var handAStr = new StringBuilder();
            handAStr.AppendLine("+-" + currentGame.Player1.Hero.Controller.Name.PadRight(10).Substring(0, 10) + "-".PadLeft(57, '-') + "[" + p1 + "]-+" + "-".PadLeft(9, '-') + "+");
            handAStr.Append(CreateHero(currentGame.Player1.Hero, true));
            handAStr.AppendLine("+-HAND-v" + "-".PadLeft(74, '-') + "+" + "-".PadLeft(9, '-') + "+");
            handAStr.Append(CreateZone(currentGame.Player1.Hand, currentGame.Player1.Deck, true));
            handAStr.AppendLine("+-PLAY-v" + "-".PadLeft(74, '-') + "+" + "-".PadLeft(9, '-') + "+");
            handAStr.Append(CreateZone(currentGame.Player1.Board, currentGame.Player1.Graveyard, true));
            handAStr.AppendLine("+" + "-".PadLeft(81, '-') + "+" + "-".PadLeft(9, '-') + "+");
            handAStr.Append(CreateZone(currentGame.Player2.Board, currentGame.Player2.Graveyard, false));
            handAStr.AppendLine("+-PLAY-^" + "-".PadLeft(74, '-') + "+" + "-".PadLeft(9, '-') + "+");
            handAStr.Append(CreateZone(currentGame.Player2.Hand, currentGame.Player2.Deck, false));
            handAStr.AppendLine("+-HAND-^" + "-".PadLeft(74, '-') + "+" + "-".PadLeft(9, '-') + "+");
            handAStr.Append(CreateHero(currentGame.Player2.Hero, false));
            handAStr.AppendLine("+-" + currentGame.Player2.Hero.Controller.Name.PadRight(10).Substring(0, 10) + "-".PadLeft(57, '-') + "[" + p2 + "]-+" + "-".PadLeft(9, '-') + "+");

            return handAStr.ToString();
        }

        private static string ManaString(Controller player)
        {
            return ("".PadLeft(player.UsedMana, 'x') 
                + "".PadLeft(player.OverloadLocked, 'q') 
                + "".PadLeft(10 - player.BaseMana, '.')).PadLeft(10, 'o');
        }

        private static string CreateHero(Hero hero, bool turn)
        {
            var cardCnt = 1;

            var zoneStr = new StringBuilder();

            var spacer = new string(' ', 5 * (8 - cardCnt) + cardCnt + 1);

            var zoneStrArray = new []
            {
                new StringBuilder("¦" + spacer),
                new StringBuilder("¦" + spacer),
                new StringBuilder("¦" + spacer),
                new StringBuilder("¦" + spacer),
                new StringBuilder("¦" + spacer),
                new StringBuilder("¦" + spacer),
            };

            var cardAscii = new CardAsciiBuilder().Create()
                .Name(hero.Card.Name)
                .Exhausted(hero.IsExhausted)
                .HeroPowerExhausted(hero.Power.IsExhausted)
                .AttackDamage(hero.AttackDamage)
                .Health(hero.Health)
                .HeroWeaponAttackDamage(hero.Weapon?.AttackDamage ?? 0)
                .HeroWeaponDurability(hero.Weapon?.Durability ?? 0)
                .Health(hero.Health)
                .Windfury(hero.HasWindfury)
                .Windfury(hero.IsFrozen)
                .Build();

            var x = turn ? 1 : 0;
            zoneStrArray[0 + x].Append(new string(cardAscii[0]));
            zoneStrArray[1 + x].Append(new string(cardAscii[1]));
            zoneStrArray[2 + x].Append(new string(cardAscii[2]));
            zoneStrArray[3 + x].Append(new string(cardAscii[3]) + " " + hero.Power.Card.AbbreviatedName(7));
            zoneStrArray[4 + x].Append(new string(cardAscii[4]));
            zoneStrArray[turn ? 0 : 5].Append(new string(cardAscii[5]) + " " + (hero.Controller == hero.Game.CurrentPlayer ? " <<==" : "     "));
            zoneStrArray[0].Append(" ");
            zoneStrArray[1].Append(" ");
            zoneStrArray[2].Append(" ");
            zoneStrArray[3].Append(" ");
            zoneStrArray[4].Append(" ");
            zoneStrArray[5].Append(" ");

            var zoneAscii = new CardAsciiBuilder().Create().ZoneCards(hero.Controller.Secrets.Count, hero.Controller.Secrets.Type).Build();

            var y = turn ? 1 : 0;
            var backStrArray = new[] { "", "", "", "", "", "" };
            backStrArray[0 + y] = new string(zoneAscii[0]);
            backStrArray[1 + y] = new string(zoneAscii[1]);
            backStrArray[2 + y] = new string(zoneAscii[2]);
            backStrArray[3 + y] = new string(zoneAscii[3]);
            backStrArray[4 + y] = new string(zoneAscii[4]);
            backStrArray[turn ? 0 : 5] = new string(zoneAscii[5]);

            zoneStr.Append(zoneStrArray[0 + y].AppendLine(spacer.Substring(1) + "¦ " + backStrArray[0] + " ¦"));
            zoneStr.Append(zoneStrArray[1 + y].AppendLine(spacer.Substring(1) + "¦ " + backStrArray[1] + " ¦"));
            zoneStr.Append(zoneStrArray[2 + y].AppendLine(spacer.Substring(1) + "¦ " + backStrArray[2] + " ¦"));
            zoneStr.Append(zoneStrArray[3 + y].AppendLine(spacer.Substring(9) + "¦ " + backStrArray[3] + " ¦"));
            zoneStr.Append(zoneStrArray[4 + y].AppendLine(spacer.Substring(1) + "¦ " + backStrArray[4] + " ¦"));
            zoneStr.Append(zoneStrArray[turn ? 0 : 5].AppendLine(spacer.Substring(7) + "¦ " + backStrArray[5] + " ¦"));
            return zoneStr.ToString();
        }

        private static string CreateZone(IZone zone, IZone outzone, bool turn)
        {
            var zoneStr = new StringBuilder();

            var spacer = zone.Count == 10 ? new string(' ', 1) : new string(' ', 5 * (8 - zone.Count) + zone.Count + 1);

            var zoneStrArray = new StringBuilder[]
            {
                new StringBuilder("¦" + spacer),
                new StringBuilder("¦" + spacer),
                new StringBuilder("¦" + spacer),
                new StringBuilder("¦" + spacer),
                new StringBuilder("¦" + spacer),
                new StringBuilder("¦" + spacer),
            };
            for (var i = 0; i < zone.Count; i++)
            {
                var cardAscii = new char[6][];

                var spell = zone[i] as Spell;
                var minion = zone[i] as Minion;
                var weapon = zone[i] as Weapon;

                if (spell != null)
                {
                    cardAscii = new CardAsciiBuilder().Create()
                        .Name(spell.Card.AbbreviatedName(7))
                        .Cost(spell.Cost)
                        .SpellText(Regex.Replace(spell.Card.Text, @"<[^>]*>", "").Replace(Environment.NewLine, ""))
                        .Type(ECardType.SPELL)
                        .Build();

                }
                else if (minion != null)
                {
                    cardAscii = new CardAsciiBuilder().Create()
                        .Name(minion.Card.AbbreviatedName(7))
                        .Cost(minion.Cost)
                        .Type(minion.Card.Type)
                        .AttackDamage(minion.AttackDamage)
                        .Health(minion.Health)
                        .Spellpower(minion[EGameTag.SPELLPOWER])
                        .Deathrattle(minion.HasDeathrattle)
                        .Battlecry(minion.HasBattleCry)
                        .Inspire(minion.HasInspire)
                        .Poisonus(minion[EGameTag.POISONOUS] == 1)
                        .Silenced(minion.IsSilenced)
                        .Freeze(minion[EGameTag.FREEZE] == 1)
                        .Charge(minion.HasCharge)
                        .Taunt(minion.HasTaunt)
                        .Stealth(minion.HasStealth)
                        .Enrage(minion.IsEnraged)
                        .Frozen(minion.IsFrozen)
                        .DivineShield(minion.HasDivineShield)
                        .Windfury(minion.HasWindfury)
                        .Exhausted(minion.IsExhausted)
                        .Build();

                }
                else if (weapon != null)
                {
                    cardAscii = new CardAsciiBuilder().Create()
                        .Name(weapon.Card.AbbreviatedName(7))
                        .Cost(weapon.Cost)
                        .Type(weapon.Card.Type)
                        .AttackDamage(weapon.AttackDamage)
                        .Durability(weapon.Durability)
                        .Deathrattle(weapon[EGameTag.DEATHRATTLE] == 1)
                        .Battlecry(weapon[EGameTag.BATTLECRY] == 1)
                        .Windfury(weapon[EGameTag.WINDFURY] == 1)
                        .Exhausted(weapon.IsExhausted)
                        .Build();
                }

                var x = turn ? 1 : 0;
                zoneStrArray[0 + x].Append(new string(cardAscii[0]));
                zoneStrArray[1 + x].Append(new string(cardAscii[1]));
                zoneStrArray[2 + x].Append(new string(cardAscii[2]));
                zoneStrArray[3 + x].Append(new string(cardAscii[3]));
                zoneStrArray[4 + x].Append(new string(cardAscii[4]));
                zoneStrArray[turn ? 0 : 5].Append(new string(cardAscii[5]));

                if (i < zone.Count)
                {
                    zoneStrArray[0].Append(" ");
                    zoneStrArray[1].Append(" ");
                    zoneStrArray[2].Append(" ");
                    zoneStrArray[3].Append(" ");
                    zoneStrArray[4].Append(" ");
                    zoneStrArray[5].Append(" ");
                }
            }

            var backStrArray = new string[] { "", "", "", "", "", "" };

            var zoneAscii = new CardAsciiBuilder().Create().ZoneCards(outzone.Count, outzone.Type).Build();
            var y = turn ? 1 : 0;
            backStrArray[0 + y] = new string(zoneAscii[0]);
            backStrArray[1 + y] = new string(zoneAscii[1]);
            backStrArray[2 + y] = new string(zoneAscii[2]);
            backStrArray[3 + y] = new string(zoneAscii[3]);
            backStrArray[4 + y] = new string(zoneAscii[4]);
            backStrArray[turn ? 0 : 5] = new string(zoneAscii[5]);

            zoneStr.Append(zoneStrArray[0].AppendLine(spacer.Substring(1) + "¦ " + backStrArray[0] + " ¦"));
            zoneStr.Append(zoneStrArray[1].AppendLine(spacer.Substring(1) + "¦ " + backStrArray[1] + " ¦"));
            zoneStr.Append(zoneStrArray[2].AppendLine(spacer.Substring(1) + "¦ " + backStrArray[2] + " ¦"));
            zoneStr.Append(zoneStrArray[3].AppendLine(spacer.Substring(1) + "¦ " + backStrArray[3] + " ¦"));
            zoneStr.Append(zoneStrArray[4].AppendLine(spacer.Substring(1) + "¦ " + backStrArray[4] + " ¦"));
            zoneStr.Append(zoneStrArray[5].AppendLine(spacer.Substring(1) + "¦ " + backStrArray[5] + " ¦"));
            return zoneStr.ToString();
        }

    }
}