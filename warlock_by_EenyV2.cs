using System;
using System.Threading;
using robotManager.Helpful;
using robotManager.Products;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using Timer = robotManager.Helpful.Timer;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Diagnostics;
using robotManager;
using System.IO;
using wManager.Wow;
using System.Linq;
using System.Threading.Tasks;

public class Main : ICustomClass
{
    public float Range { get { return 25.0f; } }
    private Random _r = new Random();
    private readonly uint _wowBase = (uint)Memory.WowMemory.Memory.GetProcess().MainModule.BaseAddress;
    private bool _isLaunched;
    private ulong _lastTarget;
    private ulong _currentTarget;
    private uint _target;
    uint oldTarget;
 
    public void Initialize() // When product started, initialize and launch Fightclass
    {
        _isLaunched = true;
        Logging.Write("Lock FC Is initialized.");
        Rotation();
    }
 
    public void Dispose() // When product stopped
    {
        _isLaunched = false;
        Logging.Write("Lock FC Stop in progress.");
    }
     
    public void ShowConfiguration() // When use click on Fight class settings
    {
    }
 
 
    // SPELLS:
    // Healthstone:
    public  Spell healthStone1 = new Spell("Create Healthstone (Minor)");
    public  Spell healthStone2 = new Spell("Create Healthstone (Lesser)");	
    public  Spell healthStone3 = new Spell("Create Healthstone");
    public  Spell healthStone4 = new Spell("Create Healthstone (Greater)");
    public  Spell healthStone5 = new Spell("Create Healthstone (Major)");	
	
    // SPELLS:
    //Pet:
    private Spell SummonImp = new Spell("Summon Imp");
    private Spell SummonVoid = new Spell("Summon Voidwalker");
    public  Spell HealthFunnel = new Spell("Health Funnel");

    // Buff:
    public Spell DemonSkin = new Spell("Demon Skin");
    public Spell DemonArmor = new Spell("Demon Armor");
    public Spell LifeTap = new Spell("Life Tap");
    public Spell UnendingBreath = new Spell("Unending Breath");

    // Close Combat:
    public Spell Sacrifice = new Spell("Sacrifice");
    public Spell Shadowburn = new Spell("Shadowburn");
    public Spell ShadowBolt = new Spell("Shadow Bolt");
    public WoWSpell Immolate = new WoWSpell("Immolate", 3500);
    public WoWSpell Corruption = new WoWSpell("Corruption", 2500);
    public WoWSpell SiphonLife = new WoWSpell("Siphon Life", 2500);
    public WoWSpell CurseofAgony = new WoWSpell("Curse of Agony", 2500);
    public Spell Shoot = new Spell("Shoot");
    public Spell DrainLife = new Spell("Drain Life");
    public Spell DrainSoul = new Spell("Drain Soul");
    public Spell DeathCoil = new Spell("Death Coil");
    public Spell Fear = new Spell("Fear");



     

    public bool WandActive()
    {
        return Memory.WowMemory.Memory.ReadInt32(_wowBase + 0x7E0BA0) > 0;
    }
 
    internal void Rotation()
    {
        Logging.Write("Lock FC started.");
        while (_isLaunched)
        {
            try
            {
                if (!Products.InPause)
                {
                    if (!ObjectManager.Me.IsDeadMe)
                    {
						PetManager();
                        Buff();
						Healstone();
						UseScroll();
						if (ObjectManager.Pet.IsValid && ObjectManager.Me.Target > 0 && ObjectManager.Target.IsAttackable)
                        {
                            Lua.LuaDoString("PetAttack();");
                        }
                        if (Fight.InFight && ObjectManager.Me.Target > 0)
                        {
                            CombatRotation();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.WriteError("Lock FC  ERROR: " + e);
            }
 
            Thread.Sleep(100); // Pause 10 ms to reduce the CPU usage.
        }
        Logging.Write("Lock FC  Is now stopped.");
    }

	internal void UseScroll()
	        {
				if (!Fight.InFight)
					{
						// Agi scroll- send to pet
						if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(3012) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(3012);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(1477) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(1477);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(4425) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(4425);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(10309) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(10309);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(27498) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(27498);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(33457) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(33457);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(43463) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(43463);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(43464) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(43464);
						}
						
						// int scroll- buff yo self
						
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(955) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(955);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(2290) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(2290);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(4419) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(4419);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(10308) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(10308);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(27499) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(27499);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(33458) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(33458);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(37091) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(37091);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(37092) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(37092);
						}
						
						// scroll of protection - sent to pet
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(3013) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(3013);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(1478) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(1478);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(4421) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(4421);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(10305) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(10305);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(27500) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(27500);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(33459) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(33459);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(43467) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(43467);
						}
						
						// scroll of spirit- buff yourself
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(1181) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(1181);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(1712) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(1712);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(4424) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(4424);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(10306) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(10306);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(27501) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(27501);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(33460) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(33460);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(37097) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(37097);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(37098) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(37098);
						}
						
						// stamina  buff self
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(1180) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(1180);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(1711) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(1711);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(4422) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(4422);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(10307) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(10307);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(27502) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(27502);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(33461) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(33461);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(37093) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(37093);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(37094) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Me.GetBaseAddress);
							ItemsManager.UseItem(37094);
						}
						
						//strength - send to pet
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(954) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(954);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(2289) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(2289);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(4426) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(4426);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(10310) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(10310);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(27503) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(27503);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(33462) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(33462);
						}
						else if (!ObjectManager.Me.IsMounted && ItemsManager.HasItemById(43465) && !ObjectManager.Me.IsDeadMe)
						{
							Interact.InteractGameObject(ObjectManager.Pet.GetBaseAddress);
							ItemsManager.UseItem(43465);
						}
				}
            }
	internal void Healstone()
		{if (!Fight.InFight)
            {
				     // health stone 1
				if (ItemsManager.GetItemCountByNameLUA("Soul Shard") >= 3 && healthStone1.KnownSpell && ItemsManager.GetItemCountByNameLUA("Minor Healthstone") < 1 && !healthStone2.KnownSpell)
				{	
					wManager.Wow.Helpers.SpellManager.CastSpellByNameLUA("Create Healthstone (Minor)()");
					Thread.Sleep(Usefuls.Latency);
					Usefuls.WaitIsCasting();
				}
				
					// health stone 2
				if (ItemsManager.GetItemCountByNameLUA("Soul Shard") >= 3 && healthStone2.KnownSpell && ItemsManager.GetItemCountByNameLUA("Lesser Healthstone") < 1 && !healthStone3.KnownSpell)
				{	
					wManager.Wow.Helpers.SpellManager.CastSpellByNameLUA("Create Healthstone (Lesser)()");
					Thread.Sleep(Usefuls.Latency);
					Usefuls.WaitIsCasting();
				}
				
					// health stone 3			
				if (ItemsManager.GetItemCountByNameLUA("Soul Shard") >= 3 && healthStone3.KnownSpell && ItemsManager.GetItemCountByNameLUA("Healthstone") < 1 && !healthStone4.KnownSpell)
				{	
					healthStone3.Launch();
					Thread.Sleep(Usefuls.Latency);
					Usefuls.WaitIsCasting();
				}
				
					// health stone 4		
				if (ItemsManager.GetItemCountByNameLUA("Soul Shard") >= 3 && healthStone4.KnownSpell && ItemsManager.GetItemCountByNameLUA("Greater Healthstone") < 1 && !healthStone5.KnownSpell)
				{	
					wManager.Wow.Helpers.SpellManager.CastSpellByNameLUA("Create Healthstone (Greater)()");
					Thread.Sleep(Usefuls.Latency);
					Usefuls.WaitIsCasting();
				}
				
					// health stone 5			
				if (ItemsManager.GetItemCountByNameLUA("Soul Shard") >= 3 && healthStone5.KnownSpell && ItemsManager.GetItemCountByNameLUA("Major Healthstone") < 1)
				{	
					wManager.Wow.Helpers.SpellManager.CastSpellByNameLUA("Create Healthstone (Major)()");
					Thread.Sleep(Usefuls.Latency);
					Usefuls.WaitIsCasting();
				}
				
			}
    }
	
	
	internal void PetManager()
    {
			    // call void
			if (!ObjectManager.Pet.IsValid && SummonVoid.KnownSpell && !ObjectManager.Me.IsDeadMe && !ObjectManager.Me.IsMounted)
			{	
				SummonVoid.Launch(true);
				Thread.Sleep(Usefuls.Latency + 12000);
			}
				// Call imp- assumming void isnt known
			if (!ObjectManager.Pet.IsValid && SummonImp.KnownSpell && !SummonVoid.KnownSpell && !ObjectManager.Me.IsDeadMe && !ObjectManager.Me.IsMounted)
			{
				SummonImp.Launch(true);
				Thread.Sleep(Usefuls.Latency + 12000);
			}
				//try waiting around if no pet??
			if (!ObjectManager.Pet.IsValid && SummonImp.KnownSpell && !ObjectManager.Me.IsDeadMe && !ObjectManager.Me.IsMounted)
			{
                Thread.Sleep(1000);
			}
		
    }


    public void Buff()
    {
        // demon skin
        if (DemonSkin.KnownSpell && !ObjectManager.Me.HaveBuff("Demon Skin") && !DemonArmor.KnownSpell)
        {
            DemonSkin.Launch();
        }

        // demon armor
        if (DemonArmor.KnownSpell && !ObjectManager.Me.HaveBuff("Demon Armor") && ObjectManager.Pet.IsValid)
        {
            DemonArmor.Launch();
        }

		// Lifetap
        if (LifeTap.KnownSpell && ObjectManager.Me.HealthPercent >= 60 && ObjectManager.Me.ManaPercentage < 40)
        {
            LifeTap.Launch();
        }

        //Health Funnel
        if (ObjectManager.Pet.IsValid && ObjectManager.Me.IsAlive &&  HealthFunnel.KnownSpell && HealthFunnel.IsDistanceGood && ObjectManager.Pet.HealthPercent <=50 && ObjectManager.Me.HealthPercent >= 60)
        {
            HealthFunnel.Launch();
			Thread.Sleep(Usefuls.Latency);
            Usefuls.WaitIsCasting();

		}
		
        if (UnendingBreath.KnownSpell && !ObjectManager.Me.HaveBuff("Unending Breath") && ObjectManager.Me.IsSwimming)
        {
            UnendingBreath.Launch();
        }			
			
        
		
    }
	
	
	private bool canDoT(WoWUnit unit)
    {
        return unit.CreatureTypeTarget != "Elemental" && unit.CreatureTypeTarget != "Mechanical";
    }

    public int EnemiesAttackingMeCount()
    {
        return ObjectManager.GetWoWUnitHostile().Where(w => w.IsTargetingMe && w.GetDistance <= 6).Count();
    }

    public int EnemiesAttackingPetCount()
    {
        return ObjectManager.GetWoWUnitHostile().Where(w => w.IsTargetingMyPet && w.GetDistance >= 6).Count();
    }
 

    internal void CombatRotation()
    {
				// auto tag avoid 
         if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && Fight.InFight)
        {
            if (Lua.LuaDoString<bool>(@"return (UnitIsTapped(""target"")) and (not UnitIsTappedByPlayer(""target""));"))
            {
                Fight.StopFight();
                Lua.LuaDoString("ClearTarget();");
                System.Threading.Thread.Sleep(400);
            }
		}

		//if one or more mobs on us and we're fairly healthly, target & pet attack
		if (EnemiesAttackingMeCount() >= 1 && ObjectManager.Me.HealthPercent <= 60 && ObjectManager.Pet.IsAlive) {
			//Target closest enemy attacking us
			var closestEnemy = ObjectManager.GetWoWUnitHostile().Where(e => e.IsAttackable && e.IsTargetingMe).OrderBy(e => e.GetDistance2D).FirstOrDefault();
            if (closestEnemy != null) {
                Logging.Write("Targeting closest enemy, lets see what happens");
                Interact.InteractGameObject(closestEnemy.GetBaseAddress);
                System.Threading.Thread.Sleep(400);
            	Lua.LuaDoString("PetAttack();");    
            	System.Threading.Thread.Sleep(700);
            }		
		}

		
		// disco candy block
		
         if (ItemsManager.GetItemCountByIdLUA(5512) > 0 && ObjectManager.Me.HealthPercent <= 50 && !(wManager.Wow.Helpers.Bag.GetContainerItemCooldown(5512) > 0))
         {
             ItemsManager.UseItem(5511);
             Thread.Sleep(Usefuls.Latency + 100);
         }
		 
         if (ItemsManager.GetItemCountByIdLUA(19007) > 0 && ObjectManager.Me.HealthPercent <= 50 && !(wManager.Wow.Helpers.Bag.GetContainerItemCooldown(19007) > 0))
         {
             ItemsManager.UseItem(19007);
             Thread.Sleep(Usefuls.Latency + 100);
         }
		 
         if (ItemsManager.GetItemCountByIdLUA(5509) > 0 && ObjectManager.Me.HealthPercent <= 50 && !(wManager.Wow.Helpers.Bag.GetContainerItemCooldown(5512) > 0))
         {
             ItemsManager.UseItem(5509);
             Thread.Sleep(Usefuls.Latency + 100);
         }
		 
         if (ItemsManager.GetItemCountByIdLUA(5510) > 0 && ObjectManager.Me.HealthPercent <= 50 && !(wManager.Wow.Helpers.Bag.GetContainerItemCooldown(5512) > 0))
         {
             ItemsManager.UseItem(5510);
             Thread.Sleep(Usefuls.Latency + 100);
         }
		 
         if (ItemsManager.GetItemCountByIdLUA(9421) > 0 && ObjectManager.Me.HealthPercent <= 50 && !(wManager.Wow.Helpers.Bag.GetContainerItemCooldown(5512) > 0))
         {
             ItemsManager.UseItem(9421);
             Thread.Sleep(Usefuls.Latency + 100);
         }
		
		if (Sacrifice.KnownSpell && ObjectManager.Pet.IsValid && ObjectManager.Me.IsAlive  && ObjectManager.Pet.HealthPercent <=15)
        {
            Sacrifice.Launch();
            Thread.Sleep(Usefuls.Latency);
            Usefuls.WaitIsCasting();

        }
		if (Sacrifice.KnownSpell && ObjectManager.Pet.IsValid && ObjectManager.Me.IsAlive  && ObjectManager.Me.HealthPercent <= 25)
        {
            Sacrifice.Launch();
            Thread.Sleep(Usefuls.Latency);
            Usefuls.WaitIsCasting();

        }
		
		// Lifetap
        if (LifeTap.KnownSpell && ObjectManager.Me.HealthPercent >= 70 && ObjectManager.Me.ManaPercentage < 60)
        {
            LifeTap.Launch();
        }


				
		//actual spells that do things block
		
        if (ObjectManager.Pet.IsValid && ObjectManager.Me.IsAlive &&  HealthFunnel.KnownSpell && HealthFunnel.IsDistanceGood && ObjectManager.Pet.HealthPercent <=30 && ObjectManager.Me.HealthPercent >= 60)
        {
            HealthFunnel.Launch();
			Thread.Sleep(Usefuls.Latency);
            Usefuls.WaitIsCasting();
            //channeling ?

        }
		
		if (Fear.KnownSpell && ObjectManager.Target.GetDistance < 10  && ObjectManager.Me.HealthPercent <= 20 && !ObjectManager.Target.HaveBuff("Fear") && !ObjectManager.Target.IsTargetingMyPet )
        {
            wManager.Wow.Helpers.SpellManager.CastSpellByNameLUA("Fear");
            Thread.Sleep(Usefuls.Latency + 1200);
        }
		


		if (Shadowburn.KnownSpell && ObjectManager.Target.GetDistance < 18 && ObjectManager.Target.HealthPercent <= 20 && ObjectManager.Me.ManaPercentage > 15 && Shadowburn.IsSpellUsable && ItemsManager.GetItemCountByNameLUA("Soul Shard") >= 4)
        {
            Shadowburn.Launch();
            Thread.Sleep(Usefuls.Latency + 1200);
        }
		
		if (CurseofAgony.KnownSpell  && ObjectManager.Target.HealthPercent >= 40 && !ObjectManager.Target.HaveBuff("Curse of Agony") && ObjectManager.Me.ManaPercentage > 15)
        {
            this.CurseofAgony.Launch();
            Thread.Sleep(Usefuls.Latency + 1200);
        }
		
        if (Corruption.KnownSpell  && ObjectManager.Target.HealthPercent >= 40 && !ObjectManager.Target.HaveBuff("Corruption") && ObjectManager.Me.ManaPercentage > 15 && ObjectManager.Target.GetDistance < 25)
        {
            this.Corruption.Launch();
            Thread.Sleep(Usefuls.Latency + 1200);
        }
		
		if (SiphonLife.KnownSpell && ObjectManager.Target.GetDistance < 25 && ObjectManager.Target.HealthPercent >= 40 && !ObjectManager.Target.HaveBuff("Siphon Life") && ObjectManager.Me.ManaPercentage > 15)
        {
            this.SiphonLife.Launch();
            Thread.Sleep(Usefuls.Latency + 1200);
        }



        if (Immolate.KnownSpell && ObjectManager.Target.GetDistance < 25 && ObjectManager.Target.HealthPercent >= 50 && !ObjectManager.Target.HaveBuff("Immolate") && ObjectManager.Me.ManaPercentage > 15 && !SiphonLife.KnownSpell)
        {
            this.Immolate.Launch();
            Thread.Sleep(Usefuls.Latency + 1200);
        }
		
		 if (DrainSoul.KnownSpell && ObjectManager.Target.GetDistance < 25 && ObjectManager.Target.HealthPercent <= 15 && ItemsManager.GetItemCountByNameLUA("Soul Shard") <= 3 )
        {
            DrainSoul.Launch();
			Thread.Sleep(Usefuls.Latency);
            Usefuls.WaitIsCasting();
        }
		
		 if (DrainLife.KnownSpell && ObjectManager.Target.GetDistance < 25 && ObjectManager.Me.ManaPercentage > 40 && ObjectManager.Me.HealthPercent <= 60 && ObjectManager.Target.HealthPercent >= 20 && ObjectManager.Target.GetDistance > 8 && ObjectManager.Target.GetDistance < 20)
        {
            DrainLife.Launch();
			Thread.Sleep(Usefuls.Latency);
            Usefuls.WaitIsCasting();
        }
		
		if (DeathCoil.KnownSpell && ObjectManager.Target.GetDistance < 25 && ObjectManager.Me.ManaPercentage > 20 && ObjectManager.Me.HealthPercent <= 20)
        {
            DeathCoil.Launch();
            Thread.Sleep(Usefuls.Latency + 500);
        }

		if (LifeTap.KnownSpell && ObjectManager.Me.HealthPercent >= 60 && ObjectManager.Me.ManaPercentage < 70)
        {
            LifeTap.Launch();
        }
		

		//low lvl showbolt
        if (!SummonVoid.KnownSpell && ObjectManager.Target.GetDistance < 25 && !Immolate.KnownSpell)
        {
            ShadowBolt.Launch();
            Thread.Sleep(Usefuls.Latency + 1000);
        }
        if (ObjectManager.Me.HaveBuff("Shadow Trance"))
        {
            ShadowBolt.Launch();
            Thread.Sleep(Usefuls.Latency + 1000);
        }
		
		
		
		//shoot
			if (!Lua.LuaDoString<bool>("return IsAutoRepeatAction(" + (SpellManager.GetSpellSlotId(SpellListManager.SpellIdByName("Shoot")) + 1) + ")") && ObjectManager.Me.HealthPercent >= 60 && ObjectManager.Pet.HealthPercent >= 30 && ObjectManager.Me.ManaPercentage >= 60 && (ObjectManager.Target.HaveBuff("Curse of Agony") && canDoT(ObjectManager.Me.TargetObject)))
				{
					if (Shoot.KnownSpell)
						SpellManager.CastSpellByNameLUA("Shoot");
					return;
				}

    }
	    public class WoWSpell : Spell
    {
        private Timer _timer;

        #region Constructor

        /// <summary>
        /// Creates a new instance of the <see cref="WoWSpell"/> class.
        /// </summary>
        /// <param name="spellNameEnglish">The spell name.</param>
        /// <param name="cooldownTimer">The cooldown time.</param>
        public WoWSpell(string spellNameEnglish, double cooldownTimer)
            : base(spellNameEnglish)
        {
            // Set timer
            this._timer = new Timer(cooldownTimer);
        }

        #endregion

        #region Public

        public bool IsReady
        {
            get
            {
                return this._timer.IsReady;
            }
        }

        /// <summary>
        /// Casts the spell if it is ready.
        /// </summary>
        public new void Launch()
        {
            // Is ready?
            if (!this.IsReady)
            {
                // Return
                return;
            }

            // Call launch
            base.Launch();

            // Reset timer
            this._timer.Reset();
        }

        #endregion
    }
}