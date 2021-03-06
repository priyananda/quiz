﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Linq;

namespace Shenoy.Question.Model
{
    public abstract class Question
    {
        public Question(int id, QuestionType type)
        {
            m_id = id;
            m_type = type;
            m_relatedQid = -1;
            m_fAnswered = false;
        }
        public virtual void Load(XElement elem)
        {
            if (this.Type != QuestionType.Concept)
            {
                var answerNode = elem.Element("answer");
                int slideid = Int32.Parse(answerNode.Attribute("slideid").Value);
                m_answer = new Answer(this, slideid);
                if (elem.Attribute("e") != null)
                    m_fExhaustive = true;
                if (elem.Attribute("s") != null)
                    m_fInOrder = true;
                if (elem.Attribute("y") != null)
                    m_fSilly = true;
                if (elem.Attribute("l") != null)
                    m_fLimited = true;
            }
            if (this.Type != QuestionType.StagedConnect)
            {
                if (elem.Attribute("points") != null)
                    m_Points = Int32.Parse(elem.Attribute("points").Value);
            }
            if (elem.Attribute("related") != null)
                m_relatedQid = Int32.Parse(elem.Attribute("related").Value);
        }
        public void AnswerQuestion()
        {
            if (!m_fAnswered)
            {
                m_fAnswered = true;
                if (Answered != null)
                    Answered(this);
                Log.SaveState();
            }
        }
        public abstract void Advance();

        public virtual int Points { get { return m_Points; } }
        public int Id { get { return m_id; } }
        public QuestionType Type { get { return m_type; } }
        public bool IsAnswered { get { return m_fAnswered; } }
        public bool AllPlay { get { return m_type == QuestionType.StagedConnect || m_type == QuestionType.LongList; } }
        public Answer Ans { get { return m_answer; } }
        public int RelatedQ { get { return m_relatedQid; } }
        public bool Exhaustive { get { return m_fExhaustive; } }
        public bool InOrder { get { return m_fInOrder; } }
        public bool Silly { get { return m_fSilly; } }
        public bool Limited { get { return m_fLimited; } } 
        public String Name { get { return "" + m_id; } }
        
        public event Action<Question> Answered;

        private int m_id;
        private QuestionType m_type;
        private bool m_fAnswered;
        private Answer m_answer;
        private int m_relatedQid;
        private bool m_fExhaustive;
        private bool m_fInOrder;
        private bool m_fSilly;
        private bool m_fLimited;
        protected int m_Points = Constants.PointsForSimpleQ;
    }
}
