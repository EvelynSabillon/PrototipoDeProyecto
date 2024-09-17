using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoCREL.Forms
{
    public partial class ProveedorLista : Form
    {

        string url = "server=.\\SQLEXPRESS; DataBase=CREL; User ID = sa; Password = 123456789";
        SqlDataAdapter adpLeer;
        DataTable dtleer;
        SqlConnection conexion;

        public ProveedorLista()
        {
            InitializeComponent();
            conexion = new SqlConnection(url);
          
            conexion.Open(); 
            dtleer = new DataTable();
            adpLeer = new SqlDataAdapter("spProveedorActivos", conexion);
            adpLeer.SelectCommand.CommandType = CommandType.StoredProcedure;

            cmbCampo.SelectedIndex = 0;
        }

        private void txtTelefono_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string rawText = new string(txtTelefono.Text.Where(char.IsDigit).ToArray());

                if (rawText.Length > 0)
                {
                    if (rawText.Length <= 4)
                    {
                        txtTelefono.Text = rawText;
                    }
                    else if (rawText.Length <= 8)
                    {
                        txtTelefono.Text = rawText.Insert(4, "-");
                    }
                    else
                    {
                        txtTelefono.Text = rawText.Insert(4, "-").Substring(0, 9);
                    }

                    // Coloca el cursor al final del texto
                    txtTelefono.SelectionStart = txtTelefono.Text.Length;
                }
            }
            catch
            {
                MessageBox.Show("El formato del teléfono debe ser 0000-0000.", "Formato Incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                //insertar nuevo proveedor
                Form newProv = new Proveedor(conexion);
                newProv.ShowDialog();

                conexion.Open();
                dtleer.Clear();
                adpLeer.Fill(dtleer);
                conexion.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmProveedor_Load(object sender, EventArgs e)
        {
            try 
            {
                cmbOpcion.Items.Add("Todos");
                cmbOpcion.Items.Add("Activos");
                cmbOpcion.Items.Add("Inactivos");
                txtTexto.Enabled = false;

                dgcrudProv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightBlue;
                dgcrudProv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgcrudProv.AllowUserToAddRows = false;
                dgcrudProv.AllowUserToDeleteRows = false;
                dgcrudProv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgcrudProv.ReadOnly = true;

                adpLeer.Fill(dtleer);
                dgcrudProv.DataSource = dtleer;
                conexion.Close();
            }
            catch(Exception ex) 
            { 

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void dgcrudProv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {   

            if (e.RowIndex >= 0)
            {
                DataGridViewRow filaSeleccionada = dgcrudProv.Rows[e.RowIndex];

                
                txtNombre.Text = filaSeleccionada.Cells["Nombre"].Value.ToString();
                txtDireccion.Text = filaSeleccionada.Cells["Direccion"].Value.ToString();
                txtTelefono.Text = filaSeleccionada.Cells["Telefono"].Value.ToString();
                chkEstado.Checked = Convert.ToBoolean(filaSeleccionada.Cells["Activo"].Value);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        { 
            // todos los campos deben ir llenos para no insertar datos no deseados 
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text) || txtTelefono.Text.Length != 9 )
            {
                MessageBox.Show("Por favor, complete todos los campos requeridos de manera correcta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            try
            {

                int proveedorId = Convert.ToInt32(dgcrudProv.CurrentRow.Cells["Proveedorid"].Value);


                using (SqlCommand cmdActualizar = new SqlCommand("spProveedorUpdate", conexion))
                {
                    cmdActualizar.CommandType = CommandType.StoredProcedure;


                    
                    // Añadir los parámetros necesarios para el procedimiento almacenado
                    cmdActualizar.Parameters.AddWithValue("@proveedorid", proveedorId);
                    cmdActualizar.Parameters.AddWithValue("@nombre", txtNombre.Text);
                    cmdActualizar.Parameters.AddWithValue("@direccion", txtDireccion.Text);
                    cmdActualizar.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                    cmdActualizar.Parameters.AddWithValue("@activo", chkEstado.Checked);

                    if (conexion.State == ConnectionState.Closed)
                    {
                        conexion.Open();
                    }

                    //Ejecutar comando de registro.
                    cmdActualizar.ExecuteNonQuery();

                    CargarDatosProveedor();

                    LimpiarCampos();

                    MessageBox.Show("Proveedor actualizado exitosamente", "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                if (conexion.State == ConnectionState.Open)
                {
                    conexion.Close();
                }
            }
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtDireccion.Clear();
            txtTelefono.Clear();
            chkEstado.Checked = false;
        }


        private void txtTelefono_Leave(object sender, EventArgs e)
        {

            if (txtTelefono.Text.Length != 9)
            {
                MessageBox.Show("El formato del Telefono debe ser 0000-0000", "Formato Incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cmdSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CargarDatosProveedor()
        {
            try
            {
                using (SqlCommand cmdCargar = new SqlCommand("spProveedorActivos", conexion))
                {
                    cmdCargar.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmdCargar);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgcrudProv.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al cargar los datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void txtTexto_TextChanged(object sender, EventArgs e)
        {

            if (txtTexto.Text.Length == 0)
            {
                dtleer.DefaultView.RowFilter = ""; 
            }
            else
            {
                if (dtleer.Columns[cmbCampo.Text].DataType == typeof(string))
                {
                    dtleer.DefaultView.RowFilter = cmbCampo.Text + " LIKE '%" + txtTexto.Text + "%'";
                }
                else
                {
                    int numero;
                    if (int.TryParse(txtTexto.Text, out numero))
                    {
                        dtleer.DefaultView.RowFilter = cmbCampo.Text + " = " + numero;
                    }
                    else
                    {
                        dtleer.DefaultView.RowFilter = "1 = 0";
                    }
                }
            }

            dgcrudProv.DataSource = dtleer.DefaultView.ToTable();
        }

        private void cmbOpcion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string spNombre = "";

                switch (cmbOpcion.SelectedItem.ToString())
                {
                    case "Todos":

                        spNombre = "spProveedorSelect";
                        break;

                    case "Activos":

                        spNombre = "spProveedorActivos";
                        break;

                    case "Inactivos":

                        spNombre = "spProveedorInactivos";
                        break;
                }

                if (!string.IsNullOrEmpty(spNombre))
                {
                    if (conexion.State == ConnectionState.Closed)
                    {
                        conexion.Open();
                    }
                    // Configura el SqlDataAdapter con el stored procedure seleccionado
                    adpLeer = new SqlDataAdapter(spNombre, conexion);
                    adpLeer.SelectCommand.CommandType = CommandType.StoredProcedure;

                    dtleer.Clear();
                    adpLeer.Fill(dtleer);
                    dgcrudProv.DataSource = dtleer;

                    conexion.Close();
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

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgcrudProv.SelectedRows.Count > 0)
            {
                int Proveedorid = (int)dtleer.DefaultView[dgcrudProv.CurrentRow.Index]["Proveedorid"];

                if (MessageBox.Show("Desea deshabilitar el Proveedor", "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand("spProveedorDesactivar", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@proveedorid", Proveedorid);

                    try
                    {
                        // Verificar si la conexión está cerrada antes de abrirla
                        if (conexion.State == ConnectionState.Closed)
                        {
                            conexion.Open();
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
                        if (conexion.State == ConnectionState.Open)
                        {
                            conexion.Close();
                        }
                    }

                    dtleer.Clear();
                    adpLeer.Fill(dtleer);

                    // Buscar la fila con el mismo Socioid y seleccionarla
                    foreach (DataGridViewRow row in dgcrudProv.Rows)
                    {
                        if ((int)row.Cells["Proveedorid"].Value == Proveedorid)
                        {
                            row.Selected = true;
                                dgcrudProv.CurrentCell = row.Cells[0]; // Establecer la celda actual para que se muestre
                            break;
                        }
                    }
                }
            }
        }
    }
}
