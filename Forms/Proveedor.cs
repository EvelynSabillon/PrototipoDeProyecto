using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace ProyectoCREL.Forms
{
    public partial class Proveedor : Form
    {

        private SqlConnection conexion;
        

        public Proveedor()
        {
            InitializeComponent();
        }

        //segundo constructor
      
        public Proveedor(SqlConnection conexion)
        {
            InitializeComponent();
            this.conexion = conexion;

        }

        private void btnAgregarProv_Click(object sender, EventArgs e)
        {
            // Validar que los campos requeridos no estén vacíos
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDireccion.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text) )
            {
                MessageBox.Show("Inserte todos los campos requeridos de manera correcta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }

            try
            {
                using (SqlCommand cmdInsertar = new SqlCommand("spProveedorInsert", conexion))
                {
                    cmdInsertar.CommandType = CommandType.StoredProcedure;

                    cmdInsertar.Parameters.AddWithValue("@nombre", txtNombre.Text);
                    cmdInsertar.Parameters.AddWithValue("@direccion", txtDireccion.Text);
                    cmdInsertar.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                    cmdInsertar.Parameters.AddWithValue("@activo", chkEstado.Checked);

                    if (conexion.State == ConnectionState.Closed)
                    {
                        conexion.Open();
                    }

                    cmdInsertar.ExecuteNonQuery();
                    MessageBox.Show("Proveedor creado exitosamente", "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Information); 
                    Close();
                }
            }
            catch (SqlException ex)
            {
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    if (ex.Errors[i].Number == 2627)
                    {
                        MessageBox.Show("El nombre debe ser unico, validar este campo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(ex.Errors[i].Message, ex.Errors[i].Number.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            finally
            {
                // Cerrar la conexión 
                if (conexion.State == ConnectionState.Open)
                {
                    conexion.Close();
                }
            }
        }


        private void cmdCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Desea salir sin guardar los cambios?", "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
               this.Close();
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

        private void txtTelefono_Leave(object sender, EventArgs e)
        {
            if (txtTelefono.Text.Length != 9)
            {
                MessageBox.Show("El formato del Telefono debe ser 0000-0000", "Formato Incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Proveedor_Load(object sender, EventArgs e)
        {

        }
    }
}
