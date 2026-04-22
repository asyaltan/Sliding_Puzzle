using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.Numerics;
using System.Drawing;

namespace SlidingPuzzle
{
    public partial class Form1 : Form
    {
        #region Gloabl Variables

        public const double PI = Math.PI;

        public Point[] pts;
        public PictureBox[] pictureBoxes;
        int emptyIndex = 8;
        public Vector2 downPos, upPos;
        public int[] indexes = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        public int index = 0;
        public string baseUrl  = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "baseImage2.jpeg");

        #endregion

        public Form1()
        {
            InitializeComponent();

            pts = new Point[] { pictureBox0.Location, pictureBox1.Location, pictureBox2.Location, pictureBox3.Location, pictureBox4.Location, pictureBox5.Location, pictureBox6.Location, pictureBox7.Location, pictureBox8.Location };
            pictureBoxes = new PictureBox[] { pictureBox0, pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8 };
            CutImage(baseUrl);
            Shuffle();
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
            }

            //MessageBox.Show(string.Join(", ", pictureBoxes));
        }

        void CutImage(string url)
        {
            Bitmap original = new Bitmap(url);

            int h = original.Height / 3;
            int w = original.Width / 3;
            
            Rectangle section0 = new Rectangle(0, 0, w ,h);
            Rectangle section1 = new Rectangle(w, 0, w, h);
            Rectangle section2 = new Rectangle(2*w, 0, w, h);
            Rectangle section3 = new Rectangle(0, h, w, h);
            Rectangle section4 = new Rectangle(w, h, w, h);
            Rectangle section5 = new Rectangle(2*w, h, w, h);
            Rectangle section6 = new Rectangle(0, 2*h, w, h);
            Rectangle section7 = new Rectangle(w, 2*h, w, h);
            
            List<int> pieces = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
            
            pictureBoxes[0].Image = original.Clone(section0, original.PixelFormat);
            pictureBoxes[1].Image = original.Clone(section1, original.PixelFormat);
            pictureBoxes[2].Image = original.Clone(section2, original.PixelFormat);
            pictureBoxes[3].Image = original.Clone(section3, original.PixelFormat);
            pictureBoxes[4].Image = original.Clone(section4, original.PixelFormat);
            pictureBoxes[5].Image = original.Clone(section5, original.PixelFormat);
            pictureBoxes[6].Image = original.Clone(section6, original.PixelFormat);
            pictureBoxes[7].Image = original.Clone(section7, original.PixelFormat);
            
        }

        void Shuffle()
        {
            Random rand = new Random();
            for (int i = 0; i < 5000; i++)
            {
                int r = rand.Next(4);
                swap(rand.Next(8), (Direction)r);
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            baseUrl = urlBox.Text;
            
            CutImage(baseUrl);
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
