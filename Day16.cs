using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Payloads;
using System.Collections.Immutable;

namespace AoC2022;

class Day16 : Puzzle
{

    public static Dictionary<string, int> FlowRates = new()
    {
        {"", 0},
    };
    public static HashSet<(string, string, int PathCost)> Tunnels = new();
    public static HashSet<(string, string)> TunnelsOne = new();
    public static List<string> FlowersByValue = new();

    public override void Parse(string filename)
    {
        base.Parse(filename);
        foreach (var line in lines)
        {
            var parts = line.Split(' ', 10);
            var room = parts[1];

            var rate = int.Parse(parts[4].Substring(5, parts[4].Length - 6));
            FlowRates.Add(room, rate);
            foreach (var link in parts[9].Split(", "))
            {
                Tunnels.Add((room, link, 1));
                TunnelsOne.Add((room, link));
            }
        }

        foreach (var room in FlowRates.Keys)
        {
            if (FlowRates[room] == 0)
            {
                var destinations = Tunnels.Where(t => t.Item1 == room).ToArray();
                if (destinations.Length == 2)
                {
                    var curTunnel0 = Tunnels.Where(t => t.Item1 == destinations[0].Item2 && t.Item2 == room).First();
                    Tunnels.Remove(curTunnel0);
                    var newTunnel0 = (curTunnel0.Item1, destinations[1].Item2, curTunnel0.PathCost + destinations[1].PathCost);
                    Tunnels.Add(newTunnel0);

                    var curTunnel1 = Tunnels.Where(t => t.Item1 == destinations[1].Item2 && t.Item2 == room).First();
                    Tunnels.Remove(curTunnel1);
                    var newTunnel1 = (curTunnel1.Item1, destinations[0].Item2, curTunnel1.PathCost + destinations[0].PathCost);
                    Tunnels.Add(newTunnel1);

                    Tunnels.Remove(destinations[0]);
                    Tunnels.Remove(destinations[1]);
                }
            }
        }

        FlowersByValue = FlowRates.Keys.Where(f => FlowRates[f] != 0).ToList();
        FlowersByValue.Sort((a, b) => FlowRates[b] - FlowRates[a]);
    }

    // public override void Part1()
    // {
    //     int maxScore = 0;
    //     List<(int minutesLeft, int score, string room, List<string> openRooms)> states = new()
    //     {
    //         (30, 0, "AA", new()),
    //     };

    //     void addState(int minutesLeft, int score, string room, List<string> openRooms)
    //     {
    //         minutesLeft = Math.Max(minutesLeft, 1);
    //         if (states.Any(p => (p.minutesLeft == minutesLeft && p.score >= score && p.room == room && openRooms.Union(p.openRooms).Count() == openRooms.Count )))
    //         {
    //             return;
    //         }
    //         states.Insert(0, (minutesLeft, score, room, openRooms));
    //         states.RemoveAll(p => (p.minutesLeft == minutesLeft && p.score < score && p.room == room && openRooms.Union(p.openRooms).Count() == openRooms.Count ));
    //     }
    //     int loopCount = 0;
    //     int lastMinutesLeft = 0;
    //     while (states.Count > 0)
    //     {
    //         if (loopCount++ % 1000 == 0) TimeCheck($"loop {loopCount} {states.Count} {states[0].minutesLeft}");
    //         var (minutesLeft, score, room, openRooms) = states[states.Count - 1];
    //         states.RemoveAt(states.Count - 1);
    //         if (minutesLeft <= 1)
    //         {
    //             if (score > maxScore)
    //             {
    //                 Console.WriteLine($"New max {score}");
    //                 maxScore = score;
    //             }
    //             continue;
    //         }
    //         foreach(var dest in Tunnels.Where(t => t.Item1 == room))
    //         {
    //             // move without opening
    //             addState(minutesLeft - dest.PathCost, score, dest.Item2, openRooms);
    //             if (!openRooms.Contains(room) && FlowRates[room] != 0)
    //             {
    //                 addState(minutesLeft - 1 - dest.PathCost, score + (minutesLeft - 1) * FlowRates[room], dest.Item2, openRooms.Append(room).ToList());
    //             }
    //         }
    //         if (minutesLeft != lastMinutesLeft)
    //         {
    //             Console.WriteLine($"--next minute {minutesLeft} {states.Count}");
    //             lastMinutesLeft = minutesLeft;
    //             states.Sort((a, b) =>
    //             {
    //                 var score = a.score - b.score;
    //                 var minutes = a.minutesLeft - b.minutesLeft;
    //                 return minutes == 0 ? score : minutes;
    //             });

    //         }
    //     }
    //     Console.WriteLine(maxScore);
    // }

    public override void Part2()
    {
        int maxScore = 0;
        List<(int minutesLeft, int score, string room, List<string> openRooms)> states = new()
        {
            (26, 0, "AA", new()),
        };

        void addState(int minutesLeft, int score, string room, List<string> openRooms)
        {
            minutesLeft = Math.Max(minutesLeft, 1);
            if (states.Any(p => (p.minutesLeft == minutesLeft && p.score >= score && p.room == room && openRooms.Union(p.openRooms).Count() == openRooms.Count )))
            {
                return;
            }
            states.Insert(0, (minutesLeft, score, room, openRooms));
            states.RemoveAll(p => (p.minutesLeft == minutesLeft && p.score < score && p.room == room && openRooms.Union(p.openRooms).Count() == openRooms.Count ));
        }
        int loopCount = 0;
        int lastMinutesLeft = 0;
        while (states.Count > 0)
        {
            if (loopCount++ % 1000 == 0) TimeCheck($"loop {loopCount} {states.Count} {states[0].minutesLeft}");
            var (minutesLeft, score, room, openRooms) = states[states.Count - 1];
            if (minutesLeft != lastMinutesLeft)
            {
                Console.WriteLine($"--next minute {minutesLeft} {states.Count}");
                lastMinutesLeft = minutesLeft;
                states.Sort((a, b) =>
                {
                    var score = b.score - a.score;
                    var minutes = a.minutesLeft - b.minutesLeft;
                    return minutes == 0 ? score : minutes;
                });
                if (minutesLeft == 1) break;
            }
            (minutesLeft, score, room, openRooms) = states[states.Count - 1];
            states.RemoveAt(states.Count - 1);
            if (minutesLeft <= 1)
            {
                if (score > maxScore)
                {
                    Console.WriteLine($"New max {score}");
                    maxScore = score;
                }
                continue;
            }
            foreach(var dest in Tunnels.Where(t => t.Item1 == room))
            {
                // move without opening
                addState(minutesLeft - dest.PathCost, score, dest.Item2, openRooms);
                if (!openRooms.Contains(room) && FlowRates[room] != 0)
                {
                    addState(minutesLeft - 1 - dest.PathCost, score + (minutesLeft - 1) * FlowRates[room], dest.Item2, openRooms.Append(room).ToList());
                }
            }
        }

        states.Sort((a,b) => b.score - a.score);

        var bestScore = states[0].score;
        var rooms = states[0].openRooms;
        var otherScore = states.Where(s => s.openRooms.Intersect(rooms).Count() == 0).FirstOrDefault().score;

        Console.WriteLine(bestScore + otherScore);
    }

    // this solution down here got too complicated - i had tried doing one step at a time, but then realized
    // that optimization was saving the most of everything i tried before so i redid the part2 solution above
    // realizing that I could just find two paths for two players that don't overlap (since they will wander)

    // class GameState
    // {
    //     public int minutesLeft;
    //     public int score;
    //     public string room1;
    //     public string room2;
    //     public List<string> openRooms;
    //     public List<string> myRooms;
    //     public List<string> theirRooms;

    //     public GameState(int minutesLeft, int score, string room1, string room2, List<string> openRooms, List<string> myRooms, List<string> theirRooms)
    //     {
    //         this.minutesLeft = minutesLeft;
    //         this.score = score;
    //         this.room1 = room1;
    //         this.room2 = room2;
    //         this.openRooms = openRooms;
    //         this.myRooms = myRooms;
    //         this.theirRooms = theirRooms;
    //     }

    //     public int flowRate => openRooms.Select(f => FlowRates[f]).Sum();

    //     public override string ToString() => $"{minutesLeft}: {score}({bestScore}) {room1} {room2} {openRooms.Aggregate("", (a,b)=>a+b)}";

    //     protected int _bestScore = 0;
    //     public int bestScore 
    //     {
    //         get
    //         {
    //             if (_bestScore != 0) return _bestScore;
    //             int score = this.score;
    //             List<string> openBest = new List<string>(this.openRooms);
    //             List<string> ourFlowers = new List<string>(FlowersByValue.Where(f => !openBest.Contains(f)));
    //             if (!openBest.Contains(room1) && ourFlowers.Contains(room1))
    //             {
    //                 openBest.Add(room1);
    //                 ourFlowers.Remove(room1);
    //             }
    //             if (!openBest.Contains(room2) && ourFlowers.Contains(room2))
    //             {
    //                 openBest.Add(room2);
    //                 ourFlowers.Remove(room2);
    //             }
    //             for (int x = 0; x<this.minutesLeft; x++)
    //             {
    //                 score += openBest.Select(f => FlowRates[f]).Sum();
    //                 if (ourFlowers.Count > 0)
    //                 {
    //                     openBest.Add(ourFlowers[0]);
    //                     ourFlowers.RemoveAt(0);
    //                 }
    //                 if (ourFlowers.Count > 0)
    //                 {
    //                     openBest.Add(ourFlowers[0]);
    //                     ourFlowers.RemoveAt(0);
    //                 }
    //             }

    //             return _bestScore = score;
    //         }
    //     }        
    // }

    // public override void Part2()
    // {
    //     int maxScore = 0;

    //     // state for elephant walk
    //     List<GameState> states = new()
    //     {
    //         new GameState(26, 0, "AA", "AA", new(), new(){}, new(){}),
    //     };



    //     void addState(int minutesLeft, int score, string room1, string room2, List<string> openRooms, List<string> myRoomList, List<string> theirRoomList)
    //     {
    //         if (minutesLeft<1)
    //         {
    //             if (score > maxScore)
    //             {
    //                 Console.WriteLine($"New max {score}");
    //                 maxScore = score;
    //             }
    //             return;
    //         }
    //         var ourState = new GameState(minutesLeft, score, room1, room2, openRooms, myRoomList, theirRoomList);
    //         // matches minutesleft, state score beats or meets ours, room state matches either direction,
    //         // and the rooms open in our state are also in theirs
    //         if (states.Any(p => (
    //             p.minutesLeft == minutesLeft &&
    //             p.bestScore >= ourState.bestScore &&
    //             ((p.room1 == room1 && p.room2 == room2) || (p.room2 == room1 && p.room1 == room2)) && 
    //             p.flowRate >= ourState.flowRate
    //         )))
    //         {
    //             // Console.WriteLine($"Skipping {ourState}");1
    //             return;
    //         }
    //         states.Add(ourState);
    //     }
    //     int loopCount = 0;
    //     int lastMinutesLeft = 0;
    //     int lastStatesCount = 1;

    //     while (states.Count > 0)
    //     {
    //         if (states[0].minutesLeft != lastMinutesLeft)
    //         {
    //             lastMinutesLeft = states[0].minutesLeft;
    //             states.Sort((a, b) => 
    //             {
    //                 var bs = b.bestScore - a.bestScore;
    //                 var s = b.score - a.score;
    //                 if (bs != 0) return bs;
    //                 return s;
    //             });
    //             // var bs = states[0].bestScore;
    //             // states.RemoveAll(a => a.bestScore < bs);
    //             TimeCheck($"--next minute {states[0].minutesLeft} {states.Count} {states[0]}");
    //         }

    //         if (loopCount++ % 2500 == 0)
    //         {
    //             TimeCheck($"loop {loopCount} {states.Count} {states.Count - lastStatesCount} {states[0]}");
    //             lastStatesCount = states.Count;
    //         }
    //         var minutesLeft = states[0].minutesLeft;
    //         var score = states[0].score;
    //         var room1 = states[0].room1;
    //         var room2 = states[0].room2;
    //         var openRooms = states[0].openRooms;
    //         var myRoomList = states[0].myRooms;
    //         var theirRoomList = states[0].theirRooms;
    //         states.RemoveAt(0);

    //         minutesLeft--;
    //         score += openRooms.Select(s => FlowRates[s]).Sum();

    //         bool canOpen1 = !openRooms.Contains(room1) && FlowRates[room1] != 0;
    //         bool canOpen2 = !openRooms.Contains(room2) && FlowRates[room2] != 0;
    //         var dests1 = TunnelsOne.Where(t => t.Item1 == room1).ToList();
    //         var dests2 = TunnelsOne.Where(t => t.Item1 == room2).ToList();
            
    //         List<string> myOptions = new();
    //         foreach(var dest in dests1)
    //         {
    //             myOptions.Add(dest.Item2);
    //         }
    //         bool anyOption = false;
    //         foreach(var dest in dests2)
    //         {
    //             foreach (var opt in myOptions)
    //             {
    //                 // Both me and elephant move, no one opens.
    //                 addState(minutesLeft, score, opt, dest.Item2, openRooms, myRoomList.Append(room1).ToList(), theirRoomList.Append(room2).ToList());
    //                 anyOption = true;
    //             }
    //             if (canOpen1)
    //             {
    //                 // I open my valve, elephant moves.
    //                 addState(minutesLeft, score, room1, dest.Item2, openRooms.Append(room1).ToList(), new(), theirRoomList.Append(room2).ToList());
    //                 anyOption = true;
    //             }
    //         }
    //         if (canOpen2)
    //         {
    //             foreach (var opt in myOptions)
    //             {
    //                 // I move, elephant opens.
    //                 addState(minutesLeft, score, opt, room2, openRooms.Append(room2).ToList(), myRoomList.Append(room1).ToList(), new());
    //                 anyOption = true;
    //             }
    //             if (canOpen1 && room1 != room2)
    //             {
    //                 // neither move, both open
    //                 addState(minutesLeft, score, room1, room2, openRooms.Append(room1).Append(room2).ToList(), new(), new());
    //                 anyOption = true;
    //             }
    //             // else
    //             // if (dests1.Count == 0)
    //             // {
    //             //     // p1 stuck, p2 opens
    //             //     addState(minutesLeft, score, "", room2, openRooms.Append(room2).ToList(), new(), new());
    //             //     anyOption = true;
    //             // }
    //         }
    //         // else
    //         // if (dests2.Count == 0)
    //         // {
    //         //     // we couldn't move, or open on player 2
    //         //     foreach (var opt in myOptions)
    //         //     {
    //         //         // p1 moves, p2 is stuck.
    //         //         addState(minutesLeft, score, opt, "", openRooms, myRoomList.Append(room1).ToList(), new());
    //         //         anyOption = true;
    //         //     }
    //         //     if (canOpen1)
    //         //     {
    //         //         // p1 opens, p2 stuck.
    //         //         addState(minutesLeft, score, room1, "", openRooms.Append(room1).ToList(), new(), new());
    //         //         anyOption = true;
    //         //     }
    //         // }
    //         if (!anyOption)
    //         {
    //             Console.WriteLine($"Stalled");
    //             addState(minutesLeft, score, "", "", openRooms, new(), new());
    //         }

    //     }

    //     //2578 too low
    //     Console.WriteLine(maxScore);
    // }
}