using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Shenoy.Quiz.Model
{
    public enum QuestionType
    {
        Simple,
        Connect,
        StagedConnect,
        LongList,
        Concept
    }
    class Constants
    {
        public const int NumTeams = 6;
        public const int PointsForSimpleQ = 10;
        public const int PointsForUnknownClue = 10;
        public const int PointsPerStage = 10;
        public const int PointsPerListItem = 2;
    }
    class Log
    {
        public static void SaveState()
        {
            if (m_writer == null)
            {
                m_writer = new StreamWriter("state.log", true);
                m_writer.AutoFlush = true;
            }
            m_writer.Write("T");
            for (int i = 0; i < Constants.NumTeams; ++i)
                m_writer.Write(" " + Teams.Get(i).Points);
            m_writer.Write(" Q");
            Questions qs = Quiz.Current.Questions;
            for (int i = 1; i < qs.Count; ++i)
                m_writer.Write(qs.Get(i).IsAnswered ? " 1" : " 0");
            m_writer.WriteLine("");
        }
        public static void RestoreState()
        {
            StreamReader reader = new StreamReader("state.log");
            String line = null, last = null;
            while ((line = reader.ReadLine()) != null)
                last = line;
            if (last != null)
            {
                string[] split = last.Split(' ');
                Questions qs = Quiz.Current.Questions;
                if ((split.Length == Constants.NumTeams + qs.Count + 2) &&
                    (split[0] == "T") && (split[Constants.NumTeams + 1] == "Q"))
                {
                    for (int i = 0; i < Constants.NumTeams; ++i)
                        Teams.Get(i).AddPoints(Int32.Parse(split[i + 1]) - Teams.Get(i).Points);

                    for (int i = 1; i < qs.Count; ++i)
                        if (Int32.Parse(split[Constants.NumTeams + i + 2]) == 1)
                            qs.Get(i).AnswerQuestion();
                }
            }
        }
        static StreamWriter m_writer;
    }
}
