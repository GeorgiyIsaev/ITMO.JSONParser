﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;


namespace CheckList
{
	
	public class Answer
	{
		
		string answer;
		bool ifTrue;		
		
		public bool if_true { get => ifTrue; set => if_true = value; } // 1-Верный ответ, 0-Не верный ответ.
        public string answerSTR { get => answer; set => answer = value; }
        public int random_nomer { get; set; } = 0;

		public Answer()
		{
			//пустой констроктор для JSON
		}
		public Answer(string str, bool if_answer)
		{
			if_true = if_answer;
			answerSTR = str;
		}

		public void RandomAnswerIt()
		{ //для перетосовки ответов
			Random rnd = new Random();
			random_nomer = rnd.Next(0, 100);
		}
		public override string ToString()
		{
			answerSTR = answerSTR.Replace("\n", "");
			answerSTR = answerSTR.Replace("\r", "");
			return answerSTR;
		}
	};
	
	
	
	public class QuestItem
	{
		/*Для контакта с листбоксом*/
		private string _description = "Добавить новый вопрос!";
		public string tName
		{
			get { return quest; }
			set { quest = value; NotifyPropertyChanged("tName"); }
		}
		public string Description
		{
			get { return _description; }
			set { _description = value; NotifyPropertyChanged("Description"); }
		} 
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}
        public QuestItem()
        {
			//пустой констрокутор для работы JSONсериализации
		}

		/*Части вопроса*/
		public string quest { get; set; } = "";
		public string comment { get; set; } = "";
		public List<Answer> answerItem = new List<Answer>();
		public int intRandomQuest { get; set; } = 0;
		public int countTrueAnswer{ get; set; } = 0;

		/*Логика работы вопроса*/
		/*Добавление верные и не верных ответов в лист*/
		public void InputAnswerList(string answer, string anAnswer)
        {		
			String[] answerMas = answer.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			String[] anAnswerMas = anAnswer.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (string tmp in answerMas)
            {
				Answer temp = new Answer(tmp, true);
				answerItem.Add(temp);			
			}	
			foreach (string tmp in anAnswerMas)
			{
				Answer temp = new Answer(tmp, false);
				answerItem.Add(temp);
			}
			EndlForSpase();
		}
		public override string ToString()
		{
			string temp = quest;
			if (answerItem.Count != 0) temp += $" -- > ОТВЕТЫ: {answerItem.Count} шт.";
			//return quest;
			return temp;
		}
		public string ToolTypeListBox()
		{
			string temp = quest;
			if (answerItem.Count != 0)
			{
				temp += $"\nOТВЕТЫ: {answerItem.Count} шт.\n";
				int count = 1;
				foreach (Answer answer in answerItem)
				{
					temp += (count++) + ". ";
					temp += answer.if_true ? "Верный: " : "Не верный: ";
					temp += answer.answerSTR +"\n";
				}
			}
			else { temp = "Добавить новый вопрос!"; }
			return temp;
		}





		public void EndlForSpase()
        {	
			while (true){
				quest = quest.Replace("\r", " ");
				quest = quest.Replace("\n", " "); 			
				if (!quest.Contains("\n")) break;
			}
			while (true){
				comment = comment.Replace("\r", " ");
				comment = comment.Replace("\n", " ");
				if (!comment.Contains("\n")) break;
			}
		}
		public string StrFullAnswer(bool if_answer = true)
        {
			StringBuilder tempSTR = new StringBuilder();
			foreach(Answer answer in answerItem)
            {
				if (answer.if_true == if_answer)
                {
					if(tempSTR.Length>=1) tempSTR.Append("\n");
					tempSTR.Append(answer.answerSTR);		
				}
			}
			return tempSTR.ToString();
        }

		public void RandomGeneratorIt()
        {
			Random rnd = new Random();
			intRandomQuest = rnd.Next(0, 100);
			foreach (Answer tmpAnswer in answerItem)
			{
				if(tmpAnswer.if_true) countTrueAnswer++;
				tmpAnswer.RandomAnswerIt();
			}
			/*Перетасовать ответы*/
			answerItem.Sort((a,b)=> a.random_nomer.CompareTo(b.random_nomer));			
		}
	}    
}

