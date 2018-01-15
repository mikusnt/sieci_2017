using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace server_test_KLIENT {
    public partial class Dodawanie : Form {

        public DialogResult result;
        public String text { get; private set; }
        public ushort port { get; private set; }
        public Dodawanie() {
            InitializeComponent();
        }
        
        private void Dodawanie_Load(object sender, EventArgs e) {
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            result = DialogResult.Cancel;
            text = null;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            result = DialogResult.OK;
            text = textName.Text;
            //MessageBox.Show(text);
            port = (ushort)numericPort.Value;
            this.Close();
        }
    }
}
