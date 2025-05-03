using System;
using System.Collections.Generic;

namespace HW44
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Squad squadA = new Squad("Alpha");
            Squad squadB = new Squad("Beta");

            squadA.Fill(10);
            squadB.Fill(10);

            Battle battle = new Battle(squadA, squadB);

            battle.Start();
        }
    }

    public static class Utils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            return s_random.Next(minValue, maxValue);
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

        public void Start()
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
    }

    public class RegularSoldier : Soldier
    {
        public RegularSoldier(int health, int damage, int armor) : base(health, damage, armor) { }

        public override void Attack(List<Soldier> enemies)
        {
            if (enemies.Count == 0)
                return;

            Soldier target = enemies[Utils.GenerateRandomNumber(0, enemies.Count)];

            DealDamage(target, Damage);
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

            Soldier target = enemies[Utils.GenerateRandomNumber(0, enemies.Count)];

            int damage = Damage * _criticalHitMultiply;

            DealDamage(target, damage);
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
                    index = Utils.GenerateRandomNumber(0, enemies.Count);
                } while (attackedIndexes.Contains(index));

                attackedIndexes.Add(index);
                DealDamage(enemies[index], Damage);
            }
        }
    }

    class MachineGunnerSoldier : Soldier
    {
        public MachineGunnerSoldier(int health, int damage, int armor) : base(health, damage, armor) { }

        public override void Attack(List<Soldier> enemies)
        {
            int maxHitsCounts = 5;
            int minHitsCounts = 2;

            int hits = Utils.GenerateRandomNumber(minHitsCounts, maxHitsCounts + 1);

            for (int i = 0; i < hits; i++)
            {
                Soldier target = enemies[Utils.GenerateRandomNumber(0, enemies.Count)];

                DealDamage(target, Damage);
            }
        }
    }

    public class Squad
    {
        private List<Soldier> _soldiers = new List<Soldier>();

        public Squad(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public void Fill(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int type = Utils.GenerateRandomNumber(1, 4);

                switch (type)
                {
                    case 1:
                        _soldiers.Add(new RegularSoldier(100, 20, 5));
                        break;

                    case 2:
                        _soldiers.Add(new SniperSoldier(90, 25, 4));
                        break;

                    case 3:
                        _soldiers.Add(new МortarmanSoldier(80, 15, 3));
                        break;

                    case 4:
                        _soldiers.Add(new MachineGunnerSoldier(85, 15, 2));
                        break;

                }
            }
        }

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