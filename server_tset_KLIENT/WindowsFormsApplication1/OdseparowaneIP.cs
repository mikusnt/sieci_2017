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
    public class OdseparowaneIP {
        public byte ip3, ip2, ip1, ip0;
        public byte m3, m2, m1, m0;
        private byte mask;
        public Int32 numberOfComputers { get; private set; }

        public OdseparowaneIP() {
            mask = 32;
            GenerateNumberOfComputers();
        }
        public override string ToString() {
            return "IP: " + IPToString() + "\nMask: " + MaskToString() + "\n Number of computers: " + numberOfComputers.ToString();
        }

        public void ResetToNetworkAddress() {
            ip3 = (byte)(ip3 & m3);
            ip2 = (byte)(ip2 & m2);
            ip1 = (byte)(ip1 & m1);
            ip0 = (byte)(ip0 & m0);
        }

        
        public String MaskToString() {
            return ValuesToString(m3, m2, m1, m0);
        }
        public String IPToString() {
            return ValuesToString(ip3, ip2, ip1, ip0);
        }
        private String ValuesToString(byte v3, byte v2, byte v1, byte v0) {
            return v3.ToString() + "." + v2.ToString() + "." + v1.ToString() + "." + v0.ToString();
        }
        private void SetIp(byte ip3, byte ip2, byte ip1, byte ip0) {
            this.ip3 = ip3;
            this.ip2 = ip2;
            this.ip1 = ip1;
            this.ip0 = ip0;
        }
        private void SetMask(Byte mask) {
            this.m3 = 0;
            this.m2 = 0;
            this.m1 = 0;
            this.m0 = 0;
            this.mask = mask;
            for (int i = 0; i < 8; i++) {
                if (i < mask)
                    this.m3 += (byte)(1 << Math.Max(7 - i, 0));
            }
            for (int i = 8; i < 16; i++) {
                if (i < mask)
                    this.m2 += (byte)(1 << Math.Max(15 - i, 0));
            }
            for (int i = 16; i < 24; i++) {
                if (i < mask)
                    this.m1 += (byte)(1 << Math.Max(23 - i, 0));
            }
            for (int i = 24; i < 32; i++) {
                if (i < mask)
                    this.m0 += (byte)(1 << Math.Max(31 - i, 0));
            }
            //MessageBox.Show(this.ToString());
        }
        public void SetIPMask(byte ip3, byte ip2, byte ip1, byte ip0, Byte mask) {
            this.ip3 = ip3;
            this.ip2 = ip2;
            this.ip1 = ip1;
            this.ip0 = ip0;
            SetMask(mask);
            this.GenerateNumberOfComputers();
        }
        private void GenerateNumberOfComputers() {
            numberOfComputers = (Int32)Math.Max(Math.Pow(2, 32 - mask) - 2, 1);
            //MessageBox.Show(this.ToString());
        }
    }
}
