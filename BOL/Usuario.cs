using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DAL;                      //Acceso a conexión Server + BD
using System.Data;              //Objetos manejadores datos
using System.Data.SqlClient;    //Acceso MSSQL Server
using ENTITIES;                 //Entidades (estructuras)

namespace BOL
{
    //Clase pública
    public class Usuario
    {
        //Instancia de la clase de conexión
        DBAccess conexion = new DBAccess();

        /// <summary>
        /// Inicia sesión utilizando datos del servidor
        /// </summary>
        /// <param name="email">Identificar o nombre de usuario</param>
        /// <returns>
        /// Objeto DataTable conteniendo toda la fila (varios campos)
        /// </returns>
        public DataTable iniciarSesion(string email)
        {
            //1. Objeto que contendrá el resultado
            DataTable dt = new DataTable();

            //2. Abrir conexión
            conexion.abrirConexion();

            //3. Objeto para enviar consulta
            SqlCommand comando = new SqlCommand("spu_usuarios_login", conexion.getConexion());

            //4. Tipo de comando (procedimiento almacenado)
            comando.CommandType = CommandType.StoredProcedure;

            //5. Pasar la(s) variable(s)
            comando.Parameters.AddWithValue("@email", email);

            //6. Ejecutar y obtener/leer los datos
            dt.Load(comando.ExecuteReader());

            //7. Cerrar
            conexion.cerrarConexion();

            //8. Retornamos el objeto con la info
            return dt;
        }

        public DataTable login(string email)
        {
            return conexion.listarDatosVariable("spu_usuarios_login", "@email", email);
        }

        public int Registrar(EUsuario entidad)
        {
            int totalRegistros = 0;
            SqlCommand comando = new SqlCommand("spu_usuarios_registrar", conexion.getConexion());
            comando.CommandType = CommandType.StoredProcedure;

            conexion.abrirConexion();

            try
            {
                comando.Parameters.AddWithValue("@apellidos", entidad.apellidos);
                comando.Parameters.AddWithValue("@nombres", entidad.nombres);
                comando.Parameters.AddWithValue("@email", entidad.email);
                comando.Parameters.AddWithValue("@claveacceso", entidad.claveacceso);
                comando.Parameters.AddWithValue("@nivelacceso", entidad.nivelAcceso);

                totalRegistros = comando.ExecuteNonQuery();
            }
            catch
            {
                totalRegistros = -1;
            }
            finally
            {
                conexion.cerrarConexion();
            }

            return totalRegistros;
        }

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            SqlCommand comando = new SqlCommand("spu_usuarios_listar", conexion.getConexion());
            comando.CommandType = CommandType.StoredProcedure;

            conexion.abrirConexion();
            dt.Load(comando.ExecuteReader());
            conexion.cerrarConexion();

            return dt;
        }
    }
}
