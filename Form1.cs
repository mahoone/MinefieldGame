using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;




namespace tryAgain
{
    public partial class Form1 : Form
    {

        //function to return the Label that is at location (X,Y)
        private Label getLabel(int x, int y)
        {
            int k = (y - 1) * 20 + x;
            string s = "label" + k.ToString();

            foreach (Control c in panel1.Controls)
            {
                if (c.GetType() == typeof(System.Windows.Forms.Label))
                {
                    if (c.Name == s)
                    {
                        return (Label)c;
                    }
                }
            }
            return null;

        }


        //Variables
        bool[,] bombs = new bool[21, 21];
        int atX = 10;
        int atY = 20;

        //Show bombs
        private void showBombs()
        {
            Label lbl;
            for (int y = 1; y < 21; y++)
            {
                for (int x = 1; x < 21; x++)
                {
                    lbl = getLabel(x, y);
                    if (bombs[x, y])
                    {
                        lbl.Image = Properties.Resources.mine11;
                        lbl.BackColor = Color.Yellow;
                    }
                    else
                    {
                        lbl.BackColor = Color.SkyBlue;
                    }
                }
            }
        }

        //Place mines
        private void placeBombs(int target)
        {
            //create a random number generator
            Random r = new Random();

            //set up variables
            int x;
            int y;
            int k = target;

            //Clear current array storing mines
            Array.Clear(bombs, 0, bombs.Length);


            //loop to fill with desired number of mines
            do
            {
                x = r.Next(1, 20);
                y = r.Next(1, 20);
                if (!bombs[x, y])
                {
                    bombs[x, y] = true;
                    k--;
                }
            } while (k > 0);
        }
        //
        //Difficulty level
        //
        private void btnEasy_CheckedChanged(object sender, EventArgs e)
        {
            if (btnEasy.Checked)
            {
                placeBombs(40);
            }
            else if (btnMedium.Checked)
            {
                placeBombs(60);
            }
            else if (btnHard.Checked)
            {
                placeBombs(80);
            }

        }

        //
        //Count and show adjacent mines

        private void countBombs(int X, int Y)
        {
            int count = 0;
            int newx;
            int newy;

            newx = X - 1;
            if (newx > -1)
            {
                if (bombs[newx, Y])
                    count++;
            }

            newx = X + 1;
            if (newx < 21)
            {
                if (bombs[newx, Y])
                    count++;
            }

            newy = Y - 1;
            if (newy > -1)
            {
                if (bombs[X, newy])
                    count++;
            }

            newy = Y + 1;
            if (newy < 21)
            {
                if (bombs[X, newy])
                    count++;
            }
            label401.Text = count.ToString();
        }

        //Check for bombs at current location

        private void chkbomb(int X, int Y)
        {
            if (bombs[X, Y])
            {
                this.BackColor = Color.Yellow;
                //end of game
                btnDown.Enabled = false;
                btnUp.Enabled = false;
                btnRight.Enabled = false;
                btnLeft.Enabled = false;
                showBombs();
                timer.Stop();

                //Yes No Pop up window. 
                DialogResult playerChoice = MessageBox.Show("New Game?", "Game Over!", MessageBoxButtons.YesNo);
                if (playerChoice == DialogResult.Yes)
                {
                    //Restart Application if Yes selected
                    Application.Restart();
                }
                else if (playerChoice == DialogResult.No)
                {
                    //Close Application if No selected
                    this.Close();
                }
            }
            else
            {
                //count bombs around current location
                countBombs(X, Y);
            }
        }

        //Function to draw spite
        private void drawsprite(int x, int y)
        {
            // Function to draw spite at location (x, y)
            Label lbl = getLabel(atX, atY);
            lbl.BackColor = Color.White;
            lbl.Image = Properties.Resources.carFace;
        }

        //function to undraw the sprite at location (x,y)
        private void wipesprite(int x, int y)
        {
            Label lbl = getLabel(atX, atY);
            lbl.Image = null;
        }

        //Save Player Name
        string playerName = "";
        private void btnSavePlayer_Click(object sender, EventArgs e)
        {
            if (txtPlayerName.Text == "")
            {
                MessageBox.Show("Enter your player name");
            }
            else
            {
                playerName = txtPlayerName.Text;
                MessageBox.Show("Player " + playerName + " added");
            }
        }

        //
        //Timer
        //
        Timer timer = new Timer();
        int seconds = 0;
        DateTime dt = new DateTime();
        //
        //
        //
        public Form1()
        {
            InitializeComponent();

            //make Bombs - By default set to Difficulty level -  Easy
            placeBombs(40);
            //place the sprite at its start-up position
            drawsprite(atX, atY);
            

            //
            //Timer
            //

            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            timer.Interval = 1000;            // Timer will tick evert second
            timer.Enabled = false;                       // Enable the timer
                               // Start the timer


            lblTimer.AutoSize = true;
            lblTimer.Text = String.Empty;

            this.Controls.Add(lblTimer);

        }

            private void timer_Tick(object sender, EventArgs e)
            {
                seconds++;
                lblTimer.Text = dt.AddSeconds(seconds).ToString("mm:ss");
            }
    

        //
        // Button to move Spite Up
        //
        private void btnUp_Click(object sender, EventArgs e)
        {
            //Enable timer after clicking any of the arrow keys (up,down etc).
            timer.Enabled = true;
            
            //Prevent icon going off the grid
            if (atY < 2)
            {
                //Save score if game completed
                string fileName = @"C:\Users\Kamil\source\repos\tryAgain\Resources\HighScore.txt";
                using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(playerName + " " + " at " + DateTime.Now + " --- Your score: " + seconds);
                }

                //Ask user for input
                DialogResult playerChoice = MessageBox.Show("Your time: " + seconds + " !" + "\nNew Game?", "You Won!", MessageBoxButtons.YesNo);
                if (playerChoice == DialogResult.Yes)
                {
                    //Restart Application if Yes selected
                    Application.Restart();
                }
                else if (playerChoice == DialogResult.No)
                {
                    //Close Application if No selected
                    this.Close();
                }
            }
            else
            {
                
                //delete sprite at current location
                wipesprite(atX, atY);
                //move up by one row
                atY--;
                //draw sprite at current location
                drawsprite(atX, atY);
                //check for bombs
                chkbomb(atX, atY);
            }
        }

        //
        // Button to move Spite Down
        private void btnDown_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
            if (atY > 19)
            {
                MessageBox.Show("Too far down");
            }
            else { 
                //delete sprite at current location
                wipesprite(atX, atY);
                //move up by one row
                atY++;
                //draw sprite at current location
                drawsprite(atX, atY);
                //check for bombs
                chkbomb(atX, atY);
            }
        }

      

        private void Window_KeyDown_Click(object sender, EventArgs e) { }

        //
        // Button to move Spite Right
        private void btnRight_Click_1(object sender, EventArgs e)
        {
            timer.Enabled = true;
            if (atX > 19)
            {
                MessageBox.Show("Too far right");
            }
            else
            {
                //delete sprite at current location
                wipesprite(atX, atY);
                //move up by one row
                atX++;
                //draw sprite at current location
                drawsprite(atX, atY);
                //check for bombs
                chkbomb(atX, atY);
            }
        }

        //
        // Button to move Spite Left
        private void btnLeft_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
            if (atX < 2)
            {
                MessageBox.Show("Too far left");
            }
            else { 
            //delete sprite at current location
            wipesprite(atX, atY);
                //move up by one row
                atX--;
                //draw sprite at current location
                drawsprite(atX, atY);
                //check for bombs
                chkbomb(atX, atY);
            }
        }

        

        //
        //.txt Help File
        //
        private void btnHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", @"C:\Users\Kamil\source\repos\tryAgain\Resources\README.txt");
        }


        //
        //Open HighScore text file
        //
        private void btnHS_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", @"C:\Users\Kamil\source\repos\tryAgain\Resources\HighScore.txt");
        }

       








        //Generate text file 

        /*private void button1_Click_1(object sender, EventArgs e)
        {

            using (StreamWriter writer = new StreamWriter("C:\\Users\\Kamil\\Desktop\\mineswpeerAll\\Minefield\\any1.txt"))
            {
                for (int n = 1; n < 401; n++)
                {
                    writer.WriteLine("this.label" + n.ToString() + " = new System.Windows.Forms.Label();");
                }
                for (int n = 400; n > 0; n--)
                {
                    writer.WriteLine("this.panel1.Controls.Add(this.label" + n.ToString() + ");");
                }
                for (int n = 1; n < 401; n++)
                {
                    writer.WriteLine("private System.Windows.Forms.Label label" + n.ToString() + ";");
                }
                {
                    int x = -20;
                    int y = 0;

                    for (int n = 1; n < 401; n++)
                    {
                        x += 20;
                        if (x == 400)
                        {
                            x = 0;
                            y += 20;
                        }

                        writer.WriteLine("//");
                        writer.WriteLine("// label " + n.ToString());
                        writer.WriteLine("//");
                        writer.WriteLine("this.label" + n.ToString() + ".BackColor = System.Drawing.Color.SkyBlue;");
                        writer.WriteLine("this.label" + n.ToString() + ".Location = new System.Drawing.Point(" + x.ToString() + ", " + y.ToString() + ");");
                        writer.WriteLine("this.label" + n.ToString() + ".Name = \"label" + n.ToString() + "\";");
                        writer.WriteLine("this.label" + n.ToString() + ".Size = new System.Drawing.Size(20, 20);");
                        writer.WriteLine("this.label" + n.ToString() + ".TabIndex = " + n.ToString() + ";");
                    }

                }
            }
        }*/
    }
}
