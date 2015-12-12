﻿using System;
using System.Linq;
using EmotionalAppraisal.Interfaces;
using KnowledgeBase.WellFormedNames;

namespace EmotionalAppraisal
{
	[Serializable]
	public class Cause
	{
		public readonly Name CauseName;
		public readonly SubstitutionSet CauseParameters;
		public readonly DateTime CauseTimestamp;

		public Cause(IEvent evt, string perspective)
		{
			Name n = Name.BuildName(
					Name.BuildName("Event"),
					Name.BuildName(evt.Subject),
					Name.BuildName(evt.Action),
					evt.Target==null?Name.NIL_SYMBOL:Name.BuildName(evt.Target)
				);

			if (!string.IsNullOrEmpty(perspective))
				n = n.ApplyPerspective(perspective);
			CauseName = n;

			SubstitutionSet set = null;
			if (evt.Parameters!=null && evt.Parameters.Any())
			{
				var subs = evt.Parameters.Select(p => new Substitution(Name.BuildName("[" + p.ParameterName + "]"), Name.BuildName(p.Value.ToString())));
				set = new SubstitutionSet(subs);
			}
			CauseParameters = set;
			CauseTimestamp = evt.Timestamp;
		}

		public bool Match(Cause other)
		{
			if (!CauseName.Match(other.CauseName))
				return false;

			var b = CauseParameters == null;
			if ((b == (other.CauseParameters == null)) && !b)
			{
				foreach (var sub in CauseParameters)
				{
					Name n = other.CauseParameters[sub.Variable];
					if (n != null && !n.Match(sub.Value))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}