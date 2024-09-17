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
    public partial class ConcentradoLista : Form
    {

        SqlDataAdapter adpConcentrado;
        DataTable tabConcentrado;
        SqlConnection con;
        public ConcentradoLista()
        {
            InitializeComponent();
        }

        public ConcentradoLista(SqlConnection conexion)
        {
            InitializeComponent();

            cmbCampo.SelectedIndex = 0;

            adpConcentrado = new SqlDataAdapter("spConcentradoActivos", conexion);
            adpConcentrado.SelectCommand.CommandType = CommandType.StoredProcedure;

            con = conexion;
        }


        private void ConcentradoLista_Load(object sender, EventArgs e)
        {
            txtTexto.Enabled = false;
            try
            {
                cmbOpcion.Items.Add("Todos");
                cmbOpcion.Items.Add("Activos");
                cmbOpcion.Items.Add("Inactivos");

                tabConcentrado = new DataTable();
                adpConcentrado.Fill(tabConcentrado);
                dataGridView1.DataSource = tabConcentrado;
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
                Concentrado frm = new Concentrado(con, -1);
                frm.ShowDialog();

                tabConcentrado.Clear();
                adpConcentrado.Fill(tabConcentrado);
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
                    int ArticuloId = (int)tabConcentrado.DefaultView[dataGridView1.CurrentRow.Index]["ArticuloID"];
                    Concentrado frm = new Concentrado(con, ArticuloId);
                    frm.ShowDialog();

                    tabConcentrado.Clear();
                    adpConcentrado.Fill(tabConcentrado);

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
                int ArticuloId = (int)tabConcentrado.DefaultView[dataGridView1.CurrentRow.Index]["ArticuloID"];

                if (MessageBox.Show("¿Desea deshabilitar el Producto?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand("spConcentradoDesactivar", con);
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

                    tabConcentrado.Clear();
                    adpConcentrado.Fill(tabConcentrado);

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
                tabConcentrado.DefaultView.RowFilter = "";
            }
            else
            {
                if (tabConcentrado.Columns[cmbCampo.Text].DataType == typeof(string))
                {
                    tabConcentrado.DefaultView.RowFilter = cmbCampo.Text + " like '%" + txtTexto.Text + "%'";
                }
                else
                {
                    int numero;
                    if (int.TryParse(txtTexto.Text, out numero))
                    {
                        tabConcentrado.DefaultView.RowFilter = cmbCampo.Text + " = " + numero;
                    }
                    else
                    {
                        tabConcentrado.DefaultView.RowFilter = "1 = 0"; // No coincidirá con nada si el texto no es un número válido
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
                        spNombre = "spConcentradoSelect";
                        break;

                    case "Activos":
                        spNombre = "spConcentradoActivos";
                        break;

                    case "Inactivos":
                        spNombre = "spConcentradoInactivos";
                        break;
                }

                if (!string.IsNullOrEmpty(spNombre))
                {
                    adpConcentrado = new SqlDataAdapter(spNombre, con);
                    adpConcentrado.SelectCommand.CommandType = CommandType.StoredProcedure;

                    tabConcentrado.Clear();
                    adpConcentrado.Fill(tabConcentrado);
                    dataGridView1.DataSource = tabConcentrado;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
