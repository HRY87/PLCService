using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ControlplastPLCService.Models;

namespace ControlplastPLCService
{
    public class ControlplastPLC : IDisposable
    {
        // Tipos de memoria
        private const int TIPO_DADOS = 0x8DFF;
        private const int TIPO_PARAMETROS = 0x08FF;
        
        // =======================
        // ENERGÍA / CONSUMO
        // =======================
        public const int ADDR_TENSION_L1 = 100;
        public const int ADDR_TENSION_L2 = 102;
        public const int ADDR_TENSION_L3 = 104;

        public const int ADDR_AMPERES_L1 = 112;
        public const int ADDR_AMPERES_L2 = 114;
        public const int ADDR_AMPERES_L3 = 116;

        public const int ADDR_CONSUMO_ACTUAL_KW = 130;
        public const int ADDR_KW_TOTAL = 136;
        public const int ADDR_KW_POR_KG = 140;
        public const int ADDR_KW_DIA = 154;

        // =======================
        // PRODUCCIÓN
        // =======================
        public const int ADDR_KG_HORA_PROGRAMADO = 517;
        public const int ADDR_ESPESOR_PROGRAMADO = 519;
        public const int ADDR_ANCHO_BRUTO_PROGRAMADO = 521;
        public const int ADDR_ANCHO_NETO_PROGRAMADO = 523;
        public const int ADDR_GRAMAJE_PROGRAMADO = 527;
        public const int ADDR_METROS_POR_MIN_PROGRAMADO = 531;
        
        public const int ADDR_KG_HORA_ACTUAL = 800;
        public const int ADDR_ESPESOR_ACTUAL = 802;
        public const int ADDR_ANCHO_BRUTO_ACTUAL = 804;
        public const int ADDR_GRAMAJE_ACTUAL = 806;
        public const int ADDR_ANCHO_NETO_ACTUAL = 808;
        public const int ADDR_METROS_POR_MIN_ACTUAL = 810;
        
        // ======================= 
        // ROSCA A – PROGRAMADO
        // =======================
        public const int ADDR_ROSCA_A_GRAMA_METRO_PROG = 533;
        public const int ADDR_ROSCA_A_ESPESOR_PROG = 537;
        public const int ADDR_ROSCA_A_PORCENTAJE_PROG = 539;
        public const int ADDR_ROSCA_A_KG_HORA_PROG = 541;
        public const int ADDR_ROSCA_A_SILO1_PROG = 543;
        public const int ADDR_ROSCA_A_SILO2_PROG = 545;
        public const int ADDR_ROSCA_A_SILO3_PROG = 547;
        public const int ADDR_ROSCA_A_SILO4_PROG = 549;
        public const int ADDR_ROSCA_A_SILO5_PROG = 551;
        public const int ADDR_ROSCA_A_SILO6_PROG = 553;
        
        public const int ADDR_ROSCA_A_GRAMA_METRO_ACTUAL = 812;
        public const int ADDR_ROSCA_A_ESPESOR_ACTUAL = 816;
        public const int ADDR_ROSCA_A_PORCENTAJE_ACTUAL = 818;
        public const int ADDR_ROSCA_A_KG_HORA_ACTUAL = 820;
        public const int ADDR_ROSCA_A_SILO1_ACTUAL = 822;
        public const int ADDR_ROSCA_A_SILO2_ACTUAL = 824;
        public const int ADDR_ROSCA_A_SILO3_ACTUAL = 826;
        public const int ADDR_ROSCA_A_SILO4_ACTUAL = 828;
        public const int ADDR_ROSCA_A_SILO5_ACTUAL = 830;
        public const int ADDR_ROSCA_A_SILO6_ACTUAL = 832;
        
        public const int ADDR_ROSCA_A_DENSIDAD_SILO1 = 555;
        public const int ADDR_ROSCA_A_DENSIDAD_SILO2 = 557;
        public const int ADDR_ROSCA_A_DENSIDAD_SILO3 = 559;
        public const int ADDR_ROSCA_A_DENSIDAD_SILO4 = 561;
        public const int ADDR_ROSCA_A_DENSIDAD_SILO5 = 563;
        public const int ADDR_ROSCA_A_DENSIDAD_SILO6 = 565;
        
        public const int ADDR_ROSCA_A_TOTAL_SILO1 = 967;
        public const int ADDR_ROSCA_A_TOTAL_SILO2 = 969;
        public const int ADDR_ROSCA_A_TOTAL_SILO3 = 971;
        public const int ADDR_ROSCA_A_TOTAL_SILO4 = 973;
        public const int ADDR_ROSCA_A_TOTAL_SILO5 = 975;
        public const int ADDR_ROSCA_A_TOTAL_SILO6 = 977;
        
        // =======================
        // ROSCA B – PROGRAMADO
        // =======================
        public const int ADDR_ROSCA_B_GRAMA_METRO_PROG = 567;
        public const int ADDR_ROSCA_B_ESPESOR_PROG = 571;
        public const int ADDR_ROSCA_B_PORCENTAJE_PROG = 573;
        public const int ADDR_ROSCA_B_KG_HORA_PROG = 575;
        public const int ADDR_ROSCA_B_SILO1_PROG = 577;
        public const int ADDR_ROSCA_B_SILO2_PROG = 579;
        public const int ADDR_ROSCA_B_SILO3_PROG = 581;
        public const int ADDR_ROSCA_B_SILO4_PROG = 583;
        public const int ADDR_ROSCA_B_SILO5_PROG = 585;
        public const int ADDR_ROSCA_B_SILO6_PROG = 587;
        public const int ADDR_ROSCA_B_GL_PROG = 589; // ⚠️ confirmar significado

        // =======================
        // ROSCA B – ACTUAL
        // =======================
        public const int ADDR_ROSCA_B_GRAMA_METRO_ACTUAL = 846;
        public const int ADDR_ROSCA_B_ESPESOR_ACTUAL = 850;
        public const int ADDR_ROSCA_B_PORCENTAJE_ACTUAL = 852;
        public const int ADDR_ROSCA_B_KG_HORA_ACTUAL = 854;
        public const int ADDR_ROSCA_B_SILO1_ACTUAL = 856;
        public const int ADDR_ROSCA_B_SILO2_ACTUAL = 858;
        public const int ADDR_ROSCA_B_SILO3_ACTUAL = 860;
        public const int ADDR_ROSCA_B_SILO4_ACTUAL = 862;
        public const int ADDR_ROSCA_B_SILO5_ACTUAL = 864;
        public const int ADDR_ROSCA_B_SILO6_ACTUAL = 866;

        // =======================
        // ROSCA B – DENSIDADES
        // =======================
        public const int ADDR_ROSCA_B_DENSIDAD_SILO1 = 589; // ⚠️ solapado con GL
        public const int ADDR_ROSCA_B_DENSIDAD_SILO2 = 591;
        public const int ADDR_ROSCA_B_DENSIDAD_SILO3 = 593;
        public const int ADDR_ROSCA_B_DENSIDAD_SILO4 = 595;
        public const int ADDR_ROSCA_B_DENSIDAD_SILO5 = 597;
        public const int ADDR_ROSCA_B_DENSIDAD_SILO6 = 599;

        // =======================
        // ROSCA B – TOTALIZADORES
        // =======================
        public const int ADDR_ROSCA_B_TOTAL_SILO1 = 967;
        public const int ADDR_ROSCA_B_TOTAL_SILO2 = 969;
        public const int ADDR_ROSCA_B_TOTAL_SILO3 = 971;
        public const int ADDR_ROSCA_B_TOTAL_SILO4 = 973;
        public const int ADDR_ROSCA_B_TOTAL_SILO5 = 975;
        public const int ADDR_ROSCA_B_TOTAL_SILO6 = 977;

        // =======================
        // ROSCA C – PROGRAMADO
        // =======================
        public const int ADDR_ROSCA_C_GRAMA_METRO_PROG = 601;
        public const int ADDR_ROSCA_C_ESPESOR_PROG = 605;
        public const int ADDR_ROSCA_C_PORCENTAJE_PROG = 607;
        public const int ADDR_ROSCA_C_KG_HORA_PROG = 609;
        public const int ADDR_ROSCA_C_SILO1_PROG = 611;
        public const int ADDR_ROSCA_C_SILO2_PROG = 613;
        public const int ADDR_ROSCA_C_SILO3_PROG = 615;
        public const int ADDR_ROSCA_C_SILO4_PROG = 617;
        public const int ADDR_ROSCA_C_SILO5_PROG = 619;
        public const int ADDR_ROSCA_C_SILO6_PROG = 621;
        public const int ADDR_ROSCA_C_GL_PROG = 623; // ⚠️ confirmar significado

        // =======================
        // ROSCA C – ACTUAL
        // =======================
        public const int ADDR_ROSCA_C_GRAMA_METRO_ACTUAL = 880;
        public const int ADDR_ROSCA_C_ESPESOR_ACTUAL = 884;
        public const int ADDR_ROSCA_C_PORCENTAJE_ACTUAL = 886;
        public const int ADDR_ROSCA_C_KG_HORA_ACTUAL = 888;
        public const int ADDR_ROSCA_C_SILO1_ACTUAL = 856; // ⚠️ solapado con Rosca B
        public const int ADDR_ROSCA_C_SILO2_ACTUAL = 858;
        public const int ADDR_ROSCA_C_SILO3_ACTUAL = 860;
        public const int ADDR_ROSCA_C_SILO4_ACTUAL = 862;
        public const int ADDR_ROSCA_C_SILO5_ACTUAL = 864;
        public const int ADDR_ROSCA_C_SILO6_ACTUAL = 866;

        // =======================
        // ROSCA C – DENSIDADES
        // =======================
        public const int ADDR_ROSCA_C_DENSIDAD_SILO1 = 623;
        public const int ADDR_ROSCA_C_DENSIDAD_SILO2 = 625;
        public const int ADDR_ROSCA_C_DENSIDAD_SILO3 = 627;
        public const int ADDR_ROSCA_C_DENSIDAD_SILO4 = 629;
        public const int ADDR_ROSCA_C_DENSIDAD_SILO5 = 631;
        public const int ADDR_ROSCA_C_DENSIDAD_SILO6 = 633;

        // =======================
        // ROSCA C – TOTALIZADORES
        // =======================
        public const int ADDR_ROSCA_C_TOTAL_SILO1 = 967; // ⚠️ compartido con Rosca A/B
        public const int ADDR_ROSCA_C_TOTAL_SILO2 = 969;
        public const int ADDR_ROSCA_C_TOTAL_SILO3 = 971;
        public const int ADDR_ROSCA_C_TOTAL_SILO4 = 973;
        public const int ADDR_ROSCA_C_TOTAL_SILO5 = 975;
        public const int ADDR_ROSCA_C_TOTAL_SILO6 = 977;

        // =======================
        // ROSCA D – PROGRAMADO
        // =======================
        public const int ADDR_ROSCA_D_GRAMA_METRO_PROG = 635;
        public const int ADDR_ROSCA_D_ESPESOR_PROG = 639;
        public const int ADDR_ROSCA_D_PORCENTAJE_PROG = 641;
        public const int ADDR_ROSCA_D_KG_HORA_PROG = 643;
        public const int ADDR_ROSCA_D_SILO1_PROG = 645;
        public const int ADDR_ROSCA_D_SILO2_PROG = 647;
        public const int ADDR_ROSCA_D_SILO3_PROG = 649;
        public const int ADDR_ROSCA_D_SILO4_PROG = 651;
        public const int ADDR_ROSCA_D_SILO5_PROG = 653;
        public const int ADDR_ROSCA_D_SILO6_PROG = 655;
        public const int ADDR_ROSCA_D_GL_PROG = 657; // ⚠️ confirmar significado

        // =======================
        // ROSCA D – ACTUAL
        // =======================
        public const int ADDR_ROSCA_D_GRAMA_METRO_ACTUAL = 1027;
        public const int ADDR_ROSCA_D_ESPESOR_ACTUAL = 1031;
        public const int ADDR_ROSCA_D_PORCENTAJE_ACTUAL = 1033;
        public const int ADDR_ROSCA_D_KG_HORA_ACTUAL = 1035;
        public const int ADDR_ROSCA_D_SILO1_ACTUAL = 1037;
        public const int ADDR_ROSCA_D_SILO2_ACTUAL = 1039;
        public const int ADDR_ROSCA_D_SILO3_ACTUAL = 1041;
        public const int ADDR_ROSCA_D_SILO4_ACTUAL = 1043;
        public const int ADDR_ROSCA_D_SILO5_ACTUAL = 1045;
        public const int ADDR_ROSCA_D_SILO6_ACTUAL = 1047;

        // =======================
        // ROSCA D – DENSIDADES
        // =======================
        public const int ADDR_ROSCA_D_DENSIDAD_SILO1 = 657;
        public const int ADDR_ROSCA_D_DENSIDAD_SILO2 = 659;
        public const int ADDR_ROSCA_D_DENSIDAD_SILO3 = 661;
        public const int ADDR_ROSCA_D_DENSIDAD_SILO4 = 663;
        public const int ADDR_ROSCA_D_DENSIDAD_SILO5 = 665;
        public const int ADDR_ROSCA_D_DENSIDAD_SILO6 = 667;

        // =======================
        // ROSCA D – TOTALIZADORES
        // =======================
        public const int ADDR_ROSCA_D_TOTAL_SILO1 = 1003;
        public const int ADDR_ROSCA_D_TOTAL_SILO2 = 1005;
        public const int ADDR_ROSCA_D_TOTAL_SILO3 = 1007;
        public const int ADDR_ROSCA_D_TOTAL_SILO4 = 1009;
        public const int ADDR_ROSCA_D_TOTAL_SILO5 = 1011;
        public const int ADDR_ROSCA_D_TOTAL_SILO6 = 1013;

        // =======================
        // ROSCA E – PROGRAMADO
        // =======================
        public const int ADDR_ROSCA_E_GRAMA_METRO_PROG = 669;
        public const int ADDR_ROSCA_E_ESPESOR_PROG = 673;
        public const int ADDR_ROSCA_E_PORCENTAJE_PROG = 675;
        public const int ADDR_ROSCA_E_KG_HORA_PROG = 677;
        public const int ADDR_ROSCA_E_SILO1_PROG = 679;
        public const int ADDR_ROSCA_E_SILO2_PROG = 681;
        public const int ADDR_ROSCA_E_SILO3_PROG = 683;
        public const int ADDR_ROSCA_E_SILO4_PROG = 685;
        public const int ADDR_ROSCA_E_SILO5_PROG = 687;
        public const int ADDR_ROSCA_E_SILO6_PROG = 689;
        public const int ADDR_ROSCA_E_GL_PROG = 691; // ⚠️ confirmar significado

        // =======================
        // ROSCA E – ACTUAL
        // =======================
        public const int ADDR_ROSCA_E_GRAMA_METRO_ACTUAL = 1061;
        public const int ADDR_ROSCA_E_ESPESOR_ACTUAL = 1065;
        public const int ADDR_ROSCA_E_PORCENTAJE_ACTUAL = 1067;
        public const int ADDR_ROSCA_E_KG_HORA_ACTUAL = 1069;
        public const int ADDR_ROSCA_E_SILO1_ACTUAL = 1071;
        public const int ADDR_ROSCA_E_SILO2_ACTUAL = 1073;
        public const int ADDR_ROSCA_E_SILO3_ACTUAL = 1075;
        public const int ADDR_ROSCA_E_SILO4_ACTUAL = 1077;
        public const int ADDR_ROSCA_E_SILO5_ACTUAL = 1079;
        public const int ADDR_ROSCA_E_SILO6_ACTUAL = 1081;

        // =======================
        // ROSCA E – DENSIDADES
        // =======================
        public const int ADDR_ROSCA_E_DENSIDAD_SILO1 = 691;
        public const int ADDR_ROSCA_E_DENSIDAD_SILO2 = 693;
        public const int ADDR_ROSCA_E_DENSIDAD_SILO3 = 695;
        public const int ADDR_ROSCA_E_DENSIDAD_SILO4 = 697;
        public const int ADDR_ROSCA_E_DENSIDAD_SILO5 = 699;
        public const int ADDR_ROSCA_E_DENSIDAD_SILO6 = 701;

        // =======================
        // ROSCA E – TOTALIZADORES
        // =======================
        public const int ADDR_ROSCA_E_TOTAL_SILO1 = 1015;
        public const int ADDR_ROSCA_E_TOTAL_SILO2 = 1017;
        public const int ADDR_ROSCA_E_TOTAL_SILO3 = 1019;
        public const int ADDR_ROSCA_E_TOTAL_SILO4 = 1021;
        public const int ADDR_ROSCA_E_TOTAL_SILO5 = 1023;
        public const int ADDR_ROSCA_E_TOTAL_SILO6 = 1025;

        // =======================
        // OP / PRODUCCIÓN GENERAL
        // =======================
        public const int ADDR_NUMERO_OP = 30000;          // String (16)
        public const int ADDR_ESTADO_OP = 30023;          // Word (entero corto)
        public const int ADDR_KG_POR_METRO_OP = 30017;
        public const int ADDR_TAMANO_BOBINA_OP = 30019;
        public const int ADDR_KG_PRODUCIDOS = 30037;
        public const int ADDR_METROS_PRODUCIDOS = 30053;
        public const int ADDR_CONSUMO_TOTAL_OP = 30059;
        public const int ADDR_RECORTES_OP = 30061;          // Float (confirmar)
        
        private readonly string _ip;
        private readonly int _port;
        private readonly int _timeout;
        private TcpClient? _tcpClient;
        private NetworkStream? _stream;
        private bool _disposed;
        
        public bool Connected { get; private set; }
        
        public ControlplastPLC(string ip, int port = 8000, int timeout = 3000)
        {
            _ip = ip;
            _port = port;
            _timeout = timeout;
        }
        
        public async Task<bool> ConnectAsync()
        {
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.SendTimeout = _timeout;
                _tcpClient.ReceiveTimeout = _timeout;
                
                var connectTask = _tcpClient.ConnectAsync(_ip, _port);
                var timeoutTask = Task.Delay(_timeout);
                
                var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                
                if (completedTask == timeoutTask)
                {
                    throw new TimeoutException($"Timeout al conectar con {_ip}:{_port}");
                }
                
                await connectTask;
                
                _stream = _tcpClient.GetStream();
                Connected = true;
                
                return true;
            }
            catch (Exception)
            {
                Connected = false;
                Disconnect();
                return false;
            }
        }
        
        public void Disconnect()
        {
            try
            {
                _stream?.Close();
                _tcpClient?.Close();
                Connected = false;
            }
            catch { }
        }
        
        private byte[] BuildRequest(int address, int numWords, int tipo = TIPO_DADOS)
        {
            byte[] header = new byte[38];
            
            header[0] = 0x01; header[1] = 0x60; header[2] = 0x00; header[3] = 0x00;
            header[4] = 0xFF; header[5] = 0x00; header[6] = 0x00; header[7] = 0x00;
            header[8] = 0x00; header[9] = 0x00; header[10] = 0x08; header[11] = 0x00;
            header[12] = 0x0C; header[13] = 0x00; header[14] = 0x69; header[15] = 0x01;
            header[16] = 0x00; header[17] = 0x00; header[18] = 0x01; header[19] = 0x00;
            header[20] = 0x00; header[21] = 0x00; header[22] = 0x00; header[23] = 0x00;
            header[24] = 0x00; header[25] = 0x00; header[26] = 0x00; header[27] = 0x00;
            
            if (tipo == TIPO_DADOS)
            {
                header[28] = 0x8D;
                header[29] = 0xFF;
            }
            else
            {
                header[28] = 0x08;
                header[29] = 0xFF;
            }
            
            header[30] = (byte)(address & 0xFF);
            header[31] = (byte)((address >> 8) & 0xFF);
            header[32] = (byte)((address >> 16) & 0xFF);
            header[33] = 0x00;
            
            header[34] = (byte)(numWords & 0xFF);
            header[35] = (byte)((numWords >> 8) & 0xFF);
            header[36] = 0x00;
            header[37] = 0x00;
            
            return header;
        }
        
        public async Task<int[]?> ReadWordsAsync(int address, int numWords, int tipo = TIPO_DADOS)
        {
            if (!Connected || _stream == null)
                return null;
            
            try
            {
                byte[] request = BuildRequest(address, numWords, tipo);
                await _stream.WriteAsync(request.AsMemory());
                
                byte[] buffer = new byte[8192];
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                
                if (bytesRead < 33)
                    return null;
                
                int[] words = new int[numWords];
                for (int i = 0; i < numWords; i++)
                {
                    int offset = 33 + (i * 2);
                    if (offset + 1 < bytesRead)
                    {
                        words[i] = buffer[offset] + (buffer[offset + 1] * 256);
                    }
                    else
                    {
                        break;
                    }
                }
                
                return words;
            }
            catch
            {
                return null;
            }
        }
        
        public async Task<int?> ReadWordAsync(int address)
        {
            var words = await ReadWordsAsync(address, 1);
            return words?[0];
        }
        
        public async Task<float?> ReadFloatAsync(int address)
        {
            var words = await ReadWordsAsync(address, 2);
            if (words != null && words.Length == 2)
            {
                try
                {
                    byte[] bytes = new byte[4];
                    bytes[0] = (byte)(words[0] & 0xFF);
                    bytes[1] = (byte)((words[0] >> 8) & 0xFF);
                    bytes[2] = (byte)(words[1] & 0xFF);
                    bytes[3] = (byte)((words[1] >> 8) & 0xFF);
                    
                    return BitConverter.ToSingle(bytes, 0);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
        
        public async Task<string?> ReadStringAsync(int address, int numChars)
        {
            int numWords = (numChars + 1) / 2;
            var words = await ReadWordsAsync(address, numWords);
            
            if (words == null) return null;
            
            StringBuilder result = new StringBuilder();
            foreach (var word in words)
            {
                char char1 = (char)(word & 0xFF);
                char char2 = (char)((word >> 8) & 0xFF);
                result.Append(char1);
                result.Append(char2);
            }
            
            return result.ToString().Replace("\0", "").Trim();
        }
        
        public async Task<DatosProduccion> GetDatosProduccionAsync()
        {
            var data = new DatosProduccion();
            
            if (!Connected)
                return data;
            
            try
            {
                // Producción Programada
                data.KgHoraProgramado = await ReadFloatAsync(ADDR_KG_HORA_PROGRAMADO) ?? 0;
                data.EspesorProgramado = await ReadFloatAsync(ADDR_ESPESOR_PROGRAMADO) ?? 0;
                data.AnchoBrutoProgramado = await ReadFloatAsync(ADDR_ANCHO_BRUTO_PROGRAMADO) ?? 0;
                data.AnchoNetoProgramado = await ReadFloatAsync(ADDR_ANCHO_NETO_PROGRAMADO) ?? 0;
                data.GramajeProgramado = await ReadFloatAsync(ADDR_GRAMAJE_PROGRAMADO) ?? 0;
                data.MetrosPorMinProgramado = await ReadFloatAsync(ADDR_METROS_POR_MIN_PROGRAMADO) ?? 0;
                
                // Rosca A - Programado
                data.RoscaA_GramaMetroProgramado = await ReadFloatAsync(ADDR_ROSCA_A_GRAMA_METRO_PROG) ?? 0;
                data.RoscaA_EspesorProgramado = await ReadFloatAsync(ADDR_ROSCA_A_ESPESOR_PROG) ?? 0;
                data.RoscaA_PorcentajeProgramado = await ReadFloatAsync(ADDR_ROSCA_A_PORCENTAJE_PROG) ?? 0;
                data.RoscaA_KgHoraProgramado = await ReadFloatAsync(ADDR_ROSCA_A_KG_HORA_PROG) ?? 0;
                data.RoscaA_Silo1Programado = await ReadFloatAsync(ADDR_ROSCA_A_SILO1_PROG) ?? 0;
                data.RoscaA_Silo2Programado = await ReadFloatAsync(ADDR_ROSCA_A_SILO2_PROG) ?? 0;
                data.RoscaA_Silo3Programado = await ReadFloatAsync(ADDR_ROSCA_A_SILO3_PROG) ?? 0;
                data.RoscaA_Silo4Programado = await ReadFloatAsync(ADDR_ROSCA_A_SILO4_PROG) ?? 0;
                data.RoscaA_Silo5Programado = await ReadFloatAsync(ADDR_ROSCA_A_SILO5_PROG) ?? 0;
                data.RoscaA_Silo6Programado = await ReadFloatAsync(ADDR_ROSCA_A_SILO6_PROG) ?? 0;
                
                // Producción Actual
                data.KgHoraActual = await ReadFloatAsync(ADDR_KG_HORA_ACTUAL) ?? 0;
                data.EspesorActual = await ReadFloatAsync(ADDR_ESPESOR_ACTUAL) ?? 0;
                data.AnchoBrutoActual = await ReadFloatAsync(ADDR_ANCHO_BRUTO_ACTUAL) ?? 0;
                data.GramajeActual = await ReadFloatAsync(ADDR_GRAMAJE_ACTUAL) ?? 0;
                data.AnchoNetoActual = await ReadFloatAsync(ADDR_ANCHO_NETO_ACTUAL) ?? 0;
                data.MetrosPorMinActual = await ReadFloatAsync(ADDR_METROS_POR_MIN_ACTUAL) ?? 0;
                
                // Rosca A - Actual
                data.RoscaA_GramaMetroActual = await ReadFloatAsync(ADDR_ROSCA_A_GRAMA_METRO_ACTUAL) ?? 0;
                data.RoscaA_EspesorActual = await ReadFloatAsync(ADDR_ROSCA_A_ESPESOR_ACTUAL) ?? 0;
                data.RoscaA_KgHoraActual = await ReadFloatAsync(ADDR_ROSCA_A_KG_HORA_ACTUAL) ?? 0;
                data.RoscaA_PorcentajeActual = await ReadFloatAsync(ADDR_ROSCA_A_PORCENTAJE_ACTUAL) ?? 0;
                data.RoscaA_Silo1Actual = await ReadFloatAsync(ADDR_ROSCA_A_SILO1_ACTUAL) ?? 0;
                data.RoscaA_Silo2Actual = await ReadFloatAsync(ADDR_ROSCA_A_SILO2_ACTUAL) ?? 0;
                data.RoscaA_Silo3Actual = await ReadFloatAsync(ADDR_ROSCA_A_SILO3_ACTUAL) ?? 0;
                data.RoscaA_Silo4Actual = await ReadFloatAsync(ADDR_ROSCA_A_SILO4_ACTUAL) ?? 0;
                data.RoscaA_Silo5Actual = await ReadFloatAsync(ADDR_ROSCA_A_SILO5_ACTUAL) ?? 0;
                data.RoscaA_Silo6Actual = await ReadFloatAsync(ADDR_ROSCA_A_SILO6_ACTUAL) ?? 0;
                
                // Consumo
                data.AmperesL1 = await ReadFloatAsync(ADDR_AMPERES_L1) ?? 0;
                data.ConsumoActualKW = await ReadFloatAsync(ADDR_CONSUMO_ACTUAL_KW) ?? 0;
                
                // OP / Producción
                data.NumeroOP = await ReadStringAsync(ADDR_NUMERO_OP, 16) ?? "N/A";
                data.KgPorMetroOP = await ReadFloatAsync(ADDR_KG_POR_METRO_OP) ?? 0;
                data.TamanoBobinaOP = await ReadFloatAsync(ADDR_TAMANO_BOBINA_OP) ?? 0;
                //data.RecortesOP = await ReadFloatAsync(ADDR_RECORTES_OP) ?? 0;
                data.EstadoOP = await ReadWordAsync(ADDR_ESTADO_OP) ?? 0;
                data.KgProducidos = await ReadFloatAsync(ADDR_KG_PRODUCIDOS) ?? 0;
                data.MetrosProducidos = await ReadFloatAsync(ADDR_METROS_PRODUCIDOS) ?? 0;
                data.ConsumoTotalOP = await ReadFloatAsync(ADDR_CONSUMO_TOTAL_OP) ?? 0;
                
                // Totalizadores
                data.RoscaA_TotalSilo1 = await ReadFloatAsync(ADDR_ROSCA_A_TOTAL_SILO1) ?? 0;
                data.RoscaA_TotalSilo2 = await ReadFloatAsync(ADDR_ROSCA_A_TOTAL_SILO2) ?? 0;
                data.RoscaA_TotalSilo3 = await ReadFloatAsync(ADDR_ROSCA_A_TOTAL_SILO3) ?? 0;
                data.RoscaA_TotalSilo4 = await ReadFloatAsync(ADDR_ROSCA_A_TOTAL_SILO4) ?? 0;
                data.RoscaA_TotalSilo5 = await ReadFloatAsync(ADDR_ROSCA_A_TOTAL_SILO5) ?? 0;
                data.RoscaA_TotalSilo6 = await ReadFloatAsync(ADDR_ROSCA_A_TOTAL_SILO6) ?? 0;
                
                // Densidades
                data.RoscaA_DensidadSilo1 = await ReadFloatAsync(ADDR_ROSCA_A_DENSIDAD_SILO1) ?? 0;
                data.RoscaA_DensidadSilo2 = await ReadFloatAsync(ADDR_ROSCA_A_DENSIDAD_SILO2) ?? 0;
                data.RoscaA_DensidadSilo3 = await ReadFloatAsync(ADDR_ROSCA_A_DENSIDAD_SILO3) ?? 0;
                data.RoscaA_DensidadSilo4 = await ReadFloatAsync(ADDR_ROSCA_A_DENSIDAD_SILO4) ?? 0;
                data.RoscaA_DensidadSilo5 = await ReadFloatAsync(ADDR_ROSCA_A_DENSIDAD_SILO5) ?? 0;
                data.RoscaA_DensidadSilo6 = await ReadFloatAsync(ADDR_ROSCA_A_DENSIDAD_SILO6) ?? 0;
            }
            catch { }
            
            return data;
        }
        
        public void Dispose()
        {
            if (_disposed) return;
            
            Disconnect();
            _disposed = true;
        }
    }
}