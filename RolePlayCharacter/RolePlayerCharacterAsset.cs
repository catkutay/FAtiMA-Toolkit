﻿using System;
using System.IO;
using System.Collections.Generic;
using EmotionalAppraisal;
using EmotionalDecisionMaking;
using GAIPS.Serialization;
using AutobiographicMemory;
using KnowledgeBase.WellFormedNames;
using RolePlayCharacter.Utilities;

namespace RolePlayCharacter
{
    public enum LOADTYPE
    {
        FROMDATABASE = 0,
        FROMFILE,
        NONE
    }

    public enum FILETYPE
    {
        XML = 0,
        JSON,
        NONE
    }

    public static class FILETYPEEXTENSION
    {
        public static string XML { get { return ".xml"; } }
        public static string JSON { get { return ".json"; } }
    }

    [Serializable]
    public class RPCEvent : IEventRecord
    {
        public string Target { get; set; }
        public string Subject { get; set; }
        public DateTime Timestamp { get; set; }

        public uint Id { get; set; }
        public string EventType { get; set; }

        public Name Action { get; set; }

        public Name EventName { get; set; }
		
        public IEnumerable<string> LinkedEmotions { get; set; }

        public Name EventObject { get; set; }

        public void LinkEmotion(string emotionType) {}

		//NOTE: Custom serialization is not needed in this case.
		//      Just use System.NonSerializedAttribute on the fields you don't require serialization (if they are serialized by default)
		//public void GetObjectData(ISerializationData dataHolder)
  //      {
  //          Target = dataHolder.GetValue<string>("Target");
  //          Subject = dataHolder.GetValue<string>("Subject");
  //          Timestamp = dataHolder.GetValue<DateTime>("TimeStamp");
  //          Id = dataHolder.GetValue<uint>("ID");
  //          EventType = dataHolder.GetValue<string>("EventType");
  //          EventName = dataHolder.GetValue<Name>("EventName");
  //          Action = dataHolder.GetValue<Name>("Action");
  //      }

  //      public void SetObjectData(ISerializationData dataHolder)
  //      {
  //          dataHolder.SetValue<string>("Target", Target);
  //          dataHolder.SetValue<string>("Subject", Subject);
  //          dataHolder.SetValue<DateTime>("TimeStamp", Timestamp);
  //          dataHolder.SetValue<uint>("ID", Id);
  //          dataHolder.SetValue<string>("EventType", EventType);
  //          dataHolder.SetValue<Name>("EventName", EventName);
  //          dataHolder.SetValue<Name>("Action", Action);
  //      }
    }

    [Serializable]
    public sealed partial class RolePlayCharacterAsset
    {
        private EmotionalAppraisalAsset _emotionalAppraisalAsset;
        private EmotionalDecisionMakingAsset _emotionalDecisionMakingAsset;

        private List<RPCEvent> _rpcEvents = new List<RPCEvent>();
        private List<Name> _rpcEventsName = new List<Name>();

        private IEnumerable<IAction> _rpcActions;

        private static LOADTYPE _loadType = LOADTYPE.NONE;
        private static FILETYPE _fileType = FILETYPE.NONE;

        private string _assetId;

        public string AssetId
        {
            get { return _assetId; }

            set { _assetId = value; }
        }

        private static string _currentFileAsset;

        private string _emotionalAppraisalPath;

        public string EmotionalAppraisalPath
        {
            get { return _emotionalAppraisalPath; }
            set { _emotionalAppraisalPath = value; }
        }

        private string _emotionalDecisionMakingPath;

        public string EmotionalDecisionMakingPath
        {
            get { return _emotionalDecisionMakingPath; }

            set { _emotionalDecisionMakingPath = value; }
        }

        private static string GetFileToLoad(string idAsset)
        {
            try
            {
                if (_fileType == FILETYPE.NONE)
                    throw new ParameterNotDefiniedException(new Messages().FILEEXTENSIONNOTDEFINED().ShowMessage());

                if (_loadType == LOADTYPE.FROMFILE)
                    throw new ParameterNotDefiniedException(new Messages().LOADTYPEINCORRECT().ShowMessage());

                return _currentFileAsset;
            }

            catch (ParameterNotDefiniedException exception)
            {
                Console.WriteLine(exception.Message);
            }

            return "Default";
        }

        public static RolePlayCharacterAsset LoadFromFile(string idAsset)
        {
            RolePlayCharacterAsset rpc;
           string filename = GetFileToLoad(idAsset);
           
            using (var f = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                var serializer = new JSONSerializer();
                rpc = serializer.Deserialize<RolePlayCharacterAsset>(f);
            }
            return rpc;
        }

        public static RolePlayCharacterAsset LoadFromDatabase()
        {
            RolePlayCharacterAsset rpc = new RolePlayCharacterAsset();
            return rpc;
        }

        public static RolePlayCharacterAsset LoadAsset(LOADTYPE loadType)
        {
            RolePlayCharacterAsset rpc = new RolePlayCharacterAsset() ;

            switch(loadType)
            {
                case LOADTYPE.FROMFILE:
                    try
                    {
                       return LoadFromFile(_currentFileAsset);
                    }

                    catch(FileNotFoundException exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                    break;
                case LOADTYPE.FROMDATABASE:
                    break;
                default:
                    break;
            }

            return rpc;
        }

        public void SetLoadingType()
        {

        }

        public RolePlayCharacterAsset ()
        {
          
        }

        public void SetEmotionAppraisalModulePath(string emotionalAppraisalPath)
        {
            _emotionalAppraisalPath = emotionalAppraisalPath;
        }

        public void SetEmotionalAppraisalModule(string name)
        {
            LoadEmotionAppraisalAssetFromFile(name);
        }

        public void SetEmotionalDecisionModulePath(string emotionalDecisionPath)
        {
            _emotionalDecisionMakingPath = emotionalDecisionPath;
        }

        public void SetEmotionalDecisionModule()
        {
            _emotionalDecisionMakingAsset = new EmotionalDecisionMakingAsset(_emotionalAppraisalAsset);
        }

        public void SaveToFile(Stream file)
        {
            var serializer = new JSONSerializer();
            serializer.Serialize(file, this);
        }

        public void AddRPCEvent(RPCEvent evt)
        {
            _rpcEvents.Add(evt);
            _rpcEventsName.Add(evt.EventName);
        }

        public void PerceiveEvent()
        {
            try
            {
                if (_emotionalAppraisalAsset != null)
                    _emotionalAppraisalAsset.AppraiseEvents(_rpcEventsName);

                else throw new FunctionalityNotImplementedException(new Messages().EMOTIONALAPPRAISALNOTIMPLEMENTED().ShowMessage());

                if (_emotionalDecisionMakingAsset != null)
                    _rpcActions = _emotionalDecisionMakingAsset.Decide();

                else throw new FunctionalityNotImplementedException(new Messages().EMOTIONDECISIONNOTIMPLEMENTED().ShowMessage());
            }
            
            catch(FunctionalityNotImplementedException exception)
            {
                Console.WriteLine(exception.ExceptionMessage);
            }  
        }

        public void RetrieveAction()
        {



        }

        private void LoadEmotionAppraisalAssetFromFile(string name)
        {
            if(name != null)
            {
                _emotionalAppraisalAsset = EmotionalAppraisalAsset.LoadFromFile(name);
            }
        }

        private void LoadEmotionDecisionMakingAssetFromFile(string name)
        {
            if(name != null)
            {
               // _emotionalDecisionMakingAsset = EmotionalDecisionMakingAsset.LoadFromFile(name);
            }
        }

        private void LoadEmotionDecisionMakingAsset(EmotionalAppraisalAsset eaa)
        {
            _emotionalDecisionMakingAsset = new EmotionalDecisionMakingAsset(eaa);

        }
        public float GetCharacterMood() { return _emotionalAppraisalAsset.Mood; }

        public void GetObjectData(ISerializationData dataHolder)
        {
            dataHolder.SetValue("EmotionalAppraisalPath", _emotionalAppraisalPath);
            dataHolder.SetValue("EmotionalDecisionMakingPath", _emotionalDecisionMakingPath);
            dataHolder.SetValue("RPC_Events", _rpcEvents);
            dataHolder.SetValue("RPC_Actions", _rpcActions);
            dataHolder.SetValue("RPC_Events_Names", _rpcEventsName);
        }

        public void SetObjectData(ISerializationData dataHolder)
        {
            _emotionalDecisionMakingPath = dataHolder.GetValue<string>("EmotionalDecisionMakingPath");
            _emotionalAppraisalPath = dataHolder.GetValue<string>("EmotionalAppraisalPath");
            _rpcEvents = dataHolder.GetValue<List<RPCEvent>>("RPC_Events");
            _rpcEventsName = dataHolder.GetValue<List<Name>>("RPC_Events_Names");
            _rpcActions = dataHolder.GetValue<IEnumerable<IAction>>("RPC_Actions");
        }
    }
}