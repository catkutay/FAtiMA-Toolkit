﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActionLibrary;
using ActionLibrary.DTOs;
using AssetManagerPackage;
using CommeillFaut.DTOs;
using CommeillFaut;
using Conditions.DTOs;
using EmotionalAppraisal;
using EmotionalAppraisal.DTOs;
using IntegratedAuthoringTool;
using KnowledgeBase;
using Microsoft.CSharp.RuntimeBinder;
using RolePlayCharacter;
using WellFormedNames;

namespace CommeillFautTutorial
{
    class Program
    {
        static List<RolePlayCharacterAsset> rpcList;

        static void Main(string[] args)
        {

            var iat = IntegratedAuthoringToolAsset.LoadFromFile("../../../Examples/CiF/CIFVERSION3.iat");
            rpcList = new List<RolePlayCharacterAsset>();


            foreach (var source in iat.GetAllCharacterSources())
            {

                var rpc = RolePlayCharacterAsset.LoadFromFile(source.Source);


                //rpc.DynamicPropertiesRegistry.RegistDynamicProperty(Name.BuildName("Volition"),cif.VolitionPropertyCalculator);
                rpc.LoadAssociatedAssets();

                iat.BindToRegistry(rpc.DynamicPropertiesRegistry);

                rpcList.Add(rpc);

            }
            /*         var cif = CommeillFautAsset.LoadFromFile(rpcList.First().CommeillFautAssetSource);

                     var newDTO = new SocialExchangeDTO
                     {
                         Action = "Flirt",
                         Intent = "toFlirt"
                     };
                     cif.AddExchange(newDTO);


                     var condSET = new ConditionSetDTO() { ConditionSet = new string[]{ "IsFriend(SELF, [x]) = True" } };


                     var inf = new InfluenceRuleDTO()
                     {
                         RuleName = "Friend?",
                         Target = "[x]",
                         RuleConditions = condSET
                 };

                     cif.m_SocialExchanges.FirstOrDefault().SetInfluenceRule(inf);
                     cif.Save();
                     */


            foreach (var actor in rpcList)
            {


                foreach (var anotherActor in rpcList)
                {
                    if (actor != anotherActor)
                    {


                        var changed = new[] { EventHelper.ActionEnd(anotherActor.CharacterName.ToString(), "Enters", "Room") };
                        actor.Perceive(changed);
                    }
                    
                }
                //         actor.SaveToFile("../../../Examples/" + actor.CharacterName + "-output1" + ".rpc");
            }



            List<Name> _events = new List<Name>();
            List<IAction> _actions = new List<IAction>();

            IAction action = null;
            while (true)
            {
                _actions.Clear();
                foreach (var rpc in rpcList)
                {

                    Console.WriteLine("Character perceiving: " + rpc.CharacterName);

                    rpc.Perceive(_events);
                    var decisions = rpc.Decide();
                    _actions.Add(decisions.FirstOrDefault());

                    foreach (var act in decisions)
                    {
                        Console.WriteLine(rpc.CharacterName + " has this action: " + act.Name);
                        
                    }





                    rpc.SaveToFile("../../../Examples/CiF/" + rpc.CharacterName + "-output" + ".rpc");


                }

                _events.Clear();
             
                    var randomGen = new Random(Guid.NewGuid().GetHashCode());

                    var pos = randomGen.Next(rpcList.Count);
                int i = 0;
                  
                    var initiator = rpcList.ElementAt(pos);
                
               
                    action = _actions.ElementAt(pos);
              
                Console.WriteLine();
                if(action == null)
                Console.WriteLine(initiator.CharacterName + " does not have any action to do ");

                if (action != null)
                {
                    Console.WriteLine("Action: " + initiator.CharacterName + " does " + action.Name + " to " +
                                      action.Target + "\n" + action.Parameters[1]);

                    //    _events.Add(EventHelper.ActionStart(initiator.CharacterName.ToString(), action.Name.ToString(),
                    //        action.Target.ToString()));

                    _events.Add(EventHelper.ActionEnd(initiator.CharacterName.ToString(), action.Name.ToString(),
                        action.Target.ToString()));

                    //         _events.Add(EventHelper.PropertyChange("DialogueState(" + initiator.CharacterName.ToString() + ")",
                    //                                   action.Parameters[1].ToString(), action.Target.ToString()));

                    Console.WriteLine();
                    Console.WriteLine("Dialogue:");
                    Console.WriteLine("Current State: " + action.Parameters[0].ToString());
                //    Console.WriteLine(initiator.CharacterName + " says: ''" +
                 //                    iat.GetDialogueActions(action.Parameters[0], action.Parameters[1], action.Parameters[2], action.Parameters[3]).FirstOrDefault().Utterance + "'' to " + action.Target);
                    Console.WriteLine("Next State: " + action.Parameters[1].ToString());

                }
                Console.WriteLine();
                Console.ReadKey();
            }
        }
    }
}


                //    actor.PerceptionActionLoop(new[] { _event });
                /*
                                foreach (var rpc in rpcList)
                                {

                                    if (rpc.m_kb.AskProperty(Name.BuildName("HasFloor(" + rpc.CharacterName + ")")) != null)
                                    {

                                        if (rpc.m_kb.AskProperty(Name.BuildName("HasFloor(" + rpc.CharacterName + ")")).ToString() ==
                                            "True")
                                            //aux = "True";
                                    }

                                    _actions.Add(rpc.Decide().FirstOrDefault());


                                    rpc.SaveToFile("../../../Examples/CiF/" + rpc.CharacterName + "-output" + ".rpc");
                                    }


                            /*
                                if (aux == "False")
                                {
                                    var rand = randomGen.Next(3);
                                    //     Console.WriteLine(rand + "");

                                    var next = rpcList.ElementAt(rand);

                                    Console.WriteLine("next: " + next.CharacterName + " index: " + rand);

                                    var finalEvents = new List<Name>();

                                    finalEvents.Add(EventHelper.PropertyChange("HasFloor(" + next.CharacterName + ")", "true",
                                        "Random"));


                                    next.Perceive(finalEvents);
                                }

                    */



    





