using System;
using System.Collections.Generic;

namespace HW44
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SquadCreator creator = new SquadCreator();

            Squad squadA = creator.Create(10, "Alpha");
            Squad squadB = creator.Create(10, "Beta");

            Battle battle = new Battle(squadA, squadB);

            battle.Work();
        }
    }

    public static class Utils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            return s_random.Next(minValue, maxValue + 1);
        }

        public static int GenerateRandomNumber(int maxValue)
        {
            return s_random.Next(maxValue);
        }
    }

    public class Battle
    {
        private Squad _squadA;
        private Squad _squadB;

        public Battle(Squad squadA, Squad squadB)
        {
            _squadA = squadA;
            _squadB = squadB;
        }

        public void Work()
        {
            while (_squadA.HasAliveSoldiers() && _squadB.HasAliveSoldiers())
            {
                Console.WriteLine("\n--- Новый раунд ---");

                _squadA.Attack(_squadB);
                _squadB.Attack(_squadA);

                _squadA.SelectAlive();
                _squadB.SelectAlive();
            }

            Console.WriteLine("\n--- Битва завершена ---");
            Console.WriteLine(_squadA.HasAliveSoldiers() ? $"Победил Взвод {_squadA.Name}!" : $"Победил Взвод {_squadB.Name}!");
        }
    }

    public abstract class Soldier
    {
        public Soldier(int healthPoints, int damage, int armorPoints)
        {
            HealthPoints = healthPoints;
            Damage = damage;
            ArmorPoints = armorPoints;
        }

        public int HealthPoints { get; protected set; }
        public int Damage { get; protected set; }
        public int ArmorPoints { get; protected set; }
        public bool IsAlive => HealthPoints > 0;

        public abstract void Attack(List<Soldier> enemies);

        protected void DealDamage(Soldier target, int damage)
        {
            int effectiveDamage = damage - target.ArmorPoints;

            if (effectiveDamage > 0)
            {
                target.HealthPoints -= effectiveDamage;
                Console.WriteLine($"{GetType().Name} атакует {target.GetType().Name} и наносит {effectiveDamage} урона.");
            }
            else
            {
                Console.WriteLine($"{GetType().Name} атакует {target.GetType().Name}, но не пробивает броню\n");
            }
        }

        public abstract Soldier Clone();
    }

    public class RegularSoldier : Soldier
    {
        public RegularSoldier(int health, int damage, int armor) : base(health, damage, armor) { }

        public override void Attack(List<Soldier> enemies)
        {
            if (enemies.Count == 0)
                return;

            Soldier target = enemies[Utils.GenerateRandomNumber(enemies.Count)];

            DealDamage(target, Damage);
        }

        public override Soldier Clone()
        {
            return new RegularSoldier(HealthPoints, Damage, ArmorPoints);
        }
    }

    public class SniperSoldier : Soldier
    {
        private int _criticalHitMultiply = 3;

        public SniperSoldier(int health, int damage, int armor) : base(health, damage, armor) { }

        public override void Attack(List<Soldier> enemies)
        {
            if (enemies.Count == 0)
                return;

            Soldier target = enemies[Utils.GenerateRandomNumber(enemies.Count)];

            int damage = Damage * _criticalHitMultiply;

            DealDamage(target, damage);
        }

        public override Soldier Clone()
        {
            return new SniperSoldier(HealthPoints, Damage, ArmorPoints);
        }
    }

    class МortarmanSoldier : Soldier
    {
        public МortarmanSoldier(int health, int damage, int armor) : base(health, damage, armor) { }

        public override void Attack(List<Soldier> enemies)
        {
            int maxHitCounts = 4;
            int index;
            List<int> attackedIndexes = new List<int>();

            int hitCounts = Utils.GenerateRandomNumber(1, maxHitCounts);
            hitCounts = Math.Min(hitCounts, enemies.Count);

            for (int i = 0; i < hitCounts; i++)
            {
                do
                {
                    index = Utils.GenerateRandomNumber(enemies.Count);
                } while (attackedIndexes.Contains(index));

                attackedIndexes.Add(index);
                DealDamage(enemies[index], Damage);
            }
        }

        public override Soldier Clone()
        {
            return new МortarmanSoldier(HealthPoints, Damage, ArmorPoints);
        }
    }

    class MachineGunnerSoldier : Soldier
    {
        public MachineGunnerSoldier(int health, int damage, int armor) : base(health, damage, armor) { }

        public override void Attack(List<Soldier> enemies)
        {
            int maxHitsCounts = 5;
            int minHitsCounts = 2;

            int hits = Utils.GenerateRandomNumber(minHitsCounts, maxHitsCounts);

            for (int i = 0; i < hits; i++)
            {
                Soldier target = enemies[Utils.GenerateRandomNumber(enemies.Count)];

                DealDamage(target, Damage);
            }
        }

        public override Soldier Clone()
        {
            return new MachineGunnerSoldier(HealthPoints, Damage, ArmorPoints);
        }
    }

    public class SquadCreator
    {
        public Squad Create(int count, string name)
        {
            List<Soldier> baseSoldiers = new List<Soldier>();
            List<Soldier> soldiers = new List<Soldier>();

            baseSoldiers.Add(new RegularSoldier(100, 20, 5));
            baseSoldiers.Add(new SniperSoldier(90, 25, 4));
            baseSoldiers.Add(new МortarmanSoldier(80, 15, 3));
            baseSoldiers.Add(new MachineGunnerSoldier(85, 15, 2));

            for (int i = 0; i < count; i++)
            {
                int index = Utils.GenerateRandomNumber(baseSoldiers.Count);

                soldiers.Add(baseSoldiers[index].Clone());
            }

            Squad squad = new Squad(name, soldiers);
            return squad;
        }
    }

    public class Squad
    {
        private List<Soldier> _soldiers;

        public Squad(string name)
        {
            Name = name;
        }

        public Squad(string name, List<Soldier> soldiers)
        {
            _soldiers = soldiers;
            Name = name;
        }

        public string Name { get; private set; }

        public void Attack(Squad target)
        {
            Console.WriteLine($"Отряд {Name} атакует!!");

            foreach (Soldier soldier in _soldiers)
            {
                if (soldier.IsAlive)
                    soldier.Attack(target._soldiers);
            }
        }

        public void SelectAlive()
        {
            List<Soldier> alive = new List<Soldier>();

            for (int i = 0; i < _soldiers.Count; i++)
            {
                if (_soldiers[i].IsAlive)
                    alive.Add(_soldiers[i]);
            }

            _soldiers = alive;
        }

        public bool HasAliveSoldiers()
        {
            for (int i = 0; i < _soldiers.Count; i++)
            {
                if (_soldiers[i].IsAlive)
                    return true;
            }
            return false;
        }
    }
}