using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BreadEngine {
    public struct ReadReturn {
        public ArrayList matrix;
        public Dictionary<char,ArrayList> identifiers;

        public ArrayList naviagation;
    }


    public class FileReader {

        /// This method reads a file, parses it
        /// and returns the data in a special
        /// struct. It also throws errors when
        /// finding them
        public static ReadReturn Read(string path) {
            try {
            using (var sr = new StreamReader(path)) {
                bool inLayout = false;
                bool inNavigation = false;
                int layoutWidth = -1;
                char currentIdentifier = '\0';
                bool previousLineIsEmpty = false;
                ArrayList matrix = new ArrayList();
                Dictionary<char,ArrayList> identifiers = new Dictionary<char, ArrayList>();
                ArrayList navigation = new ArrayList();

                int lineCounter = 0;
                while (sr.Peek() >= 0) {
                    lineCounter++;
                    string line = sr.ReadLine().Trim().ToLower(); //TODO don't make all lowercase
                    if(String.IsNullOrWhiteSpace(line)) {
                        inLayout = false;
                        inNavigation = false;
                        currentIdentifier = '\0';
                        previousLineIsEmpty = true;
                        continue;
                    }

                    if(line == ":layout") {
                        if(inNavigation){
                            ThrowError($"Start of a new identifier should always be seperated with an empty line (line {lineCounter})");
                        }
                        inNavigation = false;
                        inLayout = true;
                        continue;
                    } else if(line == ":nav") {
                        if(inLayout){
                            ThrowError($"Start of a new identifier should always be seperated with an empty line (line {lineCounter})");
                        }
                        inLayout = false;
                        inNavigation = true;
                        continue;
                    } else if(line.StartsWith(":")) {
                        if(inLayout || inNavigation || !previousLineIsEmpty){
                            ThrowError($"Start of a new identifier should always be seperated with an empty line (line {lineCounter})");
                        }
                        inLayout = false; //TODO error handling: multiple sections after
                        inNavigation = false; // each other can cause wired effect
                        try {
                            char selector = Char.Parse(line.Substring(1));
                            if(!identifiers.ContainsKey(selector)) {
                                ThrowError($"Identifier '{selector}' was not declared in layout (line {lineCounter})");
                            } else if(identifiers[selector].Count != 0){
                                ThrowError($"Identifier is defined on multiple occasions (line {lineCounter})");
                            }
                            currentIdentifier = selector;
                        } catch (Exception) {
                            ThrowError($"Identifier couldn't be parsed into a character (line {lineCounter})");
                        }
                        
                    } else {
                        if(inLayout) {
                            string[] sections = line.Split(' ');
                            char[] _sections = new char[sections.Length];
                            for (int i = 0; i < sections.Length; i++) {
                                try {
                                    _sections[i] = Char.Parse(sections[i]);
                                    if(_sections[i] == ':') {
                                        ThrowError($"Illegal layout identifier. Identifier cannot be ':' (line {lineCounter})");
                                    }
                                    if(!identifiers.ContainsKey(_sections[i])) {
                                        identifiers.Add(_sections[i], new ArrayList());
                                    }
                                }catch(Exception) {
                                    ThrowError($"Error parsing layout: '{sections[i]}' is not a character (line {lineCounter})");
                                }
                            }
                            if(layoutWidth == -1) {
                                layoutWidth = _sections.Length;
                            } else if(layoutWidth != _sections.Length) {
                                ThrowError($"Layout length is not consistent troughouth lines (line {lineCounter})");
                            }
                            matrix.Add(_sections);
                        } else if(currentIdentifier != '\0') {
                            //Parsing of components

                            if(line.StartsWith("text")) {
                                identifiers[currentIdentifier].Add(new Text(getParameter(line, lineCounter)));
                            } else if(line.StartsWith("button")) {
                                identifiers[currentIdentifier].Add(new Button(getParameter(line, lineCounter)));
                            } else if(line.StartsWith("title")) { //TODO check for multiple titles
                                identifiers[currentIdentifier].Add(new Title(getParameter(line, lineCounter)));
                            } else if(line.StartsWith("spacer")) {
                                if(line.Length > 6 && line[6] == '('){
                                    identifiers[currentIdentifier].Add(new Spacer(getParameter(line,lineCounter)));
                                }else{
                                    identifiers[currentIdentifier].Add(new Spacer());
                                }
                            } else if(line.StartsWith("loader")) {
                                identifiers[currentIdentifier].Add(new LoadBar());
                            } else {
                                ThrowError($"Unrecognized component on line {lineCounter}");
                            }
                        } else if(inNavigation){
                            string[] lineSections = line.Split(' ');
                            for (int i = 0; i < lineSections.Length; i++) {
                                if(lineSections[i].Length > 1){
                                    ThrowError($"Navigationcomponent must be of type char (line {lineCounter})");
                                } else if(!identifiers.ContainsKey(lineSections[i][0])){
                                    ThrowError($"No identifier resembles char '{lineSections[i][0]}'");
                                } else {
                                    navigation.Add(lineSections[i][0]);
                                }
                            }
                        } else {
                            ThrowError($"Cannot understand line {lineCounter} ('{line}')");
                        }
                    }
                    previousLineIsEmpty = false;
                }

                //TODO check if all regions aren't cut off
                // 1 0 0
                // 0 1 0  Like this
                // 0 1 0

                return new ReadReturn() {
                    matrix = matrix,
                    identifiers = identifiers,
                    naviagation = navigation
                };
            }
            } catch (IOException e) {
                ThrowError($"Error reading file: {e.Message}");
                return new ReadReturn();
            }
        }


        //Extracts the string from inside of
        // the caracters '(' and ')'
        public static string getParameter(string line, int lineCount) {
            int from = line.IndexOf('(');
            int to = line.LastIndexOf(')');
            if(from == -1) {
                ThrowError($"Missing Opening bracket on line {lineCount}");
                return "";
            } else if(to == -1) {
                ThrowError($"Missing Closing bracket on line {lineCount}");
                return "";
            } else {
                string toReturn = line.Substring(from+1, to - from-1);
                if(toReturn.Length == 0) {
                    ThrowError($"Empty parameter on line {lineCount}");
                }
                return toReturn;
            }
        }


        //Stops execution and prompts to user
        public static void ThrowError(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            FastConsole.Write("An error occured: ");
            FastConsole.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
            FastConsole.Flush();
            Environment.Exit(1);
        }
    }
}