/*
 * Author: Aldanis Vigo
 * Date: 12/28/2015
 * 
 * 
 * 	Before you run this, make sure you place the servohead.png file inside the SerialPortExample/bin/Debug folder. You can also put whatever image you want and just change the name to 
 *  servohead.png.
*/

using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Drawing;
using System.Timers;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Reflection;

namespace SerialPortExample
{
	class ServoDisplay : Form{
		public Image knob{ get; set; }
		private int dAngle;
		private System.Windows.Forms.Label AngleValue;
		private System.Windows.Forms.Timer updateTimer;
		public int setAngle{
			get{
				return this.dAngle;
			}
			set{ 
				this.dAngle = value;
			}
		}
		public ServoDisplay(Image knobImg){
			//Constructor
			Icon = Icon.ExtractAssociatedIcon("C:/Users/aldanisvigo/Pictures/servohead.ico");
			Text = "Servo Simulator";
			updateTimer = new System.Windows.Forms.Timer();
			updateTimer.Interval = 100;
			updateTimer.Tick += new EventHandler (Redraw_Form);
			updateTimer.Enabled = true;
			knob = knobImg;
			Width = knob.Width + 20;
			Height = knob.Height + 40;
			AngleValue = new Label ();
			AngleValue.Text = dAngle.ToString();
			AngleValue.Location = new Point (0, 0);
			AngleValue.Font = new Font ("Impact", 15.8f);
			AngleValue.Width = 60;
			Controls.Add (AngleValue);
			this.DoubleBuffered = true;
		}
		private void Redraw_Form(Object sender, EventArgs e){
			this.Invalidate ();
		}
		protected override void OnPaint(PaintEventArgs e){
			if (knob != null) {

				Bitmap newKnob = RotateImage ((Bitmap)knob,dAngle);
				e.Graphics.DrawImage (newKnob,new Point(0,0));

				AngleValue.Text = dAngle.ToString () + "°";
				//Refresh ();
			} 
		}
		private Bitmap RotateImage(Bitmap b, float angle)
		{
			//Create a new empty bitmap to hold rotated image.
			Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
			//Make a graphics object from the empty bitmap.
			Graphics g = Graphics.FromImage(returnBitmap);
			//move rotation point to center of image.
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.TranslateTransform((float) b.Width / 2, (float)b.Height / 2);
			//Rotate.        
			g.RotateTransform(angle);
			//Move image back.
			g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
			//Draw passed in image onto graphics object.
			g.DrawImage(b, new Point(0, 0));
			return returnBitmap;
		}
	}
}

