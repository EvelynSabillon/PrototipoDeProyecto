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
    public partial class VistaProveedores : Form
    {

        SqlDataAdapter adpProveedor;
        DataTable tabProveedor;
        SqlConnection con;
        CompraCon formCompraCon;

        public VistaProveedores()
        {
            InitializeComponent();
        }

        public VistaProveedores(SqlConnection conexion, CompraCon compraCon)
        {
            InitializeComponent();
            cmbCampo.SelectedIndex = 0;
            con = conexion;
            formCompraCon = compraCon; // Asignamos la referencia del formulario CompraCon
            adpProveedor = new SqlDataAdapter("spProveedorActivos", conexion);
            adpProveedor.SelectCommand.CommandType = CommandType.StoredProcedure;
        }

        private void VistaProveedores_Load(object sender, EventArgs e)
        {
            try
            {
                tabProveedor = new DataTable();
                adpProveedor.Fill(tabProveedor);
                dataGridView1.DataSource = tabProveedor;
                dataGridView1.ReadOnly = true;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void txtTexto_TextChanged(object sender, EventArgs e)
        {
            if (txtTexto.Text.Length == 0)
            {
                tabProveedor.DefaultView.RowFilter = "";
            }
            else
            {
                if (tabProveedor.Columns[cmbCampo.Text].DataType == typeof(string))
                {
                    tabProveedor.DefaultView.RowFilter = cmbCampo.Text + " like '%" + txtTexto.Text + "%'";
                }
                else
                {
                    int numero;
                    if (int.TryParse(txtTexto.Text, out numero))
                    {
                        tabProveedor.DefaultView.RowFilter = cmbCampo.Text + " = " + numero;
                    }
                    else
                    {
                        tabProveedor.DefaultView.RowFilter = "1 = 0"; // No coincidirá con nada si el texto no es un número válido
                    }
                }
            }
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtener el ID del proveedor seleccionado
                int proveedorID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ProveedorID"].Value);

                // Confirmación para seleccionar el proveedor
                DialogResult result = MessageBox.Show("¿Está seguro de seleccionar este proveedor?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Asignamos el ID del proveedor al textbox del formulario CompraCon
                    formCompraCon.txtProveedorID.Text = proveedorID.ToString();
                    this.Close(); // Cerramos el formulario VistaProveedores
                }
            }
        }

        private void cmbCampo_Click(object sender, EventArgs e)
        {
            txtTexto.Enabled = true;
        }
    }
}
