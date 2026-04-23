using System;
using System.IO;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace SlidingPuzzle
{
    public partial class Form1 : Form
    {
        #region Değişkenler
        public Label lblTime = null!;
        public Label lblMoves = null!;
        public Point[] pts = null!;
        public PictureBox[] pictureBoxes = null!;
        public int[] indexes = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        public int emptyIndex = 8;
        public Vector2 downPos, upPos;
        public int index;
        public string baseUrl = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "baseImage2.jpeg");

        public int moveCount = 0;
        public bool isGameOver = false;
        public bool isAnimating = false;
        public Image? lastPiece;

        private System.Windows.Forms.Timer gameTimer = null!;
        private int secondsElapsed = 0;
        private System.Windows.Forms.Timer animTimer = null!;
        private PictureBox? movingBox;
        private Point targetPos;
        private const int animSpeed = 25;
        #endregion

        public Form1()
        {
            InitializeComponent();

            // 1. Arayüz elemanlarını ve sistemleri hazırla
            SetupGameSystem();

            // 2. Olayları kodla bağla
            LinkEventsDynamically();

            // 3. Başlangıçta varsayılan resmi yükle
            this.Load += (s, e) => {
                if (File.Exists(baseUrl))
                {
                    CutImage(baseUrl);
                    Shuffle();
                }
                else
                {
                    MessageBox.Show("Masaüstünde 'baseImage2.jpeg' bulunamadı.\nLütfen 'Resim Seç' butonuyla bir resim yükleyin.");
                }
            };
        }

        private void SetupGameSystem()
        {
            pictureBoxes = new PictureBox[] { pictureBox0, pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8 };
            pts = new Point[] { pictureBox0.Location, pictureBox1.Location, pictureBox2.Location, pictureBox3.Location, pictureBox4.Location, pictureBox5.Location, pictureBox6.Location, pictureBox7.Location, pictureBox8.Location };

            int maxBottom = 0;
            foreach (var pb in pictureBoxes)
                if (pb.Bottom > maxBottom) maxBottom = pb.Bottom;

            lblTime = new Label
            {
                Location = new Point(pictureBox0.Left, maxBottom + 20), 
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                Text = "Süre: 0 sn"
            };

            lblMoves = new Label
            {
                Location = new Point(pictureBox0.Left + 200, maxBottom + 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Text = "Hamle: 0"
            };

            this.Controls.Add(lblTime);
            this.Controls.Add(lblMoves);
            lblTime.BringToFront();
            lblMoves.BringToFront();

            this.Height = lblTime.Bottom + 100;
            this.Width = Math.Max(this.Width, 600);

            gameTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            gameTimer.Tick += (s, e) => { secondsElapsed++; lblTime.Text = $"Süre: {secondsElapsed} sn"; };
            animTimer = new System.Windows.Forms.Timer { Interval = 10 };
            animTimer.Tick += AnimTimer_Tick!;
        }

        private void LinkEventsDynamically()
        {
            foreach (var pb in pictureBoxes)
            {
                pb.MouseDown += box_MouseDown!;
                pb.MouseUp += box_MouseUp!;
            }
        }

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            if (movingBox == null) return;
            int dx = targetPos.X - movingBox.Left;
            int dy = targetPos.Y - movingBox.Top;

            if (Math.Abs(dx) <= animSpeed && Math.Abs(dy) <= animSpeed)
            {
                movingBox.Location = targetPos;
                animTimer.Stop();
                isAnimating = false;
                if (CheckWin()) FinishGame();
            }
            else
            {
                movingBox.Left += Math.Sign(dx) * animSpeed;
                movingBox.Top += Math.Sign(dy) * animSpeed;
            }
        }

        void swap(int index, Direction direction, bool animate = true)
        {
            if (isGameOver || isAnimating) return;
            int newIndex = -1;
            switch (direction)
            {
                case Direction.Right: if (index % 3 != 2) newIndex = index + 1; break;
                case Direction.Down: if (index < 6) newIndex = index + 3; break;
                case Direction.Left: if (index % 3 != 0) newIndex = index - 1; break;
                case Direction.Up: if (index > 2) newIndex = index - 3; break;
            }

            if (newIndex == emptyIndex)
            {
                if (!gameTimer.Enabled && animate) gameTimer.Start();
                movingBox = pictureBoxes[indexes[index]];
                targetPos = pts[newIndex];

                int temp = indexes[index];
                indexes[index] = indexes[newIndex];
                indexes[newIndex] = temp;
                emptyIndex = index;

                if (animate)
                {
                    pictureBoxes[indexes[index]].Location = pts[index];
                    isAnimating = true;
                    animTimer.Start();
                    moveCount++;
                    lblMoves.Text = $"Hamle: {moveCount}";
                }
                else
                {
                    if (movingBox != null) movingBox.Location = targetPos;
                    pictureBoxes[indexes[index]].Location = pts[index];
                }
            }
        }

        bool CheckWin()
        {
            for (int i = 0; i < indexes.Length; i++)
                if (indexes[i] != i) return false;
            return true;
        }

        void FinishGame()
        {
            isGameOver = true;
            gameTimer.Stop();
            pictureBox8.Image = lastPiece;
            MessageBox.Show($"Tebrikler! {moveCount} hamle ve {secondsElapsed} saniyede bitirdin!");
        }

        void Shuffle()
        {
            Random rand = new Random();
            for (int i = 0; i < 400; i++)
                swap(rand.Next(9), (Direction)rand.Next(4), false);
            moveCount = 0; secondsElapsed = 0; isGameOver = false;
            lblMoves.Text = "Hamle: 0"; lblTime.Text = "Süre: 0 sn";
        }

        private void box_MouseDown(object sender, MouseEventArgs e)
        {
            downPos = new Vector2(e.X, e.Y);
            index = Array.IndexOf(indexes, Array.IndexOf(pictureBoxes, (PictureBox)sender));
        }

        private void box_MouseUp(object sender, MouseEventArgs e)
        {
            if (isAnimating) return;
            upPos = new Vector2(e.X, e.Y);
            if (Vector2.Distance(downPos, upPos) < 10) return;
            Vector2 dir = upPos - downPos;
            double angle = Math.Atan2(dir.Y, dir.X);
            if (angle < 0) angle += 2 * Math.PI;
            double sect = Math.PI / 4;
            Direction d = (angle <= 1 * sect || angle > 7 * sect) ? Direction.Right : (angle <= 3 * sect) ? Direction.Down : (angle <= 5 * sect) ? Direction.Left : Direction.Up;
            swap(index, d);
        }

        void CutImage(string url)
        {
            if (!File.Exists(url)) return;
            Bitmap original = new Bitmap(url);
            int w = original.Width / 3, h = original.Height / 3;
            for (int i = 0; i < 8; i++)
            {
                Rectangle rect = new Rectangle((i % 3) * w, (i / 3) * h, w, h);
                pictureBoxes[i].Image = original.Clone(rect, original.PixelFormat);
            }
            lastPiece = original.Clone(new Rectangle(2 * w, 2 * h, w, h), original.PixelFormat);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    baseUrl = ofd.FileName;
                    CutImage(baseUrl);
                    Shuffle();
                }
            }
        }

        public enum Direction { Right, Up, Left, Down }
    }
}