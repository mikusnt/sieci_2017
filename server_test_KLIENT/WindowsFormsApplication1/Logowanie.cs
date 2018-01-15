using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace server_test_KLIENT {
    public partial class Logowanie : Form {
        private ServerInfo serverInfo = new ServerInfo();
        private Form obj;
        private const String defaultStatusLogin = "Dokonaj testowego połączenia aby się zalogować";
        delegate void setThreadedConnectButtonCallback(bool enabled);
        delegate void setThreadedStatusLabelCallback(String text);
        private ToServerBuffer toServerbuffer = new ToServerBuffer();
        private FromServerBuffer fromServerBuffer;
        private OdseparowaneIP myIP = new OdseparowaneIP();

        Socket socketFd;
        System.Threading.Timer timerThread;
        System.Diagnostics.Stopwatch stopWatch;
        int timeoutflag;
        //private maskaNumeryczna maskaNum;
        public Logowanie() {
            this.obj = this;
            InitializeComponent();
            LoadDefault();
        }

        private void NetworkEnable() {
            if (radioNetwork.Checked) {
                numericMask.Enabled = true;
            } else {
                numericMask.Enabled =false;
            }
        }

        /*
         
                Funkcje asynchronicznej obslugi komponentow
                 
        */
        private void setThreadedConnectButton(bool enabled) {
            if (this.buttonLogin.InvokeRequired) {
                setThreadedConnectButtonCallback buttonCallback = new setThreadedConnectButtonCallback(setThreadedConnectButton);
                this.obj.Invoke(buttonCallback, enabled);
            } else {
                this.buttonLogin.Enabled = enabled;
            }
        }

        private void setThreadedStatusLabel(String text) {
            if (this.statusStripLogin.InvokeRequired) {
                setThreadedStatusLabelCallback statusLabelCallback = new setThreadedStatusLabelCallback(setThreadedStatusLabel);
                this.obj.Invoke(statusLabelCallback, text);
            } else {
                this.statusLogin.Text = text;
            }
        }

        /*
         
                Asynchroniczna obsluga polaczenia     
             
        */
        private void ReceiveCallback(IAsyncResult ar) {
            try {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                //Socket socketFd = state.m_SocketFd;

                /* read data */
                int size = socketFd.EndReceive(ar);

                // dla bufora jednobajtowego
                bool end = false;

                state.m_MemoryStream.Write(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE);
                if (state.m_DataBuf[0] == DataConst.endInOutBufferByte) {
                    state.endCounter++;
                    if (state.endCounter == DataConst.endInOutBufferSize)
                        end = true;
                } else state.endCounter = 0;

                if (!end) {
                    //state.m_MemoeyStream.Append(Encoding.u .GetString(state.m_DataBuf, 0, size));

                    /* get the rest of the data */
                    socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
                } else {
                    stopWatch.Stop();
                    
                    byte[] bytes = state.m_MemoryStream.ToArray();
                    if (DataConst.DEBUG)
                        MessageBox.Show(Conversions.BytesToStringHex(bytes));
                    /* all the data has arrived */
                    // koniec transmisji danych odbiorczych
                    if (bytes.Length > 0) {
                        fromServerBuffer = new FromServerBuffer(bytes);
                        if (fromServerBuffer.orderCode == OrderCode.eConnection) {
                            setThreadedConnectButton(true);
                            setThreadedStatusLabel("Test zakończony pomyślnie po " + stopWatch.ElapsedMilliseconds.ToString() + " ms");
                        } else {
                            setThreadedConnectButton(true);
                            setThreadedStatusLabel("Niepoprawna odpowiedź serwera");
                        }
                        //MessageBox.Show(Conversions.BytesToStringHex(fromServerBuffer.bytes));
                        /* shutdown and close socket */


                        socketFd.Shutdown(SocketShutdown.Both);
                        socketFd.Close();
                    }
                }
            } catch (Exception exc) {
                if (DataConst.DEBUG)
                    MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Błąd odczytu danych z serwera");
                setThreadedConnectButton(false);
            }
        }

        private void SendCallback(IAsyncResult ar) {
            try {
                /* retrieve the socket from the ar object */
               // Socket socketFd = (Socket)ar.AsyncState;

                /* end pending asynchronous send */
                int bytesSent = socketFd.EndSend(ar);
                //MessageBox.Show("Wysłano " + bytesSent.ToString() + " bajtów");

                /* create the SocketStateObject */
                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = socketFd;

                //Console.WriteLine("\t\tSent {0} bytes to the client\n\tEND connection", bytesSent);
                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);

                /* shutdown and close socket */
                //socketFd.Shutdown(SocketShutdown.Both);
                //socketFd.Close();
            } catch (Exception exc) {
                //Console.WriteLine(exc.Message.ToString());
            }

        }

        private void ConnectCallback(IAsyncResult ar) {
            if (Interlocked.CompareExchange(ref timeoutflag, 1, 0) != 0) {
                setThreadedStatusLabel("Brak połączenia do " + myIP.IPToString());
                return;
            } else {

                // we set the flag to 1, indicating it was completed.
                try {
                    setThreadedStatusLabel("Uzyskano połączenie do " + myIP.IPToString() + ", wymiana danych...");
                    if (timerThread != null) {
                        // stop the timer from firing.
                        //ping = timerThread.get
                        timerThread.Dispose();
                    }
                    /* retrieve the socket from the state object */
                    //Socket socketFd = (Socket)ar.AsyncState;

                    /* complete the connection */
                    socketFd.EndConnect(ar);



                    //setThreadedStatusLabel("Czekaj, przesyłanie danych...");

                    try {
                        byte[] dataBuf = toServerbuffer.ToBytes();
                        //MessageBox.Show("Wysylanie do serwera:\n" + Conversions.BytesToStringHex(dataBuf));

                        /* begin sending the date */
                        socketFd.BeginSend(dataBuf, 0, dataBuf.Length, 0, new AsyncCallback(SendCallback), socketFd);
                    } catch (Exception exc) {
                        //Console.WriteLine(exc.Message.ToString());
                    }

                    /* begin receiving the data */
                    //socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(SendInfo), state);
                } catch (Exception exc) {
                    if (DataConst.DEBUG)
                        MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString());
                    setThreadedStatusLabel("Brak poprawnej odpowiedzi od serwera");
                    setThreadedConnectButton(false);
                }
            }
        }

        // nieużywane, kod przeniesiony do buttonTryConnect_Click
        private void GetHostEntryCallback(IAsyncResult ar) {
            try {
                IPHostEntry hostEntry = null;
                IPAddress[] addresses = null;
                //Socket socketFd = null;
                IPEndPoint endPoint = null;
                /* complete the DNS query */
                hostEntry = Dns.EndGetHostEntry(ar);
                addresses = hostEntry.AddressList;
                
                /* create a socket */
                socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* remote endpoint for the socket, nr portu 1234 */
                endPoint = new IPEndPoint(addresses[0], DataConst.serverPort);

                setThreadedStatusLabel("Wait! Connecting...");

                /* connect to the server */
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), socketFd);
            } catch (Exception exc) {
                if (DataConst.DEBUG)
                    MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Brak połączenia do serwera");
                setThreadedConnectButton(false);
            }
        }

        // inicjalizacja polaczenia testowego
        private void buttonTryConnect_Click(object sender, EventArgs e) {
            try {
                setThreadedConnectButton(false);

                if (myIP.IPToString().Length >= 7) {
                    /* get DNS host information */
                    //socketFd = null;
                    IPEndPoint endPoint = null;
                    IPAddress iPAddress = null;

                    socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPAddress.TryParse(myIP.IPToString(), out iPAddress);
                    endPoint = new IPEndPoint(iPAddress, DataConst.serverPort);

                    setThreadedStatusLabel("Próba połączenia do " + myIP.IPToString());

                    /* connect to the server */
                    IAsyncResult result = socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), socketFd);
                    stopWatch = new System.Diagnostics.Stopwatch();
                    stopWatch.Start();
                    if (!result.IsCompleted) {
                        timeoutflag = 0;
                        timerThread = new System.Threading.Timer(OnTimer, null, DataConst.maxWaitingMSForFullAnswer, Timeout.Infinite);
                    }

                } else {
                    MessageBox.Show("Niepoprawna długość IP: " + myIP.ToString(), "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } catch (Exception exc)
            {
                if (DataConst.DEBUG)
                    MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString() + ", IP: " + myIP.ToString(), "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                setThreadedStatusLabel("Brak połączenia do serwera");
                setThreadedConnectButton(false);
            }
        }

        public class SocketStateObject {
            public const int BUF_SIZE = 1;
            public byte[] m_DataBuf = new byte[BUF_SIZE];
            public MemoryStream m_MemoryStream = new MemoryStream();
            public Socket m_SocketFd = null;
            public byte endCounter = 0;
        }

        void OnTimer(object obj) {
            if (Interlocked.CompareExchange(ref timeoutflag, 2, 0) != 0) {
                // the flag was set elsewhere, so return immediately.
                return;
            }

            timerThread.Dispose();
            socketFd.Close(); // closing the Socket cancels the async operation.
        }

        /*
         
                Funkcje wewnetrzne     
             
        */
        private void tryEnableTest()
        {
            if (textBoxLogin.Text.Length > 0)
                buttonTryConnect.Enabled = true;
            else
                buttonTryConnect.Enabled = false;
        }

        private String getIP() {
            return numericIP3.Value.ToString() + "." + numericIP2.Value.ToString() + "." + numericIP1.Value.ToString() + "." + numericIP0.Value.ToString();
        }

        private void RefreshIPData()
        {
            myIP.SetIPMask((Byte)numericIP3.Value, (Byte)numericIP2.Value, (Byte)numericIP1.Value, (Byte)numericIP0.Value, 32);
            buttonLogin.Enabled = false;
        }


        private void LoadDefault() {
            this.statusLogin.Text = defaultStatusLogin;
            statusLogin.Text = defaultStatusLogin;
            tryEnableTest();
            buttonLogin.Enabled = false;
            NetworkEnable();
            RefreshIPData();
        }

        /*
         
                Zdarzenia     
             
        */
        private void radioAddr_CheckedChanged(object sender, EventArgs e) {
            NetworkEnable();
        }

        private void radioNetwork_CheckedChanged(object sender, EventArgs e) {
            NetworkEnable();
        }

        private void znaneIP_Click(object sender, EventArgs e)
        {

        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            // if (MessageBox.Show("Czy na pewno chcesz wyjść?", "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            Application.Exit();
        }

        private void Logowanie_Load(object sender, EventArgs e)
        {

        }

        private void textBoxLogin_TextChanged(object sender, EventArgs e)
        {
            tryEnableTest();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            OknoGlowne oknoGlowne = new OknoGlowne(myIP, textBoxLogin.Text);
            this.Hide();
            oknoGlowne.ShowDialog();
            LoadDefault();
            this.Show();
        }

        private void numericIP3_ValueChanged(object sender, EventArgs e)
        {
            RefreshIPData();
            buttonLogin.Enabled = false;
        }

        private void numericPing_ValueChanged(object sender, EventArgs e)
        {
            buttonLogin.Enabled = false;
        }

        private void numericIP3_KeyDown(object sender, KeyEventArgs e)
        {
            RefreshIPData();
            buttonLogin.Enabled = false;
        }

        private void numericPing_KeyDown(object sender, KeyEventArgs e)
        {
            buttonLogin.Enabled = false;
        }
    }
}
