using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Linq;

namespace Shenoy.Quiz.Model
{
    public enum QuestionDifficulty
    {
        Red,
        Yellow,
        Green
    }

    public abstract class Question
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
            m_metamodifier = MetaModifiers._Max;
        }
        public virtual void Load(XElement elem)
        {
            if (this.Type != QuestionType.Concept)
            {
                m_answer = Answer.Parse(this, elem.Element("answer"));
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
            m_person = (Person)Enum.Parse(typeof(Person), elem.Attribute("person").Value);
            if (elem.Attribute("mm") != null)
                m_metamodifier = (MetaModifiers)Enum.Parse(typeof(MetaModifiers), elem.Attribute("mm").Value);
            if (elem.Attribute("ryg") != null)
            {
                switch (elem.Attribute("ryg").Value)
                {
                    case "R": m_difficulty = QuestionDifficulty.Red; break;
                    case "Y": m_difficulty = QuestionDifficulty.Yellow; break;
                    case "G": m_difficulty = QuestionDifficulty.Green; break;
                }
            }

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
        public void Open()
        {
            if (!m_fOpened)
            {
                m_fOpened = true;
                if (this.m_metamodifier != MetaModifiers._Max)
                    Quiz.Current.MetaManager.Activate(this.m_metamodifier);
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
        public Person Person { get { return m_person; } }
        public MetaModifiers MetaModifier { get { return m_metamodifier; } }
        public QuestionDifficulty Difficulty { get { return m_difficulty; } }

        public event Action<Question> Answered;

        private int m_id;
        private QuestionType m_type;
        private bool m_fOpened;
        private bool m_fAnswered;
        private Answer m_answer;
        private int m_relatedQid;
        private bool m_fExhaustive;
        private bool m_fInOrder;
        private bool m_fSilly;
        private bool m_fLimited;
        protected int m_Points = Constants.PointsForSimpleQ;
        private Person m_person;
        private MetaModifiers m_metamodifier;
        private QuestionDifficulty m_difficulty;

        public List<ObjectWithSlide> Slides { get; set; }
    }

    
}
