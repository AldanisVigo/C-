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

namespace SerialPortExample{
	class MainClass{
		//Windows API call to hide Console Windows
		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		[DllImport("kernel32")]
		public static extern IntPtr GetConsoleWindow();
		[DllImport("Kernel32")]
		private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);


		static ServoDisplay MyServoDisplay;
		static System.Windows.Forms.Form MyWindow;
		static System.Windows.Forms.Button ConnectButton,baudMinus,baudPlus;
		static System.Windows.Forms.ComboBox PortList;
		static System.Windows.Forms.TextBox txtBaud;
		static System.Windows.Forms.Timer myTimer;
		static System.Windows.Forms.ListBox termDisplay;
		//static Graphics myGraphics;

		static String[] ports;
		static SerialPort myPort;
		static char baudupdownflag;
		static string incoming_data;
		public static void Main (string[] args){     
			//Remove this code to show Console Window
			IntPtr hConsole = GetConsoleWindow();
			if (IntPtr.Zero != hConsole){
				ShowWindow(hConsole,0); 
			}
			Image servoKnob = Bitmap.FromFile ("servohead.png");
			MyServoDisplay = new ServoDisplay (servoKnob);
			ports = System.IO.Ports.SerialPort.GetPortNames();
			InitWindow();
		}
		public static void myPort_DataReceived(Object sender, SerialDataReceivedEventArgs e){
			incoming_data = myPort.ReadLine();
			try{
				termDisplay.Invoke (new MethodInvoker(delegate(){
					termDisplay.Items.Add("Servo Angle:" + incoming_data + "°");
					termDisplay.SelectedIndex = termDisplay.Items.Count - 1;
				}));

			}catch(Exception ex){
				MessageBox.Show(ex.Message.ToString());
			}
			int num;
			bool isNum = int.TryParse (incoming_data,out num);
			if(isNum){
				MyServoDisplay.setAngle = int.Parse(incoming_data);
			}
			Console.WriteLine("Angle:" + incoming_data);

		}
		public static void InitWindow(){
			//Main Window
			MyWindow = new System.Windows.Forms.Form ();
			MyWindow.Text = "Serial Hardware Control";
			MyWindow.FormClosed += new FormClosedEventHandler (MyWindow_Close);
			//Port List ComboBox
			PortList = new System.Windows.Forms.ComboBox();
			PortList.Items.AddRange (ports);
			try{
				PortList.Text = ports [0];
			}catch(System.IndexOutOfRangeException){
				//Do nothing
				MessageBox.Show("There are no available serial ports on your computer.");
				return;
			}
			PortList.Location = new System.Drawing.Point (10, 5);
		
			//Click Timer Setup
			myTimer = new System.Windows.Forms.Timer();
			myTimer.Interval = 100;
			myTimer.Enabled = false;
			myTimer.Tick += new EventHandler(myTimerEvent);

			//BaudPicker
			//Decrease Baud
			baudMinus = new System.Windows.Forms.Button();
			baudMinus.Text = "-";
			baudMinus.MouseDown += new MouseEventHandler (DecreaseBaud);
			baudMinus.MouseUp += new MouseEventHandler (StopMyTimer);
			baudMinus.Location = new System.Drawing.Point (PortList.Location.X + PortList.Width + 10,5);
			baudMinus.Width = baudMinus.Width / 2;
			baudMinus.Font = new Font ("Impact", 20f);

			//Display Baud
			txtBaud = new System.Windows.Forms.TextBox();
			txtBaud.Text = "9600";
			txtBaud.Location = new System.Drawing.Point (baudMinus.Location.X + baudMinus.Width + 10,5);
			txtBaud.TextAlign = HorizontalAlignment.Center;

			//Increase Baud
			baudPlus = new System.Windows.Forms.Button();
			baudPlus.Text = "+";
			baudPlus.MouseDown += new MouseEventHandler(IncreaseBaud);
			baudPlus.MouseUp += new MouseEventHandler (StopMyTimer);
			baudPlus.Location = new System.Drawing.Point(txtBaud.Location.X + txtBaud.Width + 10,5);
			baudPlus.Width = baudPlus.Width / 2;
			baudPlus.Font = new Font ("Impact", 20f);


			//Connect Button
			ConnectButton = new System.Windows.Forms.Button();
			ConnectButton.Text = "Connect";
			ConnectButton.Font = new Font("Impact", 9f);
			ConnectButton.Location  = new System.Drawing.Point(baudPlus.Location.X + baudPlus.Width + 10,5);
			ConnectButton.Click += new EventHandler(ConnectButton_Click);



			//termDisplay Label
			termDisplay = new System.Windows.Forms.ListBox();
			termDisplay.Location = new System.Drawing.Point(PortList.Location.X, ConnectButton.Location.Y + 30);
			termDisplay.ForeColor = Color.LightGreen;
			termDisplay.BackColor = Color.Black;
			termDisplay.Height = 400;
			termDisplay.Items.Add((PortList.Text == String.Empty?"NOPORT":PortList.Text) + "$>You need to press Connect!");

			//Set Window and Terminal ListBox Size
			MyWindow.Size = new System.Drawing.Size(PortList.Location.X + PortList.Width + 10 + baudMinus.Width + 10 + txtBaud.Width + 10 + baudPlus.Width + 10 + ConnectButton.Width + 25 + MyWindow.Width,termDisplay.Location.Y + termDisplay.Height + 49);
			termDisplay.Width = MyWindow.Size.Width - 36;

			//Combine Controls
			MyWindow.Controls.Add(ConnectButton);
			MyWindow.Controls.Add(PortList);
			MyWindow.Controls.Add(baudMinus);
			MyWindow.Controls.Add(baudPlus);
			MyWindow.Controls.Add(txtBaud);
			MyWindow.Controls.Add(termDisplay);

			//Show and Run
			MyServoDisplay.Show();
			MyWindow.Show();
			Application.Run();
		}
		private static void DecreaseBaud(object sender,MouseEventArgs e){
			baudupdownflag = 'd';	//Decrease flag set
			myTimer.Enabled = true; //Start the timer
		}
		private static void StopMyTimer(object sender,MouseEventArgs e){
			myTimer.Enabled = false; //Stop the timer
		}
		static void myTimerEvent (object sender,EventArgs e){
			int baud = Convert.ToInt16 (txtBaud.Text);
			if(baudupdownflag == 'u'){
				baud++;
			}
			if (baudupdownflag == 'd'){
				baud--;
			}
			txtBaud.Text = baud.ToString();
		}
		static void IncreaseBaud(object sender, EventArgs e){
			baudupdownflag = 'u';	//Increase flag set
			myTimer.Enabled = true; //Start the timer
		}
		static void MyWindow_Close(object sender, FormClosedEventArgs e){
			MyServoDisplay.Dispose();
			MyWindow.Dispose();
			System.Environment.Exit(1);
		}
		//Connect Button Click Event Handler
		static void ConnectButton_Click(object sender,EventArgs e){
			if(ConnectButton.Text == "Connect"){
				MessageBox.Show ("Connecting to " + PortList.Text + " at " + txtBaud.Text);
				myPort = new SerialPort (PortList.Text, Convert.ToInt16 (txtBaud.Text), Parity.None, 8, StopBits.One);	//Create a port handle
				try{
					if(!myPort.IsOpen){
						myPort.Open();
						if (myPort.IsOpen){
							MessageBox.Show("Successfuly opened the " + PortList.Text + "!"); 
							PortList.Enabled = false;
							txtBaud.Enabled = false;
							baudMinus.Enabled = false;
							baudPlus.Enabled = false;
							ConnectButton.Text = "Disconnect";
							myPort.DataReceived += new SerialDataReceivedEventHandler(myPort_DataReceived); //Assign the received data handler
						}else {
							MessageBox.Show("There was an error opening the " + PortList.Text + " port!");
						}
					}else{
						MessageBox.Show("The " + PortList.Text + " port is already open!");
					}
				}catch(Exception ew){
					MessageBox.Show(ew.Message.ToString());
				}
			}else if(ConnectButton.Text == "Disconnect"){
				try{
					if(myPort.IsOpen){
						String portName = myPort.PortName.ToString();
						myPort.Close();
						if(!myPort.IsOpen){
							MessageBox.Show("Successfuly disconnected from the " + portName + " port.");
							PortList.Enabled = true;
							txtBaud.Enabled = true;
							baudMinus.Enabled = true;
							baudPlus.Enabled = true;
							ConnectButton.Text = "Connect";
							termDisplay.Items.Clear();
							termDisplay.Items.Add((PortList.Text == String.Empty?"NOPORT":PortList.Text) + "$>You need to press Connect!");
						}else{
							MessageBox.Show("There was a problem disconnecting from the " + portName + "port.Please try again.");
						}
					}
				}catch(Exception ex){
					MessageBox.Show(ex.Message.ToString());
				}
			}
		}
	}
}
