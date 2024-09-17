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

namespace ProyectoCREL.Forms
{
    public partial class Concentrado : Form
    {
        SqlDataAdapter adpConcentrado;
        DataTable tabConcentrado;
        public Concentrado()
        {
            InitializeComponent();
        }

        public Concentrado(SqlConnection conexion, int ArticuloId)
        {
            InitializeComponent();

            adpConcentrado = new SqlDataAdapter();

            adpConcentrado.SelectCommand = new SqlCommand("spConcentradoSelect", conexion);
            adpConcentrado.SelectCommand.CommandType = CommandType.StoredProcedure;
            adpConcentrado.SelectCommand.Parameters.AddWithValue("@articuloid", ArticuloId);

            adpConcentrado.InsertCommand = comando("spConcentradoInsert", conexion);
            adpConcentrado.UpdateCommand = comando("spConcentradoUpdate", conexion);
        }


        private SqlCommand comando(string sql, SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@articuloid", SqlDbType.Int, 4, "ArticuloID");
            cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 50, "Nombre");
            cmd.Parameters.Add("@codigo", SqlDbType.VarChar, 50, "Codigo");
            cmd.Parameters.Add("@precio", SqlDbType.Float, 8, "Precio");
            cmd.Parameters.Add("@entrada", SqlDbType.Int, 4, "Entrada");
            cmd.Parameters.Add("@salida", SqlDbType.Int, 4, "Salida");
            cmd.Parameters.Add("@existencia", SqlDbType.Int, 4, "Existencia");
            cmd.Parameters.Add("@activo", SqlDbType.Bit, 1, "Activo");
            cmd.Parameters.Add("@costo", SqlDbType.Float, 8, "Costo");
            return cmd;
        }

        private void Concentrado_Load(object sender, EventArgs e)
        {
            try
            {
                txtProductoID.Enabled = false;

                tabConcentrado = new DataTable();
                adpConcentrado.Fill(tabConcentrado);

                if (tabConcentrado.Rows.Count == 0)
                {
                    tabConcentrado.Rows.Add();
                }
                else
                {
                    txtProductoID.Text = tabConcentrado.Rows[0]["ArticuloID"].ToString();
                    txtNombre.Text = tabConcentrado.Rows[0]["Nombre"].ToString();
                    txtCodigo.Text = tabConcentrado.Rows[0]["Codigo"].ToString();
                    txtPrecio.Text = tabConcentrado.Rows[0]["Precio"].ToString();
                    txtEntrada.Text = tabConcentrado.Rows[0]["Entrada"].ToString();
                    txtSalida.Text = tabConcentrado.Rows[0]["Salida"].ToString();
                    txtExistencia.Text = tabConcentrado.Rows[0]["Existencia"].ToString();
                    chkActivo.Checked = (bool)tabConcentrado.Rows[0]["Activo"];
                    txtCosto.Text = tabConcentrado.Rows[0]["Costo"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmdCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea salir sin guardar los cambios?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                    errorProvider1.SetError(txtNombre, "Falta el nombre del Producto");
                    errores = true;
                }

                if (txtCodigo.Text.Length == 0)
                {
                    errorProvider1.SetError(txtCodigo, "Falta el código del Producto");
                    errores = true;
                }

                if (txtPrecio.Text.Length == 0 || !decimal.TryParse(txtPrecio.Text, out _))
                {
                    errorProvider1.SetError(txtPrecio, "El precio es inválido");
                    errores = true;
                }

                if (txtExistencia.Text.Length == 0 || !int.TryParse(txtExistencia.Text, out _))
                {
                    errorProvider1.SetError(txtExistencia, "La existencia es inválida");
                    errores = true;
                }

                if (txtEntrada.Text.Length == 0 || !int.TryParse(txtEntrada.Text, out _))
                {
                    errorProvider1.SetError(txtEntrada, "La entrada es inválida");
                    errores = true;
                }

                if (txtSalida.Text.Length == 0 || !int.TryParse(txtSalida.Text, out _))
                {
                    errorProvider1.SetError(txtSalida, "La salida es inválida");
                    errores = true;
                }

                if (txtCosto.Text.Length == 0 || !decimal.TryParse(txtCosto.Text, out _))
                {
                    errorProvider1.SetError(txtCosto, "El costo es inválido");
                    errores = true;
                }

                if (!errores)
                {
                    if (MessageBox.Show("¿Desea guardar los cambios?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        tabConcentrado.Rows[0]["Nombre"] = txtNombre.Text;
                        tabConcentrado.Rows[0]["Codigo"] = txtCodigo.Text;
                        tabConcentrado.Rows[0]["Precio"] = txtPrecio.Text;
                        tabConcentrado.Rows[0]["Entrada"] = txtEntrada.Text;
                        tabConcentrado.Rows[0]["Salida"] = txtSalida.Text;
                        tabConcentrado.Rows[0]["Existencia"] = txtExistencia.Text;
                        tabConcentrado.Rows[0]["Activo"] = chkActivo.Checked;
                        tabConcentrado.Rows[0]["Costo"] = txtCosto.Text;

                        adpConcentrado.Update(tabConcentrado);
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
                        MessageBox.Show("El nombre debe ser unico, validar este campo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(ex.Errors[i].Message, ex.Errors[i].Number.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }
    }
}
