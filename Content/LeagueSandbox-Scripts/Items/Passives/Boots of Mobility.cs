using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.API;
using GameServerCore.Domain;

namespace ItemPassives
{
    //This is doran's ring ID, I'm just changing it's script to show how it would be
    public class ItemID_3117 : IItemScript
    {
        IObjAiBase Owner;

        //Created 2 variables, one to handle time since last getting hit, and one to handle if did or not get hit
        float damageTimer = 5000f;
        bool TookDamage = false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public void OnActivate(IObjAiBase owner)
        {
            Owner = owner;

            //Add a Listener to when taking damage
            ApiEventManager.OnTakeDamage.AddListener(this, owner, OnTakeDamage, false);
            //PRETEND THERES AN ADD LISTENER TO ONDEALDAMGE HERE

            //Sets the move speed debuff
            StatsModifier.MoveSpeed.FlatBonus -= 80;
        }

        //This will get called when taking damage
        public void OnTakeDamage(IDamageData damageData)
        {
            GetInCombat(damageData.Target);
        }
        //This gets called when dealing damage
        public void OnDealDamage(IDamageData damageData)
        {
            GetInCombat(damageData.Attacker);
        }
        public void GetInCombat(IAttackableUnit unit)
        {
            //Set the time since last took damage to 5 seconds
            damageTimer = 5000f;
            //If Didnt already take damage, will remove the Extra moveSpeed Stat and then set TookDamage to true
            if (!TookDamage)
            {
                unit.AddStatModifier(StatsModifier);
                TookDamage = true;
            }
        }

        //This gets called when selling the item
        public void OnDeactivate(IObjAiBase owner)
        {
            //Removes TakeDamage listener
            ApiEventManager.OnTakeDamage.RemoveListener(this);
            //If didnt take damage (which means you have the extra move speed applied) removes extra move speed
            if (TookDamage)
            {
                owner.RemoveStatModifier(StatsModifier);
            }
            //Base move speed will be handled automatically because it uses the default "StatsModifier" from the script
        }

        //This gets called every single server tick and "diff" is the time between those ticks
        public void OnUpdate(float diff)
        {   
            //If you took damage...
            if (TookDamage)
            {
                //Reduce the damageTimer by the time that has passed
                damageTimer -= diff;

                //If the damageTimer reached 0
                if (damageTimer <= 0 && Owner != null)
                {
                    //Set took damage to false and add the extra move speed back
                    TookDamage = false;
                    Owner.RemoveStatModifier(StatsModifier);
                }
            }
        }
    }
}