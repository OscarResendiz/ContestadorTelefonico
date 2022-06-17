using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ContestadorTelefonico
{
    public partial class FormTelefono : Form
    {
        public FormTelefono()
        {
            InitializeComponent();
        }
        public string Nombre
        {
            get
            {
                return TxtNombre.Text;
            }
            set
            {
                TxtNombre.Text=value;
            }
        }
        public string Numero
        {
            get
            {
                return TxtNumero.Text;
            }
            set
            {
                TxtNumero.Text = value;
            }
        }
        public string Departamento
        {
            get
            {
                return TDepartamento.Text;
            }
            set
            {
                TDepartamento.Text = value;
            }
        }
        public string Comentarios
        {
            get
            {
                return TComentarios.Text;
            }
            set
            {
                TComentarios.Text = value;
            }
        }
    }
}
