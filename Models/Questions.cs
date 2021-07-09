using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models
{
    public class Questions
    {
        public TriviaQuestion[] Results { get; set; }
    }

    public class TriviaQuestion
    {
        public string Category { get; set; }
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public string Question { get; set; }
        public string Correct_answer { get; set; }
        public string[] Incorrect_answers { get; set; }
        public string Selected { get; set; }
        public string [] Choices 
        {
            
            //Randomizes the choices so the answers are always in same spot
            get
            {
                string[] selection;
                if(Type == "boolean")
                {
                    selection = new string[2];
                    selection[0] = "True";
                    selection[1] = "False";
                }
                else
                {
                    selection = new string[4];
                    Array.Copy(Incorrect_answers, selection, Incorrect_answers.Length);
                    selection[3] = Correct_answer;
                    Random rand = new Random();

                    for (int i = 0; i < selection.Length - 1; i++)
                    {
                        int j = rand.Next(i, selection.Length);
                        string temp = selection[i];
                        selection[i] = selection[j];
                        selection[j] = temp;
                    }
                }
                
                return selection;
            }
            set
            {
            }
        }
    }



}



