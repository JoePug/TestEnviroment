using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEnviroment
{
    public class TextFunctions
    {
        Font theFont = new Font("Comic Sans MS", 16, FontStyle.Bold | FontStyle.Underline);
        List<string> linesOfText = new List<string>(); //for textboxes
        RichTextBox box;
        Graphics g;

        public void WriteTextInRect()
        {

        }

        #region Psudo Code

        /*
         * split text into a string array of words
         * 
         * loop to make sure each word will fit into the length of the box (decrease font till the current word fits)
         * - check for minimun font size. Break (or return if hit)
         * 
         * while loop from font size to minimun font size         
         * - foreach loop will attempt to fit words in box at current font
         * -- any error will cause the font size to drop by .5, break the loop and start over again.
         * 
         * 
         * 
         * 
         */

        #endregion

        bool MakeStringFit(string text, Font baseFont, RectangleF rect)
        {
            string[] words = text.Split(' ');
            List<string> lines = new List<string>();
            string currentLine = "";
            bool success = false;

            //Make sure text fits in the rect in the first place, height wise
            while (g.MeasureString(text, baseFont).Height > rect.Height)
            {
                baseFont = new Font(baseFont.FontFamily, baseFont.Size - 1, baseFont.Style);
                Log("Height ***** " + baseFont.Size.ToString());
            }

            //Make sure each word is smaller then the lenght of the box            
            foreach (string word in words)
            {
                while (g.MeasureString(word, baseFont).Width > rect.Width)
                {
                    Log(word);
                    baseFont = new Font(baseFont.FontFamily, baseFont.Size - 1, baseFont.Style);
                    Log(baseFont.Size.ToString());
                }
            }

            int count = words.Count();
            bool exitWhile = false;
            //generate lines based on width of rect
            while (!exitWhile)
            {
                //if (baseFont.Size < 4) break; //for now as I need to figure out what to do when I exit

                foreach (string word in words)
                {
                    count--; //hopefully not one off                    

                    string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                    if (g.MeasureString(testLine, baseFont).Width < rect.Width)
                    {
                        currentLine = testLine;
                    }
                    else
                    {
                        lines.Add(currentLine);
                        currentLine = word;
                    }

                    if (count == 0)
                    {
                        lines.Add(currentLine);
                    }
                }

                if (g.MeasureString(lines[0], baseFont).Height * lines.Count < rect.Height)
                {
                    exitWhile = true;
                    success = true;
                    linesOfText = lines;
                    theFont = baseFont;
                }
                else //loop again  
                {
                    lines.Clear();
                    currentLine = "";
                    count = words.Count();
                    baseFont = new Font(baseFont.FontFamily, baseFont.Size - 1, baseFont.Style);

                    if (baseFont.Size <= 3)
                    {
                        Log("Text is to long - Font Size: " + baseFont.Size.ToString());
                        exitWhile = true; //failed to fit text in rect
                    }
                }
            }

            //need to return font, and lines and if it worked or not
            return success;
        }

        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
