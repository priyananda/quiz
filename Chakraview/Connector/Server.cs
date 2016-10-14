﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Json;

namespace Shenoy.Quiz.Backend
{
    class Server
    {
        public void Push(List<TeamInfo> teams)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var team in teams)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();
                team.SerializeTo(properties);
                BuildData(builder, "team", properties);
                builder.Append("&");
            }
            ExecutePush(builder.ToString());
        }

        private void BuildData(StringBuilder builder, string paramName, Dictionary<string, object> properties)
        {
            string jsonData = JsonParser.ToJson(properties);
            builder.Append(paramName);
            builder.Append("=");
            builder.Append(WebUtility.UrlEncode(jsonData));
        }

        public List<TeamInfo> Pull()
        {
            if (UseFakeData)
                return CreateFakeData();

            string teamsAsJson = ExecutePull();
            var dic = JsonParser.FromJson(teamsAsJson);
            List<object> teamsArray = dic["teams"] as List<object>;
            List<TeamInfo> teams = new List<TeamInfo>();

            foreach (object teamPropBagObj in teamsArray)
            {
                Dictionary<string, object> properties = teamPropBagObj as Dictionary<string, object>;
                TeamInfo teaminfo = new TeamInfo();
                teaminfo.Deserialize(properties);
                teams.Add(teaminfo);
            }
            
            return teams;
        }

        private List<TeamInfo> CreateFakeData()
        {
            List<TeamInfo> teams = new List<TeamInfo>();
            Random rand = new Random();
            for (int i = 0; i < 40; ++i)
            {
                TeamInfo team = new TeamInfo();
                team.TeamId = rand.Next(100, 999);
                team.IsFinalist = (i < 6);
                teams.Add(team);
            }
            return teams;
        }

        private void ExecutePush(string dataAsString)
        {
            byte[] data = Encoding.ASCII.GetBytes(dataAsString);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                UseTestServer ? PUSH_CONNECTION_URL_TEST : PUSH_CONNECTION_URL_PROD);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            var requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            using (var myHttpWebResponse = (HttpWebResponse)request.GetResponse())
            {
                var status = myHttpWebResponse.StatusCode;
            }
        }

        private string ExecutePull()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                UseTestServer ? PULL_CONNECTION_URL_TEST : PULL_CONNECTION_URL_PROD);
            request.Method = "GET";

            using (var myHttpWebResponse = (HttpWebResponse)request.GetResponse())
            {
                using (var responseStream = myHttpWebResponse.GetResponseStream())
                {
                    using (var myStreamReader = new StreamReader(responseStream, Encoding.Default))
                    {
                        return myStreamReader.ReadToEnd();
                    }
                }
            }
        }

        private bool UseTestServer = true;
        private bool UseFakeData = true;
        private const string PUSH_CONNECTION_URL_TEST = "http://localhost:8888/prelimsset";
        private const string PUSH_CONNECTION_URL_PROD = "http://quizpl.us/prelimsset";
        private const string PULL_CONNECTION_URL_TEST = "http://localhost:8888/prelimsget";
        private const string PULL_CONNECTION_URL_PROD = "http://quizpl.us/prelimsget";
    }
}
