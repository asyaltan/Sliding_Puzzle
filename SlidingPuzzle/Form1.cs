using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.Numerics;
using System.Drawing;

namespace SlidingPuzzle
{
    public partial class Form1 : Form
    {
        #region Global Variables

        public const double PI = Math.PI;

        public Point[] pts;
        public PictureBox[] pictureBoxes;
        int emptyIndex = 8;
        Image emptyImage;
        public Vector2 downPos, upPos;
        public int[] indexes = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        public int index = 0;
        public string baseUrl;
        
        bool isShuffling = false;

        int remainingTime = 60;
        int remainingMoves = 50;
        //public string baseUrl  = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "baseImage2.jpeg");

        #endregion

        public Form1()
        {
            InitializeComponent();
            pts = new Point[] { pictureBox0.Location, pictureBox1.Location, pictureBox2.Location, pictureBox3.Location, pictureBox4.Location, pictureBox5.Location, pictureBox6.Location, pictureBox7.Location, pictureBox8.Location };

            Random a = new Random();
            int randomNum = a.Next(1, 6);

            baseUrl = Environment.CurrentDirectory + "\\..\\..\\..\\Images\\I" + randomNum.ToString() + ".jpeg";


            SetUp();
        }

        #region Mouse Events
        private void box_MouseDown(object sender, MouseEventArgs e)
        {
            downPos = new Vector2(e.X, e.Y);
            index = Array.IndexOf(indexes, Array.IndexOf(pictureBoxes, sender));

        }
        private void box_MouseUp(object sender, MouseEventArgs e)
        {
            upPos = new Vector2(e.X, e.Y);

            Direction direction = getMouseDirection(downPos, upPos);
            //MessageBox.Show(direction.ToString());

            swap(index, direction);

        }
        #endregion

        #region Helper Functions
        Direction getMouseDirection(Vector2 down, Vector2 up)
        {
            Vector2 dir = up - down;
            var angle = Math.Atan2(dir.Y, dir.X);// Math.Acos((up.X * down.X + up.Y * down.Y) / Math.Sqrt((up.X * up.X + up.Y + up.Y) * (down.X * down.X + down.Y * down.Y)));
            var sect = PI / 4;

            if (angle < 0)
                angle += 2 * PI;

            if (angle <= 1 * sect || angle > 7 * sect)
            {
                return Direction.Right;
            }

            if (angle <= 3 * sect)
            {
                return Direction.Down;
            }

            if (angle <= 5 * sect)
            {
                return Direction.Left;
            }

            return Direction.Up;
        }

        void swap(int index, Direction direction)
        {
            int newIndex = -1;

            switch (direction)
            {
                case Direction.Right:
                    if (index % 3 != 2)
                        newIndex = index + 1;
                    break;
                case Direction.Down:
                    if (index < 6)
                        newIndex = index + 3;
                    break;
                case Direction.Left:
                    if (index % 3 != 0)
                        newIndex = index - 1;
                    break;
                case Direction.Up:
                    if (index > 2)
                        newIndex = index - 3;
                    break;
            }

            if (newIndex == emptyIndex)
            {
                emptyIndex = index;

                int temp = indexes[index];
                indexes[index] = indexes[newIndex];
                indexes[newIndex] = temp;

                pictureBoxes[indexes[index]].Location = pts[index];
                pictureBoxes[indexes[newIndex]].Location = pts[newIndex];

                CheckWin();

                if(isShuffling)
                    return;


                if(remainingMoves <= 0)
                {
                    EndGame(false);
                }
                else
                {
                    remainingMoves--;

                    countLabel.Text = remainingMoves.ToString();
                }
            }
        }

        void CheckWin()
        {
            for (int i = 0; i < 9; i++)
            {
                if (indexes[i] != i)
                {
                    return;
                }
            }

            EndGame(true);
        }

        void EndGame(bool win)
        {
            DialogResult a;

            if (win)
            {
                pictureBoxes[8].Image = emptyImage;

                a = MessageBox.Show("You Win!", "Congratulations", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Information);
            }
            else
            {
                a = MessageBox.Show("You Lose!", "Game Over", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Information);
            }

            switch (a)
            {
                case DialogResult.Cancel:
                    Application.Exit();
                    break;
                case DialogResult.TryAgain:
                    SetUp();
                    break;
                case DialogResult.Continue:
                    foreach (Control c in this.Controls)
                    {
                        c.Enabled = false;

                        if (c is Button btn && btn == button1)
                        {
                            c.Enabled = true;
                        }
                    }
                    break;
                default:
                    break;
            }

            timer1.Stop();
        }

        void CutImage(string url)
        {
            Bitmap original = new Bitmap(url);

            int h = original.Height / 3;
            int w = original.Width / 3;

            Rectangle section0 = new Rectangle(0, 0, w, h);
            Rectangle section1 = new Rectangle(w, 0, w, h);
            Rectangle section2 = new Rectangle(2 * w, 0, w, h);
            Rectangle section3 = new Rectangle(0, h, w, h);
            Rectangle section4 = new Rectangle(w, h, w, h);
            Rectangle section5 = new Rectangle(2 * w, h, w, h);
            Rectangle section6 = new Rectangle(0, 2 * h, w, h);
            Rectangle section7 = new Rectangle(w, 2 * h, w, h);
            Rectangle section8 = new Rectangle(2 * w, 2 * h, w, h);

            pictureBoxes[0].Image = original.Clone(section0, original.PixelFormat);
            pictureBoxes[1].Image = original.Clone(section1, original.PixelFormat);
            pictureBoxes[2].Image = original.Clone(section2, original.PixelFormat);
            pictureBoxes[3].Image = original.Clone(section3, original.PixelFormat);
            pictureBoxes[4].Image = original.Clone(section4, original.PixelFormat);
            pictureBoxes[5].Image = original.Clone(section5, original.PixelFormat);
            pictureBoxes[6].Image = original.Clone(section6, original.PixelFormat);
            pictureBoxes[7].Image = original.Clone(section7, original.PixelFormat);
            emptyImage = original.Clone(section8, original.PixelFormat);

            pictureBoxes[8].Image = null;
            pictureBoxes[8].BackColor = Color.Black;

        }

        void Shuffle()
        {
            isShuffling = true;

            Random rand = new Random();
            for (int i = 0; i < 5000; i++)
            {
                int r = rand.Next(4);
                swap(rand.Next(8), (Direction)r);
            }

            isShuffling = false;
        }

        void SetUp()
        {
            isShuffling = true;

            pictureBoxes = new PictureBox[] { pictureBox0, pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8 };
            //pts = new Point[] { pictureBox0.Location, pictureBox1.Location, pictureBox2.Location, pictureBox3.Location, pictureBox4.Location, pictureBox5.Location, pictureBox6.Location, pictureBox7.Location, pictureBox8.Location };
            indexes = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            emptyIndex = 8;
            index = 0;

            foreach (Control c in this.Controls)
            {
                c.Enabled = true;
            }

            remainingTime = 60;
            timer1.Start();

            remainingMoves = 50;
            countLabel.Text = remainingMoves.ToString();
            
            CutImage(baseUrl);
            Shuffle();

            this.Enabled = true;
            timer1.Start();
        }



        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog filePicker = new OpenFileDialog();

            filePicker.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            DialogResult result = filePicker.ShowDialog();

            if (result == DialogResult.OK)
            {
                baseUrl = filePicker.FileName;
                SetUp();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (remainingTime <= 0)
            {
                timer1.Stop();
                EndGame(false);
            }
            else
            {
                remainingTime--;
            }

            TimeSpan time = TimeSpan.FromSeconds(remainingTime);
            timerLabel.Text = time.ToString(@"mm\:ss");
        }

        enum Direction
        {
            Right,
            Up,
            Left,
            Down
        }
    }
}
