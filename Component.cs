using System;

namespace tui_generator
{
    public class Component {
        
        public string textRepresentation{get;set;}

        public ConsoleColor foreground{get;set;}
        public ConsoleColor background{get;set;}

        public virtual char[] Draw(int width){
            char[] toReturn = new char[textRepresentation.Length];
            for (int i = 0; i < textRepresentation.Length; i++) {
                toReturn[i] = textRepresentation[i];
            }
            return toReturn;
        }
    }

    public class Text : Component {
        public Text(string text){
            textRepresentation = text;
            foreground = ConsoleColor.White;
            background = ConsoleColor.Black;
        }

        
    }

    public class Spacer : Text{
        public Spacer(): base(text:""){ }
    }

    public class Button : Component {
        public Button(string text){
            textRepresentation = text;
            foreground = ConsoleColor.Black;
            background = ConsoleColor.White;
        }
    }

    public class LoadBar : Component {

        public int percent = 90;

        public LoadBar(){
            textRepresentation = "LoadBar";
            foreground = ConsoleColor.Black;
            background = ConsoleColor.White;
        }

        public override char[] Draw(int width) {
            char[] toReturn = new char[width];
            for (int i = 0; i < width; i++) {
                if(i == 0) {
                    toReturn[i] = '[';
                } else if(i == width-1){
                    toReturn[i] = ']';
                }else {
                    if(i < (width * percent / 100)) {
                        toReturn[i] = '=';
                    } else {
                        toReturn[i] = ' ';
                    }
                }
            }
            return toReturn;
        }
    }
}