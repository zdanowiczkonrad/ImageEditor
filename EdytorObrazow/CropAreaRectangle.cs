using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EdytorObrazow
{
    class CropAreaRectangle 
    {
         private PictureBox mPictureBox;
        public Rectangle rect;
        public bool allowDeformingDuringMovement=false ;
        private bool mIsClick=false;
        private bool mMove=false;        
        private int oldX;
        private int oldY;
        private int sizeNodeRect= 6;
        private Bitmap mBmp=null;
        private PosSizableRect nodeSelected = PosSizableRect.None;
        private int angle = 30;
        public bool active = false;
        public float aspect = 1;
        private enum PosSizableRect
        {            
            UpMiddle,
            LeftMiddle,
            LeftBottom,
            LeftUp,
            RightUp,
            RightMiddle,
            RightBottom,
            BottomMiddle,
            None

        };

        public CropAreaRectangle(Rectangle r)
        {
            rect = r;
            mIsClick = false;
        }
        public void Undraw(Graphics g)
        {
            g.DrawImage(mPictureBox.Image, 0, 0, mPictureBox.Width, mPictureBox.Height);
        }

        public void Draw(Graphics g)
        {


            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(122, 0,0,0));
            g.FillRectangle(blueBrush, 0, 0, mPictureBox.Width, mPictureBox.Height);
            g.FillRectangle(blueBrush,rect);
            
            g.DrawImage(mPictureBox.Image,rect,new Rectangle((int)((float)rect.X/aspect),(int)((float)rect.Y/aspect),(int)((float)rect.Width/aspect),(int)((float)rect.Height/aspect)),GraphicsUnit.Pixel);
           
            g.DrawRectangle(new Pen(Color.FromArgb(255, 0,255,0)), rect);
            
            foreach (PosSizableRect pos in Enum.GetValues(typeof(PosSizableRect)))
            {
                
                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 255, 0)), GetRect(pos));
                g.DrawRectangle(new Pen(Color.FromArgb(255, 0, 255, 0)), GetRect(pos));
            }                       
        }
  

        public void SetBitmapFile(string filename)
        {
            this.mBmp = new Bitmap(filename);
        }

        public void SetBitmap(Bitmap bmp)
        {
            this.mBmp = bmp;
        }

        public void SetPictureBox(PictureBox p)
        {
            this.mPictureBox = p;
            mPictureBox.MouseDown +=new MouseEventHandler(mPictureBox_MouseDown);
            mPictureBox.MouseUp += new MouseEventHandler(mPictureBox_MouseUp);
            mPictureBox.MouseMove += new MouseEventHandler(mPictureBox_MouseMove);            
            mPictureBox.Paint += new PaintEventHandler(mPictureBox_Paint);
        }

        private void mPictureBox_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (active) Draw(e.Graphics);
                else Undraw(e.Graphics);
            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.Message);
            }
            
        }

        private void mPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            mIsClick = true;

            nodeSelected = PosSizableRect.None;
            nodeSelected = GetNodeSelectable(e.Location);
                
            if (rect.Contains(new Point(e.X,e.Y)))
            {
                mMove = true;                            
            }
            oldX = e.X;
            oldY = e.Y;
        }

        private void mPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            mIsClick = false;
            mMove = false;            
        }

        private void mPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            ChangeCursor(e.Location);
            if (mIsClick == false)
            {
                return;
            }

            Rectangle backupRect = rect;

            switch (nodeSelected)
            {
                case PosSizableRect.LeftUp:
                    rect.X += e.X - oldX;
                    rect.Width -= e.X - oldX;                    
                    rect.Y += e.Y - oldY;
                    rect.Height -= e.Y - oldY;
                    break;
                case PosSizableRect.LeftMiddle:
                    rect.X += e.X - oldX;
                    rect.Width -= e.X - oldX;
                    break;
                case PosSizableRect.LeftBottom:
                    rect.Width -= e.X - oldX;
                    rect.X += e.X - oldX;
                    rect.Height += e.Y - oldY;
                    break;
                case PosSizableRect.BottomMiddle:
                    rect.Height += e.Y - oldY;
                    break;
                case PosSizableRect.RightUp:
                    rect.Width += e.X - oldX;
                    rect.Y += e.Y - oldY;
                    rect.Height -= e.Y - oldY;
                    break;
                case PosSizableRect.RightBottom:
                    rect.Width +=  e.X - oldX;
                    rect.Height += e.Y - oldY;
                    break;
                case PosSizableRect.RightMiddle:
                    rect.Width += e.X - oldX;
                    break;

                case PosSizableRect.UpMiddle:
                    rect.Y += e.Y - oldY;
                    rect.Height -= e.Y - oldY;
                    break;

                default:
                    if (mMove)
                    {
                            rect.X = rect.X + e.X - oldX;
                            rect.Y = rect.Y + e.Y - oldY;
                    }
                    break;
            }
            oldX = e.X;
            oldY = e.Y;

            if (rect.Width < 5 || rect.Height < 5)
            {
                rect = backupRect;
            }

            TestIfRectInsideArea();

            mPictureBox.Invalidate();
        }

        private void TestIfRectInsideArea()
        {
            // Test if rectangle still inside the area.
            if (rect.X < 0) rect.X = 0;
            if (rect.Y < 0) rect.Y = 0;
            if (rect.Width <= 0) rect.Width = 1;
            if (rect.Height <= 0) rect.Height = 1;

            if (rect.X + rect.Width > mPictureBox.Width)
            {
                rect.Width = mPictureBox.Width - rect.X - 1; // -1 to be still show 
                if (allowDeformingDuringMovement == false)
                {
                    mIsClick = false;
                }
            }
            if (rect.Y + rect.Height > mPictureBox.Height)
            {
                rect.Height = mPictureBox.Height - rect.Y - 1;// -1 to be still show 
                if (allowDeformingDuringMovement == false)
                {
                    mIsClick = false;
                }
            }
        }        

        private Rectangle CreateRectSizableNode(int x, int y,bool duzy)
        {
            if(duzy) return new Rectangle(x - sizeNodeRect / 2, y - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);
            return new Rectangle(x - sizeNodeRect / 2, y - sizeNodeRect / 2, sizeNodeRect-1, sizeNodeRect-1);   
        }

        private Rectangle GetRect(PosSizableRect p)
        {
            switch (p)
            {
                case PosSizableRect.LeftUp:
                    return CreateRectSizableNode(rect.X, rect.Y,true);
                 
                case PosSizableRect.LeftMiddle:
                    return CreateRectSizableNode(rect.X, rect.Y + +rect.Height / 2,false);                    

                case PosSizableRect.LeftBottom:
                    return CreateRectSizableNode(rect.X, rect.Y +rect.Height,true);                                   

                case PosSizableRect.BottomMiddle:
                    return CreateRectSizableNode(rect.X  + rect.Width / 2,rect.Y + rect.Height,false);

                case PosSizableRect.RightUp:
                    return CreateRectSizableNode(rect.X + rect.Width,rect.Y,true);

                case PosSizableRect.RightBottom:
                    return CreateRectSizableNode(rect.X  + rect.Width,rect.Y  + rect.Height,true);

                case PosSizableRect.RightMiddle:
                    return CreateRectSizableNode(rect.X  + rect.Width, rect.Y  + rect.Height / 2,false);

                case PosSizableRect.UpMiddle:
                    return CreateRectSizableNode(rect.X + rect.Width/2, rect.Y,false);
                default :
                    return new Rectangle();
            }
        }

        private PosSizableRect GetNodeSelectable(Point p)
        {
           foreach (PosSizableRect r in Enum.GetValues(typeof(PosSizableRect)))
            {
                if (GetRect(r).Contains(p))
                {
                    return r;                    
                }
            }
            return PosSizableRect.None;
        }

        private void ChangeCursor(Point p)
        {
            if (rect.Contains(p))
            {
                mPictureBox.Cursor = Cursors.SizeAll;
            }
            else mPictureBox.Cursor = GetCursor(GetNodeSelectable(p));
        }

        /// <summary>
        /// Get cursor for the handle
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Cursor GetCursor(PosSizableRect p)
        {
            switch (p)
            {
                case PosSizableRect.LeftUp:
                    return Cursors.SizeNWSE;               

                case PosSizableRect.LeftMiddle:
                    return Cursors.SizeWE;

                case PosSizableRect.LeftBottom:
                    return Cursors.SizeNESW;

                case PosSizableRect.BottomMiddle:
                    return Cursors.SizeNS;

                case PosSizableRect.RightUp:
                    return Cursors.SizeNESW;

                case PosSizableRect.RightBottom:
                    return Cursors.SizeNWSE;

                case PosSizableRect.RightMiddle:
                    return Cursors.SizeWE;

                case PosSizableRect.UpMiddle:
                    return Cursors.SizeNS;
                default:
                    return Cursors.Default;
            }
        }

    }
}
