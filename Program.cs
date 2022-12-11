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
        }
    }
    public class student{
      public int favor;
      public string name;
      public string output;
      public string firstchoice;
      public string secondchoice;
      public string thirdchoice;
      public int choiceGot;
      public int grade;
    }
    public class period{
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
              }else{
                schedule[j].cap = students.Length;
              }
              schedule[j].Setup();
            }
        }
      }
      public void Initialize(){
        foreach(period s in schedule){
          Console.WriteLine(s.name);
        }
        CalculatePopularity();
      }
      void CalculatePopularity(){
        foreach(student s in students){
          schedule[identifyClass(s.firstchoice)].popularity+=3;
          schedule[identifyClass(s.secondchoice)].popularity+=2;
          schedule[identifyClass(s.thirdchoice)].popularity++;
        }
        Process();
      }
      student determineBump(period p, int favorIn){
        int lowestFavor = favorIn;
        student outputs = new student();
        foreach(student s in p.students){
          if(s.favor < lowestFavor){
            lowestFavor = s.favor;
            outputs = s;
          }
        }
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
        if(schedule[identifyClass(studentProc.firstchoice)].joined!=schedule[identifyClass(studentProc.firstchoice)].cap){
            //runs if class isn't empty
            schedule[identifyClass(studentProc.firstchoice)].students[schedule[identifyClass(studentProc.firstchoice)].joined] = studentProc;
            studentProc.choiceGot = 1;
            //no increase to favor if first choice
            schedule[identifyClass(studentProc.firstchoice)].joined++;
        }else{
          if(determineBump(schedule[identifyClass(studentProc.firstchoice)],studentProc.favor).name ==null){
            if(schedule[identifyClass(studentProc.secondchoice)].joined!=schedule[identifyClass(studentProc.secondchoice)].cap){
                //runs if class isn't empty
                schedule[identifyClass(studentProc.secondchoice)].students[schedule[identifyClass(studentProc.secondchoice)].joined] = studentProc;
                studentProc.choiceGot = 2;
                studentProc.favor +=2;
                schedule[identifyClass(studentProc.secondchoice)].joined++;
              }else{
                if(determineBump(schedule[identifyClass(studentProc.secondchoice)],studentProc.favor).name ==null){
                  if(schedule[identifyClass(studentProc.thirdchoice)].joined!=schedule[identifyClass(studentProc.thirdchoice)].cap){
                      //runs if class isn't empty
                      schedule[identifyClass(studentProc.thirdchoice)].students[schedule[identifyClass(studentProc.thirdchoice)].joined] = studentProc;
                      studentProc.choiceGot = 3;
                      studentProc.favor +=10;
                      schedule[identifyClass(studentProc.thirdchoice)].joined++;
                  }else{
                    if(determineBump(schedule[identifyClass(studentProc.secondchoice)],studentProc.favor).name ==null){
                        Console.WriteLine("");
                        Console.Write("Failed to place ");
                        Console.Write(studentProc.name);
                        Console.Write(".");
                        Console.WriteLine("");
                    }
                  }
                }else{
                  ProcessStudent(determineBump(schedule[identifyClass(studentProc.firstchoice)],studentProc.favor));
                }
            }
          }else{
            ProcessStudent(determineBump(schedule[identifyClass(studentProc.firstchoice)],studentProc.favor));
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
            Console.WriteLine(q.name);
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
    int identifyClass(string input){
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
