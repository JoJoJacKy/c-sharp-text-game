using System;
using System.Collections.Generic;
using Game;
using Characters;
using Items;
using EquipmentBuilder;
using PartyBuilder;
using System.Threading;

namespace FinalBattle
{
    class Program
    {
        static void Main(string[] args)
        {
            CreatePlayerParty playerPartyCreator = new CreatePlayerParty();
            List<CharacterObject> playerParty = playerPartyCreator.PlayerPartyInitialization();

            CreateEnemyParty enemyPartyCreator = new CreateEnemyParty();
            List<CharacterObject> skeletonParty = enemyPartyCreator.EnemySkeletonPartyInitialization();
            List<CharacterObject> bossParty = enemyPartyCreator.EnemyBossPartyInitialization();

            MainLoop battle1 = new MainLoop(playerParty, skeletonParty);
            List<CharacterObject> battle1result = battle1.Run();

            MainLoop battle2 = new MainLoop(battle1result, bossParty);
            List<CharacterObject> battle2result = battle2.Run();
        }
    }
}


namespace Game
{
    public class MainLoop
    {
        List<CharacterObject> PlayerParty { get; set; }
        List<CharacterObject> EnemyParty { get; set; }
        List<CharacterObject> AllCharacters = new List<CharacterObject>(); // Need to create a proper turn queue system; For now duct taped up
        Random random = new Random();
        public bool battleIsOver = false;

        //public event Action<CharacterObject, List<CharacterObject>, List<CharacterObject>> CharacterDeath = delegate { };

        public MainLoop(List<CharacterObject> playerParty, List<CharacterObject> enemyParty)
        {
            PlayerParty = playerParty;
            EnemyParty = enemyParty;
        }

        public ActionEnum CharacterActionMenu(CharacterObject character)
        {
            if (character is PlayerCharacter)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine();
                Console.WriteLine("=== ACTION MENU ===");
                foreach (ActionEnum action in character.Actions)
                    Console.WriteLine(">>> " + action);
                Console.Write("Choose an action: ");
                string actionChoice = Console.ReadLine();
                switch (actionChoice)
                {
                    case "attack":
                        return ActionEnum.Attack;
                    case "skip":
                        return ActionEnum.SkipTurn;
                    default:
                        return ActionEnum.SkipTurn;
                }
            }
            else
            {
                int Legnth = character.Actions.Count;
                int randomAINum = random.Next(2);
                Thread.Sleep(500);
                switch (randomAINum)
                {
                    case 0:
                        return ActionEnum.Attack;
                    case 1:
                        return ActionEnum.SkipTurn;
                    default:
                        return ActionEnum.SkipTurn;
                }
                
            }
        }

        public Attacks CharacterAttackMenu(CharacterObject character)
        {
            if (character is PlayerCharacter)
            {
                Console.WriteLine();
                Console.WriteLine("=== ATTACKS AVIALABLE ===");
                foreach (Attacks attack in character.Attacks)
                    Console.WriteLine(">>> " + attack);
                Console.Write("Choose an attack: ");
                string actionChoice = Console.ReadLine();
                switch (actionChoice)
                {
                    case "punch":
                        return Attacks.Punch;
                    case "slash":
                        return Attacks.Slash;
                    default:
                        return Attacks.Punch;
                }
            }
            else
            {
                int randomAINum = random.Next(character.Attacks.Count);
                return character.Attacks[randomAINum];
            }
        }

        public CharacterObject TargetSystem(CharacterObject characterTargeting, List<CharacterObject> playerParty, List<CharacterObject> enemyParty)
        {
            if (characterTargeting.Alignment == AlignmentEnum.Good) // Good character targeting enemyParty
            {
                if (characterTargeting is PlayerCharacter)
                {
                    do
                    {
                        int counter = 0;
                        Console.WriteLine();
                        foreach (CharacterObject character in enemyParty)
                        {
                            Console.WriteLine(">>> " + counter + " " + character.Name);
                            counter++;
                        }
                        Console.WriteLine();
                        Console.Write("Choose an enemy to target: ");
                        int userInput = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine();
                        try
                        {
                            return enemyParty[userInput];
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    while (true);
                }
                else
                {

                    int randomAINum = random.Next(enemyParty.Count);
                    return enemyParty[randomAINum];
                }
            }
            else
            {
                int randomAINum = random.Next(playerParty.Count);
                return playerParty[randomAINum];
            }
        }

        public void AttackSystem(Attacks attack, CharacterObject attacker, CharacterObject target)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            int attackDamage;
            int blockedByArmor = CheckForArmor(target);
            int extraWeaponDamage = CheckForWeapon(attacker);
            int totalDamage;
            switch (attack)
            {
                case Attacks.Punch:
                    attackDamage = 3;
                    totalDamage = attackDamage - blockedByArmor + extraWeaponDamage;
                    if (totalDamage < 0)
                        totalDamage = 0;
                    target.CurrentHP -= totalDamage;
                    Console.WriteLine($"{attacker.Name} hits {target.Name} with a {attack}! {target.Name} was hurt for {totalDamage} damage!");
                    break;
                case Attacks.BoneCrunch:
                    attackDamage = random.Next(3); // Hits for 0 or 1 damage
                    totalDamage = attackDamage - blockedByArmor + extraWeaponDamage;
                    if (totalDamage < 0)
                        totalDamage = 0;
                    target.CurrentHP -= totalDamage;
                    Console.WriteLine($"{attacker.Name} hits {target.Name} with a {attack}! {target.Name} was hurt for {totalDamage} damage!");
                    break;
                case Attacks.Slash:
                    attackDamage = 4;
                    totalDamage = attackDamage - blockedByArmor + extraWeaponDamage;
                    if (totalDamage < 0)
                        totalDamage = 0;
                    target.CurrentHP -= totalDamage;
                    Console.WriteLine($"{attacker.Name} hits {target.Name} with a {attack}! {target.Name} was hurt for {totalDamage} damage!");
                    break;
                case Attacks.Unravel:
                    attackDamage = 5;
                    totalDamage = attackDamage - blockedByArmor + extraWeaponDamage;
                    if (totalDamage < 0)
                        totalDamage = 0;
                    target.CurrentHP -= totalDamage;
                    Console.WriteLine($"{attacker.Name} hits {target.Name} with an {attack}! {target.Name} was hurt for {totalDamage} damage!");
                    break;
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        //public void ConstantCharacterAliveCheck(List<CharacterObject> AllCharacters)
        //{
        //    foreach (CharacterObject character in AllCharacters)
        //    {
        //        if (character.Status == StatusEnum.Dead)
        //            AllCharacters.Remove(character);
        //    }
        //}

        public void TurnSystem(List<CharacterObject> playerParty, List<CharacterObject> enemyParty, List<CharacterObject> allCharacters)
        {
            foreach (CharacterObject character in playerParty)
            {
                if (CheckIfConstantDead(character)) // Prevents Player companions from doing anything if considered dead
                    continue;
                if (CheckIfPartyEmpty(enemyParty))
                    continue;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"It is {character.Name}'s turn...");
                ActionEnum characterAction = CharacterActionMenu(character);
                Console.ForegroundColor = ConsoleColor.Yellow;
                switch (characterAction)
                {
                    case ActionEnum.Attack:
                        Attacks characterAttack = CharacterAttackMenu(character);
                        CharacterObject targetedCharacter = TargetSystem(character, playerParty, enemyParty);
                        AttackSystem(characterAttack, character, targetedCharacter);
                        CheckIfDead(targetedCharacter, playerParty, enemyParty);
                        Console.WriteLine();
                        break;
                    case ActionEnum.SkipTurn:
                        Console.WriteLine($"{character.Name} Skipped their turn!!");
                        Console.WriteLine();
                        break;
                    default:
                        Console.WriteLine($"{character.Name} Skipped their turn!!");
                        Console.WriteLine();
                        break;
                }
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            foreach (CharacterObject character in enemyParty)
            {
                if (CheckIfPartyEmpty(playerParty))
                    continue;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"It is {character.Name}'s turn...");
                

                ActionEnum characterAction = CharacterActionMenu(character);
                Console.ForegroundColor = ConsoleColor.Yellow;
                switch (characterAction)
                {
                    case ActionEnum.Attack:
                        Attacks characterAttack = CharacterAttackMenu(character);
                        CharacterObject targetedCharacter = TargetSystem(character, playerParty, enemyParty);
                        AttackSystem(characterAttack, character, targetedCharacter);
                        CheckIfDead(targetedCharacter, playerParty, enemyParty);
                        Console.WriteLine();
                        break;
                    case ActionEnum.SkipTurn:
                        Console.WriteLine($"{character.Name} Skipped their turn!!");
                        Console.WriteLine();
                        break;
                    default:
                        Console.WriteLine($"{character.Name} Skipped their turn!!");
                        Console.WriteLine();
                        break;
                }
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.WriteLine();

        }

        public List<CharacterObject> Run()
        {
            while (!battleIsOver)
            {
                DisplayHP(PlayerParty, EnemyParty);
                TurnSystem(PlayerParty, EnemyParty, AllCharacters);
                CheckIfBattleOver(PlayerParty, EnemyParty);
            }
            Console.WriteLine("The battle is over!");
            return PlayerParty;
        }

        public void DisplayHP(List<CharacterObject> playerParty, List<CharacterObject> enemyParty)
        {
            Thread.Sleep(750);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== CURRENT BATTLE STATUS ===");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[The Player's Party]");
            foreach (CharacterObject character in playerParty)
            {
                Console.Write($"{character.Name} - Current HP: {character.CurrentHP}/{character.MaxHP}");
                Console.Write(" - Equipment:");
                foreach (ArmorObject armor in character.CharacterArmorSlots.Values)
                {
                    if (armor != null)
                        Console.Write($" {armor.Name} |");
                }
                foreach (WeaponObject weapon in character.CharacterWeaponSlots.Values)
                {
                    if (weapon != null)
                        Console.Write($" {weapon.Name}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[The Evil Ones]");
            foreach (CharacterObject character in enemyParty)
            {
                Console.Write($"{character.Name} - Current HP: {character.CurrentHP}/{character.MaxHP}");
                Console.Write(" - Equipment:");
                foreach (ArmorObject armor in character.CharacterArmorSlots.Values)
                {
                    if (armor != null)
                        Console.Write($" {armor.Name} |");
                }
                foreach (WeaponObject weapon in character.CharacterWeaponSlots.Values)
                {
                    if (weapon != null)
                        Console.Write($" {weapon.Name}");
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
        }
        public void OnCharacterDeath(CharacterObject character, List<CharacterObject> playerParty, List<CharacterObject> enemyParty)
        {
            if (character.Alignment == AlignmentEnum.Good)
                playerParty.Remove(character);
            else
                enemyParty.Remove(character);
        }
        public void CheckIfDead(CharacterObject character, List<CharacterObject> playerParty, List<CharacterObject> enemyParty)
        {
            if (character.CurrentHP <= 0)
            {
                Console.WriteLine($"{character.Name} has been killed!");
                character.Status = StatusEnum.Dead;
                OnCharacterDeath(character, playerParty, enemyParty);
            }
        }
        public bool CheckIfConstantDead(CharacterObject character)
        {
            if (character.Status == StatusEnum.Dead)
                return true;
            else
                return false;
        }
        public void CheckIfBattleOver(List<CharacterObject> playerParty, List<CharacterObject> enemyParty)
        {
            if (playerParty.Count == 0 || enemyParty.Count == 0)
                battleIsOver = true;
        }
        public bool CheckIfPartyEmpty(List<CharacterObject> party)
        {
            if (party.Count == 0)
                return true;
            else
                return false;
        }
        public List<CharacterObject> DetermineBattleWinner()
        {
            if (PlayerParty.Count == 0)
                return PlayerParty;
            else
                return EnemyParty;
        }
        
        
        public int CheckForArmor(CharacterObject character)
        {
            int blockedDamage = 0;
            foreach (ArmorObject armor in character.CharacterArmorSlots.Values)
            {
                if (armor != null)
                    blockedDamage += armor.Armor;
            }
            return blockedDamage;
        }
        public int CheckForWeapon(CharacterObject character)
        {
            int extraDamage = 0;
            foreach (WeaponObject weapon in character.CharacterWeaponSlots.Values)
            {
                if (weapon != null)
                    extraDamage += weapon.Damage;
            }
            return extraDamage;
        }
    }
}

namespace Characters
{
    //public record CharacterObject(string Name, int MaxHP, int CurrentHP, List<ActionEnum> Actions, List<Attacks> Attacks, AlignmentEnum Alignment);
    //public record PlayerCharacter(string Name, int MaxHP, int CurrentHP, List<ActionEnum> Actions, List<Attacks> Attacks, AlignmentEnum Alignment) : CharacterObject(Name, MaxHP, CurrentHP, Actions, Attacks, Alignment);
    //public record CompanionCharacter(string Name, int MaxHP, int CurrentHP, List<ActionEnum> Actions, List<Attacks> Attacks, AlignmentEnum Alignment) : CharacterObject(Name, MaxHP, CurrentHP, Actions, Attacks, Alignment);
    //public record SkeletonCharacter(string Name, int MaxHP, int CurrentHP, List<ActionEnum> Actions, List<Attacks> Attacks, AlignmentEnum Alignment) : CharacterObject(Name, MaxHP, CurrentHP, Actions, Attacks, Alignment);
    //public record BossCharacter(string Name, int MaxHP, int CurrentHP, List<ActionEnum> Actions, List<Attacks> Attacks, AlignmentEnum Alignment) : CharacterObject(Name, MaxHP, CurrentHP, Actions, Attacks, Alignment);

    public class CharacterObject
    {
        public string Name { get; init; }
        public int MaxHP { get; init; }
        public int CurrentHP { get; set; }
        public List<ActionEnum> Actions = new List<ActionEnum>();
        public List<Attacks> Attacks = new List<Attacks>();
        public AlignmentEnum Alignment { get; init; }
        public StatusEnum Status { get; set; } = StatusEnum.Alive;

        public List<Items.ItemObject> Inventory = new List<Items.ItemObject>();

        public Dictionary<string, Items.ArmorObject> CharacterArmorSlots { get; init; } = new Dictionary<string, Items.ArmorObject>
        {
            {"Head", null }, {"Chest", null}, {"Legs", null}
        };

        public Dictionary<string, Items.WeaponObject> CharacterWeaponSlots { get; init; } = new Dictionary<string, Items.WeaponObject>
        {
            {"Right", null }, {"Left", null}, {"Two-Handed", null}
        };
    }
    public class PlayerCharacter : CharacterObject
    {
        public PlayerCharacter(string name, int maxHP, int currentHP, AlignmentEnum alignment)
        {
            Name = name;
            MaxHP = maxHP;
            CurrentHP = currentHP;
            Alignment = alignment;
        }
    }
    public class CompanionCharacter : CharacterObject
    {
        public CompanionCharacter(string name, int maxHP, int currentHP, AlignmentEnum alignment)
        {
            Name = name;
            MaxHP = maxHP;
            CurrentHP = currentHP;
            Alignment = alignment;
        }
    }
    public class SkeletonCharacter : CharacterObject
    {
        public SkeletonCharacter(string name, int maxHP, int currentHP, AlignmentEnum alignment)
        {
            Name = name;
            MaxHP = maxHP;
            CurrentHP = currentHP;
            Alignment = alignment;
        }
    }
    public class BossCharacter : CharacterObject
    {
        public BossCharacter(string name, int maxHP, int currentHP, AlignmentEnum alignment)
        {
            Name = name;
            MaxHP = maxHP;
            CurrentHP = currentHP;
            Alignment = alignment;
        }
    }

    public enum ActionEnum { Attack, SkipTurn }
    public enum Attacks { Punch, BoneCrunch, Slash, Unravel }
    public enum AlignmentEnum { Good, Evil }
    public enum StatusEnum { Alive, Dead }

}

namespace Items
{
    public class ItemObject
    {
        public string Name { get; init; }
    }
    public class ConsumableObject : ItemObject
    {
        public int NumberOfUses { get; init; }
        public ItemTypeEnum ItemType { get; init; } = ItemTypeEnum.Consumable;

    }
    public class HealingPotion : ConsumableObject
    {
        int RestoreHPAmount { get; init; }
        public HealingPotion(string name, int restoreHPAmount, int numberOfUses)
        {
            Name = name;
            RestoreHPAmount = restoreHPAmount;
            NumberOfUses = numberOfUses;
        }
    }

    public class EquippableObject : ItemObject
    {
        public EquipmentTypeEnum EquipmentType { get; init; }
        public ItemTypeEnum ItemType { get; init; } = ItemTypeEnum.Equippable;
    }
    public class ArmorObject : EquippableObject
    {
        public int Armor { get; init; }
        public ArmorObject(string name, int armor, EquipmentTypeEnum equipmentType)
        {
            Name = name;
            Armor = armor; // Subtracts damage taken by however amount of armor defined
            EquipmentType = equipmentType;
        }
    }
    public class WeaponObject : EquippableObject
    {
        public int Damage { get; init; }
        public WeaponObject(string name, int damage)
        {
            Name = name;
            Damage = damage; // Deal extra damage
            EquipmentType = EquipmentTypeEnum.Weapon;
        }
    }

    public enum ItemTypeEnum { Consumable, Equippable }
    public enum EquipmentTypeEnum { Head, Chest, Legs, Arms, Weapon, Shield }
}

namespace EquipmentBuilder
{
    public class CreateWeapon
    {

        public WeaponObject IronSword()
        {
            return new WeaponObject("Iron Sword", 2);
        }
        public WeaponObject SilverSword()
        {
            return new WeaponObject("Silver Sword", 4);
        }
        public WeaponObject IronClaymore()
        {
            return new WeaponObject("Iron Claymore", 5);
        }
    }
    public class CreateArmor
    {
        public ArmorObject IronHelmet()
        {
            return new ArmorObject("Iron Helmet", 1, EquipmentTypeEnum.Head);
        }
        public ArmorObject IronCurass()
        {
            return new ArmorObject("Iron Curass", 2, EquipmentTypeEnum.Chest);
        }
        public ArmorObject IronLeggings()
        {
            return new ArmorObject("Iron Leggings", 2, EquipmentTypeEnum.Legs);
        }
        public ArmorObject SteelHelment()
        {
            return new ArmorObject("Steel Helmet", 2, EquipmentTypeEnum.Head);
        }
    }
}

namespace PartyBuilder
{
    public class CreatePlayerParty
    {
        List<CharacterObject> PlayerParty = new List<CharacterObject>();
        CreateArmor ArmorCreator = new CreateArmor();
        CreateWeapon WeaponCreator = new CreateWeapon();

        public List<CharacterObject> PlayerPartyInitialization()
        {

            PlayerCharacter Player = new PlayerCharacter("Jojack", 25, 25, AlignmentEnum.Good);
            CompanionCharacter Companion = new CompanionCharacter("Friendo", 10, 10, AlignmentEnum.Good);

            Player.Actions.Add(ActionEnum.Attack);
            Companion.Actions.Add(ActionEnum.Attack);

            Player.Actions.Add(ActionEnum.SkipTurn);
            Companion.Actions.Add(ActionEnum.SkipTurn);

            Player.Attacks.Add(Attacks.Punch);
            Player.Attacks.Add(Attacks.Slash);
            Companion.Attacks.Add(Attacks.Slash);

            Player.CharacterArmorSlots["Head"] = ArmorCreator.IronHelmet();
            Player.CharacterArmorSlots["Chest"] = ArmorCreator.IronCurass();
            Player.CharacterArmorSlots["Legs"] = ArmorCreator.IronLeggings();
            Player.CharacterWeaponSlots["Right"] = WeaponCreator.IronSword();
            Companion.CharacterArmorSlots["Head"] = ArmorCreator.SteelHelment();
            Companion.CharacterWeaponSlots["Two-Handed"] = WeaponCreator.IronClaymore();

            PlayerParty.Add(Player);
            PlayerParty.Add(Companion);
            return PlayerParty;
        }
    }

    public class CreateEnemyParty
    {
        List<CharacterObject> bossParty = new List<CharacterObject>();
        List<CharacterObject> skeletonParty = new List<CharacterObject>();

        //public event Action<CharacterObject, List<CharacterObject>, List<CharacterObject>> CharacterDeath = delegate { };

        public List<CharacterObject> EnemyBossPartyInitialization()
        {
            SkeletonCharacter Skeleton1 = new SkeletonCharacter("Bones", 1, 1, AlignmentEnum.Evil);
            SkeletonCharacter Skeleton2 = new SkeletonCharacter("Skully", 6, 6, AlignmentEnum.Evil);
            BossCharacter Boss = new BossCharacter("TheBoss", 14, 14, AlignmentEnum.Evil);

            Skeleton1.Actions.Add(ActionEnum.Attack);
            Skeleton2.Actions.Add(ActionEnum.Attack);
            Boss.Actions.Add(ActionEnum.Attack);

            Skeleton1.Actions.Add(ActionEnum.SkipTurn);
            Skeleton2.Actions.Add(ActionEnum.SkipTurn);
            Boss.Actions.Add(ActionEnum.SkipTurn);

            Skeleton1.Attacks.Add(Attacks.BoneCrunch);
            Skeleton2.Attacks.Add(Attacks.BoneCrunch);
            Boss.Attacks.Add(Attacks.Slash);
            Boss.Attacks.Add(Attacks.Unravel);

            bossParty.Add(Skeleton1);
            bossParty.Add(Skeleton2);
            bossParty.Add(Boss);

            return bossParty;
        }       
        public List<CharacterObject> EnemySkeletonPartyInitialization()
        {
            SkeletonCharacter Skeleton1 = new SkeletonCharacter("Bones", 5, 5, AlignmentEnum.Evil);
            SkeletonCharacter Skeleton2 = new SkeletonCharacter("Boney", 2, 2, AlignmentEnum.Evil);
            SkeletonCharacter Skeleton3 = new SkeletonCharacter("Skully", 6, 6, AlignmentEnum.Evil);
            SkeletonCharacter Skeleton4 = new SkeletonCharacter("BareBone", 6, 6, AlignmentEnum.Evil);


            Skeleton1.Actions.Add(ActionEnum.Attack);
            Skeleton2.Actions.Add(ActionEnum.Attack);
            Skeleton3.Actions.Add(ActionEnum.Attack);
            Skeleton4.Actions.Add(ActionEnum.Attack);

            Skeleton1.Actions.Add(ActionEnum.SkipTurn);
            Skeleton2.Actions.Add(ActionEnum.SkipTurn);
            Skeleton3.Actions.Add(ActionEnum.SkipTurn);
            Skeleton4.Actions.Add(ActionEnum.SkipTurn);

            Skeleton1.Attacks.Add(Attacks.BoneCrunch);
            Skeleton2.Attacks.Add(Attacks.BoneCrunch);
            Skeleton3.Attacks.Add(Attacks.BoneCrunch);
            Skeleton4.Attacks.Add(Attacks.BoneCrunch);

            skeletonParty.Add(Skeleton1);
            skeletonParty.Add(Skeleton2);
            skeletonParty.Add(Skeleton3);
            skeletonParty.Add(Skeleton4);

            return skeletonParty;
        }
    }
}


namespace Dungeon
{

}