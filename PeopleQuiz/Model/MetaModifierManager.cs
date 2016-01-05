using System;
using System.Collections.Generic;

namespace Shenoy.Quiz.Model
{
    public class MetaModifierManager
    {
        public MetaModifierManager(Quiz quiz)
        {
            m_fixedModifiers[MetaModifiers.ShowFirstSet] = new EnableAllInset();
            m_fixedModifiers[MetaModifiers.DoublesDuration] = new DoublesDuration();
            m_fixedModifiers[MetaModifiers.HalvesDuration] = new HalvesDuration();
            m_fixedModifiers[MetaModifiers.RedYellowGreen] = new RedYellowGreen();

            m_celebModifiers[Celeb.Kejriwal] = new DoAKejriwal();
            m_celebModifiers[Celeb.Jayalalitha] = new DoAJayalalitha();
            m_celebModifiers[Celeb.RahulG] = new DoARahulGandhi();
            quiz.Questions.QuestionAnswered += OnQuestionAnswered;
        }

        public void Activate(MetaModifiers mm)
        {
            if (mm != MetaModifiers._Max && mm != MetaModifiers.Celeb && m_fixedModifiers.ContainsKey(mm))
                DoActivate(m_fixedModifiers[mm]);
        }

        public void Activate(Celeb celeb)
        {
            if (celeb != Celeb._Max && m_celebModifiers.ContainsKey(celeb))
                DoActivate(m_celebModifiers[celeb]);
        }

        private void DoActivate(MetaModifier metaModifier)
        {
            if (metaModifier.State == MetaModifierState.Dormant)
            {
                metaModifier.Activate();
                metaModifier.Apply();
            }
        }

        private void OnQuestionAnswered(Question obj)
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            foreach(var modifier in m_fixedModifiers.Values)
            {
                if (modifier.State == MetaModifierState.Active)
                    modifier.Apply();
            }
            foreach (var modifier in m_celebModifiers.Values)
            {
                if (modifier.State == MetaModifierState.Active)
                    modifier.Apply();
            }
        }

        private Dictionary<MetaModifiers, MetaModifier> m_fixedModifiers = new Dictionary<MetaModifiers, MetaModifier>();
        private Dictionary<Celeb, MetaModifier> m_celebModifiers = new Dictionary<Celeb, MetaModifier>();
    }
}