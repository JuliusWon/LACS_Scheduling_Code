using System;

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
          ps.Initialize();
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
    public class Processing{
      public bool debugging = false;
      period[] schedule;
      string[] classNames = new string[]{"Eng","Math","Sci","Soc","Art","Music","PE"};
      int[] classCaps = new int[]{6,5,6,3,9,9,8};
      student[] students = new student[39];
      int[] grades =  new int[] {12,10,10,11,11,9,9,10,12,12,9,12,9,10,11,9,9,11,11,9,12,10,11,10,9,12,10,9,12,12,11,10,12,9,10,10,9,12,10};
      string[] firstchoices = new string[] {"Math","PE","Art","Math","Music","Eng","Music","PE","Eng","PE","Math","Art","Soc","Music","Sci","Soc","Sci","PE","Art","Math","Soc","Sci","Art","Math","Music","Eng","PE","Soc","Eng","Soc","Math","Art","Music","PE","PE","Soc","Art","Sci","Soc"};
      string[] secondchoices = new string[] {"Soc","Eng","Math","PE","Sci","Music","Soc","Math","Soc","Eng","Soc","Math","PE","Art","Eng","Art","PE","Sci","Music","Sci","Math","Math","Music","Soc","PE","Art","Eng","Math","Soc","Math","Soc","Music","Math","Soc","Sci","PE","Music","PE","Art"};
      string[] thirdchoices = new string[] {"Eng","Music","PE","Eng","Math","Soc","PE","Eng","Art","Math","Art","Soc","Music","Math","Math","Eng","Eng","Soc","Soc","Art","Music","PE","Eng","PE","Sci","Music","Math","PE","Sci","Music","Art","Math","Sci","Music","PE","Art","Math","Art","Music"};
      string[] names = new string[] {"Julius","Pedro","Leo","Ben","Fredrick","Emilia","Sam","Aella","Garik","Sword","Student","Child","William","Tim","Ari","Kira","Matt","Vahni","Peter","Rad","Tom","Timy","Bob","Tod","Timothy","Rat","Tommethy","Zimmethy","Jame","Jimothy","Zormmmethy","Willithe","Zore","Blorpy","Zlorpy","Zeff","Zall","Grazz","Grass Eater"};
      public void Initialize(){
        schedule = new period[7];
        for(int i=0;i<schedule.Length;i++){
          schedule[i] = new period();
          schedule[i].cap = classCaps[i];
          schedule[i].name = classNames[i];
          schedule[i].Setup();
        }
        for(int i=0;i<students.Length;i++){
          students[i] = new student();
          students[i].name = names[i];
          students[i].grade = grades[i];
          students[i].favor = grades[i];
          if(students[i].name == "Julius"){
            students[i].favor = 100;
          }
          students[i].firstchoice = firstchoices[i];
          students[i].secondchoice = secondchoices[i];
          students[i].thirdchoice = thirdchoices[i];
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
      public void bump(){

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
      public void printAllInfo(){
        int i=0;
        foreach(period p in schedule){
          Console.WriteLine("-------------------------------------------------------------------");
          Console.Write(p.name);
          Console.Write(": popularity: ");
          Console.Write(p.popularity);
          Console.WriteLine("");
          foreach(student s in p.students){
            if(s!= null){
              i++;
              Console.Write(s.name);
              Console.Write(": ");
              Console.Write(s.choiceGot);
              Console.Write(" [Grade]: ");
              Console.WriteLine(s.grade);
            }
          }
        }
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
        int outputInt = 42;
        for(int i = 0;i<classNames.Length;i++){
          if(classNames[i] == input){
            outputInt = i;
          }
        }
        return outputInt;
      }
    }
}
