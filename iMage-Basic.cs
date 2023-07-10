using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using robotManager.Helpful;
using robotManager.Products;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using System.Configuration;
using System.ComponentModel;
using System.IO;
using robotManager;
using System.Collections.Generic;
using Timer = robotManager.Helpful.Timer;

public class Main : ICustomClass
{
    public float Range { get { return 29f; } }
    private Random rand;
    private bool iMageLaunched;
    private ulong _lastTarget;

    public void Initialize() // When product started, initialize and launch Fightclass
    {
        iMageLaunched = true;
        Logging.Write("iMage custom build initialized.");

        wManager.wManagerSetting.CurrentSetting.SkipRegenIfFlaggedInCombat = false;
        rand = new Random();
        Start();
    }

    public void Dispose() // When product stopped
    {
        iMageLaunched = false;
        Logging.Write("iMage custom build stopped.");
    }

    public void ShowConfiguration() // When use click on Fight class settings
    {
        iMageSettings.Load();
        iMageSettings.CurrentSetting.ToForm();
        iMageSettings.CurrentSetting.Save();
    }

    // SPELLS:
    public Spell ConjureWater = new Spell("Conjure Water");
    public Spell ConjureFood = new Spell("Conjure Food");
    public Spell ConjureManaRuby = new Spell("Conjure Mana Ruby");
    public Spell ConjureManaCitrine = new Spell("Conjure Mana Citrine");
    public Spell ConjureManaJade = new Spell("Conjure Mana Jade");
    public Spell ConjureManaAgate = new Spell("Conjure Mana Agate");
    public Spell IceArmor = new Spell("Ice Armor");
    public Spell FrostArmor = new Spell("Frost Armor");
    public Spell ArcaneInt = new Spell("Arcane Intellect");
    public Spell ManaShield = new Spell("Mana Shield");
    public Spell IceBarrier = new Spell("Ice Barrier");
    public Spell DampenMagic = new Spell("Dampen Magic");
    public Spell FrostBolt = new Spell("Frostbolt");
    public Spell FireBolt = new Spell("Fireball");
    public Spell FrostNova = new Spell("Frost Nova");
    public Spell Blink = new Spell("Blink");
    public Spell FireBlast = new Spell("Fire Blast");
    public Spell CoC = new Spell("Cone of Cold");
    public Spell CounterSpell = new Spell("Counterspell");
    public Spell PolyMorph = new Spell("Polymorph");
    public Spell Shoot = new Spell("Shoot");


    bool UseBuffDampen = true; // This will buff us with Dampen Magic.
    bool UseFBShatter = true; // This will shatter combo with Fire Blast if Cone of Cold is on cooldown.

    enum fightingStateEnum { SpamFrostBolt, TargetLowHealth, LowPlayerHealth, LowPlayerMana, Silence, TargetClose };

    fightingStateEnum fightState = fightingStateEnum.SpamFrostBolt;

    private void GetFightingState()
    {
        if (fightState == fightingStateEnum.LowPlayerMana && ObjectManager.Me.ManaPercentage >= 90)
        {
            fightState = fightingStateEnum.SpamFrostBolt;
            return;
        }
        else if (ObjectManager.Me.ManaPercentage < 30)
        {
            fightState = fightingStateEnum.LowPlayerMana;
            return;
        }
        else if (ObjectManager.Me.HealthPercent <= 15 && !ObjectManager.Me.HaveBuff("Mana Shield") && ManaShield.KnownSpell)
        {
            fightState = fightingStateEnum.LowPlayerHealth;
            return;
        }
        else if (ObjectManager.Target.IsCast && CounterSpell.IsSpellUsable)
        {
            fightState = fightingStateEnum.Silence;
            return;
        }
        else if (FrostNova.IsSpellUsable && ObjectManager.Target.GetDistance <= 9 && !ObjectManager.Target.HaveBuff("Frost Nova") && !ObjectManager.Target.HaveBuff("Frostbite") && ObjectManager.Target.HealthPercent >= 20)
        {
            fightState = fightingStateEnum.TargetClose;
            return;
        }
        else if (FireBlast.IsSpellUsable && ObjectManager.Target.HealthPercent <= 20)
        {
            fightState = fightingStateEnum.TargetLowHealth;
            return;
        }

        fightState = fightingStateEnum.SpamFrostBolt;
        return;
    }



    public string[] drinkName =
        {
            "", "Conjured Water", "Conjured Fresh Water",
            "Conjured Purified Water", "Conjured Spring Water",
            "Conjured Mineral Water", "Conjured Sparkling Water",
            "Conjured Crystal Water"
        };
    public string[] foodName =
        {
            "", "Conjured Muffin", "Conjured Bread",
            "Conjured Rye", "Conjured Pumpernickel",
            "Conjured Sourdough", "Conjured Sweet Roll",
            "Conjured Cinnamon Roll"
        };

    bool CheckedWaterRank = false;
    bool CheckedFoodRank = false;
    int waterRank;
    int foodRank;


    public void CheckConjureWaterRank()
    {
        if (ConjureWater.KnownSpell && !CheckedWaterRank && !ObjectManager.Me.IsDeadMe && !Fight.InFight)
        {
            foreach (WoWItem item in Bag.GetBagItem())
            {
                if (item.GetItemInfo.ItemName == drinkName[7])
                {
                    int drinkNumber = 7;
                    if (waterRank < drinkNumber)
                    {
                        waterRank = 7;
                    }
                    CheckedWaterRank = true;
                }
                else if (item.GetItemInfo.ItemName == drinkName[6])
                {
                    int drinkNumber = 6;
                    if (waterRank < drinkNumber)
                    {
                        waterRank = 6;
                    }
                    CheckedWaterRank = true;
                }
                else if (item.GetItemInfo.ItemName == drinkName[5])
                {
                    int drinkNumber = 5;
                    if (waterRank < drinkNumber)
                    {
                        waterRank = 5;
                    }
                    CheckedWaterRank = true;
                }
                else if (item.GetItemInfo.ItemName == drinkName[4])
                {
                    int drinkNumber = 4;
                    if (waterRank < drinkNumber)
                    {
                        waterRank = 4;
                    }
                    CheckedWaterRank = true;
                }
                else if (item.GetItemInfo.ItemName == drinkName[3])
                {
                    int drinkNumber = 3;
                    if (waterRank < drinkNumber)
                    {
                        waterRank = 3;
                    }
                    CheckedWaterRank = true;
                }
                else if (item.GetItemInfo.ItemName == drinkName[2])
                {
                    int drinkNumber = 2;
                    if (waterRank < drinkNumber)
                    {
                        waterRank = 2;
                    }
                    CheckedWaterRank = true;
                }
                else if (item.GetItemInfo.ItemName == drinkName[1])
                {
                    int drinkNumber = 1;
                    if (waterRank < drinkNumber)
                    {
                        waterRank = 1;
                    }
                    CheckedWaterRank = true;
                }
            }

            if (!CheckedWaterRank)
            {
                SpellManager.CastSpellByNameLUA("Conjure Water");
            }
        }
        else if (!ConjureWater.KnownSpell && !ConjureWater.KnownSpell)
        {
            CheckedWaterRank = false;
            waterRank = 0;
            return;
        }
    }

    public void CheckConjureFoodRank()
    {
        //ItemsManager.GetItemCountById(11188) < 7

        if (ConjureFood.KnownSpell && !CheckedFoodRank)
        {
            foreach (WoWItem item in Bag.GetBagItem())
            {
                if (item.GetItemInfo.ItemName == foodName[7] && !CheckedFoodRank)
                {
                    int foodNumber = 7;
                    if (foodRank < foodNumber)
                    {
                        foodRank = 7;
                    }
                    CheckedFoodRank = true;
                }
                else if (item.GetItemInfo.ItemName == foodName[6] && !CheckedFoodRank)
                {
                    int foodNumber = 6;
                    if (foodRank < foodNumber)
                    {
                        foodRank = 6;
                    }
                    CheckedFoodRank = true;
                }
                else if (item.GetItemInfo.ItemName == foodName[5] && !CheckedFoodRank)
                {
                    int foodNumber = 5;
                    if (foodRank < foodNumber)
                    {
                        foodRank = 5;
                    }
                    CheckedFoodRank = true;
                }
                else if (item.GetItemInfo.ItemName == foodName[4] && !CheckedFoodRank)
                {
                    int foodNumber = 4;
                    if (foodRank < foodNumber)
                    {
                        foodRank = 4;
                    }
                    CheckedFoodRank = true;
                }
                else if (item.GetItemInfo.ItemName == foodName[3] && !CheckedFoodRank)
                {
                    int foodNumber = 3;
                    if (foodRank < foodNumber)
                    {
                        foodRank = 3;
                    }
                    CheckedFoodRank = true;
                }
                else if (item.GetItemInfo.ItemName == foodName[2] && !CheckedFoodRank)
                {
                    int foodNumber = 2;
                    if (foodRank < foodNumber)
                    {
                        foodRank = 2;
                    }
                    CheckedFoodRank = true;
                }
                else if (item.GetItemInfo.ItemName == foodName[1] && !CheckedFoodRank)
                {
                    int foodNumber = 1;
                    if (foodRank < foodNumber)
                    {
                        foodRank = 1;
                    }
                    CheckedFoodRank = true;
                }
            }
            if (!CheckedFoodRank)
            {
                SpellManager.CastSpellByNameLUA("Conjure Food");
            }
        }
        else if (!ConjureFood.KnownSpell && !ConjureFood.KnownSpell)
        {
            CheckedFoodRank = false;
            foodRank = 0;
            return;
        }
    }

    public bool ConjureRefreshments()
    {
        // Does Bot Know Spell - RunCode
        //!wManager.Wow.Helpers.SpellManager.KnowSpell(688)


        if (ConjureWater.KnownSpell)
        {
            if (ItemsManager.GetItemCountByNameLUA(drinkName[waterRank]) <= 4)
            {
                CheckedWaterRank = false;
                SpellManager.CastSpellByNameLUA("Conjure Water");
            }
        }

        if (ConjureFood.KnownSpell)
        {
            if (ItemsManager.GetItemCountByNameLUA(foodName[foodRank]) <= 4)
            {
                CheckedFoodRank = false;
                SpellManager.CastSpellByNameLUA("Conjure Food");
            }
        }

        return true;
    }


    public bool BuffInt()
    {
        if (ArcaneInt.KnownSpell)
        {
            if (!ObjectManager.Me.HaveBuff("Arcane Intellect"))
            {
                Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
                SpellManager.CastSpellByNameLUA("Arcane Intellect", true);
                return false;
            }
        }
        return true;
    }


    public bool BuffArmor()
    {
        if (IceArmor.KnownSpell)
        {
            if (!ObjectManager.Me.HaveBuff("Ice Armor"))
            {
                SpellManager.CastSpellByNameLUA("Ice Armor");
                return false;
            }
        }
        else if (!ObjectManager.Me.HaveBuff("Frost Armor"))
        {
            SpellManager.CastSpellByNameLUA("Frost Armor");
            return false;
        }
        return true;
    }


    public bool BuffDampen()
    {
        if (DampenMagic.KnownSpell && UseBuffDampen == true)
        {
            if (!ObjectManager.Me.HaveBuff("Dampen Magic"))
            {
                Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
                SpellManager.CastSpellByNameLUA("Dampen Magic");
                return false;
            }
        }
        return true;
    }


    public void BuffShield()
    {
        if (ManaShield.KnownSpell && !ObjectManager.Me.HaveBuff("Mana Shield") && ObjectManager.Me.ManaPercentage > 25)
            SpellManager.CastSpellByNameLUA("Mana Shield");

        if (IceBarrier.KnownSpell && !ObjectManager.Me.HaveBuff("Ice Barrier"))
        {
            if (IceBarrier.KnownSpell)
                SpellManager.CastSpellByNameLUA("Ice Barrier");
            return;
        }
    }

    public void CastNova()
    {
        if (FrostNova.KnownSpell)
        {
            SpellManager.CastSpellByNameLUA("Frost Nova");

            if(ObjectManager.Target.GetDistance <= 9)
                Move.Backward(Move.MoveAction.PressKey, 500);
        }
    }

    public void CastInstant()
    {
        if (FireBlast.KnownSpell)
        {
            SpellManager.CastSpellByNameLUA("Fire Blast");
            return;
        }
    }

    public void CastSpam()
    {
        if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && Fight.InFight)
        {
            if (Lua.LuaDoString<bool>(@"return (UnitIsTapped(""target"")) and (not UnitIsTappedByPlayer(""target""));"))
            {
                Fight.StopFight();
                Lua.LuaDoString("ClearTarget();");
                Logging.Write("iMage Stopped attacking target tapped by other player");
                Thread.Sleep(1000);
                return;
            }
        }

        if (ObjectManager.Target.HaveBuff("Frost Nova") || ObjectManager.Target.HaveBuff("Frostbite"))
        {
            //cone of cold
            if (CoC.KnownSpell)
            {
                if (CoC.KnownSpell && ObjectManager.Target.GetDistance <= 10)
                {
                    SpellManager.CastSpellByNameLUA("Frostbolt");
                    SpellManager.CastSpellByNameLUA("Cone of Cold");
                    return;
                }
                else if (!CoC.KnownSpell && UseFBShatter == true)
                {
                    SpellManager.CastSpellByNameLUA("Frostbolt");
                    SpellManager.CastSpellByNameLUA("Fire Blast");
                    return;
                }
                else
                    SpellManager.CastSpellByNameLUA("Frostbolt");
                return;
            }
            else
            {
                SpellManager.CastSpellByNameLUA("Frostbolt");
                return;
            }
        }
        else if (!FrostBolt.KnownSpell)
        {
            if (FireBolt.KnownSpell)
            {
                SpellManager.CastSpellByNameLUA("Fireball");
                return;
            }
        }
        else if (FrostBolt.KnownSpell)
        {

            if(rand.Next(0, 2) == 1 || ObjectManager.Me.ManaPercentage <= 40)
            {
                //this works but it isnt optimal
                if (SpellManager.SpellUsableLUA("Shoot"))
                {
                    SpellManager.CastSpellByNameLUA("Shoot");
                    //Logging.Write("Shoot");
                }

                Thread.Sleep(2000);
            }
            else
            {
                if (ObjectManager.Target.GetDistance <= 40)
                {
                    SpellManager.CastSpellByNameLUA("Frostbolt");
                    Thread.Sleep(2000);
                }
            }

            return;
        }
    }

    public void CastSilence()
    {
        if (CounterSpell.KnownSpell)
        {
            Move.Backward(Move.MoveAction.PressKey, 50 * 2);
            SpellManager.CastSpellByNameLUA("Counterspell");
            return;
        }
    }

    public void RegenMana()
    {
        try
        {
            if (ObjectManager.Me.InCombatFlagOnly)
            {
                //Logging.Write("iMage - RegenMana() - In combat");
                return;
            }

            //stop moving
            if (ObjectManager.Me.GetMove)
            {
                Logging.Write("iMage - RegenMana() - Stopping movement");
                MovementManager.StopMoveTo(true, 500);

                //force sit down
                if (!wManager.Wow.ObjectManager.ObjectManager.Me.IsSitting)
                {
                    Logging.Write("iMage - RegenMana() - SitStandOrDescend()");
                    wManager.Wow.Helpers.Move.SitStandOrDescend();
                    Thread.Sleep(1000);
                }

                //alternative approach
                //Lua.LuaDoString("DoEmote('SIT')");﻿

                //if you wanna stand up specifically
                //Lua.LuaDoString("DoEmote('STAND')");
            }

            if (ConjureManaAgate.KnownSpell)
            {
                Logging.Write("Conjure Mana Agate");
                SpellManager.CastSpellByNameLUA("Conjure Mana Agate");
                //5514
            }
            else if (ConjureManaRuby.KnownSpell)
            {
                Logging.Write("Conjure Mana Ruby");
                SpellManager.CastSpellByNameLUA("Conjure Mana Ruby");
                //8008
            }
            else if (ConjureManaCitrine.KnownSpell)
            {
                Logging.Write("Conjure Mana Citrine");
                SpellManager.CastSpellByNameLUA("Conjure Mana Citrine");
                //8007
            }
            else if (ConjureManaJade.KnownSpell)
            {
                Logging.Write("Conjure Mana Jade");
                SpellManager.CastSpellByNameLUA("Conjure Mana Jade");
                //5513
            }
            else
            {
                /*
				8075 	Conjured Sourdough
				8076 	Conjured Sweet Roll
				8077 	Conjured Mineral Water
				8078 	Conjured Sparkling Water
				8079 	Conjured Crystal Water
				5350 	Conjured Water
                5349    Conjured Muffin
				*/
                Logging.Write("UseItem(5350) and rest.");
                wManager.Wow.Helpers.Fight.StopFight();
                Lua.LuaDoString("ClearTarget();");

                ItemsManager.UseItem(5350);
                ItemsManager.UseItem(5349);
                Thread.Sleep(15000);
            }


        }
        catch(Exception e)
        {
            Logging.WriteError("RegenMana() - Exception: " + e);
        }
    }

    public void Start()
    {
        Logging.Write("iMage Loaded");

        /*
        try
        {
            var best = Bag.GetBagItem()
                .Where(i => i != null && !string.IsNullOrWhiteSpace(i.Name) && ItemsManager.GetItemSpell(i.Name) == SpellListManager.SpellNameInGameByName("Drink"consumableType.ToString()) && i.GetItemInfo.ItemMinLevel <= ObjectManager.Me.Level)
                .OrderByDescending(i => i.GetItemInfo.ItemLevel)
                .FirstOrDefault();
            if (best != null && best.IsValid && !string.IsNullOrWhiteSpace(best.Name))
            {
                Logging.Write("ITEMS=" + best.Name);
            }
        }
        catch(Exception e)
        {
            Logging.Write("Exception in Start() : " + e);
        }
        */

        while (iMageLaunched)
        {
            if (!Products.InPause)
            {
                GetFightingState();
                if (!ObjectManager.Me.IsDeadMe)
                {
                    GetFightingState();
                    if (!ObjectManager.Me.IsMounted && BuffRotation() == false)
                    {
                        BuffRotation();
                    }
                    
                    //CheckConjureWaterRank();
                    //CheckConjureFoodRank();

                    if (!Fight.InFight && ObjectManager.Me.ManaPercentage <= 30)
                    {
                        Thread.Sleep(1500);
                        Logging.Write("Low mana. Fight state: " + fightState);
                    }
                    else
                    {
                        fightState = fightingStateEnum.SpamFrostBolt;
                    }

                    if (Fight.InFight && ObjectManager.Me.Target > 0)
                    {
                        BuffShield();
                        GetFightingState();
                        CombatRotation();
                    }
                }
            }
            Thread.Sleep(25);
        }
        Logging.Write("iMage is now stopped.");
    }

    public bool BuffRotation()
    {
        //ConjureRefreshments();
        BuffInt();
        BuffArmor();
        BuffDampen();
        if (/*ConjureRefreshments() == true &&*/ BuffInt() == true && BuffArmor() == true && BuffDampen() == true)
            return true;
        else
            return false;
    }

    // ObjectManager.Me.HaveBuff(71)

    public void CombatRotation()
    {
        switch (fightState)
        {

            case fightingStateEnum.LowPlayerHealth:
                {
                    BuffShield();
                    break;
                }
            case fightingStateEnum.Silence:
                {
                    CastSilence();
                    break;
                }
            case fightingStateEnum.TargetClose:
                {
                    CastNova();
                    break;
                }
            case fightingStateEnum.TargetLowHealth:
                {
                    // Check number of enemies attacking you or attacking pet
                    //ObjectManager.GetWoWUnit﻿Attackables().Where(x => x.Target == ObjectManager.Me.GetBaseAddress || x.Target == ObjectManager.Pet.GetBaseAddress).Count() >= 2﻿)

                    // Move Backward (RunCode)
                    //wManager.Wow.Helpers.Move.Backward(Move.MoveAction.PressKey,2000);

                    // Move Forward (RunCode)
                    //wManager.Wow.Helpers.Move.Forward(Move.MoveAction.PressKey,2000);

                    CastInstant();
                    break;
                }
            case fightingStateEnum.LowPlayerMana:
                {
                    //RegenMana();
                    break;
                }
            case fightingStateEnum.SpamFrostBolt:
                {
                    CastSpam();
                    break;
                }
        }
    }


    [Serializable]
    public class iMageSettings : Settings
    {
        public iMageSettings()
        {
            ConfigWinForm(new System.Drawing.Point(400, 400), "iMage" + Translate.Get("Settings"));
        }

        public static iMageSettings CurrentSetting { get; set; }

        public bool Save()
        {
            try
            {
                return Save(AdviserFilePathAndName("iMage-Mage", ObjectManager.Me.Name + "." + Usefuls.RealmName));
            }
            catch (Exception e)
            {
                Logging.WriteError("iMageSettings > Save(): " + e);
                return false;
            }
        }

        public static bool Load()
        {
            try
            {
                if (File.Exists(AdviserFilePathAndName("iMage-Mage", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
                {
                    CurrentSetting =
                        Load<iMageSettings>(AdviserFilePathAndName("iMage-Mage",
                                                                    ObjectManager.Me.Name + "." + Usefuls.RealmName));
                    return true;
                }
                CurrentSetting = new iMageSettings();
            }
            catch (Exception e)
            {
                Logging.WriteError("iMageSettings > Load(): " + e);
            }
            return false;
        }
    }
}