using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ProyectoCREL.Forms
{
    public partial class MenuForm : Form
    {

        SqlConnection myconexion;
        private int childFormNumber = 0;

        public MenuForm()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Ventana " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }


        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }


        private void Menu_Load(object sender, EventArgs e)
        {
            Login frm = new Login();
            frm.ShowDialog();

            if (frm.Conectado)
                myconexion = frm.Conexion;
            else
                Dispose();
        }

        private void sociosToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SocioLista frm = new SocioLista(myconexion);
            frm.MdiParent = this;
            frm.Show();
        }

        private void proveedoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProveedorLista frm = new ProveedorLista();
            frm.MdiParent = this;
            frm.Show();
        }

        private void planillaQuincenalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlanillaQuincenal frm = new PlanillaQuincenal();
            frm.MdiParent = this;
            frm.Show();
        }

        private void ingresoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlLeche frm = new ControlLeche();
            frm.MdiParent = this;
            frm.Show();
        }

        private void consultaQuincenaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConsultaQuincenal frm = new ConsultaQuincenal();
            frm.MdiParent = this;
            frm.Show();
        }

        private void concentradoYOtrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConcentradoLista frm = new ConcentradoLista(myconexion);
            frm.MdiParent = this;
            frm.Show();
        }

        private void salidasMedicamentosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalidaMedicamentosLista frm = new SalidaMedicamentosLista(myconexion);
            frm.MdiParent = this;
            frm.Show();
        }

        private void salidasConcentradoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalidaConcentradoLista frm = new SalidaConcentradoLista(myconexion);
            frm.MdiParent = this;
            frm.Show();
        }

        private void comprasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComprasMedLista frm = new ComprasMedLista(myconexion);
            frm.MdiParent = this;
            frm.Show();
        }


        private void entradasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MedicamentosLista frm = new MedicamentosLista(myconexion);
            frm.MdiParent = this;
            frm.Show();
        }

        private void prestamosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrestamosLista frm = new PrestamosLista();
            frm.MdiParent = this;
            frm.Show();
        }

        private void comprasConcentradoYOtrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComprasConLista frm = new ComprasConLista(myconexion);
            frm.MdiParent = this;
            frm.Show();
        }

        private void anticiposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnticiposLista frm = new AnticiposLista();
            frm.MdiParent = this;
            frm.Show();
        }
    }
}
