using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
namespace EdytorObrazow
{
    public partial class Form1 : Form
    {
        string sciezka;
        Image obraz;
        float aspect = 1.0f;
        Image obraz_prev1;
        Image obraz_prev2;
        Image obraz_prev3;
        Image obraz_next1;
        Image obraz_next2;
        Image obraz_next3;
        Image obraz_copy;
        Image obraz_original;
        int undos_left = 0;
        int redos_left = 0;
        List<string> zoom_values;
        int typ_wywolania = 1;
        CropAreaRectangle obszarCropowania;

        public void updateUndoHistory()
        {
            if (undos_left < 3)
            {
                undos_left++;
            }
            obraz_prev3 = obraz_prev2;
            obraz_prev2 = obraz_prev1;
            obraz_prev1 = obraz;

            if (undos_left > 0) toolStripButton2.Enabled = true;
            else toolStripButton2.Enabled = false;
        }
        public void refreshView()
        {
            /* undo/redo */
            /* wyznaczenie parametrow okna */

           mainImage.Image = obraz;
           mainImage.Width = (int)((float)obraz.Width*aspect);
           mainImage.Height = (int)((float)obraz.Height*aspect);
           FileInfo f = new FileInfo(sciezka);
           label9.Text = f.Name;
           label11.Text = f.FullName;
           label12.Text = obraz.Width + "px x " + obraz.Height + "px";
           label14.Text = (f.Length / 1024).ToString() + " KB";
           label16.Text = f.LastWriteTime.ToShortDateString();
           statusText.Text = "Obraz " + label9.Text + " [ " + label11.Text + " ] | wymiar: " + label12.Text + " | rozmiar: " + label14.Text + " | data modyfikacji: " + label16.Text + " | Zoom " + (aspect * 100).ToString() + "%";    
           

        }
        public void przygotujObraz(string adres)
        {
            sciezka = adres;
            obraz = Image.FromFile(sciezka);
            obraz_prev1 = obraz;
            obraz_prev2 = obraz;
            obraz_prev3 = obraz;
            obraz_next1 = obraz;
            obraz_next2 = obraz;
            obraz_next3 = obraz;
            obraz_original = obraz;
            aspect = 1;
            
            comboBox1.Text = "100%";

        }
        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(typ_wywolania==2)
             switch(
                 MessageBox.Show(this,
                 "Czy chcesz zapisać zmiany w pliku?",
                 "Edytor obrazów",
                 MessageBoxButtons.YesNoCancel,
                 MessageBoxIcon.Exclamation))
            {
                case DialogResult.Cancel:e.Cancel = true; break;
                case DialogResult.Yes: e.Cancel = true; saveAsButton.PerformClick(); break;
                case DialogResult.No:; break;
            }
        }
        public Form1(string kat, int typ)
        {

            InitializeComponent();
            this.Closing += new CancelEventHandler(this.Form1_Closing);
            toolStripButton2.Enabled = false;
            toolStripButton3.Enabled = false;
            toolStripButton9.Visible = false;
            toolStripButton10.Visible = false;
            typ_wywolania = typ;

            sciezka = kat;

            przygotujObraz(sciezka);
            zoom_values = new List<string>();
            zoom_values.Add("25%");
            zoom_values.Add("50%");
            zoom_values.Add("75%");
            zoom_values.Add("100%");
            zoom_values.Add("125%");
            zoom_values.Add("150%");
            zoom_values.Add("200%");
            zoom_values.Add("400%");

            foreach (string el in zoom_values)
            {
                comboBox1.Items.Add(el);
            }
            comboBox1.Text = "100%";



            obszarCropowania = new CropAreaRectangle(new Rectangle(-100,-100, 10, 10));
            obszarCropowania.SetPictureBox(mainImage);

            obszarCropowania.allowDeformingDuringMovement = true;

            if (typ_wywolania == 1)
            {
                toolStrip1.Visible = false;
                colorsToolbarPanel.Visible = false;
                checkBox1.Visible = false;
                panel4.Visible = true;
            }

            refreshView();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public float FromPercentageString(string value)
        {
            return float.Parse(value.Replace("%", "")) / 100;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            aspect=FromPercentageString(comboBox1.Text);
            obszarCropowania.aspect = aspect;
            refreshView();
        }


        private void window_Resize(object sender, EventArgs e)
        {
            refreshView();
        }
        private void imagePanel_Scroll(object sender, EventArgs e)
        {
            refreshView();
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            updateUndoHistory();
            obraz.RotateFlip(RotateFlipType.RotateNoneFlipY);
            refreshView();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void imagePanel_Paint(object sender, PaintEventArgs e)
        {
            refreshView();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            updateUndoHistory();
            obraz.RotateFlip(RotateFlipType.Rotate270FlipNone);
            refreshView();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            updateUndoHistory();
            obraz.RotateFlip(RotateFlipType.Rotate90FlipNone);
            refreshView();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            updateUndoHistory();
            obraz.RotateFlip(RotateFlipType.RotateNoneFlipX);
            refreshView();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateUndoHistory();
            double contrast = (double)numericUpDown1.Value;
            Bitmap bitmapImage = new Bitmap(obraz);
            double A, R, G, B;

            Color pixelColor;

            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;

                    R = pixelColor.R / 255.0;
                    R -= 0.5;
                    R *= contrast;
                    R += 0.5;
                    R *= 255;

                    if (R > 255)
                    {
                        R = 255;
                    }
                    else if (R < 0)
                    {
                        R = 0;
                    }

                    G = pixelColor.G / 255.0;
                    G -= 0.5;
                    G *= contrast;
                    G += 0.5;
                    G *= 255;
                    if (G > 255)
                    {
                        G = 255;
                    }
                    else if (G < 0)
                    {
                        G = 0;
                    }

                    B = pixelColor.B / 255.0;
                    B -= 0.5;
                    B *= contrast;
                    B += 0.5;
                    B *= 255;
                    if (B > 255)
                    {
                        B = 255;
                    }
                    else if (B < 0)
                    {
                        B = 0;
                    }

                    bitmapImage.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
                }
            }
            obraz = bitmapImage;
            refreshView();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            updateUndoHistory();
            int brightness = (int)numericUpDown2.Value;
            Bitmap bitmapImage = new Bitmap(obraz);
            int A, R, G, B;
            Color pixelColor;

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;
                    R = pixelColor.R + brightness;
                    if (R > 255)
                    {
                        R = 255;
                    }
                    else if (R < 0)
                    {
                        R = 0;
                    }

                    G = pixelColor.G + brightness;
                    if (G > 255)
                    {
                        G = 255;
                    }
                    else if (G < 0)
                    {
                        G = 0;
                    }

                    B = pixelColor.B + brightness;
                    if (B > 255)
                    {
                        B = 255;
                    }
                    else if (B < 0)
                    {
                        B = 0;
                    }

                    bitmapImage.SetPixel(x, y, Color.FromArgb(A, R, G, B));
                }
            }
            obraz = bitmapImage;
            refreshView();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            updateUndoHistory();
            Bitmap bitmapImage = new Bitmap(obraz);
            int A, R, G, B;
            Color pixelColor;

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;
                    R = 255 - pixelColor.R;
                    G = 255 - pixelColor.G;
                    B = 255 - pixelColor.B;
                    bitmapImage.SetPixel(x, y, Color.FromArgb(A, R, G, B));
                }

            }
            obraz = bitmapImage;
            refreshView();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            updateUndoHistory();
            Bitmap bitmapImage = new Bitmap(obraz);
            int A, R, G, B;
            Color pixelColor;
            int ile = (int)numericUpDown3.Value;
            //threshold
            for (int i = 0; i < ile; i++)
            {
                for (int y = 1; y < bitmapImage.Height - 1; y++)
                {
                    for (int x = 1; x < bitmapImage.Width - 1; x++)
                    {
                        pixelColor = bitmapImage.GetPixel(x, y);
                        Color p0 = bitmapImage.GetPixel(x - 1, y - 1);
                        Color p1 = bitmapImage.GetPixel(x - 1, y);
                        Color p2 = bitmapImage.GetPixel(x - 1, y + 1);
                        Color p3 = bitmapImage.GetPixel(x + 1, y - 1);
                        Color p4 = bitmapImage.GetPixel(x + 1, y);
                        Color p5 = bitmapImage.GetPixel(x + 1, y + 1);
                        Color p6 = bitmapImage.GetPixel(x, y - 1);
                        Color p7 = bitmapImage.GetPixel(x, y);
                        Color p8 = bitmapImage.GetPixel(x, y + 1);

                        A = pixelColor.A;
                        R = (int)(p0.R + p1.R + p2.R + p3.R + p4.R + p5.R + p6.R + p7.R + p8.R) / 9;
                        G = (int)(p0.G + p1.G + p2.G + p3.G + p4.G + p5.G + p6.G + p7.G + p8.G) / 9;
                        B = (int)(p0.B + p1.B + p2.B + p3.B + p4.B + p5.B + p6.B + p7.B + p8.B) / 9;
                        bitmapImage.SetPixel(x, y, Color.FromArgb(A, R, G, B));
                    }

                }
            }
            obraz = bitmapImage;
            refreshView();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            updateUndoHistory();
            Bitmap bitmapImage = new Bitmap(obraz);
            byte A, R, G, B;
            Color pixelColor;

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;
                    R = (byte)((0.299 * pixelColor.R) + (0.587 * pixelColor.G) + (0.114 * pixelColor.B));
                    G = B = R;

                    bitmapImage.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
                }
            }
            obraz = bitmapImage;
            refreshView();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            updateUndoHistory();
            Bitmap bitmapImage = new Bitmap(obraz);
            byte A, R, G, B;
            Color pixelColor;
            float[] val = new float[9];
            Random random = new Random();
            for (int i = 0; i < 9; i++)
            {

                val[i]=(float)random.Next(0, 1000) / 1000;
         
                
            }

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;
                    R = (byte)((pixelColor.R * val[0]) + (pixelColor.G * val[1]) + (pixelColor.B * val[2]));


                    G = (byte)((pixelColor.R * val[3]) + (pixelColor.G * val[4]) + (pixelColor.B * val[5]));
                    B = (byte)((pixelColor.R * val[6]) + (pixelColor.G * val[7]) + (pixelColor.B * val[8]));



                    bitmapImage.SetPixel(x, y, Color.FromArgb((int)A, (int)(R < 255 ? R : 255), (int)(G < 255 ? G : 255), (int)(B < 255 ? B : 255)));
                }
            }
            obraz = bitmapImage;
            refreshView();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            updateUndoHistory();
            obraz = obraz_original;
            refreshView();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            
            undos_left--;
            if(redos_left<3) redos_left++;
            obraz_next3 = obraz_next2;
            obraz_next2 = obraz_next1;
            obraz_next1 = obraz;
            obraz = obraz_prev1;
            obraz_prev1 = obraz_prev2;
            obraz_prev2 = obraz_prev3;
            toolStripButton3.Enabled = true;
            if (undos_left == 0) toolStripButton2.Enabled = false;
            refreshView();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            
            redos_left--;
         
            if(undos_left<3) undos_left++;
            obraz_prev3 = obraz_prev2;
            obraz_prev2 = obraz_prev1;
            obraz_prev1 = obraz;
           toolStripButton2.Enabled = true;
            if (redos_left == 0) toolStripButton3.Enabled = false;
            obraz = obraz_next1;
            obraz_next1 = obraz_next2;
            obraz_next2 = obraz_next3;
            refreshView();
            
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            obraz_copy = obraz;
            obszarCropowania.active = true;
            /* disable toolstrip */
            saveAsButton.Enabled = false;
            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = false;
            toolStripButton3.Enabled = false;
            toolStripButton4.Enabled = false;
            toolStripButton5.Enabled = false;
            toolStripButton6.Enabled = false;
            toolStripButton7.Enabled = false;
            panel3.Visible = false;
            colorsToolbarPanel.Enabled = false;
            toolStripButton8.Enabled = false;
            toolStripButton11.Enabled = false;
            panel2.Enabled = false;

            /* show additional buttons */
            toolStripButton10.Visible = true;
            toolStripButton9.Visible = true;

            /* create rectangle inside the panel */
            obszarCropowania.aspect = aspect;
            obszarCropowania.rect.Width = 100;
            obszarCropowania.rect.Height = 100;
            obszarCropowania.rect.X = 10;
            obszarCropowania.rect.Y = 10;
            refreshView();
            



        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
          
            updateUndoHistory();
            /* get croppped rectangle */
            obszarCropowania.rect.X = (int)((float)obszarCropowania.rect.X/aspect);
            obszarCropowania.rect.Y = (int)((float)obszarCropowania.rect.Y / aspect);
            obszarCropowania.rect.Width =(int)((float)obszarCropowania.rect.Width/aspect);
            obszarCropowania.rect.Height =(int)((float)obszarCropowania.rect.Height/ aspect);
            
            
            Bitmap bmpImage = new Bitmap(obraz_copy);
            Bitmap bmpCrop = bmpImage.Clone(obszarCropowania.rect,
            bmpImage.PixelFormat);
            obraz=(Image)(bmpCrop);
            
            /* hide rectangle */
            obszarCropowania.rect.X = -100;
            obszarCropowania.rect.Y = -100;
            obszarCropowania.rect.Width = 1;
            obszarCropowania.rect.Height = 1;

            obszarCropowania.active = false;
            refreshView();


            /* restore disabling */
           
            saveAsButton.Enabled = true;
            toolStripButton1.Enabled = true;
            toolStripButton2.Enabled = true;
            toolStripButton3.Enabled = true;
            toolStripButton4.Enabled = true;
            toolStripButton5.Enabled = true;
            toolStripButton6.Enabled = true;
            toolStripButton7.Enabled = true;
            colorsToolbarPanel.Enabled = true;
            toolStripButton11.Enabled = true;
            toolStripButton8.Enabled = true;
            panel2.Enabled = true;

            
            toolStripButton10.Visible = false;
            toolStripButton9.Visible = false;
            
            
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            /* hide rectangle */
            obszarCropowania.rect.X = -100;
            obszarCropowania.rect.Y = -100;
            obszarCropowania.rect.Width = 1;
            obszarCropowania.rect.Height = 1;
            obszarCropowania.active = false;
            refreshView();
            /* restore disabling */
           
            saveAsButton.Enabled = true;
            toolStripButton1.Enabled = true;
            toolStripButton2.Enabled = true;
            toolStripButton3.Enabled = true;
            toolStripButton4.Enabled = true;
            toolStripButton5.Enabled = true;
            toolStripButton6.Enabled = true;
            toolStripButton7.Enabled = true;
            colorsToolbarPanel.Enabled = true;
            toolStripButton11.Enabled = true;
            toolStripButton8.Enabled = true;
            panel2.Enabled = true;

           
            toolStripButton10.Visible = false;
            toolStripButton9.Visible = false;
        
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

            
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {


            saveFileDialog1.Title = "Zapisz zmodyfikowany obraz jako...";
            saveFileDialog1.Filter = "Plik grafiki rastrowej skompresowanej JPEG|*.jpeg;*.jpg|Bitmapa nieskompresowana BMP|*.bmp|Plik grafiki skompresowanej GIF|*.gif|Wszystkie pliki|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = "Nowy obrazek";
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                   
                    if(checkBox1.Checked)
                    {
                        
                        int originalW = obraz.Width;
                        int originalH = obraz.Height;

                      
                        int resizedW = (int)(originalW * aspect);
                        int resizedH = (int)(originalH * aspect);

                      
                        Bitmap bmp = new Bitmap(resizedW, resizedH);
                        //create a new graphic from the Bitmap
                        Graphics graphic = Graphics.FromImage((Image)bmp);
                        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        //draw the newly resized image
                        graphic.DrawImage(obraz, 0, 0, resizedW, resizedH);
                        //dispose and free up the resources
                        graphic.Dispose();
                        //return the image
                        obraz=(Image)bmp;
                        aspect = 1;
                        comboBox1.Text = "100%";
                        refreshView();
                    }
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    switch (saveFileDialog1.FilterIndex)
                    {

                        case 2:
                            obraz.Save(fs,
                            System.Drawing.Imaging.ImageFormat.Bmp);
                            break;

                        case 3:
                            obraz.Save(fs,
                            System.Drawing.Imaging.ImageFormat.Gif);
                            break;

                        default:
                            obraz.Save(fs,
                            System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;

                    }
                    fs.Close();
                }

            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Title = "Otwórz obraz";
            openFileDialog1.Filter = "Plik grafiki rastrowej skompresowanej JPEG|*.jpeg|Bitmapa nieskompresowana BMP|*.bmp|Plik grafiki skompresowanej GIF|*.gif|Wszystkie pliki|*.*";
            openFileDialog1.FilterIndex = 4;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName != "")
                {
                    przygotujObraz(openFileDialog1.FileName);
                }
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            updateUndoHistory();
            Bitmap bitmapImage = new Bitmap(obraz);
            double r= (double)numericUpDown4.Value/255;
            double g = (double)numericUpDown5.Value/255;
            double b = (double)numericUpDown6.Value/255;
  
            byte A, R, G, B;  
            Color pixelColor;  
  
            for (int y = 0; y < bitmapImage.Height; y++)  
            {  
                for (int x = 0; x < bitmapImage.Width; x++)  
                {  
                    pixelColor = bitmapImage.GetPixel(x, y);  
                    A = pixelColor.A;  
                    R = (byte)(pixelColor.R * r);  
                    G = (byte)(pixelColor.G * g);  
                    B = (byte)(pixelColor.B * b);  
                    bitmapImage.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));  
                }  
            }
            obraz = bitmapImage;
            refreshView();
            panel3.Visible = false;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            numericUpDown4.Value = hScrollBar1.Value;
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            numericUpDown5.Value = hScrollBar2.Value;
        }

        private void hScrollBar3_Scroll(object sender, ScrollEventArgs e)
        {
            numericUpDown6.Value = hScrollBar3.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
             hScrollBar1.Value=(int)numericUpDown4.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            hScrollBar2.Value = (int)numericUpDown5.Value;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            hScrollBar3.Value = (int)numericUpDown6.Value;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (panel3.Visible) panel3.Visible = false;
            else panel3.Visible = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            //button7.PerformClick();
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                button7.PerformClick();
            }
        }
        private bool mouseDown;
        private Point mousePos;

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                mousePos = new Point(e.X, e.Y);
            }
        }

        private void panel3_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                panel3.Location = PointToClient(this.panel3.PointToScreen(new Point(e.X - mousePos.X, e.Y - mousePos.Y)));
            }
        }

        private void panel3_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                mouseDown = false;
            }
        }

    }
}
