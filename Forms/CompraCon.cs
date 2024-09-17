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
    public partial class CompraCon : Form
    {

        SqlDataAdapter adpCompraCon;
        DataTable tabCompraCon;

        public CompraCon()
        {
            InitializeComponent();
        }

        public CompraCon(SqlConnection conexion, int CompraId)
        {
            InitializeComponent();


            adpCompraCon = new SqlDataAdapter();

            adpCompraCon.SelectCommand = new SqlCommand("spSocioSelect", conexion);
            adpCompraCon.SelectCommand.CommandType = CommandType.StoredProcedure;
            adpCompraCon.SelectCommand.Parameters.AddWithValue("@Socioid", CompraId);

            adpCompraCon.InsertCommand = comando("spSocioInsert", conexion);
            adpCompraCon.UpdateCommand = comando("spSocioUpdate", conexion);

        }

        private SqlCommand comando(String sql, SqlConnection con)
        {
            //Metodo para evitar escirbir el command type  y setear parametros
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@socioid", SqlDbType.Int, 4, "SocioID");
            cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 50, "Nombre");
            cmd.Parameters.Add("@direccion", SqlDbType.VarChar, 100, "Direccion");
            cmd.Parameters.Add("@telefono", SqlDbType.VarChar, 20, "Telefono");
            cmd.Parameters.Add("@activo", SqlDbType.Bit, 1, "Activo");
            cmd.Parameters.Add("@dni", SqlDbType.VarChar, 20, "DNI");
            return cmd;
        }


        private void CompraCon_Load(object sender, EventArgs e)
        {

        }
    }
}
