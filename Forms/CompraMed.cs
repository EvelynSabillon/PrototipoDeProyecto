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
    public partial class CompraMed : Form
    {

        SqlDataAdapter adpCompraMed;
        DataTable tabCompraMed;
        public CompraMed()
        {
            InitializeComponent();
        }

        public CompraMed(SqlConnection conexion, int CompraId)
        {
            InitializeComponent();


            adpCompraMed = new SqlDataAdapter();

            adpCompraMed.SelectCommand = new SqlCommand("spSocioSelect", conexion);
            adpCompraMed.SelectCommand.CommandType = CommandType.StoredProcedure;
            adpCompraMed.SelectCommand.Parameters.AddWithValue("@Socioid", CompraId);

            adpCompraMed.InsertCommand = comando("spSocioInsert", conexion);
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

        private void CompraMed_Load(object sender, EventArgs e)
        {

        }
    }
}
