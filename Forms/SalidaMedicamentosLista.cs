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
    public partial class SalidaMedicamentosLista : Form
    {

        SqlDataAdapter adpSalidasMed;
        DataTable tabSalidasMed;
        SqlConnection con;
        public SalidaMedicamentosLista()
        {
            InitializeComponent();
        }

        public SalidaMedicamentosLista(SqlConnection conexion)
        {
            InitializeComponent();

            cmbCampo.SelectedIndex = 0;

            adpSalidasMed = new SqlDataAdapter("spComprasMedicamentosActivos", conexion);
            adpSalidasMed.SelectCommand.CommandType = CommandType.StoredProcedure;

            con = conexion;

        }


        private void txtTexto_TextChanged(object sender, EventArgs e)
        {

            if (txtTexto.Text.Length == 0)
            {
                tabSalidasMed.DefaultView.RowFilter = "";
            }
            else
            {
                if (tabSalidasMed.Columns[cmbCampo.Text].DataType == typeof(string))
                {
                    tabSalidasMed.DefaultView.RowFilter = cmbCampo.Text + " like '%" + txtTexto.Text + "%'";
                }
                else
                {
                    int numero;
                    if (int.TryParse(txtTexto.Text, out numero))
                    {
                        tabSalidasMed.DefaultView.RowFilter = cmbCampo.Text + " = " + numero;
                    }
                    else
                    {
                        tabSalidasMed.DefaultView.RowFilter = "1 = 0"; // No coincidirá con nada si el texto no es un número válido
                    }
                }

            }
        }

        private void SalidaMedicamentosLista_Load(object sender, EventArgs e)
        {
            txtTexto.Enabled = false;
            try
            {
                cmbOpcion.Items.Add("Todos");
                cmbOpcion.Items.Add("Activos");
                cmbOpcion.Items.Add("Inactivos");


                tabSalidasMed = new DataTable();
                adpSalidasMed.Fill(tabSalidasMed);
                dataGridView1.DataSource = tabSalidasMed;
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

        private void cmdInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                SalidaMedicamentos frm = new SalidaMedicamentos(con, -1);
                frm.ShowDialog();

                tabSalidasMed.Clear();
                adpSalidasMed.Fill(tabSalidasMed);

            }
            catch (Exception ex)
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
                    int Salidaid = (int)tabSalidasMed.DefaultView[dataGridView1.CurrentRow.Index]["SalidaID"];
                    SalidaMedicamentos frm = new SalidaMedicamentos(con, Salidaid);
                    frm.ShowDialog();

                    tabSalidasMed.Clear();
                    adpSalidasMed.Fill(tabSalidasMed);

                    // Buscar la fila con el mismo Socio y seleccionarla
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if ((int)row.Cells["SalidaID"].Value == Salidaid)
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
                int Salidaid = (int)tabSalidasMed.DefaultView[dataGridView1.CurrentRow.Index]["SalidaID"];

                if (MessageBox.Show("Desea deshabilitar la Salida?", "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand("spSocioDesactivar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Socioid", Salidaid);

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

                    tabSalidasMed.Clear();
                    adpSalidasMed.Fill(tabSalidasMed);

                    // Buscar la fila con el mismo Socioid y seleccionarla
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if ((int)row.Cells["SalidaID"].Value == Salidaid)
                        {
                            row.Selected = true;
                            dataGridView1.CurrentCell = row.Cells[0]; // Establecer la celda actual para que se muestre
                            break;
                        }
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
                    adpSalidasMed = new SqlDataAdapter(spNombre, con);
                    adpSalidasMed.SelectCommand.CommandType = CommandType.StoredProcedure;

                    // Vuelve a llenar el DataTable y asigna los datos al DataGridView
                    tabSalidasMed.Clear();
                    adpSalidasMed.Fill(tabSalidasMed);
                    dataGridView1.DataSource = tabSalidasMed;
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
    }
}
