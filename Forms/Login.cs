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
    public partial class Login : Form
    {

        private SqlConnection conexion;
        private bool conectado;

        public SqlConnection Conexion
        {
            get { return conexion; }
        }

        public bool Conectado
        {
            get { return conectado; }
        }

        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void cmdCancelar_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void cmdIngresar_Click(object sender, EventArgs e)
        {
            string servidor = ".\\SQLEXPRESS";
            string bd = "CREL";
            string usuario = txtUsuario.Text.Trim();
            string pw =  txtPassword.Text.Trim();

            
            string connectionString = $"Server={servidor};Database={bd};User Id={usuario};Password={pw};";

            try
            {
                conectado = false;
                conexion = new SqlConnection(connectionString);
                conexion.Open();
                conectado = true;
                MessageBox.Show("Se conecto correctamente a la base de datos","Conexion Exitosa",MessageBoxButtons.OK,MessageBoxIcon.Information);
                Dispose();

            }
            catch (SqlException ex)
            {
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    if (ex.Errors[i].Number == 18456)
                    {
                        MessageBox.Show("El usuario o contraseña es incorrecto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("No se logro conectar a la base de datos " + ex.ToString());
                    }
                }

            }
        }
    }
}
