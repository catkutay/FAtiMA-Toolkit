﻿using System.Collections.Generic;
using EmotionalAppraisal.Interfaces;
using KnowledgeBase.WellFormedNames;

namespace EmotionalAppraisal
{
    public static class EventOperations
    {
        public static bool MatchEvents(IEvent matchRule, IEvent eventPerception)
        {
            if (matchRule.Action != null && !matchRule.Action.Equals(eventPerception.Action))
                return false;

            if (matchRule.Subject != null && eventPerception.Subject != null)
            {
                if (!matchRule.Subject.Equals(eventPerception.Subject))
                    return false;
            }

            if (matchRule.Target != null && !matchRule.Target.Equals(eventPerception.Target))
                return false;

            IEnumerator<IEventParameter> it1 = matchRule.Parameters.GetEnumerator();
            IEnumerator<IEventParameter> it2 = eventPerception.Parameters.GetEnumerator();

            while (it1.MoveNext() && it2.MoveNext())
            {
                if (!it1.Current.ParameterName.Equals(it2.Current.ParameterName))
                    return false;

                if (!it1.Current.Value.Equals("*") && !it1.Current.Value.Equals(it2.Current.Value))
                    return false;
            }

            if (it1.MoveNext() || it2.MoveNext())
                return false;

            return true;
        }

	    public static Name ToName(this IEvent evt)
	    {
			List<Name> terms = new List<Name>();
			terms.Add(new Symbol("Event"));
			terms.Add(new Symbol(evt.Subject));
			terms.Add(new Symbol(evt.Action));
			terms.Add(evt.Target==null?Symbol.UNIVERSAL_SYMBOL:new Symbol(evt.Target));
		    if (evt.Parameters != null)
		    {
				foreach (var parameter in evt.Parameters)
				{
					var p = new ComposedName(new Symbol(parameter.ParameterName), new Symbol(parameter.Value.ToString()));
					terms.Add(p);
				}
		    }
			return new ComposedName(terms);
	    }

	    /// <summary>
	    /// Generates a set of bindings that associate the Variables
	    /// [Subject],[Action],[Target],[P1_Name],[P2_Name],... respectively to the event's subject, action, target and parameters 
	    /// </summary>
	    /// <returns>the mentioned set of substitutions</returns>
	    public static IEnumerable<Substitution> GenerateBindings(this IEvent evt)
	    {
			yield return new Substitution("[Subject]", evt.Subject);
			yield return new Substitution("[Action]", evt.Action);
			yield return new Substitution("[Target]", evt.Target ?? Symbol.UNIVERSAL_STRING);

			foreach (var p in evt.Parameters)
			{
				yield return new Substitution("[" + p.ParameterName + "]", p.Value.ToString());
			}
	    }
    }
}