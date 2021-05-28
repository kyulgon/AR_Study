using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Threading // 레이스 컨디션 해결(단 속도가 많이 느려짐)
{
    class Program
    {
        public static int gold;
        public static int item;
        public static object lockGold = new object();
        public static object lockItem = new object();
        static void Main(string[] args)
        {
            Program.gold = 0;
            Program.item = 0;

            Thread thread0 = new Thread(() => Hunt());
            thread0.Start();
            Thread thread1 = new Thread(() => BuyItem());
            thread1.Start();

            Console.WriteLine(string.Format($"골드 양 {Program.gold}"));
        }

        public static void Hunt()
        {
            Console.WriteLine(string.Format("사냥 시작!"));

            for (int i = 0; i < 100; i++)
            {
                lock (Program.lockGold)
                {
                    Program.gold += 10;
                    Console.WriteLine(string.Format($"현재 골드 {gold}"));

                    lock(Program.lockItem)
                    {
                        Program.item += 1;
                        Console.WriteLine(string.Format($"현재 퀘스트템 {item}"));
                    }
                    Thread.Sleep(10);
                }
            }
           
            Console.WriteLine(string.Format("사냥 종료!"));
        }

        public static void BuyItem()
        {
            Console.WriteLine(string.Format("아이템 구매 시작!"));

            for (int i = 0; i < 100; i++)
            {
                lock (Program.lockItem)
                {
                    Program.item += 1;
                    Console.WriteLine(string.Format($"현재 퀘스트템 {item}"));

                    lock (Program.lockGold)
                    {
                        Program.gold -= 1;
                        Console.WriteLine(string.Format($"소비한 골드 {gold}"));
                        
                    }
                    Thread.Sleep(10);
                }
                
            }           
            Console.WriteLine(string.Format("아이템 구매 종료!"));
        }
        
    }
}
