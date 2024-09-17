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
    public partial class MedicamentosLista : Form
    {
        SqlDataAdapter adpMedicamentos;
        DataTable tabMedicamentos;
        SqlConnection con;
        public MedicamentosLista()
        {
            InitializeComponent();
        }

        public MedicamentosLista(SqlConnection conexion)
        {
            InitializeComponent();

            cmbCampo.SelectedIndex = 0;

            adpMedicamentos = new SqlDataAdapter("spMedicamentosActivos", conexion);
            adpMedicamentos.SelectCommand.CommandType = CommandType.StoredProcedure;

            con = conexion;
        }

        private void MedicamentosLista_Load(object sender, EventArgs e)
        {
            txtTexto.Enabled = false;
            try
            {
                cmbOpcion.Items.Add("Todos");
                cmbOpcion.Items.Add("Activos");
                cmbOpcion.Items.Add("Inactivos");

                tabMedicamentos = new DataTable();
                adpMedicamentos.Fill(tabMedicamentos);
                dataGridView1.DataSource = tabMedicamentos;
                dataGridView1.ReadOnly = true;

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
                Medicamentos frm = new Medicamentos(con, -1);
                frm.ShowDialog();

                tabMedicamentos.Clear();
                adpMedicamentos.Fill(tabMedicamentos);
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
                    int ArticuloId = (int)tabMedicamentos.DefaultView[dataGridView1.CurrentRow.Index]["ArticuloID"];
                    Medicamentos frm = new Medicamentos(con, ArticuloId);
                    frm.ShowDialog();

                    tabMedicamentos.Clear();
                    adpMedicamentos.Fill(tabMedicamentos);

                    // Buscar la fila con el mismo Concentrado y seleccionarla
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if ((int)row.Cells["ArticuloID"].Value == ArticuloId)
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
                int ArticuloId = (int)tabMedicamentos.DefaultView[dataGridView1.CurrentRow.Index]["ArticuloID"];

                if (MessageBox.Show("¿Desea deshabilitar el Articulo?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand("spMedicamentosDesactivar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@articuloid", ArticuloId);

                    try
                    {
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
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }

                    tabMedicamentos.Clear();
                    adpMedicamentos.Fill(tabMedicamentos);

                    // Buscar la fila con el mismo ConcentradoId y seleccionarla
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if ((int)row.Cells["ArticuloID"].Value == ArticuloId)
                        {
                            row.Selected = true;
                            dataGridView1.CurrentCell = row.Cells[0];
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
                tabMedicamentos.DefaultView.RowFilter = "";
            }
            else
            {
                if (tabMedicamentos.Columns[cmbCampo.Text].DataType == typeof(string))
                {
                    tabMedicamentos.DefaultView.RowFilter = cmbCampo.Text + " like '%" + txtTexto.Text + "%'";
                }
                else
                {
                    int numero;
                    if (int.TryParse(txtTexto.Text, out numero))
                    {
                        tabMedicamentos.DefaultView.RowFilter = cmbCampo.Text + " = " + numero;
                    }
                    else
                    {
                        tabMedicamentos.DefaultView.RowFilter = "1 = 0"; // No coincidirá con nada si el texto no es un número válido
                    }
                }
            }
        }

        private void cmbCampo_Click(object sender, EventArgs e)
        {
            txtTexto.Enabled = true;
        }

        private void cmbOpcion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string spNombre = "";

                switch (cmbOpcion.SelectedItem.ToString())
                {
                    case "Todos":
                        spNombre = "spMedicamentosSelect";
                        break;

                    case "Activos":
                        spNombre = "spMedicamentosActivos";
                        break;

                    case "Inactivos":
                        spNombre = "spMedicamentosInactivos";
                        break;
                }

                if (!string.IsNullOrEmpty(spNombre))
                {
                    adpMedicamentos = new SqlDataAdapter(spNombre, con);
                    adpMedicamentos.SelectCommand.CommandType = CommandType.StoredProcedure;

                    tabMedicamentos.Clear();
                    adpMedicamentos.Fill(tabMedicamentos);
                    dataGridView1.DataSource = tabMedicamentos;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
