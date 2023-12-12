using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ENTITIES;
using BOL;
using DESIGNER.Tools;

namespace DESIGNER.Modulos
{
    public partial class frmProductos : Form
    {
        EProducto entProducto = new EProducto();
        Producto producto = new Producto();
        Categoria categoria = new Categoria();
        SubCategoria subcategoria = new SubCategoria();
        Marca marca = new Marca();

        //Bandera = variable LÓGICA que controla estados
        bool categoriasListas = false;

        public frmProductos()
        {
            InitializeComponent();
        }

        #region Métodos para carga de datos

        private void actualizarMarcas()
        {
            cboMarcas.DataSource = marca.listar();
            cboMarcas.DisplayMember = "marca";      //Mostrar | debe coincidir con la consulta
            cboMarcas.ValueMember = "idmarca";      //PK      | debe coincidir con la consulta
            cboMarcas.Refresh();
            cboMarcas.Text = "";
        }

        private void actualizarCategorias()
        {
            cboCategorias.DataSource = categoria.listar();
            cboCategorias.DisplayMember = "categoria";
            cboCategorias.ValueMember = "idcategoria";
            cboCategorias.Refresh();
            cboCategorias.Text = "";
        }

        private void actualizarProductos()
        {
            gridProductos.DataSource = producto.listar();
            gridProductos.Refresh();
        }

        #endregion

        private void frmProductos_Load(object sender, EventArgs e)
        {
            actualizarProductos();
            actualizarMarcas();
            actualizarCategorias();

            gridProductos.Columns[0].Visible = false;
            gridProductos.Columns[1].Width = 130;
            gridProductos.Columns[2].Width = 130;
            gridProductos.Columns[3].Width = 150;
            gridProductos.Columns[4].Width = 240;
            gridProductos.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridProductos.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridProductos.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            categoriasListas = true;
        }

        private void cboCategorias_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (categoriasListas)
            {
                //Invocar al método que carga las subcategorias
                int idcategoria = Convert.ToInt32(cboCategorias.SelectedValue.ToString());
                cboSubCategorias.DataSource = subcategoria.listar(idcategoria);
                cboSubCategorias.DisplayMember = "subcategoria";
                cboSubCategorias.ValueMember = "idsubcategoria";
                cboSubCategorias.Refresh();
                cboSubCategorias.Text = "";
            }
        }

        private void gridProductos_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            gridProductos.ClearSelection();
        }

        private void reiniciarInterfaz()
        {
            cboMarcas.Text = "";
            cboCategorias.Text = "";
            cboSubCategorias.Text = "";
            txtGarantia.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
        }


        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Aviso.Preguntar("¿Está seguro de registrar un nuevo producto?") == DialogResult.Yes)
            {
                entProducto.idmarca = Convert.ToInt32(cboMarcas.SelectedValue.ToString());
                entProducto.idsubcategoria = Convert.ToInt32(cboSubCategorias.SelectedValue.ToString());
                entProducto.descripcion = txtDescripcion.Text;
                entProducto.garantia = Convert.ToInt32(txtGarantia.Text);
                entProducto.precio = Convert.ToDouble(txtPrecio.Text);
                entProducto.stock = Convert.ToInt32(txtStock.Text);

                if (producto.registrar(entProducto) > 0)
                {
                    reiniciarInterfaz();
                    actualizarProductos();
                }
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            //1. Crear instancia del reporte (CrystalReport)
            Reportes.rptProductos reporte = new Reportes.rptProductos();

            //2. Asignar los datos al objeto reporte (creado en el paso 1)
            reporte.SetDataSource(producto.listar());
            reporte.Refresh();

            //3. Instanciar el formulario donde se mostrarán los reportes
            Reportes.frmVisorReportes formulario = new Reportes.frmVisorReportes();

            //4. Pasamos el reporte al visor
            formulario.visor.ReportSource = reporte;
            formulario.visor.Refresh();

            //5. Mostramos el formulario conteniendo el reporte
            formulario.ShowDialog();
        }

        /// <summary>
        /// Genera un archivo físico del reporte
        /// </summary>
        /// <param name="extension">Utilice: XLS o PDF</param>
        private void ExportarDatos(string extension)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Title = "Reporte de productos";
            sd.Filter = $"Archivos en formato {extension.ToUpper()}|*.{extension.ToLower()}";

            if (sd.ShowDialog() == DialogResult.OK)
            {
                //Creamos una versión del reporte en formato PDF
                //1. Instancia del objeto reporte (CrystalReport)
                Reportes.rptProductos reporte = new Reportes.rptProductos();

                //2. Asignar los datos al objeto reporte (creado en el paso 1)
                reporte.SetDataSource(producto.listar());
                reporte.Refresh();

                //3. El reporte creado y en memoria se ESCRIBIRÁ en el STORAGE
                if (extension.ToUpper() == "PDF")
                {
                    reporte.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, sd.FileName);
                }
                else if (extension.ToUpper() == "XLSX")
                {
                    reporte.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.ExcelWorkbook, sd.FileName);
                }

                //4. Notificar al usuario la creación del archivo
                Aviso.Informar("Se ha creado el reporte correctamente");
            }
        }

        private void btnExportarPDF_Click(object sender, EventArgs e)
        {
            ExportarDatos("PDF");
        }

        private void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExportarDatos("XLSX");
        }
    }
}
