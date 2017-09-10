﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Linq;

namespace ConnQuiz.Model
{
    abstract class Question
    {
        public static Question Create(XElement elem)
        {
            Question ret = null;
            int id = Int32.Parse(elem.Attribute("id").Value);
            switch (elem.Attribute("type").Value)
            {
                case "simple":
                    ret = new SimpleQuestion(id);
                    break;
                case "connect":
                    ret = new ConnectQuestion(id);
                    break;
                case "stagedconnect":
                    ret = new StagedConnectQuestion(id);
                    break;
                case "longlist":
                    ret = new LongListQuestion(id);
                    break;
                case "concept":
                    ret = new Concept(id);
                    break;
            }
            ret.Load(elem);
            return ret;
        }
        public Question(int id, QuestionType type)
        {
            m_id = id;
            m_type = type;
            m_relatedQid = -1;
            m_fAnswered = false;
        }
        public virtual void Load(XElement elem)
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
            if (elem.Attribute("label") != null)
                m_label = elem.Attribute("label").Value;
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

        public String GetText()
        {
            if (m_type == QuestionType.Concept)
                return m_label;
            return String.Format("Q{0}\n{1}|-{2}", m_id, this.Points, "0");
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
        private String m_label;
    }

    class Questions
    {
        public static Question Get(int i)
        {
            return m_questions[i];
        }
        public static void Load(string filename)
        {
            XElement questions = XElement.Load(filename);
            var qkids = questions.Elements("question");
            m_questions = new Question[qkids.Count() + 1];
            int i = 1;
            foreach (var qelem in qkids)
            {
                m_questions[i] = Question.Create(qelem);
                m_questions[i].Answered += new Action<Question>(OnQuestionAnswered);
                if (!m_questions[i++].AllPlay)
                    ++m_csingleplay;
            }
            m_chalfway = m_csingleplay / 2;
            Clue.ResolveConnections();
        }

        public static IEnumerable<Question> QList
        {
            get { return m_questions; }
        }
        public static int Count
        {
            get { return m_questions.Length; }
        }
        public static Color ColorForQuestion(int qid)
        {
            float x = 0.1f + (qid) / (50.0f + m_questions.Length);
            return Color.FromScRgb(1.0f, x, x, x);
        }
        private static void OnQuestionAnswered(Question obj)
        {
            if (!obj.AllPlay)
                m_csingleplay--;
            if (m_csingleplay == m_chalfway)
                if (DirectionChange != null)
                    DirectionChange();
        }
        private static Question[] m_questions;
        private static int m_csingleplay;
        private static int m_chalfway;
        public static event Action DirectionChange;
    }
}
