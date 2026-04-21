using BASTAB;
using Guna.UI2.WinForms;
using Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace curso_free
{
    public partial class NexusMenu : Form
    {
        private System.Windows.Forms.Timer timer1;
        private List<LineParticle> particles = new List<LineParticle>();
        private Random random = new Random();
        private Point mousePosition;


        private static Bastab BASTAB = new Bastab();
        private Dictionary<long, int> OriginalValuesCheckbox1 = new Dictionary<long, int>();

        // Novas variáveis adicionadas
        string AimbotScan = ("FF FF ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 A5 43");
        string AimbotScanf = ("FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A5 43 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 BF ?? ?? ?? ?? 00 00 00 00 00 00 ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00");
        string headoffset = ("0x80");
        string chestoffset = ("0x7C");

        string novaoffset = ("0x80");
        string novaoffset1 = ("0x7C");
        private Dictionary<long, int> OrginalValues1 = new Dictionary<long, int>();
        private Dictionary<long, int> OrginalValues2 = new Dictionary<long, int>();
        private Dictionary<long, int> OrginalValues3 = new Dictionary<long, int>();
        private Dictionary<long, int> OrginalValues4 = new Dictionary<long, int>();

        private const uint PROCESS_CREATE_THREAD = 0x0002;
        private const uint PROCESS_QUERY_INFORMATION = 0x0400;
        private const uint PROCESS_VM_OPERATION = 0x0008;
        private const uint PROCESS_VM_WRITE = 0x0020;
        private const uint PROCESS_VM_READ = 0x0010;
        private const uint MEM_COMMIT = 0x1000;
        private const uint PAGE_READWRITE = 0x04;


        // Stream Mode - Importações
        private bool Streaming;
        //Vision 10x
        bool cameraright = false;
        //2x
        bool tracking = false;

        [DllImport("user32.dll")]
        private static extern bool SetWindowDisplayAffinity(IntPtr hwnd, uint affinity);

        private const uint WDA_NONE = 0x00000000;
        private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

        // Importações da kernel32.dll
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes,
            IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags,
            IntPtr lpThreadId);


        // Adicione junto com as outras importações da kernel32.dll
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        // Variáveis para animação do Form (slide up)
        private System.Windows.Forms.Timer animationTimer;
        private int targetY;

        // Variáveis para Memory
        Beyondmem.Memory fastMemory = new Beyondmem.Memory(); Mem memory = new Mem();
        Mem Memory = new Mem();
        private bool aimbotAtivado;
        private bool isInjected = false;
        private bool fovAtivado = false;
        private Dictionary<long, byte[]> ValoresOriginaisFov = new Dictionary<long, byte[]>();



        public NexusMenu()
        {
            InitializeComponent();
            // Inicializa o timer
            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 30; // A cada 30ms
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            // Evita flickering (atraso visual)
            this.DoubleBuffered = true;
        }

        // Atualiza as partículas a cada tick do Timer
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



        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        // ==================== AIMBOT HEAD ====================

        private async void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!guna2CheckBox1.Checked)
                    return;

                var procs = Process.GetProcessesByName("HD-Player");
                if (procs == null || procs.Length == 0)
                {
                    MessageBox.Show("Processo \"HD-Player\" não encontrado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int procId = procs[0].Id;

                // Abre processo (shim). Substitua por implementação legítima se necessário.
                Memory.OpenProcess(procId);

                string pattern = "FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 A5 43 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ??";

                var result = await Memory.AoBScan(pattern, true, true);

                if (result != null && result.Any())
                {
                    foreach (var currentAddress in result)
                    {
                        long readAddr = currentAddress + 0xAC;
                        long writeAddr = currentAddress + 0xA8;

                        var readValue = Memory.ReadMemory<int>(readAddr.ToString("X"));
                        Memory.WriteMemory(writeAddr.ToString("X"), "int", readValue.ToString());
                    }

                    MessageBox.Show("AIMBOT HEAD ACTIVATED", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("PATCHED / padrão não encontrado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao executar AoBScan: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void label4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox3.Checked)
            {
                const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
                NexusMenu.SetWindowDisplayAffinity(this.Handle, WDA_EXCLUDEFROMCAPTURE);
            }
            else
            {
                const uint WDA_EXCLUDEFROMCAPTURE = 0x00000000;
                NexusMenu.SetWindowDisplayAffinity(this.Handle, WDA_EXCLUDEFROMCAPTURE);
            }
        }
        //Task Bar Show
        private const int WS_EX_APPWINDOW = 0x00040000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int GWL_EXSTYLE = -20;
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private bool isHiddenFromTaskbarAndAltTab = false;
        private void guna2CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (!isHiddenFromTaskbarAndAltTab)
            {
                this.ShowInTaskbar = false;
                this.WindowState = FormWindowState.Normal;
                IntPtr hwnd = this.Handle;
                int style = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, style | WS_EX_TOOLWINDOW);
            }
            else
            {
                IntPtr hwnd = this.Handle;
                int style = GetWindowLong(hwnd, GWL_EXSTYLE);
                this.ShowInTaskbar = true;
                SetWindowLong(hwnd, GWL_EXSTYLE, (style & ~WS_EX_TOOLWINDOW) | WS_EX_APPWINDOW);
            }

            isHiddenFromTaskbarAndAltTab = !isHiddenFromTaskbarAndAltTab;
        }

        private async void guna2CheckBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("HD-Player").Length == 0)
            {
                status.Text = "Emulator Not Found!!, Open Emulator First";
                guna2CheckBox7.Checked = false;
                return;
            }

            try
            {
                Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
                status.Text = "Abrindo processo...";
                bool opened = BASTAB.OpenProcess(proc);
                if (!opened)
                {
                    status.Text = "ERRO: Execute como ADMIN!";
                    guna2CheckBox7.Checked = false;
                    MessageBox.Show("Execute o programa como ADMINISTRADOR!", "Erro de Permissão", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                status.Text = "Scanning...";
                await Task.Delay(300);

                bool success = false;

                if (guna2CheckBox7.Checked)
                {
                    // ATIVAR HS Neck
                    status.Text = "Ativando HS Neck...";
                    await Task.Delay(300);

                    // Primeiro patch
                    var result1 = await BASTAB.AoBScan2("dc 52 39 bd 27 c1 8b 3c c0 d0 f8 b9", writable: true, executable: true);
                    if (result1 != null && result1.Any())
                    {
                        foreach (var addr in result1)
                        {
                            BASTAB.WriteMemory(addr.ToString("X"), "bytes", "dc 52 39 0a 0a d7 23 3d d2 a5 f9 b9");
                            status.Text = $"HS Neck patch 1 aplicado em 0x{addr:X}";
                            await Task.Delay(100);
                        }
                    }

                    // Segundo patch
                    var result2 = await BASTAB.AoBScan2("63 71 b0 bd 90 98 74 bb", writable: true, executable: true);
                    if (result2 != null && result2.Any())
                    {
                        foreach (var addr in result2)
                        {
                            BASTAB.WriteMemory(addr.ToString("X"), "bytes", "cd dc 79 bd 90 98 74 bb");
                            status.Text = $"HS Neck patch 2 aplicado em 0x{addr:X}";
                            await Task.Delay(100);
                        }
                    }

                    if ((result1 != null && result1.Any()) || (result2 != null && result2.Any()))
                    {
                        status.Text = "HS Neck Enabled ✅";
                        success = true;
                    }
                    else
                    {
                        status.Text = "Pattern não encontrado para ativar HS Neck";
                        guna2CheckBox7.Checked = false;
                    }
                }
                else
                {
                    // DESATIVAR HS Neck
                    status.Text = "Desativando HS Neck...";
                    await Task.Delay(300);

                    // Reverter primeiro patch
                    var result1 = await BASTAB.AoBScan2("dc 52 39 0a 0a d7 23 3d d2 a5 f9 b9", writable: true, executable: true);
                    if (result1 != null && result1.Any())
                    {
                        foreach (var addr in result1)
                        {
                            BASTAB.WriteMemory(addr.ToString("X"), "bytes", "dc 52 39 bd 27 c1 8b 3c c0 d0 f8 b9");
                            status.Text = $"HS Neck patch 1 removido em 0x{addr:X}";
                            await Task.Delay(100);
                        }
                    }

                    // Reverter segundo patch
                    var result2 = await BASTAB.AoBScan2("cd dc 79 bd 90 98 74 bb", writable: true, executable: true);
                    if (result2 != null && result2.Any())
                    {
                        foreach (var addr in result2)
                        {
                            BASTAB.WriteMemory(addr.ToString("X"), "bytes", "63 71 b0 bd 90 98 74 bb");
                            status.Text = $"HS Neck patch 2 removido em 0x{addr:X}";
                            await Task.Delay(100);
                        }
                    }

                    if ((result1 != null && result1.Any()) || (result2 != null && result2.Any()))
                    {
                        status.Text = "HS Neck Disabled ❌";
                        success = true;
                    }
                    else
                    {
                        status.Text = "Pattern modificado não encontrado";
                    }
                }

                BASTAB.CloseProcess();

                if (!success && guna2CheckBox7.Checked)
                {
                    status.Text = "Falha ao ativar HS Neck";
                    guna2CheckBox7.Checked = false;
                }
            }
            catch (Exception ex)
            {
                status.Text = $"Erro: {ex.Message}";
                guna2CheckBox7.Checked = false;
                MessageBox.Show($"Erro ao processar:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool noRecoilAtivado = false;
        private string enderecoOriginal = "";
        private object process;
        private async void guna2CheckBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("HD-Player").Length == 0)
            {
                status.Text = "Emulator Not Found!!, Open Emulator First";
                guna2CheckBox6.Checked = false;
                return;
            }

            memory.OpenProcess("HD-Player");

            if (guna2CheckBox6.Checked)
            {
                // ATIVAR NO RECOIL
                status.Text = "Activating no recoil...";

                string search = "01 EE 00 0A 81 EE 10 0A 10 EE 10 8C BD E8 00 00 7A 44 F0 48 2D E9 10 B0 8D E2 02 8B 2D ED 08 D0 4D E2 00 50 A0 E1";
                string replace = "01 EE 00 0A 81 EE 10 0A 10 EE 10 8C BD E8 00 00 7A FF F0 48 2D E9 10 B0 8D E2 02 8B 2D ED 08 D0 4D E2 00 50 A0 E1";

                IEnumerable<long> wl = await memory.AoBScan(search, writable: true);

                if (wl.Count() != 0)
                {
                    enderecoOriginal = wl.FirstOrDefault().ToString("X");

                    for (int i = 0; i < wl.Count(); i++)
                    {
                        memory.WriteMemory(wl.ElementAt(i).ToString("X"), "bytes", replace);
                    }

                    noRecoilAtivado = true;
                    status.Text = "No Recoil Activated";
                }
                else
                {
                    status.Text = "Activation Failed - Pattern not found";
                    guna2CheckBox6.Checked = false;
                }
            }
            else
            {
                // DESATIVAR NO RECOIL
                status.Text = "Deactivating no recoil...";

                if (noRecoilAtivado && !string.IsNullOrEmpty(enderecoOriginal))
                {
                    string original = "01 EE 00 0A 81 EE 10 0A 10 EE 10 8C BD E8 00 00 7A 44 F0 48 2D E9 10 B0 8D E2 02 8B 2D ED 08 D0 4D E2 00 50 A0 E1";

                    memory.WriteMemory(enderecoOriginal, "bytes", original);

                    noRecoilAtivado = false;
                    enderecoOriginal = "";
                    status.Text = "No Recoil Deactivated";
                }
                else
                {
                    status.Text = "Nothing to deactivate";
                }
            }
        }




        private async void guna2CheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("HD-Player").Length == 0)
            {
                return;
            }

            memory.OpenProcess("HD-Player");

            // Padrão original e patch
            string search = "DB 0F 49 40 10 2A 00 EE 00 10 80 E5 10 3A 01 EE 14 10 80 E5 00 2A 30 EE 00 10 00 E3 41 3A 30 EE 80 1F 4B E3 01 0A 30 EE 2C 10 80 E5 50 00 C0 F2 04 10 80 E2 00 20 A0 E3 30 20 80 E5 34 20 80 E5 01 1A 22 EE 3C 20 80 E5 8F 0A 41 F4 18 10 80 E2 03 0A 80 EE 03 1A 81 EE 8F 0A";
            string replace = "DB 0F A9 40 10 2A 00 EE 00 10 80 E5 10 3A 01 EE";

            // Alterna variável
            cameraright = !cameraright;

            if (cameraright)
            {
                // ⚡ ATIVAR VISION 10X
                var originalAddresses = await memory.AoBScan(search, writable: true);

                if (originalAddresses.Any())
                {
                    foreach (var addr in originalAddresses)
                        memory.WriteMemory(addr.ToString("X"), "bytes", replace);

                }
                else
                {
                }
            }
            else
            {
                // ❌ DESATIVAR VISION 10X (REVERSÃO)
                var patchedAddresses = await memory.AoBScan(replace, writable: true);

                if (patchedAddresses.Any())
                {
                    foreach (var addr in patchedAddresses)
                        memory.WriteMemory(addr.ToString("X"), "bytes", search);

                }
                else
                {
                }
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            P3.BringToFront();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            P1.BringToFront();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            P2.BringToFront();
        }

        private async void guna2CheckBox8_CheckedChanged(object sender, EventArgs e)
        {

        }
        private List<long> savedAddressesCheckbox1 = new List<long>();
        private async void guna2CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            memory.OpenProcess("HD-Player");

            if (guna2CheckBox2.Checked)
            {
                // ========== ATIVAR AIMBOT ==========

                // Se já temos endereços salvos, reutilizar
                if (savedAddressesCheckbox1.Count > 0)
                {
                    status.Text = "⏳ Ativando Aimbot...";
                    await Task.Delay(100);

                    // Reativar usando endereços salvos
                    foreach (var current in savedAddressesCheckbox1)
                    {
                        long offsetRead = current + 0x0B;
                        long offsetWrite = current + 0x7C;

                        int newValue = memory.ReadMemory<int>(offsetRead.ToString("X"));
                        memory.WriteMemory(offsetWrite.ToString("X"), "int", newValue.ToString());
                    }

                    status.Text = "✅ Aimbot Ativado";
                }
                else
                {
                    // Primeira ativação - fazer scan completo
                    status.Text = "🔍 Procurando padrão...";
                    await Task.Delay(100);

                    OriginalValuesCheckbox1.Clear();
                    savedAddressesCheckbox1.Clear();

                    var scan = await memory.AoBScan("FF FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 A5 43", true, true);

                    if (scan.Any())
                    {
                        status.Text = "💉 Injetando Aimbot...";
                        await Task.Delay(100);

                        foreach (var current in scan)
                        {
                            // Salvar endereço para reutilizar depois
                            savedAddressesCheckbox1.Add(current);

                            long offsetRead = current + 0xB0;
                            long offsetWrite = current + 0x7C;

                            int originalValue = memory.ReadMemory<int>(offsetWrite.ToString("X"));
                            OriginalValuesCheckbox1[offsetWrite] = originalValue;

                            int newValue = memory.ReadMemory<int>(offsetRead.ToString("X"));
                            memory.WriteMemory(offsetWrite.ToString("X"), "int", newValue.ToString());
                        }

                        status.Text = "✅ Aimbot Ativado com sucesso";
                    }
                    else
                    {
                        status.Text = "❌ Falha - Pattern não encontrado";
                        guna2CheckBox2.Checked = false; // Desmarca o checkbox
                    }
                }
            }
            else
            {
                // ========== DESATIVAR AIMBOT ==========
                status.Text = "⏳ Desativando Aimbot...";
                await Task.Delay(100);

                // Restaurar valores originais
                foreach (var kvp in OriginalValuesCheckbox1)
                {
                    memory.WriteMemory(kvp.Key.ToString("X"), "int", kvp.Value.ToString());
                }

                status.Text = "🛑 Aimbot Desativado";

            }
        }

        private async void guna2CheckBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("HD-Player").Length == 0)
            {
                status.Text = "Emulator Not Found!!, Open Emulator First";
                guna2CheckBox9.Checked = false; // Desmarca se não encontrar o emulador
                return;
            }

            memory.OpenProcess("HD-Player");

            // Patterns originais e modificados
            string searchPattern = "00 F0 41 00 00 48 42 00 00 00 3F 33 33 13 40 00 00 D0 3F 00 00 80 3F 01 00";
            string modifiedPattern = "00 F0 41 00 00 48 42 00 00 00 3F 33 33 13 40 00 00 D0 3F 00 00 80 5C 01 00";

            bool anySuccess = false;

            if (guna2CheckBox9.Checked)
            {
                // ATIVAR - Procura pelo padrão original e substitui pelo modificado
                status.Text = "Injetando 4x Tracking";

                var result = await memory.AoBScan(searchPattern, writable: true);
                if (result.Any())
                {
                    foreach (var address in result)
                    {
                        memory.WriteMemory(address.ToString("X"), "bytes", modifiedPattern);
                    }
                    anySuccess = true;
                    status.Text = anySuccess ? "Inject Success" : "Inject Failed";
                }
                else
                {
                    status.Text = "Pattern not found - Already injected?";
                }
            }
            else
            {
                // DESATIVAR - Procura pelo padrão modificado e restaura o original
                status.Text = "Desativando 4x Tracking";

                var result = await memory.AoBScan(modifiedPattern, writable: true);
                if (result.Any())
                {
                    foreach (var address in result)
                    {
                        memory.WriteMemory(address.ToString("X"), "bytes", searchPattern);
                    }
                    anySuccess = true;
                    status.Text = anySuccess ? "Deactivated Successfully" : "Deactivation Failed";
                }
                else
                {
                    status.Text = "Pattern not found - Already deactivated?";
                }
            }
        }

        private async void guna2CheckBox8_CheckedChanged_1(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("HD-Player").Length == 0)
            {
                status.Text = "Emulator Not Found!!, Open Emulator First";
            }
            else
            {
                memory.OpenProcess("HD-Player");

                string search = "33 33 93 3F 8F C2 F5 3C CD CC CC 3D 02 00 00 00 EC 51 B8 3D CD CC 4C 3F 00 00 00 00 00 00 A0 42 00 00 C0 3F 33 33 13 40 00 00 F0 3F 00 00 80 3F 01 00";
                string replace = "33 33 93 3F 8F C2 F5 3C CD CC CC 3D 02 00 00 00 EC 51 B8 3D CD CC 4C 3F 00 00 00 00 00 00 A0 42 00 00 C0 3F 33 33 13 40 00 00 F0 3F 00 00 29 5C 01 00";

                bool anySuccess = false;

                // Tenta encontrar patch ativo (replace)
                var patchedAddresses = await memory.AoBScan(replace, writable: true);

                if (patchedAddresses.Any())
                {
                    // Patch tá ativo, então remove ele (desativa)
                    foreach (var address in patchedAddresses)
                    {
                        memory.WriteMemory(address.ToString("X"), "bytes", search);
                    }
                    status.Text = "Aim Track Disabled";
                    anySuccess = true;
                }
                else
                {
                    // Patch não tá ativo, ativa ele
                    var originalAddresses = await memory.AoBScan(search, writable: true);

                    if (originalAddresses.Any())
                    {
                        foreach (var address in originalAddresses)
                        {
                            memory.WriteMemory(address.ToString("X"), "bytes", replace);
                        }
                        status.Text = "Aim Track Enabled";
                        anySuccess = true;
                    }
                }

                if (!anySuccess)
                {
                    status.Text = "Aim Track Failed";
                }
            }
        }

        private void P2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2CheckBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox10.Checked)
            {
                // Verifica se o processo "HD-Player" está em execução
                if (Process.GetProcessesByName("HD-Player").Length == 0)
                {
                    status.Text = " erro ";
                    Console.Beep(400, 300);
                }
                else
                {
                    status.Text = " DONE ";
                    Console.Beep(400, 300);

                    try
                    {
                        string processName = "HD-Player"; // Nome do processo alvo
                        string dllUrl = "https://www.dropbox.com/scl/fi/t5r2o7uwlcozv9zoa7av6/BLUE_AND_WHITE.dll?rlkey=6lllga3vcvc9tyyr3xgbxe1tj&st=2a8k72fr&raw=1";
                        string tempDllPath = Path.Combine(Path.GetTempPath(), "BLUE_AND_WHITE.dll");

                        // Baixa o arquivo DLL para o caminho temporário
                        using (var client = new System.Net.WebClient())
                        {
                            client.DownloadFile(dllUrl, tempDllPath);
                        }

                        Process[] targetProcesses = Process.GetProcessesByName(processName);
                        if (targetProcesses.Length == 0)
                        {
                            Console.WriteLine($"Esperando {processName}.exe...");
                            return;
                        }

                        Process targetProcess = targetProcesses[0];
                        IntPtr hProcess = OpenProcess(
                            PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ,
                            false,
                            targetProcess.Id
                        );

                        if (hProcess == IntPtr.Zero)
                        {
                            MessageBox.Show("Falha ao abrir o processo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                        if (loadLibraryAddr == IntPtr.Zero)
                        {
                            MessageBox.Show("Falha ao localizar LoadLibraryA.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        IntPtr allocMemAddress = VirtualAllocEx(
                            hProcess,
                            IntPtr.Zero,
                            (IntPtr)tempDllPath.Length,
                            MEM_COMMIT,
                            PAGE_READWRITE
                        );

                        if (allocMemAddress == IntPtr.Zero)
                        {
                            MessageBox.Show("Falha ao alocar memória no processo alvo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        bool success = WriteProcessMemory(
                            hProcess,
                            allocMemAddress,
                            System.Text.Encoding.ASCII.GetBytes(tempDllPath),
                            (uint)tempDllPath.Length,
                            out _
                        );

                        if (!success)
                        {
                            MessageBox.Show("Falha ao escrever na memória do processo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        IntPtr remoteThread = CreateRemoteThread(
                            hProcess,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            loadLibraryAddr,
                            allocMemAddress,
                            0,
                            IntPtr.Zero
                        );

                        if (remoteThread == IntPtr.Zero)
                        {
                            MessageBox.Show("Falha ao criar o thread remoto.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Injeção concluída com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            // Verifica se o processo "HD-Player" está em execução
            if (Process.GetProcessesByName("HD-Player").Length == 0)
            {

                status.Text = " erro ";
                Console.Beep(400, 300);
            }
            else
            {

                status.Text = " Ativado ";
                Console.Beep(400, 300);

                try
                {
                    string processName = "HD-Player"; // Nome do processo alvo
                    string dllUrl = "https://github.com/Darwingum/Darkzin-hub/raw/refs/heads/main/public.dll";
                    string tempDllPath = Path.Combine(Path.GetTempPath(), "public.dll");

                    // Baixa o arquivo DLL para o caminho temporário
                    using (var client = new System.Net.WebClient())
                    {
                        client.DownloadFile(dllUrl, tempDllPath);
                    }

                    Process[] targetProcesses = Process.GetProcessesByName(processName);
                    if (targetProcesses.Length == 0)
                    {
                        Console.WriteLine($"Esperando {processName}.exe...");
                        return;
                    }

                    Process targetProcess = targetProcesses[0];
                    IntPtr hProcess = OpenProcess(
                        PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ,
                        false,
                        targetProcess.Id
                    );

                    if (hProcess == IntPtr.Zero)
                    {
                        MessageBox.Show("Falha ao abrir o processo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                    if (loadLibraryAddr == IntPtr.Zero)
                    {
                        MessageBox.Show("Falha ao localizar LoadLibraryA.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    IntPtr allocMemAddress = VirtualAllocEx(
                        hProcess,
                        IntPtr.Zero,
                        (IntPtr)tempDllPath.Length,
                        MEM_COMMIT,
                        PAGE_READWRITE
                    );

                    if (allocMemAddress == IntPtr.Zero)
                    {
                        MessageBox.Show("Falha ao alocar memória no processo alvo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    bool success = WriteProcessMemory(
                        hProcess,
                        allocMemAddress,
                        System.Text.Encoding.ASCII.GetBytes(tempDllPath),
                        (uint)tempDllPath.Length,
                        out _
                    );

                    if (!success)
                    {
                        MessageBox.Show("Falha ao escrever na memória do processo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    IntPtr remoteThread = CreateRemoteThread(
                        hProcess,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        loadLibraryAddr,
                        allocMemAddress,
                        0,
                        IntPtr.Zero
                    );

                    if (remoteThread == IntPtr.Zero)
                    {
                        MessageBox.Show("Falha ao criar o thread remoto.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}