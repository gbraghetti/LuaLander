using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEditor;
using UnityEngine;
using System;


namespace CodeMonkey.CSharpCourse.Companion {

    [InitializeOnLoad]
    public class CodeMonkeyCompanion {


        public static event EventHandler OnCompilationFinished;
        public static event EventHandler OnAfterAssemblyReloaded;
        public static event EventHandler OnCompilerMessage;

        public enum MessageType {
            Info,
            Warning,
            Error,
        }

        [Serializable]
        public class OnCompanionMessageEventArgs : EventArgs {
            public MessageType messageType;
            public string message;
        }

        public static event EventHandler<OnCompanionMessageEventArgs> OnCompanionMessage;


        public static List<CompilerMessage> errorCompilerMessageList = new List<CompilerMessage>();


        static CodeMonkeyCompanion() {
            CompilationPipeline.assemblyCompilationFinished -= AssemblyCompilationFinished;
            CompilationPipeline.assemblyCompilationFinished += AssemblyCompilationFinished;
            AssemblyReloadEvents.afterAssemblyReload -= AfterAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += AfterAssemblyReload;
            Application.logMessageReceived -= Application_logMessageReceived;
            Application.logMessageReceived += Application_logMessageReceived;
        }

        private static void Application_logMessageReceived(string condition, string stackTrace, LogType type) {
            if (condition.Contains("InvalidOperationException: Collection was modified; enumeration operation may not execute.")) {
                SendCompanionMessage(MessageType.Error, "InvalidOperationException\n" +
                    "You cannot modify a collection while you are cycling through it in a foreach\r\n" +
                    "If you want to do that, you can create a copy of the collection and cycle through the copy while modifying the original.\r\n" +
                    "Or do a for loop instead. Although remember that if you are removing/adding elements to the collection then it might impact the for iterator.\r\n" +
                    "For example if you remove an element you need to do i--; before the end of the loop so you don't skip an element.\r\n");
            }

            if (condition.Contains("NullReferenceException")) {
                SendCompanionMessage(MessageType.Error, "NullReferenceException\n" +
                    "You tried to access something on a null object.\r\n" +
                    "Here's my 4 step process for solving NullReferenceException\r\n" +
                    "1. Go to the offending line\r\n" +
                    "2. Analyze all objects that could be null\r\n" +
                    "3. Identify the null object with Debug.Log();\r\n" +
                    "4. Decide what to do with that object\r\n" +
                    "I made a video on this topic:\n##REF##video_small, 5irv30-bTJw, How to Fix NullReferenceException in C#!###REF##\r\n\r\n");
            }

            if (condition.Contains("InvalidCastException")) {
                SendCompanionMessage(MessageType.Error,
                    "InvalidCastException\n" +
                    "There is no valid way to cast from this type onto the target type. Are you sure you're using the types you want to use?\r\n" +
                    "For example you cannot directly cast a string onto an int, if you want to do that then use int.Parse();\r\n" +
                    "Or maybe the variable that you're trying to cast is of a different type than you expect, add a Debug.Log to verify what type it is.\r\n" +
                    "If you are sure of the types then you need to define on your custom type how it should be cast onto the target type.\r\n");
            }

            if (condition.Contains("DivideByZeroException")) {
                SendCompanionMessage(MessageType.Error,
                    "You cannot divide by zero, this is mathematically impossible.\r\n" +
                    "If you're working with a variable then compare it against 0 before doing any division.\r\n");
            }

            if (condition.Contains("MissingReferenceException: The object of type") && condition.Contains("has been destroyed but you are still trying to access it.")) {
                SendCompanionMessage(MessageType.Error,
                    "The component you're trying to access has since been destroyed.\r\n" +
                    "Is the component supposed to have been destroyed? Maybe the whole game object was destroyed?" +
                    "Maybe you have a dangling event that is still active? If so remember to unsubscribe when that object is destroyed.\r\n" +
                    "Add some Debug.Log to see the sequence of events that led to it being destroyed and then called.\r\n");
            }



            if (condition.Contains("asdf123456")) {
                SendCompanionMessage(MessageType.Error, 
                    "AAAAAAAAA");
            }
        }

        private static void AfterAssemblyReload() {
            OnAfterAssemblyReloaded?.Invoke(null, EventArgs.Empty);
        }

        private static void AssemblyCompilationFinished(string s, CompilerMessage[] compilerMessageArray) {
            CodeMonkeyCompanionSO.ClearLastCompanionMessageEventArgs();

            errorCompilerMessageList.Clear();

            foreach (CompilerMessage compilerMessage in compilerMessageArray) {
                if (compilerMessage.type == CompilerMessageType.Error) {
                    errorCompilerMessageList.Add(compilerMessage);
                }
            }

            OnCompilationFinished?.Invoke(null, EventArgs.Empty);

            foreach (CompilerMessage compilerMessage in compilerMessageArray) {
                OnCompilerMessage?.Invoke(compilerMessage, EventArgs.Empty);
                if (OnCompilerMessage == null || OnCompilerMessage.GetInvocationList().Length <= 0) {
                    // No listeners, handle it by default
                    HandleCompilerMessage(compilerMessage);
                }
            }
        }

        public static bool HasErrors() {
            return errorCompilerMessageList.Count > 0;
        }

        public static void HandleCompilerMessage(CompilerMessage compilerMessage) {
            if (compilerMessage.message.Contains("CS0246")) {
                // Type or namespace not found
                SendCompanionMessage(MessageType.Error, "Type not found, did you write the Type name perfectly?\nRemember how code is <b>case sensitive!</b>\nOr maybe you forgot a 'using Namespace;' statement.");

                if (compilerMessage.message.Contains("Int") || compilerMessage.message.Contains("INT")) {
                    SendCompanionMessage(MessageType.Error, "Did you accidentally write the type 'Int' or 'INT' instead of 'int'?\nRemember how code is <b>case sensitive!</b>");
                }
                if (compilerMessage.message.Contains("Float") || compilerMessage.message.Contains("FLOAT")) {
                    SendCompanionMessage(MessageType.Error, "Did you accidentally write the type 'FLOAT' or 'FLOAT' instead of 'float'?\nRemember how code is <b>case sensitive!</b>");
                }
                if (compilerMessage.message.Contains("Bool") || compilerMessage.message.Contains("BOOL")) {
                    SendCompanionMessage(MessageType.Error, "Did you accidentally write the type 'Bool' or 'BOOL' instead of 'bool'?\nRemember how code is <b>case sensitive!</b>");
                }
            }

            if (compilerMessage.message.Contains("CS1002")) {
                // ; expected
                SendCompanionMessage(MessageType.Error, 
                    "It seems you forgot a <b>;</b> somewhere\nAlways remember to terminate instructions with a semi-colon ;\n\n" +
                    "Or you're trying to write some text but wrote outside a comment //\n" +
                    "Or outside the string \" quotation marks \" ");
            }

            if (compilerMessage.message.Contains("CS0103")) {
                // The name 'playerSpeed' does not exist in the current context
                SendCompanionMessage(MessageType.Error, 
                    "It seems you are trying to access/use a variable that does not exist.\n" +
                    "Did you write the name perfectly exact? <b>Remember how code is case sensitive.</b>\n" +
                    "Or maybe the variable you want exists inside an object? If so then you want myObject.myVariable instead of just myVariable" +
                    "Or maybe it's simply not accessible from that scope? Check the Scope lecture.\n" +
                    "If you define a local variable in a function it only exists in that function, not in other functions.");

                if (compilerMessage.message.Contains("True") || compilerMessage.message.Contains("TRUE") || compilerMessage.message.Contains("truE")) {
                    SendCompanionMessage(MessageType.Error,
                        "Are you trying to write a boolean value? If so, it's 'true' not 'True' or 'TRUE'.");
                }
                if (compilerMessage.message.Contains("False") || compilerMessage.message.Contains("FALSE") || compilerMessage.message.Contains("falsE")) {
                    SendCompanionMessage(MessageType.Error,
                        "Are you trying to write a boolean value? If so, it's 'false' not 'False' or 'FALSE'.");
                }
            }

            if (compilerMessage.message.Contains("CS1061")) {
                // 'ExerciseSceneTester' does not contain a definition for 'SetPlayerSpee' and no accessible extension method 'SetPlayerSpee'
                // accepting a first argument of type 'ExerciseSceneTester' could be found
                // (are you missing a using directive or an assembly reference?)
                SendCompanionMessage(MessageType.Error, 
                    "You are trying to call a function that does not exist!\n" +
                    "Did you write the function name perfectly?\n" +
                    "Did you call it on the correct type? Does that type/class/object have a function of that name?\n" +
                    "Or maybe it's the argument type that does not match what the function expects?");
            }

            if (compilerMessage.message.Contains("CS0266") || compilerMessage.message.Contains("Cannot implicitly convert type")) {
                // Cannot implicitly convert type 'double' to 'int'. An explicit conversion exists (are you missing a cast?)
                SendCompanionMessage(MessageType.Error, 
                    "It seems you are trying to implicitly convert two types for which there is no implicit conversion.\n" +
                    "Check the Lecture on Data Types.\n" +
                    "If you want to forcefully convert a type you can force a cast like:\n" +
                    "int myInt = (int)myDoubleVariable;");
            }

            if (compilerMessage.message.Contains("CS1503")) {
                // Argument 1: cannot convert from 'double' to 'float'
                SendCompanionMessage(MessageType.Error, 
                    "You are calling a function with an argument of an incorrect type.\n" +
                    "Double check what types the function expects for each parameter.\n" +
                    "Remember how some types cannot be automatically converted onto other types, maybe you need a cast.\n" +
                    "Check the Data Types lecture.");
            }

            if (compilerMessage.message.Contains("CS0165")) {
                // Use of unassigned local variable 'age'
                SendCompanionMessage(MessageType.Error, 
                    "Seems you are trying to use a variable before assigning some value to it.\n" +
                    "Make sure you assign some value before using it, like <b>int age = 35;</b> instead of just <b>int age;</b>\n" +
                    "Or if you're assigning a value inside an 'if', make sure it also has a value if the code does not go inside the 'if'.");
            }

            if (compilerMessage.message.Contains("CS0163")) {
                // Control cannot fall through from one case label ('case "Code Monkey":') to another
                SendCompanionMessage(MessageType.Error, 
                    "Remember that every case needs to have a break!\n" +
                    "Or you can have multiple cases run the same logic, but they cannot have any logic inside their cases without a break;" +
                    "Watch the lecture on Switch");
            }

            if (compilerMessage.message.Contains("CS0019")) {
                // Operator '+' cannot be applied to operands of type 'MainWindow.Player' and 'MainWindow.Player'
                SendCompanionMessage(MessageType.Error, 
                    "It seems you are trying to do some math operation or equality operation on 2 types that don't work.\r\n" +
                    "If you are using built in types like string and int, remember that you cannot do \"string\" == int, or float * \"string\".\r\n" +
                    "If you are using a custom type, you need to override the operators to be able to do + - * / or equality ==\r\n");
            }

            if (compilerMessage.message.Contains("CS0021")) {
                // Cannot apply indexing to type
                SendCompanionMessage(MessageType.Error, 
                    "Are you trying to access a custom type on an index with [] ?\r\n" +
                    "If so you need to explicitly define how that works https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/indexers/\r\n");
            }

            if (compilerMessage.message.Contains("CS0023")) {
                // Operator ‘operator’ cannot be applied to operand of type ‘type’
                SendCompanionMessage(MessageType.Error, 
                    "Are you trying to do some math on custom types?\r\n" +
                    "If so then you need to override the operators in your custom class to be able to do + - * / or equality ==\r\n");
            }

            if (compilerMessage.message.Contains("CS0026")) {
                // Keyword this cannot be used in a static method
                SendCompanionMessage(MessageType.Error, 
                    "While working inside a static method you cannot use 'this' because while in a static method there is no object instance for 'this' to refer to.\r\n" +
                    "Check out the lecture on Static\r\n");
            }

            if (compilerMessage.message.Contains("CS0029")) {
                // Cannot implicitly convert type ‘type’ to ‘type’
                SendCompanionMessage(MessageType.Error, 
                    "You cannot implicitly convert between these two types. \r\n" +
                    "Maybe you're missing a cast?\r\n" +
                    "Are you sure what types you're working with? Use Debug.Log to verify\r\n");
            }

            if (compilerMessage.message.Contains("CS0050") ||
                compilerMessage.message.Contains("CS0051") ||
                compilerMessage.message.Contains("CS0052") ||
                compilerMessage.message.Contains("CS0060") ||
                compilerMessage.message.Contains("CS0061")) {
                // Inconsistent accessibility: return type ‘type’ is less accessible than method ‘method’
                SendCompanionMessage(MessageType.Error, 
                    "You have inconsistent accessibility. \r\n" +
                    "For example maybe you have a custom type that is marked as private but you're using that type in a function that is marked as public. You cannot do that because other classes that can call that function cannot use that type.\r\n" +
                    "If you make a function/property/field public then the type needs to also be public\r\n");
            }

            if (compilerMessage.message.Contains("CS0100")) {
                // The parameter name ‘parameter name’ is a duplicate
                SendCompanionMessage(MessageType.Error, "You cannot define the exact same name for multiple parameters, even if they are different types, always use different names for each parameter.");
            }

            if (compilerMessage.message.Contains("CS0101")) {
                // The namespace ‘namespace’ already contains a definition for ‘type’
                SendCompanionMessage(MessageType.Error, 
                    "The namespace already contains a type with that exact same name.\r\n" +
                    "Either change the type name or place it in a different namespace or sub namespace.\r\n");
            }

            if (compilerMessage.message.Contains("CS0102")) {
                // The type ‘type name’ already contains a definition for ‘identifier’
                SendCompanionMessage(MessageType.Error, 
                    "It seems you declared a variable of the exact same name more than once. Variable names must be unique within the same context.\r\n" +
                    "Watch the lecture on Scope.\r\n");
            }

            if (compilerMessage.message.Contains("CS0111")) {
                // Type ‘class’ already defines a member called ‘member’ with the same parameter types
                SendCompanionMessage(MessageType.Error, 
                    "It seems you declared multiple functions with the exact same name and exact same parameter types.\r\n" +
                    "You can have functions with the exact same name but they need to have a different number of parameters or different type of parameters. The name of the parameters does not matter, only the type.\r\n");
            }

            if (compilerMessage.message.Contains("CS0117")) {
                // ‘type’ does not contain a definition for ‘identifier’
                SendCompanionMessage(MessageType.Error, 
                    "You are accessing something that does not exist. Maybe calling a function on an object that doesn't have that function, or accessing a field that object does not have.\r\n" +
                    "Are you sure you're using the type you think you're using? Use Debug.Log to verify.\r\n");
            }

            if (compilerMessage.message.Contains("CS0120")) {
                // An object reference is required for the non-static field, method, or property ‘member’
                SendCompanionMessage(MessageType.Error, 
                    "It seems you're trying to access something through the class name itself rather than an object but the element you're accessing is not static.\r\n" +
                    "You need to use an instance of that object in order to access that field/function.\r\n" +
                    "Or maybe you intended to make it static but forgot the keyword.\r\n" +
                    "Watch the lecture on Classes and the one on Static\r\n");
            }

            if (compilerMessage.message.Contains("CS0176")) {
                // Static member ‘member’ cannot be accessed with an instance reference; qualify it with a type name instead
                SendCompanionMessage(MessageType.Error, "It seems you're trying to access something that is static through an object instance instead of the class itself.\r\n" +
                    "If it's static you need to access it through the class name. Or maybe you intended to make it non-static but accidentally wrote static.\r\n" +
                    "Watch the lecture on Classes and the one on Static\r\n");
            }

            if (compilerMessage.message.Contains("CS0122")) {
                // ‘member’ is inaccessible due to its protection level
                SendCompanionMessage(MessageType.Error, 
                    "You cannot access this member from this context. \r\n" +
                    "Maybe it's marked as private but you are trying to access it from another class, if so it needs to be public.\r\n");
            }

            if (compilerMessage.message.Contains("CS0127")) {
                // Since ‘function’ returns void, a return keyword must not be followed by an object expression
                SendCompanionMessage(MessageType.Error,
                    "It seems you're trying to return some value from a function but the function returns void.\r\n" +
                    "If you want it to return void then it cannot return any data, it can only be terminated with return;\r\n" +
                    "Or if you do want to return some data then swap the function return type.\r\n");
            }

            if (compilerMessage.message.Contains("CS0128")) {
                // A local variable named ‘variable’ is already defined in this scope
                SendCompanionMessage(MessageType.Error,
                    "A local variable of the exact same name is already defined within this scope. Variable names in the same context need to have unique names.\r\n" +
                    "Watch the lecture on Scope.\r\n");
            }

            if (compilerMessage.message.Contains("CS0133")) {
                // The expression being assigned to ‘variable’ must be constant
                SendCompanionMessage(MessageType.Error,
                    "It seems you are defining a const field but assigning a value that cannot be evaluated at compile time.\r\n" +
                    "Constants can only use basic types which can have their value defined at compile time.\r\n" +
                    "If you want something similar to a constant but with a value that is only defined at runtime then use a static readonly field instead.\r\n" +
                    "Watch the lecture on Constants and Readonly.\r\n");
            }

            if (compilerMessage.message.Contains("CS0152")) {
                // The label ‘label’ already occurs in this switch statement
                SendCompanionMessage(MessageType.Error,
                    "It seems you have multiple case statements within the same switch that have the exact same value.\r\n" +
                    "Case labels must be unique.\r\n");
            }

            if (compilerMessage.message.Contains("CS0161")) {
                // Not all code paths return a value
                SendCompanionMessage(MessageType.Error, 
                    "It seems you defined a function that should return some data but not all code paths lead to a return statement.\r\n" +
                    "If you have some conditional if logic you need to add a return on all possible branches the code could take.\r\n" +
                    "When in doubt you can just return a default value at the very end of the function.\r\n");
            }

            if (compilerMessage.message.Contains("CS0201")) {
                // Only assignment, call, increment, decrement, and new object expressions can be used as a statement
                SendCompanionMessage(MessageType.Error,
                    "It seems you wrote some code that isn't doing anything, it's not assigning anything, doing any math or calling any function.\r\n" +
                    "Maybe you intended to call a function but forgot the parenthesis?\r\n" +
                    "Or in that line maybe you forgot to assign some value = or do some math ++; or --;\r\n");
            }

            if (compilerMessage.message.Contains("CS0501")) {
                // ‘member function’ must declare a body because it is not marked abstract, extern, or partial
                SendCompanionMessage(MessageType.Error,
                    "You need to define a body for the function inside curly braces { }\r\n" +
                    "You can only omit the body if you're defining a function in an interface or its marked as abstract.\r\n" +
                    "There is a lecture on Interfaces and one on Class Intermediate which covers Abstract.\r\n");
            }

            if (compilerMessage.message.Contains("CS0506")) {
                // ‘function1’ : cannot override inherited member ‘function2’ because it is not marked “virtual”, “abstract”, or “override”
                SendCompanionMessage(MessageType.Error,
                    "It seems you're trying to override a function but forgot to mark the base function as \"virtual\" or \"abstract\"\r\n" +
                    "Or maybe you forgot override on the extended function.\r\n" +
                    "Watch the lectures on Class and Class Intermediate\r\n");
            }

            if (compilerMessage.message.Contains("CS0513")) {
                // ‘function’ is abstract but it is contained in nonabstract class ‘class’
                SendCompanionMessage(MessageType.Error, 
                    "You marked a function as 'abstract' but forgot to mark the class itself as abstract.\r\n" +
                    "Abstract functions can only be used in abstract classes.\r\n");
            }

            if (compilerMessage.message.Contains("CS0525")) {
                // Interfaces cannot contain fields
                SendCompanionMessage(MessageType.Error,
                    "You cannot define a field inside an interface, only functions, properties and events.\r\n" +
                    "Interfaces are just a contract so they cannot store any data themselves.\r\n" +
                    "Watch the lecture on Interfaces.\r\n");
            }

            if (compilerMessage.message.Contains("CS0526")) {
                // Interfaces cannot contain constructors
                SendCompanionMessage(MessageType.Error,
                    "You cannot define a constructor inside an interface, only functions, properties and events.\r\n" +
                    "Interfaces are just a contract so they cannot be directly constructed.\r\n" +
                    "Watch the lecture on Interfaces.\r\n");
            }

            if (compilerMessage.message.Contains("CS0844")) {
                // Cannot use local variable ‘name’ before it is declared.
                SendCompanionMessage(MessageType.Error,
                    "It seems you are trying to access a local variable before it is declared.\r\n" +
                    "Remember how code runs from top to bottom, so you need to declare the function above before it is used.\r\n");
            }

            if (compilerMessage.message.Contains("CS1501")) {
                // No overload for method ‘method’ takes ‘number’ arguments
                SendCompanionMessage(MessageType.Error,
                    "You're trying to call a function but using an incorrect number of arguments.\r\n");
            }

            if (compilerMessage.message.Contains("CS1552")) {
                // Array type specifier, [], must appear before parameter name
                SendCompanionMessage(MessageType.Error,
                    "int[] myArray; instead of int myArray[];.\r\n" +
                    "It seems you wrote something like int myArray[]; instead of int[] myArray;\r\n" +
                    "When defining an array the square brackets need to come after the type, not after the name.\r\n");
            }

            if (compilerMessage.message.Contains("CS1612")) {
                // Cannot modify the return value of 'Transform.position' because it is not a variable
                SendCompanionMessage(MessageType.Error,
                    "You cannot modify individual parts of a transform.position, you need to assign it to a brand new Vector3.\r\n" +
                    "Vector3 newPosition = transform.position;\r\n" +
                    "newPosition.x += 5;\r\n" +
                    "transform.position = newPosition;\r\n");
            }





            if (compilerMessage.message.Contains("CS0000")) {
                // AAAAAAAAAAAAAAAAAAAAA
                SendCompanionMessage(MessageType.Error, 
                    "AAAAAAAAA");
            }
        }

            public static void SendCompanionMessage(MessageType messageType, string message) {
            OnCompanionMessageEventArgs lastCompanionMessageEventArgs = new OnCompanionMessageEventArgs {
                messageType = messageType,
                message = message,
            };
            CodeMonkeyCompanionSO.SetLastCompanionMessageEventArgs(lastCompanionMessageEventArgs);
            OnCompanionMessage?.Invoke(null, lastCompanionMessageEventArgs);
        }

        public static OnCompanionMessageEventArgs GetLastCompanionMessageEventArgs() {
            return CodeMonkeyCompanionSO.GetLastCompanionMessageEventArgs();
        }

    }

}