﻿using AutobiographicMemory;
using Conditions.DTOs;
using EmotionalAppraisal.AppraisalRules;
using EmotionalAppraisal.DTOs;
using EmotionalAppraisal.OCCModel;
using GAIPS.Rage;
using KnowledgeBase;
using SerializationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using WellFormedNames;

namespace EmotionalAppraisal
{
    /// <summary>
    /// Main class of the Emotional Appraisal Asset.
    /// </summary>
    [Serializable]
    public sealed partial class EmotionalAppraisalAsset : LoadableAsset<EmotionalAppraisalAsset>, ICustomSerialization
    {
        private ReactiveAppraisalDerivator m_appraisalDerivator;
        private Dictionary<string, EmotionDisposition> m_emotionDispositions;
        private EmotionDisposition m_defaultEmotionalDisposition;

        [NonSerialized]
        private long _lastFrameAppraisal = 0;

        [NonSerialized]
        private OCCAffectDerivationComponent m_occAffectDerivator;


        public EmotionDispositionDTO DefaultEmotionDisposition
        {
            get { return m_defaultEmotionalDisposition.ToDto(); }
            set { m_defaultEmotionalDisposition = new EmotionDisposition(value); }
        }

        /// <summary>
	    /// A short description of the asset's configuration
	    /// </summary>
	    public string Description { get; set; }

        protected override string OnAssetLoaded()
        {
            return null;
        }

        #region EmotionDispositions

        /// <summary>
        /// The asset's currently defined Emotion Dispositions.
        /// </summary>
        public IEnumerable<EmotionDispositionDTO> EmotionDispositions
        {
            get { return m_emotionDispositions.Values.Select(disp => disp.ToDto()); }
        }

        /// <summary>
        /// Returns the emotional dispotion associated to a given emotion type.
        /// </summary>
        /// <param name="emotionType">The emotion type key of the emotional disposition to retrieve</param>
        /// <returns>The dto containing the retrieved emotional disposition information</returns>
        public EmotionDispositionDTO GetEmotionDisposition(String emotionName)
        {
            EmotionDisposition disp;
            if (this.m_emotionDispositions.TryGetValue(emotionName, out disp))
                return disp.ToDto();

            return m_defaultEmotionalDisposition.ToDto();
        }

        /// <summary>
        /// Creates and adds an emotional disposition to the asset.
        /// </summary>
        /// <param name="emotionDispositionDto">The dto containing the parameters to create a new emotional disposition on the asset</param>
        public void AddEmotionDisposition(EmotionDispositionDTO emotionDispositionDto)
        {
            var disp = new EmotionDisposition(emotionDispositionDto);
            this.m_emotionDispositions.Add(disp.Emotion, disp);
        }

        /// <summary>
        /// Removes an emotional disposition from the asset.
        /// </summary>
        /// <param name="emotionType">The emotion type key of the emotional disposition to remove</param>
        public void RemoveEmotionDisposition(string emotionType)
        {
            this.m_emotionDispositions.Remove(emotionType);
        }

        #endregion EmotionDispositions

        /// <summary>
        /// The currently supported emotional type keys
        /// </summary>
        public static IEnumerable<string> EmotionTypes => OCCEmotionType.Types;

        /// <summary>
        /// Adds an emotional reaction to an event
        /// </summary>
        /// <param name="emotionalAppraisalRule">the AppraisalRule to add</param>
        public void AddOrUpdateAppraisalRule(AppraisalRuleDTO emotionalAppraisalRule)
        {
            m_appraisalDerivator.AddOrUpdateAppraisalRule(emotionalAppraisalRule);
        }

        /// <summary>
        /// Returns all the appraisal rules
        /// </summary>
        /// <returns>The set of dtos containing the information for all the appraisal rules</returns>
        public IEnumerable<AppraisalRuleDTO> GetAllAppraisalRules()
        {
            return this.m_appraisalDerivator.GetAppraisalRules().Select(r => new AppraisalRuleDTO
            {
                Id = r.Id,
                EventMatchingTemplate = r.EventName,
                Desirability = r.Desirability,
                Praiseworthiness = r.Praiseworthiness,
                Conditions = r.Conditions.ToDTO()
            });
        }

        /// <summary>
        /// Returns the condition set used for evaluating a particular appraisal rule set.
        /// </summary>
        /// <param name="ruleId">The unique identifier of the appraisal rule.</param>
        /// <returns>The dto of the condition set associated to the requested appraisal rule.</returns>
        /// <exception cref="ArgumentException">Thrown if the requested appraisal rule could not be found.</exception>
        public ConditionSetDTO GetAllAppraisalRuleConditions(Guid ruleId)
        {
            var rule = m_appraisalDerivator.GetAppraisalRule(ruleId);
            if (rule == null)
                throw new ArgumentException($"Could not found requested appraisal rule for the id \"{ruleId}\"", nameof(ruleId));

            return rule.Conditions.ToDTO();
        }

        /// <summary>
        /// Removes appraisal rules from the asset.
        /// </summary>
        /// <param name="appraisalRules">A dto set of the appraisal rules to remove</param>
        public void RemoveAppraisalRules(IEnumerable<AppraisalRuleDTO> appraisalRules)
        {
            foreach (var appraisalRuleDto in appraisalRules)
            {
                m_appraisalDerivator.RemoveAppraisalRule(new AppraisalRule(appraisalRuleDto));
            }
        }

        /// <summary>
        /// Asset constructor.
        /// Creates a new empty Emotional Appraisal Asset.
        /// </summary>
        /// <param name="perspective">The initial perspective of the asset.</param>
        public EmotionalAppraisalAsset()
        {
            m_emotionDispositions = new Dictionary<string, EmotionDisposition>();
            m_defaultEmotionalDisposition = new EmotionDisposition("*", 1, 1);
            m_occAffectDerivator = new OCCAffectDerivationComponent();
            m_appraisalDerivator = new ReactiveAppraisalDerivator();
        }

        /// <summary>
        /// Appraises a set of event strings.
        ///
        /// Durring appraisal, the events will be recorded in the asset's autobiographical memory,
        /// and Property Change Events will update the asset's knowledge about the world, allowing
        /// the asset to use the new information derived from the events to appraise the correspondent
        /// emotions.
        /// </summary>
        /// <param name="eventNames">A set of string representation of the events to appraise</param>
        public void AppraiseEvents(IEnumerable<Name> eventNames, Name perspective, IEmotionalState emotionalState, AM am, KB kb)
        {
            var appraisalFrame = new InternalAppraisalFrame();
            appraisalFrame.Perspective = kb.Perspective;

            foreach (var n in eventNames)
            {
                var evt = am.RecordEvent(n, am.Tick);
                var propEvt = evt as IPropertyChangedEvent;
                if (propEvt != null)
                {
                    var fact = propEvt.Property;
                    var value = propEvt.NewValue;
                    kb.Tell(fact, value, perspective);
                }

                appraisalFrame.Reset(evt);
                var componentFrame = appraisalFrame.RequestComponentFrame(m_appraisalDerivator, m_appraisalDerivator.AppraisalWeight);
                m_appraisalDerivator.Appraisal(kb, evt, componentFrame);
                UpdateEmotions(appraisalFrame, emotionalState, am);
            }
        }

        public void AppraiseEvents(IEnumerable<Name> eventNames, IEmotionalState emotionalState, AM am, KB kb)
        {
            AppraiseEvents(eventNames, Name.SELF_SYMBOL, emotionalState, am, kb);
        }

        private void UpdateEmotions(IAppraisalFrame frame, IEmotionalState emotionalState, AM am)
        {
            if (_lastFrameAppraisal > frame.LastChange)
            {
                return;
            }

            var emotions = m_occAffectDerivator.AffectDerivation(this, frame);
            foreach (var emotion in emotions)
            {
                var activeEmotion = emotionalState.AddEmotion(emotion, am, GetEmotionDisposition(emotion.EmotionType), am.Tick);
                if (activeEmotion == null)
                    continue;
            }

            _lastFrameAppraisal = frame.LastChange;
        }

        /// @cond DEV

        #region ICustomSerialization

        public void GetObjectData(ISerializationData dataHolder, ISerializationContext context)
        {
            dataHolder.SetValue("Description", Description);
            dataHolder.SetValue("AppraisalRules", m_appraisalDerivator);
            dataHolder.SetValue("EmotionDispositions", m_emotionDispositions.Values.Prepend(m_defaultEmotionalDisposition).ToArray());
        }

        public void SetObjectData(ISerializationData dataHolder, ISerializationContext context)
        {
            Description = dataHolder.GetValue<string>("Description");

            m_appraisalDerivator = dataHolder.GetValue<ReactiveAppraisalDerivator>("AppraisalRules");
            m_occAffectDerivator = new OCCAffectDerivationComponent();

            if (m_emotionDispositions == null)
                m_emotionDispositions = new Dictionary<string, EmotionDisposition>();
            else
                m_emotionDispositions.Clear();

            var dispositions = dataHolder.GetValue<EmotionDisposition[]>("EmotionDispositions");
            EmotionDisposition defaultDisposition = null;
            foreach (var disposition in dispositions)
            {
                if (string.Equals(disposition.Emotion, "*", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (defaultDisposition == null)
                        defaultDisposition = disposition;
                }
                else
                    m_emotionDispositions.Add(disposition.Emotion, disposition);
            }
            if (defaultDisposition == null)
                defaultDisposition = new EmotionDisposition("*", 1, 1);
            m_defaultEmotionalDisposition = defaultDisposition;
        }

        #endregion ICustomSerialization

        /// @endcond
    }
}