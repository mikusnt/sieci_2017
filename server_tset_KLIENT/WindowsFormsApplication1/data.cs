using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;

namespace server_test_KLIENT {
    public static class DataConst {
        public const byte serverDomainBufferSize = 64;
        public const byte userNameBufferSize = 16;
        public const byte usernamePathBufferSize = 32;
        public const byte lineFileBufferSize = 64;
        public const byte serverInfoBufferSize = 70;
        //public const short maxPing = 2000;
        public const byte maxServerDomainIndex = 127;
        public const byte inOutBufferSize = 90;
        public const short maxWaitingMSForFullAnswer = 1000;
        public const short defaultMaxPing = 200;
        public const ushort serverPort = 1234;
        // edycja dwoch ponizszych wymaga recznej zmiany typu i zawartosci zmiennej endInOutBuffer
        public const byte endInOutBufferByte = 0xFF;
        public const byte endInOutBufferSize = 3;
        public const uint endInOutBuffer = 0xFFFFFFF0;
    }

    public enum OrderCode {
        eMaxPing = 0x98,
        eConnection = 0x99,
        eLogin = 0xA0,
        eLogout = 0xA1,
        eServerData = 0xC0,
        eDeleteServer = 0xB0,
        eDeleteAllServers = 0xB1,
        eAddServer = 0xB2
    };

    public class Conversions {
        public static String BytesToStringHex(byte[] bytes) {
            String wyr = "";
            for (int i = 0; i < bytes.Length; i++) {
                wyr += "0x" + bytes[i].ToString("X2") + " ";
            }
            return wyr;
        }
        public static byte[] CharArrayToBytes(Char[] chars) {
            byte[] bytes = new byte[chars.Length];
            for (int i = 0; i < chars.Length; i++) {
                bytes[i] = Convert.ToByte(chars[i]);
            }
            return bytes;
        }

        public static char[] BytesToCharArray(byte[] bytes) {
            char[] array = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; i++) {
                array[i] = Convert.ToChar(bytes[i]);
            }
            return array;
        }
    }
    public class ServerInfo {
        public byte index;
        public ushort port;
        private char[] nameTable;
        private String _name;
        public String name {
            get { return _name; }
            set { _name = value; Array.Copy(_name.ToArray(), this.nameTable, _name.Length); }
        }
        public short ping;
        public byte[] bytes;

        public override string ToString() {
            return String.Format("Index: {0}\nPort: {1}\nName: {2}\nPing: {3}\n", index, port, name, ping);
        }
        public ServerInfo(byte index, ushort port, String name, short ping) {
            this.index = index;
            this.port = port;
            this.nameTable = new char[DataConst.serverDomainBufferSize];
            this.name = name;
            
            this.ping = ping;
        }
        public ServerInfo(byte[] bytes) {
            FromBytes(bytes);
        }
        public ServerInfo() : this(0, 0, "", 0) { }
        private byte[] ToBytes() {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            writer.Write(this.index);
            writer.Write(0);
            writer.Write(this.port);
            writer.Write(Conversions.CharArrayToBytes(nameTable));
            writer.Write(this.ping);
            //byte[] bytes = stream.ToArray();
            return stream.ToArray();
        }

        public void FromBytes(byte[] bytes) {
            //if (bytes.Length >= DataConst.serverInfoBufferSize) {
                this.bytes = new byte[DataConst.inOutBufferSize];
                bytes.CopyTo(this.bytes, 0);
                var reader = new BinaryReader(new MemoryStream(bytes));
                this.index = reader.ReadByte();
                reader.ReadByte();
                this.port = reader.ReadUInt16();
            //this.nameTable = Conversions.BytesToCharArray(reader.ReadBytes(DataConst.serverDomainBufferSize));
            String temp = new String(Conversions.BytesToCharArray(reader.ReadBytes(DataConst.serverDomainBufferSize)));
            temp = temp.Replace("\0", String.Empty);
            
            this.nameTable = new char[DataConst.serverDomainBufferSize];
            this.name = temp;
            this.ping = reader.ReadInt16();
            //}
        }
    }

    public class ToServerBuffer {
        public OrderCode orderCode;
        private char[] userNameTable;
        private String _userName;
        public String userName {
            get { return _userName; }
            set { _userName = value; Array.Copy(userName.ToArray(), this.userNameTable, userName.Length); }             
        }
        public sbyte serverIndex;
        private char[] serverDomainTable;
        private String _serverDomain;
        public String serverDomain {
            get { return _serverDomain; }
            set { _serverDomain = value; Array.Copy(serverDomain.ToArray(), this.serverDomainTable, serverDomain.Length); }
        }
        public short maxPing;
        public ushort port;

        public ToServerBuffer() : this(OrderCode.eConnection, "", 0, "", 200, 80) { }

        public ToServerBuffer(OrderCode orderCode, String userName, sbyte serverIndex, String serverDomain, short maxPing, ushort port) {
            this.orderCode = orderCode;
            this.userNameTable = new char[DataConst.userNameBufferSize];
            this.userName = userName;
            this.serverIndex = serverIndex;
            this.serverDomainTable = new char[DataConst.serverDomainBufferSize];
            this.serverDomain = serverDomain;
            this.maxPing = maxPing;
            this.port = port; 
        }

        public byte[] ToBytes() {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write((Byte)orderCode);
            if (orderCode != OrderCode.eConnection) {
                writer.Write(Conversions.CharArrayToBytes(userNameTable));
            }
            if (orderCode == OrderCode.eMaxPing) {
                writer.Write(maxPing);
            } else if (orderCode == OrderCode.eDeleteServer) {
                 writer.Write(serverIndex);
            } else if (orderCode == OrderCode.eAddServer) {
                writer.Write(port);
                writer.Write(Conversions.CharArrayToBytes(serverDomainTable));
            }
            writer.Write(DataConst.endInOutBuffer);
            return stream.ToArray();
        }
        public override string ToString() {
            return "orderCode: " + orderCode.ToString() + "\nuserName: " + userName + "\nserverIndex: " + serverIndex 
                + "\nserverDomain: " + serverDomain + "\nmaxPing: " + maxPing + "\nport: " + port;
        }
    }

    public class FromServerBuffer {
        public OrderCode orderCode { get; private set; }
        public ushort orderResponse { get; private set; }
        public ServerInfo serverInfo { get; private set; }
        public byte[] bytes { get; private set; }

        //public FromServerBuffer() : this(new byte[DataConst.inOutBufferSize]) { }
        public FromServerBuffer(byte[] bytes) {
            FromBytes(bytes);    
        }
        
        public void FromBytes(byte[] bytes) {
            //if (bytes.Length >= DataConst.inOutBufferSize) {
                this.bytes = new byte[DataConst.inOutBufferSize];
                bytes.CopyTo(this.bytes, 0);
                var reader = new BinaryReader(new MemoryStream(bytes));
                this.orderCode = (OrderCode)reader.ReadByte();
                if ((orderCode != OrderCode.eConnection) && (orderCode != OrderCode.eServerData)) {
                    this.orderResponse = reader.ReadByte();
                } else if (orderCode == OrderCode.eServerData) {
                    this.serverInfo = new ServerInfo(reader.ReadBytes(DataConst.serverInfoBufferSize));
                }
           // }
        }

        public override string ToString() {
            return String.Format("orderCode: {0}\norderResponse: {1}\nserverInfo{2}", 
                this.orderCode, this.orderResponse, this.serverInfo.ToString());
        }
    }
}
