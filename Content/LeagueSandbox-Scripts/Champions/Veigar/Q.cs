using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;


namespace Spells
{
    public class VeigarBalefulStrike : ISpellScript
    {
        int ticks;
        IObjAiBase Owner;
        IStatsModifier statsModifier = new StatsModifier();
        ISpell Spell;
        float stacks;
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Target
            }
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellMissileHit.AddListener(this, new System.Collections.Generic.KeyValuePair<ISpell, IObjAiBase>(spell, owner), TargetExecute, false);
            Owner = owner;

        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            var ownerSkinID = owner.Skin;
            var APratio = owner.Stats.AbilityPower.Total * 0.6f;
            var damage = 80f + ((spell.CastInfo.SpellLevel - 1) * 45) + APratio;
            var StacksPerLevel = spell.CastInfo.SpellLevel;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            if (ownerSkinID == 8)
            {
                AddParticleTarget(owner, target, "Veigar_Skin08_Q_tar.troy", target, lifetime: 1f);
            }
            else
            {
                AddParticleTarget(owner, target, "Veigar_Base_Q_tar.troy", target, lifetime: 1f);
            }

            if (target.IsDead)
            {
                if (target is IChampion)
                {
                    var buffer = owner.Stats.AbilityPower.FlatBonus;

                    statsModifier.AbilityPower.FlatBonus = owner.Stats.AbilityPower.FlatBonus + (StacksPerLevel + 2) - buffer;
                    owner.AddStatModifier(statsModifier);
                }
                else
                {
                    var buffer = owner.Stats.AbilityPower.FlatBonus;

                    statsModifier.AbilityPower.FlatBonus = owner.Stats.AbilityPower.FlatBonus + 1f - buffer;
                    owner.AddStatModifier(statsModifier);
                }
                if (ownerSkinID == 8)
                {
                    AddParticleTarget(owner, owner, "Veigar_Skin08_Q_powerup.troy", owner, lifetime: 1f);
                }
                else
                {
                    AddParticleTarget(owner, owner, "Veigar_Base_Q_powerup.troy", owner, lifetime: 1f);
                }
            }
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {

            
        }
    }
}