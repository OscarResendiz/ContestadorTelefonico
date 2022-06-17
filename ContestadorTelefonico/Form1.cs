using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace ContestadorTelefonico
{
    public partial class Form1 : Form
    {
        PicUSB.PicUSBAPI usbapi = new PicUSB.PicUSBAPI();
        private string data;
        static ASCIIEncoding ASCIIEncoder = new ASCIIEncoding();
        public Form1()
        {
            InitializeComponent();
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //SE ALMACENA EL EVENTO ACTUAL DEL PUERTO EN LA VARIABLE DATA
            //data = ComPort.ReadExisting();
            //OBTENER FECHA PARA ESCRIBIR EL LA BITACORA
           // LMensaje.Text = data;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string port = "COM3";
            int baud = 9600;
            //cargo la lista de telefonos
            if (File.Exists("Telefonos.xml") == true)
            {
                try
                {
                    dataSet1.ReadXml("Telefonos.xml");
                }
                catch (System.Exception ex2)
                {
                    MessageBox.Show(ex2.Message,"Error en el XML");
                }
            }
            try
            {
                InitializeComPort(port, baud);
            }
            catch (System.Exception ex)
            {
                string s = "No se pudo conectar al modem" + ex.Message;
                notifyIcon1.ShowBalloonTip(500, "ERROR de puerto serial",s, ToolTipIcon.Info);
                //MessageBox.Show("No se pudo conectar al modem"+ex.Message);
                return;
            }
            //ACTIVAR IDENTIFICADOR DE LLAMADAS
            String ComandoATID;
            ComandoATID = "AT#CID=1";
//            ComPort.Write(ComandoATID + '\r');
        }
        private void InitializeComPort(string port, int baud)
        {
            //ComPort.Parity = Parity.None;
            //ComPort.StopBits = StopBits.One;
            //ComPort.DataBits = 8;
            //ComPort.Handshake = Handshake.None;
//            ComPort.Open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //comando de marcar el numero telefonico
            //primero mando el numero al pic
            string s = TTelefono.Text;
            int i, n;
            List<uint>  numeros = new List<uint>();
            n = s.Length;
            for (i = 0; i < n; i++)
            {
                switch (s[i])
                {
                    case '0':
                        numeros.Add(0x0A);
                        break;
                    case '1':
                        numeros.Add(0x01);
                        break;
                    case '2':
                        numeros.Add(0x02);
                        break;
                    case '3':
                        numeros.Add(0x03);
                        break;
                    case '4':
                        numeros.Add(0x04);
                        break;
                    case '5':
                        numeros.Add(0x05);
                        break;
                    case '6':
                        numeros.Add(0x06);
                        break;
                    case '7':
                        numeros.Add(0x07);
                        break;
                    case '8':
                        numeros.Add(0x08);
                        break;
                    case '9':
                        numeros.Add(0x09);
                        break;
                    case '*':
                        numeros.Add(0x0B);
                        break;
                    case '#':
                        numeros.Add(0x0C);
                        break;
                }
            }
            //hago el recorrido de los numeros que hay que enviar
            n = numeros.Count;
            //usbapi.SumaPIC(uint.Parse(sumando1.Text), uint.Parse(sumando2.Text)); 
            usbapi.SumaPIC((uint)0x04, (uint)n);
            //comienzo a enviar los numeros
            for (i = 0; i < n; i++)
            {
                usbapi.LedPIC(numeros[i]);
            }
            // ahora marco el numero telfonico
            usbapi.LedPIC(0x03);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //mando el comando para descolgar el telefono y escuchar la conversacion
            usbapi.LedPIC(0x01);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //cuelgo el telefono
            usbapi.LedPIC(0x00);


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //modifico la aplicacion para detectar si hay alguna llamada entrante
            uint x = usbapi.ResultadoPIC();
            if (x ==0x25)
            {
                //se detecto una llamada y mando el mesnaje
                notifyIcon1.ShowBalloonTip(500, "llmana", "Hay una llamada entrante", ToolTipIcon.Info);
            }
            //if (LMensaje.Text != data)
            //{
            //    LMensaje.Text = data;
            //    if (data == "\r\nRING\r\n")
            //    {
            //notifyIcon1.ShowBalloonTip(500, "llmana", "Hay una llamada entrante", ToolTipIcon.Info);
            //    }
            //}
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataSet1.Tables[0].Rows.Count > 0)
            {
               // dataSet1.WriteXml("Telefonos.xml");
            }
            if (Visible == true)
            {
                Visible = false;
                e.Cancel=true;
            }
        }

        private void marcarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Visible == false)
                Visible = true;
        }

        private void cerrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Visible = false;
            timer2.Enabled = false;
            timer1.Enabled = true;
        }

        private void notifyIcon1_BalloonTipClosed(object sender, EventArgs e)
        {
            LMensaje.Text = "";
            data = "";
        }




        //private void Lista_DoubleClick(object sender, EventArgs e)
        //{
        //    if (Lista.SelectedIndex == -1)
        //        return;
        //    = Lista.SelectedValue.ToString();
        //}

        private void BCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BEditar_Click(object sender, EventArgs e)
        {
            if (GridTelefonos.SelectedRows.Count <=0)
                return;
            int id;
            FormTelefono dlg = new FormTelefono();
            dlg.Numero = GridTelefonos.SelectedRows[0].Cells[0].Value.ToString();
            dlg.Nombre = GridTelefonos.SelectedRows[0].Cells[1].Value.ToString();
            dlg.Departamento= GridTelefonos.SelectedRows[0].Cells[2].Value.ToString();
            dlg.Comentarios = GridTelefonos.SelectedRows[0].Cells[3].Value.ToString();
            id = int.Parse(GridTelefonos.SelectedRows[0].Cells[4].Value.ToString());
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            dataSet1.Tables["Telefonos"].Rows.Find(id)["Nombre"] = dlg.Nombre;
            dataSet1.Tables["Telefonos"].Rows.Find(id)["Numero"] = dlg.Numero;
            dataSet1.Tables["Telefonos"].Rows.Find(id)["Departamento"] = dlg.Departamento;
            dataSet1.Tables["Telefonos"].Rows.Find(id)["Comentarios"] = dlg.Comentarios;
            dataSet1.WriteXml("Telefonos.xml");
        }

        private void BEliminar_Click(object sender, EventArgs e)
        {
            if (GridTelefonos.SelectedRows.Count <= 0)
                return;
            if (MessageBox.Show("Eliminar", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            int id;
            id = int.Parse(GridTelefonos.SelectedRows[0].Cells[4].Value.ToString());
            dataSet1.Tables["Telefonos"].Rows.Remove(dataSet1.Tables["Telefonos"].Rows.Find(id));
            dataSet1.WriteXml("Telefonos.xml");
        }

        private void BAgregar_Click(object sender, EventArgs e)
        {
            FormTelefono dlg = new FormTelefono();
            if (dlg.ShowDialog() != DialogResult.OK)
                return;
            System.Data.DataRow dr = dataSet1.Tables["Telefonos"].NewRow();
            dr["Nombre"] = dlg.Nombre;
            dr["Numero"] = dlg.Numero;
            dr["Departamento"] = dlg.Departamento;
            dr["Comentarios"] = dlg.Comentarios;
            dataSet1.Tables["Telefonos"].Rows.Add(dr);
            dataSet1.WriteXml("Telefonos.xml");

        }

        private void GridTelefonos_DoubleClick(object sender, EventArgs e)
        {
            if (GridTelefonos.SelectedRows.Count <= 0)
                return;
            TTelefono.Text = GridTelefonos.SelectedRows[0].Cells[0].Value.ToString();
        }

        private void TBuscar_TextChanged(object sender, EventArgs e)
        {
            bindingSource1.Filter = "Nombre like '%" + TBuscar.Text + "%' or Numero like '%" + TBuscar.Text + "%' or Departamento like '%" + TBuscar.Text + "%' or Comentarios like '%" + TBuscar.Text + "%' or Correo  like '%" + TBuscar.Text + "%'";
            GridTelefonos.DataSource = bindingSource1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //mando el comando de mute
            usbapi.LedPIC(0x06);
        }

        private void BGuardar_Click(object sender, EventArgs e)
        {
             dataSet1.WriteXml("Telefonos.xml");
        }
    }

}
