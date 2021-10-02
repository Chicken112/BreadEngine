using System;
using System.Collections;
using System.Collections.Generic;

namespace tui_generator
{
    public class Panel
    {
        public ArrayList cells = new ArrayList();

        private ArrayList objects = new ArrayList();
        public ArrayList cellsVerticalMatrix = new ArrayList();

        private int panelCount;
        public Panel(ArrayList objects, int panelCount){
            this.objects = objects;
            this.panelCount = panelCount;
        }

        

        bool firstDrawCall = true;
        public void Draw(){
            if(firstDrawCall){
                //Fill the cellsverticalmatrix with empty arrays
                for (int i = 0; i < /*Count of rows, need outsideinfo*/panelCount; i++) {
                    cellsVerticalMatrix.Add(new ArrayList());
                }
                //Add cells to cellsverticalmatrix based on their
                //matrixY position
                for (int i = 0; i < cells.Count; i++) {
                    Cell cell = (Cell)cells[i];
                    ((ArrayList)cellsVerticalMatrix[cell.matrixY]).Add(cell);
                }
                //remove the elements from the arraylist
                //with the help of another arrylist
                ArrayList newArray = new ArrayList();
                for (int i = 0; i < cellsVerticalMatrix.Count; i++) {
                    if (((ArrayList)cellsVerticalMatrix[i]).Count != 0) {
                        newArray.Add(cellsVerticalMatrix[i]);
                    }
                }
                cellsVerticalMatrix = newArray;

                firstDrawCall = false;
            }

            int ScreenWidth = Console.WindowWidth;
            int ScreenHeight = Console.WindowHeight;

            int objectIndex = 0;
            int textIndex = 0;
            for (int rowCount = 0; rowCount < cellsVerticalMatrix.Count; rowCount++) {
                ArrayList currentRow = (ArrayList)cellsVerticalMatrix[rowCount];
                Cell lastCell = (Cell)currentRow[currentRow.Count-1];
                for (int y = 0; y < ((Cell)currentRow[0]).Height; y++) {
                    for (int cellIndex = 0; cellIndex < currentRow.Count; cellIndex++) {
                        Cell cell = (Cell)currentRow[cellIndex];
                        bool l,r,t,b;
                        l = isOurCell(cell.matrixX-1, cell.matrixY);
                        r = isOurCell(cell.matrixX+1, cell.matrixY);
                        t = isOurCell(cell.matrixX, cell.matrixY-1);
                        b = isOurCell(cell.matrixX, cell.matrixY+1);
                        for (int x = 0; x < cell.Width; x++) {
                            char toWrite = '*';

                            if (!t && !l && x == 0 && y == 0) {
                                toWrite = '┌';
                            } else if(!t && !r && x == cell.Width-1 && y == 0) {
                                toWrite = '┐';
                            } else if(!l && !b && x == 0 && y == cell.Height-1) {
                                toWrite = '└';
                            } else if(!r && !b && x == cell.Width-1 && y == cell.Height-1) {
                                toWrite = '┘';
                            } else if((x == 0 && !l) || (x == cell.Width-1 && !r)) {
                                toWrite = '│';
                            } else if((y == 0 && !t) || (y == cell.Height-1 && !b)) {
                                toWrite = '─';
                            } else{
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                if(objectIndex >= objects.Count){
                                    toWrite = ' ';
                                }else{
                                    string objString = (string)objects[objectIndex];
                                    if(textIndex >= objString.Length){
                                        //If textIndex out of range

                                        if(cell.Equals(lastCell) && x == cell.Width-2){
                                            //Last char of line
                                            if (textIndex < objString.Length) {
                                                //Haven't finished string
                                            }else{
                                                //Finished string
                                                textIndex = 0;
                                                objectIndex++;
                                            }
                                            toWrite = ' ';
                                        }else{
                                            toWrite = ' ';
                                        }
                                    }else{
                                        //Textindex is not out of range
                                        toWrite = objString[textIndex];
                                        textIndex++;
                                    }
                                }
                            }

                            Console.SetCursorPosition((cell.Width * cell.matrixX) + x, (cell.Height * cell.matrixY) + y);
                            Console.Write(toWrite);
                            Console.ResetColor();
                        }
                    }
                }
            }


            // for (int rowCount = 0; rowCount < cellsVerticalMatrix.Count; rowCount++) {
            //     for (int cellIndex = 0; cellIndex < ((ArrayList)cellsVerticalMatrix[rowCount]).Count; cellIndex++) {
                    
                    
            //         Cell cell = (Cell)((ArrayList)cellsVerticalMatrix[rowCount])[cellIndex];
            //         bool l,r,t,b;
            //         l = isOurCell(cell.matrixX-1, cell.matrixY);
            //         r = isOurCell(cell.matrixX+1, cell.matrixY);
            //         t = isOurCell(cell.matrixX, cell.matrixY-1);
            //         b = isOurCell(cell.matrixX, cell.matrixY+1);
            //         for (int x = 0; x < cell.Width; x++) {
            //             for (int y = 0; y < cell.Height; y++) {
            //                 char toWrite = ((string)objects[0])[0];

            //                 if (!t && !l && x == 0 && y == 0) {
            //                     toWrite = '┌';
            //                 } else if(!t && !r && x == cell.Width-1 && y == 0) {
            //                     toWrite = '┐';
            //                 } else if(!l && !b && x == 0 && y == cell.Height-1) {
            //                     toWrite = '└';
            //                 } else if(!r && !b && x == cell.Width-1 && y == cell.Height-1) {
            //                     toWrite = '┘';
            //                 } else if((x == 0 && !l) || (x == cell.Width-1 && !r)) {
            //                     toWrite = '│';
            //                 } else if((y == 0 && !t) || (y == cell.Height-1 && !b)) {
            //                     toWrite = '─';
            //                 } else{
            //                     Console.ForegroundColor = ConsoleColor.DarkGray;
            //                     toWrite = (rowCount+x).ToString()[0];
            //                 }
            //                 Console.SetCursorPosition((cell.Width * cell.matrixX) + x, (cell.Height * cell.matrixY) + y);
            //                 Console.Write(toWrite);
            //                 Console.ResetColor();
            //             }
            //         }
            //     }
            // }

            // for (int i = 0; i < cells.Count; i++) {
            //     Cell cell = (Cell)cells[i];
            //     bool l,r,t,b;
            //     l = isOurCell(cell.matrixX-1, cell.matrixY);
            //     r = isOurCell(cell.matrixX+1, cell.matrixY);
            //     t = isOurCell(cell.matrixX, cell.matrixY-1);
            //     b = isOurCell(cell.matrixX, cell.matrixY+1);
            //     for (int x = 0; x < cell.Width; x++) {
            //         for (int y = 0; y < cell.Height; y++) {
            //             Console.SetCursorPosition((cell.Width * cell.matrixX) + x, (cell.Height * cell.matrixY) + y);
            //             char toWrite = ((string)objects[0])[0];

            //             //if(x == 0 || x == cell.Width-1 || y == 0 || y == cell.Height-1){
            //                 if (!t && !l && x == 0 && y == 0) {
            //                     toWrite = '┌';
            //                 } else if(!t && !r && x == cell.Width-1 && y == 0) {
            //                     toWrite = '┐';
            //                 } else if(!l && !b && x == 0 && y == cell.Height-1) {
            //                     toWrite = '└';
            //                 } else if(!r && !b && x == cell.Width-1 && y == cell.Height-1) {
            //                     toWrite = '┘';
            //                 } else if((x == 0 && !l) || (x == cell.Width-1 && !r)) {
            //                     toWrite = '│';
            //                 } else if((y == 0 && !t) || (y == cell.Height-1 && !b)) {
            //                     toWrite = '─';
            //                 } else{
            //                     Console.ForegroundColor = ConsoleColor.DarkGray;
            //                     toWrite = i.ToString()[0];
            //                 }
            //                 Console.Write(toWrite);
            //                 Console.ResetColor();
            //         }
            //     }
            // }
        }

        // This function returns weather we own a
        // cell given it's coordinates in the matrix
        public bool isOurCell(int x, int y){
            foreach (Cell cell in cells) {
                if(cell.matrixX == x && cell.matrixY == y){
                    return true;
                }
            }
            return false;
        }

        // [0] [1] [2]  [3]         [0,1]           0,1
        // [4] [5] [6]  [7]         [4,5]
        // [8] [9][10] [11]         [8,9]
        // [12][13][14][15]         [12,13]
    }

    public struct Cell{
        public int Width, Height, matrixX, matrixY;

        public override bool Equals(object obj) {
            if(obj.GetType() == this.GetType()){
                Cell other = (Cell)obj;
                return (matrixX == other.matrixX) && (matrixY == other.matrixY);
            }else{
                return false;
            }
        }
    }
}