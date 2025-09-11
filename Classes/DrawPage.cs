

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
        Sites? site;
        float dpiScale; //had to fix the fonts with this

        public DrawPage()
        {
            bmp = new Bitmap(850, 1100);
            bmp.SetResolution(300,300);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            //g.PageUnit = GraphicsUnit.Point; //not working for me
            //g.PageScale = 1.0f;
            dpiScale = g.DpiY / 96f;  // 96 is the default DPI
        }

        public Bitmap CreateOnCallLog(Sites _site) //only gonna draw one page so no need to make it do anything else
        {  
            site = _site;

            //g.DrawRectangle(new Pen(Color.Black), 30, 30, 775, 1000); //Temp Box - Draw within this box to be safe with most printers

            WriteData();

            return (Bitmap)bmp.Clone();
        }

        private void WriteData()
        {
            WriteTopPart();
            WriteFifteenDates();
            WriteStaffNameAndNumbers();
            WriteBottomLine();
        }

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

        public void Dispose()
        {
            g?.Dispose();
            bmp?.Dispose();
        }
    }
}
