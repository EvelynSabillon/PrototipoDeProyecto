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
    public partial class Medicamentos : Form
    {

        SqlDataAdapter adpMedicamentos;
        DataTable tabMedicamentos;
        public Medicamentos()
        {
            InitializeComponent();
        }

        public Medicamentos(SqlConnection conexion, int ArticuloId)
        {
            InitializeComponent();

            adpMedicamentos = new SqlDataAdapter();

            adpMedicamentos.SelectCommand = new SqlCommand("spMedicamentosSelect", conexion);
            adpMedicamentos.SelectCommand.CommandType = CommandType.StoredProcedure;
            adpMedicamentos.SelectCommand.Parameters.AddWithValue("@articuloid", ArticuloId);

            adpMedicamentos.InsertCommand = comando("spMedicamentosInsert", conexion);
            adpMedicamentos.UpdateCommand = comando("spMedicamentosUpdate", conexion);
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


        private void Medicamentos_Load(object sender, EventArgs e)
        {
            try
            {
                txtArticuloID.Enabled = false;

                tabMedicamentos = new DataTable();
                adpMedicamentos.Fill(tabMedicamentos);

                if (tabMedicamentos.Rows.Count == 0)
                {
                    tabMedicamentos.Rows.Add();
                }
                else
                {
                    txtArticuloID.Text = tabMedicamentos.Rows[0]["ArticuloID"].ToString();
                    txtNombre.Text = tabMedicamentos.Rows[0]["Nombre"].ToString();
                    txtCodigo.Text = tabMedicamentos.Rows[0]["Codigo"].ToString();
                    txtPrecio.Text = tabMedicamentos.Rows[0]["Precio"].ToString();
                    txtEntrada.Text = tabMedicamentos.Rows[0]["Entrada"].ToString();
                    txtSalida.Text = tabMedicamentos.Rows[0]["Salida"].ToString();
                    txtExistencia.Text = tabMedicamentos.Rows[0]["Existencia"].ToString();
                    chkActivo.Checked = (bool)tabMedicamentos.Rows[0]["Activo"];
                    txtCosto.Text = tabMedicamentos.Rows[0]["Costo"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmdGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                bool errores = false;
                errorProvider1.Clear();

                if (txtNombre.Text.Length == 0)
                {
                    errorProvider1.SetError(txtNombre, "Falta el nombre del Articulo");
                    errores = true;
                }

                if (txtCodigo.Text.Length == 0)
                {
                    errorProvider1.SetError(txtCodigo, "Falta el código del Articulo");
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
                        tabMedicamentos.Rows[0]["Nombre"] = txtNombre.Text;
                        tabMedicamentos.Rows[0]["Codigo"] = txtCodigo.Text;
                        tabMedicamentos.Rows[0]["Precio"] = txtPrecio.Text;
                        tabMedicamentos.Rows[0]["Entrada"] = txtEntrada.Text;
                        tabMedicamentos.Rows[0]["Salida"] = txtSalida.Text;
                        tabMedicamentos.Rows[0]["Existencia"] = txtExistencia.Text;
                        tabMedicamentos.Rows[0]["Activo"] = chkActivo.Checked;
                        tabMedicamentos.Rows[0]["Costo"] = txtCosto.Text;

                        adpMedicamentos.Update(tabMedicamentos);
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

        private void cmdCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea salir sin guardar los cambios?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Dispose();
        }
    }
}
