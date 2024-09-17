using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProyectoCREL.Forms
{
    public partial class SocioLista : Form
    {

        SqlDataAdapter adpSocio;
        DataTable tabSocio;
        SqlConnection con;

        public SocioLista()
        {
            InitializeComponent();

        }

        public SocioLista(SqlConnection conexion)
        {
            InitializeComponent();

            cmbCampo.SelectedIndex = 0;

            adpSocio = new SqlDataAdapter("spSocioActivos", conexion);
            adpSocio.SelectCommand.CommandType = CommandType.StoredProcedure;

            con = conexion;

        }


        private void cmdInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                Socio frm = new Socio(con, -1);
                frm.ShowDialog();

                tabSocio.Clear();
                adpSocio.Fill(tabSocio);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmdSalir_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void cmdModificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int Socioid = (int)tabSocio.DefaultView[dataGridView1.CurrentRow.Index]["SocioId"];
                    Socio frm = new Socio(con, Socioid);
                    frm.ShowDialog();

                    tabSocio.Clear();
                    adpSocio.Fill(tabSocio);

                    // Buscar la fila con el mismo Socio y seleccionarla
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if ((int)row.Cells["SocioId"].Value == Socioid)
                        {
                            row.Selected = true;
                            dataGridView1.CurrentCell = row.Cells[0]; // Establecer la celda actual para que se muestre
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void cmdDesactivar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int Socioid = (int)tabSocio.DefaultView[dataGridView1.CurrentRow.Index]["Socioid"];

                if (MessageBox.Show("Desea deshabilitar el Socio", "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand("spSocioDesactivar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Socioid", Socioid);

                    try
                    {
                        // Verificar si la conexión está cerrada antes de abrirla
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        // Asegurarse de cerrar la conexión si fue abierta en este método
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }

                    tabSocio.Clear();
                    adpSocio.Fill(tabSocio);

                    // Buscar la fila con el mismo Socioid y seleccionarla
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if ((int)row.Cells["Socioid"].Value == Socioid)
                        {
                            row.Selected = true;
                            dataGridView1.CurrentCell = row.Cells[0]; // Establecer la celda actual para que se muestre
                            break;
                        }
                    }
                }
            }
        }

        private void txtTexto_TextChanged(object sender, EventArgs e)
        {
            if (txtTexto.Text.Length == 0)
            {
                tabSocio.DefaultView.RowFilter = "";
            }
            else
            {
                if (tabSocio.Columns[cmbCampo.Text].DataType == typeof(string))
                {
                    tabSocio.DefaultView.RowFilter = cmbCampo.Text + " like '%" + txtTexto.Text + "%'";
                }
                else
                {
                    int numero;
                    if (int.TryParse(txtTexto.Text, out numero))
                    {
                        tabSocio.DefaultView.RowFilter = cmbCampo.Text + " = " + numero;
                    }
                    else
                    {
                        tabSocio.DefaultView.RowFilter = "1 = 0"; // No coincidirá con nada si el texto no es un número válido
                    }
                }

            }
        }

        private void cmbOpcion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string spNombre = "";

                switch (cmbOpcion.SelectedItem.ToString())
                {
                    case "Todos":

                        spNombre = "spSocioSelect";
                        break;

                    case "Activos":

                        spNombre = "spSocioActivos";
                        break;

                    case "Inactivos":

                        spNombre = "spSocioInactivos";
                        break;
                }

                if (!string.IsNullOrEmpty(spNombre))
                {
                    // Configura el SqlDataAdapter con el stored procedure seleccionado
                    adpSocio = new SqlDataAdapter(spNombre, con);
                    adpSocio.SelectCommand.CommandType = CommandType.StoredProcedure;

                    // Vuelve a llenar el DataTable y asigna los datos al DataGridView
                    tabSocio.Clear();
                    adpSocio.Fill(tabSocio);
                    dataGridView1.DataSource = tabSocio;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbCampo_Click(object sender, EventArgs e)
        {
            txtTexto.Enabled = true;

        }

        private void SocioLista_Load(object sender, EventArgs e)
        {
            txtTexto.Enabled = false;
            try
            {
                cmbOpcion.Items.Add("Todos");
                cmbOpcion.Items.Add("Activos");
                cmbOpcion.Items.Add("Inactivos");


                tabSocio = new DataTable();
                adpSocio.Fill(tabSocio);
                dataGridView1.DataSource = tabSocio;
                dataGridView1.ReadOnly = true; //solo para leer

                dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightBlue;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


    }
}
