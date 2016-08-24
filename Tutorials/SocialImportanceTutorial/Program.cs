﻿using System;
using System.IO;
using AssetManagerPackage;
using EmotionalAppraisal;
using GAIPS.Rage;
using SocialImportance;

namespace SocialImportanceTutorial
{
    class Program
    {
        //This is a small console program to exemplify the main functionality of the Social Importance Asset
        static void Main(string[] args)
        {
			AssetManager.Instance.Bridge = new BasicIOBridge();
            var siTarget = "Player";

            Console.WriteLine(Directory.GetCurrentDirectory());
            //First, we load the asset from an existing profile
            var siAsset = SocialImportanceAsset.LoadFromFile("../../../Examples/SITest.si");

            //Second, we need to associate an existing EmotionalAppraisalAsset to the new instance
            var ea = EmotionalAppraisalAsset.LoadFromFile("../../../Examples/EATest.ea");
            siAsset.BindEmotionalAppraisalAsset(ea);
            
            Console.WriteLine("The SI attributed to "+siTarget+" is:" + siAsset.GetSocialImportance(siTarget));

            Console.WriteLine("Conferral to execute: " + siAsset.DecideConferral("SELF")?.ActionName);

            Console.ReadKey();
        }
    }
}
