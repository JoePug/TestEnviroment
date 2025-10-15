

using static System.Windows.Forms.DataFormats;

namespace TestEnviroment
{
    public class DrawPage : IDisposable
    {
        //takes a Site class, uses it then draws the page with the information from Site
        int x = 0;
        int y = 0;
        Bitmap bmp;
        Graphics g;
        Brush brush = Brushes.Black;
        Font theFont = new Font("Comic Sans MS", 16, FontStyle.Bold | FontStyle.Underline);
        string text = "ON CALL  SCHEDULE";
        //Sites? site;
        float dpiScale; //had to fix the fonts with this
        RichTextBox box;

        List<string> linesOfText = new List<string>(); //for textboxes

        public DrawPage(RichTextBox _box)
        {
            box = _box;

            bmp = new Bitmap(850, 1100);
            bmp.SetResolution(300,300);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            //g.PageUnit = GraphicsUnit.Point; //not working for me
            //g.PageScale = 1.0f;
            dpiScale = g.DpiY / 96f;  // 96 is the default DPI
            //CreateOnCallLog();
        }

        public Bitmap CreateOnCallLog() //only gonna draw one page so no need to make it do anything else
        {  
            g.DrawRectangle(new Pen(Color.Black), 30, 30, 775, 1000); //Temp Box - Draw within this box to be safe with most printers

            TestWrite();

            return (Bitmap)bmp.Clone();
        }

        private void TestWrite()
        {
            int x = 100;
            int y = 100;
            int width = 200;
            int height = 100;
            int fontSize = 26;
            string text = "I would like to fit this entire text into that box automatically, without any issues.";
            //string text = "This";
            Font theFont = new Font("Times New Roman", fontSize / dpiScale, FontStyle.Regular);
            Brush brush = Brushes.Black;

            g.DrawRectangle(new Pen(Color.Black), x, y, width, height);

            Log("Testing.....");

            if (MakeStringFit(text, theFont, new RectangleF(x, y, width, height)))
            {
                PrintTextInRectangle(new RectangleF(x, y, width, height));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("***** It Didn't Work *****");
            }
        }

        void PrintTextInRectangle(RectangleF rect)
        {
            int height = (int)g.MeasureString(linesOfText[0], theFont).Height;
            int _x = (int)rect.X;
            int _y = (int)rect.Y;

            foreach(string line in linesOfText)
            {
                g.DrawString(line, theFont, brush, x, y);
                _y = y + height;
            }
        }

        void PrintTextInRectangle(RectangleF rect, string text, Font baseFont, Brush brush)
        {
            StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                //FormatFlags = StringFormatFlags.LineLimit
            };

            // Measure how much text fits
            SizeF size = g.MeasureString(text, baseFont, rect.Size, format);

            // Optional: shrink font if it overflows
            Log("Size: " + size.Height.ToString());
            Log("Rect: " + rect.Height.ToString());
            Log("Font: " + baseFont.Size.ToString()); 

            while (size.Height > rect.Height && baseFont.Size > 4)
            {
                baseFont = new Font(baseFont.FontFamily, baseFont.Size - 1, baseFont.Style);
                size = g.MeasureString(text, baseFont, rect.Size, format);
                Log("***** " + baseFont.Size.ToString());
            }            

            g.DrawString(text, baseFont, brush, rect, format);
        }

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
                Log("***** " + baseFont.Size.ToString());
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
                    if (g.MeasureString(word, baseFont).Width > rect.Width)
                    {
                        //If the one word is bigger, lower font and exit I guess
                        lines.Clear();
                        currentLine = "";
                        count = words.Count();
                        baseFont = new Font(baseFont.FontFamily, baseFont.Size - 1, baseFont.Style);
                        break;
                    }

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

                    if(baseFont.Size <= 4) exitWhile = true; //failed to fit text in rect
                    
                }
            }

            //need to return font, and lines and if it worked or not
            return success;
        }

        float MeasureMultilineHeight(Graphics g, string text, Font font, float maxWidth)
        {
            StringFormat format = new StringFormat
            {
                FormatFlags = StringFormatFlags.MeasureTrailingSpaces,
                Trimming = StringTrimming.Word
            };

            string[] words = text.Split(' ');
            List<string> lines = new List<string>();
            string currentLine = "";

            foreach (var word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                SizeF testSize = g.MeasureString(testLine, font, new SizeF(maxWidth, float.MaxValue), format);

                if (testSize.Width > maxWidth && !string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);

            float lineHeight = font.GetHeight(g);
            return lines.Count * lineHeight;
        }

        public void Dispose()
        {
            g?.Dispose();
            bmp?.Dispose();
        }

        private void Log(string message)
        {
            box.AppendText(message + Environment.NewLine);
        }
        
        /*
        private void WriteTopPart()
        {
            x = 405;
            y = 30;
            theFont = new Font("Comic Sans MS", 16 / dpiScale, FontStyle.Bold | FontStyle.Underline);
            g.DrawString(text, theFont, brush, x - (g.MeasureString(text, theFont).Width / 2), y);  //  centered

            y += (int)g.MeasureString(text, theFont).Height;
            text = site.SiteName.ToUpper();
            g.DrawString(text, theFont, brush, x - (g.MeasureString(text, theFont).Width / 2), y);  //  centered

            y += (int)g.MeasureString(text, theFont).Height;
            text = site.Year.ToString();

            theFont = new Font("Comic Sans MS", 14 / dpiScale, FontStyle.Bold);
            g.DrawString(text, theFont, new SolidBrush(Color.FromArgb(0x00, 0x2F, 0x6C)), // close enough to original
                                    x - (g.MeasureString(text, theFont).Width / 2), y);
        }

        private void WriteFifteenDates()
        {
            x = 30;
            y = 180;
            theFont = new Font("Comic Sans MS", 14 / dpiScale, FontStyle.Bold);
            string[] splitText = [];

            for (int i = 0; i < 15; i++)
            {
                splitText = SplitString(site.CurrentPageOfLines[i], "- " + site.CurrentPageOfNamesAndNumbers[i].Item1);
                for (int j = 0; j < 8; j++)
                {

                    if (j == 2 || j == 6)
                    {
                        //J = 2 or 6, switch font and after switch back, subtract maybe 5 from x to move remove space before suffix.
                        theFont = new Font("Comic Sans MS", 8 / dpiScale, FontStyle.Bold);
                        x -= 5;
                        g.DrawString(splitText[j], theFont, brush, x, y);
                        x += (int)g.MeasureString(splitText[j], theFont).Width;
                        theFont = new Font("Comic Sans MS", 14 / dpiScale, FontStyle.Bold);
                    }
                    else
                    {
                        g.DrawString(splitText[j], theFont, brush, x, y);
                        x += (int)g.MeasureString(splitText[j], theFont).Width;
                    }      
                }
                
                y += 55;
                x = 30;
            }
        }

        private string[] SplitString(string text1, string text2)
        {
            string[] first = [];
            string[] second = new string[8];

            first = text1.Split(' ');                               
            second[0] = first[0];                             //month
            second[1] = first[1].Substring(0, first[1].Length - 2); //day
            second[2] = first[1].Substring(first[1].Length - 2);    //suffix
            second[3] = "-";                                      
            second[4] =  first[3];                            //second month
            second[5] = first[4].Substring(0, first[4].Length - 2); //second day
            second[6] = first[4].Substring(first[4].Length - 2);    //second suffix  
            second[7] = text2;

            return second;
        }

        private void WriteBottomLine()
        {
            if(site?.CommentActive == false ) return;

            x = 405;
            y = 1010;
            text = site.CommentToPrint;
            theFont = new Font("Comic Sans MS", 12 / dpiScale, FontStyle.Bold);

            g.DrawString(text, theFont, brush, x - (g.MeasureString(text, theFont).Width / 2), y); //  centered
        }

        private void WriteStaffNameAndNumbers()
        {
            List<(string, string)> staff = site.GetStaffNameAndNumbers().GetNameAndNumbersList();
            int count = staff.Count();

            theFont = new Font("Comic Sans MS", 15 / dpiScale, FontStyle.Bold);
            text = "On Call Staff";
            x = 595;
            y = 150;

            g.DrawString(text, theFont, brush, x, y);

            theFont = new Font("Comic Sans MS", 10 / dpiScale, FontStyle.Bold);
            int height = (int)g.MeasureString(text, theFont).Height * count;

            // Height of rectangle will confrom to lines of text. But it never checks for the end of page.
            g.DrawRectangle(new Pen(Color.Black), 520, 180, 280, height);

            y = 180;
            foreach((string, string) name in staff)
            {
                text = name.Item1.Split(' ')[0];
                g.DrawString(text, theFont, brush, 535, y);
                text = name.Item2;
                g.DrawString(text, theFont, brush, 680, y);
                y += (int)g.MeasureString(text, theFont).Height;
            }
        }
        */
    }
}
