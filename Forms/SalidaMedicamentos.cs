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
    public partial class SalidaMedicamentos : Form
    {
        SqlDataAdapter adpSalidasMed;
        DataTable tabSalidasMed;

        public SalidaMedicamentos()
        {
            InitializeComponent();
        }

        public SalidaMedicamentos(SqlConnection conexion, int Salidaid)
        {
            InitializeComponent();


            adpSalidasMed = new SqlDataAdapter();

            adpSalidasMed.SelectCommand = new SqlCommand("spSocioSelect", conexion);
            adpSalidasMed.SelectCommand.CommandType = CommandType.StoredProcedure;
            adpSalidasMed.SelectCommand.Parameters.AddWithValue("@Socioid", Salidaid);

            adpSalidasMed.InsertCommand = comando("spSocioInsert", conexion);
            adpSalidasMed.UpdateCommand = comando("spSocioUpdate", conexion);

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


        private void SalidaMedicamentos_Load(object sender, EventArgs e)
        {

        }
    }
}
