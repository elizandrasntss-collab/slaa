using KeyAuth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace curso_free
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer timer1;
        private List<LineParticle> particles = new List<LineParticle>();
        private Random random = new Random();
        private Point mousePosition;


        public Form1()
        {
            InitializeComponent();
            KeyAuthApp.init();
            // Inicializa o timer
            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 30; // A cada 30ms
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            // Evita flickering (atraso visual)
            this.DoubleBuffered = true;
        }

        public static api KeyAuthApp = new api(
    name: "Selmadarini56's Application", // App name
    ownerid: "f5lhpT645Q", // Account ID
    secret: "b5d31550caa5ae7a4043bbe2d6b41e570bb623492a48275b5c16056cb0a99aba",
    version: "1.0" // Application version. Used for automatic downloads see video here https://www.youtube.com/watch?v=kW195PLCBKs
                   //path: @"Your_Path_Here" // (OPTIONAL) see tutorial here https://www.youtube.com/watch?v=I9rxt821gMk&t=1s
);// Atualiza as partículas a cada tick do Timer
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (particles.Count < 20)
            {
                particles.Add(new LineParticle(ClientSize.Width, ClientSize.Height, random));
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update(ClientSize, mousePosition);

                if (particles[i].IsOffScreen(ClientSize))
                {
                    particles.RemoveAt(i);
                }
            }

            Invalidate();
        }


        // Método para desenhar as partículas
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // FUNDO PRETO
            e.Graphics.Clear(Color.Black);

            foreach (var particle in particles)
            {
                particle.Draw(e.Graphics);
            }
        }


        // Evento para capturar o movimento do mouse
        protected override void OnMouseMove(MouseEventArgs e)
        {
            mousePosition = e.Location;
        }



        public class LineParticle
        {
            public PointF Position { get; private set; }
            private float speedX;
            private float speedY;
            private float size;

            public LineParticle(int screenWidth, int screenHeight, Random random)
            {
                Position = new PointF(
                    random.Next(0, screenWidth),
                    random.Next(0, screenHeight)
                );

                speedX = (float)(random.NextDouble() * 2 - 1);
                speedY = (float)(random.NextDouble() * 2 - 1);

                // TAMANHO MAIOR
                size = random.Next(6, 10);
            }

            public void Update(Size screenSize, Point mousePos)
            {
                Position = new PointF(Position.X + speedX, Position.Y + speedY);

                if (Position.X < 0 || Position.X > screenSize.Width)
                    speedX = -speedX;
                if (Position.Y < 0 || Position.Y > screenSize.Height)
                    speedY = -speedY;

                float dx = Position.X - mousePos.X;
                float dy = Position.Y - mousePos.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                if (dist < 100)
                {
                    float force = (100 - dist) / 100;
                    Position = new PointF(
                        Position.X + dx * 0.05f * force,
                        Position.Y + dy * 0.05f * force
                    );
                }
            }

            // DESENHO COM GLOW BRANCO
            public void Draw(Graphics g)
            {
                using (Brush glow = new SolidBrush(Color.FromArgb(60, 255, 255, 255)))
                using (Brush core = new SolidBrush(Color.FromArgb(255, 255, 255, 255)))
                {
                    g.FillEllipse(
                        glow,
                        Position.X - size,
                        Position.Y - size,
                        size * 2,
                        size * 2
                    );

                    g.FillEllipse(
                        core,
                        Position.X - size / 2,
                        Position.Y - size / 2,
                        size,
                        size
                    );
                }
            }

            public bool IsOffScreen(Size screenSize)
            {
                return Position.X < 0 || Position.X > screenSize.Width ||
                       Position.Y < 0 || Position.Y > screenSize.Height;
            }
        }



        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }


        //BUTTON
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            {
                KeyAuthApp.login(Username.Text, Pass.Text);
                if (KeyAuthApp.response.success)
                {
                    status.Text = KeyAuthApp.response.message;
                    Form2 main = new Form2();
                    main.Show();
                    this.Hide();
                }
                else
                {
                    status.Text = KeyAuthApp.response.message;
                }

            }
        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {

        }
        //EXIT
        private void label4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
