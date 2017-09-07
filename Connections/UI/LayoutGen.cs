using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ConnQuiz.Model;
using System.IO;

namespace ConnQuiz.UI
{
    class LayoutGen
    {
        public static void Generate(string outfilename)
        {
            XElement elemRoot;
            XElement designerElems;
            XElement connsElems;
            bool fReload = true;

            LoadTemplate();

            if (!File.Exists(outfilename))
                fReload = true;

            if (!fReload)
            {
                elemRoot = new XElement("Root");
                elemRoot.Add(new XElement("DesignerItems"));
                elemRoot.Add(new XElement("Connections"));
            }
            else
            {
                elemRoot = XElement.Load(outfilename);
            }

            designerElems = elemRoot.Element("DesignerItems");
            connsElems = elemRoot.Element("Connections");

            bool fNewConnsAdded = CreateElements(designerElems, connsElems, fReload);

            if (fNewConnsAdded)
                _UpdateConns(designerElems, connsElems);

            elemRoot.Save(outfilename);
        }

        private static void _UpdateConns(XElement designerElems, XElement connsElems)
        {
            var map = new Dictionary<string, string>(); //guid->qid
            foreach (var item in designerElems.Elements("DesignerItem"))
                map.Add(item.Element("ID").Value, item.Element("Question").Value);
            foreach (var conn in connsElems.Elements("Connection"))
            {
                XElement tag = conn.Element("Tag");
                if (tag == null)
                {
                    tag = new XElement("Tag");
                    conn.Add(tag);
                }
                string qidSrc = map[conn.Element("SourceID").Value];
                string qidDest = map[conn.Element("SinkID").Value];
                tag.Value = qidSrc + "=" + qidDest;
            }
        }

        private static bool CreateElements(XElement designerElems, XElement connsElems, bool fReload)
        {
            Random rand = new Random();
            String[] questionGuids = new string[Questions.Count];
            for (int i = 1; i < questionGuids.Length; ++i)
            {
                if (!fReload || (questionGuids[i] = GuidForQuestion(designerElems, i)) == null)
                    questionGuids[i] = Guid.NewGuid().ToString();
            }
            foreach (Question question in Questions.QList)
            {
                XElement qelem = null;

                if (question == null)
                    continue;

                if (fReload && GuidForQuestion(designerElems, question.Id) != null)
                    continue;

                switch (question.Type)
                {
                    case QuestionType.Simple:
                        qelem = XElement.Parse(tmplSimpleQ);
                        break;
                    case QuestionType.Connect:
                        qelem = XElement.Parse(tmplConnectQ);
                        break;
                    case QuestionType.StagedConnect:
                        qelem = XElement.Parse(tmplStagedGroup);
                        break;
                    case QuestionType.LongList:
                        qelem = XElement.Parse(tmplLongList);
                        break;
                    case QuestionType.Concept:
                        qelem = XElement.Parse(tmplConcept);
                        break;
                }
                qelem.Element("ID").Value = questionGuids[question.Id];
                qelem.Element("Question").Value = question.Id + "";
                qelem.Element("zIndex").Value = question.Id + "";
                RandomizeLocation(rand, qelem);
                designerElems.Add(qelem);

                if (question.Type == QuestionType.Simple)
                {
                    SimpleQuestion sq = question as SimpleQuestion;
                    if (sq.Clue != null && sq.Clue.Source != null)
                    {
                        XElement connElem = CreateConn(questionGuids, sq.Clue.Source, qelem);
                        RandomizeConnection(rand, connElem);
                        connsElems.Add(connElem);
                    }
                }

                if (question.Type == QuestionType.Connect)
                {
                    ConnectQuestion cq = question as ConnectQuestion;
                    foreach (Clue clue in cq.Clues)
                    {
                        if (clue.Source != null)
                        {
                            XElement connElem = CreateConn(questionGuids, clue.Source, qelem);
                            RandomizeConnection(rand, connElem);
                            connsElems.Add(connElem);
                        }
                    }
                }

                if (question.Type == QuestionType.StagedConnect)
                {
                    var stage1 = XElement.Parse(tmplStage1);
                    var stage2 = XElement.Parse(tmplStage2);
                    var stage3 = XElement.Parse(tmplStage3);

                    stage1.Element("ID").Value = Guid.NewGuid().ToString();
                    stage2.Element("ID").Value = Guid.NewGuid().ToString();
                    stage3.Element("ID").Value = Guid.NewGuid().ToString();

                    stage1.Element("ParentID").Value = qelem.Element("ID").Value;
                    stage2.Element("ParentID").Value = qelem.Element("ID").Value;
                    stage3.Element("ParentID").Value = qelem.Element("ID").Value;

                    stage1.Element("Top").Value = qelem.Element("Top").Value;
                    stage2.Element("Top").Value = qelem.Element("Top").Value;
                    stage3.Element("Top").Value = qelem.Element("Top").Value;

                    int left = Int32.Parse(qelem.Element("Left").Value);
                    stage1.Element("Left").Value = (left + 36 + 36) + "";
                    stage2.Element("Left").Value = (left + 36) + "";
                    stage3.Element("Left").Value = left + "";

                    stage1.Element("Question").Value = qelem.Element("Question").Value;
                    stage2.Element("Question").Value = qelem.Element("Question").Value;
                    stage3.Element("Question").Value = qelem.Element("Question").Value;

                    designerElems.Add(stage1);
                    designerElems.Add(stage2);
                    designerElems.Add(stage3);

                    StagedConnectQuestion cq = question as StagedConnectQuestion;
                    for (int i = 0; i < cq.ClueSets.Length; ++i)
                    {
                        ClueSet clueset = cq.ClueSets[i];
                        foreach (Clue clue in clueset)
                        {
                            if (clue.Source != null)
                            {
                                XElement connElem = CreateConn(questionGuids, clue.Source,
                                    (i == 0) ? stage1 : ((i == 1) ?  stage2 : stage3) );
                                RandomizeConnection(rand, connElem);
                                connsElems.Add(connElem);
                            }
                        }
                    }
                    qelem = stage1;
                }

                if (question.RelatedQ >= 0)
                {
                    XElement connElem = CreateConn(questionGuids, question.RelatedQ, qelem, true);
                    RandomizeConnection(rand, connElem, true);
                    connsElems.Add(connElem);
                }
                else if (question.Type == QuestionType.Concept)
                {
                    foreach (int qid in ((Concept)question).RelatedQuestions)
                    {
                        XElement connElem = CreateConn(questionGuids, qid, qelem, true);
                        RandomizeConnection(rand, connElem, true);
                        connsElems.Add(connElem);
                    }
                }
            }

            return false;
        }

        private static string GuidForQuestion(XElement designerElems, int qid)
        {
            string sqid = qid.ToString();
            foreach (var item in designerElems.Elements("DesignerItem"))
                if (item.Element("Question").Value == sqid)
                    return item.Element("ID").Value;
            return null;
        }

        private static XElement CreateConn(string[] questionGuids, ConnQuiz.Model.Connection connection, XElement qelem)
        {
            return CreateConn(questionGuids, connection.SourceQ.Id, qelem, false);
        }

        private static XElement CreateConn(string[] questionGuids, int from, XElement qelem, bool weak = false)
        {
            XElement connElem = XElement.Parse(weak ? tmplRelation : tmplConnection);
            connElem.Element("SourceID").Value = questionGuids[from];
            connElem.Element("SinkID").Value = qelem.Element("ID").Value;
            return connElem;
        }

        private static void RandomizeLocation(Random rand, XElement qelem)
        {
            int top = (int)(500 * rand.NextDouble());
            int left = (int)(700 * rand.NextDouble());
            qelem.Element("Left").Value = left + "";
            qelem.Element("Top").Value = top + "";
        }

        private static void RandomizeConnection(Random rand, XElement connelem, bool weak = false)
        {
            int src = rand.Next(4);
            int dest = rand.Next(4);
            var strs = Enum.GetNames(typeof(ConnectorOrientation));
            if (weak)
                connelem.Element("SourceConnectorName").Value = strs[src + 1];
            connelem.Element("SinkConnectorName").Value = strs[dest + 1];
        }

        private static void LoadTemplate()
        {
            var elemRoot = XElement.Load("template.xml");
            var ditemRoot = elemRoot.Element("DesignerItems");
            var connsRoot = elemRoot.Element("Connections");

            var ditems = ditemRoot.Elements("DesignerItem");
            int i = 0;
            foreach(var ditem in ditems)
            {
                switch (i++)
                {
                    case 0: tmplSimpleQ = ditem.ToString(); break;
                    case 1: tmplConnectQ = ditem.ToString(); break;
                    case 2: tmplStage1 = ditem.ToString(); break;
                    case 3: tmplStage2 = ditem.ToString(); break;
                    case 4: tmplStage3 = ditem.ToString(); break;
                    case 5: tmplLongList = ditem.ToString(); break;
                    case 6: tmplStagedGroup = ditem.ToString(); break;
                    case 7: tmplConcept = ditem.ToString(); break;
                }
            }
            tmplConnection = connsRoot.Elements("Connection").First().ToString();
            tmplRelation = connsRoot.Elements("Connection").Last().ToString();
        }
        private static string tmplSimpleQ;
        private static string tmplConnectQ;
        private static string tmplStagedGroup;
        private static string tmplStage1;
        private static string tmplStage2;
        private static string tmplStage3;
        private static string tmplLongList;
        private static string tmplConcept;
        private static string tmplConnection;
        private static string tmplRelation;
    }
}