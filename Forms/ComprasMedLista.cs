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
    public partial class ComprasMedLista : Form
    {

        SqlDataAdapter adpComprasMed;
        DataTable tabComprasMed;
        SqlConnection con;

        public ComprasMedLista()
        {
            InitializeComponent();
        }

        public ComprasMedLista(SqlConnection conexion)
        {
            InitializeComponent();

            cmbCampo.SelectedIndex = 0;

            adpComprasMed = new SqlDataAdapter("spComprasMedicamentosActivos", conexion);
            adpComprasMed.SelectCommand.CommandType = CommandType.StoredProcedure;

            con = conexion;

        }

        private void ComprasMedLista_Load(object sender, EventArgs e)
        {
            txtTexto.Enabled = false;
            try
            {
                cmbOpcion.Items.Add("Todos");
                cmbOpcion.Items.Add("Activos");
                cmbOpcion.Items.Add("Inactivos");


                tabComprasMed = new DataTable();
                adpComprasMed.Fill(tabComprasMed);
                dataGridView1.DataSource = tabComprasMed;
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
                CompraMed frm = new CompraMed(con, -1);
                frm.ShowDialog();

                tabComprasMed.Clear();
                adpComprasMed.Fill(tabComprasMed);

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


        private void cmdDesactivar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int Compraid = (int)tabComprasMed.DefaultView[dataGridView1.CurrentRow.Index]["CompraID"];

                if (MessageBox.Show("Desea deshabilitar la Compra?", "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand("spSocioDesactivar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Socioid", Compraid);

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

                    tabComprasMed.Clear();
                    adpComprasMed.Fill(tabComprasMed);

                    // Buscar la fila con el mismo Socioid y seleccionarla
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if ((int)row.Cells["CompraID"].Value == Compraid)
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
                tabComprasMed.DefaultView.RowFilter = "";
            }
            else
            {
                if (tabComprasMed.Columns[cmbCampo.Text].DataType == typeof(string))
                {
                    tabComprasMed.DefaultView.RowFilter = cmbCampo.Text + " like '%" + txtTexto.Text + "%'";
                }
                else
                {
                    int numero;
                    if (int.TryParse(txtTexto.Text, out numero))
                    {
                        tabComprasMed.DefaultView.RowFilter = cmbCampo.Text + " = " + numero;
                    }
                    else
                    {
                        tabComprasMed.DefaultView.RowFilter = "1 = 0"; // No coincidirá con nada si el texto no es un número válido
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
                    adpComprasMed = new SqlDataAdapter(spNombre, con);
                    adpComprasMed.SelectCommand.CommandType = CommandType.StoredProcedure;

                    // Vuelve a llenar el DataTable y asigna los datos al DataGridView
                    tabComprasMed.Clear();
                    adpComprasMed.Fill(tabComprasMed);
                    dataGridView1.DataSource = tabComprasMed;
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
