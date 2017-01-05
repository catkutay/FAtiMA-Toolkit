﻿using System;
using AssetManagerPackage;
using GAIPS.Rage;
using RolePlayCharacter;

namespace RolePlayCharacterTutorial
{
    class Program
    {
        static void Main(string[] args)
        {
			AssetManager.Instance.Bridge = new BasicIOBridge();
            //Loading the asset
	        var profile = RolePlayCharacterAsset.LoadFromFile("../../../Examples/RPCTest.rpc");
            var rpc = profile.BuildRPCFromProfile();
            var eventStr = "Event(Action-Finished, Player, Kick, Client)";

            Console.WriteLine("The name of the character loaded is: " + rpc.CharacterName);
            Console.WriteLine("Mood: " + rpc.Mood);
            Console.WriteLine("Strongest emotion: " + rpc.GetStrongestActiveEmotion()?.EmotionType + "-"+ rpc.GetStrongestActiveEmotion()?.Intensity);
            Console.WriteLine("Selected Action: " + rpc.PerceptionActionLoop(new []{eventStr})?.ActionName);
            Console.ReadKey();
        }
    }
}
