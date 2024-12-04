﻿using canchacubo.clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace canchacubo
{
    public partial class editarpromocion : Form
    {
        DataTable dtpromociones = new DataTable();
        clsPromocion promo = new clsPromocion();
        string idpromocion;
        public event EventHandler EdicionExitosa;
        public editarpromocion()
        {
            InitializeComponent();
            CargarPromocionesEnComboBox();           
            cbx_estado.DropDownStyle = ComboBoxStyle.DropDownList;  // Deshabilita la edición
            cbx_promociones.DropDownStyle = ComboBoxStyle.DropDownList;  // Deshabilita la edición
        }

        private void btn_volver_Click(object sender, EventArgs e)
        {
            promociones promo = new promociones();
            promo.Show();
            this.Hide();
        }

        private void btn_editar_Click(object sender, EventArgs e)
        {
            String descuento = txt_descuento.Text;
            String estado ;
            DateTime fechainicio = obtenerFechaideal();
            DateTime fechafin = dtp_fechafin.Value.Date;
            if (string.IsNullOrEmpty(descuento) || dtp_fechainicio.Checked == false || cbx_estado.SelectedIndex == -1)
            {

                MessageBox.Show("Debe diligenciar todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                estado = obtenerestado();
            }
            this.EdicionExitosa += RefrescarPromociones;
            bool resultado = promo.EditarPromocion(idpromocion,fechainicio, fechafin, estado, descuento);
            if (resultado)
            {              
                // Si el registro fue exitoso, disparamos el evento edicionRegistrado
                MessageBox.Show("Promocion editada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EdicionExitosa?.Invoke(this, EventArgs.Empty); // Dispara el evento si no es null               
            }

        }
        public void RefrescarPromociones(object sender, EventArgs e)
        {
            CargarPromocionesEnComboBox();
        }
        private void RecargarDatosPromocion()
        {
            try
            {
                dtpromociones = promo.ObtenerTablapPromociones();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al recargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarPromocionesEnComboBox()

        {

            RecargarDatosPromocion();  // Cargar los datos en dtpromociones


            // Verificar si la columna "InformacionPromo" ya existe para evitar errores

            if (!dtpromociones.Columns.Contains("InformacionPromo"))

            {

                dtpromociones.Columns.Add("InformacionPromo", typeof(string));

            }


            // Formatear cada fila existente con el formato deseado para mostrar en el ComboBox

            foreach (DataRow row in dtpromociones.Rows)

            {
                string idepromocion = row["identificador"].ToString();

                string descuento = row["descuento"].ToString();

                string informacionPromo = $"Promocion: {idepromocion} -- {descuento} %";

                row["InformacionPromo"] = informacionPromo;

            }
            // Crear una fila para la opción "Ninguno" y agregarla como la primera fila

            DataRow rowNinguno = dtpromociones.NewRow();

            rowNinguno["identificador"] = DBNull.Value;  // Valor de identificador vacío

            rowNinguno["descuento"] = DBNull.Value;      // Valor de descuento vacío

            rowNinguno["InformacionPromo"] = "";

            dtpromociones.Rows.InsertAt(rowNinguno, 0);

            // Asignar la DataSource y definir DisplayMember y ValueMember

            cbx_promociones.DataSource = dtpromociones;

            cbx_promociones.DisplayMember = "InformacionPromo";

            cbx_promociones.ValueMember = "identificador";  // Permite obtener el ID de la promoción seleccionada

        }

        private void cbx_promociones_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Verifica que haya una selección válida
            if (cbx_promociones.SelectedItem is DataRowView selectedRow)
            {

                // Verifica si la opción seleccionada es "Ninguno"
                if (selectedRow["InformacionPromo"].ToString() != "")
                {
                    idpromocion = selectedRow["identificador"].ToString();


                }
            }           
        }

        private DateTime obtenerFechaideal()
        {
            DateTime fechaSeleccionada = dtp_fechainicio.Value.Date;//obtener solo la fecha
            if (dtp_fechainicio.Checked == false)
            {
                fechaSeleccionada = DateTime.Now.Date;
            }
            return fechaSeleccionada;
        }
        private String obtenerestado()
        {
            string estado = cbx_estado.SelectedItem.ToString(); ;//obtener estado
            if (estado == "Activo")
            {
                return "1";
            }
            else return "0";

        }
       
    }
}
