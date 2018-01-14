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
    public partial class OknoGlowne : Form {
        private OdseparowaneIP myIP;

        Form obj;
        bool pingWarning;
        //int selectedRow;
        delegate void setThreadedColorCallback();
        delegate void setThreadedInsertRowCallback(ServerInfo info);
        delegate void setThreadedClearDataRecordsCallback();
        delegate void setThreadedStatusLabelCallback(String info);
        delegate void setThreadedConnectionTimeCallback(int time);
        delegate void setThreadedCheckPingCallback();
        delegate void setThreadedTimerEnableCallback(bool enabled);
        delegate void setThreadedTimeCallback();

        // NormalConnection
        Socket socketFd;
        System.Threading.Timer timerThread;
        System.Diagnostics.Stopwatch stopWatch;
        int timeoutflag;
        private ToServerBuffer toServerBuffer;
        private String oPDomain;
        private ushort oPPort;
        private FromServerBuffer fromServerBuffer;

        // REPConnection
        Socket socketFdREP;
        System.Threading.Timer timerThreadREP;
        System.Diagnostics.Stopwatch stopWatchREP;
        int timeoutflagREP;
        private ToServerBuffer toServerBufferREP;
        private FromServerBuffer fromServerBufferREP;


        public OknoGlowne(OdseparowaneIP odseparowaneIP, String userName) {
            this.myIP = odseparowaneIP;
            toServerBuffer = new ToServerBuffer(OrderCode.eLogin, userName, 0, "", DataConst.defaultMaxPing, 80);
            toServerBufferREP = new ToServerBuffer(OrderCode.eServerData, userName, 0, "", DataConst.defaultMaxPing, 80);
            InitializeComponent();
            timerDelayInfo.Interval = DataConst.maxWaitingMSForFullAnswer;
            this.numericPing.Value = toServerBuffer.maxPing;
            //MessageBox.Show(odseparowaneIP.IPToString());
            this.obj = this;
            LoadSelectedRow();
            CheckPing();
            StartNormalConnection();
        }

        public OknoGlowne() : this(new OdseparowaneIP(), "mikusnt") { }

        /*
         Thread thread = New Thread(X);
         thread.Start();
         thread.Abort();
         
         static void X();
         // shared data between threads
         https://stackoverflow.com/questions/1360533/how-to-share-data-between-different-threads-in-c-sharp-using-aop 
             
             */

        /*

            Funkcje asynchronicznej obslugi komponentow

        */
        void setThreadedColor() {
            if (this.pictureColor.InvokeRequired) {
                setThreadedColorCallback colorCallback = new setThreadedColorCallback(setThreadedColor);
                this.obj.Invoke(colorCallback);
            } else {
                pictureColor.BackColor = Color.LimeGreen;
                timerColor.Start();
            }

        }

        void setThreadedInsertRow(ServerInfo info) {
            if (this.dataRecords.InvokeRequired) {
                setThreadedInsertRowCallback InsertRowCallback = new setThreadedInsertRowCallback(setThreadedInsertRow);
                this.obj.Invoke(InsertRowCallback, info);
            } else {
                try {
                    if (dataRecords.Rows.Count == 0)
                        dataRecords.Rows.Add();
                    int roznica = info.index - dataRecords.Rows.Count;
                    if (roznica >= 0) {
                        dataRecords.Rows.Insert(dataRecords.Rows.Count, roznica + 1);
                        /*for (int i = 0; i <= roznica; i++) {
                            DataGridViewRow first = (DataGridViewRow)dataRecords.Rows[0].Clone();
                            first.Cells[0].Value = "asd";
                            first.Cells[1].Value = "";
                            first.Cells[2].Value = "";
                            first.Cells[3].Value = "";
                            dataRecords.Rows.Insert(dataRecords.Rows.Count - 1, first);
                            
                        }*/
                            //dataRecords.Rows.
                    }

                    dataRecords.Rows[info.index].Cells[0].Value = (info.index + 1).ToString();
                    dataRecords.Rows[info.index].Cells[1].Value = info.name;
                    dataRecords.Rows[info.index].Cells[2].Value = info.port;
                    if ((info.ping >= 0) && (info.ping <= toServerBuffer.maxPing))
                        dataRecords.Rows[info.index].Cells[3].Value = info.ping.ToString();
                    else
                        dataRecords.Rows[info.index].Cells[3].Value = "not connected";
                    dataRecords.Update();
                    LoadSelectedRow();
                } catch (Exception exc) {
                    MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString());
                }
            }
        }

        void setThreadedClearDataRecords() {
            if (this.dataRecords.InvokeRequired) {
                setThreadedClearDataRecordsCallback ClearDataRecordsCallback = new setThreadedClearDataRecordsCallback(setThreadedClearDataRecords);
                this.obj.Invoke(ClearDataRecordsCallback);
            } else {
                dataRecords.Rows.Clear();
                LoadSelectedRow();
            }
        }

        void setThreadedStatusLabel(String info) {
            if (this.statusStripInfo.InvokeRequired) {
                setThreadedStatusLabelCallback StatusLabel = new setThreadedStatusLabelCallback(setThreadedStatusLabel);
                this.obj.Invoke(StatusLabel, info);
            } else {
                statusGlowne.Text = info;
            }
        }

        void setThreadedConnectionTime(int time) {
            if (this.labelConnection.InvokeRequired) {
                setThreadedConnectionTimeCallback ConnectionTime = new setThreadedConnectionTimeCallback(setThreadedConnectionTime);
                this.obj.Invoke(ConnectionTime, time);
            } else {
                if (time >= 0) {
                    labelConnection.Text = "Czas obsługi: " + time.ToString() + " ms";
                } else {
                    labelConnection.Text = "Czas obsługi: brak połączenia";
                }
            }
        }

        void setThreadedTime() {
            if (this.labelTime.InvokeRequired) {
                setThreadedTimeCallback Time = new setThreadedTimeCallback(setThreadedTime);
                this.obj.Invoke(Time);
            } else {
                labelTime.Text = "Dane z " + DateTime.Now.ToString("HH:mm:ss tt");
            }
        }

        /*
         
                Asynchroniczna obsluga polaczenia NormalConnection
             
        */
        private void ReceiveCallback(IAsyncResult ar) {
            try {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;

                
                Socket socketFd = state.m_SocketFd;

                // dla bufora jednobajtowego
                bool end = false;
                state.m_MemoryStream.Write(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE);
                if (state.m_DataBuf[0] == DataConst.endInOutBufferByte) {
                    state.endCounter++;
                    if (state.endCounter == DataConst.endInOutBufferSize)
                        end = true;
                } else state.endCounter = 0;



                /* read data */
                int size = socketFd.EndReceive(ar);

                if (!end) {
                    
                    /* get the rest of the data */
                    socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
                } else {
                    stopWatch.Stop();
                    /* all the data has arrived */
                    // koniec transmisji danych odbiorczych
                    byte[] bytes = state.m_MemoryStream.ToArray();
                    if (bytes.Length > 0) {
                        
                        setThreadedConnectionTime((int)stopWatch.ElapsedMilliseconds);
                        fromServerBuffer = new FromServerBuffer(bytes);
                        MessageBox.Show(Conversions.BytesToStringHex(fromServerBuffer.bytes));
                        switch (fromServerBuffer.orderCode) {
                            case OrderCode.eMaxPing: {
                                if (fromServerBuffer.orderResponse == 0) {
                                    setThreadedStatusLabel("Aktualizowano maksymalny ping do " + toServerBuffer.maxPing);
                                } else {
                                    setThreadedStatusLabel("Błąd podczas ustawienia maksymalnego pingu " + toServerBuffer.maxPing);
                                }
                            } break;
                            // //TODO obsluga startu watku odbierania danych
                            case OrderCode.eLogin: {
                                if (fromServerBuffer.orderResponse == 0) {
                                    setThreadedStatusLabel("Logowanie zakończone po " + stopWatch.ElapsedMilliseconds.ToString() + " ms");
                                    if (this.InvokeRequired) {
                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            this.timerDelayInfo.Enabled = true;
                                        });
                                    }
                                } else
                                    setThreadedStatusLabel("Błąd podczas logowania");
                            }
                            break;
                            // zakonczono testy
                            case OrderCode.eLogout: {
                                //CloseREP();
                                if (fromServerBuffer.orderResponse == 0) {
                                    if (this.InvokeRequired) {
                                        this.Invoke((MethodInvoker)delegate {
                                            //CloseREP();
                                            // rozwiazanie awaryjne
                                            this.Close();
                                        });
                                    }
                                }
                                else
                                    setThreadedStatusLabel("Błąd podczas wylogowania");
                            }
                            break;
                            case OrderCode.eAddServer: {
                                if (fromServerBuffer.orderResponse == 0) {
                                    setThreadedStatusLabel("Dodano serwer " + oPDomain + " o porcie " + oPPort);
                                } else {
                                    setThreadedStatusLabel("Błąd podczas dodania serwera " + oPDomain + " o porcie " + oPPort);
                                }
                            } break;

                            case OrderCode.eDeleteServer: {
                                if (fromServerBuffer.orderResponse == 0) {
                                    setThreadedStatusLabel("Usunięto serwer " + oPDomain + " o porcie " + oPPort);
                                    setThreadedClearDataRecords();
                                } else {
                                    setThreadedStatusLabel("Błąd podczas usuwania serwera " + oPDomain + " o porcie " + oPPort);
                                }
                            } break;
                            case OrderCode.eDeleteAllServers: {
                                if (fromServerBuffer.orderResponse == 0) {
                                    setThreadedStatusLabel("Usunięto wszystkie serwery");
                                    setThreadedClearDataRecords();
                                } else {
                                    setThreadedStatusLabel("Błąd podczas usuwania wszystkich serwerow");
                                }
                            }
                            break;
                            default: {
                                setThreadedStatusLabel("Niepoprawna odpowiedź serwera");
                            } break;

                        }


                        /* shutdown and close socket */
                        socketFd.Shutdown(SocketShutdown.Both);
                        socketFd.Close();
                    }
                }
            } catch (Exception exc) {
                MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Błąd odczytu danych z serwera");
            }
        }

        private void SendCallback(IAsyncResult ar) {
            try {
                /* retrieve the socket from the ar object */
                //Socket socketFd = (Socket)ar.AsyncState;

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
                Console.WriteLine(exc.Message.ToString());
            }

        }

        private void ConnectCallback(IAsyncResult ar) {
            if (Interlocked.CompareExchange(ref timeoutflag, 1, 0) != 0) {
                setThreadedStatusLabel("Brak połączenia do " + myIP.IPToString());
                setThreadedConnectionTime(-1);
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
                        byte[] dataBuf = toServerBuffer.ToBytes();
                        //MessageBox.Show("Wysylanie do serwera:\n" + Conversions.BytesToStringHex(dataBuf));

                        /* begin sending the date */
                        socketFd.BeginSend(dataBuf, 0, dataBuf.Length, 0, new AsyncCallback(SendCallback), socketFd);
                    } catch (Exception exc) {
                        Console.WriteLine(exc.Message.ToString());
                    }

                    /* begin receiving the data */
                    //socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(SendInfo), state);
                } catch (Exception exc) {
                    MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString());
                    setThreadedStatusLabel("Brak poprawnej odpowiedzi od serwera");
                    //setThreadedConnectButton(false);
                }
            }
        }

        // nieużywane, kod przeniesiony do buttonTryConnect_Click
        private void GetHostEntryCallback(IAsyncResult ar) {
            try {
                IPHostEntry hostEntry = null;
                IPAddress[] addresses = null;
                Socket socketFd = null;
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
                MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Brak połączenia do serwera");
                //setThreadedConnectButton(false);
            }
        }

        // inicjalizacja polaczenia testowego
        private void StartNormalConnection() {
            try {
                setThreadedColor();

                if (myIP.IPToString().Length >= 7) {
                    /* get DNS host information */
                    socketFd = null;
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
                        timerThread = new System.Threading.Timer(OnTimer, null, (int)numericPing.Value * 2, Timeout.Infinite);
                    }

                } else {
                    MessageBox.Show("Niepoprawna długość IP: " + myIP.ToString(), "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } catch (Exception exc) {
                MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString() + ", IP: " + myIP.ToString(), "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                setThreadedStatusLabel("Brak połączenia do serwera");
                //setThreadedConnectButton(false);
            }
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
         
                Asynchroniczna obsluga polaczenia REPConnection
             
        */

        private void ReceiveCallbackREP(IAsyncResult ar) {
            try {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                //Socket socketFd = state.m_SocketFd;

                /* read data */
                int size = socketFdREP.EndReceive(ar);

                // dla bufora jednobajtowego
                bool end = false;
                if (state.start) {
                    if (state.m_DataBuf[0] != 0) {
                        state.start = false;
                        setThreadedColor();
                        //MessageBox.Show(colorCount.ToString());
                    }
                }
                if (!state.start) {
                    
                    state.m_MemoryStream.Write(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE);
                    if (state.m_DataBuf[0] == DataConst.endInOutBufferByte) {
                        state.endCounter++;
                        if (state.endCounter == DataConst.endInOutBufferSize)
                            end = true;
                    } else state.endCounter = 0;
                }
                if (!end) {

                    /* get the rest of the data */
                    socketFdREP.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallbackREP), state);
                } else {
                    stopWatchREP.Stop();
                    byte[] bytes = state.m_MemoryStream.ToArray();
                    /* all the data has arrived */
                    // koniec transmisji danych odbiorczych
                    if (bytes.Length > 0) {
                        
                        //MessageBox.Show(Conversions.BytesToStringHex( Conversions.CharArrayToBytes(state.m_StringBuilder.ToString().ToCharArray())));
                        fromServerBufferREP = new FromServerBuffer(bytes);

                        //MessageBox.Show(Conversions.BytesToStringHex(fromServerBufferREP.bytes));
                        //MessageBox.Show(fromServerBufferREP.serverInfo.ToString());
                        if (fromServerBufferREP.orderCode == OrderCode.eServerData) {
                            setThreadedTime();
                            setThreadedInsertRow(fromServerBufferREP.serverInfo);
                        } else {
                            setThreadedStatusLabel("Niepoprawna odpowiedź serwera");
                        }

                        // nalezy dodac zapetlenie
                        state.m_MemoryStream = new MemoryStream();
                        state.endCounter = 0;
                        state.start = true;
                        socketFdREP.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallbackREP), state);

                        
                    }
                }
            } catch (Exception exc) {
                MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Błąd odczytu danych połaczeń z serwera");
            }
        }


        private void SendCallbackREP(IAsyncResult ar) {
            try {
                /* retrieve the socket from the ar object */
                //Socket socketFd = (Socket)ar.AsyncState;

                /* end pending asynchronous send */
                int bytesSent = socketFdREP.EndSend(ar);
                //MessageBox.Show("Wysłano " + bytesSent.ToString() + " bajtów");

                /* create the SocketStateObject */
                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = socketFdREP;
                //Console.WriteLine("\t\tSent {0} bytes to the client\n\tEND connection", bytesSent);
                socketFdREP.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallbackREP), state);

                /* shutdown and close socket */
                //socketFd.Shutdown(SocketShutdown.Both);
                //socketFd.Close();
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            }

        }

        private void ConnectCallbackREP(IAsyncResult ar) {
            if (Interlocked.CompareExchange(ref timeoutflagREP, 1, 0) != 0) {
                setThreadedStatusLabel("Brak połączenia do serwera");
                return;
            } else {

                // we set the flag to 1, indicating it was completed.
                try {
                    //setThreadedStatusLabel("Uzyskano połączenie do " + myIP.IPToString() + ", wymiana danych...");
                    if (timerThreadREP != null) {
                        // stop the timer from firing.
                        //ping = timerThread.get
                        timerThreadREP.Dispose();
                    }
                    /* retrieve the socket from the state object */
                    //Socket socketFd = (Socket)ar.AsyncState;

                    /* complete the connection */
                    socketFdREP.EndConnect(ar);



                    //setThreadedStatusLabel("Czekaj, przesyłanie danych...");

                    try {
                        byte[] dataBuf = toServerBufferREP.ToBytes();
                        //MessageBox.Show("Wysylanie do serwera:\n" + Conversions.BytesToStringHex(dataBuf));

                        /* begin sending the date */
                        socketFdREP.BeginSend(dataBuf, 0, dataBuf.Length, 0, new AsyncCallback(SendCallbackREP), socketFdREP);
                    } catch (Exception exc) {
                        Console.WriteLine(exc.Message.ToString());
                    }

                    /* begin receiving the data */
                    //socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(SendInfo), state);
                } catch (Exception exc) {
                    MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString());
                    setThreadedStatusLabel("Brak poprawnej odpowiedzi od serwera");
                    //setThreadedConnectButton(false);
                }
            }
        }

        // nieużywane, kod przeniesiony do buttonTryConnect_Click
        private void GetHostEntryCallbackREP(IAsyncResult ar) {
            try {
                IPHostEntry hostEntry = null;
                IPAddress[] addresses = null;
                IPEndPoint endPoint = null;
                /* complete the DNS query */
                hostEntry = Dns.EndGetHostEntry(ar);
                addresses = hostEntry.AddressList;

                /* create a socket */
                socketFdREP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* remote endpoint for the socket, nr portu 1234 */
                endPoint = new IPEndPoint(addresses[0], DataConst.serverPort);

                /* connect to the server */
                socketFdREP.BeginConnect(endPoint, new AsyncCallback(ConnectCallbackREP), socketFdREP);
            } catch (Exception exc) {
                MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString());
                setThreadedStatusLabel("Brak połączenia do serwera");
                //setThreadedConnectButton(false);
            }
        }

        // inicjalizacja polaczenia testowego
        private void StartREPConnection() {
            try {
                setThreadedColor();

                if (myIP.IPToString().Length >= 7) {
                    /* get DNS host information */
                    IPEndPoint endPoint = null;
                    IPAddress iPAddress = null;

                    socketFdREP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPAddress.TryParse(myIP.IPToString(), out iPAddress);
                    endPoint = new IPEndPoint(iPAddress, DataConst.serverPort);

                    //setThreadedStatusLabel("Próba połączenia do " + myIP.IPToString());

                    /* connect to the server */
                    IAsyncResult result = socketFdREP.BeginConnect(endPoint, new AsyncCallback(ConnectCallbackREP), socketFdREP);
                    stopWatchREP = new System.Diagnostics.Stopwatch();
                    stopWatchREP.Start();
                    if (!result.IsCompleted) {
                        timeoutflagREP = 0;
                        timerThreadREP = new System.Threading.Timer(OnTimerREP, null, DataConst.maxWaitingMSForFullAnswer, Timeout.Infinite);
                    }

                } else {
                    MessageBox.Show("Niepoprawna długość IP: " + myIP.ToString(), "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } catch (Exception exc) {
                MessageBox.Show("Wyjątek:\t\n" + exc.Message.ToString() + ", IP: " + myIP.ToString(), "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                setThreadedStatusLabel("Brak połączenia do serwera");
                //setThreadedConnectButton(false);
            }
        }


        void OnTimerREP(object obj) {
            if (Interlocked.CompareExchange(ref timeoutflagREP, 2, 0) != 0) {
                // the flag was set elsewhere, so return immediately.
                return;
            }

            timerThreadREP.Dispose();
            socketFdREP.Close(); // closing the Socket cancels the async operation.
        }



        public class SocketStateObject {
            public const int BUF_SIZE = 1;
            public byte[] m_DataBuf = new byte[BUF_SIZE];
            public MemoryStream m_MemoryStream = new MemoryStream();
            public Socket m_SocketFd = null;
            public byte endCounter = 0;
            public bool start = true;
        }


        /*
         
                Funkcje wewnetrzne     
             
        */

        private void CloseREP() {
            //timerThreadREP.Dispose();
            /* shutdown and close socket */
            //socketFdREP.Shutdown(SocketShutdown.Both);
            //socketFdREP.Close();
            //socketFdREP = null;
        }

        private void LoadSelectedRow() {
            if ((dataRecords.SelectedRows.Count > 0) && (dataRecords.SelectedRows[0].Cells[1].Value != null)) {
                buttonDelete.Enabled = true;
                toServerBuffer.serverIndex = (sbyte)dataRecords.SelectedRows[0].Index;
                toServerBuffer.serverDomain = (String)dataRecords.SelectedRows[0].Cells[1].Value;

            } else {
                buttonDelete.Enabled = false;
                toServerBuffer.serverDomain = "";
            }
        }

        private void CheckPing() {
            if (numericPing.Value != toServerBuffer.maxPing) {
                pingWarning = true;
                buttonPing.Enabled = true;
            } else {
                pingWarning = false;
                buttonPing.Enabled = false;
            }        
        }

        /*
         
                Zdarzenia     
             
        */
        private void buttonAdd_Click(object sender, EventArgs e) {
            if (dataRecords.Rows.Count < (DataConst.maxServerDomainIndex - 1)) {
                Dodawanie dodawanie = new Dodawanie();
                dodawanie.ShowDialog();
                if (dodawanie.result == DialogResult.OK) {
                    toServerBuffer.orderCode = OrderCode.eAddServer;

                    MessageBox.Show(dodawanie.text);
                    toServerBuffer.serverDomain = dodawanie.text;
                    oPDomain = dodawanie.text;
                    toServerBuffer.port = dodawanie.port;
                    oPPort = dodawanie.port;
                    StartNormalConnection();
                }
            } else MessageBox.Show("Osiagnięto maksymalną liczbę klientów");
        }

        private void buttonDelete_Click(object sender, EventArgs e) {
            LoadSelectedRow();
            toServerBuffer.orderCode = OrderCode.eDeleteServer;
            oPDomain = dataRecords.SelectedRows[0].Cells[1].Value.ToString();
            oPPort = (ushort)dataRecords.SelectedRows[0].Cells[2].Value;
            MessageBox.Show(toServerBuffer.ToString());
            // nr usuwanego indeksu i nazwa domeny zaladowane wczesniej przez akcje klikniecia
            //dataRecords.Rows.Clear();
            StartNormalConnection();
        }

        private void buttonLogout_Click(object sender, EventArgs e) {
            // zamykanie okna glownego po pozytywnym odebraniu danych
            toServerBuffer.orderCode = OrderCode.eLogout;
            StartNormalConnection();
        }

        private void dataRecords_CellClick(object sender, DataGridViewCellEventArgs e) {
            //MessageBox.Show(dataRecords.SelectedRows[0].Index.ToString());
            LoadSelectedRow();
        }

        private void buttonDeleteAll_Click(object sender, EventArgs e) {
            toServerBuffer.orderCode = OrderCode.eDeleteAllServers;
            StartNormalConnection();
        }

        private void numericPing_ValueChanged(object sender, EventArgs e) {
            CheckPing();
        }

        private void numericPing_KeyDown(object sender, KeyEventArgs e) {
            CheckPing();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (pingWarning) {
                if (buttonPing.ForeColor == SystemColors.ControlText)
                    buttonPing.ForeColor = Color.Red;
                else
                    buttonPing.ForeColor = SystemColors.ControlText;
            } else
                buttonPing.ForeColor = SystemColors.ControlText;
        }

        private void buttonPing_Click(object sender, EventArgs e) {
            toServerBuffer.maxPing = (short)numericPing.Value;
            toServerBuffer.orderCode = OrderCode.eMaxPing;
            CheckPing();
            StartNormalConnection();
        }

        private void OknoGlowne_FormClosed(object sender, FormClosedEventArgs e) {
            toServerBuffer.orderCode = OrderCode.eLogout;
            StartNormalConnection();
        }

        private void timer2_Tick(object sender, EventArgs e) {
            timerDelayInfo.Stop();
            //MessageBox.Show("stop");
            toServerBufferREP.orderCode = OrderCode.eServerData;
            StartREPConnection();
        }

        private void timer3_Tick(object sender, EventArgs e) {
            pictureColor.BackColor = SystemColors.Control;
            timerColor.Stop();
        }
    }
}
