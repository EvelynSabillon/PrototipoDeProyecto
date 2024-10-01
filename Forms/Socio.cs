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


namespace ProyectoCREL.Forms
{
    public partial class Socio : Form
    {
        SqlDataAdapter adpSocio;
        DataTable tabSocio;


        public Socio()
        {
            InitializeComponent();
        }

        public Socio(SqlConnection conexion, int SocioId )
        {
            InitializeComponent();

            
            adpSocio = new SqlDataAdapter();

            adpSocio.SelectCommand = new SqlCommand("spSocioSelect", conexion);
            adpSocio.SelectCommand.CommandType = CommandType.StoredProcedure;
            adpSocio.SelectCommand.Parameters.AddWithValue("@Socioid", SocioId);

            adpSocio.InsertCommand = comando("spSocioInsert", conexion);
            adpSocio.UpdateCommand = comando("spSocioUpdate", conexion);

        }

        private SqlCommand comando(String sql, SqlConnection con)
        {
            //Metodo para evitar escirbir el command type  y setear parametros
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 50, "Nombre");
            cmd.Parameters.Add("@direccion", SqlDbType.VarChar, 100, "Direccion");
            cmd.Parameters.Add("@telefono", SqlDbType.VarChar, 20, "Telefono");
            cmd.Parameters.Add("@activo", SqlDbType.Bit, 1, "Activo");
            return cmd;
        }


        private void cmdCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Desea salir sin guardar los cambios?", "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Dispose();
        }

        private void cmdGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                bool errores = false;
                errorProvider1.Clear();


                if (txtNombre.Text.Length == 0)
                {
                    errorProvider1.SetError(txtNombre, "Falta el nombre del Socio");
                    errores = true;
                }
                else
                {
                    if (txtNombre.Text.Any(char.IsDigit))
                    {
                        errorProvider1.SetError(txtNombre, "El nombre no debe contener números");
                        errores = true;
                    }
                }

                if (txtDireccion.Text.Length == 0)
                {
                    errorProvider1.SetError(txtDireccion, "Falta la direccion del Socio");
                    errores = true;
                }

                if (txtTelefono.Text.Length != 9)
                {
                    errorProvider1.SetError(txtTelefono, "Falta el telefono del Socio o no es valido");
                    errores = true;
                }


                if (chkActivo.Checked == false)
                {
                    errorProvider1.SetError(chkActivo, "Marque la casilla de activo");
                }

                if (!errores)
                {
                    if(MessageBox.Show("Desea guardar los cambios?", "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        String[] sexos = { "M", "F" };

                        tabSocio.Rows[0]["nombre"] = txtNombre.Text;
                        tabSocio.Rows[0]["direccion"] = txtDireccion.Text;
                        tabSocio.Rows[0]["telefono"] = txtTelefono.Text;
                        
                        tabSocio.Rows[0]["activo"] = chkActivo.Checked;

                        adpSocio.Update(tabSocio);
                        Close();

                    }
                }

            }
            catch (SqlException ex)
            {
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    if (ex.Errors[i].Number == 2627)
                    {
                        MessageBox.Show("El numero de telefono debe ser unico, validar este campo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(ex.Errors[i].Message, ex.Errors[i].Number.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
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

        private void Socio_Load(object sender, EventArgs e)
        {
            try
            {

                tabSocio = new DataTable();
                adpSocio.Fill(tabSocio);


                if (tabSocio.Rows.Count == 0)
                {
                    tabSocio.Rows.Add();
                }
                else
                {
                    txtNombre.Text = tabSocio.Rows[0]["nombre"].ToString();
                    txtDireccion.Text = tabSocio.Rows[0]["direccion"].ToString();
                    txtTelefono.Text = tabSocio.Rows[0]["telefono"].ToString();
                    chkActivo.Checked = (bool)tabSocio.Rows[0]["activo"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
