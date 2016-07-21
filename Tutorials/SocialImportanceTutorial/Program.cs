﻿using System;
using System.IO;
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
            Console.WriteLine(Directory.GetCurrentDirectory());
            //First, we load the asset from an existing profile
            var siAsset = SocialImportanceAsset.LoadFromFile(LocalStorageProvider.Instance, "../../../Examples/SITest.si");

            //Second, we need to associate an existing EmotionalAppraisalAsset to the new instance
            var ea = EmotionalAppraisalAsset.LoadFromFile(LocalStorageProvider.Instance, "../../../Examples/EATest.ea");
            siAsset.BindEmotionalAppraisalAsset(ea);
            
            Console.WriteLine("The SI attributed to the player is:" + siAsset.GetSocialImportance("Player"));

            Console.ReadKey();
        }
    }
}
