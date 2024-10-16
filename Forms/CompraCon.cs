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
    public partial class CompraCon : Form
    {

        
        SqlConnection con;
        SqlParameter prmCompraConDetalle;
        SqlDataAdapter adpCompraCon;
        SqlDataAdapter adpCompraConDetalle;

        DataSet dsTablas;

        public CompraCon()
        {
            InitializeComponent();
        }

        public CompraCon(SqlConnection conexion, int CompraId)
        {

            InitializeComponent();

            cmbCampo.SelectedIndex = 0;


            prmCompraConDetalle = new SqlParameter();
            prmCompraConDetalle.ParameterName = "@compraid";
            prmCompraConDetalle.Value = 0;

            adpCompraCon = new SqlDataAdapter();

            adpCompraCon.SelectCommand = new SqlCommand("spCompraConSelect", conexion);
            adpCompraCon.SelectCommand.CommandType = CommandType.StoredProcedure;
            adpCompraCon.SelectCommand.Parameters.AddWithValue("@compraid", CompraId);

            adpCompraCon.InsertCommand = comando("spCompraConInsert", conexion);
            adpCompraCon.UpdateCommand = comando("spCompraConUpdate", conexion);


            adpCompraConDetalle = new SqlDataAdapter("spCompraDetalleConSelect", conexion);
            adpCompraConDetalle.SelectCommand.CommandType = CommandType.StoredProcedure;
            adpCompraConDetalle.SelectCommand.Parameters.AddWithValue("@compraid", CompraId);


            adpCompraConDetalle.InsertCommand = new SqlCommand("spCompraDetalleConInsert", conexion);
            adpCompraConDetalle.InsertCommand.CommandType = CommandType.StoredProcedure;
            adpCompraConDetalle.InsertCommand.Parameters.Add(prmCompraConDetalle);
            adpCompraConDetalle.InsertCommand.Parameters.Add("@articuloid", SqlDbType.Int, 4, "ArticuloID");
            adpCompraConDetalle.InsertCommand.Parameters.Add("@cantidad", SqlDbType.Int, 4, "Cantidad");
            adpCompraConDetalle.InsertCommand.Parameters.Add("@costo", SqlDbType.Float, 8, "Costo");
            adpCompraConDetalle.InsertCommand.Parameters.Add("@activo", SqlDbType.Bit, 1, "Activo");

            adpCompraConDetalle.UpdateCommand = new SqlCommand("spCompraDetalleConUpdate", conexion);
            adpCompraConDetalle.UpdateCommand.CommandType = CommandType.StoredProcedure;
            adpCompraConDetalle.UpdateCommand.Parameters.Add("@compradetid", SqlDbType.Int, 4, "CompraDetID");
            adpCompraConDetalle.UpdateCommand.Parameters.Add("@compraid", SqlDbType.Int, 4, "CompraID");
            adpCompraConDetalle.UpdateCommand.Parameters.Add("@articuloid", SqlDbType.Int, 4, "ArticuloID");
            adpCompraConDetalle.UpdateCommand.Parameters.Add("@cantidad", SqlDbType.Int, 4, "Cantidad");
            adpCompraConDetalle.UpdateCommand.Parameters.Add("@costo", SqlDbType.Float, 8, "Costo");
            adpCompraConDetalle.UpdateCommand.Parameters.Add("@activo", SqlDbType.Bit, 1, "Activo");

            con = conexion;

        }

        private SqlCommand comando(String sql, SqlConnection con)
        {
            //Metodo para evitar escirbir el command type  y setear parametros
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@compraid", SqlDbType.Int, 4, "CompraID").Direction = ParameterDirection.InputOutput;
            cmd.Parameters.Add("@proveedorid", SqlDbType.Int, 4, "ProveedorID");
            cmd.Parameters.Add("@fecha", SqlDbType.DateTime, 8, "Fecha");
            cmd.Parameters.Add("@documento", SqlDbType.VarChar, 20, "Documento");
            cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 10, "Tipo");
            cmd.Parameters.Add("@activo", SqlDbType.Bit, 1, "Activo");
            return cmd;
        }


        private void CompraCon_Load(object sender, EventArgs e)
        {
            txtTexto.Enabled = false;

            cmbTipo.Items.Add("Contado");
            cmbTipo.Items.Add("Pendiente");

            try
            {
                dataGridView1.ReadOnly = false;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.AllowUserToAddRows = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;

                dsTablas = new DataSet();
                dsTablas.Tables.Add("CompraCon");
                dsTablas.Tables.Add("CompraDetalleCon");

                adpCompraCon.Fill(dsTablas.Tables["CompraCon"]);
                adpCompraConDetalle.Fill(dsTablas.Tables["CompraDetalleCon"]);

                if(dsTablas.Tables["CompraCon"].Rows.Count == 0)
                {
                    dsTablas.Tables["CompraCon"].Rows.Add();
                    txtCompraID.Enabled = false;
                }
                else
                {
                    txtCompraID.Enabled = false;
                    txtCompraID.Text = dsTablas.Tables["CompraCon"].Rows[0]["compraid"].ToString();
                    txtProveedorID.Text = dsTablas.Tables["CompraCon"].Rows[0]["proveedorid"].ToString();
                    dtpFecha.Value = (DateTime)dsTablas.Tables["CompraCon"].Rows[0]["fecha"];
                    txtDocumento.Text = dsTablas.Tables["CompraCon"].Rows[0]["documento"].ToString();
                    cmbTipo.SelectedValue= dsTablas.Tables["CompraCon"].Rows[0]["tipo"].ToString();
                    chkActivo.Checked = (bool)dsTablas.Tables["CompraCon"].Rows[0]["activo"];
                }

                dataGridView1.DataSource = dsTablas.Tables["CompraDetalleCon"];
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void cmdCrearProv_Click(object sender, EventArgs e)
        {

            Form newProv = new Proveedor(con);
            newProv.ShowDialog();
        }

        public void SetProveedorID(int proveedorID)
        {
            txtProveedorID.Text = proveedorID.ToString(); // Asigna el ID al textbox
        }

        private void cmdVerProv_Click(object sender, EventArgs e)
        {
            try
            {
                // Pasamos la referencia de CompraCon al constructor de VistaProveedores
                VistaProveedores frm = new VistaProveedores(con, this);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Cancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea salir sin guardar los cambios?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Dispose();
        }

        private void cmbCampo_Click(object sender, EventArgs e)
        {
            txtTexto.Enabled = true;
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                String col = dataGridView1.Columns[e.ColumnIndex].Name.ToLower();

                if (col == "productoid")
                {
                    if(e.FormattedValue.ToString().Length > 0 )
                    {
                        SqlDataAdapter adpArticulo = new SqlDataAdapter("spCompraConArticulosActivosSelect" + e.FormattedValue, con);
                        DataTable dtArticulo = new DataTable();
                        adpArticulo.Fill(dtArticulo);

                        if(dtArticulo.Rows.Count == 0)
                        {
                            MessageBox.Show("El artículo no existe o no está activo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                        }
                        else
                        {
                            dataGridView1.Refresh();
                        }
                    }
                }
                else if (col == "cantidad")
                {
                    int cantidad = Convert.ToInt32(e.FormattedValue);
                    if (cantidad <= 0)
                    {
                        MessageBox.Show("La cantidad debe ser mayor a cero", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
                else if (col == "costo")
                {
                    double costo = Convert.ToDouble(e.FormattedValue);
                    if (costo <= 0)
                    {
                        MessageBox.Show("El costo debe ser mayor a cero", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
                else if (col == "activo")
                {
                    bool activo = Convert.ToBoolean(e.FormattedValue);
                    if (activo == false)
                    {
                        MessageBox.Show("El campo activo no puede ser falso", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }

            }
            catch (Exception ex)
            {
                //Mensaje de error
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void cmdGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                bool errores = false;
                errorProvider1.Clear();

                if (txtProveedorID.Text.Length == 0)
                {
                    errorProvider1.SetError(txtProveedorID, "Debe seleccionar un proveedor");
                    errores = true;
                }

                if(txtDocumento.Text.Length == 0)
                {
                    errorProvider1.SetError(txtDocumento, "Debe ingresar un numero de documento");
                    errores = true;
                }

                if(dtpFecha.Value > DateTime.Now)
                {
                    errorProvider1.SetError(dtpFecha, "La fecha no puede ser mayor a la fecha actual");
                    errores = true;
                }

                if (cmbTipo.SelectedIndex == -1)
                {
                    errorProvider1.SetError(cmbTipo, "Debe seleccionar un tipo de compra");
                    errores = true;
                }

                if (!errores)
                {
                    try
                    {
                        if (MessageBox.Show("Desea guardar los cambios?", "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            int compraID;

                            if(string.IsNullOrEmpty(txtCompraID.Text))
                            {
                                compraID = InsertarCompra();
                                txtCompraID.Text = compraID.ToString();
                            }
                            else
                            {
                                compraID = Convert.ToInt32(txtCompraID.Text);
                                dsTablas.Tables["CompraCon"].Rows[0]["compraid"] = compraID;
                                dsTablas.Tables["CompraCon"].Rows[0]["proveedorid"] = txtProveedorID;
                                dsTablas.Tables["CompraCon"].Rows[0]["fecha"] = dtpFecha.Value;
                                dsTablas.Tables["CompraCon"].Rows[0]["documento"] = txtDocumento.Text;
                                dsTablas.Tables["CompraCon"].Rows[0]["tipo"] = cmbTipo.SelectedValue;
                                dsTablas.Tables["CompraCon"].Rows[0]["activo"] = chkActivo.Checked;

                                adpCompraCon.Update(dsTablas.Tables["CompraCon"]);
                            }

                            prmCompraConDetalle.Value = txtCompraID.Text;
                            adpCompraConDetalle.Update(dsTablas.Tables["CompraDetalleCon"]);
                            
                            Close();

                            if(dataGridView1.Rows.Count > 0 )
                            {
                                int lastRowIndex = dataGridView1.Rows.Count - 1;
                                dataGridView1.CurrentCell = dataGridView1.Rows[lastRowIndex].Cells[1];
                                dataGridView1.Rows[lastRowIndex].Selected = true;
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                        //mensaje de error
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            catch (SqlException ex)
            {
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    if (ex.Errors[i].Number == 515)
                    {
                        MessageBox.Show("Por favor validar que el campo activo en el detalle este marcado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(ex.Errors[i].Message, ex.Errors[i].Number.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private int InsertarCompra()
        {

            string servidor = ".\\SQLEXPRESS";
            string bd = "CREL";
            string usuario = "sa";
            string pw = "123456789";

            String connectionString = $"Server={servidor};Database={bd};User Id={usuario};Password={pw};";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using(SqlCommand cmd = new SqlCommand("spCompraConInsert", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter outputIdParam = new SqlParameter("@compraid", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputIdParam);
                    cmd.Parameters.AddWithValue("@proveedorid",txtProveedorID.Text);
                    cmd.Parameters.AddWithValue("@fecha", dtpFecha.Value);
                    cmd.Parameters.AddWithValue("@documento",txtDocumento.Text);
                    if (cmbTipo.SelectedItem != null)
                    {
                        cmd.Parameters.AddWithValue("@tipo", cmbTipo.SelectedItem.ToString()); // Usar el texto del item seleccionado
                    }
                    else
                    {
                        MessageBox.Show("Por favor selecciona un tipo válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1; // Salir si no hay valor válido
                    }
                    cmd.Parameters.AddWithValue("@activo", chkActivo.Checked);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    return Convert.ToInt32(outputIdParam.Value);
                }
            }
                
        }
    }
}
