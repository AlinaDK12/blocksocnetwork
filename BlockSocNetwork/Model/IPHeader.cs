using System;
using System.Net;
using System.IO;
using System.Windows;

namespace BlockSocNetwork
{
    public enum Protocol
    {
        TCP = 6,
        UDP = 17,
        Unknown = -1
    }

    public class IPHeader
    {
        private byte _versionAndHeaderLength;   
        private byte _differentiatedServices;   
        private ushort _totalLength;              
        private ushort _identification;           
        private ushort _flagsAndOffset;           
        private byte _TTL;                      
        private byte _protocol;                 
        private short _checksum;                  
                                                 
        private uint _sourceIPAddress;          
        private uint _destinationIPAddress;    
                                                

        private byte _headerLength;            
        private byte[] _IPData = new byte[4096];  


        public IPHeader(byte[] byBuffer, int nReceived)
        {

            try
            {
                //Создание MemoryStream из полученных байтов
                MemoryStream memoryStream = new MemoryStream(byBuffer, 0, nReceived);
                //Создание BinaryReader из MemoryStream
                BinaryReader binaryReader = new BinaryReader(memoryStream);

                //Первые 8 бит - версия и длина заголовка
                _versionAndHeaderLength = binaryReader.ReadByte();

                //8 бит - дифференцированные сервисы
                _differentiatedServices = binaryReader.ReadByte();

                //8 бит - общая длина
                _totalLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                //16 бит - идентификация
                _identification = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                //16 бит - флаг и смещение фрагмента
                _flagsAndOffset = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                //8 бит - время существования
                _TTL = binaryReader.ReadByte();

                //8 бит - протокол
                _protocol = binaryReader.ReadByte();

                //16 бит - контрольная сумма заголовка
                _checksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                //32 бита - IP адрес источника
                _sourceIPAddress = (uint)(binaryReader.ReadInt32());

                //32 бита - IP адрес назначения
                _destinationIPAddress = (uint)(binaryReader.ReadInt32());

                //длина заголовка
                _headerLength = _versionAndHeaderLength;

                //извлечение длины заголовка
                _headerLength <<= 4;
                _headerLength >>= 4;
                //точная длина заголовка
                _headerLength *= 4;

                Array.Copy(byBuffer,
                           _headerLength, 
                           _IPData, 0,
                           _totalLength - _headerLength);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка!");
            }
        }

        public string Version
        {
            get
            {
                if ((_versionAndHeaderLength >> 4) == 4)
                {
                    return "IP v4";
                }
                else if ((_versionAndHeaderLength >> 4) == 6)
                {
                    return "IP v6";
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public string HeaderLength
        {
            get
            {
                return _headerLength.ToString();
            }
        }

        public ushort MessageLength
        {
            get
            {
                return (ushort)(_totalLength - _headerLength);
            }
        }

        public string DifferentiatedServices
        {
            get
            {
                //в 16-ричном формате
                return string.Format("0x{0:x2} ({1})", _differentiatedServices,
                    _differentiatedServices);
            }
        }

        public string Flags
        {
            get
            {
                int nFlags = _flagsAndOffset >> 13;
                if (nFlags == 2)
                {
                    return "Don't fragment";
                }
                else if (nFlags == 1)
                {
                    return "More fragments to come";
                }
                else
                {
                    return nFlags.ToString();
                }
            }
        }

        public string FragmentationOffset
        {
            get
            {
                int nOffset = _flagsAndOffset << 3;
                nOffset >>= 3;

                return nOffset.ToString();
            }
        }

        public string TTL
        {
            get
            {
                return _TTL.ToString();
            }
        }

        public Protocol ProtocolType
        {
            get
            {
                if (_protocol == 6)        
                {
                    return Protocol.TCP;
                }
                else if (_protocol == 17) 
                {
                    return Protocol.UDP;
                }
                else
                {
                    return Protocol.Unknown;
                }
            }
        }

        public string Checksum
        {
            get
            {
                //в 16-ричном формате
                return string.Format("0x{0:x2}", _checksum);
            }
        }

        public IPAddress SourceAddress
        {
            get
            {
                return new IPAddress(_sourceIPAddress);
            }
        }

        public IPAddress DestinationAddress
        {
            get
            {
                return new IPAddress(_destinationIPAddress);
            }
        }


        public string TotalLength
        {
            get
            {
                return _totalLength.ToString();
            }
        }

        public string Identification
        {
            get
            {
                return _identification.ToString();
            }
        }

        public byte[] Data
        {
            get
            {
                return _IPData;
            }
        }
    }
}
