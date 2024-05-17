using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Resources;
using System.Media;

namespace PongEvolved
{
    public partial class Form1 : Form
    {
        Random rand = new Random();
        Rectangle player = new Rectangle(0, 0, 30, 30);
        Rectangle CPU = new Rectangle(0, 0, 30, 30);
        Rectangle ball = new Rectangle(0, 0, 15, 15);
        SolidBrush brush = new SolidBrush(Color.White);

        SoundPlayer hit = new SoundPlayer(Properties.Resources.hit_Sound);
        SoundPlayer jumpSound = new SoundPlayer(Properties.Resources.jump_Sound);
        SoundPlayer score = new SoundPlayer(Properties.Resources.score_Sound);
        SoundPlayer winner = new SoundPlayer(Properties.Resources.win_Sound);

        bool playerJumping = false;
        bool CPUJumping = false;

        int playerScore = 0;
        int CPUscore = 0;
        int gravity = 8;
        int ballBounce = 4;

        double JumpPower = -40;
        double playerVelocity = 0;
        double CPUvelocity = 0;
        double ballVelocityX =20;
        double ballVelocityY = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            ScoreLabel.Text = $"{playerScore}:{CPUscore}";
            
            if (ball.Y + ball.Height >= this.Height)
            {
                ballVelocityY = -(ballVelocityY + ballBounce)/1.2;
                ball.Y = this.Height - ball.Height;
                new SoundPlayer(Properties.Resources.hit_Sound).Play();
                if (ballVelocityX > 0)
                {
                    ballVelocityX -= ballVelocityX / 10;
                }
                else if (ballVelocityX < 0)
                {
                    ballVelocityX += ballVelocityX / 10;
                }
            }
            if (ball.Y <= 0)
            {
                ballVelocityY = 0;
                ball.Y = 0 + ball.Height;
                new SoundPlayer(Properties.Resources.hit_Sound).Play();
            }

            if (ball.X + ball.Width >= this.Width)
            {
                score.Play();
                ball.Location = new Point(this.Width / 2, this.Height - 30);
                player.Location = new Point(30, this.Height - player.Height);
                CPU.Location = new Point(this.Width - 80, this.Height - CPU.Height);
                ballVelocityX = -10;
                ballVelocityY = rand.Next(-10, 0);
                playerScore++;
            }
            if (ball.X <= 0)
            {
                score.Play();
                ball.Location = new Point(this.Width / 2 , this.Height - 30);
                player.Location = new Point(30, this.Height - player.Height);
                CPU.Location = new Point(this.Width - 80, this.Height - CPU.Height);
                ballVelocityX = 10;
                ballVelocityY = rand.Next(-10, 0);
                CPUscore++;
            }

            if (player.IntersectsWith(ball) && ballVelocityX < 0)
            {
                new SoundPlayer(Properties.Resources.hit_Sound).Play();
                ballVelocityX = (int)(-ballVelocityX*1.2);
                ballVelocityY += (playerVelocity + ballBounce*1.5);
            }
            else if (CPU.IntersectsWith(ball) && ballVelocityX > 0)
            {
                new SoundPlayer(Properties.Resources.hit_Sound).Play();
                ballVelocityX = (int)(-ballVelocityX * 1.2);
                ballVelocityY += (CPUvelocity + ballBounce*1.5);
            }
            if (playerJumping == true)
            {
                new SoundPlayer(Properties.Resources.jump_Sound).Play();
                playerVelocity = JumpPower;
                player.Y -= 10;
                playerJumping = false;
            }

            if (player.Y + player.Height >= this.Height)
            {
                if (playerJumping == false)
                {
                    playerVelocity = 0;
                    player.Y = this.Height - player.Height;
                }
                if (playerJumping == true)
                {
                    playerJumping = false;
                    player.Y = this.Height - player.Height;
                    playerVelocity = 0;
                }
            }
            else
            {
                player.Y += (int)(playerVelocity + gravity*2);
            }
            if (player.Y + player.Height < this.Height)
            {
                playerVelocity += gravity/4;
            }

            if (CPUJumping == true)
            {
                new SoundPlayer(Properties.Resources.jump_Sound).Play();
                CPUvelocity = JumpPower;
                CPU.Y -= 10;
                CPUJumping = false;
            }

            if (CPU.Y + CPU.Height >= this.Height)
            {
                if (CPUJumping == false)
                {
                    CPUvelocity = 0;
                    CPU.Y = this.Height - CPU.Height;
                }
                if (CPUJumping == true)
                {
                    CPUJumping = false;
                    CPU.Y = this.Height - CPU.Height;
                    CPUvelocity = 0;
                }
            }
            else
            {
                CPU.Y += (int)(CPUvelocity + gravity * 2);
            }
            if (CPU.Y + CPU.Height < this.Height)
            {
                CPUvelocity += gravity/4;
            }
            if (CPU.Y <= 0)
            {
                CPUvelocity = 0;
                CPU.Y = 0 + CPU.Height/2;
            }
            if (player.Y <= 0)
            {
                playerVelocity = 0;
                player.Y = 0 + player.Height / 2;
            }
            if (ball.Y + ball.Height < this.Height)
            {
                ballVelocityY += gravity/4;
            }
            //ballVelocityY -= ballVelocityY / 50;

            ball.X += (int)ballVelocityX;
            ball.Y += (int)ballVelocityY;

            if (ball.Y < CPU.Y - CPU.Height/2 && ball.X >= this.Width / 2 + this.Width/4 && ballVelocityY <= -5)
            {
                CPUJumping = true;
            }

            if (playerScore >= 1 && CPUscore >= 1)
            {
                if (playerScore > 15)
                {
                    ScoreLabel.ForeColor = Color.Blue;
                    ScoreLabel.Text = "PLAYER HAS WON!";
                    GameTimer.Stop();
                    Refresh();
                    Thread.Sleep(5000);
                    Close();
                }
                else if (CPUscore > 15)
                {
                    ScoreLabel.ForeColor = Color.Red;
                    ScoreLabel.Text = "COMPUTER HAS WON!";
                    GameTimer.Stop();
                    Refresh();
                    Thread.Sleep(5000);
                    Close();
                }
            }

            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            brush.Color = Color.White;
            e.Graphics.FillRectangle(brush, ball);
            brush.Color = Color.Blue;
            e.Graphics.FillRectangle(brush, player);
            brush.Color = Color.Red;
            e.Graphics.FillRectangle(brush, CPU);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            hit.Load();
            jumpSound.Load();
            score.Load();
            winner.Load();
            ball.Location = new Point(this.Width / 2, this.Height / 2 + this.Height / 3);
            player.Location = new Point(30, this.Height - player.Height);
            CPU.Location = new Point(this.Width - 80, this.Height - CPU.Height);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && playerJumping == false)
            {
                playerJumping = true;
            }
        }
    }
}
