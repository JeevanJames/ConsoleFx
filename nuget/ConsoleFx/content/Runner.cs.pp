using System;
using System.Collections.Generic;

using ConsoleFx;
using ConsoleFx.Programs.Simple;
using ConsoleFx.Validators;

namespace $rootnamespace$
{
   internal static class Runner
   {
       //TODO: Call this method from your Main method.
       //Optional, but recommended: Change your Main method to return int, and return the value from this method.
       internal static int Run()
       {
           var app = new ConsoleProgram(Handler, );
           try
           {
               //TODO: Specify the options here using app.AddOption
               //TODO: Specify the arguments here using app.AddArgument
               return app.Run();
           }
           catch (Exception ex)
           {
               return app.HandleError(ex);
           }
       }
       
       private static int Handler()
       {
           //TODO: Write your logic here
           throw new NotImplementedException();
       }
   }
}