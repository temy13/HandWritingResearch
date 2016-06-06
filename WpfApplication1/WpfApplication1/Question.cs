using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace WpfApplication1
{
    [DataContract]
    public class Question
    {
        [DataMember]
        public string question;
        [DataMember]
        public string answer;
        [DataMember]
        public int questionNumber;
        [DataMember]
        public int solveType;
        /* 
         * 0:未回答
         * 1:word miss
         * 2:grammer miss
         * 3: double miss
         * 4:correct

        */

        public Question(string question, string answer, int questionNumber)
        {
            this.question = question;
            this.answer = answer;
            this.questionNumber = questionNumber;
            solveType = 0;
        }
    }
}
