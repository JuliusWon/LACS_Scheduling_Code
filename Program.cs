using System;
using CsvHelper;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace MyApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Processing ps = new Processing();
            if(args.Length > 0){
                if(args[0] == "--devmode"){
                    ps.debugging = true;
                }
            }
            ps.querryUser();
            ps.getStudents();
            ps.Initialize();
            ps.writeToTextFile();
            Console.ReadLine();
        }
    }
    public class student{
        public int favor;
        public string name = "qwfwq";
        public string output;
        public string firstchoice;
        public string secondchoice;
        public string thirdchoice;
        public int choiceGot;
        public int grade;
    }
    public class period
    {
        public int lowBound;
        public int highBound;
        public int popularity;
        public int joined = 0;
        public student[] students;
        public int cap;
        public string name;
        public void Setup(){
            students = new student[cap];
        }
    }
    class studentFromCSV {
        public string First {get; set;}
        public string Last {get;set;}
        public string Primary {get; set;}
        public string Secondary {get; set;}
        public string Final {get; set;}
        public string Grade {get; set;}
    }
    public class periodFromCSV {
        public string Name {get;set;}
        public int Cap {get;set;}
    }
    public class Processing{
        string[] outputToTxt;
        List<string> classNames = new List<string>(1);
        public bool debugging = false;
        period[] schedule;
        int[] classCaps;
        student[] students;
        public void querryUser(){
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Num. Students");
            students = new student[int.Parse(Console.ReadLine())];
            Console.ForegroundColor = ConsoleColor.Blue;
        }
        bool checkForClassAlreadyInList(string className){
            bool output = false;
            foreach(string name in classNames){
                if(name  == className){
                    output =true;
                }
            }
            return output;
        }
        public void getStudents(){
            string outputPath = "output.txt";
            using (var reader = new StreamReader("example4.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<studentFromCSV>();
                int i = 0;
                foreach(var ex in records){
                    students[i] = new student();
                    students[i].name = ex.First+" "+ex.Last;
                    students[i].firstchoice = ex.Primary;
                    students[i].secondchoice = ex.Secondary;
                    students[i].thirdchoice = ex.Final;
                    students[i].grade = int.Parse(ex.Grade);
                    students[i].favor = students[i].grade - 6;
                    i++;
                }
            }
            using (var reader = new StreamReader("example4.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records3 = csv.GetRecords<studentFromCSV>();
                int i = 0;
                foreach(var records2 in records3){
                    if(!checkForClassAlreadyInList(records2.Primary)){
                        classNames.Add(records2.Primary);
                    }
                    if(!checkForClassAlreadyInList(records2.Secondary)){
                        classNames.Add(records2.Secondary);
                    }
                    if(!checkForClassAlreadyInList(records2.Final)){
                        classNames.Add(records2.Final);
                    }
                }
                schedule = new period[classNames.Count()];
                Console.WriteLine(schedule.Length);
                for(int j = 0;j<schedule.Length;j++){
                    schedule[j] = new period();
                    schedule[j].name = classNames[j];
                    //Ask the user the caps for all the classes. Could also be done through a CSV
                    if(schedule[j].name != ""&&schedule[j].name != "Other"){
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Capacity of "+schedule[j].name+":");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        schedule[j].cap = int.Parse(Console.ReadLine());
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Youngest grade accepted for "+schedule[j].name+":");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        schedule[j].lowBound = int.Parse(Console.ReadLine());
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Oldest grade accepted for "+schedule[j].name+":");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        schedule[j].highBound= int.Parse(Console.ReadLine());
                    }else{
                        schedule[j].cap = students.Length;
                    }
                    schedule[j].Setup();
                }
            }
        }

        public bool WithinAgeBounds(period testingPeriod ,int gradeIn)
        {
            return testingPeriod.lowBound <= gradeIn && testingPeriod.highBound >= gradeIn;
        }
        public void Initialize(){
            CalculatePopularity();
        }
        //If this throws an index out of range exception, most likely there is a typo in the CSV
        //this is because the identifyClass function returns an arbitrarily high number when the class isn't recognized.
        void CalculatePopularity(){
            foreach(student s in students){
                schedule[IdentifyClass(s.firstchoice)].popularity+=3;
                schedule[IdentifyClass(s.secondchoice)].popularity+=2;
                schedule[IdentifyClass(s.thirdchoice)].popularity++;
            }
            Process();
        }

        int DetermineFallbackCap(student studentIn)
        {
            period fallback = new period();
            if (studentIn.choiceGot == 1)
            {
                fallback = schedule[IdentifyClass(studentIn.secondchoice)];
            }
            if (studentIn.choiceGot == 2)
            {
                fallback = schedule[IdentifyClass(studentIn.thirdchoice)];
            }
            return fallback.cap;
        }
        student DetermineBump(period p, int favorIn){
            int lowestFavor = favorIn;
            int highestCap = 0;
            student outputs = new student();
            student lowestFavorStudent = new student();
            foreach(student s in p.students){
                if(s.favor < lowestFavor){
                    lowestFavor = s.favor;
                    lowestFavorStudent = s;
                }
            }
            outputs = lowestFavorStudent;
            return outputs;
        }
        public void Process(){
            for(int i = 0;i< students.Length;i++){
                ProcessStudent(students[i]);
            }
            printAllInfo();
        }
        public void ProcessStudent(student studentProc){
            if(studentProc.firstchoice == null || studentProc.name == null || studentProc.secondchoice == null){
                Console.WriteLine("Error");
                return;
            }
            //if the class has room
            if(schedule[IdentifyClass(studentProc.firstchoice)].joined!=schedule[IdentifyClass(studentProc.firstchoice)].cap){
                //runs if class isn't empty
                schedule[IdentifyClass(studentProc.firstchoice)].students[schedule[IdentifyClass(studentProc.firstchoice)].joined] = studentProc;
                studentProc.choiceGot = 1;
                //no increase to favor if first choice
                schedule[IdentifyClass(studentProc.firstchoice)].joined++;
            }else{
                //if the student has lower favor than anyone in the full class
                if(DetermineBump(schedule[IdentifyClass(studentProc.firstchoice)],studentProc.favor).name =="qwfwq")
                {
                    //bump their favor
                    studentProc.favor++;
                    //if the second choice isn't full
                    if(schedule[IdentifyClass(studentProc.secondchoice)].joined!=schedule[IdentifyClass(studentProc.secondchoice)].cap){
                        //runs if class isn't empty
                        schedule[IdentifyClass(studentProc.secondchoice)].students[schedule[IdentifyClass(studentProc.secondchoice)].joined] = studentProc;
                        studentProc.choiceGot = 2;
                        studentProc.favor +=2;
                        schedule[IdentifyClass(studentProc.secondchoice)].joined++;
                    }else{
                        if(DetermineBump(schedule[IdentifyClass(studentProc.secondchoice)],studentProc.favor).name =="qwfwq")
                        {
                            studentProc.favor += 2;
                            if(schedule[IdentifyClass(studentProc.thirdchoice)].joined!=schedule[IdentifyClass(studentProc.thirdchoice)].cap){
                                //runs if class isn't empty
                                schedule[IdentifyClass(studentProc.thirdchoice)].students[schedule[IdentifyClass(studentProc.thirdchoice)].joined] = studentProc;
                                studentProc.choiceGot = 3;
                                studentProc.favor +=10;
                                schedule[IdentifyClass(studentProc.thirdchoice)].joined++;
                            }else
                            {
                                studentProc.favor += 3;
                                if(DetermineBump(schedule[IdentifyClass(studentProc.secondchoice)],studentProc.favor).name ==""){
                                    Console.WriteLine("");
                                    Console.Write("Failed to place ");
                                    Console.Write(studentProc.name);
                                    Console.Write(".");
                                    Console.WriteLine("");
                                }
                            }
                        }else{
                            ProcessStudent(DetermineBump(schedule[IdentifyClass(studentProc.firstchoice)],studentProc.favor));
                        }
                    }
                }else{
                    ProcessStudent(DetermineBump(schedule[IdentifyClass(studentProc.firstchoice)],studentProc.favor));
                }
            }
        }
        public void writeToTextFile(){
            outputToTxt = new string[students.Length];
            for(int k = 0;k<students.Length;k++){
                if(students[k].choiceGot == 1){
                    outputToTxt[k] = students[k].name+": "+students[k].firstchoice;
                }
                if(students[k].choiceGot == 3){
                    outputToTxt[k] = students[k].name+": "+students[k].thirdchoice;
                }
                if(students[k].choiceGot == 2){
                    outputToTxt[k] = students[k].name+": "+students[k].secondchoice;
                }
            }
            using (StreamWriter outputFile = new StreamWriter("output.txt"))
            {
                foreach (string line in outputToTxt)
                    outputFile.WriteLine(line);
            }
        }
        public void printAllInfo(){
            int i=0;
            foreach(period p in schedule){
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("-------------------------------------------------------------------");
                Console.Write(p.name);
                Console.Write(": Students: ");
                //Count the number of students actually in the class
                int counter = 0;
                foreach(student st in p.students){
                    if(st != null){
                        counter++;
                    }
                }
                Console.Write(counter);
                Console.Write("/");
                Console.Write(p.cap);
                Console.WriteLine("");
                foreach(student s in p.students){
                    if(s!= null){
                        i++;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(s.name);
                        Console.Write(": ");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(s.choiceGot);
                        Console.Write(" favor: ");
                        Console.Write(s.favor);
                        Console.Write(" choice: ");
                        Console.Write(s.choiceGot);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" [Grade]: ");
                        Console.WriteLine(s.grade);
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine("For Manual Sorting:");
            foreach(student q in students){
                if(q.choiceGot == 0){
                    Console.Write(q.name);
                    Console.Write(" grade:");
                    Console.WriteLine(q.grade);
                }
            }
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine(i);
            if(debugging){
                Console.WriteLine("-------------------------------------------------------------------");
                foreach(student q in students){
                    if(q.grade == 12){
                        Console.WriteLine("");
                        Console.Write(q.name);
                        Console.Write(":");
                        Console.Write(q.choiceGot);
                    }
                }
            }
        }
        int IdentifyClass(string input){
            int outputInt = 39;
            for(int i = 0;i<schedule.Length;i++)
            {
                if(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(schedule[i].name) == CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input))
                {
                    outputInt = i;
                }
            }
            if(outputInt == 39){
                Console.WriteLine(input);
            }
            return outputInt;
        }
    }
}