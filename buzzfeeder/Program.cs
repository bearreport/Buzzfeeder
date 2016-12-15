using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
namespace ConsoleApplication4
{
    public class maketests
    {
        public void createuser(SqlConnection connection)
        {
            Console.WriteLine("Enter your username");
            string username = Console.ReadLine();
            Console.WriteLine("Enter your password");
            string password = Console.ReadLine();
            connection.Open();
            SqlCommand createuser = new SqlCommand("INSERT INTO Users (Username, Userpassword) VALUES (@username, @password)", connection);
            createuser.Parameters.AddWithValue("@username", username);
            createuser.Parameters.AddWithValue("@password", password);
            createuser.ExecuteNonQuery();
            connection.Close();
        }
        public int loginuser(SqlConnection connection)
        {
            connection.Close();
            int UserId = 0;
            bool login = false;
            while (login == false)
            {
                Console.WriteLine("Enter your username");
                string username = Console.ReadLine();
                Console.WriteLine("Enter your password");
                string password = Console.ReadLine();
                connection.Open();
                SqlCommand checkuser = new SqlCommand("SELECT CAST(UserId AS Int) FROM Users WHERE Username =@username AND Userpassword =@password", connection);
                checkuser.Parameters.AddWithValue("@username", username);
                checkuser.Parameters.AddWithValue("@password", password);
                SqlDataReader datareader = checkuser.ExecuteReader();

                if (datareader.HasRows)
                {
                    while (datareader.Read())
                    {
                        UserId = datareader.GetInt32(0);
                    }
                    login = true;
                }
                else
                {
                    Console.WriteLine("Try again");
                }
                connection.Close();
            }
            return UserId;
        }
        public int getTest(SqlConnection connection, int UserID)
        {
            connection.Close();
            Console.WriteLine("What is the title of your quiz? ");
            string title = Console.ReadLine();
            connection.Open();
            SqlCommand SetTitle = new SqlCommand("INSERT INTO Tests (Title, UserID) VALUES (@title, @UserID)", connection);
            SetTitle.Parameters.AddWithValue("@UserID", UserID);
            SetTitle.Parameters.AddWithValue("@title", title);
            SetTitle.ExecuteNonQuery();
            connection.Close();
            connection.Open();
            SqlCommand TestID = new SqlCommand("SELECT CAST(MAX(TestId) AS int) from Tests", connection);
            SqlDataReader datareader = TestID.ExecuteReader();
            int IDTest = 0;
            if (datareader.HasRows)
            {
                while (datareader.Read())
                {
                    IDTest = datareader.GetInt32(0);
                }
            }
            return IDTest;
            connection.Close();
        }

        public int getQuestion(SqlConnection connection, int IDTest, int questionnumber)
        {
            connection.Close();
            connection.Open();
            Console.WriteLine("Please enter your question");
            string Question = Console.ReadLine();

            SqlCommand SendQuestion = new SqlCommand("INSERT INTO Questions (TestId, Question, SortOrder) VALUES (@TestID, @Question, @questionnumber)", connection);
            SendQuestion.Parameters.AddWithValue("@TestID", IDTest);
            SendQuestion.Parameters.AddWithValue("@Question", Question);
            SendQuestion.Parameters.AddWithValue("@questionnumber", questionnumber);
            SendQuestion.ExecuteNonQuery();
            connection.Close();
            connection.Open();
            SqlCommand QuestionID = new SqlCommand("SELECT CAST(MAX(QuestionId) AS int) from Questions", connection);
            SqlDataReader QuestionReader = QuestionID.ExecuteReader();
            int IDQuestion = 0;
            if (QuestionReader.HasRows)
            {
                while (QuestionReader.Read())
                {
                    IDQuestion = QuestionReader.GetInt32(0);
                }
            }
            connection.Close();
            return IDQuestion;
        }

        public void getAnswer(SqlConnection connection, int IDQuestion)
        {
            Console.WriteLine("Enter your answer");
            string Answer = Console.ReadLine();
            Console.WriteLine("How many points does this answer score?");
            int value = Convert.ToInt32(Console.ReadLine());
            connection.Open();
            SqlCommand SendAnswer = new SqlCommand("INSERT INTO Answers(QuestionId, Answer, Value) VALUES (@QuestionID, @Answer, @value)", connection);
            SendAnswer.Parameters.AddWithValue("@QuestionID", IDQuestion);
            SendAnswer.Parameters.AddWithValue("@Answer", Answer);
            SendAnswer.Parameters.AddWithValue("@value", value);
            SendAnswer.ExecuteNonQuery();
            connection.Close();
        }

        public void getResult(SqlConnection connection, int IDTest)
        {
            Console.WriteLine("What is the title of your result");
            string ResultTitle = Console.ReadLine();
            Console.WriteLine("What is the description of your result");
            string ResultDescription = Console.ReadLine();
            Console.WriteLine("What is the score value for this result");
            int score = Convert.ToInt32(Console.ReadLine());
            connection.Open();
            SqlCommand ResultAnswer = new SqlCommand("INSERT INTO Results(Title, Text, TestId, Value) VALUES (@ResultTitle, @ResultDescription, @IDTest, @value)", connection);
            ResultAnswer.Parameters.AddWithValue("@ResultTitle", ResultTitle);
            ResultAnswer.Parameters.AddWithValue("@ResultDescription", ResultDescription);
            ResultAnswer.Parameters.AddWithValue("@IdTest", IDTest);
            ResultAnswer.Parameters.AddWithValue("@value", score);
            ResultAnswer.ExecuteNonQuery();
            connection.Close();
        }
        
    }
            
    
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=10.1.10.148;Initial Catalog=Buzzfeed-session2; User ID=(username);Password=(password)");
            maketests testmaker = new maketests();


            
            int testid=testmaker.getTest(connection, 1);

                       
            bool setquestions = true;
            bool setanswers = true;
            int questionsort = 1;

            while (setquestions == true)
            {
                int questionid = testmaker.getQuestion(connection, testid, questionsort);

                    while (setanswers ==true)
                {
                    testmaker.getAnswer(connection, questionid);
                    Console.WriteLine("Do you want to enter another answer? y or n");
                    string answerchoice = Console.ReadLine();
                    answerchoice = answerchoice.ToLower();

                    if (answerchoice == "n")
                    {
                        setanswers = false;
                    }
                
                }

                Console.WriteLine("Do you want to enter another question?");

                string questionchoice = Console.ReadLine();
                questionchoice = questionchoice.ToLower();

                if (questionchoice == "n")
                {
                    setquestions = false;
                }
                questionid = questionid + 1;
                setanswers = true;
            }

            bool setresults = true;
            int resultsort = 1;

            while (setresults == true)
            {
                testmaker.getResult(connection, testid);
                Console.WriteLine("Do you want to submit another result?");

                string resultchoice = Console.ReadLine();
                resultchoice = resultchoice.ToLower();
                if (resultchoice == "n")
                {
                    setresults = false;
                }
                resultsort++;
            }

            Console.ReadLine();

        }
    }
}